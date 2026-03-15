import Foundation

enum VendorCategory: String, CaseIterable, Codable, Hashable {
    case caterer
    case entertainer
    case decorator
    case photographer
    case florist
    case baker
    case rentalCompany
    case avTechnician
    case transportation
    case security
}

enum VendorTier: String, CaseIterable, Codable, Hashable {
    case budget
    case standard
    case premium
    case luxury
}
