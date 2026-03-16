import Foundation

struct EventData: Codable, Equatable, Identifiable {
    var id: String
    var clientId: String
    var clientName: String
    var eventTitle: String
    var eventTypeId: String
    var subCategory: String
    var status: EventStatus = .inquiry
    var phase: EventPhase = .booking
    var eventDate: GameDate
    var acceptedDate: GameDate?
    var personality: ClientPersonality
    var guestCount: Int
    var budget: EventBudget = EventBudget()
    var venueId: String?
    var vendors: [VendorAssignment] = []
    var tasks: [EventTask] = []
    var results: EventResults?
    var serviceFeePercent: Double = 0
    var serviceFee: Double = 0
    var negotiationRoundsUsed: Int = 0
    var isCompanyEvent: Bool = false
    var isReferral: Bool = false
    var referredByClientName: String?
}

struct VendorAssignment: Codable, Equatable {
    var vendorId: String
    var category: VendorCategory
    var agreedPrice: Double
    var isConfirmed: Bool
    var bookingDate: GameDate
}

struct EventBudget: Codable, Equatable {
    var total: Double = 0
    var venueAllocation: Double = 0
    var cateringAllocation: Double = 0
    var entertainmentAllocation: Double = 0
    var decorationsAllocation: Double = 0
    var staffingAllocation: Double = 0
    var contingencyAllocation: Double = 0
    var spent: Double = 0

    var remaining: Double { total - spent }
    var overageAmount: Double { max(0, spent - total) }
    var overagePercent: Double { total > 0 ? (overageAmount / total) * 100.0 : 0 }
}

struct EventResults: Codable, Equatable {
    var venueScore: Double = 0
    var foodScore: Double = 0
    var entertainmentScore: Double = 0
    var decorationScore: Double = 0
    var serviceScore: Double = 0
    var expectationScore: Double = 0
    var finalSatisfaction: Double = 0
    var randomEventModifier: Double = 1.0
    var profit: Double = 0
    var reputationChange: Int = 0
    var triggeredReferral: Bool = false
    var clientFeedback: String = ""
    var randomEventsOccurred: [String] = []
}

struct ClientData: Codable, Equatable {
    var clientId: String
    var clientName: String
    var personality: ClientPersonality
    var guestCount: Int
    var budgetTotal: Double
    var specialRequirements: [String]

    static func fromEvent(_ event: EventData) -> ClientData {
        ClientData(
            clientId: event.clientId,
            clientName: event.clientName,
            personality: event.personality,
            guestCount: event.guestCount,
            budgetTotal: event.budget.total,
            specialRequirements: []
        )
    }
}
