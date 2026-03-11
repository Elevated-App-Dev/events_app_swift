import Foundation

struct BookingEntry: Codable, Equatable {
    var entityId: String
    var bookedDates: [GameDate] = []

    func isBookedOn(_ date: GameDate) -> Bool {
        bookedDates.contains(date)
    }

    mutating func addBooking(_ date: GameDate) -> Bool {
        guard !isBookedOn(date) else { return false }
        bookedDates.append(date)
        return true
    }

    mutating func removeBooking(_ date: GameDate) -> Bool {
        guard let idx = bookedDates.firstIndex(of: date) else { return false }
        bookedDates.remove(at: idx)
        return true
    }
}

struct SaveData: Codable, Equatable {
    var saveVersion: String = "1.0"
    var lastSavedTime: Date = Date()
    var playerData: PlayerData = PlayerData()
    var activeEvents: [EventData] = []
    var eventHistory: [EventData] = []
    var workHours: WorkHoursData = WorkHoursData()
    var weather: WeatherSystemData = WeatherSystemData()
    var currentDate: GameDate = GameDate()
    var vendorBookings: [BookingEntry] = []
    var venueBookings: [BookingEntry] = []
    var settings: GameSettings = GameSettings()
    var excellenceStreak: Int = 0
    var pendingInquiries: [ClientInquiry] = []
    var milestoneProgress: MilestoneProgress = MilestoneProgress()
    var journeyStartTime: Date?

    func getVendorBookedDates(_ vendorId: String) -> [GameDate] {
        vendorBookings.first(where: { $0.entityId == vendorId })?.bookedDates ?? []
    }

    func getVenueBookedDates(_ venueId: String) -> [GameDate] {
        venueBookings.first(where: { $0.entityId == venueId })?.bookedDates ?? []
    }

    mutating func addVendorBooking(_ vendorId: String, date: GameDate) {
        if let idx = vendorBookings.firstIndex(where: { $0.entityId == vendorId }) {
            _ = vendorBookings[idx].addBooking(date)
        } else {
            var entry = BookingEntry(entityId: vendorId)
            _ = entry.addBooking(date)
            vendorBookings.append(entry)
        }
    }

    mutating func addVenueBooking(_ venueId: String, date: GameDate) {
        if let idx = venueBookings.firstIndex(where: { $0.entityId == venueId }) {
            _ = venueBookings[idx].addBooking(date)
        } else {
            var entry = BookingEntry(entityId: venueId)
            _ = entry.addBooking(date)
            venueBookings.append(entry)
        }
    }

    func isVendorAvailable(_ vendorId: String, on date: GameDate) -> Bool {
        !(vendorBookings.first(where: { $0.entityId == vendorId })?.isBookedOn(date) ?? false)
    }

    func isVenueAvailable(_ venueId: String, on date: GameDate) -> Bool {
        !(venueBookings.first(where: { $0.entityId == venueId })?.isBookedOn(date) ?? false)
    }

    mutating func removeVendorBooking(_ vendorId: String, date: GameDate) -> Bool {
        guard let idx = vendorBookings.firstIndex(where: { $0.entityId == vendorId }) else { return false }
        return vendorBookings[idx].removeBooking(date)
    }

    mutating func removeVenueBooking(_ venueId: String, date: GameDate) -> Bool {
        guard let idx = venueBookings.firstIndex(where: { $0.entityId == venueId }) else { return false }
        return venueBookings[idx].removeBooking(date)
    }
}

struct MilestoneProgress: Codable, Equatable {
    var hasSeenStage3Milestone: Bool = false
    var hasChosenPath: Bool = false
    var chosenPath: CareerPath = .none
    var canSkipMilestoneSequence: Bool = false
}

struct CareerSummaryData: Codable, Equatable {
    var totalEventsCompleted: Int = 0
    var firstEventName: String = ""
    var highestSatisfactionEventName: String = ""
    var highestSatisfactionScore: Double = 0
    var totalMoneyEarned: Double = 0
    var currentReputation: Int = 0
    var journeyStartDate: Date?
    var stage3ReachedDate: Date?
}
