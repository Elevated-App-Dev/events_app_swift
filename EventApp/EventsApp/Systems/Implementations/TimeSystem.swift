import Foundation

/// Handles event date scheduling based on complexity.
/// Stripped of real-time progression — the AdvanceSystem now owns
/// currentDate and time advancement via player-initiated turns.
class TimeSystem: TimeSystemProtocol {

    /// Event scheduling ranges in days by complexity.
    private static let schedulingRanges: [(min: Int, max: Int)] = [
        (3, 7),     // Low complexity
        (7, 14),    // Medium complexity
        (14, 21),   // High complexity
        (21, 30)    // Very High complexity
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
