import Foundation

enum RandomEventType: String, CaseIterable, Codable, Hashable {
    case vendorNoShow
    case vendorLate
    case vendorUnderperformance
    case equipmentFailure
    case powerOutage
    case avMalfunction
    case guestConflict
    case unexpectedGuests
    case guestInjury
    case weatherChange
    case extremeWeather
    case lastMinuteChanges
    case clientComplaint
    case budgetDispute
    case unexpectedCompliment
    case mediaCoverage
    case celebrityAppearance
}
