import Foundation

/// A single scheduled step in the event planning process.
/// These populate the player's inbox on their scheduled date.
/// Each vendor interaction, client touchpoint, and event milestone
/// is represented as a PlanningActivity with a concrete game date.
struct PlanningActivity: Codable, Equatable, Identifiable {
    let id: String
    let eventId: String
    let vendorId: String?
    let vendorCategory: VendorCategory?
    let clientName: String?
    let type: ActivityType
    let medium: CommunicationMedium
    let scheduledDate: GameDate
    var status: ActivityStatus
    var responseDeadline: GameDate?
    var content: ActivityContent

    /// Creates an activity with a unique ID.
    static func create(
        eventId: String,
        vendorId: String? = nil,
        vendorCategory: VendorCategory? = nil,
        clientName: String? = nil,
        type: ActivityType,
        medium: CommunicationMedium,
        scheduledDate: GameDate,
        responseDeadline: GameDate? = nil,
        content: ActivityContent
    ) -> PlanningActivity {
        PlanningActivity(
            id: UUID().uuidString,
            eventId: eventId,
            vendorId: vendorId,
            vendorCategory: vendorCategory,
            clientName: clientName,
            type: type,
            medium: medium,
            scheduledDate: scheduledDate,
            status: .scheduled,
            responseDeadline: responseDeadline,
            content: content
        )
    }
}

// MARK: - Activity Content

/// The payload of a planning activity — what the player sees in their inbox.
/// Different activity types populate different fields.
struct ActivityContent: Codable, Equatable {
    var senderName: String
    var subject: String
    var body: String

    // Vendor-specific payloads
    var menuOptions: [MenuOption]?
    var quoteAmount: Double?
    var counterOfferAmount: Double?
    var negotiationRound: Int?
    var alternateDate: GameDate?
    var depositAmount: Double?

    // Client-specific payloads
    var dialogueTranscript: [DialogueLine]?
    var revealedRequirements: [String]?
    var contractAmount: Double?
}

// MARK: - Dialogue

/// A single line of dialogue from a client meeting or call.
/// The player reads these to infer personality, budget signals, and requirements.
struct DialogueLine: Codable, Equatable {
    let speaker: DialogueSpeaker
    let text: String
}

enum DialogueSpeaker: String, Codable, Equatable {
    case client
    case player
}

// MARK: - Menu / Options

/// A selectable option from a vendor (menu item, package, setup style, etc.)
struct MenuOption: Codable, Equatable, Identifiable {
    let id: String
    let name: String
    let description: String
    let price: Double
    let category: String

    static func create(
        name: String,
        description: String,
        price: Double,
        category: String = ""
    ) -> MenuOption {
        MenuOption(
            id: UUID().uuidString,
            name: name,
            description: description,
            price: price,
            category: category
        )
    }
}

// MARK: - Vendor Process Template

/// Defines the steps and timing for a vendor category's booking process.
/// Used by the planning system to schedule activities when a player
/// initiates contact with a vendor.
struct VendorProcessTemplate {
    let category: VendorCategory
    let steps: [VendorProcessStep]
}

struct VendorProcessStep {
    let activityType: ActivityType
    let medium: CommunicationMedium
    let daysOffset: ClosedRange<Int>
    let hasDeadline: Bool
    let deadlineDaysAfter: Int
    let isPlayerInitiated: Bool

    init(
        activityType: ActivityType,
        medium: CommunicationMedium,
        daysOffset: ClosedRange<Int>,
        hasDeadline: Bool = false,
        deadlineDaysAfter: Int = 3,
        isPlayerInitiated: Bool = false
    ) {
        self.activityType = activityType
        self.medium = medium
        self.daysOffset = daysOffset
        self.hasDeadline = hasDeadline
        self.deadlineDaysAfter = deadlineDaysAfter
        self.isPlayerInitiated = isPlayerInitiated
    }
}

// MARK: - Decision Point

/// Represents the next moment in the game that requires player attention.
/// The AdvanceSystem scans all scheduled activities and returns the nearest one.
struct DecisionPoint: Equatable {
    let date: GameDate
    let activities: [PlanningActivity]

    var isEmpty: Bool { activities.isEmpty }
}
