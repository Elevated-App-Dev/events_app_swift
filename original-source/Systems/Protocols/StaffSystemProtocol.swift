import Foundation
import Combine

struct StaffMember: Codable, Equatable, Identifiable {
    var id: String { staffId }
    var staffId: String
    var name: String
    var role: StaffRole
    var specialty: StaffSpecialty
    var skillLevel: Double
    var reliability: Double
    var efficiency: Double
    var weeklySalary: Double
    var assignedEventIds: [String] = []
}

protocol StaffSystemProtocol {
    func getAllStaff() -> [StaffMember]
    func hireStaff(_ staff: StaffMember)
    func fireStaff(_ staffId: String)
    func assignStaffToEvent(staffId: String, eventId: String)
    func unassignStaffFromEvent(staffId: String, eventId: String)
    func getStaffForEvent(_ eventId: String) -> [StaffMember]
    func getTaskEfficiencyModifier(_ eventId: String) -> Double
    func checkTaskFailure(eventId: String, baseFailureRate: Double) -> Bool
    func getWeeklySalaryExpenses() -> Double
    func updateStaffExperience(staffId: String, satisfaction: Double)
    func requiresCompanyApproval(_ path: CareerPath) -> Bool
    var onStaffHired: AnyPublisher<StaffMember, Never> { get }
    var onStaffFired: AnyPublisher<String, Never> { get }
    var onStaffAssigned: AnyPublisher<(String, String), Never> { get }
}
