using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using EventPlannerSim.Core;
using EventPlannerSim.Data;
using EventPlannerSim.Systems;

namespace EventPlannerSim.Tests.EditMode
{
    /// <summary>
    /// Unit tests for UnityServicesManager.
    /// Validates: Requirements R31.1-R31.8, R35.23-R35.24
    /// </summary>
    [TestFixture]
    public class UnityServicesManagerTests
    {
        private UnityServicesManagerImpl _servicesManager;

        [SetUp]
        public void Setup()
        {
            _servicesManager = new UnityServicesManagerImpl();
        }

        #region Initialization Tests (R31.4)

        /// <summary>
        /// Test that constructor creates uninitialized manager.
        /// **Validates: Requirements R31.4**
        /// </summary>
        [Test]
        public void Constructor_CreatesUninitializedManager()
        {
            Assert.IsFalse(_servicesManager.IsInitialized);
        }

        /// <summary>
        /// Test that Initialize sets IsInitialized to true.
        /// **Validates: Requirements R31.4**
        /// </summary>
        [Test]
        public void Initialize_SetsIsInitializedToTrue()
        {
            _servicesManager.Initialize();
            
            Assert.IsTrue(_servicesManager.IsInitialized);
        }

        /// <summary>
        /// Test that Initialize fires OnInitialized event.
        /// **Validates: Requirements R31.4**
        /// </summary>
        [Test]
        public void Initialize_FiresOnInitializedEvent()
        {
            bool? initResult = null;
            _servicesManager.OnInitialized += result => initResult = result;
            
            _servicesManager.Initialize();
            
            Assert.IsTrue(initResult.HasValue);
            Assert.IsTrue(initResult.Value);
        }

        /// <summary>
        /// Test that Initialize can be called multiple times safely.
        /// **Validates: Requirements R31.4**
        /// </summary>
        [Test]
        public void Initialize_CanBeCalledMultipleTimesSafely()
        {
            int eventCount = 0;
            _servicesManager.OnInitialized += _ => eventCount++;
            
            _servicesManager.Initialize();
            _servicesManager.Initialize();
            
            Assert.AreEqual(1, eventCount, "Should only fire once");
        }

        #endregion

        #region Analytics Consent Tests (R35.23, R35.24)

        /// <summary>
        /// Test that HasAnalyticsConsent defaults to false.
        /// **Validates: Requirements R35.23**
        /// </summary>
        [Test]
        public void HasAnalyticsConsent_DefaultsToFalse()
        {
            Assert.IsFalse(_servicesManager.HasAnalyticsConsent);
        }

        /// <summary>
        /// Test that SetAnalyticsConsent updates consent status.
        /// **Validates: Requirements R35.23, R35.24**
        /// </summary>
        [Test]
        public void SetAnalyticsConsent_UpdatesConsentStatus()
        {
            _servicesManager.SetAnalyticsConsent(true);
            
            Assert.IsTrue(_servicesManager.HasAnalyticsConsent);
        }

        /// <summary>
        /// Test that SetAnalyticsConsent can revoke consent.
        /// **Validates: Requirements R35.24**
        /// </summary>
        [Test]
        public void SetAnalyticsConsent_CanRevokeConsent()
        {
            _servicesManager.SetAnalyticsConsent(true);
            _servicesManager.SetAnalyticsConsent(false);
            
            Assert.IsFalse(_servicesManager.HasAnalyticsConsent);
        }

        #endregion

        #region TrackEvent Tests (R31.1, R31.5)

        /// <summary>
        /// Test that TrackEvent does nothing when not initialized.
        /// **Validates: Requirements R31.1**
        /// </summary>
        [Test]
        public void TrackEvent_DoesNothingWhenNotInitialized()
        {
            _servicesManager.SetAnalyticsConsent(true);
            
            _servicesManager.TrackEvent(AnalyticsEvents.TutorialStarted);
            
            Assert.AreEqual(0, _servicesManager.GetEventCount());
        }

        /// <summary>
        /// Test that TrackEvent does nothing without consent.
        /// **Validates: Requirements R35.24**
        /// </summary>
        [Test]
        public void TrackEvent_DoesNothingWithoutConsent()
        {
            _servicesManager.Initialize();
            
            _servicesManager.TrackEvent(AnalyticsEvents.TutorialStarted);
            
            Assert.AreEqual(0, _servicesManager.GetEventCount());
        }

        /// <summary>
        /// Test that TrackEvent tracks valid event with consent.
        /// **Validates: Requirements R31.1, R31.5**
        /// </summary>
        [Test]
        public void TrackEvent_TracksValidEventWithConsent()
        {
            _servicesManager.Initialize();
            _servicesManager.SetAnalyticsConsent(true);
            
            _servicesManager.TrackEvent(AnalyticsEvents.TutorialStarted);
            
            Assert.AreEqual(1, _servicesManager.GetEventCount());
        }

        /// <summary>
        /// Test that TrackEvent fires OnEventTracked event.
        /// **Validates: Requirements R31.1**
        /// </summary>
        [Test]
        public void TrackEvent_FiresOnEventTrackedEvent()
        {
            _servicesManager.Initialize();
            _servicesManager.SetAnalyticsConsent(true);
            
            string trackedEventName = null;
            _servicesManager.OnEventTracked += (name, _) => trackedEventName = name;
            
            _servicesManager.TrackEvent(AnalyticsEvents.TutorialStarted);
            
            Assert.AreEqual(AnalyticsEvents.TutorialStarted, trackedEventName);
        }

        /// <summary>
        /// Test that TrackEvent includes parameters.
        /// **Validates: Requirements R31.5**
        /// </summary>
        [Test]
        public void TrackEvent_IncludesParameters()
        {
            _servicesManager.Initialize();
            _servicesManager.SetAnalyticsConsent(true);
            
            var parameters = AnalyticsEvents.EventAcceptedParams("Kids Birthday", "$500-$2000", 1);
            _servicesManager.TrackEvent(AnalyticsEvents.EventAccepted, parameters);
            
            var events = _servicesManager.GetEventsByName(AnalyticsEvents.EventAccepted);
            Assert.AreEqual(1, events.Count);
            Assert.AreEqual("Kids Birthday", events[0].Parameters["event_type"]);
            Assert.AreEqual("$500-$2000", events[0].Parameters["budget_range"]);
            Assert.AreEqual(1, events[0].Parameters["stage"]);
        }

        /// <summary>
        /// Test that TrackEvent rejects invalid event names.
        /// **Validates: Requirements R31.5**
        /// </summary>
        [Test]
        public void TrackEvent_RejectsInvalidEventNames()
        {
            _servicesManager.Initialize();
            _servicesManager.SetAnalyticsConsent(true);
            
            _servicesManager.TrackEvent("invalid_event_name");
            
            Assert.AreEqual(0, _servicesManager.GetEventCount());
        }

        /// <summary>
        /// Test that TrackEvent handles null event name.
        /// **Validates: Requirements R31.5**
        /// </summary>
        [Test]
        public void TrackEvent_HandlesNullEventName()
        {
            _servicesManager.Initialize();
            _servicesManager.SetAnalyticsConsent(true);
            
            _servicesManager.TrackEvent(null);
            
            Assert.AreEqual(0, _servicesManager.GetEventCount());
        }

        /// <summary>
        /// Test that TrackEvent handles empty event name.
        /// **Validates: Requirements R31.5**
        /// </summary>
        [Test]
        public void TrackEvent_HandlesEmptyEventName()
        {
            _servicesManager.Initialize();
            _servicesManager.SetAnalyticsConsent(true);
            
            _servicesManager.TrackEvent("");
            
            Assert.AreEqual(0, _servicesManager.GetEventCount());
        }

        #endregion

        #region Remote Config Tests (R31.2)

        /// <summary>
        /// Test that GetRemoteConfig returns default when not initialized.
        /// **Validates: Requirements R31.2**
        /// </summary>
        [Test]
        public void GetRemoteConfig_ReturnsDefaultWhenNotInitialized()
        {
            var result = _servicesManager.GetRemoteConfig("some_key", 42);
            
            Assert.AreEqual(42, result);
        }

        /// <summary>
        /// Test that GetRemoteConfig returns cached value when initialized.
        /// **Validates: Requirements R31.2**
        /// </summary>
        [Test]
        public void GetRemoteConfig_ReturnsCachedValueWhenInitialized()
        {
            _servicesManager.Initialize();
            
            var result = _servicesManager.GetRemoteConfig("inquiry_interval_stage1_min", 0f);
            
            Assert.AreEqual(8f, result);
        }

        /// <summary>
        /// Test that GetRemoteConfig returns default for unknown key.
        /// **Validates: Requirements R31.2**
        /// </summary>
        [Test]
        public void GetRemoteConfig_ReturnsDefaultForUnknownKey()
        {
            _servicesManager.Initialize();
            
            var result = _servicesManager.GetRemoteConfig("unknown_key", "default_value");
            
            Assert.AreEqual("default_value", result);
        }

        /// <summary>
        /// Test that SetRemoteConfig updates cached value.
        /// **Validates: Requirements R31.2**
        /// </summary>
        [Test]
        public void SetRemoteConfig_UpdatesCachedValue()
        {
            _servicesManager.Initialize();
            _servicesManager.SetRemoteConfig("custom_key", 123);
            
            var result = _servicesManager.GetRemoteConfig("custom_key", 0);
            
            Assert.AreEqual(123, result);
        }

        /// <summary>
        /// Test that GetRemoteConfig handles type conversion.
        /// **Validates: Requirements R31.2**
        /// </summary>
        [Test]
        public void GetRemoteConfig_HandlesTypeConversion()
        {
            _servicesManager.Initialize();
            _servicesManager.SetRemoteConfig("int_value", 42);
            
            // Request as float
            var result = _servicesManager.GetRemoteConfig("int_value", 0f);
            
            Assert.AreEqual(42f, result);
        }

        /// <summary>
        /// Test that GetRemoteConfig returns default on type mismatch.
        /// **Validates: Requirements R31.2**
        /// </summary>
        [Test]
        public void GetRemoteConfig_ReturnsDefaultOnTypeMismatch()
        {
            _servicesManager.Initialize();
            _servicesManager.SetRemoteConfig("string_value", "not_a_number");
            
            // Request as int - should return default
            var result = _servicesManager.GetRemoteConfig("string_value", 99);
            
            Assert.AreEqual(99, result);
        }

        #endregion

        #region LogException Tests (R31.3)

        /// <summary>
        /// Test that LogException handles null exception.
        /// **Validates: Requirements R31.3**
        /// </summary>
        [Test]
        public void LogException_HandlesNullException()
        {
            Assert.DoesNotThrow(() => _servicesManager.LogException(null));
        }

        /// <summary>
        /// Test that LogException logs exception details.
        /// **Validates: Requirements R31.3**
        /// </summary>
        [Test]
        public void LogException_LogsExceptionDetails()
        {
            var exception = new InvalidOperationException("Test error");
            
            _servicesManager.LogException(exception);
            
            var events = _servicesManager.GetEventsByName("_exception");
            Assert.AreEqual(1, events.Count);
            Assert.AreEqual("InvalidOperationException", events[0].Parameters["type"]);
            Assert.AreEqual("Test error", events[0].Parameters["message"]);
        }

        #endregion

        #region Event Log Tests

        /// <summary>
        /// Test that GetEventLog returns copy of events.
        /// </summary>
        [Test]
        public void GetEventLog_ReturnsCopyOfEvents()
        {
            _servicesManager.Initialize();
            _servicesManager.SetAnalyticsConsent(true);
            _servicesManager.TrackEvent(AnalyticsEvents.TutorialStarted);
            
            var log1 = _servicesManager.GetEventLog();
            var log2 = _servicesManager.GetEventLog();
            
            Assert.AreNotSame(log1, log2);
            Assert.AreEqual(log1.Count, log2.Count);
        }

        /// <summary>
        /// Test that ClearEventLog clears all events.
        /// </summary>
        [Test]
        public void ClearEventLog_ClearsAllEvents()
        {
            _servicesManager.Initialize();
            _servicesManager.SetAnalyticsConsent(true);
            _servicesManager.TrackEvent(AnalyticsEvents.TutorialStarted);
            
            _servicesManager.ClearEventLog();
            
            Assert.AreEqual(0, _servicesManager.GetEventCount());
        }

        #endregion

        #region Analytics Events Validation Tests (R31.5)

        /// <summary>
        /// Test that all defined analytics events are valid.
        /// **Validates: Requirements R31.5**
        /// </summary>
        [Test]
        public void AnalyticsEvents_AllDefinedEventsAreValid()
        {
            Assert.IsTrue(AnalyticsEvents.IsValidEvent(AnalyticsEvents.TutorialStarted));
            Assert.IsTrue(AnalyticsEvents.IsValidEvent(AnalyticsEvents.TutorialCompleted));
            Assert.IsTrue(AnalyticsEvents.IsValidEvent(AnalyticsEvents.TutorialSkipped));
            Assert.IsTrue(AnalyticsEvents.IsValidEvent(AnalyticsEvents.EventAccepted));
            Assert.IsTrue(AnalyticsEvents.IsValidEvent(AnalyticsEvents.EventCompleted));
            Assert.IsTrue(AnalyticsEvents.IsValidEvent(AnalyticsEvents.EventFailed));
            Assert.IsTrue(AnalyticsEvents.IsValidEvent(AnalyticsEvents.StageAdvanced));
            Assert.IsTrue(AnalyticsEvents.IsValidEvent(AnalyticsEvents.Stage3PathChosen));
            Assert.IsTrue(AnalyticsEvents.IsValidEvent(AnalyticsEvents.VendorBooked));
            Assert.IsTrue(AnalyticsEvents.IsValidEvent(AnalyticsEvents.VenueBooked));
            Assert.IsTrue(AnalyticsEvents.IsValidEvent(AnalyticsEvents.PurchaseInitiated));
            Assert.IsTrue(AnalyticsEvents.IsValidEvent(AnalyticsEvents.PurchaseCompleted));
            Assert.IsTrue(AnalyticsEvents.IsValidEvent(AnalyticsEvents.PurchaseFailed));
            Assert.IsTrue(AnalyticsEvents.IsValidEvent(AnalyticsEvents.AdWatched));
            Assert.IsTrue(AnalyticsEvents.IsValidEvent(AnalyticsEvents.AdSkipped));
            Assert.IsTrue(AnalyticsEvents.IsValidEvent(AnalyticsEvents.FamilyHelpUsed));
            Assert.IsTrue(AnalyticsEvents.IsValidEvent(AnalyticsEvents.FinancialCrisis));
            Assert.IsTrue(AnalyticsEvents.IsValidEvent(AnalyticsEvents.SessionStart));
            Assert.IsTrue(AnalyticsEvents.IsValidEvent(AnalyticsEvents.SessionEnd));
        }

        /// <summary>
        /// Test that GetAllEvents returns all 19 events.
        /// **Validates: Requirements R31.5**
        /// </summary>
        [Test]
        public void AnalyticsEvents_GetAllEventsReturns19Events()
        {
            var allEvents = AnalyticsEvents.GetAllEvents().ToList();
            
            Assert.AreEqual(19, allEvents.Count);
        }

        #endregion

        #region Parameter Builder Tests (R31.5)

        /// <summary>
        /// Test EventAcceptedParams creates correct parameters.
        /// **Validates: Requirements R31.5**
        /// </summary>
        [Test]
        public void EventAcceptedParams_CreatesCorrectParameters()
        {
            var parameters = AnalyticsEvents.EventAcceptedParams("Kids Birthday", "$500-$2000", 1);
            
            Assert.AreEqual("Kids Birthday", parameters["event_type"]);
            Assert.AreEqual("$500-$2000", parameters["budget_range"]);
            Assert.AreEqual(1, parameters["stage"]);
        }

        /// <summary>
        /// Test EventCompletedParams creates correct parameters.
        /// **Validates: Requirements R31.5**
        /// </summary>
        [Test]
        public void EventCompletedParams_CreatesCorrectParameters()
        {
            var parameters = AnalyticsEvents.EventCompletedParams(85.5f, 250f, "Adult Birthday");
            
            Assert.AreEqual(85.5f, parameters["satisfaction_score"]);
            Assert.AreEqual(250f, parameters["profit"]);
            Assert.AreEqual("Adult Birthday", parameters["event_type"]);
        }

        /// <summary>
        /// Test StageAdvancedParams creates correct parameters.
        /// **Validates: Requirements R31.5**
        /// </summary>
        [Test]
        public void StageAdvancedParams_CreatesCorrectParameters()
        {
            var parameters = AnalyticsEvents.StageAdvancedParams(2, 3600f);
            
            Assert.AreEqual(2, parameters["new_stage"]);
            Assert.AreEqual(3600f, parameters["total_playtime"]);
        }

        /// <summary>
        /// Test VendorBookedParams creates correct parameters.
        /// **Validates: Requirements R31.5**
        /// </summary>
        [Test]
        public void VendorBookedParams_CreatesCorrectParameters()
        {
            var parameters = AnalyticsEvents.VendorBookedParams(
                VendorCategory.Caterer, 
                VendorTier.Premium, 
                3);
            
            Assert.AreEqual("Caterer", parameters["vendor_category"]);
            Assert.AreEqual("Premium", parameters["vendor_tier"]);
            Assert.AreEqual(3, parameters["relationship_level"]);
        }

        /// <summary>
        /// Test VenueBookedParams creates correct parameters.
        /// **Validates: Requirements R31.5**
        /// </summary>
        [Test]
        public void VenueBookedParams_CreatesCorrectParameters()
        {
            var parameters = AnalyticsEvents.VenueBookedParams(VenueType.CommunityCenter, 0.75f);
            
            Assert.AreEqual("CommunityCenter", parameters["venue_type"]);
            Assert.AreEqual(0.75f, parameters["capacity_utilization"]);
        }

        /// <summary>
        /// Test AdWatchedParams creates correct parameters.
        /// **Validates: Requirements R31.5**
        /// </summary>
        [Test]
        public void AdWatchedParams_CreatesCorrectParameters()
        {
            var parameters = AnalyticsEvents.AdWatchedParams("EmergencyFunding", "currency");
            
            Assert.AreEqual("EmergencyFunding", parameters["ad_placement"]);
            Assert.AreEqual("currency", parameters["reward_type"]);
        }

        /// <summary>
        /// Test SessionStartParams creates correct parameters.
        /// **Validates: Requirements R31.5**
        /// </summary>
        [Test]
        public void SessionStartParams_CreatesCorrectParameters()
        {
            var parameters = AnalyticsEvents.SessionStartParams(7200f);
            
            Assert.AreEqual(7200f, parameters["time_since_last_session"]);
        }

        /// <summary>
        /// Test SessionEndParams creates correct parameters.
        /// **Validates: Requirements R31.5**
        /// </summary>
        [Test]
        public void SessionEndParams_CreatesCorrectParameters()
        {
            var parameters = AnalyticsEvents.SessionEndParams(1800f);
            
            Assert.AreEqual(1800f, parameters["session_duration"]);
        }

        #endregion
    }
}
