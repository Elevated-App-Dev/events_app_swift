import Foundation

class SatisfactionCalculator: SatisfactionCalculatorProtocol {
    // Category weights (R13.2)
    private let venueWeight: Double = 0.20
    private let foodWeight: Double = 0.25
    private let entertainmentWeight: Double = 0.20
    private let decorationWeight: Double = 0.15
    private let serviceWeight: Double = 0.10
    private let expectationWeight: Double = 0.10

    func calculate(event: EventData, client: ClientData) -> SatisfactionResult {
        let results = event.results ?? EventResults()

        // Calculate weighted base satisfaction (R13.2)
        let baseSatisfaction = calculateWeightedSatisfaction(
            venue: results.venueScore,
            food: results.foodScore,
            entertainment: results.entertainmentScore,
            decoration: results.decorationScore,
            service: results.serviceScore,
            expectation: results.expectationScore
        )

        // Apply random event modifier (R13.3)
        let modifiedSatisfaction = baseSatisfaction * results.randomEventModifier

        // Clamp to 0-100 (R13.8)
        let finalSatisfaction = Self.clampSatisfaction(modifiedSatisfaction)

        // Get client threshold based on personality (R15.2-R15.6)
        let clientThreshold = getPersonalityThreshold(client.personality)

        return SatisfactionResult(
            categoryScores: [
                "venue": results.venueScore,
                "food": results.foodScore,
                "entertainment": results.entertainmentScore,
                "decoration": results.decorationScore,
                "service": results.serviceScore,
                "expectation": results.expectationScore
            ],
            finalSatisfaction: finalSatisfaction,
            personalityThreshold: clientThreshold,
            isAboveThreshold: finalSatisfaction >= clientThreshold
        )
    }

    private func calculateWeightedSatisfaction(
        venue: Double, food: Double, entertainment: Double,
        decoration: Double, service: Double, expectation: Double
    ) -> Double {
        venue * venueWeight +
        food * foodWeight +
        entertainment * entertainmentWeight +
        decoration * decorationWeight +
        service * serviceWeight +
        expectation * expectationWeight
    }

    static func clampSatisfaction(_ satisfaction: Double) -> Double {
        max(0, min(100, satisfaction))
    }

    func calculateCategoryScore(event: EventData, category: BudgetCategory) -> Double {
        guard let results = event.results else { return 0 }
        switch category {
        case .venue: return results.venueScore
        case .catering: return results.foodScore
        case .entertainment: return results.entertainmentScore
        case .decorations: return results.decorationScore
        default: return 50
        }
    }

    func getPersonalityThreshold(_ personality: ClientPersonality) -> Double {
        switch personality {
        case .easyGoing: return 50       // R15.2
        case .budgetConscious: return 60 // R15.3
        case .perfectionist: return 85   // R15.4
        case .indecisive: return 65      // R15.7
        case .demanding: return 80       // R15.8
        case .celebrity: return 70       // R15.9
        }
    }

    func getOverageTolerance(_ personality: ClientPersonality) -> Double {
        switch personality {
        case .easyGoing: return 15
        case .budgetConscious: return 0
        case .perfectionist: return 5
        case .indecisive: return 10
        case .demanding: return 5
        case .celebrity: return 10
        }
    }

    func isOverageWithinTolerance(_ overage: Double, personality: ClientPersonality) -> Bool {
        overage <= getOverageTolerance(personality)
    }
}
