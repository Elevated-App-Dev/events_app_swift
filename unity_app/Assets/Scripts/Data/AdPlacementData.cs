using System;
using EventPlannerSim.Core;

namespace EventPlannerSim.Data
{
    /// <summary>
    /// Data class representing an ad placement configuration.
    /// Requirements: R30.1-R30.10
    /// 
    /// Note: In a full Unity implementation, this would be a ScriptableObject.
    /// For testability and pure C# logic, we use a serializable class.
    /// </summary>
    [Serializable]
    public class AdPlacementData
    {
        /// <summary>
        /// The placement type/context for this ad.
        /// </summary>
        public AdPlacement placement;

        /// <summary>
        /// Display name for UI.
        /// </summary>
        public string displayName;

        /// <summary>
        /// Description of what watching this ad provides.
        /// </summary>
        public string description;

        /// <summary>
        /// Cooldown in seconds between ad watches for this placement.
        /// Requirements: R30.4
        /// </summary>
        public float cooldownSeconds;

        /// <summary>
        /// Maximum number of times this ad can be watched per day.
        /// Requirements: R30.2, R30.4
        /// </summary>
        public int dailyLimit;

        /// <summary>
        /// The reward granted for watching this ad.
        /// Requirements: R28.3
        /// </summary>
        public AdReward reward;

        /// <summary>
        /// Whether this placement is currently enabled.
        /// </summary>
        public bool isEnabled = true;

        /// <summary>
        /// Minimum player funds required to show this ad option.
        /// Requirements: R30.3 (Emergency funding shows when below $500)
        /// </summary>
        public float minFundsRequired;

        /// <summary>
        /// Maximum player funds - ad won't show if player has more.
        /// Requirements: R30.3
        /// </summary>
        public float maxFundsAllowed = float.MaxValue;

        /// <summary>
        /// Whether this ad requires a specific game state to be available.
        /// </summary>
        public bool requiresSpecificState;

        /// <summary>
        /// Icon identifier for UI display.
        /// </summary>
        public string iconId;

        /// <summary>
        /// Priority for display order (higher = shown first).
        /// </summary>
        public int priority;

        /// <summary>
        /// Create default Emergency Funding ad placement.
        /// Requirements: R30.1, R30.3
        /// </summary>
        public static AdPlacementData CreateEmergencyFunding()
        {
            return new AdPlacementData
            {
                placement = AdPlacement.EmergencyFunding,
                displayName = "Emergency Funding",
                description = "Watch an ad to unlock emergency funding options",
                cooldownSeconds = 300f, // 5 minutes
                dailyLimit = 3,
                maxFundsAllowed = 500f, // Only shows when below $500
                reward = new AdReward(
                    AdPlacement.EmergencyFunding,
                    AdRewardType.EmergencyFunding,
                    500f,
                    "Emergency funding unlocked"
                ),
                priority = 100
            };
        }

        /// <summary>
        /// Create default Overtime Hours ad placement.
        /// Requirements: R30.2
        /// </summary>
        public static AdPlacementData CreateOvertimeHours()
        {
            return new AdPlacementData
            {
                placement = AdPlacement.OvertimeHours,
                displayName = "Overtime Hours",
                description = "Watch an ad to gain 4 extra work hours",
                cooldownSeconds = 0f, // No cooldown, limited by daily count
                dailyLimit = 2, // Max 2 per day
                requiresSpecificState = true, // Only when work hours exhausted
                reward = new AdReward(
                    AdPlacement.OvertimeHours,
                    AdRewardType.OvertimeHours,
                    4f,
                    "+4 overtime hours"
                ),
                priority = 90
            };
        }

        /// <summary>
        /// Create default Random Event Mitigation ad placement.
        /// Requirements: R30.1
        /// </summary>
        public static AdPlacementData CreateRandomEventMitigation()
        {
            return new AdPlacementData
            {
                placement = AdPlacement.RandomEventMitigation,
                displayName = "Event Mitigation",
                description = "Watch an ad to unlock emergency mitigation options",
                cooldownSeconds = 600f, // 10 minutes
                dailyLimit = 5,
                requiresSpecificState = true, // Only during random events
                reward = new AdReward(
                    AdPlacement.RandomEventMitigation,
                    AdRewardType.EventMitigation,
                    1f,
                    "Mitigation options unlocked"
                ),
                priority = 95
            };
        }

        /// <summary>
        /// Create default Time Skip ad placement.
        /// Requirements: R30.8
        /// </summary>
        public static AdPlacementData CreateTimeSkip()
        {
            return new AdPlacementData
            {
                placement = AdPlacement.TimeSkip,
                displayName = "Skip Wait Time",
                description = "Watch an ad to skip waiting time",
                cooldownSeconds = 180f, // 3 minutes
                dailyLimit = 10,
                reward = new AdReward(
                    AdPlacement.TimeSkip,
                    AdRewardType.TimeSkip,
                    1f,
                    "Time skipped"
                ),
                priority = 50
            };
        }

        /// <summary>
        /// Check if this ad placement should be shown based on player funds.
        /// </summary>
        public bool ShouldShowForFunds(float playerFunds)
        {
            return playerFunds >= minFundsRequired && playerFunds <= maxFundsAllowed;
        }
    }
}
