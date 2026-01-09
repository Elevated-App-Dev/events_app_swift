using System;
using UnityEngine;
using EventPlannerSim.Core;

namespace EventPlannerSim.Data
{
    /// <summary>
    /// ScriptableObject defining a venue's attributes and capabilities.
    /// Requirements: R9.1
    /// </summary>
    [CreateAssetMenu(fileName = "Venue", menuName = "EventPlanner/Venue")]
    public class VenueData : ScriptableObject
    {
        [Header("Identity")]
        public string id;
        public string venueName;
        public VenueType venueType;
        public VendorTier tier;

        [Header("Capacity")]
        public int capacityMin;
        public int capacityMax;
        public int capacityComfortable;

        [Header("Pricing")]
        public float basePrice;
        public float pricePerGuest;

        [Header("Attributes")]
        public bool isIndoor;
        public bool hasOutdoorSpace;
        public bool weatherDependent;
        
        [Range(1f, 5f)]
        public float ambianceRating;

        [Header("Unlock Requirements")]
        public MapZone zone;
        public BusinessStage requiredStage;
    }
}
