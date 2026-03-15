import Foundation

enum AnalyticsEvents {
    // Event Names
    static let tutorialStarted = "tutorial_started"
    static let tutorialCompleted = "tutorial_completed"
    static let tutorialSkipped = "tutorial_skipped"
    static let eventAccepted = "event_accepted"
    static let eventCompleted = "event_completed"
    static let eventFailed = "event_failed"
    static let stageAdvanced = "stage_advanced"
    static let stage3PathChosen = "stage3_path_chosen"
    static let vendorBooked = "vendor_booked"
    static let venueBooked = "venue_booked"
    static let purchaseInitiated = "purchase_initiated"
    static let purchaseCompleted = "purchase_completed"
    static let purchaseFailed = "purchase_failed"
    static let adWatched = "ad_watched"
    static let adSkipped = "ad_skipped"
    static let familyHelpUsed = "family_help_used"
    static let financialCrisis = "financial_crisis"
    static let sessionStart = "session_start"
    static let sessionEnd = "session_end"

    static func isValidEvent(_ eventName: String) -> Bool {
        allEvents.contains(eventName)
    }

    static var allEvents: [String] {
        [tutorialStarted, tutorialCompleted, tutorialSkipped,
         eventAccepted, eventCompleted, eventFailed,
         stageAdvanced, stage3PathChosen,
         vendorBooked, venueBooked,
         purchaseInitiated, purchaseCompleted, purchaseFailed,
         adWatched, adSkipped,
         familyHelpUsed, financialCrisis,
         sessionStart, sessionEnd]
    }

    // Parameter builders
    static func eventAcceptedParams(eventTypeId: String, budget: Double, guestCount: Int, stage: Int) -> [String: Any] {
        ["event_type_id": eventTypeId, "budget": budget, "guest_count": guestCount, "stage": stage]
    }

    static func eventCompletedParams(eventTypeId: String, satisfaction: Double, profit: Double, stage: Int) -> [String: Any] {
        ["event_type_id": eventTypeId, "satisfaction": satisfaction, "profit": profit, "stage": stage]
    }

    static func stageAdvancedParams(fromStage: Int, toStage: Int) -> [String: Any] {
        ["from_stage": fromStage, "to_stage": toStage]
    }

    static func purchaseParams(productId: String, price: Double) -> [String: Any] {
        ["product_id": productId, "price": price]
    }

    static func adParams(placement: AdPlacement, completed: Bool) -> [String: Any] {
        ["placement": placement.rawValue, "completed": completed]
    }
}
