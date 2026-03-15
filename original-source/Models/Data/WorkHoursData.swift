import Foundation

struct WorkHoursData: Codable, Equatable {
    var dailyCapacity: Int = 8
    var hoursUsedToday: Int = 0
    var overtimeUsedToday: Int = 0

    static let maxOvertimeAdsPerDay = 2
    static let hoursPerOvertimeAd = 4

    var remainingHours: Int {
        dailyCapacity + (overtimeUsedToday * Self.hoursPerOvertimeAd) - hoursUsedToday
    }

    var canUseOvertime: Bool {
        overtimeUsedToday < Self.maxOvertimeAdsPerDay
    }

    var totalAvailableHours: Int {
        dailyCapacity + (overtimeUsedToday * Self.hoursPerOvertimeAd)
    }

    mutating func resetDaily() {
        hoursUsedToday = 0
        overtimeUsedToday = 0
    }

    mutating func useHours(_ hours: Int) -> Bool {
        guard hours <= remainingHours else { return false }
        hoursUsedToday += hours
        return true
    }

    mutating func addOvertime() -> Bool {
        guard canUseOvertime else { return false }
        overtimeUsedToday += 1
        return true
    }
}
