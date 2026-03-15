import Foundation

struct RandomEventResult: Codable, Equatable {
    var eventType: RandomEventType
    var eventDescription: String
    var baseSatisfactionImpact: Double
    var mitigationCost: Double
    var canBeMitigated: Bool
    var wasMitigated: Bool = false
    var mitigationDescription: String
    var failureDescription: String

    func getFinalImpact() -> Double {
        wasMitigated ? baseSatisfactionImpact * 0.25 : baseSatisfactionImpact
    }
}

struct MitigationResult: Codable, Equatable {
    var canMitigate: Bool
    var requiredBudget: Double
    var availableBudget: Double
    var mitigationOption: String
    var reducedImpact: Double
}
