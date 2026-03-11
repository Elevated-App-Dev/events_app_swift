import Foundation

/// Result of event phase calculation containing phase info and timing.
struct EventPhaseInfo: Codable, Equatable {
    var phase: EventPhase
    var stressWeight: Double
    var daysRemainingInPhase: Int
    var totalDaysInPhase: Int
    var daysUntilEvent: Int

    var isHighStressPhase: Bool { phase == .finalPrep || phase == .executionDay }
}

/// Phase schedule boundaries for an event.
struct EventPhaseSchedule: Codable, Equatable {
    var acceptedDate: GameDate
    var eventDate: GameDate
    var bookingEndDate: GameDate
    var prePlanningEndDate: GameDate
    var activePlanningEndDate: GameDate
    var finalPrepEndDate: GameDate
    var bookingDays: Int
    var prePlanningDays: Int
    var activePlanningDays: Int
    var finalPrepDays: Int
}

/// Calculator for event phases and stress weights.
/// Stress weights: Booking 0.5x, PrePlanning 0.75x, ActivePlanning 1.0x,
/// FinalPrep 1.5x, ExecutionDay 2.0x, Results 0.0x.
enum EventPhaseCalculator {

    // Stress weights by phase
    private static let stressWeights: [Double] = [
        0.5,   // Booking
        0.75,  // PrePlanning
        1.0,   // ActivePlanning
        1.5,   // FinalPrep
        2.0,   // ExecutionDay
        0.0    // Results
    ]

    // Phase time allocation constants
    private static let bookingDaysConst = 2
    private static let prePlanningPercent = 0.25
    private static let activePlanningPercent = 0.50
    private static let finalPrepPercent = 0.20

    static func getStressWeight(_ phase: EventPhase) -> Double {
        let index: Int
        switch phase {
        case .booking:        index = 0
        case .prePlanning:    index = 1
        case .activePlanning: index = 2
        case .finalPrep:      index = 3
        case .executionDay:   index = 4
        case .results:        index = 5
        }
        guard index >= 0, index < stressWeights.count else { return 1.0 }
        return stressWeights[index]
    }

    static func calculatePhaseSchedule(acceptedDate: GameDate, eventDate: GameDate) -> EventPhaseSchedule {
        let totalDays = acceptedDate.daysBetween(eventDate)

        // Very short event - compress phases
        if totalDays < 3 {
            return EventPhaseSchedule(
                acceptedDate: acceptedDate,
                eventDate: eventDate,
                bookingEndDate: acceptedDate.adding(days: min(1, totalDays - 1)),
                prePlanningEndDate: acceptedDate.adding(days: min(1, totalDays - 1)),
                activePlanningEndDate: acceptedDate.adding(days: max(1, totalDays - 1)),
                finalPrepEndDate: eventDate.adding(days: -1),
                bookingDays: min(1, totalDays - 1),
                prePlanningDays: 0,
                activePlanningDays: max(0, totalDays - 2),
                finalPrepDays: 1
            )
        }

        var bookingDays = min(bookingDaysConst, totalDays - 1)
        let remainingAfterBooking = totalDays - bookingDays - 1 // -1 for execution day

        var prePlanningDays = max(1, Int(Double(remainingAfterBooking) * prePlanningPercent))
        var activePlanningDays = max(1, Int(Double(remainingAfterBooking) * activePlanningPercent))
        var finalPrepDays = max(1, Int(Double(remainingAfterBooking) * finalPrepPercent))

        // Adjust to ensure we don't exceed total days
        var allocatedDays = bookingDays + prePlanningDays + activePlanningDays + finalPrepDays + 1

        if allocatedDays > totalDays {
            var excess = allocatedDays - totalDays
            let reduceActive = min(excess, activePlanningDays - 1)
            activePlanningDays -= reduceActive
            excess -= reduceActive

            if excess > 0 {
                let reducePrePlanning = min(excess, prePlanningDays - 1)
                prePlanningDays -= reducePrePlanning
                excess -= reducePrePlanning
            }
            if excess > 0 {
                let reduceFinalPrep = min(excess, finalPrepDays - 1)
                finalPrepDays -= reduceFinalPrep
            }
        } else if allocatedDays < totalDays {
            activePlanningDays += totalDays - allocatedDays
        }

        let bookingEndDate = acceptedDate.adding(days: bookingDays)
        let prePlanningEndDate = bookingEndDate.adding(days: prePlanningDays)
        let activePlanningEndDate = prePlanningEndDate.adding(days: activePlanningDays)
        let finalPrepEndDate = activePlanningEndDate.adding(days: finalPrepDays)

        return EventPhaseSchedule(
            acceptedDate: acceptedDate,
            eventDate: eventDate,
            bookingEndDate: bookingEndDate,
            prePlanningEndDate: prePlanningEndDate,
            activePlanningEndDate: activePlanningEndDate,
            finalPrepEndDate: finalPrepEndDate,
            bookingDays: bookingDays,
            prePlanningDays: prePlanningDays,
            activePlanningDays: activePlanningDays,
            finalPrepDays: finalPrepDays
        )
    }

    static func getCurrentPhase(currentDate: GameDate, schedule: EventPhaseSchedule) -> EventPhaseInfo {
        let daysUntilEvent = currentDate.daysBetween(schedule.eventDate)

        // Results phase
        if currentDate > schedule.eventDate {
            return EventPhaseInfo(phase: .results, stressWeight: getStressWeight(.results),
                                  daysRemainingInPhase: 0, totalDaysInPhase: 0, daysUntilEvent: daysUntilEvent)
        }

        // Execution Day
        if currentDate == schedule.eventDate {
            return EventPhaseInfo(phase: .executionDay, stressWeight: getStressWeight(.executionDay),
                                  daysRemainingInPhase: 0, totalDaysInPhase: 1, daysUntilEvent: 0)
        }

        // Final Prep
        if currentDate > schedule.activePlanningEndDate {
            let total = schedule.activePlanningEndDate.daysBetween(schedule.eventDate)
            let remaining = currentDate.daysBetween(schedule.eventDate)
            return EventPhaseInfo(phase: .finalPrep, stressWeight: getStressWeight(.finalPrep),
                                  daysRemainingInPhase: remaining, totalDaysInPhase: total, daysUntilEvent: daysUntilEvent)
        }

        // Active Planning
        if currentDate > schedule.prePlanningEndDate {
            let total = schedule.prePlanningEndDate.daysBetween(schedule.activePlanningEndDate)
            let remaining = currentDate.daysBetween(schedule.activePlanningEndDate)
            return EventPhaseInfo(phase: .activePlanning, stressWeight: getStressWeight(.activePlanning),
                                  daysRemainingInPhase: remaining, totalDaysInPhase: total, daysUntilEvent: daysUntilEvent)
        }

        // Pre-Planning
        if currentDate > schedule.bookingEndDate {
            let total = schedule.bookingEndDate.daysBetween(schedule.prePlanningEndDate)
            let remaining = currentDate.daysBetween(schedule.prePlanningEndDate)
            return EventPhaseInfo(phase: .prePlanning, stressWeight: getStressWeight(.prePlanning),
                                  daysRemainingInPhase: remaining, totalDaysInPhase: total, daysUntilEvent: daysUntilEvent)
        }

        // Booking
        let bookingTotal = schedule.acceptedDate.daysBetween(schedule.bookingEndDate)
        let bookingRemaining = currentDate.daysBetween(schedule.bookingEndDate)
        return EventPhaseInfo(phase: .booking, stressWeight: getStressWeight(.booking),
                              daysRemainingInPhase: bookingRemaining, totalDaysInPhase: bookingTotal, daysUntilEvent: daysUntilEvent)
    }

    /// 10% per overlapping event beyond the first.
    static func calculateOverlappingStressPenalty(highStressEventCount: Int) -> Double {
        guard highStressEventCount > 1 else { return 0 }
        return Double(highStressEventCount - 1) * 0.10
    }

    /// Base penalty multiplied by phase stress weight.
    static func calculateEffectiveWorkloadPenalty(basePenalty: Double, phase: EventPhase) -> Double {
        basePenalty * getStressWeight(phase)
    }
}
