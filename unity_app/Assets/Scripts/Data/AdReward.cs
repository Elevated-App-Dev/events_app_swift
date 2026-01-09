using System;
using EventPlannerSim.Core;

namespace EventPlannerSim.Data
{
    /// <summary>
    /// Represents the reward granted for watching an ad.
    /// Requirements: R28.3
    /// </summary>
    [Serializable]
    public class AdReward
    {
        public AdPlacement placement;
        public AdRewardType rewardType;
        public float amount;
        public string description;

        public AdReward() { }

        public AdReward(AdPlacement placement, AdRewardType rewardType, float amount, string description)
        {
            this.placement = placement;
            this.rewardType = rewardType;
            this.amount = amount;
            this.description = description;
        }
    }

    /// <summary>
    /// Types of rewards that can be granted from ads.
    /// </summary>
    public enum AdRewardType
    {
        Currency,
        OvertimeHours,
        EmergencyFunding,
        EventMitigation,
        TimeSkip
    }
}
