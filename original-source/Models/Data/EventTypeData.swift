import Foundation

struct EventTypeData: Codable, Equatable, Identifiable {
    var id: String { eventTypeId }
    var eventTypeId: String
    var displayName: String
    var complexity: EventComplexity
    var minStageRequired: Int = 1
    var minBudget: Int
    var maxBudget: Int
    var subCategories: [String]
    var requiredVendors: [VendorCategory]
    var optionalVendors: [VendorCategory]
    var recommendedBudgetSplit: [Double] = [0.30, 0.30, 0.15, 0.15, 0.05, 0.05]

    var recommendedVenuePercent: Double { recommendedBudgetSplit.indices.contains(0) ? recommendedBudgetSplit[0] : 0.30 }
    var recommendedCateringPercent: Double { recommendedBudgetSplit.indices.contains(1) ? recommendedBudgetSplit[1] : 0.30 }
    var recommendedEntertainmentPercent: Double { recommendedBudgetSplit.indices.contains(2) ? recommendedBudgetSplit[2] : 0.15 }
    var recommendedDecorationsPercent: Double { recommendedBudgetSplit.indices.contains(3) ? recommendedBudgetSplit[3] : 0.15 }
    var recommendedStaffingPercent: Double { recommendedBudgetSplit.indices.contains(4) ? recommendedBudgetSplit[4] : 0.05 }
    var recommendedContingencyPercent: Double { recommendedBudgetSplit.indices.contains(5) ? recommendedBudgetSplit[5] : 0.05 }

    func generateRandomBudget() -> Int {
        Int.random(in: minBudget...maxBudget)
    }

    func getRandomSubCategory() -> String {
        subCategories.randomElement() ?? displayName
    }

    func isAvailableAtStage(_ stage: Int) -> Bool {
        stage >= minStageRequired
    }

    func getSchedulingRange() -> (min: Int, max: Int) {
        switch complexity {
        case .low: return (3, 7)
        case .medium: return (7, 14)
        case .high: return (14, 21)
        case .veryHigh: return (21, 30)
        }
    }
}
