using System;
using System.Collections.Generic;
using System.Linq;
using EventPlannerSim.Core;
using EventPlannerSim.Data;

namespace EventPlannerSim.Systems
{
    /// <summary>
    /// Implementation of the notification system.
    /// Manages push notifications and local reminders.
    /// Requirements: R36.1-R36.15
    /// </summary>
    public class NotificationSystemImpl : INotificationSystem
    {
        // Constants for notification limits
        private const int MaxDailyNotifications = 3; // R36.10
        private const int QuietHoursStart = 22; // 10 PM - R36.9
        private const int QuietHoursEnd = 8; // 8 AM - R36.9

        // Priority values (higher = more urgent) - R36.11
        private static readonly Dictionary<NotificationType, int> NotificationPriorities = new Dictionary<NotificationType, int>
        {
            { NotificationType.EventDeadline, 5 },
            { NotificationType.TaskDeadline, 4 },
            { NotificationType.FinancialWarning, 3 },
            { NotificationType.Referral, 2 },
            { NotificationType.NewInquiry, 1 },
            { NotificationType.WeatherAlert, 3 } // Same priority as financial warning
        };

        private NotificationSettings _settings;
        private readonly List<ScheduledNotification> _scheduledNotifications = new List<ScheduledNotification>();
        private readonly Dictionary<string, List<string>> _eventNotificationIds = new Dictionary<string, List<string>>();
        
        // Daily tracking
        private DateTime _lastResetDate;
        private int _notificationsSentToday;
        private bool _newInquirySentToday; // R36.3: max 1 per day

        // Events
        public event Action<ScheduledNotification> OnNotificationScheduled;
        public event Action<string> OnNotificationCancelled;
        public event Action<NotificationType, bool> OnSettingsChanged;

        public NotificationSystemImpl()
        {
            _settings = NotificationSettings.CreateDefault();
            _lastResetDate = DateTime.Today;
            _notificationsSentToday = 0;
            _newInquirySentToday = false;
        }

        public NotificationSystemImpl(NotificationSettings settings) : this()
        {
            _settings = settings ?? NotificationSettings.CreateDefault();
        }

        /// <summary>
        /// Initialize the notification system with settings.
        /// </summary>
        public void Initialize(NotificationSettings settings)
        {
            _settings = settings ?? NotificationSettings.CreateDefault();
            _lastResetDate = DateTime.Today;
            _notificationsSentToday = 0;
            _newInquirySentToday = false;
        }

        /// <summary>
        /// Update settings reference.
        /// </summary>
        public void UpdateSettings(NotificationSettings settings)
        {
            _settings = settings ?? NotificationSettings.CreateDefault();
        }

        /// <summary>
        /// Schedule a notification for future delivery.
        /// Requirements: R36.1-R36.5, R36.9-R36.12, R36.14
        /// </summary>
        public string ScheduleNotification(NotificationType type, string title, string message, DateTime deliveryTime)
        {
            // Check if we can send this notification
            if (!CanSendNotification(type, deliveryTime))
            {
                return null;
            }

            // Adjust delivery time if in quiet hours
            DateTime adjustedTime = AdjustForQuietHours(deliveryTime);

            var notification = new ScheduledNotification(type, title, message, adjustedTime)
            {
                priority = GetNotificationPriority(type)
            };

            _scheduledNotifications.Add(notification);
            _notificationsSentToday++;
            
            // Track new inquiry limit
            if (type == NotificationType.NewInquiry && adjustedTime.Date == DateTime.Today)
            {
                _newInquirySentToday = true;
            }

            OnNotificationScheduled?.Invoke(notification);
            return notification.id;
        }

        /// <summary>
        /// Cancel a previously scheduled notification.
        /// </summary>
        public void CancelNotification(string notificationId)
        {
            if (string.IsNullOrEmpty(notificationId))
                return;

            var notification = _scheduledNotifications.FirstOrDefault(n => n.id == notificationId);
            if (notification != null)
            {
                _scheduledNotifications.Remove(notification);
                OnNotificationCancelled?.Invoke(notificationId);
            }
        }

        /// <summary>
        /// Cancel all scheduled notifications.
        /// </summary>
        public void CancelAllNotifications()
        {
            var ids = _scheduledNotifications.Select(n => n.id).ToList();
            _scheduledNotifications.Clear();
            _eventNotificationIds.Clear();

            foreach (var id in ids)
            {
                OnNotificationCancelled?.Invoke(id);
            }
        }

        /// <summary>
        /// Check if notification type is enabled by player.
        /// Requirements: R36.6
        /// </summary>
        public bool IsNotificationEnabled(NotificationType type)
        {
            if (_settings == null)
                return false;

            return type switch
            {
                NotificationType.EventDeadline => _settings.eventDeadlines,
                NotificationType.TaskDeadline => _settings.taskDeadlines,
                NotificationType.NewInquiry => _settings.newInquiries,
                NotificationType.Referral => _settings.referrals,
                NotificationType.FinancialWarning => _settings.financialWarnings,
                NotificationType.WeatherAlert => _settings.eventDeadlines, // Weather alerts follow event deadline setting
                _ => false
            };
        }

        /// <summary>
        /// Update notification preferences.
        /// Requirements: R36.6
        /// </summary>
        public void SetNotificationEnabled(NotificationType type, bool enabled)
        {
            if (_settings == null)
                return;

            switch (type)
            {
                case NotificationType.EventDeadline:
                    _settings.eventDeadlines = enabled;
                    break;
                case NotificationType.TaskDeadline:
                    _settings.taskDeadlines = enabled;
                    break;
                case NotificationType.NewInquiry:
                    _settings.newInquiries = enabled;
                    break;
                case NotificationType.Referral:
                    _settings.referrals = enabled;
                    break;
                case NotificationType.FinancialWarning:
                    _settings.financialWarnings = enabled;
                    break;
                case NotificationType.WeatherAlert:
                    // Weather alerts follow event deadline setting
                    _settings.eventDeadlines = enabled;
                    break;
            }

            OnSettingsChanged?.Invoke(type, enabled);
        }

        /// <summary>
        /// Schedule event-related notifications (deadline reminders, etc.).
        /// Requirements: R36.1, R36.2
        /// </summary>
        public void ScheduleEventNotifications(EventData eventData, GameDate currentDate)
        {
            if (eventData == null)
                return;

            // Cancel any existing notifications for this event
            CancelEventNotifications(eventData.id);

            var notificationIds = new List<string>();

            // Schedule event deadline notification (1 day before)
            // R36.1: "Your event '[Event Name]' is tomorrow! Make sure everything is ready."
            if (IsNotificationEnabled(NotificationType.EventDeadline))
            {
                int daysUntilEvent = GameDate.DaysBetween(currentDate, eventData.eventDate);
                if (daysUntilEvent > 1)
                {
                    // Schedule for the day before at 9 AM (after quiet hours)
                    DateTime deliveryTime = DateTime.Today.AddDays(daysUntilEvent - 1).AddHours(9);
                    string title = "Event Tomorrow!";
                    string message = $"Your event '{eventData.eventTitle}' is tomorrow! Make sure everything is ready.";
                    
                    string id = ScheduleNotificationInternal(
                        NotificationType.EventDeadline, 
                        title, 
                        message, 
                        deliveryTime, 
                        eventData.id);
                    
                    if (!string.IsNullOrEmpty(id))
                        notificationIds.Add(id);
                }
            }

            // Schedule task deadline notifications for critical tasks
            // R36.2: "[Task Name] is due in 2 hours" (only for critical tasks)
            if (IsNotificationEnabled(NotificationType.TaskDeadline) && eventData.tasks != null)
            {
                foreach (var task in eventData.tasks.Where(t => t.status != TaskStatus.Completed && t.isCritical))
                {
                    int daysUntilDeadline = GameDate.DaysBetween(currentDate, task.deadline);
                    if (daysUntilDeadline >= 0)
                    {
                        // Schedule 2 hours before deadline (simulated as morning of deadline day)
                        DateTime deliveryTime = DateTime.Today.AddDays(daysUntilDeadline).AddHours(10);
                        string title = "Task Deadline Approaching";
                        string message = $"{task.taskName} is due in 2 hours";
                        
                        string id = ScheduleNotificationInternal(
                            NotificationType.TaskDeadline, 
                            title, 
                            message, 
                            deliveryTime, 
                            eventData.id);
                        
                        if (!string.IsNullOrEmpty(id))
                            notificationIds.Add(id);
                    }
                }
            }

            if (notificationIds.Count > 0)
            {
                _eventNotificationIds[eventData.id] = notificationIds;
            }
        }

        /// <summary>
        /// Cancel all notifications for a specific event.
        /// </summary>
        public void CancelEventNotifications(string eventId)
        {
            if (string.IsNullOrEmpty(eventId))
                return;

            if (_eventNotificationIds.TryGetValue(eventId, out var notificationIds))
            {
                foreach (var id in notificationIds)
                {
                    CancelNotification(id);
                }
                _eventNotificationIds.Remove(eventId);
            }
        }

        /// <summary>
        /// Get all currently scheduled notifications.
        /// </summary>
        public IReadOnlyList<ScheduledNotification> GetScheduledNotifications()
        {
            return _scheduledNotifications.AsReadOnly();
        }

        /// <summary>
        /// Get the count of notifications sent today.
        /// Requirements: R36.10
        /// </summary>
        public int GetTodayNotificationCount()
        {
            CheckAndResetDailyCounters();
            return _notificationsSentToday;
        }

        /// <summary>
        /// Check if a notification can be sent (respects daily limits and quiet hours).
        /// Requirements: R36.9-R36.11
        /// </summary>
        public bool CanSendNotification(NotificationType type, DateTime deliveryTime)
        {
            CheckAndResetDailyCounters();

            // Check if notification type is enabled
            if (!IsNotificationEnabled(type))
                return false;

            // Check daily limit (R36.10)
            if (_notificationsSentToday >= MaxDailyNotifications)
            {
                // Check if this notification has higher priority than any scheduled
                int newPriority = GetNotificationPriority(type);
                var lowestPriorityScheduled = _scheduledNotifications
                    .Where(n => n.scheduledTime.Date == deliveryTime.Date && !n.isSent)
                    .OrderBy(n => n.priority)
                    .FirstOrDefault();

                // If no lower priority notification to replace, can't send
                if (lowestPriorityScheduled == null || lowestPriorityScheduled.priority >= newPriority)
                    return false;
            }

            // Check new inquiry limit (R36.3: max 1 per day)
            if (type == NotificationType.NewInquiry && _newInquirySentToday)
                return false;

            return true;
        }

        /// <summary>
        /// Reset daily notification counters (called at midnight).
        /// </summary>
        public void ResetDailyCounters()
        {
            _lastResetDate = DateTime.Today;
            _notificationsSentToday = 0;
            _newInquirySentToday = false;
        }

        /// <summary>
        /// Get the priority of a notification type (higher = more urgent).
        /// Requirements: R36.11
        /// Event Deadline > Task Deadline > Financial Warning > Referral > New Inquiry
        /// </summary>
        public int GetNotificationPriority(NotificationType type)
        {
            return NotificationPriorities.TryGetValue(type, out int priority) ? priority : 0;
        }

        #region Private Helper Methods

        /// <summary>
        /// Internal method to schedule notification with event tracking.
        /// </summary>
        private string ScheduleNotificationInternal(NotificationType type, string title, string message, DateTime deliveryTime, string eventId)
        {
            if (!CanSendNotification(type, deliveryTime))
                return null;

            DateTime adjustedTime = AdjustForQuietHours(deliveryTime);

            var notification = new ScheduledNotification(type, title, message, adjustedTime)
            {
                priority = GetNotificationPriority(type),
                relatedEventId = eventId
            };

            _scheduledNotifications.Add(notification);
            _notificationsSentToday++;

            if (type == NotificationType.NewInquiry && adjustedTime.Date == DateTime.Today)
            {
                _newInquirySentToday = true;
            }

            OnNotificationScheduled?.Invoke(notification);
            return notification.id;
        }

        /// <summary>
        /// Adjust delivery time to respect quiet hours (10 PM - 8 AM).
        /// Requirements: R36.9
        /// </summary>
        private DateTime AdjustForQuietHours(DateTime deliveryTime)
        {
            int hour = deliveryTime.Hour;

            // If in quiet hours (10 PM to midnight or midnight to 8 AM)
            if (hour >= QuietHoursStart || hour < QuietHoursEnd)
            {
                // Move to 8 AM
                if (hour >= QuietHoursStart)
                {
                    // After 10 PM, move to next day 8 AM
                    return deliveryTime.Date.AddDays(1).AddHours(QuietHoursEnd);
                }
                else
                {
                    // Before 8 AM, move to 8 AM same day
                    return deliveryTime.Date.AddHours(QuietHoursEnd);
                }
            }

            return deliveryTime;
        }

        /// <summary>
        /// Check if daily counters need to be reset.
        /// </summary>
        private void CheckAndResetDailyCounters()
        {
            if (DateTime.Today > _lastResetDate)
            {
                ResetDailyCounters();
            }
        }

        #endregion
    }
}
