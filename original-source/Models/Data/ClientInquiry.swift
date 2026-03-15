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
    var createdTime: Date
    var expiresTime: Date
    var isReferral: Bool
    var referredByClientName: String?

    static let expirationMinutes: Int = 20

    var isExpired: Bool {
        Date() > expiresTime
    }

    static func create(
        clientName: String,
        eventTypeId: String,
        subCategory: String,
        personality: ClientPersonality,
        budget: Int,
        guestCount: Int,
        eventDate: GameDate
    ) -> ClientInquiry {
        let now = Date()
        return ClientInquiry(
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
            createdTime: now,
            expiresTime: now.addingTimeInterval(Double(expirationMinutes) * 60),
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
        referredByClientName: String
    ) -> ClientInquiry {
        var inquiry = create(
            clientName: clientName,
            eventTypeId: eventTypeId,
            subCategory: subCategory,
            personality: personality,
            budget: budget,
            guestCount: guestCount,
            eventDate: eventDate
        )
        inquiry.isReferral = true
        inquiry.referredByClientName = referredByClientName
        return inquiry
    }
}
