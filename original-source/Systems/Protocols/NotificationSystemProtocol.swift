import Foundation
import Combine

protocol NotificationSystemProtocol {
    func scheduleNotification(type: NotificationType, title: String, message: String, at time: Date) -> String
    func cancelNotification(_ id: String)
    func cancelAllNotifications()
    func isNotificationEnabled(_ type: NotificationType) -> Bool
    func setNotificationEnabled(_ type: NotificationType, enabled: Bool)
    func scheduleEventNotifications(for event: EventData, currentDate: GameDate)
    func cancelEventNotifications(eventId: String)
    func getScheduledNotifications() -> [ScheduledNotification]
    func getTodayNotificationCount() -> Int
    func canSendNotification(_ type: NotificationType, at time: Date) -> Bool
    func resetDailyCounters()
    func getNotificationPriority(_ type: NotificationType) -> Int
    func initialize(settings: NotificationSettings)
    func updateSettings(_ settings: NotificationSettings)
    var onNotificationScheduled: AnyPublisher<ScheduledNotification, Never> { get }
    var onNotificationCancelled: AnyPublisher<String, Never> { get }
    var onSettingsChanged: AnyPublisher<NotificationSettings, Never> { get }
}
