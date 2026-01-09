using System;
using EventPlannerSim.Core;

namespace EventPlannerSim.Data
{
    /// <summary>
    /// Result of a random event evaluation during event execution.
    /// Requirements: R12.3, R12.6
    /// </summary>
    [Serializable]
    public class RandomEventResult
    {
        public RandomEventType eventType;
        public string eventDescription;
        public float baseSatisfactionImpact; // Negative for problems, positive for bonuses
        public float mitigationCost; // Cost to mitigate via contingency
        public bool canBeMitigated;
        public bool wasMitigated;
        public string mitigationDescription; // What happened if mitigated
        public string failureDescription; // What happened if not mitigated

        /// <summary>
        /// Calculate final satisfaction impact based on mitigation status.
        /// If mitigated, impact is reduced to 25% of base impact.
        /// Requirements: R12.3, R12.6
        /// </summary>
        public float GetFinalImpact() => wasMitigated ? baseSatisfactionImpact * 0.25f : baseSatisfactionImpact;
    }

    /// <summary>
    /// Result of attempting to mitigate a random event.
    /// Requirements: R7.15-R7.17
    /// </summary>
    [Serializable]
    public class MitigationResult
    {
        public bool canMitigate;
        public float requiredBudget;
        public float availableBudget;
        public string mitigationOption; // Description of what can be done
        public float reducedImpact; // Satisfaction impact if mitigated
    }
}
