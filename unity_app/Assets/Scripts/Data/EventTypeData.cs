using System;
using System.Collections.Generic;
using UnityEngine;
using EventPlannerSim.Core;

namespace EventPlannerSim.Data
{
    /// <summary>
    /// ScriptableObject defining an event type with its characteristics.
    /// Requirements: R6.1-R6.17
    /// </summary>
    [CreateAssetMenu(fileName = "EventType", menuName = "EventPlanner/EventType")]
    public class EventTypeData : ScriptableObject
    {
        [Header("Identity")]
        /// <summary>
        /// Unique identifier for this event type.
        /// </summary>
        public string eventTypeId;

        /// <summary>
        /// Display name shown to the player.
        /// </summary>
        public string displayName;

        [Header("Complexity and Requirements")]
        /// <summary>
        /// Complexity level affecting scheduling and difficulty.
        /// </summary>
        public EventComplexity complexity;

        /// <summary>
        /// Minimum stage required to receive this event type.
        /// </summary>
        public int minStageRequired = 1;

        [Header("Budget")]
        /// <summary>
        /// Minimum budget for this event type.
        /// </summary>
        public int minBudget;

        /// <summary>
        /// Maximum budget for this event type.
        /// </summary>
        public int maxBudget;

        [Header("Sub-Categories")]
        /// <summary>
        /// List of possible sub-categories for this event type.
        /// </summary>
        public List<string> subCategories = new List<string>();

        [Header("Vendor Requirements")]
        /// <summary>
        /// Vendor categories that must be booked for this event type.
        /// </summary>
        public List<VendorCategory> requiredVendors = new List<VendorCategory>();

        /// <summary>
        /// Vendor categories that are optional but recommended.
        /// </summary>
        public List<VendorCategory> optionalVendors = new List<VendorCategory>();

        [Header("Recommended Budget Split")]
        /// <summary>
        /// Recommended budget percentages for each category.
        /// Order: Venue, Catering, Entertainment, Decorations, Staffing, Contingency
        /// </summary>
        [Tooltip("Order: Venue, Catering, Entertainment, Decorations, Staffing, Contingency")]
        public float[] recommendedBudgetSplit = new float[6] { 0.30f, 0.30f, 0.15f, 0.15f, 0.05f, 0.05f };

        /// <summary>
        /// Gets the recommended venue budget percentage.
        /// </summary>
        public float RecommendedVenuePercent => recommendedBudgetSplit.Length > 0 ? recommendedBudgetSplit[0] : 0.30f;

        /// <summary>
        /// Gets the recommended catering budget percentage.
        /// </summary>
        public float RecommendedCateringPercent => recommendedBudgetSplit.Length > 1 ? recommendedBudgetSplit[1] : 0.30f;

        /// <summary>
        /// Gets the recommended entertainment budget percentage.
        /// </summary>
        public float RecommendedEntertainmentPercent => recommendedBudgetSplit.Length > 2 ? recommendedBudgetSplit[2] : 0.15f;

        /// <summary>
        /// Gets the recommended decorations budget percentage.
        /// </summary>
        public float RecommendedDecorationsPercent => recommendedBudgetSplit.Length > 3 ? recommendedBudgetSplit[3] : 0.15f;

        /// <summary>
        /// Gets the recommended staffing budget percentage.
        /// </summary>
        public float RecommendedStaffingPercent => recommendedBudgetSplit.Length > 4 ? recommendedBudgetSplit[4] : 0.05f;

        /// <summary>
        /// Gets the recommended contingency budget percentage.
        /// </summary>
        public float RecommendedContingencyPercent => recommendedBudgetSplit.Length > 5 ? recommendedBudgetSplit[5] : 0.05f;

        /// <summary>
        /// Generates a random budget within the event type's range.
        /// </summary>
        public int GenerateRandomBudget()
        {
            return UnityEngine.Random.Range(minBudget, maxBudget + 1);
        }

        /// <summary>
        /// Gets a random sub-category for this event type.
        /// </summary>
        public string GetRandomSubCategory()
        {
            if (subCategories == null || subCategories.Count == 0)
                return displayName;

            return subCategories[UnityEngine.Random.Range(0, subCategories.Count)];
        }

        /// <summary>
        /// Checks if this event type is available at the given stage.
        /// </summary>
        public bool IsAvailableAtStage(int stage)
        {
            return stage >= minStageRequired;
        }

        /// <summary>
        /// Gets the scheduling range in days based on complexity.
        /// Low = 3-7 days, Medium = 7-14 days, High = 14-21 days, VeryHigh = 21-30 days
        /// </summary>
        public (int min, int max) GetSchedulingRange()
        {
            return complexity switch
            {
                EventComplexity.Low => (3, 7),
                EventComplexity.Medium => (7, 14),
                EventComplexity.High => (14, 21),
                EventComplexity.VeryHigh => (21, 30),
                _ => (7, 14)
            };
        }
    }
}
