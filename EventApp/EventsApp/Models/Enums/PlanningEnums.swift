import Foundation

// MARK: - Communication

enum CommunicationMedium: String, CaseIterable, Codable, Hashable {
    case email
    case text
    case call
    case inPerson
}

// MARK: - Planning Activity Types

/// Each step in the vendor/client process that creates a calendar entry.
enum ActivityType: String, CaseIterable, Codable, Hashable {
    // Client process
    case clientMeeting
    case clientContractSent
    case clientContractSigned
    case clientDepositReceived
    case clientFinalPayment
    case clientDateChangeRequest

    // Vendor process
    case vendorAvailabilityRequest
    case vendorAvailabilityResponse
    case vendorOptionsReview
    case vendorTasting
    case vendorSiteVisit
    case vendorNegotiationOffer
    case vendorNegotiationResponse
    case vendorContractSent
    case vendorDepositPayment
    case vendorFinalConfirmation
    case vendorOverdueWarning
    case vendorOverdueFinal

    // Event lifecycle
    case eventExecution
    case eventResults
    case newInquiry
}

// MARK: - Activity Status

enum ActivityStatus: String, CaseIterable, Codable, Hashable {
    case scheduled   // Future date, not yet actionable
    case ready       // Arrived in inbox, waiting for player
    case completed   // Player handled it
    case missed      // Player advanced past the deadline without acting
    case overdue     // Past deadline but still recoverable
    case cancelled   // No longer relevant (e.g., vendor dropped, event cancelled)
}

// MARK: - Negotiation

enum NegotiationOutcome: String, CaseIterable, Codable, Hashable {
    case accepted
    case countered
    case rejected
}
