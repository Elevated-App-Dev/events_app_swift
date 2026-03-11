import Foundation

struct VendorRelationship: Codable, Equatable {
    var vendorId: String
    var timesHired: Int = 0
    var reliabilityRevealed: Bool = false
    var flexibilityRevealed: Bool = false
    var playerNote: String = ""

    var relationshipLevel: Int {
        switch timesHired {
        case 0: return 0
        case 1...2: return 1
        case 3...4: return 2
        default: return 3
        }
    }

    var discountPercent: Double {
        switch relationshipLevel {
        case 0: return 0
        case 1: return 0.05
        case 2: return 0.10
        default: return 0.15
        }
    }

    mutating func recordHire() {
        timesHired += 1
        updateRevealedAttributes()
    }

    mutating func updateRevealedAttributes() {
        if timesHired >= 2 { reliabilityRevealed = true }
        if timesHired >= 3 { flexibilityRevealed = true }
    }
}
