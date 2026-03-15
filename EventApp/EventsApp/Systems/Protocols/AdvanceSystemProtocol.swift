import Foundation

/// Manages turn-based game advancement.
/// Replaces the real-time timer loop with player-initiated progression.
/// Scans all scheduled planning activities to find the next decision point.
protocol AdvanceSystemProtocol {
    var currentDate: GameDate { get }
    var scheduledActivities: [PlanningActivity] { get }
    var nextScheduledInquiryDate: GameDate? { get }

    /// Find the next date that has pending activities for the player.
    func findNextDecisionPoint() -> DecisionPoint?

    /// Advance the game to a specific decision point.
    /// Marks all activities on that date as ready.
    /// Returns activities that became overdue during the jump.
    mutating func advance(to point: DecisionPoint) -> [PlanningActivity]

    /// Schedule a new planning activity.
    mutating func scheduleActivity(_ activity: PlanningActivity)

    /// Schedule the next client inquiry arrival.
    mutating func scheduleNextInquiry(stage: Int, reputation: Int)

    /// Mark an activity as completed.
    mutating func completeActivity(id: String)

    /// Cancel all activities for an event.
    mutating func cancelActivitiesForEvent(eventId: String)

    /// Get all ready (inbox) activities for the current date.
    func getInboxActivities() -> [PlanningActivity]

    /// Get all activities for a specific event, sorted by date.
    func getActivitiesForEvent(eventId: String) -> [PlanningActivity]

    /// Check for overdue activities and generate warning messages.
    mutating func processOverdueActivities() -> [PlanningActivity]

    /// Set the current date (for save/load).
    mutating func setCurrentDate(_ date: GameDate)
}
