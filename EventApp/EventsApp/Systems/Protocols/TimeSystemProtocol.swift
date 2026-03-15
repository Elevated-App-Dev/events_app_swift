import Foundation

/// Handles event date scheduling and date math.
/// No longer manages real-time progression — that's handled by AdvanceSystem.
protocol TimeSystemProtocol {
    func scheduleEvent(complexity: EventComplexity, from currentDate: GameDate) -> GameDate
}
