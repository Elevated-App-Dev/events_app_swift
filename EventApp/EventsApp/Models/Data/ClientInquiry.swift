import Foundation

struct ClientInquiry: Codable, Equatable, Identifiable {
    var id: String { inquiryId }
    var inquiryId: String
    var clientName: String
    var eventTypeId: String
    var subCategory: String
    var eventDisplayName: String
    var personality: ClientPersonality
    var budget: Int
    var guestCount: Int
    var eventDate: GameDate
    var specialRequirements: [String]
    var arrivedDate: GameDate
    var isReferral: Bool
    var referredByClientName: String?

    static func create(
        clientName: String,
        eventTypeId: String,
        subCategory: String,
        personality: ClientPersonality,
        budget: Int,
        guestCount: Int,
        eventDate: GameDate,
        arrivedDate: GameDate
    ) -> ClientInquiry {
        ClientInquiry(
            inquiryId: UUID().uuidString,
            clientName: clientName,
            eventTypeId: eventTypeId,
            subCategory: subCategory,
            eventDisplayName: "\(clientName)'s \(subCategory)",
            personality: personality,
            budget: budget,
            guestCount: guestCount,
            eventDate: eventDate,
            specialRequirements: [],
            arrivedDate: arrivedDate,
            isReferral: false,
            referredByClientName: nil
        )
    }

    static func createReferral(
        clientName: String,
        eventTypeId: String,
        subCategory: String,
        personality: ClientPersonality,
        budget: Int,
        guestCount: Int,
        eventDate: GameDate,
        arrivedDate: GameDate,
        referredByClientName: String
    ) -> ClientInquiry {
        var inquiry = create(
            clientName: clientName,
            eventTypeId: eventTypeId,
            subCategory: subCategory,
            personality: personality,
            budget: budget,
            guestCount: guestCount,
            eventDate: eventDate,
            arrivedDate: arrivedDate
        )
        inquiry.isReferral = true
        inquiry.referredByClientName = referredByClientName
        return inquiry
    }
}
