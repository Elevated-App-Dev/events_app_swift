using EventPlannerSim.Data;

namespace EventPlannerSim.Systems
{
    /// <summary>
    /// Result of a referral calculation.
    /// </summary>
    public class ReferralResult
    {
        /// <summary>
        /// The calculated referral probability (0-1).
        /// </summary>
        public float Probability { get; set; }

        /// <summary>
        /// Whether a referral was triggered based on the probability.
        /// </summary>
        public bool WasTriggered { get; set; }

        /// <summary>
        /// The base chance before streak bonuses (0-1).
        /// </summary>
        public float BaseChance { get; set; }

        /// <summary>
        /// The bonus from excellence streak (0-1).
        /// </summary>
        public float StreakBonus { get; set; }

        /// <summary>
        /// The satisfaction score used for calculation.
        /// </summary>
        public float Satisfaction { get; set; }

        /// <summary>
        /// The excellence streak used for calculation.
        /// </summary>
        public int ExcellenceStreak { get; set; }
    }

    /// <summary>
    /// Result of updating the excellence streak.
    /// </summary>
    public class ExcellenceStreakResult
    {
        /// <summary>
        /// The new excellence streak value.
        /// </summary>
        public int NewStreak { get; set; }

        /// <summary>
        /// The previous excellence streak value.
        /// </summary>
        public int PreviousStreak { get; set; }

        /// <summary>
        /// Whether the streak was incremented.
        /// </summary>
        public bool WasIncremented { get; set; }

        /// <summary>
        /// Whether the streak was reset.
        /// </summary>
        public bool WasReset { get; set; }

        /// <summary>
        /// Whether the streak was unchanged.
        /// </summary>
        public bool WasUnchanged { get; set; }

        /// <summary>
        /// The satisfaction score that caused this change.
        /// </summary>
        public float Satisfaction { get; set; }
    }

    /// <summary>
    /// Manages referral opportunities and excellence streak tracking.
    /// Requirements: R23.1-R23.13
    /// </summary>
    public interface IReferralSystem
    {
        /// <summary>
        /// Calculate the referral probability based on satisfaction and excellence streak.
        /// Requirements: R23.1-R23.2, R23.6-R23.8
        /// 
        /// Base chances:
        /// - 95-100% satisfaction: 80% referral chance
        /// - 90-94% satisfaction: 50% referral chance
        /// - Below 90%: 0% chance
        /// 
        /// Streak bonuses (only apply when satisfaction >= 90%):
        /// - 3+ streak: +10% chance
        /// - 5+ streak: +20% chance (total)
        /// </summary>
        /// <param name="satisfaction">Final satisfaction score (0-100).</param>
        /// <param name="excellenceStreak">Current excellence streak count.</param>
        /// <returns>Referral probability (0-1).</returns>
        float CalculateReferralProbability(float satisfaction, int excellenceStreak);

        /// <summary>
        /// Calculate referral probability and determine if a referral was triggered.
        /// Requirements: R23.1-R23.8
        /// </summary>
        /// <param name="satisfaction">Final satisfaction score (0-100).</param>
        /// <param name="excellenceStreak">Current excellence streak count.</param>
        /// <returns>ReferralResult with probability and trigger status.</returns>
        ReferralResult EvaluateReferral(float satisfaction, int excellenceStreak);

        /// <summary>
        /// Update the excellence streak based on event satisfaction.
        /// Requirements: R23.6-R23.9
        /// 
        /// Rules:
        /// - Satisfaction >= 90%: Increment streak by 1
        /// - Satisfaction < 80%: Reset streak to 0
        /// - Satisfaction 80-89%: Streak unchanged
        /// </summary>
        /// <param name="currentStreak">Current excellence streak value.</param>
        /// <param name="satisfaction">Final satisfaction score (0-100).</param>
        /// <returns>ExcellenceStreakResult with new streak value and change info.</returns>
        ExcellenceStreakResult UpdateExcellenceStreak(int currentStreak, float satisfaction);

        /// <summary>
        /// Get the streak bonus percentage for a given streak count.
        /// Requirements: R23.6-R23.8
        /// </summary>
        /// <param name="excellenceStreak">Current excellence streak count.</param>
        /// <returns>Bonus percentage (0-0.20).</returns>
        float GetStreakBonus(int excellenceStreak);

        /// <summary>
        /// Check if satisfaction qualifies for referral consideration.
        /// Requirements: R23.1
        /// </summary>
        /// <param name="satisfaction">Final satisfaction score (0-100).</param>
        /// <returns>True if satisfaction >= 90%.</returns>
        bool QualifiesForReferral(float satisfaction);

        /// <summary>
        /// Get the base referral chance for a satisfaction score (without streak bonus).
        /// Requirements: R23.2
        /// </summary>
        /// <param name="satisfaction">Final satisfaction score (0-100).</param>
        /// <returns>Base referral chance (0-1).</returns>
        float GetBaseReferralChance(float satisfaction);
    }
}
