import Foundation

protocol TimeSystemProtocol {
    var currentDate: GameDate { get }
    var isPaused: Bool { get }
    func advanceTime(deltaTime: Double, stage: Int)
    func getTimeRate(stage: Int) -> Double
    func scheduleEvent(complexity: EventComplexity, from currentDate: GameDate) -> GameDate
    func pause()
    func resume()
    func skipToDate(_ date: GameDate)
    func setCurrentDate(_ date: GameDate)
}
