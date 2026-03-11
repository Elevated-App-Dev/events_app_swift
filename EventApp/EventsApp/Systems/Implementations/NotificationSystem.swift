import Foundation
import Combine

/// Manages push notifications and local reminders.
/// Daily limit of 3, quiet hours 10PM-8AM, priority-based scheduling.
class NotificationSystem: NotificationSystemProtocol {

    private static let maxDailyNotifications = 3
    private static let quietHoursStart = 22  // 10 PM
    private static let quietHoursEnd = 8     // 8 AM

    private static let notificationPriorities: [NotificationType: Int] = [
        .eventDeadline:     5,
        .taskDeadline:      4,
        .financialWarning:  3,
        .referral:          2,
        .newInquiry:        1,
        .weatherAlert:      3
    ]

    private var settings: NotificationSettings
    private var scheduledNotifications: [ScheduledNotification] = []
    private var eventNotificationIds: [String: [String]] = [:]
    private var lastResetDate: Date
    private var notificationsSentToday = 0
    private var newInquirySentToday = false

    private let _onNotificationScheduled = PassthroughSubject<ScheduledNotification, Never>()
    private let _onNotificationCancelled = PassthroughSubject<String, Never>()

    var onNotificationScheduled: AnyPublisher<ScheduledNotification, Never> { _onNotificationScheduled.eraseToAnyPublisher() }
    var onNotificationCancelled: AnyPublisher<String, Never> { _onNotificationCancelled.eraseToAnyPublisher() }

    init(settings: NotificationSettings? = nil) {
        self.settings = settings ?? NotificationSettings()
        self.lastResetDate = Calendar.current.startOfDay(for: Date())
    }

    func initialize(settings: NotificationSettings) {
        self.settings = settings
        resetDailyCounters()
    }

    func scheduleNotification(type: NotificationType, title: String, message: String, deliveryTime: Date) -> String? {
        guard canSendNotification(type: type, deliveryTime: deliveryTime) else { return nil }

        let adjustedTime = adjustForQuietHours(deliveryTime)
        let notification = ScheduledNotification(
            id: UUID().uuidString,
            type: type,
            title: title,
            message: message,
            scheduledTime: adjustedTime
        )

        scheduledNotifications.append(notification)
        notificationsSentToday += 1

        if type == .newInquiry && Calendar.current.isDateInToday(adjustedTime) {
            newInquirySentToday = true
        }

        _onNotificationScheduled.send(notification)
        return notification.id
    }

    func cancelNotification(_ notificationId: String) {
        scheduledNotifications.removeAll { $0.id == notificationId }
        _onNotificationCancelled.send(notificationId)
    }

    func cancelAllNotifications() {
        let ids = scheduledNotifications.map { $0.id }
        scheduledNotifications.removeAll()
        eventNotificationIds.removeAll()
        ids.forEach { _onNotificationCancelled.send($0) }
    }

    func cancelEventNotifications(_ eventId: String) {
        guard let ids = eventNotificationIds[eventId] else { return }
        ids.forEach { cancelNotification($0) }
        eventNotificationIds.removeValue(forKey: eventId)
    }

    func isNotificationEnabled(_ type: NotificationType) -> Bool {
        switch type {
        case .eventDeadline:    return settings.eventDeadlines
        case .taskDeadline:     return settings.taskDeadlines
        case .newInquiry:       return settings.newInquiries
        case .referral:         return settings.referrals
        case .financialWarning: return settings.financialWarnings
        case .weatherAlert:     return settings.eventDeadlines
        }
    }

    func setNotificationEnabled(type: NotificationType, enabled: Bool) {
        switch type {
        case .eventDeadline:    settings.eventDeadlines = enabled
        case .taskDeadline:     settings.taskDeadlines = enabled
        case .newInquiry:       settings.newInquiries = enabled
        case .referral:         settings.referrals = enabled
        case .financialWarning: settings.financialWarnings = enabled
        case .weatherAlert:     settings.eventDeadlines = enabled
        }
    }

    func canSendNotification(type: NotificationType, deliveryTime: Date) -> Bool {
        checkAndResetDailyCounters()
        guard isNotificationEnabled(type) else { return false }
        if notificationsSentToday >= Self.maxDailyNotifications { return false }
        if type == .newInquiry && newInquirySentToday { return false }
        return true
    }

    func getNotificationPriority(_ type: NotificationType) -> Int {
        Self.notificationPriorities[type] ?? 0
    }

    func getScheduledNotifications() -> [ScheduledNotification] {
        scheduledNotifications
    }

    func getTodayNotificationCount() -> Int {
        checkAndResetDailyCounters()
        return notificationsSentToday
    }

    func resetDailyCounters() {
        lastResetDate = Calendar.current.startOfDay(for: Date())
        notificationsSentToday = 0
        newInquirySentToday = false
    }

    // MARK: - Private

    private func adjustForQuietHours(_ time: Date) -> Date {
        let hour = Calendar.current.component(.hour, from: time)
        if hour >= Self.quietHoursStart {
            // After 10 PM → next day 8 AM
            return Calendar.current.startOfDay(for: time).addingTimeInterval(TimeInterval((24 + Self.quietHoursEnd) * 3600))
        } else if hour < Self.quietHoursEnd {
            // Before 8 AM → 8 AM same day
            return Calendar.current.startOfDay(for: time).addingTimeInterval(TimeInterval(Self.quietHoursEnd * 3600))
        }
        return time
    }

    private func checkAndResetDailyCounters() {
        let today = Calendar.current.startOfDay(for: Date())
        if today > lastResetDate {
            resetDailyCounters()
        }
    }
}
