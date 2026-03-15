import Foundation

enum MarketingTier: String, CaseIterable, Codable, Hashable {
    case none
    case basic
    case standard
    case premium
}

enum MarketingFocus: String, CaseIterable, Codable, Hashable {
    case general
    case corporate
    case wedding
    case social
    case luxury
}
