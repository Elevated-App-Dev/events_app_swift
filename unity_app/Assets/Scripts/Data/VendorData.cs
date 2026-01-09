using System;
using UnityEngine;
using EventPlannerSim.Core;

namespace EventPlannerSim.Data
{
    /// <summary>
    /// ScriptableObject defining a vendor's attributes and capabilities.
    /// Requirements: R8.2
    /// </summary>
    [CreateAssetMenu(fileName = "Vendor", menuName = "EventPlanner/Vendor")]
    public class VendorData : ScriptableObject
    {
        [Header("Identity")]
        public string id;
        public string vendorName;
        public VendorCategory category;
        public VendorTier tier;
        public float basePrice;

        [Header("Visible Attributes")]
        [Range(1f, 5f)]
        public float qualityRating;
        public string specialty;
        public MapZone zone;

        [Header("Hidden Attributes (revealed through relationship)")]
        [Range(0f, 1f)]
        [Tooltip("Probability of showing up and performing as expected")]
        public float reliability;
        
        [Range(0f, 1f)]
        [Tooltip("Ability to handle last-minute changes")]
        public float flexibility;
    }
}
