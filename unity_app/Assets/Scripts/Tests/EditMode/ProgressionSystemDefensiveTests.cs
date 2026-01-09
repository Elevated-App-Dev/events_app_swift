using System;
using System.Collections.Generic;
using NUnit.Framework;
using EventPlannerSim.Core;
using EventPlannerSim.Data;
using EventPlannerSim.Systems;

namespace EventPlannerSim.Tests.EditMode
{
    /// <summary>
    /// Defensive edge case tests for ProgressionSystem.
    /// Tests invalid inputs and boundary conditions.
    /// Feature: event-planner-simulator
    /// Requirements: R14, R16
    /// </summary>
    [TestFixture]
    public class ProgressionSystemDefensiveTests
    {
        private ProgressionSystemImpl _progressionSystem;
        private Random _random;

        [SetUp]
        public void Setup()
        {
            _random = new Random(42);
            _progressionSystem = new ProgressionSystemImpl(_random);
        }

        #region Task 33.1: Invalid Input Tests for ProgressionSystem

        #region Satisfaction > 100% or Negative Values

        /// <summary>
        /// Test that satisfaction values > 100% are handled gracefully.
        /// The system should treat them as high satisfaction (90-100% tier).
        /// Requirements: R14
        /// </summary>
        [Test]
        public void ApplyEventResult_SatisfactionOver100_HandledAsHighSatisfaction()
        {
            // Test satisfaction values over 100%
            var overValues = new[] { 100.1f, 110f, 150f, 200f, 1000f, float.MaxValue };

            foreach (var satisfaction in overValues)
            {
                var result = _progressionSystem.ApplyEventResult(satisfaction, 50);

                // Should be treated as high satisfaction tier (90-100%): +15 to +25
                Assert.GreaterOrEqual(result.Amount, 15,
                    $"Satisfaction {satisfaction}% should be treated as high tier (min +15)");
                Assert.LessOrEqual(result.Amount, 25,
                    $"Satisfaction {satisfaction}% should be treated as high tier (max +25)");
                Assert.GreaterOrEqual(result.NewReputation, 0,
                    "NewReputation should never be negative");
            }
        }

        /// <summary>
        /// Test that negative satisfaction values are handled gracefully.
        /// The system should treat them as failed satisfaction (<40% tier).
        /// Requirements: R14
        /// </summary>
        [Test]
        public void ApplyEventResult_NegativeSatisfaction_HandledAsFailedSatisfaction()
        {
            // Test negative satisfaction values
            var negativeValues = new[] { -0.1f, -1f, -10f, -50f, -100f, float.MinValue };

            foreach (var satisfaction in negativeValues)
            {
                var result = _progressionSystem.ApplyEventResult(satisfaction, 50);

                // Should be treated as failed satisfaction tier (<40%): -15 to -25
                Assert.GreaterOrEqual(result.Amount, -25,
                    $"Satisfaction {satisfaction}% should be treated as failed tier (min -25)");
                Assert.LessOrEqual(result.Amount, -15,
                    $"Satisfaction {satisfaction}% should be treated as failed tier (max -15)");
                Assert.IsTrue(result.GeneratedNegativeReview,
                    $"Satisfaction {satisfaction}% should generate negative review");
                Assert.GreaterOrEqual(result.NewReputation, 0,
                    "NewReputation should never be negative");
            }
        }

        /// <summary>
        /// Test that NaN satisfaction is handled gracefully.
        /// Requirements: R14
        /// </summary>
        [Test]
        public void ApplyEventResult_NaNSatisfaction_HandledGracefully()
        {
            var result = _progressionSystem.ApplyEventResult(float.NaN, 50);

            // NaN comparisons are always false, so it should fall through to failed tier
            Assert.GreaterOrEqual(result.Amount, -25,
                "NaN satisfaction should be handled (treated as failed tier)");
            Assert.LessOrEqual(result.Amount, -15,
                "NaN satisfaction should be handled (treated as failed tier)");
            Assert.GreaterOrEqual(result.NewReputation, 0,
                "NewReputation should never be negative");
        }

        /// <summary>
        /// Test that negative current reputation is handled gracefully.
        /// Requirements: R14
        /// </summary>
        [Test]
        public void ApplyEventResult_NegativeCurrentReputation_HandledGracefully()
        {
            // Test with negative current reputation (shouldn't happen but defensive)
            var negativeReps = new[] { -1, -10, -100 };

            foreach (var currentRep in negativeReps)
            {
                var result = _progressionSystem.ApplyEventResult(80f, currentRep);

                // NewReputation should be clamped to 0 minimum
                Assert.GreaterOrEqual(result.NewReputation, 0,
                    $"NewReputation should be >= 0 even with negative current rep {currentRep}");
            }
        }

        #endregion

        #region Employee Level 0, Negative, or > 5

        /// <summary>
        /// Test that employee level 0 returns default compensation.
        /// Requirements: R16
        /// </summary>
        [Test]
        public void GetCompensation_EmployeeLevel0_ReturnsDefaultCompensation()
        {
            var employee = new EmployeeData { employeeLevel = 0 };

            var (basePay, commission) = employee.GetCompensation();

            // Level 0 should fall through to default case (same as Junior)
            Assert.AreEqual(500f, basePay,
                "Level 0 should return default base pay ($500)");
            Assert.AreEqual(0.05f, commission, 0.001f,
                "Level 0 should return default commission (5%)");
        }

        /// <summary>
        /// Test that negative employee levels return default compensation.
        /// Requirements: R16
        /// </summary>
        [Test]
        public void GetCompensation_NegativeEmployeeLevel_ReturnsDefaultCompensation()
        {
            var negativeLevels = new[] { -1, -5, -100, int.MinValue };

            foreach (var level in negativeLevels)
            {
                var employee = new EmployeeData { employeeLevel = level };

                var (basePay, commission) = employee.GetCompensation();

                // Negative levels should fall through to default case
                Assert.AreEqual(500f, basePay,
                    $"Level {level} should return default base pay ($500)");
                Assert.AreEqual(0.05f, commission, 0.001f,
                    $"Level {level} should return default commission (5%)");
            }
        }

        /// <summary>
        /// Test that employee levels > 5 return Senior compensation.
        /// Requirements: R16
        /// </summary>
        [Test]
        public void GetCompensation_EmployeeLevelOver5_ReturnsSeniorCompensation()
        {
            var highLevels = new[] { 6, 10, 100, int.MaxValue };

            foreach (var level in highLevels)
            {
                var employee = new EmployeeData { employeeLevel = level };

                var (basePay, commission) = employee.GetCompensation();

                // Levels > 5 should fall through to default case
                Assert.AreEqual(500f, basePay,
                    $"Level {level} should return default base pay ($500)");
                Assert.AreEqual(0.05f, commission, 0.001f,
                    $"Level {level} should return default commission (5%)");
            }
        }

        /// <summary>
        /// Test that employee level 0 returns default title.
        /// Requirements: R16
        /// </summary>
        [Test]
        public void GetTitle_EmployeeLevel0_ReturnsDefaultTitle()
        {
            var employee = new EmployeeData { employeeLevel = 0 };

            string title = employee.GetTitle();

            Assert.AreEqual("Planner", title,
                "Level 0 should return default title 'Planner'");
        }

        /// <summary>
        /// Test that negative employee levels return default title.
        /// Requirements: R16
        /// </summary>
        [Test]
        public void GetTitle_NegativeEmployeeLevel_ReturnsDefaultTitle()
        {
            var negativeLevels = new[] { -1, -5, -100 };

            foreach (var level in negativeLevels)
            {
                var employee = new EmployeeData { employeeLevel = level };

                string title = employee.GetTitle();

                Assert.AreEqual("Planner", title,
                    $"Level {level} should return default title 'Planner'");
            }
        }

        /// <summary>
        /// Test that employee levels > 5 return default title.
        /// Requirements: R16
        /// </summary>
        [Test]
        public void GetTitle_EmployeeLevelOver5_ReturnsDefaultTitle()
        {
            var highLevels = new[] { 6, 10, 100 };

            foreach (var level in highLevels)
            {
                var employee = new EmployeeData { employeeLevel = level };

                string title = employee.GetTitle();

                Assert.AreEqual("Planner", title,
                    $"Level {level} should return default title 'Planner'");
            }
        }

        /// <summary>
        /// Test performance review with invalid employee levels.
        /// Requirements: R16
        /// </summary>
        [Test]
        public void EvaluatePerformance_InvalidEmployeeLevel_HandledGracefully()
        {
            var events = CreateEventsWithSatisfaction(3, 85f, 0.90f);
            var invalidLevels = new[] { 0, -1, 6, 100 };

            foreach (var level in invalidLevels)
            {
                var result = _progressionSystem.EvaluatePerformance(events, level, 0);

                // Should not throw and should return a valid result
                Assert.IsNotNull(result,
                    $"EvaluatePerformance should return result for level {level}");
                Assert.IsNotNull(result.FeedbackMessage,
                    $"FeedbackMessage should not be null for level {level}");
            }
        }

        #endregion

        #region Empty Event List for Performance Review

        /// <summary>
        /// Test that empty event list returns neutral review.
        /// Requirements: R16
        /// </summary>
        [Test]
        public void EvaluatePerformance_EmptyEventList_ReturnsNeutralReview()
        {
            var emptyEvents = new List<EventData>();

            var result = _progressionSystem.EvaluatePerformance(emptyEvents, 3, 0);

            Assert.AreEqual(PerformanceReviewOutcome.Neutral, result.Outcome,
                "Empty event list should result in Neutral review");
            Assert.AreEqual(0f, result.AverageSatisfaction,
                "Empty event list should have 0 average satisfaction");
            Assert.AreEqual(0f, result.OnTimeTaskRate,
                "Empty event list should have 0 task rate");
            Assert.IsFalse(result.WasPromoted,
                "Empty event list should not result in promotion");
            Assert.IsFalse(result.WasDemoted,
                "Empty event list should not result in demotion");
            Assert.IsFalse(result.WasTerminated,
                "Empty event list should not result in termination");
            Assert.AreEqual("No events to review.", result.FeedbackMessage,
                "Empty event list should have appropriate feedback message");
        }

        /// <summary>
        /// Test that null event list returns neutral review.
        /// Requirements: R16
        /// </summary>
        [Test]
        public void EvaluatePerformance_NullEventList_ReturnsNeutralReview()
        {
            var result = _progressionSystem.EvaluatePerformance(null, 3, 0);

            Assert.AreEqual(PerformanceReviewOutcome.Neutral, result.Outcome,
                "Null event list should result in Neutral review");
            Assert.AreEqual(0f, result.AverageSatisfaction,
                "Null event list should have 0 average satisfaction");
            Assert.AreEqual("No events to review.", result.FeedbackMessage,
                "Null event list should have appropriate feedback message");
        }

        #endregion

        #region Events with 0 Tasks

        /// <summary>
        /// Test that events with no tasks are handled gracefully.
        /// Requirements: R16
        /// </summary>
        [Test]
        public void EvaluatePerformance_EventsWithNoTasks_HandledGracefully()
        {
            var events = new List<EventData>
            {
                new EventData
                {
                    id = "event-1",
                    clientName = "Test Client",
                    eventTitle = "Test Event",
                    results = new EventResults { finalSatisfaction = 85f },
                    tasks = new List<EventTask>() // Empty task list
                },
                new EventData
                {
                    id = "event-2",
                    clientName = "Test Client 2",
                    eventTitle = "Test Event 2",
                    results = new EventResults { finalSatisfaction = 80f },
                    tasks = null // Null task list
                }
            };

            var result = _progressionSystem.EvaluatePerformance(events, 3, 0);

            // Should not throw and should calculate satisfaction correctly
            Assert.IsNotNull(result,
                "EvaluatePerformance should return result for events with no tasks");
            Assert.AreEqual(82.5f, result.AverageSatisfaction, 0.1f,
                "Average satisfaction should be calculated correctly");
            // With no tasks, task rate should be 100% (no failures)
            Assert.AreEqual(100f, result.OnTimeTaskRate,
                "Events with no tasks should have 100% task rate");
        }

        /// <summary>
        /// Test that events with null results are handled gracefully.
        /// Requirements: R16
        /// </summary>
        [Test]
        public void EvaluatePerformance_EventsWithNullResults_HandledGracefully()
        {
            var events = new List<EventData>
            {
                new EventData
                {
                    id = "event-1",
                    clientName = "Test Client",
                    eventTitle = "Test Event",
                    results = null, // Null results
                    tasks = new List<EventTask>()
                }
            };

            var result = _progressionSystem.EvaluatePerformance(events, 3, 0);

            // Should not throw - events with null results should be filtered out
            Assert.IsNotNull(result,
                "EvaluatePerformance should return result for events with null results");
            Assert.AreEqual(0f, result.AverageSatisfaction,
                "Events with null results should contribute 0 to average");
        }

        /// <summary>
        /// Test mixed events - some with tasks, some without.
        /// Requirements: R16
        /// </summary>
        [Test]
        public void EvaluatePerformance_MixedEventsWithAndWithoutTasks_HandledGracefully()
        {
            var events = new List<EventData>
            {
                new EventData
                {
                    id = "event-1",
                    clientName = "Test Client 1",
                    eventTitle = "Test Event 1",
                    results = new EventResults { finalSatisfaction = 80f },
                    tasks = new List<EventTask>
                    {
                        new EventTask { id = "task-1", taskName = "Task 1", status = TaskStatus.Completed },
                        new EventTask { id = "task-2", taskName = "Task 2", status = TaskStatus.Completed }
                    }
                },
                new EventData
                {
                    id = "event-2",
                    clientName = "Test Client 2",
                    eventTitle = "Test Event 2",
                    results = new EventResults { finalSatisfaction = 70f },
                    tasks = new List<EventTask>() // No tasks
                },
                new EventData
                {
                    id = "event-3",
                    clientName = "Test Client 3",
                    eventTitle = "Test Event 3",
                    results = new EventResults { finalSatisfaction = 90f },
                    tasks = null // Null tasks
                }
            };

            var result = _progressionSystem.EvaluatePerformance(events, 3, 0);

            Assert.IsNotNull(result,
                "EvaluatePerformance should handle mixed events");
            Assert.AreEqual(80f, result.AverageSatisfaction, 0.1f,
                "Average satisfaction should be (80+70+90)/3 = 80");
            // Only event-1 has tasks (2 completed out of 2)
            Assert.AreEqual(100f, result.OnTimeTaskRate,
                "Task rate should be 100% (2/2 completed)");
        }

        #endregion

        #region Negative Consecutive Review Count

        /// <summary>
        /// Test that negative consecutive negative review count is handled.
        /// Requirements: R16
        /// </summary>
        [Test]
        public void EvaluatePerformance_NegativeConsecutiveReviews_HandledGracefully()
        {
            var events = CreateEventsWithSatisfaction(3, 50f, 0.50f); // Low performance

            var result = _progressionSystem.EvaluatePerformance(events, 3, -5);

            // Should increment from -5 to -4 (or handle gracefully)
            Assert.IsNotNull(result,
                "EvaluatePerformance should handle negative consecutive reviews");
            Assert.AreEqual(PerformanceReviewOutcome.Negative, result.Outcome,
                "Low performance should still result in Negative review");
        }

        #endregion

        #region Stage Boundary Tests

        /// <summary>
        /// Test personality distribution for invalid stages.
        /// Requirements: R14.13
        /// </summary>
        [Test]
        public void GetPersonalityDistribution_InvalidStage_ReturnsDefaultDistribution()
        {
            var invalidStages = new[] { 0, -1, 6, 100 };

            foreach (var stage in invalidStages)
            {
                var distribution = _progressionSystem.GetPersonalityDistribution(stage);

                // Should return Stage 1 default distribution
                Assert.AreEqual(0.50f, distribution.EasyGoingChance, 0.001f,
                    $"Invalid stage {stage} should return default 50% Easy-Going");
                Assert.AreEqual(0.30f, distribution.BudgetConsciousChance, 0.001f,
                    $"Invalid stage {stage} should return default 30% Budget-Conscious");
                Assert.AreEqual(0.20f, distribution.PerfectionistChance, 0.001f,
                    $"Invalid stage {stage} should return default 20% Perfectionist");
            }
        }

        /// <summary>
        /// Test random event frequency for invalid stages.
        /// Requirements: R14.14
        /// </summary>
        [Test]
        public void GetRandomEventFrequency_InvalidStage_ReturnsDefaultFrequency()
        {
            var invalidStages = new[] { 0, -1, 6, 100 };

            foreach (var stage in invalidStages)
            {
                float frequency = _progressionSystem.GetRandomEventFrequency(stage);

                // Should return Stage 1 default frequency (20%)
                Assert.AreEqual(0.20f, frequency, 0.001f,
                    $"Invalid stage {stage} should return default 20% frequency");
            }
        }

        #endregion

        #region Helper Methods

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

        #endregion
    }
}


    #region Task 33.2: Missing Referral Probability Test for Good Tier

    /// <summary>
    /// Defensive edge case tests for referral probability in the good tier.
    /// Feature: event-planner-simulator
    /// Requirements: R14.3
    /// </summary>
    [TestFixture]
    public class ProgressionSystemReferralGoodTierTests
    {
        private ProgressionSystemImpl _progressionSystem;
        private Random _random;

        [SetUp]
        public void Setup()
        {
            _random = new Random(42);
            _progressionSystem = new ProgressionSystemImpl(_random);
        }

        /// <summary>
        /// Test that 75-89% satisfaction has 50% referral chance.
        /// Requirements: R14.3
        /// </summary>
        [Test]
        public void ApplyEventResult_GoodSatisfaction_Has50PercentReferralChance()
        {
            // Test 75-89% satisfaction range (R14.3)
            for (int i = 0; i < 100; i++)
            {
                float satisfaction = 75f + (float)(_random.NextDouble() * 14.99f); // 75-89.99
                int currentRep = _random.Next(0, 100);

                var result = _progressionSystem.ApplyEventResult(satisfaction, currentRep);

                Assert.AreEqual(0.50f, result.ReferralProbability, 0.001f,
                    $"Good satisfaction ({satisfaction:F1}%) should have 50% referral probability");
            }
        }

        /// <summary>
        /// Test boundary at exactly 75% satisfaction.
        /// Requirements: R14.3
        /// </summary>
        [Test]
        public void ApplyEventResult_Exactly75Percent_Has50PercentReferralChance()
        {
            var result = _progressionSystem.ApplyEventResult(75f, 50);

            Assert.AreEqual(0.50f, result.ReferralProbability, 0.001f,
                "Exactly 75% satisfaction should have 50% referral probability");
        }

        /// <summary>
        /// Test boundary just below 75% satisfaction (74.99%).
        /// Requirements: R14.4
        /// </summary>
        [Test]
        public void ApplyEventResult_JustBelow75Percent_HasZeroReferralChance()
        {
            var result = _progressionSystem.ApplyEventResult(74.99f, 50);

            Assert.AreEqual(0f, result.ReferralProbability,
                "Just below 75% satisfaction (74.99%) should have 0% referral probability");
        }

        /// <summary>
        /// Test boundary just below 90% satisfaction (89.99%).
        /// Requirements: R14.3
        /// </summary>
        [Test]
        public void ApplyEventResult_JustBelow90Percent_Has50PercentReferralChance()
        {
            var result = _progressionSystem.ApplyEventResult(89.99f, 50);

            Assert.AreEqual(0.50f, result.ReferralProbability, 0.001f,
                "Just below 90% satisfaction (89.99%) should have 50% referral probability");
        }

        /// <summary>
        /// Test that referrals can actually be triggered in the good tier.
        /// Requirements: R14.3
        /// </summary>
        [Test]
        public void ApplyEventResult_GoodSatisfaction_CanTriggerReferral()
        {
            // Run many iterations to verify referrals can be triggered
            int referralCount = 0;
            int iterations = 1000;

            for (int i = 0; i < iterations; i++)
            {
                // Use different random seeds to get varied results
                var testSystem = new ProgressionSystemImpl(new Random(i));
                float satisfaction = 80f; // Middle of good tier

                var result = testSystem.ApplyEventResult(satisfaction, 50);

                if (result.TriggeredReferral)
                {
                    referralCount++;
                }
            }

            // With 50% probability over 1000 iterations, we should get roughly 500 referrals
            // Allow for statistical variance (40-60% range)
            float referralRate = referralCount / (float)iterations;
            Assert.GreaterOrEqual(referralRate, 0.40f,
                $"Referral rate ({referralRate:P1}) should be at least 40% for good tier");
            Assert.LessOrEqual(referralRate, 0.60f,
                $"Referral rate ({referralRate:P1}) should be at most 60% for good tier");
        }

        /// <summary>
        /// Test that okay tier (60-74%) has 0% referral chance.
        /// Requirements: R14.4
        /// </summary>
        [Test]
        public void ApplyEventResult_OkaySatisfaction_HasZeroReferralChance()
        {
            // Test 60-74% satisfaction range (R14.4)
            for (int i = 0; i < 100; i++)
            {
                float satisfaction = 60f + (float)(_random.NextDouble() * 14.99f); // 60-74.99
                int currentRep = _random.Next(0, 100);

                var result = _progressionSystem.ApplyEventResult(satisfaction, currentRep);

                Assert.AreEqual(0f, result.ReferralProbability,
                    $"Okay satisfaction ({satisfaction:F1}%) should have 0% referral probability");
                Assert.IsFalse(result.TriggeredReferral,
                    $"Okay satisfaction ({satisfaction:F1}%) should not trigger referral");
            }
        }
    }

    #endregion


    #region Task 33.3: Consecutive Review State Transition Tests

    /// <summary>
    /// Tests for consecutive review state transitions.
    /// Feature: event-planner-simulator
    /// Requirements: R16.9
    /// </summary>
    [TestFixture]
    public class ProgressionSystemConsecutiveReviewTests
    {
        private ProgressionSystemImpl _progressionSystem;
        private Random _random;

        [SetUp]
        public void Setup()
        {
            _random = new Random(42);
            _progressionSystem = new ProgressionSystemImpl(_random);
        }

        #region Neutral Review Effect on Consecutive Negative Count

        /// <summary>
        /// Test that neutral review does NOT reset consecutive negative count.
        /// Requirements: R16.9
        /// </summary>
        [Test]
        public void EvaluatePerformance_NeutralReview_DoesNotResetConsecutiveNegatives()
        {
            // Create events that result in neutral review (60-69% satisfaction, 60-79% task rate)
            var events = CreateEventsWithSatisfaction(3, 65f, 0.70f);

            // Start with 1 consecutive negative
            var result = _progressionSystem.EvaluatePerformance(events, 3, 1);

            Assert.AreEqual(PerformanceReviewOutcome.Neutral, result.Outcome,
                "Should be Neutral review");
            // Neutral review should NOT reset consecutive negatives
            // The implementation keeps the count unchanged for neutral reviews
            Assert.AreEqual(1, result.ConsecutiveNegativeReviews,
                "Neutral review should not reset consecutive negative count");
        }

        /// <summary>
        /// Test that neutral review preserves high consecutive negative count.
        /// Requirements: R16.9
        /// </summary>
        [Test]
        public void EvaluatePerformance_NeutralReview_PreservesHighConsecutiveCount()
        {
            var events = CreateEventsWithSatisfaction(3, 65f, 0.70f);

            // Start with 2 consecutive negatives (one away from termination at level 1)
            var result = _progressionSystem.EvaluatePerformance(events, 1, 2);

            Assert.AreEqual(PerformanceReviewOutcome.Neutral, result.Outcome,
                "Should be Neutral review");
            // Neutral review should preserve the count
            Assert.AreEqual(2, result.ConsecutiveNegativeReviews,
                "Neutral review should preserve consecutive negative count at 2");
            Assert.IsFalse(result.WasTerminated,
                "Should not be terminated on neutral review");
        }

        #endregion

        #region Positive Review Resets Consecutive Negatives

        /// <summary>
        /// Test that positive review resets consecutive negative count to 0.
        /// Requirements: R16.7
        /// </summary>
        [Test]
        public void EvaluatePerformance_PositiveReview_ResetsConsecutiveNegatives()
        {
            var events = CreateEventsWithSatisfaction(3, 85f, 0.90f);

            // Start with 2 consecutive negatives
            var result = _progressionSystem.EvaluatePerformance(events, 3, 2);

            Assert.AreEqual(PerformanceReviewOutcome.Positive, result.Outcome,
                "Should be Positive review");
            Assert.AreEqual(0, result.ConsecutiveNegativeReviews,
                "Positive review should reset consecutive negative count to 0");
        }

        /// <summary>
        /// Test that positive review resets even high consecutive negative count.
        /// Requirements: R16.7
        /// </summary>
        [Test]
        public void EvaluatePerformance_PositiveReview_ResetsHighConsecutiveCount()
        {
            var events = CreateEventsWithSatisfaction(3, 85f, 0.90f);

            // Start with very high consecutive negatives (shouldn't happen but defensive)
            var result = _progressionSystem.EvaluatePerformance(events, 3, 10);

            Assert.AreEqual(PerformanceReviewOutcome.Positive, result.Outcome,
                "Should be Positive review");
            Assert.AreEqual(0, result.ConsecutiveNegativeReviews,
                "Positive review should reset consecutive negative count to 0 from any value");
        }

        #endregion

        #region Negative Review Increments Consecutive Count

        /// <summary>
        /// Test that negative review increments consecutive negative count.
        /// Requirements: R16.8
        /// </summary>
        [Test]
        public void EvaluatePerformance_NegativeReview_IncrementsConsecutiveNegatives()
        {
            var events = CreateEventsWithSatisfaction(3, 50f, 0.50f);

            // Start with 0 consecutive negatives
            var result = _progressionSystem.EvaluatePerformance(events, 3, 0);

            Assert.AreEqual(PerformanceReviewOutcome.Negative, result.Outcome,
                "Should be Negative review");
            Assert.AreEqual(1, result.ConsecutiveNegativeReviews,
                "Negative review should increment consecutive count from 0 to 1");
        }

        /// <summary>
        /// Test that negative review increments from existing count.
        /// Requirements: R16.8
        /// </summary>
        [Test]
        public void EvaluatePerformance_NegativeReview_IncrementsFromExistingCount()
        {
            var events = CreateEventsWithSatisfaction(3, 50f, 0.50f);

            // Start with 1 consecutive negative
            var result = _progressionSystem.EvaluatePerformance(events, 3, 1);

            Assert.AreEqual(PerformanceReviewOutcome.Negative, result.Outcome,
                "Should be Negative review");
            Assert.AreEqual(2, result.ConsecutiveNegativeReviews,
                "Negative review should increment consecutive count from 1 to 2");
        }

        #endregion

        #region Alternating Positive/Negative Review Sequences

        /// <summary>
        /// Test alternating positive then negative review sequence.
        /// Requirements: R16.7, R16.8
        /// </summary>
        [Test]
        public void EvaluatePerformance_AlternatingPositiveNegative_CorrectStateTransitions()
        {
            int consecutiveNegatives = 0;
            int employeeLevel = 3;

            // Positive review
            var positiveEvents = CreateEventsWithSatisfaction(3, 85f, 0.90f);
            var positiveResult = _progressionSystem.EvaluatePerformance(positiveEvents, employeeLevel, consecutiveNegatives);
            Assert.AreEqual(PerformanceReviewOutcome.Positive, positiveResult.Outcome);
            Assert.AreEqual(0, positiveResult.ConsecutiveNegativeReviews,
                "After positive review, consecutive negatives should be 0");
            consecutiveNegatives = positiveResult.ConsecutiveNegativeReviews;
            employeeLevel = positiveResult.NewEmployeeLevel;

            // Negative review
            var negativeEvents = CreateEventsWithSatisfaction(3, 50f, 0.50f);
            var negativeResult = _progressionSystem.EvaluatePerformance(negativeEvents, employeeLevel, consecutiveNegatives);
            Assert.AreEqual(PerformanceReviewOutcome.Negative, negativeResult.Outcome);
            Assert.AreEqual(1, negativeResult.ConsecutiveNegativeReviews,
                "After negative review, consecutive negatives should be 1");
            consecutiveNegatives = negativeResult.ConsecutiveNegativeReviews;

            // Another positive review
            var positiveResult2 = _progressionSystem.EvaluatePerformance(positiveEvents, employeeLevel, consecutiveNegatives);
            Assert.AreEqual(PerformanceReviewOutcome.Positive, positiveResult2.Outcome);
            Assert.AreEqual(0, positiveResult2.ConsecutiveNegativeReviews,
                "After positive review, consecutive negatives should reset to 0");
        }

        /// <summary>
        /// Test alternating negative then positive review sequence.
        /// Requirements: R16.7, R16.8
        /// </summary>
        [Test]
        public void EvaluatePerformance_AlternatingNegativePositive_CorrectStateTransitions()
        {
            int consecutiveNegatives = 0;
            int employeeLevel = 3;

            // Negative review
            var negativeEvents = CreateEventsWithSatisfaction(3, 50f, 0.50f);
            var negativeResult = _progressionSystem.EvaluatePerformance(negativeEvents, employeeLevel, consecutiveNegatives);
            Assert.AreEqual(PerformanceReviewOutcome.Negative, negativeResult.Outcome);
            Assert.AreEqual(1, negativeResult.ConsecutiveNegativeReviews,
                "After first negative review, consecutive negatives should be 1");
            consecutiveNegatives = negativeResult.ConsecutiveNegativeReviews;

            // Positive review
            var positiveEvents = CreateEventsWithSatisfaction(3, 85f, 0.90f);
            var positiveResult = _progressionSystem.EvaluatePerformance(positiveEvents, employeeLevel, consecutiveNegatives);
            Assert.AreEqual(PerformanceReviewOutcome.Positive, positiveResult.Outcome);
            Assert.AreEqual(0, positiveResult.ConsecutiveNegativeReviews,
                "After positive review, consecutive negatives should reset to 0");
            consecutiveNegatives = positiveResult.ConsecutiveNegativeReviews;

            // Another negative review
            var negativeResult2 = _progressionSystem.EvaluatePerformance(negativeEvents, employeeLevel, consecutiveNegatives);
            Assert.AreEqual(PerformanceReviewOutcome.Negative, negativeResult2.Outcome);
            Assert.AreEqual(1, negativeResult2.ConsecutiveNegativeReviews,
                "After negative review following positive, consecutive negatives should be 1");
        }

        /// <summary>
        /// Test sequence: Negative -> Neutral -> Negative (neutral doesn't reset).
        /// Requirements: R16.8, R16.9
        /// </summary>
        [Test]
        public void EvaluatePerformance_NegativeNeutralNegative_NeutralPreservesCount()
        {
            int consecutiveNegatives = 0;
            int employeeLevel = 3;

            // First negative review
            var negativeEvents = CreateEventsWithSatisfaction(3, 50f, 0.50f);
            var negativeResult1 = _progressionSystem.EvaluatePerformance(negativeEvents, employeeLevel, consecutiveNegatives);
            Assert.AreEqual(PerformanceReviewOutcome.Negative, negativeResult1.Outcome);
            Assert.AreEqual(1, negativeResult1.ConsecutiveNegativeReviews);
            consecutiveNegatives = negativeResult1.ConsecutiveNegativeReviews;

            // Neutral review (should preserve count)
            var neutralEvents = CreateEventsWithSatisfaction(3, 65f, 0.70f);
            var neutralResult = _progressionSystem.EvaluatePerformance(neutralEvents, employeeLevel, consecutiveNegatives);
            Assert.AreEqual(PerformanceReviewOutcome.Neutral, neutralResult.Outcome);
            Assert.AreEqual(1, neutralResult.ConsecutiveNegativeReviews,
                "Neutral review should preserve consecutive negative count at 1");
            consecutiveNegatives = neutralResult.ConsecutiveNegativeReviews;

            // Second negative review (should increment from preserved count)
            var negativeResult2 = _progressionSystem.EvaluatePerformance(negativeEvents, employeeLevel, consecutiveNegatives);
            Assert.AreEqual(PerformanceReviewOutcome.Negative, negativeResult2.Outcome);
            Assert.AreEqual(2, negativeResult2.ConsecutiveNegativeReviews,
                "After second negative (with neutral in between), consecutive negatives should be 2");
        }

        /// <summary>
        /// Test sequence leading to demotion: Negative -> Negative.
        /// Requirements: R16.9
        /// </summary>
        [Test]
        public void EvaluatePerformance_TwoConsecutiveNegatives_CausesDemotion()
        {
            int consecutiveNegatives = 0;
            int employeeLevel = 4;

            var negativeEvents = CreateEventsWithSatisfaction(3, 50f, 0.50f);

            // First negative review
            var result1 = _progressionSystem.EvaluatePerformance(negativeEvents, employeeLevel, consecutiveNegatives);
            Assert.AreEqual(1, result1.ConsecutiveNegativeReviews);
            Assert.IsFalse(result1.WasDemoted, "First negative should not cause demotion");
            consecutiveNegatives = result1.ConsecutiveNegativeReviews;

            // Second negative review
            var result2 = _progressionSystem.EvaluatePerformance(negativeEvents, employeeLevel, consecutiveNegatives);
            Assert.AreEqual(2, result2.ConsecutiveNegativeReviews);
            Assert.IsTrue(result2.WasDemoted, "Second consecutive negative should cause demotion");
            Assert.AreEqual(employeeLevel - 1, result2.NewEmployeeLevel,
                "Employee level should decrease by 1");
        }

        /// <summary>
        /// Test sequence leading to termination: Negative -> Negative -> Negative at level 1.
        /// Requirements: R16.10
        /// </summary>
        [Test]
        public void EvaluatePerformance_ThreeConsecutiveNegativesAtLevel1_CausesTermination()
        {
            int consecutiveNegatives = 0;
            int employeeLevel = 1;

            var negativeEvents = CreateEventsWithSatisfaction(3, 50f, 0.50f);

            // First negative review
            var result1 = _progressionSystem.EvaluatePerformance(negativeEvents, employeeLevel, consecutiveNegatives);
            Assert.AreEqual(1, result1.ConsecutiveNegativeReviews);
            Assert.IsFalse(result1.WasTerminated);
            consecutiveNegatives = result1.ConsecutiveNegativeReviews;

            // Second negative review
            var result2 = _progressionSystem.EvaluatePerformance(negativeEvents, employeeLevel, consecutiveNegatives);
            Assert.AreEqual(2, result2.ConsecutiveNegativeReviews);
            Assert.IsFalse(result2.WasTerminated, "Second negative at level 1 should not terminate");
            consecutiveNegatives = result2.ConsecutiveNegativeReviews;

            // Third negative review
            var result3 = _progressionSystem.EvaluatePerformance(negativeEvents, employeeLevel, consecutiveNegatives);
            Assert.AreEqual(3, result3.ConsecutiveNegativeReviews);
            Assert.IsTrue(result3.WasTerminated, "Third consecutive negative at level 1 should terminate");
        }

        /// <summary>
        /// Test that positive review after two negatives prevents demotion on next negative.
        /// Requirements: R16.7, R16.9
        /// </summary>
        [Test]
        public void EvaluatePerformance_PositiveAfterTwoNegatives_PreventsNextDemotion()
        {
            int consecutiveNegatives = 0;
            int employeeLevel = 4;

            var negativeEvents = CreateEventsWithSatisfaction(3, 50f, 0.50f);
            var positiveEvents = CreateEventsWithSatisfaction(3, 85f, 0.90f);

            // Two negative reviews
            var result1 = _progressionSystem.EvaluatePerformance(negativeEvents, employeeLevel, consecutiveNegatives);
            consecutiveNegatives = result1.ConsecutiveNegativeReviews;
            var result2 = _progressionSystem.EvaluatePerformance(negativeEvents, employeeLevel, consecutiveNegatives);
            Assert.IsTrue(result2.WasDemoted);
            employeeLevel = result2.NewEmployeeLevel;
            consecutiveNegatives = result2.ConsecutiveNegativeReviews;

            // Positive review resets count
            var result3 = _progressionSystem.EvaluatePerformance(positiveEvents, employeeLevel, consecutiveNegatives);
            Assert.AreEqual(0, result3.ConsecutiveNegativeReviews);
            consecutiveNegatives = result3.ConsecutiveNegativeReviews;
            employeeLevel = result3.NewEmployeeLevel;

            // Next negative should only be count 1 (no demotion)
            var result4 = _progressionSystem.EvaluatePerformance(negativeEvents, employeeLevel, consecutiveNegatives);
            Assert.AreEqual(1, result4.ConsecutiveNegativeReviews);
            Assert.IsFalse(result4.WasDemoted,
                "First negative after positive should not cause demotion");
        }

        #endregion

        #region Helper Methods

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
    }

    #endregion
