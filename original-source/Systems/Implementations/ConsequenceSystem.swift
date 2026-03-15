import Foundation

/// Handles random events during event execution.
/// Stage-based frequency from 20% (Stage 1) to 80% (Stage 5).
class ConsequenceSystem: ConsequenceSystemProtocol {

    // MARK: - Stage Event Frequency

    private static let stageEventFrequency: [Int: Double] = [
        1: 0.20,  // Stage 1 = 20% chance
        2: 0.35,  // Stage 2 = 35% chance
        3: 0.50,  // Stage 3 = 50% chance
        4: 0.65,  // Stage 4 = 65% chance
        5: 0.80   // Stage 5 = 80% chance
    ]

    // MARK: - Base Satisfaction Impacts

    private static let baseImpacts: [RandomEventType: Double] = [
        // Vendor Issues
        .vendorNoShow:            -25,
        .vendorLate:              -10,
        .vendorUnderperformance:  -15,
        // Equipment Issues
        .equipmentFailure:        -15,
        .powerOutage:             -20,
        .avMalfunction:           -12,
        // Guest Issues
        .guestConflict:            -8,
        .unexpectedGuests:         -5,
        .guestInjury:             -18,
        // Weather
        .weatherChange:           -10,
        .extremeWeather:          -25,
        // Client Issues
        .lastMinuteChanges:       -12,
        .clientComplaint:          -8,
        .budgetDispute:           -10,
        // Positive Events
        .unexpectedCompliment:      5,
        .mediaCoverage:             8,
        .celebrityAppearance:      10
    ]

    // MARK: - Mitigation Cost Multipliers

    private static let mitigationCostMultiplier: [RandomEventType: Double] = [
        .vendorNoShow:           150,
        .vendorLate:              50,
        .vendorUnderperformance:  75,
        .equipmentFailure:       100,
        .powerOutage:            120,
        .avMalfunction:           80,
        .guestConflict:           30,
        .unexpectedGuests:        40,
        .guestInjury:            100,
        .weatherChange:           80,
        .extremeWeather:         200,
        .lastMinuteChanges:      100,
        .clientComplaint:         50,
        .budgetDispute:           75
    ]

    // MARK: - Protocol Conformance

    func evaluateRandomEvents(event: EventData, stage: Int) -> [RandomEventResult] {
        var results: [RandomEventResult] = []
        let frequency = getRandomEventFrequency(stage: stage)

        // Check if a random event occurs based on stage frequency
        guard Double.random(in: 0..<1) < frequency else { return results }

        // Determine which type of random event occurs
        var possibleEvents = getPossibleEvents(for: event)
        guard !possibleEvents.isEmpty else { return results }

        // Select a random event type
        let selectedType = possibleEvents.randomElement()!
        results.append(createRandomEvent(type: selectedType, event: event))

        // Small chance of multiple events at higher stages
        if stage >= 3 && Double.random(in: 0..<1) < 0.15 {
            possibleEvents.removeAll { $0 == selectedType }
            if let secondType = possibleEvents.randomElement() {
                results.append(createRandomEvent(type: secondType, event: event))
            }
        }

        return results
    }

    func calculateRandomEventModifier(results: [RandomEventResult]) -> Double {
        results.reduce(0) { $0 + $1.getFinalImpact() }
    }

    func checkMitigation(event: RandomEventResult, availableBudget: Double) -> MitigationResult {
        // Positive events don't need mitigation
        if event.baseSatisfactionImpact >= 0 {
            return MitigationResult(
                canMitigate: false,
                requiredBudget: event.mitigationCost,
                availableBudget: availableBudget,
                mitigationOption: "No mitigation needed for positive events.",
                reducedImpact: event.baseSatisfactionImpact
            )
        }

        // Check if event can be mitigated
        guard event.canBeMitigated else {
            return MitigationResult(
                canMitigate: false,
                requiredBudget: event.mitigationCost,
                availableBudget: availableBudget,
                mitigationOption: "This event cannot be mitigated.",
                reducedImpact: event.baseSatisfactionImpact
            )
        }

        // Check if we have enough contingency budget
        if availableBudget >= event.mitigationCost {
            return MitigationResult(
                canMitigate: true,
                requiredBudget: event.mitigationCost,
                availableBudget: availableBudget,
                mitigationOption: event.mitigationDescription,
                reducedImpact: event.baseSatisfactionImpact * 0.25
            )
        } else {
            return MitigationResult(
                canMitigate: false,
                requiredBudget: event.mitigationCost,
                availableBudget: availableBudget,
                mitigationOption: "Insufficient contingency budget. Need $\(Int(event.mitigationCost)), have $\(Int(availableBudget)).",
                reducedImpact: event.baseSatisfactionImpact
            )
        }
    }

    func getRandomEventFrequency(stage: Int) -> Double {
        if let freq = Self.stageEventFrequency[stage] { return freq }
        return stage < 1 ? 0.20 : 0.80
    }

    // MARK: - Private Helpers

    private func getPossibleEvents(for event: EventData) -> [RandomEventType] {
        var possible: [RandomEventType] = [
            // Always possible vendor issues
            .vendorLate, .vendorUnderperformance,
            // Equipment issues
            .equipmentFailure, .avMalfunction,
            // Guest issues
            .guestConflict, .unexpectedGuests,
            // Client issues
            .lastMinuteChanges, .clientComplaint
        ]

        // Vendor no-show is less common but possible
        if Double.random(in: 0..<1) < 0.3 { possible.append(.vendorNoShow) }
        // Power outage is rare
        if Double.random(in: 0..<1) < 0.2 { possible.append(.powerOutage) }
        // Guest injury is rare
        if Double.random(in: 0..<1) < 0.1 { possible.append(.guestInjury) }

        // Budget dispute based on personality
        if event.personality == .budgetConscious || event.personality == .demanding {
            possible.append(.budgetDispute)
        }

        // Positive events are rare
        if Double.random(in: 0..<1) < 0.10 { possible.append(.unexpectedCompliment) }
        if Double.random(in: 0..<1) < 0.05 { possible.append(.mediaCoverage) }

        return possible
    }

    private func createRandomEvent(type: RandomEventType, event: EventData) -> RandomEventResult {
        let baseImpact = Self.baseImpacts[type] ?? -10
        let mitigationCost = calculateMitigationCost(type: type, event: event)
        let canMitigate = baseImpact < 0 && Self.mitigationCostMultiplier[type] != nil

        return RandomEventResult(
            eventType: type,
            eventDescription: getEventDescription(type),
            baseSatisfactionImpact: baseImpact,
            mitigationCost: mitigationCost,
            canBeMitigated: canMitigate,
            wasMitigated: false,
            mitigationDescription: getMitigationDescription(type),
            failureDescription: getFailureDescription(type)
        )
    }

    private func calculateMitigationCost(type: RandomEventType, event: EventData) -> Double {
        guard let multiplier = Self.mitigationCostMultiplier[type] else { return 0 }
        let baseCost = event.budget.total * 0.05  // 5% of total budget
        return baseCost * (multiplier / 100.0)
    }

    // MARK: - Event Descriptions

    private func getEventDescription(_ type: RandomEventType) -> String {
        switch type {
        case .vendorNoShow:           return "A vendor failed to show up for the event!"
        case .vendorLate:             return "A vendor arrived late, causing delays."
        case .vendorUnderperformance: return "A vendor's service quality was below expectations."
        case .equipmentFailure:       return "Equipment malfunctioned during the event."
        case .powerOutage:            return "A power outage disrupted the event."
        case .avMalfunction:          return "Audio/visual equipment experienced issues."
        case .guestConflict:          return "A conflict arose between guests."
        case .unexpectedGuests:       return "More guests arrived than expected."
        case .guestInjury:            return "A guest was injured during the event."
        case .weatherChange:          return "Weather conditions changed unexpectedly."
        case .extremeWeather:         return "Extreme weather severely impacted the event."
        case .lastMinuteChanges:      return "The client requested last-minute changes."
        case .clientComplaint:        return "The client raised concerns during the event."
        case .budgetDispute:          return "A budget disagreement arose with the client."
        case .unexpectedCompliment:   return "Guests gave unexpected praise for the event!"
        case .mediaCoverage:          return "The event received positive media attention!"
        case .celebrityAppearance:    return "A celebrity made a surprise appearance!"
        }
    }

    private func getMitigationDescription(_ type: RandomEventType) -> String {
        switch type {
        case .vendorNoShow:           return "Emergency backup vendor was called in."
        case .vendorLate:             return "Adjusted schedule to accommodate the delay."
        case .vendorUnderperformance: return "Provided additional support to improve service."
        case .equipmentFailure:       return "Replacement equipment was quickly sourced."
        case .powerOutage:            return "Backup generators were activated."
        case .avMalfunction:          return "Technical support resolved the issue."
        case .guestConflict:          return "Staff diplomatically resolved the situation."
        case .unexpectedGuests:       return "Additional catering and seating arranged."
        case .guestInjury:            return "First aid provided and situation handled professionally."
        case .weatherChange:          return "Contingency plans were activated."
        case .extremeWeather:         return "Event was moved to backup location."
        case .lastMinuteChanges:      return "Team adapted quickly to new requirements."
        case .clientComplaint:        return "Concerns were addressed promptly."
        case .budgetDispute:          return "Transparent cost breakdown provided."
        default:                      return "The situation was handled professionally."
        }
    }

    private func getFailureDescription(_ type: RandomEventType) -> String {
        switch type {
        case .vendorNoShow:           return "No backup was available, leaving a gap in services."
        case .vendorLate:             return "The delay caused noticeable disruption."
        case .vendorUnderperformance: return "Service quality disappointed guests."
        case .equipmentFailure:       return "The malfunction impacted the event experience."
        case .powerOutage:            return "The outage significantly disrupted activities."
        case .avMalfunction:          return "Technical issues affected presentations."
        case .guestConflict:          return "The conflict created an uncomfortable atmosphere."
        case .unexpectedGuests:       return "Resources were stretched thin."
        case .guestInjury:            return "The incident overshadowed the event."
        case .weatherChange:          return "Weather conditions affected outdoor activities."
        case .extremeWeather:         return "Severe weather ruined outdoor elements."
        case .lastMinuteChanges:      return "Changes couldn't be fully accommodated."
        case .clientComplaint:        return "Client concerns went unresolved."
        case .budgetDispute:          return "Financial disagreement soured the relationship."
        default:                      return "The situation was not handled well."
        }
    }
}
