using System;
using EventPlannerSim.Core;

namespace EventPlannerSim.Data
{
    /// <summary>
    /// Helper class for generating notification messages.
    /// Requirements: R36.1-R36.5
    /// </summary>
    public static class NotificationMessages
    {
        /// <summary>
        /// Generate event deadline notification message.
        /// R36.1: "Your event '[Event Name]' is tomorrow! Make sure everything is ready."
        /// </summary>
        public static (string title, string message) GetEventDeadlineMessage(string eventName)
        {
            return (
                "Event Tomorrow!",
                $"Your event '{eventName}' is tomorrow! Make sure everything is ready."
            );
        }

        /// <summary>
        /// Generate task deadline notification message.
        /// R36.2: "[Task Name] is due in 2 hours" (only for critical tasks)
        /// </summary>
        public static (string title, string message) GetTaskDeadlineMessage(string taskName)
        {
            return (
                "Task Deadline Approaching",
                $"{taskName} is due in 2 hours"
            );
        }

        /// <summary>
        /// Generate new inquiry notification message.
        /// R36.3: "A new client is interested in your services!" (max 1 per day)
        /// </summary>
        public static (string title, string message) GetNewInquiryMessage()
        {
            return (
                "New Client Inquiry",
                "A new client is interested in your services!"
            );
        }

        /// <summary>
        /// Generate referral notification message.
        /// R36.4: "You received a referral from [Client Name]!"
        /// </summary>
        public static (string title, string message) GetReferralMessage(string clientName)
        {
            return (
                "New Referral!",
                $"You received a referral from {clientName}!"
            );
        }

        /// <summary>
        /// Generate financial warning notification message.
        /// R36.5: "Your funds are running low. Check your Bank app."
        /// </summary>
        public static (string title, string message) GetFinancialWarningMessage()
        {
            return (
                "Low Funds Warning",
                "Your funds are running low. Check your Bank app."
            );
        }

        /// <summary>
        /// Generate weather alert notification message.
        /// </summary>
        public static (string title, string message) GetWeatherAlertMessage(string eventName, GameDate eventDate)
        {
            return (
                "Weather Alert",
                $"Weather conditions may affect your event '{eventName}' on {eventDate.ToDisplayString()}. Check the forecast!"
            );
        }
    }

    /// <summary>
    /// Configuration for notification scheduling behavior.
    /// </summary>
    [Serializable]
    public class NotificationConfig
    {
        /// <summary>
        /// Maximum notifications per day.
        /// Requirements: R36.10
        /// </summary>
        public int maxDailyNotifications = 3;

        /// <summary>
        /// Start of quiet hours (hour in 24h format).
        /// Requirements: R36.9
        /// </summary>
        public int quietHoursStart = 22; // 10 PM

        /// <summary>
        /// End of quiet hours (hour in 24h format).
        /// Requirements: R36.9
        /// </summary>
        public int quietHoursEnd = 8; // 8 AM

        /// <summary>
        /// Maximum new inquiry notifications per day.
        /// Requirements: R36.3
        /// </summary>
        public int maxNewInquiriesPerDay = 1;

        /// <summary>
        /// Days before event to send deadline notification.
        /// </summary>
        public int eventDeadlineReminderDays = 1;

        /// <summary>
        /// Hours before task deadline to send notification.
        /// Requirements: R36.2
        /// </summary>
        public int taskDeadlineReminderHours = 2;

        /// <summary>
        /// Creates default notification configuration.
        /// </summary>
        public static NotificationConfig CreateDefault()
        {
            return new NotificationConfig();
        }
    }
}
