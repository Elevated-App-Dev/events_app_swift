import Foundation

struct ReputationChange: Codable, Equatable {
    var previousReputation: Int
    var newReputation: Int
    var change: Int
    var satisfactionTier: String
}

struct StageRequirements: Codable, Equatable {
    var requiredReputation: Int
    var requiredMoney: Double
    var requiredEventsCompleted: Int
    var description: String
}

struct PersonalityDistribution: Codable, Equatable {
    var weights: [String: Double]

    func weight(for personality: ClientPersonality) -> Double {
        weights[personality.rawValue] ?? 0
    }
}

struct PerformanceReviewResult: Codable, Equatable {
    var outcome: PerformanceReviewOutcome
    var message: String
    var scoreChange: Int
}

protocol ProgressionSystemProtocol {
    func applyEventResult(satisfaction: Double, currentReputation: Int, stage: Int) -> ReputationChange
    func canAdvanceStage(player: PlayerData) -> Bool
    func getStageRequirements(_ stage: BusinessStage) -> StageRequirements
    func getPersonalityDistribution(stage: Int) -> PersonalityDistribution
    func evaluatePerformance(events: [EventData], performanceScore: Int, level: Int) -> PerformanceReviewResult
    func getRandomEventFrequency(stage: Int) -> Double
    func getMinimumReputationThreshold(_ stage: BusinessStage) -> Int
    func calculateCelebrityReputationChange(satisfaction: Double, press: PressCoverage) -> Int
    func shouldTriggerStage3Milestone(player: PlayerData, progress: MilestoneProgress) -> Bool
    func advanceToNextStage(_ player: inout PlayerData)
}
