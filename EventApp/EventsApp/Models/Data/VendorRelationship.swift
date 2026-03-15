import Foundation

/// Tracks the player's relationship with a specific vendor.
/// Hidden from the player — they feel it through vendor behavior
/// (pricing flexibility, response speed, willingness to negotiate).
struct VendorRelationship: Codable, Equatable, Identifiable {
    var id: String { vendorId }
    var vendorId: String

    // Relationship score: 0-100, starts at 50 (neutral)
    var score: Int = 50

    // Tracking metrics that feed into the score
    var totalBookings: Int = 0
    var completedBookings: Int = 0
    var missedDeadlines: Int = 0
    var onTimeResponses: Int = 0
    var lateResponses: Int = 0
    var totalNegotiations: Int = 0
    var successfulNegotiations: Int = 0
    var cancelledBookings: Int = 0

    // Legacy fields retained for compatibility
    var timesHired: Int = 0
    var reliabilityRevealed: Bool = false
    var flexibilityRevealed: Bool = false
    var playerNote: String = ""

    var tier: RelationshipTier {
        switch score {
        case 0..<20: return .burned
        case 20..<40: return .strained
        case 40..<60: return .neutral
        case 60..<80: return .good
        default: return .preferred
        }
    }

    // Legacy computed property — now derived from tier
    var relationshipLevel: Int {
        switch tier {
        case .burned: return 0
        case .strained: return 1
        case .neutral: return 1
        case .good: return 2
        case .preferred: return 3
        }
    }

    var discountPercent: Double { pricingFlexibility }

    static func createNew(vendorId: String) -> VendorRelationship {
        VendorRelationship(vendorId: vendorId)
    }

    // MARK: - Relationship Effects

    /// How much the vendor is willing to flex on price (0.0 to 0.15).
    var pricingFlexibility: Double {
        switch tier {
        case .burned: return 0
        case .strained: return 0.02
        case .neutral: return 0.05
        case .good: return 0.10
        case .preferred: return 0.15
        }
    }

    /// How quickly the vendor responds. Returns a reduction in days.
    var responseSpeedBonus: Int {
        switch tier {
        case .good, .preferred: return 1
        default: return 0
        }
    }

    /// Whether the vendor will even accept a booking request.
    var willAcceptBooking: Bool {
        tier != .burned
    }

    /// Base probability that the vendor will negotiate.
    var negotiationWillingness: Double {
        switch tier {
        case .burned: return 0
        case .strained: return 0.2
        case .neutral: return 0.5
        case .good: return 0.7
        case .preferred: return 0.9
        }
    }

    // MARK: - Mutations

    mutating func recordOnTimeResponse() {
        onTimeResponses += 1
        adjustScore(by: 2)
    }

    mutating func recordLateResponse() {
        lateResponses += 1
        adjustScore(by: -5)
    }

    mutating func recordMissedDeadline() {
        missedDeadlines += 1
        adjustScore(by: -10)
    }

    mutating func recordCompletedBooking() {
        completedBookings += 1
        timesHired += 1
        adjustScore(by: 5)
        updateRevealedAttributes()
    }

    mutating func recordCancelledBooking() {
        cancelledBookings += 1
        adjustScore(by: -8)
    }

    mutating func recordNegotiation(successful: Bool) {
        totalNegotiations += 1
        if successful {
            successfulNegotiations += 1
        } else {
            adjustScore(by: -3)
        }
    }

    mutating func recordBookingStarted() {
        totalBookings += 1
        adjustScore(by: 1)
    }

    mutating func recordHire() {
        timesHired += 1
        updateRevealedAttributes()
    }

    mutating func updateRevealedAttributes() {
        if timesHired >= 2 { reliabilityRevealed = true }
        if timesHired >= 3 { flexibilityRevealed = true }
    }

    private mutating func adjustScore(by amount: Int) {
        score = max(0, min(100, score + amount))
    }
}

// MARK: - Relationship Tier

enum RelationshipTier: String, CaseIterable, Codable, Hashable {
    case burned      // 0-19: vendor won't work with you
    case strained    // 20-39: slow responses, no flexibility
    case neutral     // 40-59: standard behavior
    case good        // 60-79: better pricing, faster responses
    case preferred   // 80-100: best pricing, priority availability
}
