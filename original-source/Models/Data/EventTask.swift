import Foundation

struct EventTask: Codable, Equatable, Identifiable {
    var id: String
    var taskName: String
    var description: String
    var status: TaskStatus = .pending
    var deadline: GameDate
    var hoursRequired: Int
    var dependencyTaskIds: [String] = []
    var failureConsequence: String?
    var isCritical: Bool = false
    var usedCompanyHelp: Bool = false
    var companyHelpCount: Int = 0

    static let maxCompanyHelpPerEvent = 2

    func canStart(allTasks: [EventTask]) -> Bool {
        guard status == .pending else { return false }
        for depId in dependencyTaskIds {
            if let dep = allTasks.first(where: { $0.id == depId }),
               dep.status != .completed {
                return false
            }
        }
        return true
    }

    func isOverdue(currentDate: GameDate) -> Bool {
        currentDate > deadline && status != .completed
    }

    mutating func checkDeadline(currentDate: GameDate) {
        if isOverdue(currentDate: currentDate) && status == .pending {
            status = .failed
        }
    }
}
