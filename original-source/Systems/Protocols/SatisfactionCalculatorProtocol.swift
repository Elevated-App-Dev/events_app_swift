import Foundation

struct SatisfactionResult: Codable, Equatable {
    var categoryScores: [String: Double]
    var finalSatisfaction: Double
    var personalityThreshold: Double
    var isAboveThreshold: Bool
}

protocol SatisfactionCalculatorProtocol {
    func calculate(event: EventData, client: ClientData) -> SatisfactionResult
    func calculateCategoryScore(event: EventData, category: BudgetCategory) -> Double
    func getPersonalityThreshold(_ personality: ClientPersonality) -> Double
    func getOverageTolerance(_ personality: ClientPersonality) -> Double
    func isOverageWithinTolerance(_ overage: Double, personality: ClientPersonality) -> Bool
}
