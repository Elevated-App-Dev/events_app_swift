import Foundation

enum BusinessStage: String, CaseIterable, Codable, Hashable {
    case solo
    case employee
    case smallCompany
    case established
    case premier
}

enum CareerPath: String, CaseIterable, Codable, Hashable {
    case none
    case entrepreneur
    case corporate
}

enum EmployeeLevel: String, CaseIterable, Codable, Hashable {
    case junior
    case planner
    case senior
}
