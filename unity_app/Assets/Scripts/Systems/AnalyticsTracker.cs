using System;
using System.Collections.Generic;
using EventPlannerSim.Core;
using EventPlannerSim.Data;

namespace EventPlannerSim.Systems
{
    /// <summary>
    /// Helper class for tracking analytics events throughout the game.
    /// Provides convenience methods for common tracking scenarios.
    /// Requirements: R31.5
    /// </summary>
    public class AnalyticsTracker
    {
        private readonly IUnityServicesManager _servicesManager;
        private DateTime _sessionStartTime;
        private DateTime _lastSessionEndTime;

        public AnalyticsTracker(IUnityServicesManager servicesManager)
        {
            _servicesManager = servicesManager ?? throw new ArgumentNullException(nameof(servicesManager));
            _sessionStartTime = DateTime.UtcNow;
            _lastSessionEndTime = DateTime.MinValue;
        }

        #region Session Events

        /// <summary>
        /// Track session start event.
        /// Requirements: R31.5 - session_start
        /// </summary>
        public void TrackSessionStart()
        {
            float timeSinceLastSession = 0f;
            if (_lastSessionEndTime != DateTime.MinValue)
            {
                timeSinceLastSession = (float)(DateTime.UtcNow - _lastSessionEndTime).TotalSeconds;
            }

            _sessionStartTime = DateTime.UtcNow;
            
            _servicesManager.TrackEvent(
                AnalyticsEvents.SessionStart,
                AnalyticsEvents.SessionStartParams(timeSinceLastSession));
        }

        /// <summary>
        /// Track session end event.
        /// Requirements: R31.5 - session_end
        /// </summary>
        public void TrackSessionEnd()
        {
            float sessionDuration = (float)(DateTime.UtcNow - _sessionStartTime).TotalSeconds;
            _lastSessionEndTime = DateTime.UtcNow;
            
            _servicesManager.TrackEvent(
                AnalyticsEvents.SessionEnd,
                AnalyticsEvents.SessionEndParams(sessionDuration));
        }

        /// <summary>
        /// Set the last session end time (for loading from save data).
        /// </summary>
        public void SetLastSessionEndTime(DateTime lastEndTime)
        {
            _lastSessionEndTime = lastEndTime;
        }

        #endregion

        #region Tutorial Events

        /// <summary>
        /// Track tutorial started event.
        /// Requirements: R31.5 - tutorial_started
        /// </summary>
        public void TrackTutorialStarted()
        {
            _servicesManager.TrackEvent(AnalyticsEvents.TutorialStarted);
        }

        /// <summary>
        /// Track tutorial completed event.
        /// Requirements: R31.5 - tutorial_completed
        /// </summary>
        public void TrackTutorialCompleted()
        {
            _servicesManager.TrackEvent(AnalyticsEvents.TutorialCompleted);
        }

        /// <summary>
        /// Track tutorial skipped event.
        /// Requirements: R31.5 - tutorial_skipped
        /// </summary>
        public void TrackTutorialSkipped()
        {
            _servicesManager.TrackEvent(AnalyticsEvents.TutorialSkipped);
        }

        #endregion

        #region Event Lifecycle Events

        /// <summary>
        /// Track event accepted.
        /// Requirements: R31.5 - event_accepted
        /// </summary>
        public void TrackEventAccepted(string eventType, float minBudget, float maxBudget, int stage)
        {
            string budgetRange = $"${minBudget:N0}-${maxBudget:N0}";
            _servicesManager.TrackEvent(
                AnalyticsEvents.EventAccepted,
                AnalyticsEvents.EventAcceptedParams(eventType, budgetRange, stage));
        }

        /// <summary>
        /// Track event accepted with EventData.
        /// Requirements: R31.5 - event_accepted
        /// </summary>
        public void TrackEventAccepted(EventData eventData, int stage)
        {
            if (eventData == null) return;
            
            string eventType = eventData.subCategory ?? eventData.eventTypeId ?? "Unknown";
            string budgetRange = $"${eventData.budget.total:N0}";
            
            _servicesManager.TrackEvent(
                AnalyticsEvents.EventAccepted,
                AnalyticsEvents.EventAcceptedParams(eventType, budgetRange, stage));
        }

        /// <summary>
        /// Track event completed.
        /// Requirements: R31.5 - event_completed
        /// </summary>
        public void TrackEventCompleted(float satisfactionScore, float profit, string eventType)
        {
            _servicesManager.TrackEvent(
                AnalyticsEvents.EventCompleted,
                AnalyticsEvents.EventCompletedParams(satisfactionScore, profit, eventType));
        }

        /// <summary>
        /// Track event completed with EventData and results.
        /// Requirements: R31.5 - event_completed
        /// </summary>
        public void TrackEventCompleted(EventData eventData, float satisfactionScore, float profit)
        {
            if (eventData == null) return;
            
            string eventType = eventData.subCategory ?? eventData.eventTypeId ?? "Unknown";
            _servicesManager.TrackEvent(
                AnalyticsEvents.EventCompleted,
                AnalyticsEvents.EventCompletedParams(
                    satisfactionScore, 
                    profit, 
                    eventType));
        }

        /// <summary>
        /// Track event failed (satisfaction below threshold).
        /// Requirements: R31.5 - event_failed
        /// </summary>
        public void TrackEventFailed(float satisfactionScore, string eventType)
        {
            _servicesManager.TrackEvent(
                AnalyticsEvents.EventFailed,
                AnalyticsEvents.EventFailedParams(satisfactionScore, eventType));
        }

        /// <summary>
        /// Track event failed with EventData.
        /// Requirements: R31.5 - event_failed
        /// </summary>
        public void TrackEventFailed(EventData eventData, float satisfactionScore)
        {
            if (eventData == null) return;
            
            string eventType = eventData.subCategory ?? eventData.eventTypeId ?? "Unknown";
            _servicesManager.TrackEvent(
                AnalyticsEvents.EventFailed,
                AnalyticsEvents.EventFailedParams(satisfactionScore, eventType));
        }

        #endregion

        #region Progression Events

        /// <summary>
        /// Track stage advanced.
        /// Requirements: R31.5 - stage_advanced
        /// </summary>
        public void TrackStageAdvanced(int newStage, float totalPlaytimeSeconds)
        {
            _servicesManager.TrackEvent(
                AnalyticsEvents.StageAdvanced,
                AnalyticsEvents.StageAdvancedParams(newStage, totalPlaytimeSeconds));
        }

        /// <summary>
        /// Track stage advanced with BusinessStage.
        /// Requirements: R31.5 - stage_advanced
        /// </summary>
        public void TrackStageAdvanced(BusinessStage newStage, float totalPlaytimeSeconds)
        {
            _servicesManager.TrackEvent(
                AnalyticsEvents.StageAdvanced,
                AnalyticsEvents.StageAdvancedParams((int)newStage, totalPlaytimeSeconds));
        }

        /// <summary>
        /// Track Stage 3 path choice.
        /// Requirements: R31.5 - stage_3_path_chosen
        /// </summary>
        public void TrackStage3PathChosen(CareerPath pathChoice)
        {
            _servicesManager.TrackEvent(
                AnalyticsEvents.Stage3PathChosen,
                AnalyticsEvents.Stage3PathChosenParams(pathChoice));
        }

        #endregion

        #region Booking Events

        /// <summary>
        /// Track vendor booked.
        /// Requirements: R31.5 - vendor_booked
        /// </summary>
        public void TrackVendorBooked(VendorCategory category, VendorTier tier, int relationshipLevel)
        {
            _servicesManager.TrackEvent(
                AnalyticsEvents.VendorBooked,
                AnalyticsEvents.VendorBookedParams(category, tier, relationshipLevel));
        }

        /// <summary>
        /// Track vendor booked with VendorData.
        /// Requirements: R31.5 - vendor_booked
        /// </summary>
        public void TrackVendorBooked(VendorData vendor, int relationshipLevel)
        {
            if (vendor == null) return;
            
            _servicesManager.TrackEvent(
                AnalyticsEvents.VendorBooked,
                AnalyticsEvents.VendorBookedParams(vendor.category, vendor.tier, relationshipLevel));
        }

        /// <summary>
        /// Track venue booked.
        /// Requirements: R31.5 - venue_booked
        /// </summary>
        public void TrackVenueBooked(VenueType venueType, float capacityUtilization)
        {
            _servicesManager.TrackEvent(
                AnalyticsEvents.VenueBooked,
                AnalyticsEvents.VenueBookedParams(venueType, capacityUtilization));
        }

        /// <summary>
        /// Track venue booked with VenueData and guest count.
        /// Requirements: R31.5 - venue_booked
        /// </summary>
        public void TrackVenueBooked(VenueData venue, int guestCount)
        {
            if (venue == null) return;
            
            float capacityUtilization = venue.capacityMax > 0 
                ? (float)guestCount / venue.capacityMax 
                : 0f;
            
            _servicesManager.TrackEvent(
                AnalyticsEvents.VenueBooked,
                AnalyticsEvents.VenueBookedParams(venue.venueType, capacityUtilization));
        }

        #endregion

        #region Monetization Events

        /// <summary>
        /// Track purchase initiated.
        /// Requirements: R31.5 - purchase_initiated
        /// </summary>
        public void TrackPurchaseInitiated(string productId)
        {
            _servicesManager.TrackEvent(
                AnalyticsEvents.PurchaseInitiated,
                AnalyticsEvents.PurchaseInitiatedParams(productId));
        }

        /// <summary>
        /// Track purchase completed.
        /// Requirements: R31.5 - purchase_completed
        /// </summary>
        public void TrackPurchaseCompleted(string productId, float price)
        {
            _servicesManager.TrackEvent(
                AnalyticsEvents.PurchaseCompleted,
                AnalyticsEvents.PurchaseCompletedParams(productId, price));
        }

        /// <summary>
        /// Track purchase failed.
        /// Requirements: R31.5 - purchase_failed
        /// </summary>
        public void TrackPurchaseFailed(string productId, string errorType)
        {
            _servicesManager.TrackEvent(
                AnalyticsEvents.PurchaseFailed,
                AnalyticsEvents.PurchaseFailedParams(productId, errorType));
        }

        /// <summary>
        /// Track ad watched.
        /// Requirements: R31.5 - ad_watched
        /// </summary>
        public void TrackAdWatched(string adPlacement, string rewardType)
        {
            _servicesManager.TrackEvent(
                AnalyticsEvents.AdWatched,
                AnalyticsEvents.AdWatchedParams(adPlacement, rewardType));
        }

        /// <summary>
        /// Track ad watched with AdPlacement enum.
        /// Requirements: R31.5 - ad_watched
        /// </summary>
        public void TrackAdWatched(AdPlacement placement, string rewardType)
        {
            _servicesManager.TrackEvent(
                AnalyticsEvents.AdWatched,
                AnalyticsEvents.AdWatchedParams(placement.ToString(), rewardType));
        }

        /// <summary>
        /// Track ad skipped.
        /// Requirements: R31.5 - ad_skipped
        /// </summary>
        public void TrackAdSkipped(string adPlacement)
        {
            _servicesManager.TrackEvent(
                AnalyticsEvents.AdSkipped,
                AnalyticsEvents.AdSkippedParams(adPlacement));
        }

        /// <summary>
        /// Track ad skipped with AdPlacement enum.
        /// Requirements: R31.5 - ad_skipped
        /// </summary>
        public void TrackAdSkipped(AdPlacement placement)
        {
            _servicesManager.TrackEvent(
                AnalyticsEvents.AdSkipped,
                AnalyticsEvents.AdSkippedParams(placement.ToString()));
        }

        #endregion

        #region Financial Events

        /// <summary>
        /// Track family help used.
        /// Requirements: R31.5 - family_help_used
        /// </summary>
        public void TrackFamilyHelpUsed(int requestNumber)
        {
            _servicesManager.TrackEvent(
                AnalyticsEvents.FamilyHelpUsed,
                AnalyticsEvents.FamilyHelpUsedParams(requestNumber));
        }

        /// <summary>
        /// Track financial crisis.
        /// Requirements: R31.5 - financial_crisis
        /// </summary>
        public void TrackFinancialCrisis(int stage, int totalEventsCompleted)
        {
            _servicesManager.TrackEvent(
                AnalyticsEvents.FinancialCrisis,
                AnalyticsEvents.FinancialCrisisParams(stage, totalEventsCompleted));
        }

        /// <summary>
        /// Track financial crisis with BusinessStage.
        /// Requirements: R31.5 - financial_crisis
        /// </summary>
        public void TrackFinancialCrisis(BusinessStage stage, int totalEventsCompleted)
        {
            _servicesManager.TrackEvent(
                AnalyticsEvents.FinancialCrisis,
                AnalyticsEvents.FinancialCrisisParams((int)stage, totalEventsCompleted));
        }

        #endregion
    }
}
