using System;
using System.Collections.Generic;
using EventPlannerSim.Core;

namespace EventPlannerSim.Data
{
    /// <summary>
    /// Represents the player's current game state and progression.
    /// Serializable for save/load support.
    /// </summary>
    [Serializable]
    public class PlayerData
    {
        // Identity
        public string playerName = "Alex";
        public AvatarData avatar = new AvatarData();

        // Progression
        public BusinessStage stage = BusinessStage.Solo;
        public CareerPath careerPath = CareerPath.None;
        public int reputation = 0;
        public float money = 500f;

        // Stage 2 Employee Data
        public EmployeeData employeeData;

        // Tracking
        public List<string> activeEventIds = new List<string>();
        public List<string> completedEventIds = new List<string>();
        public List<VendorRelationship> vendorRelationships = new List<VendorRelationship>();

        // Hidden Metrics (not displayed to player)
        public int hiddenAboveAndBeyondCount = 0;
        public int hiddenExploitationCount = 0;

        // Emergency Funding
        public int familyHelpUsed = 0; // Max 3 in Stages 1-2

        // Unlocks
        public List<MapZone> unlockedZones = new List<MapZone> { MapZone.Neighborhood };
    }

    /// <summary>
    /// Represents the player's employee status in Stage 2.
    /// </summary>
    [Serializable]
    public class EmployeeData
    {
        public string companyName = "Premier Events Co.";
        public int employeeLevel = 1; // 1-5
        public int performanceScore = 0;
        public int consecutiveNegativeReviews = 0;
        public int eventsCompletedSinceReview = 0;
        public int activeSideGigs = 0;

        /// <summary>
        /// Gets the employee title based on level.
        /// Level 1-2 = Junior Planner, Level 3-4 = Planner, Level 5 = Senior Planner
        /// </summary>
        public string GetTitle() => employeeLevel switch
        {
            1 or 2 => "Junior Planner",
            3 or 4 => "Planner",
            5 => "Senior Planner",
            _ => "Planner"
        };

        /// <summary>
        /// Gets the compensation structure based on employee level.
        /// Returns (basePay, commissionRate) tuple.
        /// Requirements: R16.3
        /// </summary>
        public (float basePay, float commission) GetCompensation() => employeeLevel switch
        {
            1 or 2 => (500f, 0.05f),  // Junior: $500 base + 5% commission
            3 or 4 => (750f, 0.10f),  // Planner: $750 base + 10% commission
            5 => (1000f, 0.15f),      // Senior: $1000 base + 15% commission
            _ => (500f, 0.05f)
        };
    }

    /// <summary>
    /// Represents the player's avatar customization.
    /// </summary>
    [Serializable]
    public class AvatarData
    {
        public int faceShapeIndex = 0;
        public int skinToneIndex = 0;
        public int hairStyleIndex = 0;
        public int hairColorIndex = 0;
        public int outfitIndex = 0;
    }
}
