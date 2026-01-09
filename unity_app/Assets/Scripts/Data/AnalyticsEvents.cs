using System;
using System.Collections.Generic;
using EventPlannerSim.Core;

namespace EventPlannerSim.Data
{
    /// <summary>
    /// Defines all analytics events from the Analytics Events List.
    /// Requirements: R31.5
    /// </summary>
    public static class AnalyticsEvents
    {
        // Tutorial Events
        public const string TutorialStarted = "tutorial_started";
        public const string TutorialCompleted = "tutorial_completed";
        public const string TutorialSkipped = "tutorial_skipped";

        // Event Lifecycle Events
        public const string EventAccepted = "event_accepted";
        public const string EventCompleted = "event_completed";
        public const string EventFailed = "event_failed";

        // Progression Events
        public const string StageAdvanced = "stage_advanced";
        public const string Stage3PathChosen = "stage_3_path_chosen";

        // Booking Events
        public const string VendorBooked = "vendor_booked";
        public const string VenueBooked = "venue_booked";

        // Monetization Events
        public const string PurchaseInitiated = "purchase_initiated";
        public const string PurchaseCompleted = "purchase_completed";
        public const string PurchaseFailed = "purchase_failed";
        public const string AdWatched = "ad_watched";
        public const string AdSkipped = "ad_skipped";

        // Financial Events
        public const string FamilyHelpUsed = "family_help_used";
        public const string FinancialCrisis = "financial_crisis";

        // Session Events
        public const string SessionStart = "session_start";
        public const string SessionEnd = "session_end";

        /// <summary>
        /// Set of all valid event names for validation.
        /// </summary>
        private static readonly HashSet<string> ValidEvents = new HashSet<string>
        {
            TutorialStarted,
            TutorialCompleted,
            TutorialSkipped,
            EventAccepted,
            EventCompleted,
            EventFailed,
            StageAdvanced,
            Stage3PathChosen,
            VendorBooked,
            VenueBooked,
            PurchaseInitiated,
            PurchaseCompleted,
            PurchaseFailed,
            AdWatched,
            AdSkipped,
            FamilyHelpUsed,
            FinancialCrisis,
            SessionStart,
            SessionEnd
        };

        /// <summary>
        /// Check if an event name is valid.
        /// </summary>
        public static bool IsValidEvent(string eventName)
        {
            return ValidEvents.Contains(eventName);
        }

        /// <summary>
        /// Get all valid event names.
        /// </summary>
        public static IEnumerable<string> GetAllEvents()
        {
            return ValidEvents;
        }

        #region Event Parameter Builders

        /// <summary>
        /// Create parameters for event_accepted event.
        /// </summary>
        public static Dictionary<string, object> EventAcceptedParams(
            string eventType,
            string budgetRange,
            int stage)
        {
            return new Dictionary<string, object>
            {
                { "event_type", eventType },
                { "budget_range", budgetRange },
                { "stage", stage }
            };
        }

        /// <summary>
        /// Create parameters for event_completed event.
        /// </summary>
        public static Dictionary<string, object> EventCompletedParams(
            float satisfactionScore,
            float profit,
            string eventType)
        {
            return new Dictionary<string, object>
            {
                { "satisfaction_score", satisfactionScore },
                { "profit", profit },
                { "event_type", eventType }
            };
        }

        /// <summary>
        /// Create parameters for event_failed event.
        /// </summary>
        public static Dictionary<string, object> EventFailedParams(
            float satisfactionScore,
            string eventType)
        {
            return new Dictionary<string, object>
            {
                { "satisfaction_score", satisfactionScore },
                { "event_type", eventType }
            };
        }

        /// <summary>
        /// Create parameters for stage_advanced event.
        /// </summary>
        public static Dictionary<string, object> StageAdvancedParams(
            int newStage,
            float totalPlaytime)
        {
            return new Dictionary<string, object>
            {
                { "new_stage", newStage },
                { "total_playtime", totalPlaytime }
            };
        }

        /// <summary>
        /// Create parameters for stage_3_path_chosen event.
        /// </summary>
        public static Dictionary<string, object> Stage3PathChosenParams(
            CareerPath pathChoice)
        {
            return new Dictionary<string, object>
            {
                { "path_choice", pathChoice.ToString() }
            };
        }

        /// <summary>
        /// Create parameters for vendor_booked event.
        /// </summary>
        public static Dictionary<string, object> VendorBookedParams(
            VendorCategory vendorCategory,
            VendorTier vendorTier,
            int relationshipLevel)
        {
            return new Dictionary<string, object>
            {
                { "vendor_category", vendorCategory.ToString() },
                { "vendor_tier", vendorTier.ToString() },
                { "relationship_level", relationshipLevel }
            };
        }

        /// <summary>
        /// Create parameters for venue_booked event.
        /// </summary>
        public static Dictionary<string, object> VenueBookedParams(
            VenueType venueType,
            float capacityUtilization)
        {
            return new Dictionary<string, object>
            {
                { "venue_type", venueType.ToString() },
                { "capacity_utilization", capacityUtilization }
            };
        }

        /// <summary>
        /// Create parameters for purchase_initiated event.
        /// </summary>
        public static Dictionary<string, object> PurchaseInitiatedParams(
            string productId)
        {
            return new Dictionary<string, object>
            {
                { "product_id", productId }
            };
        }

        /// <summary>
        /// Create parameters for purchase_completed event.
        /// </summary>
        public static Dictionary<string, object> PurchaseCompletedParams(
            string productId,
            float price)
        {
            return new Dictionary<string, object>
            {
                { "product_id", productId },
                { "price", price }
            };
        }

        /// <summary>
        /// Create parameters for purchase_failed event.
        /// </summary>
        public static Dictionary<string, object> PurchaseFailedParams(
            string productId,
            string errorType)
        {
            return new Dictionary<string, object>
            {
                { "product_id", productId },
                { "error_type", errorType }
            };
        }

        /// <summary>
        /// Create parameters for ad_watched event.
        /// </summary>
        public static Dictionary<string, object> AdWatchedParams(
            string adPlacement,
            string rewardType)
        {
            return new Dictionary<string, object>
            {
                { "ad_placement", adPlacement },
                { "reward_type", rewardType }
            };
        }

        /// <summary>
        /// Create parameters for ad_skipped event.
        /// </summary>
        public static Dictionary<string, object> AdSkippedParams(
            string adPlacement)
        {
            return new Dictionary<string, object>
            {
                { "ad_placement", adPlacement }
            };
        }

        /// <summary>
        /// Create parameters for family_help_used event.
        /// </summary>
        public static Dictionary<string, object> FamilyHelpUsedParams(
            int requestNumber)
        {
            return new Dictionary<string, object>
            {
                { "request_number", requestNumber }
            };
        }

        /// <summary>
        /// Create parameters for financial_crisis event.
        /// </summary>
        public static Dictionary<string, object> FinancialCrisisParams(
            int stage,
            int totalEventsCompleted)
        {
            return new Dictionary<string, object>
            {
                { "stage", stage },
                { "total_events_completed", totalEventsCompleted }
            };
        }

        /// <summary>
        /// Create parameters for session_start event.
        /// </summary>
        public static Dictionary<string, object> SessionStartParams(
            float timeSinceLastSession)
        {
            return new Dictionary<string, object>
            {
                { "time_since_last_session", timeSinceLastSession }
            };
        }

        /// <summary>
        /// Create parameters for session_end event.
        /// </summary>
        public static Dictionary<string, object> SessionEndParams(
            float sessionDuration)
        {
            return new Dictionary<string, object>
            {
                { "session_duration", sessionDuration }
            };
        }

        #endregion
    }
}
