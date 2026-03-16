import Foundation

struct GameDate: Codable, Equatable, Hashable, Comparable {
    var month: Int
    var day: Int
    var year: Int

    init(month: Int = 1, day: Int = 1, year: Int = 2026) {
        self.month = month
        self.day = day
        self.year = year
    }

    // MARK: - Display

    var formatted: String {
        "\(monthName) \(day), \(year)"
    }

    var shortFormatted: String {
        "\(month)/\(day)/\(year)"
    }

    var monthName: String {
        let names = ["January", "February", "March", "April", "May", "June",
                     "July", "August", "September", "October", "November", "December"]
        guard month >= 1 && month <= 12 else { return "Unknown" }
        return names[month - 1]
    }

    // MARK: - Calculations

    func adding(days: Int) -> GameDate {
        var result = self
        var remaining = days
        let daysInMonth = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31]

        while remaining > 0 {
            let currentMonthDays = daysInMonth[result.month - 1]
            let daysLeftInMonth = currentMonthDays - result.day

            if remaining <= daysLeftInMonth {
                result.day += remaining
                remaining = 0
            } else {
                remaining -= (daysLeftInMonth + 1)
                result.month += 1
                result.day = 1
                if result.month > 12 {
                    result.month = 1
                    result.year += 1
                }
            }
        }
        return result
    }

    func daysBetween(_ other: GameDate) -> Int {
        let selfTotal = totalDays
        let otherTotal = other.totalDays
        return otherTotal - selfTotal
    }

    private var totalDays: Int {
        let daysInMonth = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31]
        var total = (year - 1) * 365
        for m in 0..<(month - 1) {
            total += daysInMonth[m]
        }
        total += day
        return total
    }

    // MARK: - Comparable

    static func < (lhs: GameDate, rhs: GameDate) -> Bool {
        if lhs.year != rhs.year { return lhs.year < rhs.year }
        if lhs.month != rhs.month { return lhs.month < rhs.month }
        return lhs.day < rhs.day
    }
}
