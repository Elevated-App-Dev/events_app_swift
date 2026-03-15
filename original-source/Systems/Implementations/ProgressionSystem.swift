import Foundation

class ProgressionSystem: ProgressionSystemProtocol {
    // Reputation change ranges by satisfaction tier (R14.2-R14.6)
    private let highSatRange = 15...25
    private let goodSatRange = 5...14
    private let okaySatRange = 1...4
    private let poorSatRange = (-10)...(-5)
    private let failedSatRange = (-25)...(-15)

    // Satisfaction thresholds
    private let highThreshold: Double = 90
    private let goodThreshold: Double = 75
    private let okayThreshold: Double = 60
    private let poorThreshold: Double = 40

    // Performance review thresholds (R16.6-R16.8)
    private let positiveReviewSatisfaction: Double = 70
    private let positiveReviewTaskRate: Double = 80
    private let negativeReviewSatisfaction: Double = 60
    private let negativeReviewTaskRate: Double = 60

    // Celebrity reputation loss cap (R15.19a)
    private let celebrityRepLossCap = -50

    func applyEventResult(satisfaction: Double, currentReputation: Int, stage: Int) -> ReputationChange {
        let change: Int
        let tier: String

        if satisfaction >= highThreshold {
            change = Int.random(in: highSatRange)
            tier = "Excellent"
        } else if satisfaction >= goodThreshold {
            change = Int.random(in: goodSatRange)
            tier = "Good"
        } else if satisfaction >= okayThreshold {
            change = Int.random(in: okaySatRange)
            tier = "Okay"
        } else if satisfaction >= poorThreshold {
            change = Int.random(in: poorSatRange)
            tier = "Poor"
        } else {
            change = Int.random(in: failedSatRange)
            tier = "Failed"
        }

        return ReputationChange(
            previousReputation: currentReputation,
            newReputation: max(0, currentReputation + change),
            change: change,
            satisfactionTier: tier
        )
    }

    func canAdvanceStage(player: PlayerData) -> Bool {
        let requirements = getStageRequirements(player.stage)
        if player.reputation < requirements.requiredReputation { return false }
        if player.money < requirements.requiredMoney { return false }
        if player.stage == .employee {
            guard let empData = player.employeeData else { return false }
            if empData.employeeLevel < requirements.requiredEventsCompleted { return false }
        }
        return true
    }

    func getStageRequirements(_ stage: BusinessStage) -> StageRequirements {
        switch stage {
        case .solo:
            return StageRequirements(requiredReputation: 25, requiredMoney: 5000, requiredEventsCompleted: 0,
                                     description: "Reach 25 reputation and save $5,000 to unlock Stage 2")
        case .employee:
            return StageRequirements(requiredReputation: 50, requiredMoney: 25000, requiredEventsCompleted: 5,
                                     description: "Reach Senior Planner (Level 5), 50 reputation, and save $25,000")
        case .smallCompany:
            return StageRequirements(requiredReputation: 100, requiredMoney: 75000, requiredEventsCompleted: 0,
                                     description: "Reach 100 reputation and save $75,000 to unlock Stage 4")
        case .established:
            return StageRequirements(requiredReputation: 200, requiredMoney: 250000, requiredEventsCompleted: 0,
                                     description: "Reach 200 reputation and save $250,000 to unlock Stage 5")
        case .premier:
            return StageRequirements(requiredReputation: 0, requiredMoney: 0, requiredEventsCompleted: 0,
                                     description: "Maximum stage reached!")
        }
    }

    func getPersonalityDistribution(stage: Int) -> PersonalityDistribution {
        switch stage {
        case 1:
            return PersonalityDistribution(weights: [
                ClientPersonality.easyGoing.rawValue: 0.50,
                ClientPersonality.budgetConscious.rawValue: 0.30,
                ClientPersonality.perfectionist.rawValue: 0.20
            ])
        case 2:
            return PersonalityDistribution(weights: [
                ClientPersonality.easyGoing.rawValue: 0.40,
                ClientPersonality.budgetConscious.rawValue: 0.35,
                ClientPersonality.perfectionist.rawValue: 0.25
            ])
        default:
            return PersonalityDistribution(weights: [
                ClientPersonality.easyGoing.rawValue: 0.33,
                ClientPersonality.budgetConscious.rawValue: 0.33,
                ClientPersonality.perfectionist.rawValue: 0.34
            ])
        }
    }

    func evaluatePerformance(events: [EventData], performanceScore: Int, level: Int) -> PerformanceReviewResult {
        guard !events.isEmpty else {
            return PerformanceReviewResult(outcome: .neutral, message: "No events to review.", scoreChange: 0)
        }

        let avgSatisfaction = events
            .compactMap { $0.results?.finalSatisfaction }
            .reduce(0, +) / max(1, Double(events.count))

        var totalTasks = 0
        var completedOnTime = 0
        for event in events {
            for task in event.tasks {
                totalTasks += 1
                if task.status == .completed { completedOnTime += 1 }
            }
        }
        let onTimeRate = totalTasks > 0 ? (Double(completedOnTime) / Double(totalTasks)) * 100 : 100

        if avgSatisfaction >= positiveReviewSatisfaction && onTimeRate >= positiveReviewTaskRate {
            let promoted = level < 5
            return PerformanceReviewResult(
                outcome: .positive,
                message: promoted ? "Excellent work! You've been promoted." : "Outstanding performance!",
                scoreChange: promoted ? 1 : 0
            )
        } else if avgSatisfaction < negativeReviewSatisfaction || onTimeRate < negativeReviewTaskRate {
            return PerformanceReviewResult(
                outcome: .negative,
                message: "Your performance needs improvement.",
                scoreChange: -1
            )
        } else {
            return PerformanceReviewResult(
                outcome: .neutral,
                message: "Your performance is acceptable. Keep working to improve.",
                scoreChange: 0
            )
        }
    }

    func getRandomEventFrequency(stage: Int) -> Double {
        switch stage {
        case 1: return 0.20
        case 2: return 0.35
        case 3: return 0.50
        case 4: return 0.65
        case 5: return 0.80
        default: return 0.20
        }
    }

    func getMinimumReputationThreshold(_ stage: BusinessStage) -> Int {
        switch stage {
        case .solo: return 0
        case .employee: return 10
        case .smallCompany: return 30
        case .established: return 75
        case .premier: return 150
        }
    }

    func calculateCelebrityReputationChange(satisfaction: Double, press: PressCoverage) -> Int {
        let baseChange: Int
        if satisfaction >= highThreshold {
            baseChange = Int.random(in: highSatRange)
        } else if satisfaction >= goodThreshold {
            baseChange = Int.random(in: goodSatRange)
        } else if satisfaction >= okayThreshold {
            baseChange = Int.random(in: okaySatRange)
        } else if satisfaction >= poorThreshold {
            baseChange = Int.random(in: poorSatRange)
        } else {
            baseChange = Int.random(in: failedSatRange)
        }

        let multiplier: Double
        switch press {
        case .positive: multiplier = satisfaction >= 70 ? 3.0 : 2.0
        case .neutral: multiplier = satisfaction >= 70 ? 2.0 : 2.5
        case .negative: multiplier = satisfaction >= 70 ? 1.5 : 3.0
        }

        let finalChange = Int(Double(baseChange) * multiplier)
        return max(celebrityRepLossCap, finalChange)
    }

    func shouldTriggerStage3Milestone(player: PlayerData, progress: MilestoneProgress) -> Bool {
        if progress.hasSeenStage3Milestone && progress.hasChosenPath { return false }
        return player.stage == .smallCompany
    }

    func advanceToNextStage(_ player: inout PlayerData) {
        guard canAdvanceStage(player: player) else { return }
        switch player.stage {
        case .solo: player.stage = .employee
        case .employee: player.stage = .smallCompany
        case .smallCompany: player.stage = .established
        case .established: player.stage = .premier
        case .premier: break
        }
    }
}
