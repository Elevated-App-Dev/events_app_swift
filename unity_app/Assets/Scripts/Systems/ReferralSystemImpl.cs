using System;

namespace EventPlannerSim.Systems
{
    /// <summary>
    /// Implementation of the referral system.
    /// Manages referral opportunities and excellence streak tracking.
    /// Requirements: R23.1-R23.13
    /// </summary>
    public class ReferralSystemImpl : IReferralSystem
    {
        private readonly Random _random;

        // Referral chance thresholds (R23.2)
        private const float ExcellentSatisfactionThreshold = 95f;
        private const float HighSatisfactionThreshold = 90f;
        private const float StreakResetThreshold = 80f;

        // Base referral chances (R23.2)
        private const float ExcellentReferralChance = 0.80f; // 95-100%
        private const float HighReferralChance = 0.50f;      // 90-94%

        // Streak bonuses (R23.6-R23.8)
        private const float Streak3Bonus = 0.10f;  // +10% at 3+ streak
        private const float Streak5Bonus = 0.20f;  // +20% at 5+ streak (total)

        public ReferralSystemImpl() : this(new Random())
        {
        }

        public ReferralSystemImpl(Random random)
        {
            _random = random ?? new Random();
        }

        /// <inheritdoc/>
        public float CalculateReferralProbability(float satisfaction, int excellenceStreak)
        {
            float baseChance = GetBaseReferralChance(satisfaction);

            // No referral possible below 90% satisfaction
            if (baseChance <= 0f)
            {
                return 0f;
            }

            // Apply streak bonus (R23.6-R23.8)
            float streakBonus = GetStreakBonus(excellenceStreak);

            // Cap at 100%
            return Math.Min(1.0f, baseChance + streakBonus);
        }

        /// <inheritdoc/>
        public ReferralResult EvaluateReferral(float satisfaction, int excellenceStreak)
        {
            float baseChance = GetBaseReferralChance(satisfaction);
            float streakBonus = baseChance > 0f ? GetStreakBonus(excellenceStreak) : 0f;
            float probability = Math.Min(1.0f, baseChance + streakBonus);

            bool wasTriggered = probability > 0f && _random.NextDouble() < probability;

            return new ReferralResult
            {
                Probability = probability,
                WasTriggered = wasTriggered,
                BaseChance = baseChance,
                StreakBonus = streakBonus,
                Satisfaction = satisfaction,
                ExcellenceStreak = excellenceStreak
            };
        }

        /// <inheritdoc/>
        public ExcellenceStreakResult UpdateExcellenceStreak(int currentStreak, float satisfaction)
        {
            var result = new ExcellenceStreakResult
            {
                PreviousStreak = currentStreak,
                Satisfaction = satisfaction,
                WasIncremented = false,
                WasReset = false,
                WasUnchanged = false
            };

            if (satisfaction >= HighSatisfactionThreshold)
            {
                // 90%+ satisfaction: increment streak (R23.6)
                result.NewStreak = currentStreak + 1;
                result.WasIncremented = true;
            }
            else if (satisfaction < StreakResetThreshold)
            {
                // Below 80%: reset streak (R23.9)
                result.NewStreak = 0;
                result.WasReset = true;
            }
            else
            {
                // 80-89%: streak unchanged
                result.NewStreak = currentStreak;
                result.WasUnchanged = true;
            }

            return result;
        }

        /// <inheritdoc/>
        public float GetStreakBonus(int excellenceStreak)
        {
            if (excellenceStreak >= 5)
            {
                return Streak5Bonus; // +20% at 5+ streak
            }
            else if (excellenceStreak >= 3)
            {
                return Streak3Bonus; // +10% at 3+ streak
            }
            return 0f;
        }

        /// <inheritdoc/>
        public bool QualifiesForReferral(float satisfaction)
        {
            return satisfaction >= HighSatisfactionThreshold;
        }

        /// <inheritdoc/>
        public float GetBaseReferralChance(float satisfaction)
        {
            if (satisfaction >= ExcellentSatisfactionThreshold)
            {
                return ExcellentReferralChance; // 80% for 95-100%
            }
            else if (satisfaction >= HighSatisfactionThreshold)
            {
                return HighReferralChance; // 50% for 90-94%
            }
            return 0f; // No referral below 90%
        }
    }
}
