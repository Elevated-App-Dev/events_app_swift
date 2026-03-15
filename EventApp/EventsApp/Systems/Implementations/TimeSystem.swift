import Foundation

/// Handles event date scheduling based on complexity.
/// Stripped of real-time progression — the AdvanceSystem now owns
/// currentDate and time advancement via player-initiated turns.
class TimeSystem: TimeSystemProtocol {

    /// Event scheduling ranges in days by complexity.
    /// Includes buffer for planning process (meeting + contract + vendor booking).
    private static let schedulingRanges: [(min: Int, max: Int)] = [
        (14, 21),   // Low complexity
        (21, 30),   // Medium complexity
        (30, 45),   // High complexity
        (45, 60)    // Very High complexity
    ]

    func scheduleEvent(complexity: EventComplexity, from currentDate: GameDate) -> GameDate {
        let complexityIndex: Int
        switch complexity {
        case .low:      complexityIndex = 0
        case .medium:   complexityIndex = 1
        case .high:     complexityIndex = 2
        case .veryHigh: complexityIndex = 3
        }
        let clamped = max(0, min(3, complexityIndex))
        let range = Self.schedulingRanges[clamped]
        let daysUntilEvent = Int.random(in: range.min...range.max)
        return currentDate.adding(days: daysUntilEvent)
    }
}
