import Foundation

enum StaffRole: String, CaseIterable, Codable, Hashable {
    case assistant
    case coordinator
    case specialist
    case manager
}

enum StaffSpecialty: String, CaseIterable, Codable, Hashable {
    case general
    case weddings
    case corporate
    case social
    case luxury
}

enum OfficeType: String, CaseIterable, Codable, Hashable {
    case homeOffice
    case companyOffice
    case smallOffice
    case agencyOffice
    case flagshipShowroom
    case directorOffice
    case regionalHeadquarters
    case executiveSuite
}
