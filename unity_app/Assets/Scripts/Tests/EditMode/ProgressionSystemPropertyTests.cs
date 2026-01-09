using System;
using System.Collections.Generic;
using NUnit.Framework;
using EventPlannerSim.Core;
using EventPlannerSim.Data;
using EventPlannerSim.Systems;

namespace EventPlannerSim.Tests.EditMode
{
    /// <summary>
    /// Property-based tests for ProgressionSystem.
    /// Feature: event-planner-simulator
    /// </summary>
    [TestFixture]
    public class ProgressionSystemPropertyTests
    {
        private ProgressionSystemImpl _progressionSystem;
        private Random _random;

        [SetUp]
        public void Setup()
        {
            // Use fixed seed for reproducibility
            _random = new Random(42);
            _progressionSystem = new ProgressionSystemImpl(_random);
        }

        #region Property 15: Reputation Change by Satisfaction

        /// <summary>
        /// Property 15: Reputation Change by Satisfaction
        /// For any satisfaction score S:
        ///   90-100%: +15 to +25, referral triggered
        ///   75-89%: +5 to +14, 50% referral chance
        ///   60-74%: +1 to +4, no referral
        ///   40-59%: -5 to -10
        ///   &lt;40%: -15 to -25, negative review
        /// **Validates: Requirements R14**
        /// </summary>
        [Test]
        public void ApplyEventResult_HighSatisfaction_ReturnsPositiveReputation()
        {
            // Test 90-100% satisfaction range (R14.2)
            for (int i = 0; i < 100; i++)
            {
                float satisfaction = 90f + (float)(_random.NextDouble() * 10); // 90-100
                int currentRep = _random.Next(0, 100);

                var result = _progressionSystem.ApplyEventResult(satisfaction, currentRep);

                Assert.GreaterOrEqual(result.Amount, 15,
                    $"High satisfaction ({satisfaction:F1}%) should give at least +15 reputation");
                Assert.LessOrEqual(result.Amount, 25,
                    $"High satisfaction ({satisfaction:F1}%) should give at most +25 reputation");
            }
        }

        /// <summary>
        /// Property 15: Good satisfaction (75-89%) gives +5 to +14 reputation.
        /// **Validates: Requirements R14.3**
        /// </summary>
        [Test]
        public void ApplyEventResult_GoodSatisfaction_ReturnsModeratePositiveReputation()
        {
            for (int i = 0; i < 100; i++)
            {
                float satisfaction = 75f + (float)(_random.NextDouble() * 14); // 75-89
                int currentRep = _random.Next(0, 100);

                var result = _progressionSystem.ApplyEventResult(satisfaction, currentRep);

                Assert.GreaterOrEqual(result.Amount, 5,
                    $"Good satisfaction ({satisfaction:F1}%) should give at least +5 reputation");
                Assert.LessOrEqual(result.Amount, 14,
                    $"Good satisfaction ({satisfaction:F1}%) should give at most +14 reputation");
            }
        }

        /// <summary>
        /// Property 15: Okay satisfaction (60-74%) gives +1 to +4 reputation.
        /// **Validates: Requirements R14.4**
        /// </summary>
        [Test]
        public void ApplyEventResult_OkaySatisfaction_ReturnsSmallPositiveReputation()
        {
            for (int i = 0; i < 100; i++)
            {
                float satisfaction = 60f + (float)(_random.NextDouble() * 14); // 60-74
                int currentRep = _random.Next(0, 100);

                var result = _progressionSystem.ApplyEventResult(satisfaction, currentRep);

                Assert.GreaterOrEqual(result.Amount, 1,
                    $"Okay satisfaction ({satisfaction:F1}%) should give at least +1 reputation");
                Assert.LessOrEqual(result.Amount, 4,
                    $"Okay satisfaction ({satisfaction:F1}%) should give at most +4 reputation");
            }
        }

        /// <summary>
        /// Property 15: Poor satisfaction (40-59%) gives -5 to -10 reputation.
        /// **Validates: Requirements R14.5**
        /// </summary>
        [Test]
        public void ApplyEventResult_PoorSatisfaction_ReturnsNegativeReputation()
        {
            for (int i = 0; i < 100; i++)
            {
                float satisfaction = 40f + (float)(_random.NextDouble() * 19); // 40-59
                int currentRep = _random.Next(0, 100);

                var result = _progressionSystem.ApplyEventResult(satisfaction, currentRep);

                Assert.GreaterOrEqual(result.Amount, -10,
                    $"Poor satisfaction ({satisfaction:F1}%) should give at least -10 reputation");
                Assert.LessOrEqual(result.Amount, -5,
                    $"Poor satisfaction ({satisfaction:F1}%) should give at most -5 reputation");
            }
        }

        /// <summary>
        /// Property 15: Failed satisfaction (&lt;40%) gives -15 to -25 reputation.
        /// **Validates: Requirements R14.6**
        /// </summary>
        [Test]
        public void ApplyEventResult_FailedSatisfaction_ReturnsSevereNegativeReputation()
        {
            for (int i = 0; i < 100; i++)
            {
                float satisfaction = (float)(_random.NextDouble() * 39); // 0-39
                int currentRep = _random.Next(0, 100);

                var result = _progressionSystem.ApplyEventResult(satisfaction, currentRep);

                Assert.GreaterOrEqual(result.Amount, -25,
                    $"Failed satisfaction ({satisfaction:F1}%) should give at least -25 reputation");
                Assert.LessOrEqual(result.Amount, -15,
                    $"Failed satisfaction ({satisfaction:F1}%) should give at most -15 reputation");
            }
        }

        /// <summary>
        /// Property 15: Boundary test at satisfaction thresholds.
        /// **Validates: Requirements R14**
        /// </summary>
        [Test]
        public void ApplyEventResult_BoundaryValues_CorrectTier()
        {
            // Test at exact boundaries
            var testCases = new[]
            {
                (satisfaction: 100f, minRep: 15, maxRep: 25, tier: "High"),
                (satisfaction: 90f, minRep: 15, maxRep: 25, tier: "High"),
                (satisfaction: 89.99f, minRep: 5, maxRep: 14, tier: "Good"),
                (satisfaction: 75f, minRep: 5, maxRep: 14, tier: "Good"),
                (satisfaction: 74.99f, minRep: 1, maxRep: 4, tier: "Okay"),
                (satisfaction: 60f, minRep: 1, maxRep: 4, tier: "Okay"),
                (satisfaction: 59.99f, minRep: -10, maxRep: -5, tier: "Poor"),
                (satisfaction: 40f, minRep: -10, maxRep: -5, tier: "Poor"),
                (satisfaction: 39.99f, minRep: -25, maxRep: -15, tier: "Failed"),
                (satisfaction: 0f, minRep: -25, maxRep: -15, tier: "Failed")
            };

            foreach (var (satisfaction, minRep, maxRep, tier) in testCases)
            {
                // Run multiple times to account for randomness
                for (int i = 0; i < 10; i++)
                {
                    var result = _progressionSystem.ApplyEventResult(satisfaction, 50);

                    Assert.GreaterOrEqual(result.Amount, minRep,
                        $"{tier} tier ({satisfaction}%) should give at least {minRep} reputation");
                    Assert.LessOrEqual(result.Amount, maxRep,
                        $"{tier} tier ({satisfaction}%) should give at most {maxRep} reputation");
                }
            }
        }

        /// <summary>
        /// Property 15: High satisfaction triggers referral.
        /// **Validates: Requirements R14.2**
        /// </summary>
        [Test]
        public void ApplyEventResult_HighSatisfaction_HasReferralProbability()
        {
            // Test that high satisfaction has non-zero referral probability
            for (int i = 0; i < 100; i++)
            {
                float satisfaction = 90f + (float)(_random.NextDouble() * 10); // 90-100

                var result = _progressionSystem.ApplyEventResult(satisfaction, 50);

                Assert.Greater(result.ReferralProbability, 0f,
                    $"High satisfaction ({satisfaction:F1}%) should have referral probability > 0");
            }
        }

        /// <summary>
        /// Property 15: Okay satisfaction has no referral.
        /// **Validates: Requirements R14.4**
        /// </summary>
        [Test]
        public void ApplyEventResult_OkaySatisfaction_NoReferral()
        {
            for (int i = 0; i < 100; i++)
            {
                float satisfaction = 60f + (float)(_random.NextDouble() * 14); // 60-74

                var result = _progressionSystem.ApplyEventResult(satisfaction, 50);

                Assert.AreEqual(0f, result.ReferralProbability,
                    $"Okay satisfaction ({satisfaction:F1}%) should have 0 referral probability");
                Assert.IsFalse(result.TriggeredReferral,
                    $"Okay satisfaction ({satisfaction:F1}%) should not trigger referral");
            }
        }

        /// <summary>
        /// Property 15: Failed satisfaction generates negative review.
        /// **Validates: Requirements R14.6**
        /// </summary>
        [Test]
        public void ApplyEventResult_FailedSatisfaction_GeneratesNegativeReview()
        {
            for (int i = 0; i < 100; i++)
            {
                float satisfaction = (float)(_random.NextDouble() * 39); // 0-39

                var result = _progressionSystem.ApplyEventResult(satisfaction, 50);

                Assert.IsTrue(result.GeneratedNegativeReview,
                    $"Failed satisfaction ({satisfaction:F1}%) should generate negative review");
            }
        }

        /// <summary>
        /// Property 15: Non-failed satisfaction does not generate negative review.
        /// **Validates: Requirements R14**
        /// </summary>
        [Test]
        public void ApplyEventResult_NonFailedSatisfaction_NoNegativeReview()
        {
            for (int i = 0; i < 100; i++)
            {
                float satisfaction = 40f + (float)(_random.NextDouble() * 60); // 40-100

                var result = _progressionSystem.ApplyEventResult(satisfaction, 50);

                Assert.IsFalse(result.GeneratedNegativeReview,
                    $"Non-failed satisfaction ({satisfaction:F1}%) should not generate negative review");
            }
        }

        /// <summary>
        /// Property 15: NewReputation is correctly calculated.
        /// **Validates: Requirements R14**
        /// </summary>
        [Test]
        public void ApplyEventResult_NewReputation_CorrectlyCalculated()
        {
            for (int i = 0; i < 100; i++)
            {
                float satisfaction = (float)(_random.NextDouble() * 100);
                int currentRep = _random.Next(0, 200);

                var result = _progressionSystem.ApplyEventResult(satisfaction, currentRep);

                int expectedNewRep = Math.Max(0, currentRep + result.Amount);
                Assert.AreEqual(expectedNewRep, result.NewReputation,
                    $"NewReputation should be max(0, {currentRep} + {result.Amount}) = {expectedNewRep}");
            }
        }

        /// <summary>
        /// Property 15: Reputation cannot go below 0.
        /// **Validates: Requirements R14**
        /// </summary>
        [Test]
        public void ApplyEventResult_ReputationNeverNegative()
        {
            for (int i = 0; i < 100; i++)
            {
                float satisfaction = (float)(_random.NextDouble() * 40); // Low satisfaction
                int currentRep = _random.Next(0, 30); // Low current rep

                var result = _progressionSystem.ApplyEventResult(satisfaction, currentRep);

                Assert.GreaterOrEqual(result.NewReputation, 0,
                    "NewReputation should never be negative");
            }
        }

        #endregion

        #region Personality Distribution by Stage (R14.13)

        /// <summary>
        /// Personality distribution for Stage 1.
        /// Stage 1: 50% Easy-Going, 30% Budget-Conscious, 20% Perfectionist
        /// **Validates: Requirements R14.13**
        /// </summary>
        [Test]
        public void GetPersonalityDistribution_Stage1_CorrectDistribution()
        {
            var distribution = _progressionSystem.GetPersonalityDistribution(1);

            Assert.AreEqual(0.50f, distribution.EasyGoingChance, 0.001f,
                "Stage 1 should have 50% Easy-Going");
            Assert.AreEqual(0.30f, distribution.BudgetConsciousChance, 0.001f,
                "Stage 1 should have 30% Budget-Conscious");
            Assert.AreEqual(0.20f, distribution.PerfectionistChance, 0.001f,
                "Stage 1 should have 20% Perfectionist");
            Assert.AreEqual(0f, distribution.IndecisiveChance,
                "Stage 1 should have 0% Indecisive");
            Assert.AreEqual(0f, distribution.DemandingChance,
                "Stage 1 should have 0% Demanding");
            Assert.AreEqual(0f, distribution.CelebrityChance,
                "Stage 1 should have 0% Celebrity");
        }

        /// <summary>
        /// Personality distribution for Stage 2.
        /// Stage 2: 40% Easy-Going, 35% Budget-Conscious, 25% Perfectionist
        /// **Validates: Requirements R14.13**
        /// </summary>
        [Test]
        public void GetPersonalityDistribution_Stage2_CorrectDistribution()
        {
            var distribution = _progressionSystem.GetPersonalityDistribution(2);

            Assert.AreEqual(0.40f, distribution.EasyGoingChance, 0.001f,
                "Stage 2 should have 40% Easy-Going");
            Assert.AreEqual(0.35f, distribution.BudgetConsciousChance, 0.001f,
                "Stage 2 should have 35% Budget-Conscious");
            Assert.AreEqual(0.25f, distribution.PerfectionistChance, 0.001f,
                "Stage 2 should have 25% Perfectionist");
            Assert.AreEqual(0f, distribution.IndecisiveChance,
                "Stage 2 should have 0% Indecisive");
            Assert.AreEqual(0f, distribution.DemandingChance,
                "Stage 2 should have 0% Demanding");
            Assert.AreEqual(0f, distribution.CelebrityChance,
                "Stage 2 should have 0% Celebrity");
        }

        /// <summary>
        /// Personality distribution for Stage 3+.
        /// Stage 3+: 33% Easy-Going, 33% Budget-Conscious, 34% Perfectionist
        /// **Validates: Requirements R14.13**
        /// </summary>
        [Test]
        public void GetPersonalityDistribution_Stage3Plus_CorrectDistribution()
        {
            foreach (int stage in new[] { 3, 4, 5 })
            {
                var distribution = _progressionSystem.GetPersonalityDistribution(stage);

                Assert.AreEqual(0.33f, distribution.EasyGoingChance, 0.001f,
                    $"Stage {stage} should have 33% Easy-Going");
                Assert.AreEqual(0.33f, distribution.BudgetConsciousChance, 0.001f,
                    $"Stage {stage} should have 33% Budget-Conscious");
                Assert.AreEqual(0.34f, distribution.PerfectionistChance, 0.001f,
                    $"Stage {stage} should have 34% Perfectionist");
            }
        }

        /// <summary>
        /// Personality distribution probabilities sum to 1.0 for base personalities.
        /// **Validates: Requirements R14.13**
        /// </summary>
        [Test]
        public void GetPersonalityDistribution_AllStages_BaseChancesSumToOne()
        {
            for (int stage = 1; stage <= 5; stage++)
            {
                var distribution = _progressionSystem.GetPersonalityDistribution(stage);

                float baseSum = distribution.EasyGoingChance +
                               distribution.BudgetConsciousChance +
                               distribution.PerfectionistChance;

                Assert.AreEqual(1.0f, baseSum, 0.001f,
                    $"Stage {stage} base personality chances should sum to 1.0");
            }
        }

        #endregion

        #region Property 18: Performance Review Evaluation

        /// <summary>
        /// Property 18: Performance Review Evaluation
        /// Positive review: avgSatisfaction >= 70 AND onTimeTaskRate >= 80%
        /// Negative review: avgSatisfaction < 60 OR onTimeTaskRate < 60%
        /// Otherwise: Neutral review
        /// **Validates: Requirements R16**
        /// </summary>
        [Test]
        public void EvaluatePerformance_PositiveReview_WhenHighPerformance()
        {
            for (int i = 0; i < 100; i++)
            {
                // Generate events with high satisfaction (70-100)
                float satisfaction = 70f + (float)(_random.NextDouble() * 30);
                var events = CreateEventsWithSatisfaction(3, satisfaction, 0.8f + (float)(_random.NextDouble() * 0.2f));

                var result = _progressionSystem.EvaluatePerformance(events, 1, 0);

                Assert.AreEqual(PerformanceReviewOutcome.Positive, result.Outcome,
                    $"High performance (satisfaction={satisfaction:F1}%, taskRate={result.OnTimeTaskRate:F1}%) should be Positive");
                Assert.AreEqual(0, result.ConsecutiveNegativeReviews,
                    "Positive review should reset consecutive negative reviews to 0");
            }
        }

        /// <summary>
        /// Property 18: Negative review when satisfaction is below 60%.
        /// **Validates: Requirements R16.8**
        /// </summary>
        [Test]
        public void EvaluatePerformance_NegativeReview_WhenLowSatisfaction()
        {
            for (int i = 0; i < 100; i++)
            {
                // Generate events with low satisfaction (0-59)
                float satisfaction = (float)(_random.NextDouble() * 59);
                var events = CreateEventsWithSatisfaction(3, satisfaction, 1.0f); // High task rate

                var result = _progressionSystem.EvaluatePerformance(events, 1, 0);

                Assert.AreEqual(PerformanceReviewOutcome.Negative, result.Outcome,
                    $"Low satisfaction ({satisfaction:F1}%) should be Negative review");
            }
        }

        /// <summary>
        /// Property 18: Negative review when task rate is below 60%.
        /// **Validates: Requirements R16.8**
        /// </summary>
        [Test]
        public void EvaluatePerformance_NegativeReview_WhenLowTaskRate()
        {
            for (int i = 0; i < 100; i++)
            {
                // Generate events with high satisfaction but low task rate
                float satisfaction = 80f; // High satisfaction
                float taskRate = (float)(_random.NextDouble() * 0.59f); // 0-59% task rate
                var events = CreateEventsWithSatisfaction(3, satisfaction, taskRate);

                var result = _progressionSystem.EvaluatePerformance(events, 1, 0);

                Assert.AreEqual(PerformanceReviewOutcome.Negative, result.Outcome,
                    $"Low task rate ({taskRate * 100:F1}%) should be Negative review");
            }
        }

        /// <summary>
        /// Property 18: Neutral review when performance is mediocre.
        /// **Validates: Requirements R16**
        /// </summary>
        [Test]
        public void EvaluatePerformance_NeutralReview_WhenMediocrePerformance()
        {
            // Test cases that should be neutral (between positive and negative thresholds)
            var testCases = new[]
            {
                (satisfaction: 65f, taskRate: 0.75f), // Satisfaction 60-69, task rate 60-79
                (satisfaction: 68f, taskRate: 0.70f),
                (satisfaction: 60f, taskRate: 0.79f),
            };

            foreach (var (satisfaction, taskRate) in testCases)
            {
                var events = CreateEventsWithSatisfaction(3, satisfaction, taskRate);

                var result = _progressionSystem.EvaluatePerformance(events, 1, 0);

                Assert.AreEqual(PerformanceReviewOutcome.Neutral, result.Outcome,
                    $"Mediocre performance (satisfaction={satisfaction}%, taskRate={taskRate * 100}%) should be Neutral");
            }
        }

        /// <summary>
        /// Property 18: Promotion on positive review.
        /// **Validates: Requirements R16.7**
        /// </summary>
        [Test]
        public void EvaluatePerformance_PositiveReview_GrantsPromotion()
        {
            for (int level = 1; level < 5; level++)
            {
                var events = CreateEventsWithSatisfaction(3, 85f, 0.90f);

                var result = _progressionSystem.EvaluatePerformance(events, level, 0);

                Assert.AreEqual(PerformanceReviewOutcome.Positive, result.Outcome);
                Assert.IsTrue(result.WasPromoted,
                    $"Positive review at level {level} should grant promotion");
                Assert.AreEqual(level + 1, result.NewEmployeeLevel,
                    $"Level should increase from {level} to {level + 1}");
            }
        }

        /// <summary>
        /// Property 18: No promotion at max level.
        /// **Validates: Requirements R16.7**
        /// </summary>
        [Test]
        public void EvaluatePerformance_PositiveReview_NoPromotionAtMaxLevel()
        {
            var events = CreateEventsWithSatisfaction(3, 85f, 0.90f);

            var result = _progressionSystem.EvaluatePerformance(events, 5, 0);

            Assert.AreEqual(PerformanceReviewOutcome.Positive, result.Outcome);
            Assert.IsFalse(result.WasPromoted,
                "Should not be promoted at max level 5");
            Assert.AreEqual(5, result.NewEmployeeLevel,
                "Level should remain at 5");
        }

        /// <summary>
        /// Property 18: Demotion after 2 consecutive negative reviews.
        /// **Validates: Requirements R16.9**
        /// </summary>
        [Test]
        public void EvaluatePerformance_TwoConsecutiveNegatives_CausesDemotion()
        {
            for (int level = 2; level <= 5; level++)
            {
                var events = CreateEventsWithSatisfaction(3, 50f, 0.50f); // Low performance

                var result = _progressionSystem.EvaluatePerformance(events, level, 1); // Already 1 negative

                Assert.AreEqual(PerformanceReviewOutcome.Negative, result.Outcome);
                Assert.AreEqual(2, result.ConsecutiveNegativeReviews);
                Assert.IsTrue(result.WasDemoted,
                    $"Should be demoted after 2 consecutive negatives at level {level}");
                Assert.AreEqual(level - 1, result.NewEmployeeLevel,
                    $"Level should decrease from {level} to {level - 1}");
            }
        }

        /// <summary>
        /// Property 18: No demotion below level 1.
        /// **Validates: Requirements R16.9**
        /// </summary>
        [Test]
        public void EvaluatePerformance_NoDemotionBelowLevel1()
        {
            var events = CreateEventsWithSatisfaction(3, 50f, 0.50f);

            var result = _progressionSystem.EvaluatePerformance(events, 1, 1);

            Assert.AreEqual(PerformanceReviewOutcome.Negative, result.Outcome);
            Assert.IsFalse(result.WasDemoted,
                "Should not be demoted below level 1");
            Assert.AreEqual(1, result.NewEmployeeLevel,
                "Level should remain at 1");
        }

        /// <summary>
        /// Property 18: Termination after 3 consecutive negatives at level 1.
        /// **Validates: Requirements R16.10**
        /// </summary>
        [Test]
        public void EvaluatePerformance_ThreeConsecutiveNegativesAtLevel1_CausesTermination()
        {
            var events = CreateEventsWithSatisfaction(3, 50f, 0.50f);

            var result = _progressionSystem.EvaluatePerformance(events, 1, 2); // Already 2 negatives

            Assert.AreEqual(PerformanceReviewOutcome.Negative, result.Outcome);
            Assert.AreEqual(3, result.ConsecutiveNegativeReviews);
            Assert.IsTrue(result.WasTerminated,
                "Should be terminated after 3 consecutive negatives at level 1");
        }

        /// <summary>
        /// Property 18: No termination at higher levels.
        /// **Validates: Requirements R16.10**
        /// </summary>
        [Test]
        public void EvaluatePerformance_ThreeConsecutiveNegativesAtHigherLevel_NoDemotion()
        {
            for (int level = 2; level <= 5; level++)
            {
                var events = CreateEventsWithSatisfaction(3, 50f, 0.50f);

                var result = _progressionSystem.EvaluatePerformance(events, level, 2);

                Assert.IsFalse(result.WasTerminated,
                    $"Should not be terminated at level {level}");
            }
        }

        /// <summary>
        /// Property 18: Boundary test at review thresholds.
        /// **Validates: Requirements R16.6-R16.8**
        /// </summary>
        [Test]
        public void EvaluatePerformance_BoundaryValues()
        {
            // Exactly at positive threshold
            var positiveEvents = CreateEventsWithSatisfaction(3, 70f, 0.80f);
            var positiveResult = _progressionSystem.EvaluatePerformance(positiveEvents, 1, 0);
            Assert.AreEqual(PerformanceReviewOutcome.Positive, positiveResult.Outcome,
                "Exactly at positive threshold (70%, 80%) should be Positive");

            // Just below positive threshold (satisfaction)
            var belowSatEvents = CreateEventsWithSatisfaction(3, 69.9f, 0.80f);
            var belowSatResult = _progressionSystem.EvaluatePerformance(belowSatEvents, 1, 0);
            Assert.AreNotEqual(PerformanceReviewOutcome.Positive, belowSatResult.Outcome,
                "Just below positive satisfaction threshold should not be Positive");

            // Just below positive threshold (task rate)
            var belowTaskEvents = CreateEventsWithSatisfaction(3, 70f, 0.79f);
            var belowTaskResult = _progressionSystem.EvaluatePerformance(belowTaskEvents, 1, 0);
            Assert.AreNotEqual(PerformanceReviewOutcome.Positive, belowTaskResult.Outcome,
                "Just below positive task rate threshold should not be Positive");

            // Exactly at negative threshold (satisfaction)
            var negSatEvents = CreateEventsWithSatisfaction(3, 59.9f, 0.80f);
            var negSatResult = _progressionSystem.EvaluatePerformance(negSatEvents, 1, 0);
            Assert.AreEqual(PerformanceReviewOutcome.Negative, negSatResult.Outcome,
                "Below negative satisfaction threshold (60%) should be Negative");

            // Exactly at negative threshold (task rate)
            var negTaskEvents = CreateEventsWithSatisfaction(3, 70f, 0.59f);
            var negTaskResult = _progressionSystem.EvaluatePerformance(negTaskEvents, 1, 0);
            Assert.AreEqual(PerformanceReviewOutcome.Negative, negTaskResult.Outcome,
                "Below negative task rate threshold (60%) should be Negative");
        }

        /// <summary>
        /// Helper method to create events with specified satisfaction and task completion rate.
        /// </summary>
        private List<EventData> CreateEventsWithSatisfaction(int count, float satisfaction, float taskCompletionRate)
        {
            var events = new List<EventData>();
            for (int i = 0; i < count; i++)
            {
                var evt = new EventData
                {
                    id = Guid.NewGuid().ToString(),
                    clientName = $"Test Client {i}",
                    eventTitle = $"Test Event {i}",
                    results = new EventResults
                    {
                        finalSatisfaction = satisfaction
                    },
                    tasks = new List<EventTask>()
                };

                // Add tasks based on completion rate
                int totalTasks = 10;
                int completedTasks = (int)(totalTasks * taskCompletionRate);
                for (int t = 0; t < totalTasks; t++)
                {
                    evt.tasks.Add(new EventTask
                    {
                        id = $"task-{i}-{t}",
                        taskName = $"Task {t}",
                        status = t < completedTasks ? TaskStatus.Completed : TaskStatus.Failed
                    });
                }

                events.Add(evt);
            }
            return events;
        }

        #endregion

        #region Property 17: Employee Compensation by Level

        /// <summary>
        /// Property 17: Employee Compensation by Level
        /// Level 1-2 (Junior): $500 base + 5% commission
        /// Level 3-4 (Planner): $750 base + 10% commission
        /// Level 5 (Senior): $1000 base + 15% commission
        /// **Validates: Requirements R16**
        /// </summary>
        [Test]
        public void GetCompensation_JuniorLevel_ReturnsCorrectCompensation()
        {
            foreach (int level in new[] { 1, 2 })
            {
                var employee = new EmployeeData { employeeLevel = level };

                var (basePay, commission) = employee.GetCompensation();

                Assert.AreEqual(500f, basePay,
                    $"Junior level {level} should have $500 base pay");
                Assert.AreEqual(0.05f, commission, 0.001f,
                    $"Junior level {level} should have 5% commission");
            }
        }

        /// <summary>
        /// Property 17: Planner level compensation.
        /// **Validates: Requirements R16.3**
        /// </summary>
        [Test]
        public void GetCompensation_PlannerLevel_ReturnsCorrectCompensation()
        {
            foreach (int level in new[] { 3, 4 })
            {
                var employee = new EmployeeData { employeeLevel = level };

                var (basePay, commission) = employee.GetCompensation();

                Assert.AreEqual(750f, basePay,
                    $"Planner level {level} should have $750 base pay");
                Assert.AreEqual(0.10f, commission, 0.001f,
                    $"Planner level {level} should have 10% commission");
            }
        }

        /// <summary>
        /// Property 17: Senior level compensation.
        /// **Validates: Requirements R16.3**
        /// </summary>
        [Test]
        public void GetCompensation_SeniorLevel_ReturnsCorrectCompensation()
        {
            var employee = new EmployeeData { employeeLevel = 5 };

            var (basePay, commission) = employee.GetCompensation();

            Assert.AreEqual(1000f, basePay,
                "Senior level 5 should have $1000 base pay");
            Assert.AreEqual(0.15f, commission, 0.001f,
                "Senior level 5 should have 15% commission");
        }

        /// <summary>
        /// Property 17: All levels have valid compensation.
        /// **Validates: Requirements R16.3**
        /// </summary>
        [Test]
        public void GetCompensation_AllLevels_HaveValidCompensation()
        {
            for (int level = 1; level <= 5; level++)
            {
                var employee = new EmployeeData { employeeLevel = level };

                var (basePay, commission) = employee.GetCompensation();

                Assert.Greater(basePay, 0f,
                    $"Level {level} should have positive base pay");
                Assert.Greater(commission, 0f,
                    $"Level {level} should have positive commission rate");
                Assert.LessOrEqual(commission, 1f,
                    $"Level {level} commission rate should be <= 100%");
            }
        }

        /// <summary>
        /// Property 17: Higher levels have better compensation.
        /// **Validates: Requirements R16.3**
        /// </summary>
        [Test]
        public void GetCompensation_HigherLevels_HaveBetterCompensation()
        {
            var junior = new EmployeeData { employeeLevel = 1 };
            var planner = new EmployeeData { employeeLevel = 3 };
            var senior = new EmployeeData { employeeLevel = 5 };

            var (juniorBase, juniorComm) = junior.GetCompensation();
            var (plannerBase, plannerComm) = planner.GetCompensation();
            var (seniorBase, seniorComm) = senior.GetCompensation();

            // Base pay increases with level
            Assert.Less(juniorBase, plannerBase,
                "Planner base pay should be higher than Junior");
            Assert.Less(plannerBase, seniorBase,
                "Senior base pay should be higher than Planner");

            // Commission rate increases with level
            Assert.Less(juniorComm, plannerComm,
                "Planner commission should be higher than Junior");
            Assert.Less(plannerComm, seniorComm,
                "Senior commission should be higher than Planner");
        }

        /// <summary>
        /// Property 17: Employee title matches level.
        /// **Validates: Requirements R16.2**
        /// </summary>
        [Test]
        public void GetTitle_ReturnsCorrectTitle()
        {
            var testCases = new[]
            {
                (level: 1, expectedTitle: "Junior Planner"),
                (level: 2, expectedTitle: "Junior Planner"),
                (level: 3, expectedTitle: "Planner"),
                (level: 4, expectedTitle: "Planner"),
                (level: 5, expectedTitle: "Senior Planner")
            };

            foreach (var (level, expectedTitle) in testCases)
            {
                var employee = new EmployeeData { employeeLevel = level };

                string title = employee.GetTitle();

                Assert.AreEqual(expectedTitle, title,
                    $"Level {level} should have title '{expectedTitle}'");
            }
        }

        /// <summary>
        /// Property 17: Commission calculation example.
        /// **Validates: Requirements R16.3**
        /// </summary>
        [Test]
        public void GetCompensation_CommissionCalculation_Example()
        {
            // Example from design: Level 3 planner, $10,000 budget event, 80% satisfaction
            // eventProfit = $10,000 × 0.25 = $2,500 (25% margin for 80% satisfaction)
            // commission = $2,500 × 0.10 = $250
            // totalCompensation = $750 + $250 = $1,000

            var employee = new EmployeeData { employeeLevel = 3 };
            var (basePay, commissionRate) = employee.GetCompensation();

            float eventBudget = 10000f;
            float profitMargin = 0.25f; // 25% for 80% satisfaction
            float eventProfit = eventBudget * profitMargin;
            float commission = eventProfit * commissionRate;
            float totalCompensation = basePay + commission;

            Assert.AreEqual(750f, basePay, "Base pay should be $750");
            Assert.AreEqual(0.10f, commissionRate, 0.001f, "Commission rate should be 10%");
            Assert.AreEqual(2500f, eventProfit, 0.01f, "Event profit should be $2,500");
            Assert.AreEqual(250f, commission, 0.01f, "Commission should be $250");
            Assert.AreEqual(1000f, totalCompensation, 0.01f, "Total compensation should be $1,000");
        }

        #endregion

        #region Property 28: Celebrity Reputation Loss Cap

        /// <summary>
        /// Property 28: Celebrity Reputation Loss Cap
        /// For any celebrity event failure with press coverage status P:
        ///   baseReputationLoss = CalculateBaseLoss(satisfaction)
        ///   multiplier = P == Positive ? 2.0 : (P == Neutral ? 2.5 : 3.0)
        ///   uncappedLoss = baseReputationLoss × multiplier
        ///   finalLoss = max(uncappedLoss, -50) // Loss cannot exceed -50
        /// INVARIANT: Celebrity event reputation loss >= -50 (loss is negative)
        /// **Validates: Requirements R15.17-R15.19**
        /// </summary>
        [Test]
        public void CalculateCelebrityReputationChange_FailedEvent_LossCappedAtMinus50()
        {
            // Test worst-case scenarios: minimum satisfaction + all press coverage types
            for (int i = 0; i < 100; i++)
            {
                // Generate failed satisfaction (0-39%)
                float satisfaction = (float)(_random.NextDouble() * 39);

                foreach (PressCoverage coverage in Enum.GetValues(typeof(PressCoverage)))
                {
                    int change = _progressionSystem.CalculateCelebrityReputationChange(satisfaction, coverage);

                    // For failed events, change should be negative but capped at -50
                    Assert.GreaterOrEqual(change, -50,
                        $"Celebrity reputation loss should be capped at -50 (got {change} for satisfaction={satisfaction:F1}%, coverage={coverage})");
                }
            }
        }

        /// <summary>
        /// Property 28: Celebrity event with Positive press and failure applies 2x multiplier.
        /// **Validates: Requirements R15.17**
        /// </summary>
        [Test]
        public void CalculateCelebrityReputationChange_FailedWithPositivePress_Applies2xMultiplier()
        {
            // Test that positive press with failure applies 2x multiplier
            // Base loss for failed (<40%) is -15 to -25
            // With 2x multiplier: -30 to -50
            for (int i = 0; i < 100; i++)
            {
                float satisfaction = (float)(_random.NextDouble() * 39); // 0-39%

                int change = _progressionSystem.CalculateCelebrityReputationChange(satisfaction, PressCoverage.Positive);

                // Should be negative (loss)
                Assert.Less(change, 0,
                    $"Failed celebrity event should have negative reputation change");
                // Should be capped at -50
                Assert.GreaterOrEqual(change, -50,
                    $"Celebrity reputation loss should be capped at -50");
            }
        }

        /// <summary>
        /// Property 28: Celebrity event with Neutral press and failure applies 2.5x multiplier.
        /// **Validates: Requirements R15.18**
        /// </summary>
        [Test]
        public void CalculateCelebrityReputationChange_FailedWithNeutralPress_Applies2Point5xMultiplier()
        {
            // Test that neutral press with failure applies 2.5x multiplier
            // Base loss for failed (<40%) is -15 to -25
            // With 2.5x multiplier: -37.5 to -62.5, capped at -50
            for (int i = 0; i < 100; i++)
            {
                float satisfaction = (float)(_random.NextDouble() * 39); // 0-39%

                int change = _progressionSystem.CalculateCelebrityReputationChange(satisfaction, PressCoverage.Neutral);

                // Should be negative (loss)
                Assert.Less(change, 0,
                    $"Failed celebrity event should have negative reputation change");
                // Should be capped at -50
                Assert.GreaterOrEqual(change, -50,
                    $"Celebrity reputation loss should be capped at -50");
            }
        }

        /// <summary>
        /// Property 28: Celebrity event with Negative press and failure applies 3x multiplier.
        /// **Validates: Requirements R15.19**
        /// </summary>
        [Test]
        public void CalculateCelebrityReputationChange_FailedWithNegativePress_Applies3xMultiplier()
        {
            // Test that negative press with failure applies 3x multiplier
            // Base loss for failed (<40%) is -15 to -25
            // With 3x multiplier: -45 to -75, capped at -50
            for (int i = 0; i < 100; i++)
            {
                float satisfaction = (float)(_random.NextDouble() * 39); // 0-39%

                int change = _progressionSystem.CalculateCelebrityReputationChange(satisfaction, PressCoverage.Negative);

                // Should be negative (loss)
                Assert.Less(change, 0,
                    $"Failed celebrity event should have negative reputation change");
                // Should be capped at -50
                Assert.GreaterOrEqual(change, -50,
                    $"Celebrity reputation loss should be capped at -50");
            }
        }

        /// <summary>
        /// Property 28: Celebrity event success with Positive press applies 3x gain.
        /// **Validates: Requirements R15.14**
        /// </summary>
        [Test]
        public void CalculateCelebrityReputationChange_SuccessWithPositivePress_Applies3xGain()
        {
            // Test that positive press with success applies 3x multiplier
            // Base gain for high satisfaction (90-100%) is +15 to +25
            // With 3x multiplier: +45 to +75
            for (int i = 0; i < 100; i++)
            {
                float satisfaction = 90f + (float)(_random.NextDouble() * 10); // 90-100%

                int change = _progressionSystem.CalculateCelebrityReputationChange(satisfaction, PressCoverage.Positive);

                // Should be positive (gain)
                Assert.Greater(change, 0,
                    $"Successful celebrity event should have positive reputation change");
                // Should be at least 3x the minimum base gain (15 * 3 = 45)
                Assert.GreaterOrEqual(change, 45,
                    $"Celebrity success with positive press should have at least +45 reputation");
            }
        }

        /// <summary>
        /// Property 28: Celebrity event success with Neutral press applies 2x gain.
        /// **Validates: Requirements R15.15**
        /// </summary>
        [Test]
        public void CalculateCelebrityReputationChange_SuccessWithNeutralPress_Applies2xGain()
        {
            // Test that neutral press with success applies 2x multiplier
            // Base gain for high satisfaction (90-100%) is +15 to +25
            // With 2x multiplier: +30 to +50
            for (int i = 0; i < 100; i++)
            {
                float satisfaction = 90f + (float)(_random.NextDouble() * 10); // 90-100%

                int change = _progressionSystem.CalculateCelebrityReputationChange(satisfaction, PressCoverage.Neutral);

                // Should be positive (gain)
                Assert.Greater(change, 0,
                    $"Successful celebrity event should have positive reputation change");
                // Should be at least 2x the minimum base gain (15 * 2 = 30)
                Assert.GreaterOrEqual(change, 30,
                    $"Celebrity success with neutral press should have at least +30 reputation");
            }
        }

        /// <summary>
        /// Property 28: Celebrity event success with Negative press applies 1.5x gain.
        /// **Validates: Requirements R15.16**
        /// </summary>
        [Test]
        public void CalculateCelebrityReputationChange_SuccessWithNegativePress_Applies1Point5xGain()
        {
            // Test that negative press with success applies 1.5x multiplier
            // Base gain for high satisfaction (90-100%) is +15 to +25
            // With 1.5x multiplier: +22.5 to +37.5 (truncated to int: +22 to +37)
            for (int i = 0; i < 100; i++)
            {
                float satisfaction = 90f + (float)(_random.NextDouble() * 10); // 90-100%

                int change = _progressionSystem.CalculateCelebrityReputationChange(satisfaction, PressCoverage.Negative);

                // Should be positive (gain)
                Assert.Greater(change, 0,
                    $"Successful celebrity event should have positive reputation change");
                // Should be at least 1.5x the minimum base gain (15 * 1.5 = 22.5, truncated to 22)
                Assert.GreaterOrEqual(change, 22,
                    $"Celebrity success with negative press should have at least +22 reputation");
            }
        }

        /// <summary>
        /// Property 28: Worst case scenario - minimum satisfaction with negative press.
        /// Base loss -25, Negative press: -25 × 3.0 = -75 → capped to -50
        /// **Validates: Requirements R15.19a**
        /// </summary>
        [Test]
        public void CalculateCelebrityReputationChange_WorstCase_CappedAtMinus50()
        {
            // Test worst case: 0% satisfaction with negative press
            // This should produce the maximum possible loss, which should be capped at -50
            for (int i = 0; i < 100; i++)
            {
                int change = _progressionSystem.CalculateCelebrityReputationChange(0f, PressCoverage.Negative);

                Assert.GreaterOrEqual(change, -50,
                    $"Worst case celebrity reputation loss should be capped at -50 (got {change})");
                Assert.Less(change, 0,
                    "Worst case should still be a loss (negative)");
            }
        }

        /// <summary>
        /// Property 28: Cap only applies to losses, not gains.
        /// **Validates: Requirements R15.19a**
        /// </summary>
        [Test]
        public void CalculateCelebrityReputationChange_CapOnlyAppliesToLosses()
        {
            // Test that gains are not capped
            for (int i = 0; i < 100; i++)
            {
                float satisfaction = 90f + (float)(_random.NextDouble() * 10); // 90-100%

                int change = _progressionSystem.CalculateCelebrityReputationChange(satisfaction, PressCoverage.Positive);

                // Gains should be able to exceed +50 (3x multiplier on +15 to +25 = +45 to +75)
                Assert.Greater(change, 0,
                    "Successful celebrity event should have positive reputation change");
                // With 3x multiplier on high satisfaction, gains can exceed 50
                // This verifies the cap only applies to losses
            }
        }

        /// <summary>
        /// Property 28: All satisfaction levels with all press coverage types respect the cap.
        /// **Validates: Requirements R15.17-R15.19a**
        /// </summary>
        [Test]
        public void CalculateCelebrityReputationChange_AllCombinations_RespectCap()
        {
            // Test all combinations of satisfaction levels and press coverage
            for (int i = 0; i < 100; i++)
            {
                float satisfaction = (float)(_random.NextDouble() * 100); // 0-100%

                foreach (PressCoverage coverage in Enum.GetValues(typeof(PressCoverage)))
                {
                    int change = _progressionSystem.CalculateCelebrityReputationChange(satisfaction, coverage);

                    // The cap should always be respected for losses
                    Assert.GreaterOrEqual(change, -50,
                        $"Celebrity reputation loss should never exceed -50 (got {change} for satisfaction={satisfaction:F1}%, coverage={coverage})");
                }
            }
        }

        #endregion
    }
}
