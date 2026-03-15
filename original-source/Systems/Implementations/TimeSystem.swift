import Foundation

/// Manages in-game time passage and event scheduling.
/// Stage-based time rates from 3 min/day (Stage 1) to 1 min/day (Stage 5).
class TimeSystem: TimeSystemProtocol {

    // MARK: - Constants

    /// Real minutes per game day, indexed by stage (1-5)
    private static let timeRatesByStage: [Double] = [
        3.0,   // Stage 1: 1 day per 3 real minutes
        2.5,   // Stage 2: 1 day per 2.5 real minutes
        2.0,   // Stage 3: 1 day per 2 real minutes
        1.5,   // Stage 4: 1 day per 1.5 real minutes
        1.0    // Stage 5: 1 day per 1 real minute
    ]

    /// Event scheduling ranges in days by complexity
    private static let schedulingRanges: [(min: Int, max: Int)] = [
        (3, 7),     // Low complexity
        (7, 14),    // Medium complexity
        (14, 21),   // High complexity
        (21, 30)    // Very High complexity
    ]

    // MARK: - State

    private(set) var currentDate: GameDate
    private(set) var isPaused: Bool
    private var accumulatedSeconds: Double

    // MARK: - Init

    init(startDate: GameDate = GameDate(month: 1, day: 1, year: 1)) {
        self.currentDate = startDate
        self.isPaused = false
        self.accumulatedSeconds = 0
    }

    // MARK: - Protocol Conformance

    func advanceTime(deltaTime: Double, stage: Int) {
        guard !isPaused, deltaTime > 0 else { return }

        let clampedStage = max(1, min(5, stage))
        let minutesPerDay = getTimeRate(stage: clampedStage)
        let secondsPerDay = minutesPerDay * 60.0

        accumulatedSeconds += deltaTime

        let daysToAdvance = Int(accumulatedSeconds / secondsPerDay)
        if daysToAdvance > 0 {
            currentDate = currentDate.adding(days: daysToAdvance)
            accumulatedSeconds -= Double(daysToAdvance) * secondsPerDay
        }
    }

    func getTimeRate(stage: Int) -> Double {
        let index = max(0, min(4, stage - 1))
        return Self.timeRatesByStage[index]
    }

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

    func pause() {
        isPaused = true
    }

    func resume() {
        isPaused = false
    }

    func skipToDate(_ date: GameDate) {
        if date > currentDate {
            currentDate = date
            accumulatedSeconds = 0
        }
    }

    func setCurrentDate(_ date: GameDate) {
        currentDate = date
        accumulatedSeconds = 0
    }

    // MARK: - Save/Load Support

    func setAccumulatedSeconds(_ seconds: Double) {
        accumulatedSeconds = max(0, seconds)
    }

    func getAccumulatedSeconds() -> Double {
        accumulatedSeconds
    }
}
