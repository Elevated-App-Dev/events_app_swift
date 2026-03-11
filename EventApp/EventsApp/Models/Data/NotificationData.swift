import Foundation

enum NotificationMessages {
    static func getEventDeadlineMessage(_ eventName: String) -> (title: String, message: String) {
        ("Event Reminder", "\(eventName) is coming up soon! Make sure everything is ready.")
    }

    static func getTaskDeadlineMessage(_ taskName: String) -> (title: String, message: String) {
        ("Task Due", "\(taskName) needs to be completed soon.")
    }

    static func getNewInquiryMessage() -> (title: String, message: String) {
        ("New Inquiry", "A potential client is interested in your services!")
    }

    static func getReferralMessage(_ clientName: String) -> (title: String, message: String) {
        ("Referral!", "\(clientName) has referred a new client to you!")
    }

    static func getFinancialWarningMessage() -> (title: String, message: String) {
        ("Low Funds", "Your balance is running low. Consider accepting more events.")
    }

    static func getWeatherAlertMessage(_ eventName: String, eventDate: GameDate) -> (title: String, message: String) {
        ("Weather Alert", "Bad weather predicted for \(eventName) on \(eventDate.formatted). Consider contingency plans.")
    }
}

struct NotificationConfig: Codable, Equatable {
    var maxDailyNotifications: Int = 3
    var quietHoursStart: Int = 22
    var quietHoursEnd: Int = 8
    var maxNewInquiriesPerDay: Int = 1
    var eventDeadlineReminderDays: Int = 1
    var taskDeadlineReminderHours: Int = 2

    static func createDefault() -> NotificationConfig {
        NotificationConfig()
    }
}

struct ScheduledNotification: Codable, Equatable, Identifiable {
    var id: String
    var type: NotificationType
    var title: String
    var message: String
    var scheduledTime: Date
}
