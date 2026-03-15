import Foundation

enum CalendarEntryType: String, CaseIterable, Codable, Hashable {
    case event
    case taskDeadline
    case prepTask
    case meeting
    case reminder
}

enum MessageContactType: String, CaseIterable, Codable, Hashable {
    case client
    case vendor
    case company
    case system
}

enum MessagePriority: String, CaseIterable, Codable, Hashable {
    case normal
    case important
    case urgent
}

enum TransactionType: String, CaseIterable, Codable, Hashable {
    case income
    case expense
    case pending
}

enum TransactionCategory: String, CaseIterable, Codable, Hashable {
    case eventPayment
    case vendorPayment
    case venuePayment
    case salary
    case commission
    case sideGigIncome
    case familyHelp
    case contingency
    case other
}

enum ContactType: String, CaseIterable, Codable, Hashable {
    case vendor
    case client
    case venue
    case company
}
