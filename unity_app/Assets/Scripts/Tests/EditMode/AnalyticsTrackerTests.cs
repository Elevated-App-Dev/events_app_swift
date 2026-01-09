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
    /// Unit tests for AnalyticsTracker.
    /// Validates: Requirements R31.5
    /// </summary>
    [TestFixture]
    public class AnalyticsTrackerTests
    {
        private UnityServicesManagerImpl _servicesManager;
        private AnalyticsTracker _tracker;

        [SetUp]
        public void Setup()
        {
            _servicesManager = new UnityServicesManagerImpl();
            _servicesManager.Initialize();
            _servicesManager.SetAnalyticsConsent(true);
            _tracker = new AnalyticsTracker(_servicesManager);
        }

        #region Constructor Tests

        /// <summary>
        /// Test that constructor throws on null services manager.
        /// </summary>
        [Test]
        public void Constructor_ThrowsOnNullServicesManager()
        {
            Assert.Throws<ArgumentNullException>(() => new AnalyticsTracker(null));
        }

        #endregion

        #region Session Event Tests

        /// <summary>
        /// Test that TrackSessionStart tracks session_start event.
        /// **Validates: Requirements R31.5**
        /// </summary>
        [Test]
        public void TrackSessionStart_TracksSessionStartEvent()
        {
            _tracker.TrackSessionStart();
            
            var events = _servicesManager.GetEventsByName(AnalyticsEvents.SessionStart);
            Assert.AreEqual(1, events.Count);
            Assert.IsTrue(events[0].Parameters.ContainsKey("time_since_last_session"));
        }

        /// <summary>
        /// Test that TrackSessionEnd tracks session_end event.
        /// **Validates: Requirements R31.5**
        /// </summary>
        [Test]
        public void TrackSessionEnd_TracksSessionEndEvent()
        {
            _tracker.TrackSessionEnd();
            
            var events = _servicesManager.GetEventsByName(AnalyticsEvents.SessionEnd);
            Assert.AreEqual(1, events.Count);
            Assert.IsTrue(events[0].Parameters.ContainsKey("session_duration"));
        }

        #endregion

        #region Tutorial Event Tests

        /// <summary>
        /// Test that TrackTutorialStarted tracks tutorial_started event.
        /// **Validates: Requirements R31.5**
        /// </summary>
        [Test]
        public void TrackTutorialStarted_TracksTutorialStartedEvent()
        {
            _tracker.TrackTutorialStarted();
            
            var events = _servicesManager.GetEventsByName(AnalyticsEvents.TutorialStarted);
            Assert.AreEqual(1, events.Count);
        }

        /// <summary>
        /// Test that TrackTutorialCompleted tracks tutorial_completed event.
        /// **Validates: Requirements R31.5**
        /// </summary>
        [Test]
        public void TrackTutorialCompleted_TracksTutorialCompletedEvent()
        {
            _tracker.TrackTutorialCompleted();
            
            var events = _servicesManager.GetEventsByName(AnalyticsEvents.TutorialCompleted);
            Assert.AreEqual(1, events.Count);
        }

        /// <summary>
        /// Test that TrackTutorialSkipped tracks tutorial_skipped event.
        /// **Validates: Requirements R31.5**
        /// </summary>
        [Test]
        public void TrackTutorialSkipped_TracksTutorialSkippedEvent()
        {
            _tracker.TrackTutorialSkipped();
            
            var events = _servicesManager.GetEventsByName(AnalyticsEvents.TutorialSkipped);
            Assert.AreEqual(1, events.Count);
        }

        #endregion

        #region Event Lifecycle Tests

        /// <summary>
        /// Test that TrackEventAccepted tracks event_accepted event.
        /// **Validates: Requirements R31.5**
        /// </summary>
        [Test]
        public void TrackEventAccepted_TracksEventAcceptedEvent()
        {
            _tracker.TrackEventAccepted("Kids Birthday", 500f, 2000f, 1);
            
            var events = _servicesManager.GetEventsByName(AnalyticsEvents.EventAccepted);
            Assert.AreEqual(1, events.Count);
            Assert.AreEqual("Kids Birthday", events[0].Parameters["event_type"]);
            Assert.AreEqual("$500-$2,000", events[0].Parameters["budget_range"]);
            Assert.AreEqual(1, events[0].Parameters["stage"]);
        }

        /// <summary>
        /// Test that TrackEventCompleted tracks event_completed event.
        /// **Validates: Requirements R31.5**
        /// </summary>
        [Test]
        public void TrackEventCompleted_TracksEventCompletedEvent()
        {
            _tracker.TrackEventCompleted(85.5f, 250f, "Adult Birthday");
            
            var events = _servicesManager.GetEventsByName(AnalyticsEvents.EventCompleted);
            Assert.AreEqual(1, events.Count);
            Assert.AreEqual(85.5f, events[0].Parameters["satisfaction_score"]);
            Assert.AreEqual(250f, events[0].Parameters["profit"]);
            Assert.AreEqual("Adult Birthday", events[0].Parameters["event_type"]);
        }

        /// <summary>
        /// Test that TrackEventFailed tracks event_failed event.
        /// **Validates: Requirements R31.5**
        /// </summary>
        [Test]
        public void TrackEventFailed_TracksEventFailedEvent()
        {
            _tracker.TrackEventFailed(45.0f, "Corporate Meeting");
            
            var events = _servicesManager.GetEventsByName(AnalyticsEvents.EventFailed);
            Assert.AreEqual(1, events.Count);
            Assert.AreEqual(45.0f, events[0].Parameters["satisfaction_score"]);
            Assert.AreEqual("Corporate Meeting", events[0].Parameters["event_type"]);
        }

        #endregion

        #region Progression Event Tests

        /// <summary>
        /// Test that TrackStageAdvanced tracks stage_advanced event.
        /// **Validates: Requirements R31.5**
        /// </summary>
        [Test]
        public void TrackStageAdvanced_TracksStageAdvancedEvent()
        {
            _tracker.TrackStageAdvanced(2, 3600f);
            
            var events = _servicesManager.GetEventsByName(AnalyticsEvents.StageAdvanced);
            Assert.AreEqual(1, events.Count);
            Assert.AreEqual(2, events[0].Parameters["new_stage"]);
            Assert.AreEqual(3600f, events[0].Parameters["total_playtime"]);
        }

        /// <summary>
        /// Test that TrackStageAdvanced with BusinessStage tracks correctly.
        /// **Validates: Requirements R31.5**
        /// </summary>
        [Test]
        public void TrackStageAdvanced_WithBusinessStage_TracksCorrectly()
        {
            _tracker.TrackStageAdvanced(BusinessStage.Employee, 7200f);
            
            var events = _servicesManager.GetEventsByName(AnalyticsEvents.StageAdvanced);
            Assert.AreEqual(1, events.Count);
            Assert.AreEqual(2, events[0].Parameters["new_stage"]); // Employee = 2
        }

        /// <summary>
        /// Test that TrackStage3PathChosen tracks stage_3_path_chosen event.
        /// **Validates: Requirements R31.5**
        /// </summary>
        [Test]
        public void TrackStage3PathChosen_TracksStage3PathChosenEvent()
        {
            _tracker.TrackStage3PathChosen(CareerPath.Entrepreneur);
            
            var events = _servicesManager.GetEventsByName(AnalyticsEvents.Stage3PathChosen);
            Assert.AreEqual(1, events.Count);
            Assert.AreEqual("Entrepreneur", events[0].Parameters["path_choice"]);
        }

        #endregion

        #region Booking Event Tests

        /// <summary>
        /// Test that TrackVendorBooked tracks vendor_booked event.
        /// **Validates: Requirements R31.5**
        /// </summary>
        [Test]
        public void TrackVendorBooked_TracksVendorBookedEvent()
        {
            _tracker.TrackVendorBooked(VendorCategory.Caterer, VendorTier.Premium, 3);
            
            var events = _servicesManager.GetEventsByName(AnalyticsEvents.VendorBooked);
            Assert.AreEqual(1, events.Count);
            Assert.AreEqual("Caterer", events[0].Parameters["vendor_category"]);
            Assert.AreEqual("Premium", events[0].Parameters["vendor_tier"]);
            Assert.AreEqual(3, events[0].Parameters["relationship_level"]);
        }

        /// <summary>
        /// Test that TrackVenueBooked tracks venue_booked event.
        /// **Validates: Requirements R31.5**
        /// </summary>
        [Test]
        public void TrackVenueBooked_TracksVenueBookedEvent()
        {
            _tracker.TrackVenueBooked(VenueType.CommunityCenter, 0.75f);
            
            var events = _servicesManager.GetEventsByName(AnalyticsEvents.VenueBooked);
            Assert.AreEqual(1, events.Count);
            Assert.AreEqual("CommunityCenter", events[0].Parameters["venue_type"]);
            Assert.AreEqual(0.75f, events[0].Parameters["capacity_utilization"]);
        }

        #endregion

        #region Monetization Event Tests

        /// <summary>
        /// Test that TrackPurchaseInitiated tracks purchase_initiated event.
        /// **Validates: Requirements R31.5**
        /// </summary>
        [Test]
        public void TrackPurchaseInitiated_TracksPurchaseInitiatedEvent()
        {
            _tracker.TrackPurchaseInitiated("com.game.currency_small");
            
            var events = _servicesManager.GetEventsByName(AnalyticsEvents.PurchaseInitiated);
            Assert.AreEqual(1, events.Count);
            Assert.AreEqual("com.game.currency_small", events[0].Parameters["product_id"]);
        }

        /// <summary>
        /// Test that TrackPurchaseCompleted tracks purchase_completed event.
        /// **Validates: Requirements R31.5**
        /// </summary>
        [Test]
        public void TrackPurchaseCompleted_TracksPurchaseCompletedEvent()
        {
            _tracker.TrackPurchaseCompleted("com.game.currency_small", 0.99f);
            
            var events = _servicesManager.GetEventsByName(AnalyticsEvents.PurchaseCompleted);
            Assert.AreEqual(1, events.Count);
            Assert.AreEqual("com.game.currency_small", events[0].Parameters["product_id"]);
            Assert.AreEqual(0.99f, events[0].Parameters["price"]);
        }

        /// <summary>
        /// Test that TrackPurchaseFailed tracks purchase_failed event.
        /// **Validates: Requirements R31.5**
        /// </summary>
        [Test]
        public void TrackPurchaseFailed_TracksPurchaseFailedEvent()
        {
            _tracker.TrackPurchaseFailed("com.game.currency_small", "user_cancelled");
            
            var events = _servicesManager.GetEventsByName(AnalyticsEvents.PurchaseFailed);
            Assert.AreEqual(1, events.Count);
            Assert.AreEqual("com.game.currency_small", events[0].Parameters["product_id"]);
            Assert.AreEqual("user_cancelled", events[0].Parameters["error_type"]);
        }

        /// <summary>
        /// Test that TrackAdWatched tracks ad_watched event.
        /// **Validates: Requirements R31.5**
        /// </summary>
        [Test]
        public void TrackAdWatched_TracksAdWatchedEvent()
        {
            _tracker.TrackAdWatched("EmergencyFunding", "currency");
            
            var events = _servicesManager.GetEventsByName(AnalyticsEvents.AdWatched);
            Assert.AreEqual(1, events.Count);
            Assert.AreEqual("EmergencyFunding", events[0].Parameters["ad_placement"]);
            Assert.AreEqual("currency", events[0].Parameters["reward_type"]);
        }

        /// <summary>
        /// Test that TrackAdWatched with AdPlacement enum tracks correctly.
        /// **Validates: Requirements R31.5**
        /// </summary>
        [Test]
        public void TrackAdWatched_WithAdPlacement_TracksCorrectly()
        {
            _tracker.TrackAdWatched(AdPlacement.EmergencyFunding, "currency");
            
            var events = _servicesManager.GetEventsByName(AnalyticsEvents.AdWatched);
            Assert.AreEqual(1, events.Count);
            Assert.AreEqual("EmergencyFunding", events[0].Parameters["ad_placement"]);
        }

        /// <summary>
        /// Test that TrackAdSkipped tracks ad_skipped event.
        /// **Validates: Requirements R31.5**
        /// </summary>
        [Test]
        public void TrackAdSkipped_TracksAdSkippedEvent()
        {
            _tracker.TrackAdSkipped("OvertimeHours");
            
            var events = _servicesManager.GetEventsByName(AnalyticsEvents.AdSkipped);
            Assert.AreEqual(1, events.Count);
            Assert.AreEqual("OvertimeHours", events[0].Parameters["ad_placement"]);
        }

        #endregion

        #region Financial Event Tests

        /// <summary>
        /// Test that TrackFamilyHelpUsed tracks family_help_used event.
        /// **Validates: Requirements R31.5**
        /// </summary>
        [Test]
        public void TrackFamilyHelpUsed_TracksFamilyHelpUsedEvent()
        {
            _tracker.TrackFamilyHelpUsed(2);
            
            var events = _servicesManager.GetEventsByName(AnalyticsEvents.FamilyHelpUsed);
            Assert.AreEqual(1, events.Count);
            Assert.AreEqual(2, events[0].Parameters["request_number"]);
        }

        /// <summary>
        /// Test that TrackFinancialCrisis tracks financial_crisis event.
        /// **Validates: Requirements R31.5**
        /// </summary>
        [Test]
        public void TrackFinancialCrisis_TracksFinancialCrisisEvent()
        {
            _tracker.TrackFinancialCrisis(1, 5);
            
            var events = _servicesManager.GetEventsByName(AnalyticsEvents.FinancialCrisis);
            Assert.AreEqual(1, events.Count);
            Assert.AreEqual(1, events[0].Parameters["stage"]);
            Assert.AreEqual(5, events[0].Parameters["total_events_completed"]);
        }

        /// <summary>
        /// Test that TrackFinancialCrisis with BusinessStage tracks correctly.
        /// **Validates: Requirements R31.5**
        /// </summary>
        [Test]
        public void TrackFinancialCrisis_WithBusinessStage_TracksCorrectly()
        {
            _tracker.TrackFinancialCrisis(BusinessStage.Solo, 3);
            
            var events = _servicesManager.GetEventsByName(AnalyticsEvents.FinancialCrisis);
            Assert.AreEqual(1, events.Count);
            Assert.AreEqual(1, events[0].Parameters["stage"]); // Solo = 1
        }

        #endregion
    }
}
