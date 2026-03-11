import Foundation
import Combine

protocol NotificationSystemProtocol {
    func initialize(settings: NotificationSettings)
    func scheduleNotification(type: NotificationType, title: String, message: String, deliveryTime: Date) -> String?
    func cancelNotification(_ notificationId: String)
    func cancelAllNotifications()
    func cancelEventNotifications(_ eventId: String)
    func isNotificationEnabled(_ type: NotificationType) -> Bool
    func setNotificationEnabled(type: NotificationType, enabled: Bool)
    func canSendNotification(type: NotificationType, deliveryTime: Date) -> Bool
    func getNotificationPriority(_ type: NotificationType) -> Int
    func getScheduledNotifications() -> [ScheduledNotification]
    func getTodayNotificationCount() -> Int
    func resetDailyCounters()
    var onNotificationScheduled: AnyPublisher<ScheduledNotification, Never> { get }
    var onNotificationCancelled: AnyPublisher<String, Never> { get }
}
