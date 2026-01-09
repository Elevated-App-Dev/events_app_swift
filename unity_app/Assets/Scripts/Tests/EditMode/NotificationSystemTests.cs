using System;
using System.Linq;
using NUnit.Framework;
using EventPlannerSim.Core;
using EventPlannerSim.Data;
using EventPlannerSim.Systems;

namespace EventPlannerSim.Tests.EditMode
{
    /// <summary>
    /// Unit tests for NotificationSystemImpl.
    /// Validates: Requirements R36.1-R36.15
    /// </summary>
    [TestFixture]
    public class NotificationSystemTests
    {
        private NotificationSystemImpl _notificationSystem;
        private NotificationSettings _settings;

        [SetUp]
        public void Setup()
        {
            _settings = new NotificationSettings
            {
                eventDeadlines = true,
                taskDeadlines = true,
                newInquiries = true,
                referrals = true,
                financialWarnings = true
            };
            _notificationSystem = new NotificationSystemImpl(_settings);
        }

        #region Initialization Tests

        /// <summary>
        /// Test that NotificationSystem initializes with default settings (all OFF).
        /// **Validates: Requirements R36.7**
        /// </summary>
        [Test]
        public void Constructor_Default_AllNotificationsOff()
        {
            var defaultSettings = NotificationSettings.CreateDefault();
            var system = new NotificationSystemImpl(defaultSettings);

            Assert.IsFalse(system.IsNotificationEnabled(NotificationType.EventDeadline));
            Assert.IsFalse(system.IsNotificationEnabled(NotificationType.TaskDeadline));
            Assert.IsFalse(system.IsNotificationEnabled(NotificationType.NewInquiry));
            Assert.IsFalse(system.IsNotificationEnabled(NotificationType.Referral));
            Assert.IsFalse(system.IsNotificationEnabled(NotificationType.FinancialWarning));
        }

        /// <summary>
        /// Test that NotificationSystem initializes with provided settings.
        /// </summary>
        [Test]
        public void Constructor_WithSettings_UsesProvidedSettings()
        {
            Assert.IsTrue(_notificationSystem.IsNotificationEnabled(NotificationType.EventDeadline));
            Assert.IsTrue(_notificationSystem.IsNotificationEnabled(NotificationType.TaskDeadline));
        }

        #endregion

        #region Enable/Disable Tests (R36.6)

        /// <summary>
        /// Test that SetNotificationEnabled enables a notification type.
        /// **Validates: Requirements R36.6**
        /// </summary>
        [Test]
        public void SetNotificationEnabled_True_EnablesNotificationType()
        {
            var defaultSettings = NotificationSettings.CreateDefault();
            var system = new NotificationSystemImpl(defaultSettings);

            system.SetNotificationEnabled(NotificationType.EventDeadline, true);

            Assert.IsTrue(system.IsNotificationEnabled(NotificationType.EventDeadline));
        }

        /// <summary>
        /// Test that SetNotificationEnabled disables a notification type.
        /// **Validates: Requirements R36.6**
        /// </summary>
        [Test]
        public void SetNotificationEnabled_False_DisablesNotificationType()
        {
            _notificationSystem.SetNotificationEnabled(NotificationType.EventDeadline, false);

            Assert.IsFalse(_notificationSystem.IsNotificationEnabled(NotificationType.EventDeadline));
        }

        /// <summary>
        /// Test that SetNotificationEnabled fires OnSettingsChanged event.
        /// **Validates: Requirements R36.6**
        /// </summary>
        [Test]
        public void SetNotificationEnabled_FiresOnSettingsChangedEvent()
        {
            NotificationType? changedType = null;
            bool? changedValue = null;
            _notificationSystem.OnSettingsChanged += (type, enabled) =>
            {
                changedType = type;
                changedValue = enabled;
            };

            _notificationSystem.SetNotificationEnabled(NotificationType.Referral, false);

            Assert.AreEqual(NotificationType.Referral, changedType);
            Assert.IsFalse(changedValue);
        }

        #endregion

        #region Priority Tests (R36.11)

        /// <summary>
        /// Test that notification priorities are correctly ordered.
        /// **Validates: Requirements R36.11**
        /// Event Deadline > Task Deadline > Financial Warning > Referral > New Inquiry
        /// </summary>
        [Test]
        public void GetNotificationPriority_ReturnsCorrectOrder()
        {
            int eventDeadlinePriority = _notificationSystem.GetNotificationPriority(NotificationType.EventDeadline);
            int taskDeadlinePriority = _notificationSystem.GetNotificationPriority(NotificationType.TaskDeadline);
            int financialWarningPriority = _notificationSystem.GetNotificationPriority(NotificationType.FinancialWarning);
            int referralPriority = _notificationSystem.GetNotificationPriority(NotificationType.Referral);
            int newInquiryPriority = _notificationSystem.GetNotificationPriority(NotificationType.NewInquiry);

            Assert.Greater(eventDeadlinePriority, taskDeadlinePriority, "EventDeadline should have higher priority than TaskDeadline");
            Assert.Greater(taskDeadlinePriority, financialWarningPriority, "TaskDeadline should have higher priority than FinancialWarning");
            Assert.Greater(financialWarningPriority, referralPriority, "FinancialWarning should have higher priority than Referral");
            Assert.Greater(referralPriority, newInquiryPriority, "Referral should have higher priority than NewInquiry");
        }

        /// <summary>
        /// Test that EventDeadline has highest priority (5).
        /// **Validates: Requirements R36.11**
        /// </summary>
        [Test]
        public void GetNotificationPriority_EventDeadline_HasHighestPriority()
        {
            int priority = _notificationSystem.GetNotificationPriority(NotificationType.EventDeadline);
            Assert.AreEqual(5, priority);
        }

        /// <summary>
        /// Test that NewInquiry has lowest priority (1).
        /// **Validates: Requirements R36.11**
        /// </summary>
        [Test]
        public void GetNotificationPriority_NewInquiry_HasLowestPriority()
        {
            int priority = _notificationSystem.GetNotificationPriority(NotificationType.NewInquiry);
            Assert.AreEqual(1, priority);
        }

        #endregion

        #region Quiet Hours Tests (R36.9)

        /// <summary>
        /// Test that notifications scheduled at 11 PM are moved to 8 AM next day.
        /// **Validates: Requirements R36.9**
        /// </summary>
        [Test]
        public void ScheduleNotification_At11PM_MovesTo8AMNextDay()
        {
            DateTime deliveryTime = DateTime.Today.AddHours(23); // 11 PM

            string id = _notificationSystem.ScheduleNotification(
                NotificationType.EventDeadline,
                "Test",
                "Test message",
                deliveryTime);

            var scheduled = _notificationSystem.GetScheduledNotifications().FirstOrDefault(n => n.id == id);
            Assert.IsNotNull(scheduled);
            Assert.AreEqual(8, scheduled.scheduledTime.Hour, "Should be moved to 8 AM");
            Assert.AreEqual(DateTime.Today.AddDays(1).Date, scheduled.scheduledTime.Date, "Should be next day");
        }

        /// <summary>
        /// Test that notifications scheduled at 3 AM are moved to 8 AM same day.
        /// **Validates: Requirements R36.9**
        /// </summary>
        [Test]
        public void ScheduleNotification_At3AM_MovesTo8AMSameDay()
        {
            DateTime deliveryTime = DateTime.Today.AddHours(3); // 3 AM

            string id = _notificationSystem.ScheduleNotification(
                NotificationType.EventDeadline,
                "Test",
                "Test message",
                deliveryTime);

            var scheduled = _notificationSystem.GetScheduledNotifications().FirstOrDefault(n => n.id == id);
            Assert.IsNotNull(scheduled);
            Assert.AreEqual(8, scheduled.scheduledTime.Hour, "Should be moved to 8 AM");
            Assert.AreEqual(DateTime.Today.Date, scheduled.scheduledTime.Date, "Should be same day");
        }

        /// <summary>
        /// Test that notifications scheduled at 10 AM are not adjusted.
        /// **Validates: Requirements R36.9**
        /// </summary>
        [Test]
        public void ScheduleNotification_At10AM_NotAdjusted()
        {
            DateTime deliveryTime = DateTime.Today.AddHours(10); // 10 AM

            string id = _notificationSystem.ScheduleNotification(
                NotificationType.EventDeadline,
                "Test",
                "Test message",
                deliveryTime);

            var scheduled = _notificationSystem.GetScheduledNotifications().FirstOrDefault(n => n.id == id);
            Assert.IsNotNull(scheduled);
            Assert.AreEqual(10, scheduled.scheduledTime.Hour, "Should remain at 10 AM");
        }

        /// <summary>
        /// Test that notifications scheduled at exactly 10 PM are moved.
        /// **Validates: Requirements R36.9**
        /// </summary>
        [Test]
        public void ScheduleNotification_AtExactly10PM_MovesToNextDay()
        {
            DateTime deliveryTime = DateTime.Today.AddHours(22); // 10 PM exactly

            string id = _notificationSystem.ScheduleNotification(
                NotificationType.EventDeadline,
                "Test",
                "Test message",
                deliveryTime);

            var scheduled = _notificationSystem.GetScheduledNotifications().FirstOrDefault(n => n.id == id);
            Assert.IsNotNull(scheduled);
            Assert.AreEqual(8, scheduled.scheduledTime.Hour, "Should be moved to 8 AM");
        }

        #endregion

        #region Daily Limit Tests (R36.10)

        /// <summary>
        /// Test that daily limit of 3 notifications is enforced.
        /// **Validates: Requirements R36.10**
        /// </summary>
        [Test]
        public void ScheduleNotification_ExceedsDailyLimit_ReturnsNull()
        {
            DateTime deliveryTime = DateTime.Today.AddHours(10);

            // Schedule 3 notifications (the limit)
            _notificationSystem.ScheduleNotification(NotificationType.EventDeadline, "Test 1", "Message 1", deliveryTime);
            _notificationSystem.ScheduleNotification(NotificationType.TaskDeadline, "Test 2", "Message 2", deliveryTime);
            _notificationSystem.ScheduleNotification(NotificationType.FinancialWarning, "Test 3", "Message 3", deliveryTime);

            // 4th notification with same or lower priority should fail
            string id = _notificationSystem.ScheduleNotification(
                NotificationType.NewInquiry, // Lowest priority
                "Test 4",
                "Message 4",
                deliveryTime);

            Assert.IsNull(id, "Should return null when daily limit exceeded with lower priority");
        }

        /// <summary>
        /// Test that GetTodayNotificationCount returns correct count.
        /// **Validates: Requirements R36.10**
        /// </summary>
        [Test]
        public void GetTodayNotificationCount_ReturnsCorrectCount()
        {
            DateTime deliveryTime = DateTime.Today.AddHours(10);

            Assert.AreEqual(0, _notificationSystem.GetTodayNotificationCount());

            _notificationSystem.ScheduleNotification(NotificationType.EventDeadline, "Test", "Message", deliveryTime);
            Assert.AreEqual(1, _notificationSystem.GetTodayNotificationCount());

            _notificationSystem.ScheduleNotification(NotificationType.TaskDeadline, "Test 2", "Message 2", deliveryTime);
            Assert.AreEqual(2, _notificationSystem.GetTodayNotificationCount());
        }

        /// <summary>
        /// Test that ResetDailyCounters resets the count.
        /// **Validates: Requirements R36.10**
        /// </summary>
        [Test]
        public void ResetDailyCounters_ResetsCount()
        {
            DateTime deliveryTime = DateTime.Today.AddHours(10);
            _notificationSystem.ScheduleNotification(NotificationType.EventDeadline, "Test", "Message", deliveryTime);

            _notificationSystem.ResetDailyCounters();

            Assert.AreEqual(0, _notificationSystem.GetTodayNotificationCount());
        }

        #endregion

        #region New Inquiry Limit Tests (R36.3)

        /// <summary>
        /// Test that only 1 new inquiry notification per day is allowed.
        /// **Validates: Requirements R36.3**
        /// </summary>
        [Test]
        public void ScheduleNotification_SecondNewInquiry_ReturnsNull()
        {
            DateTime deliveryTime = DateTime.Today.AddHours(10);

            // First new inquiry should succeed
            string id1 = _notificationSystem.ScheduleNotification(
                NotificationType.NewInquiry,
                "New Client",
                "A new client is interested!",
                deliveryTime);

            // Second new inquiry should fail
            string id2 = _notificationSystem.ScheduleNotification(
                NotificationType.NewInquiry,
                "Another Client",
                "Another client is interested!",
                deliveryTime);

            Assert.IsNotNull(id1, "First new inquiry should succeed");
            Assert.IsNull(id2, "Second new inquiry should fail");
        }

        #endregion

        #region Schedule/Cancel Tests

        /// <summary>
        /// Test that ScheduleNotification returns a valid ID.
        /// </summary>
        [Test]
        public void ScheduleNotification_ReturnsValidId()
        {
            DateTime deliveryTime = DateTime.Today.AddHours(10);

            string id = _notificationSystem.ScheduleNotification(
                NotificationType.EventDeadline,
                "Test",
                "Test message",
                deliveryTime);

            Assert.IsNotNull(id);
            Assert.IsNotEmpty(id);
        }

        /// <summary>
        /// Test that ScheduleNotification fires OnNotificationScheduled event.
        /// </summary>
        [Test]
        public void ScheduleNotification_FiresOnNotificationScheduledEvent()
        {
            ScheduledNotification scheduledNotification = null;
            _notificationSystem.OnNotificationScheduled += n => scheduledNotification = n;

            DateTime deliveryTime = DateTime.Today.AddHours(10);
            _notificationSystem.ScheduleNotification(
                NotificationType.EventDeadline,
                "Test Title",
                "Test message",
                deliveryTime);

            Assert.IsNotNull(scheduledNotification);
            Assert.AreEqual("Test Title", scheduledNotification.title);
            Assert.AreEqual("Test message", scheduledNotification.message);
            Assert.AreEqual(NotificationType.EventDeadline, scheduledNotification.type);
        }

        /// <summary>
        /// Test that CancelNotification removes the notification.
        /// </summary>
        [Test]
        public void CancelNotification_RemovesNotification()
        {
            DateTime deliveryTime = DateTime.Today.AddHours(10);
            string id = _notificationSystem.ScheduleNotification(
                NotificationType.EventDeadline,
                "Test",
                "Test message",
                deliveryTime);

            _notificationSystem.CancelNotification(id);

            var scheduled = _notificationSystem.GetScheduledNotifications();
            Assert.IsFalse(scheduled.Any(n => n.id == id), "Notification should be removed");
        }

        /// <summary>
        /// Test that CancelNotification fires OnNotificationCancelled event.
        /// </summary>
        [Test]
        public void CancelNotification_FiresOnNotificationCancelledEvent()
        {
            DateTime deliveryTime = DateTime.Today.AddHours(10);
            string id = _notificationSystem.ScheduleNotification(
                NotificationType.EventDeadline,
                "Test",
                "Test message",
                deliveryTime);

            string cancelledId = null;
            _notificationSystem.OnNotificationCancelled += cid => cancelledId = cid;

            _notificationSystem.CancelNotification(id);

            Assert.AreEqual(id, cancelledId);
        }

        /// <summary>
        /// Test that CancelAllNotifications removes all notifications.
        /// </summary>
        [Test]
        public void CancelAllNotifications_RemovesAllNotifications()
        {
            DateTime deliveryTime = DateTime.Today.AddHours(10);
            _notificationSystem.ScheduleNotification(NotificationType.EventDeadline, "Test 1", "Message 1", deliveryTime);
            _notificationSystem.ScheduleNotification(NotificationType.TaskDeadline, "Test 2", "Message 2", deliveryTime);

            _notificationSystem.CancelAllNotifications();

            Assert.AreEqual(0, _notificationSystem.GetScheduledNotifications().Count);
        }

        #endregion

        #region Disabled Notification Tests

        /// <summary>
        /// Test that disabled notification types cannot be scheduled.
        /// **Validates: Requirements R36.6**
        /// </summary>
        [Test]
        public void ScheduleNotification_DisabledType_ReturnsNull()
        {
            _notificationSystem.SetNotificationEnabled(NotificationType.EventDeadline, false);

            DateTime deliveryTime = DateTime.Today.AddHours(10);
            string id = _notificationSystem.ScheduleNotification(
                NotificationType.EventDeadline,
                "Test",
                "Test message",
                deliveryTime);

            Assert.IsNull(id, "Should not schedule disabled notification type");
        }

        /// <summary>
        /// Test that CanSendNotification returns false for disabled types.
        /// **Validates: Requirements R36.6**
        /// </summary>
        [Test]
        public void CanSendNotification_DisabledType_ReturnsFalse()
        {
            _notificationSystem.SetNotificationEnabled(NotificationType.Referral, false);

            bool canSend = _notificationSystem.CanSendNotification(
                NotificationType.Referral,
                DateTime.Today.AddHours(10));

            Assert.IsFalse(canSend);
        }

        #endregion

        #region ScheduledNotification Class Tests

        /// <summary>
        /// Test that ScheduledNotification generates unique IDs.
        /// </summary>
        [Test]
        public void ScheduledNotification_GeneratesUniqueIds()
        {
            var notification1 = new ScheduledNotification();
            var notification2 = new ScheduledNotification();

            Assert.AreNotEqual(notification1.id, notification2.id);
        }

        /// <summary>
        /// Test that ScheduledNotification constructor sets all properties.
        /// </summary>
        [Test]
        public void ScheduledNotification_Constructor_SetsAllProperties()
        {
            DateTime scheduledTime = DateTime.Now.AddHours(1);
            var notification = new ScheduledNotification(
                NotificationType.Referral,
                "Test Title",
                "Test Message",
                scheduledTime);

            Assert.IsNotNull(notification.id);
            Assert.AreEqual(NotificationType.Referral, notification.type);
            Assert.AreEqual("Test Title", notification.title);
            Assert.AreEqual("Test Message", notification.message);
            Assert.AreEqual(scheduledTime, notification.scheduledTime);
            Assert.IsFalse(notification.isSent);
        }

        #endregion
    }
}
