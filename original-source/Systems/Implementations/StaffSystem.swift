import Foundation
import Combine

/// Stub implementation of StaffSystem for MVP.
/// Returns empty/default values — full implementation is Post-MVP.
class StaffSystem: StaffSystemProtocol {

    private let _onStaffHired = PassthroughSubject<StaffMember, Never>()
    private let _onStaffFired = PassthroughSubject<String, Never>()
    private let _onStaffAssigned = PassthroughSubject<(String, String), Never>()

    var onStaffHired: AnyPublisher<StaffMember, Never> { _onStaffHired.eraseToAnyPublisher() }
    var onStaffFired: AnyPublisher<String, Never> { _onStaffFired.eraseToAnyPublisher() }
    var onStaffAssigned: AnyPublisher<(String, String), Never> { _onStaffAssigned.eraseToAnyPublisher() }

    func getAllStaff() -> [StaffMember] { [] }
    func hireStaff(_ staff: StaffMember) { }
    func fireStaff(_ staffId: String) { }
    func assignStaffToEvent(staffId: String, eventId: String) { }
    func unassignStaffFromEvent(staffId: String, eventId: String) { }
    func getStaffForEvent(_ eventId: String) -> [StaffMember] { [] }
    func getTaskEfficiencyModifier(_ eventId: String) -> Double { 1.0 }

    func checkTaskFailure(eventId: String, baseFailureRate: Double) -> Bool { false }
    func getWeeklySalaryExpenses() -> Double { 0 }
    func updateStaffExperience(staffId: String, satisfaction: Double) { }

    func requiresCompanyApproval(_ path: CareerPath) -> Bool {
        path == .corporate
    }
}
