import Foundation

/// Turn-based advancement engine. Replaces the real-time timer loop.
/// Maintains a list of scheduled planning activities and finds the next
/// date that requires player attention. Time only moves when the player
/// chooses to advance.
struct AdvanceSystem: AdvanceSystemProtocol {

    // MARK: - Inquiry Intervals (game-days between inquiries, by stage)

    private static let inquiryDayRanges: [(min: Int, max: Int)] = [
        (3, 5),   // Stage 1
        (2, 4),   // Stage 2
        (2, 3),   // Stage 3
        (1, 2),   // Stage 4
        (1, 2)    // Stage 5
    ]

    private static let stageMinReputation = [0, 25, 50, 100, 200]

    // MARK: - State

    private(set) var currentDate: GameDate
    private(set) var scheduledActivities: [PlanningActivity]
    private(set) var nextScheduledInquiryDate: GameDate?

    // MARK: - Init

    init(startDate: GameDate = GameDate(month: 3, day: 1, year: 2026)) {
        self.currentDate = startDate
        self.scheduledActivities = []
        self.nextScheduledInquiryDate = nil
    }

    // MARK: - Find Next Decision Point

    func findNextDecisionPoint() -> DecisionPoint? {
        // Collect all future dates that have scheduled or ready activities
        var candidateDates: Set<GameDate> = []

        for activity in scheduledActivities {
            // Only look at scheduled (future) activities, not ready (already arrived) ones
            guard activity.status == .scheduled else { continue }
            if activity.scheduledDate > currentDate {
                candidateDates.insert(activity.scheduledDate)
            }
        }

        // Include the next inquiry date if it's in the future
        if let inquiryDate = nextScheduledInquiryDate, inquiryDate > currentDate {
            candidateDates.insert(inquiryDate)
        }

        guard let nextDate = candidateDates.min() else { return nil }

        // Gather all activities for that date
        let activitiesForDate = scheduledActivities.filter {
            $0.scheduledDate == nextDate && $0.status == .scheduled
        }

        return DecisionPoint(date: nextDate, activities: activitiesForDate)
    }

    // MARK: - Advance

    mutating func advance(to point: DecisionPoint) -> [PlanningActivity] {
        var overdueActivities: [PlanningActivity] = []

        for i in scheduledActivities.indices {
            let activity = scheduledActivities[i]
            guard activity.status == .scheduled else { continue }

            // Activities on or before the target date become ready
            if activity.scheduledDate <= point.date {
                scheduledActivities[i].status = .ready
            }

            // Check for activities that became overdue during the jump
            if let deadline = activity.responseDeadline,
               deadline < point.date,
               scheduledActivities[i].status == .ready {
                scheduledActivities[i].status = .overdue
                overdueActivities.append(scheduledActivities[i])
            }
        }

        currentDate = point.date
        return overdueActivities
    }

    // MARK: - Schedule Activities

    mutating func scheduleActivity(_ activity: PlanningActivity) {
        scheduledActivities.append(activity)
    }

    mutating func scheduleNextInquiry(stage: Int, reputation: Int) {
        let clampedStage = max(1, min(5, stage))
        let idx = clampedStage - 1
        let range = Self.inquiryDayRanges[idx]

        // Base interval in game-days
        let baseDays = Double(range.min + range.max) / 2.0

        // Reduce interval by reputation above stage minimum
        let stageMinRep = Self.stageMinReputation[idx]
        let reputationAboveMin = Double(max(0, reputation - stageMinRep))
        let reductionPercent = (reputationAboveMin / 25.0) * 5.0
        let reduction = baseDays * (reductionPercent / 100.0)
        let adjustedDays = max(Double(range.min), baseDays - reduction)

        let daysUntilInquiry = max(range.min, Int(adjustedDays.rounded()))
        nextScheduledInquiryDate = currentDate.adding(days: daysUntilInquiry)
    }

    // MARK: - Activity Management

    mutating func completeActivity(id: String) {
        if let index = scheduledActivities.firstIndex(where: { $0.id == id }) {
            scheduledActivities[index].status = .completed
        }
    }

    mutating func cancelActivitiesForEvent(eventId: String) {
        for i in scheduledActivities.indices {
            if scheduledActivities[i].eventId == eventId &&
                (scheduledActivities[i].status == .scheduled || scheduledActivities[i].status == .ready) {
                scheduledActivities[i].status = .cancelled
            }
        }
    }

    func getInboxActivities() -> [PlanningActivity] {
        scheduledActivities.filter {
            $0.scheduledDate <= currentDate && $0.status == .ready
        }
    }

    func getActivitiesForEvent(eventId: String) -> [PlanningActivity] {
        scheduledActivities
            .filter { $0.eventId == eventId }
            .sorted { $0.scheduledDate < $1.scheduledDate }
    }

    // MARK: - Overdue Processing

    mutating func processOverdueActivities() -> [PlanningActivity] {
        var newWarnings: [PlanningActivity] = []

        for i in scheduledActivities.indices {
            let activity = scheduledActivities[i]
            guard let deadline = activity.responseDeadline,
                  currentDate > deadline,
                  activity.status == .ready else { continue }

            // Transition ready → overdue
            scheduledActivities[i].status = .overdue

            // Generate a vendor warning activity
            if let vendorId = activity.vendorId {
                let warning = PlanningActivity.create(
                    eventId: activity.eventId,
                    vendorId: vendorId,
                    vendorCategory: activity.vendorCategory,
                    type: .vendorOverdueWarning,
                    medium: .email,
                    scheduledDate: currentDate,
                    content: ActivityContent(
                        senderName: activity.content.senderName,
                        subject: "Following up — response needed",
                        body: "Hi, we haven't heard back regarding your booking. Please confirm at your earliest convenience or we may need to release your date."
                    )
                )
                newWarnings.append(warning)
            }
        }

        // Add warnings to the schedule (they'll show in inbox immediately)
        for var warning in newWarnings {
            warning.status = .ready
            scheduledActivities.append(warning)
        }

        return newWarnings
    }

    // MARK: - Date Management

    mutating func setCurrentDate(_ date: GameDate) {
        currentDate = date
    }

    // MARK: - Save/Load

    func getState() -> AdvanceSystemState {
        AdvanceSystemState(
            currentDate: currentDate,
            scheduledActivities: scheduledActivities,
            nextScheduledInquiryDate: nextScheduledInquiryDate
        )
    }

    mutating func restoreState(_ state: AdvanceSystemState) {
        currentDate = state.currentDate
        scheduledActivities = state.scheduledActivities
        nextScheduledInquiryDate = state.nextScheduledInquiryDate
    }
}

/// Serializable snapshot of the advance system for save/load.
struct AdvanceSystemState: Codable, Equatable {
    var currentDate: GameDate
    var scheduledActivities: [PlanningActivity]
    var nextScheduledInquiryDate: GameDate?
}
