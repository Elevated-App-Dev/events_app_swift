using System;
using System.Collections.Generic;
using EventPlannerSim.Core;
using EventPlannerSim.Data;

namespace EventPlannerSim.Systems
{
    /// <summary>
    /// Manages push notifications and local reminders.
    /// Requirements: R36.1-R36.15
    /// </summary>
    public interface INotificationSystem
    {
        /// <summary>
        /// Schedule a notification for future delivery.
        /// Returns the notification ID for later cancellation.
        /// Requirements: R36.1-R36.5, R36.14
        /// </summary>
        string ScheduleNotification(NotificationType type, string title, string message, DateTime deliveryTime);

        /// <summary>
        /// Cancel a previously scheduled notification.
        /// </summary>
        void CancelNotification(string notificationId);

        /// <summary>
        /// Cancel all scheduled notifications.
        /// </summary>
        void CancelAllNotifications();

        /// <summary>
        /// Check if notification type is enabled by player.
        /// Requirements: R36.6
        /// </summary>
        bool IsNotificationEnabled(NotificationType type);

        /// <summary>
        /// Update notification preferences.
        /// Requirements: R36.6
        /// </summary>
        void SetNotificationEnabled(NotificationType type, bool enabled);

        /// <summary>
        /// Schedule event-related notifications (deadline reminders, etc.).
        /// Requirements: R36.1, R36.2
        /// </summary>
        void ScheduleEventNotifications(EventData eventData, GameDate currentDate);

        /// <summary>
        /// Cancel all notifications for a specific event.
        /// </summary>
        void CancelEventNotifications(string eventId);

        /// <summary>
        /// Get all currently scheduled notifications.
        /// </summary>
        IReadOnlyList<ScheduledNotification> GetScheduledNotifications();

        /// <summary>
        /// Get the count of notifications sent today.
        /// Requirements: R36.10
        /// </summary>
        int GetTodayNotificationCount();

        /// <summary>
        /// Check if a notification can be sent (respects daily limits and quiet hours).
        /// Requirements: R36.9-R36.11
        /// </summary>
        bool CanSendNotification(NotificationType type, DateTime deliveryTime);

        /// <summary>
        /// Reset daily notification counters (called at midnight).
        /// </summary>
        void ResetDailyCounters();

        /// <summary>
        /// Get the priority of a notification type (higher = more urgent).
        /// Requirements: R36.11
        /// </summary>
        int GetNotificationPriority(NotificationType type);

        /// <summary>
        /// Initialize the notification system with settings.
        /// </summary>
        void Initialize(NotificationSettings settings);

        /// <summary>
        /// Update settings reference.
        /// </summary>
        void UpdateSettings(NotificationSettings settings);

        /// <summary>
        /// Event fired when a notification is scheduled.
        /// </summary>
        event Action<ScheduledNotification> OnNotificationScheduled;

        /// <summary>
        /// Event fired when a notification is cancelled.
        /// </summary>
        event Action<string> OnNotificationCancelled;

        /// <summary>
        /// Event fired when notification settings change.
        /// </summary>
        event Action<NotificationType, bool> OnSettingsChanged;
    }

    /// <summary>
    /// Represents a scheduled notification.
    /// </summary>
    [Serializable]
    public class ScheduledNotification
    {
        public string id;
        public NotificationType type;
        public string title;
        public string message;
        public DateTime scheduledTime;
        public string relatedEventId; // Optional: for event-related notifications
        public int priority;
        public bool isSent;

        public ScheduledNotification()
        {
            id = Guid.NewGuid().ToString();
        }

        public ScheduledNotification(NotificationType type, string title, string message, DateTime scheduledTime)
        {
            this.id = Guid.NewGuid().ToString();
            this.type = type;
            this.title = title;
            this.message = message;
            this.scheduledTime = scheduledTime;
            this.isSent = false;
        }
    }
}
