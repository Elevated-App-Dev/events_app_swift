import Foundation

protocol ConsequenceSystemProtocol {
    func evaluateRandomEvents(event: EventData, stage: Int) -> [RandomEventResult]
    func calculateRandomEventModifier(results: [RandomEventResult]) -> Double
    func checkMitigation(event: RandomEventResult, availableBudget: Double) -> MitigationResult
    func getRandomEventFrequency(stage: Int) -> Double
}
