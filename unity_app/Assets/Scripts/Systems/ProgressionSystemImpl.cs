using System;
using System.Collections.Generic;
using System.Linq;
using EventPlannerSim.Core;
using EventPlannerSim.Data;

namespace EventPlannerSim.Systems
{
    /// <summary>
    /// Implementation of the progression system.
    /// Manages player progression through stages and reputation.
    /// Requirements: R14.1-R14.12, R16.5-R16.10
    /// </summary>
    public class ProgressionSystemImpl : IProgressionSystem
    {
        private readonly Random _random;

        // Reputation change ranges by satisfaction tier (R14.2-R14.6)
        private const int HighSatisfactionMinRep = 15;
        private const int HighSatisfactionMaxRep = 25;
        private const int GoodSatisfactionMinRep = 5;
        private const int GoodSatisfactionMaxRep = 14;
        private const int OkaySatisfactionMinRep = 1;
        private const int OkaySatisfactionMaxRep = 4;
        private const int PoorSatisfactionMinRep = -10;
        private const int PoorSatisfactionMaxRep = -5;
        private const int FailedSatisfactionMinRep = -25;
        private const int FailedSatisfactionMaxRep = -15;

        // Satisfaction thresholds
        private const float HighSatisfactionThreshold = 90f;
        private const float GoodSatisfactionThreshold = 75f;
        private const float OkaySatisfactionThreshold = 60f;
        private const float PoorSatisfactionThreshold = 40f;

        // Referral chances (R23.1-R23.2)
        private const float ExcellentReferralChance = 0.80f; // 95-100%
        private const float HighReferralChance = 0.50f; // 90-94%

        // Performance review thresholds (R16.6-R16.8)
        private const float PositiveReviewSatisfactionThreshold = 70f;
        private const float PositiveReviewTaskRateThreshold = 80f;
        private const float NegativeReviewSatisfactionThreshold = 60f;
        private const float NegativeReviewTaskRateThreshold = 60f;

        // Celebrity reputation loss cap (R15.19a)
        private const int CelebrityReputationLossCap = -50;

        public ProgressionSystemImpl() : this(new Random())
        {
        }

        public ProgressionSystemImpl(Random random)
        {
            _random = random ?? new Random();
        }

        /// <inheritdoc/>
        public ReputationChange ApplyEventResult(float satisfaction, int currentReputation, int excellenceStreak = 0)
        {
            var result = new ReputationChange
            {
                TriggeredReferral = false,
                GeneratedNegativeReview = false,
                ReferralProbability = 0f
            };

            // Calculate reputation change based on satisfaction tier (R14.2-R14.6)
            if (satisfaction >= HighSatisfactionThreshold)
            {
                // 90-100%: +15 to +25, referral triggered (R14.2)
                result.Amount = _random.Next(HighSatisfactionMinRep, HighSatisfactionMaxRep + 1);
                result.ReferralProbability = CalculateReferralProbability(satisfaction, excellenceStreak);
                result.TriggeredReferral = _random.NextDouble() < result.ReferralProbability;
                result.Description = "Excellent event! Client is thrilled.";
            }
            else if (satisfaction >= GoodSatisfactionThreshold)
            {
                // 75-89%: +5 to +14, 50% referral chance (R14.3)
                result.Amount = _random.Next(GoodSatisfactionMinRep, GoodSatisfactionMaxRep + 1);
                result.ReferralProbability = 0.50f;
                result.TriggeredReferral = _random.NextDouble() < result.ReferralProbability;
                result.Description = "Good event! Client is satisfied.";
            }
            else if (satisfaction >= OkaySatisfactionThreshold)
            {
                // 60-74%: +1 to +4, no referral (R14.4)
                result.Amount = _random.Next(OkaySatisfactionMinRep, OkaySatisfactionMaxRep + 1);
                result.ReferralProbability = 0f;
                result.Description = "Acceptable event. Room for improvement.";
            }
            else if (satisfaction >= PoorSatisfactionThreshold)
            {
                // 40-59%: -5 to -10 (R14.5)
                result.Amount = _random.Next(PoorSatisfactionMinRep, PoorSatisfactionMaxRep + 1);
                result.Description = "Disappointing event. Client was not happy.";
            }
            else
            {
                // <40%: -15 to -25, negative review (R14.6)
                result.Amount = _random.Next(FailedSatisfactionMinRep, FailedSatisfactionMaxRep + 1);
                result.GeneratedNegativeReview = true;
                result.Description = "Failed event. Client left a negative review.";
            }

            result.NewReputation = Math.Max(0, currentReputation + result.Amount);
            return result;
        }

        /// <summary>
        /// Calculate referral probability based on satisfaction and excellence streak.
        /// Requirements: R23.1-R23.8
        /// </summary>
        private float CalculateReferralProbability(float satisfaction, int excellenceStreak)
        {
            float baseChance;

            if (satisfaction >= 95f)
            {
                baseChance = ExcellentReferralChance; // 80%
            }
            else if (satisfaction >= HighSatisfactionThreshold)
            {
                baseChance = HighReferralChance; // 50%
            }
            else
            {
                return 0f; // No referral below 90%
            }

            // Apply excellence streak bonus (R23.6-R23.8)
            float streakBonus = 0f;
            if (excellenceStreak >= 5)
            {
                streakBonus = 0.20f; // +20% at 5+ streak
            }
            else if (excellenceStreak >= 3)
            {
                streakBonus = 0.10f; // +10% at 3+ streak
            }

            return Math.Min(1.0f, baseChance + streakBonus);
        }

        /// <inheritdoc/>
        public bool CanAdvanceStage(PlayerData player)
        {
            var requirements = GetStageRequirements(player.stage);
            if (requirements == null) return false;

            // Check reputation requirement
            if (player.reputation < requirements.MinReputation) return false;

            // Check money requirement
            if (player.money < requirements.MinMoney) return false;

            // Check employee level for Stage 2 -> 3 transition
            if (player.stage == BusinessStage.Employee)
            {
                if (player.employeeData == null) return false;
                if (player.employeeData.employeeLevel < requirements.MinEmployeeLevel) return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public StageRequirements GetStageRequirements(BusinessStage currentStage)
        {
            return currentStage switch
            {
                // Stage 1 -> 2: 25 reputation, $5,000 (R14.7)
                BusinessStage.Solo => new StageRequirements
                {
                    MinReputation = 25,
                    MinMoney = 5000f,
                    MinEmployeeLevel = 0,
                    Description = "Reach 25 reputation and save $5,000 to unlock Stage 2"
                },
                // Stage 2 -> 3: Senior Planner (Level 5), 50 reputation, $25,000 (R14.10)
                BusinessStage.Employee => new StageRequirements
                {
                    MinReputation = 50,
                    MinMoney = 25000f,
                    MinEmployeeLevel = 5,
                    Description = "Reach Senior Planner (Level 5), 50 reputation, and save $25,000 to unlock Stage 3"
                },
                // Stage 3 -> 4: 100 reputation, $75,000 (R14.11)
                BusinessStage.SmallCompany => new StageRequirements
                {
                    MinReputation = 100,
                    MinMoney = 75000f,
                    MinEmployeeLevel = 0,
                    Description = "Reach 100 reputation and save $75,000 to unlock Stage 4"
                },
                // Stage 4 -> 5: 200 reputation, $250,000 (R14.12)
                BusinessStage.Established => new StageRequirements
                {
                    MinReputation = 200,
                    MinMoney = 250000f,
                    MinEmployeeLevel = 0,
                    Description = "Reach 200 reputation and save $250,000 to unlock Stage 5"
                },
                // Stage 5 is max
                BusinessStage.Premier => null,
                _ => null
            };
        }

        /// <inheritdoc/>
        public PersonalityDistribution GetPersonalityDistribution(int stage)
        {
            // Requirements: R14.13
            return stage switch
            {
                // Stage 1: 50% Easy-Going, 30% Budget-Conscious, 20% Perfectionist
                1 => new PersonalityDistribution
                {
                    EasyGoingChance = 0.50f,
                    BudgetConsciousChance = 0.30f,
                    PerfectionistChance = 0.20f,
                    IndecisiveChance = 0f,
                    DemandingChance = 0f,
                    CelebrityChance = 0f
                },
                // Stage 2: 40% Easy-Going, 35% Budget-Conscious, 25% Perfectionist
                2 => new PersonalityDistribution
                {
                    EasyGoingChance = 0.40f,
                    BudgetConsciousChance = 0.35f,
                    PerfectionistChance = 0.25f,
                    IndecisiveChance = 0f,
                    DemandingChance = 0f,
                    CelebrityChance = 0f
                },
                // Stage 3+: 33% Easy-Going, 33% Budget-Conscious, 34% Perfectionist
                // Stage 3 also unlocks Indecisive (R15.7)
                3 => new PersonalityDistribution
                {
                    EasyGoingChance = 0.33f,
                    BudgetConsciousChance = 0.33f,
                    PerfectionistChance = 0.34f,
                    IndecisiveChance = 0f, // Available but not in base distribution
                    DemandingChance = 0f,
                    CelebrityChance = 0f
                },
                // Stage 4: Same base distribution, Demanding unlocked (R15.8)
                4 => new PersonalityDistribution
                {
                    EasyGoingChance = 0.33f,
                    BudgetConsciousChance = 0.33f,
                    PerfectionistChance = 0.34f,
                    IndecisiveChance = 0f,
                    DemandingChance = 0f,
                    CelebrityChance = 0f
                },
                // Stage 5: Same base distribution, Celebrity unlocked (R15.9)
                5 => new PersonalityDistribution
                {
                    EasyGoingChance = 0.33f,
                    BudgetConsciousChance = 0.33f,
                    PerfectionistChance = 0.34f,
                    IndecisiveChance = 0f,
                    DemandingChance = 0f,
                    CelebrityChance = 0f
                },
                // Default to Stage 1 distribution
                _ => new PersonalityDistribution
                {
                    EasyGoingChance = 0.50f,
                    BudgetConsciousChance = 0.30f,
                    PerfectionistChance = 0.20f,
                    IndecisiveChance = 0f,
                    DemandingChance = 0f,
                    CelebrityChance = 0f
                }
            };
        }

        /// <inheritdoc/>
        public PerformanceReviewResult EvaluatePerformance(
            List<EventData> recentEvents,
            int currentEmployeeLevel,
            int consecutiveNegativeReviews)
        {
            var result = new PerformanceReviewResult
            {
                NewEmployeeLevel = currentEmployeeLevel,
                ConsecutiveNegativeReviews = consecutiveNegativeReviews,
                WasPromoted = false,
                WasDemoted = false,
                WasTerminated = false
            };

            if (recentEvents == null || recentEvents.Count == 0)
            {
                result.Outcome = PerformanceReviewOutcome.Neutral;
                result.AverageSatisfaction = 0f;
                result.OnTimeTaskRate = 0f;
                result.FeedbackMessage = "No events to review.";
                return result;
            }

            // Calculate average satisfaction
            result.AverageSatisfaction = recentEvents
                .Where(e => e.results != null)
                .Select(e => e.results.finalSatisfaction)
                .DefaultIfEmpty(0f)
                .Average();

            // Calculate on-time task completion rate
            int totalTasks = 0;
            int completedOnTime = 0;
            foreach (var evt in recentEvents)
            {
                if (evt.tasks != null)
                {
                    foreach (var task in evt.tasks)
                    {
                        totalTasks++;
                        if (task.status == TaskStatus.Completed)
                        {
                            completedOnTime++;
                        }
                    }
                }
            }
            result.OnTimeTaskRate = totalTasks > 0 ? (completedOnTime / (float)totalTasks) * 100f : 100f;

            // Determine outcome (R16.6-R16.8)
            if (result.AverageSatisfaction >= PositiveReviewSatisfactionThreshold &&
                result.OnTimeTaskRate >= PositiveReviewTaskRateThreshold)
            {
                // Positive review (R16.7)
                result.Outcome = PerformanceReviewOutcome.Positive;
                result.ConsecutiveNegativeReviews = 0;

                // Progress toward next level
                if (currentEmployeeLevel < 5)
                {
                    result.NewEmployeeLevel = currentEmployeeLevel + 1;
                    result.WasPromoted = true;
                    result.FeedbackMessage = $"Excellent work! You've been promoted to {GetEmployeeTitle(result.NewEmployeeLevel)}.";
                }
                else
                {
                    result.FeedbackMessage = "Outstanding performance! You're at the top of your game.";
                }
            }
            else if (result.AverageSatisfaction < NegativeReviewSatisfactionThreshold ||
                     result.OnTimeTaskRate < NegativeReviewTaskRateThreshold)
            {
                // Negative review (R16.8)
                result.Outcome = PerformanceReviewOutcome.Negative;
                result.ConsecutiveNegativeReviews = consecutiveNegativeReviews + 1;

                if (result.ConsecutiveNegativeReviews >= 3 && currentEmployeeLevel == 1)
                {
                    // Termination (R16.10)
                    result.WasTerminated = true;
                    result.FeedbackMessage = "We're sorry, but we have to let you go. Your performance has not met expectations.";
                }
                else if (result.ConsecutiveNegativeReviews >= 2)
                {
                    // Demotion (R16.9)
                    result.NewEmployeeLevel = Math.Max(1, currentEmployeeLevel - 1);
                    result.WasDemoted = currentEmployeeLevel > 1;
                    if (result.WasDemoted)
                    {
                        result.FeedbackMessage = $"Your performance needs improvement. You've been demoted to {GetEmployeeTitle(result.NewEmployeeLevel)}.";
                    }
                    else
                    {
                        result.FeedbackMessage = "This is your final warning. One more negative review and we'll have to let you go.";
                    }
                }
                else
                {
                    result.FeedbackMessage = "Your performance needs improvement. This is a warning.";
                }
            }
            else
            {
                // Neutral review
                result.Outcome = PerformanceReviewOutcome.Neutral;
                result.FeedbackMessage = "Your performance is acceptable. Keep working to improve.";
            }

            return result;
        }

        /// <summary>
        /// Get employee title based on level.
        /// </summary>
        private string GetEmployeeTitle(int level) => level switch
        {
            1 or 2 => "Junior Planner",
            3 or 4 => "Planner",
            5 => "Senior Planner",
            _ => "Planner"
        };

        /// <inheritdoc/>
        public float GetRandomEventFrequency(int stage)
        {
            // Requirements: R14.14
            return stage switch
            {
                1 => 0.20f, // 20%
                2 => 0.35f, // 35%
                3 => 0.50f, // 50%
                4 => 0.65f, // 65%
                5 => 0.80f, // 80%
                _ => 0.20f
            };
        }

        /// <inheritdoc/>
        public int GetMinimumReputationThreshold(BusinessStage stage)
        {
            // Requirements: R14.16
            return stage switch
            {
                BusinessStage.Solo => 0,      // No minimum for Stage 1
                BusinessStage.Employee => 10,  // Stage 2 requires 10+
                BusinessStage.SmallCompany => 30, // Stage 3 requires 30+
                BusinessStage.Established => 75,  // Stage 4 requires 75+
                BusinessStage.Premier => 150,     // Stage 5 requires 150+
                _ => 0
            };
        }

        /// <inheritdoc/>
        public int CalculateCelebrityReputationChange(float satisfaction, PressCoverage pressCoverage)
        {
            // First calculate base reputation change
            int baseChange;
            if (satisfaction >= HighSatisfactionThreshold)
            {
                baseChange = _random.Next(HighSatisfactionMinRep, HighSatisfactionMaxRep + 1);
            }
            else if (satisfaction >= GoodSatisfactionThreshold)
            {
                baseChange = _random.Next(GoodSatisfactionMinRep, GoodSatisfactionMaxRep + 1);
            }
            else if (satisfaction >= OkaySatisfactionThreshold)
            {
                baseChange = _random.Next(OkaySatisfactionMinRep, OkaySatisfactionMaxRep + 1);
            }
            else if (satisfaction >= PoorSatisfactionThreshold)
            {
                baseChange = _random.Next(PoorSatisfactionMinRep, PoorSatisfactionMaxRep + 1);
            }
            else
            {
                baseChange = _random.Next(FailedSatisfactionMinRep, FailedSatisfactionMaxRep + 1);
            }

            // Apply press coverage multiplier
            float multiplier = pressCoverage switch
            {
                PressCoverage.Positive => satisfaction >= 70f ? 3.0f : 2.0f,
                PressCoverage.Neutral => satisfaction >= 70f ? 2.0f : 2.5f,
                PressCoverage.Negative => satisfaction >= 70f ? 1.5f : 3.0f,
                _ => 1.0f
            };

            int finalChange = (int)(baseChange * multiplier);

            // Apply cap for losses (R15.19a)
            if (finalChange < CelebrityReputationLossCap)
            {
                finalChange = CelebrityReputationLossCap;
            }

            return finalChange;
        }
    }
}
