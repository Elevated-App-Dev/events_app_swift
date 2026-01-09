using System;
using NUnit.Framework;
using EventPlannerSim.Systems;

namespace EventPlannerSim.Tests.EditMode
{
    /// <summary>
    /// Property-based tests for ReferralSystem.
    /// Feature: event-planner-simulator
    /// </summary>
    [TestFixture]
    public class ReferralSystemPropertyTests
    {
        private ReferralSystemImpl _referralSystem;
        private Random _random;

        [SetUp]
        public void Setup()
        {
            // Use fixed seed for reproducibility
            _random = new Random(42);
            _referralSystem = new ReferralSystemImpl(_random);
        }

        #region Property 20: Referral Probability by Satisfaction

        /// <summary>
        /// Property 20: Referral Probability by Satisfaction
        /// For any satisfaction S >= 95: base chance = 80%
        /// For any satisfaction 90 <= S < 95: base chance = 50%
        /// For any satisfaction S < 90: base chance = 0%
        /// **Validates: Requirements R23**
        /// </summary>
        [Test]
        public void CalculateReferralProbability_ExcellentSatisfaction_Returns80PercentBase()
        {
            // Test 95-100% satisfaction range (R23.2)
            for (int i = 0; i < 100; i++)
            {
                float satisfaction = 95f + (float)(_random.NextDouble() * 5); // 95-100
                int streak = 0; // No streak bonus

                float probability = _referralSystem.CalculateReferralProbability(satisfaction, streak);

                Assert.AreEqual(0.80f, probability, 0.001f,
                    $"Excellent satisfaction ({satisfaction:F1}%) with no streak should have 80% referral chance");
            }
        }

        /// <summary>
        /// Property 20: High satisfaction (90-94%) gives 50% base chance.
        /// **Validates: Requirements R23.2**
        /// </summary>
        [Test]
        public void CalculateReferralProbability_HighSatisfaction_Returns50PercentBase()
        {
            // Test 90-94% satisfaction range
            for (int i = 0; i < 100; i++)
            {
                float satisfaction = 90f + (float)(_random.NextDouble() * 4.99f); // 90-94.99
                int streak = 0; // No streak bonus

                float probability = _referralSystem.CalculateReferralProbability(satisfaction, streak);

                Assert.AreEqual(0.50f, probability, 0.001f,
                    $"High satisfaction ({satisfaction:F1}%) with no streak should have 50% referral chance");
            }
        }

        /// <summary>
        /// Property 20: Below 90% satisfaction gives 0% chance.
        /// **Validates: Requirements R23.1**
        /// </summary>
        [Test]
        public void CalculateReferralProbability_BelowThreshold_ReturnsZero()
        {
            // Test 0-89% satisfaction range
            for (int i = 0; i < 100; i++)
            {
                float satisfaction = (float)(_random.NextDouble() * 89.99f); // 0-89.99
                int streak = _random.Next(0, 10); // Any streak

                float probability = _referralSystem.CalculateReferralProbability(satisfaction, streak);

                Assert.AreEqual(0f, probability,
                    $"Below threshold satisfaction ({satisfaction:F1}%) should have 0% referral chance regardless of streak ({streak})");
            }
        }

        /// <summary>
        /// Property 20: Boundary test at satisfaction thresholds.
        /// **Validates: Requirements R23**
        /// </summary>
        [Test]
        public void CalculateReferralProbability_BoundaryValues_CorrectChance()
        {
            // Test at exact boundaries
            var testCases = new[]
            {
                (satisfaction: 100f, streak: 0, expectedChance: 0.80f, description: "100% satisfaction"),
                (satisfaction: 95f, streak: 0, expectedChance: 0.80f, description: "95% satisfaction (excellent threshold)"),
                (satisfaction: 94.99f, streak: 0, expectedChance: 0.50f, description: "94.99% satisfaction (just below excellent)"),
                (satisfaction: 90f, streak: 0, expectedChance: 0.50f, description: "90% satisfaction (high threshold)"),
                (satisfaction: 89.99f, streak: 0, expectedChance: 0f, description: "89.99% satisfaction (just below high)"),
                (satisfaction: 80f, streak: 0, expectedChance: 0f, description: "80% satisfaction"),
                (satisfaction: 0f, streak: 0, expectedChance: 0f, description: "0% satisfaction")
            };

            foreach (var (satisfaction, streak, expectedChance, description) in testCases)
            {
                float probability = _referralSystem.CalculateReferralProbability(satisfaction, streak);

                Assert.AreEqual(expectedChance, probability, 0.001f,
                    $"{description} should have {expectedChance * 100}% referral chance");
            }
        }

        /// <summary>
        /// Property 20: Streak bonus of +10% at 3+ streak.
        /// **Validates: Requirements R23.7**
        /// </summary>
        [Test]
        public void CalculateReferralProbability_Streak3_Adds10Percent()
        {
            // Test streak 3-4 bonus
            foreach (int streak in new[] { 3, 4 })
            {
                // Excellent satisfaction (95%+)
                float excellentSat = 95f + (float)(_random.NextDouble() * 5);
                float excellentProb = _referralSystem.CalculateReferralProbability(excellentSat, streak);
                Assert.AreEqual(0.90f, excellentProb, 0.001f,
                    $"Excellent satisfaction with streak {streak} should have 90% chance (80% + 10%)");

                // High satisfaction (90-94%)
                float highSat = 90f + (float)(_random.NextDouble() * 4.99f);
                float highProb = _referralSystem.CalculateReferralProbability(highSat, streak);
                Assert.AreEqual(0.60f, highProb, 0.001f,
                    $"High satisfaction with streak {streak} should have 60% chance (50% + 10%)");
            }
        }

        /// <summary>
        /// Property 20: Streak bonus of +20% at 5+ streak.
        /// **Validates: Requirements R23.8**
        /// </summary>
        [Test]
        public void CalculateReferralProbability_Streak5Plus_Adds20Percent()
        {
            // Test streak 5+ bonus
            foreach (int streak in new[] { 5, 6, 10, 100 })
            {
                // Excellent satisfaction (95%+)
                float excellentSat = 95f + (float)(_random.NextDouble() * 5);
                float excellentProb = _referralSystem.CalculateReferralProbability(excellentSat, streak);
                Assert.AreEqual(1.0f, excellentProb, 0.001f,
                    $"Excellent satisfaction with streak {streak} should have 100% chance (80% + 20%, capped)");

                // High satisfaction (90-94%)
                float highSat = 90f + (float)(_random.NextDouble() * 4.99f);
                float highProb = _referralSystem.CalculateReferralProbability(highSat, streak);
                Assert.AreEqual(0.70f, highProb, 0.001f,
                    $"High satisfaction with streak {streak} should have 70% chance (50% + 20%)");
            }
        }

        /// <summary>
        /// Property 20: Streak bonus does NOT apply below 90% satisfaction.
        /// **Validates: Requirements R23 (correction in design)**
        /// </summary>
        [Test]
        public void CalculateReferralProbability_BelowThreshold_StreakBonusNotApplied()
        {
            // Test that streak bonus doesn't apply below 90%
            for (int i = 0; i < 100; i++)
            {
                float satisfaction = (float)(_random.NextDouble() * 89.99f); // 0-89.99
                int streak = 5 + _random.Next(0, 10); // High streak

                float probability = _referralSystem.CalculateReferralProbability(satisfaction, streak);

                Assert.AreEqual(0f, probability,
                    $"Below threshold satisfaction ({satisfaction:F1}%) should have 0% chance even with streak {streak}");
            }
        }

        /// <summary>
        /// Property 20: Probability is capped at 100%.
        /// **Validates: Requirements R23**
        /// </summary>
        [Test]
        public void CalculateReferralProbability_CappedAt100Percent()
        {
            // Test that probability never exceeds 1.0
            for (int i = 0; i < 100; i++)
            {
                float satisfaction = 95f + (float)(_random.NextDouble() * 5); // 95-100
                int streak = 5 + _random.Next(0, 100); // Very high streak

                float probability = _referralSystem.CalculateReferralProbability(satisfaction, streak);

                Assert.LessOrEqual(probability, 1.0f,
                    $"Probability should never exceed 100% (got {probability * 100}%)");
                Assert.GreaterOrEqual(probability, 0f,
                    $"Probability should never be negative (got {probability * 100}%)");
            }
        }

        /// <summary>
        /// Property 20: GetBaseReferralChance returns correct values.
        /// **Validates: Requirements R23.2**
        /// </summary>
        [Test]
        public void GetBaseReferralChance_ReturnsCorrectValues()
        {
            var testCases = new[]
            {
                (satisfaction: 100f, expected: 0.80f),
                (satisfaction: 95f, expected: 0.80f),
                (satisfaction: 94.99f, expected: 0.50f),
                (satisfaction: 90f, expected: 0.50f),
                (satisfaction: 89.99f, expected: 0f),
                (satisfaction: 50f, expected: 0f),
                (satisfaction: 0f, expected: 0f)
            };

            foreach (var (satisfaction, expected) in testCases)
            {
                float baseChance = _referralSystem.GetBaseReferralChance(satisfaction);

                Assert.AreEqual(expected, baseChance, 0.001f,
                    $"Base chance for {satisfaction}% satisfaction should be {expected * 100}%");
            }
        }

        /// <summary>
        /// Property 20: QualifiesForReferral returns correct values.
        /// **Validates: Requirements R23.1**
        /// </summary>
        [Test]
        public void QualifiesForReferral_ReturnsCorrectValues()
        {
            // Test qualifying satisfaction (90%+)
            for (int i = 0; i < 100; i++)
            {
                float qualifyingSat = 90f + (float)(_random.NextDouble() * 10); // 90-100
                Assert.IsTrue(_referralSystem.QualifiesForReferral(qualifyingSat),
                    $"Satisfaction {qualifyingSat:F1}% should qualify for referral");
            }

            // Test non-qualifying satisfaction (<90%)
            for (int i = 0; i < 100; i++)
            {
                float nonQualifyingSat = (float)(_random.NextDouble() * 89.99f); // 0-89.99
                Assert.IsFalse(_referralSystem.QualifiesForReferral(nonQualifyingSat),
                    $"Satisfaction {nonQualifyingSat:F1}% should not qualify for referral");
            }
        }

        /// <summary>
        /// Property 20: EvaluateReferral returns consistent results.
        /// **Validates: Requirements R23**
        /// </summary>
        [Test]
        public void EvaluateReferral_ReturnsConsistentResults()
        {
            for (int i = 0; i < 100; i++)
            {
                float satisfaction = (float)(_random.NextDouble() * 100);
                int streak = _random.Next(0, 10);

                var result = _referralSystem.EvaluateReferral(satisfaction, streak);

                // Verify probability matches direct calculation
                float expectedProb = _referralSystem.CalculateReferralProbability(satisfaction, streak);
                Assert.AreEqual(expectedProb, result.Probability, 0.001f,
                    "EvaluateReferral probability should match CalculateReferralProbability");

                // Verify base chance
                float expectedBase = _referralSystem.GetBaseReferralChance(satisfaction);
                Assert.AreEqual(expectedBase, result.BaseChance, 0.001f,
                    "EvaluateReferral base chance should match GetBaseReferralChance");

                // Verify streak bonus
                float expectedBonus = expectedBase > 0 ? _referralSystem.GetStreakBonus(streak) : 0f;
                Assert.AreEqual(expectedBonus, result.StreakBonus, 0.001f,
                    "EvaluateReferral streak bonus should match GetStreakBonus");

                // Verify input values are stored
                Assert.AreEqual(satisfaction, result.Satisfaction,
                    "EvaluateReferral should store satisfaction");
                Assert.AreEqual(streak, result.ExcellenceStreak,
                    "EvaluateReferral should store excellence streak");

                // Verify trigger logic
                if (result.Probability == 0f)
                {
                    Assert.IsFalse(result.WasTriggered,
                        "Referral should not be triggered when probability is 0");
                }
            }
        }

        #endregion

        #region Property 21: Excellence Streak Tracking

        /// <summary>
        /// Property 21: Excellence Streak Tracking
        /// For any current streak E and satisfaction S:
        ///   S >= 90: E = E + 1 (increment)
        ///   S < 80: E = 0 (reset)
        ///   80 <= S < 90: E unchanged
        /// **Validates: Requirements R23**
        /// </summary>
        [Test]
        public void UpdateExcellenceStreak_HighSatisfaction_IncrementsStreak()
        {
            // Test 90%+ satisfaction increments streak (R23.6)
            for (int i = 0; i < 100; i++)
            {
                int currentStreak = _random.Next(0, 20);
                float satisfaction = 90f + (float)(_random.NextDouble() * 10); // 90-100

                var result = _referralSystem.UpdateExcellenceStreak(currentStreak, satisfaction);

                Assert.AreEqual(currentStreak + 1, result.NewStreak,
                    $"Satisfaction {satisfaction:F1}% should increment streak from {currentStreak} to {currentStreak + 1}");
                Assert.IsTrue(result.WasIncremented,
                    "WasIncremented should be true");
                Assert.IsFalse(result.WasReset,
                    "WasReset should be false");
                Assert.IsFalse(result.WasUnchanged,
                    "WasUnchanged should be false");
            }
        }

        /// <summary>
        /// Property 21: Below 80% satisfaction resets streak.
        /// **Validates: Requirements R23.9**
        /// </summary>
        [Test]
        public void UpdateExcellenceStreak_LowSatisfaction_ResetsStreak()
        {
            // Test <80% satisfaction resets streak (R23.9)
            for (int i = 0; i < 100; i++)
            {
                int currentStreak = _random.Next(1, 20); // Non-zero streak
                float satisfaction = (float)(_random.NextDouble() * 79.99f); // 0-79.99

                var result = _referralSystem.UpdateExcellenceStreak(currentStreak, satisfaction);

                Assert.AreEqual(0, result.NewStreak,
                    $"Satisfaction {satisfaction:F1}% should reset streak from {currentStreak} to 0");
                Assert.IsTrue(result.WasReset,
                    "WasReset should be true");
                Assert.IsFalse(result.WasIncremented,
                    "WasIncremented should be false");
                Assert.IsFalse(result.WasUnchanged,
                    "WasUnchanged should be false");
            }
        }

        /// <summary>
        /// Property 21: 80-89% satisfaction leaves streak unchanged.
        /// **Validates: Requirements R23**
        /// </summary>
        [Test]
        public void UpdateExcellenceStreak_MediumSatisfaction_StreakUnchanged()
        {
            // Test 80-89% satisfaction leaves streak unchanged
            for (int i = 0; i < 100; i++)
            {
                int currentStreak = _random.Next(0, 20);
                float satisfaction = 80f + (float)(_random.NextDouble() * 9.99f); // 80-89.99

                var result = _referralSystem.UpdateExcellenceStreak(currentStreak, satisfaction);

                Assert.AreEqual(currentStreak, result.NewStreak,
                    $"Satisfaction {satisfaction:F1}% should leave streak unchanged at {currentStreak}");
                Assert.IsTrue(result.WasUnchanged,
                    "WasUnchanged should be true");
                Assert.IsFalse(result.WasIncremented,
                    "WasIncremented should be false");
                Assert.IsFalse(result.WasReset,
                    "WasReset should be false");
            }
        }

        /// <summary>
        /// Property 21: Boundary test at satisfaction thresholds.
        /// **Validates: Requirements R23**
        /// </summary>
        [Test]
        public void UpdateExcellenceStreak_BoundaryValues()
        {
            int currentStreak = 5;

            // Test at exact boundaries
            var testCases = new[]
            {
                (satisfaction: 100f, expectedStreak: 6, wasIncremented: true, wasReset: false, wasUnchanged: false),
                (satisfaction: 90f, expectedStreak: 6, wasIncremented: true, wasReset: false, wasUnchanged: false),
                (satisfaction: 89.99f, expectedStreak: 5, wasIncremented: false, wasReset: false, wasUnchanged: true),
                (satisfaction: 80f, expectedStreak: 5, wasIncremented: false, wasReset: false, wasUnchanged: true),
                (satisfaction: 79.99f, expectedStreak: 0, wasIncremented: false, wasReset: true, wasUnchanged: false),
                (satisfaction: 0f, expectedStreak: 0, wasIncremented: false, wasReset: true, wasUnchanged: false)
            };

            foreach (var (satisfaction, expectedStreak, wasIncremented, wasReset, wasUnchanged) in testCases)
            {
                var result = _referralSystem.UpdateExcellenceStreak(currentStreak, satisfaction);

                Assert.AreEqual(expectedStreak, result.NewStreak,
                    $"Satisfaction {satisfaction}% should result in streak {expectedStreak}");
                Assert.AreEqual(wasIncremented, result.WasIncremented,
                    $"WasIncremented should be {wasIncremented} for satisfaction {satisfaction}%");
                Assert.AreEqual(wasReset, result.WasReset,
                    $"WasReset should be {wasReset} for satisfaction {satisfaction}%");
                Assert.AreEqual(wasUnchanged, result.WasUnchanged,
                    $"WasUnchanged should be {wasUnchanged} for satisfaction {satisfaction}%");
            }
        }

        /// <summary>
        /// Property 21: Previous streak is correctly stored.
        /// **Validates: Requirements R23**
        /// </summary>
        [Test]
        public void UpdateExcellenceStreak_StoresPreviousStreak()
        {
            for (int i = 0; i < 100; i++)
            {
                int currentStreak = _random.Next(0, 20);
                float satisfaction = (float)(_random.NextDouble() * 100);

                var result = _referralSystem.UpdateExcellenceStreak(currentStreak, satisfaction);

                Assert.AreEqual(currentStreak, result.PreviousStreak,
                    "PreviousStreak should match input streak");
                Assert.AreEqual(satisfaction, result.Satisfaction,
                    "Satisfaction should be stored in result");
            }
        }

        /// <summary>
        /// Property 21: Streak can grow indefinitely.
        /// **Validates: Requirements R23.6**
        /// </summary>
        [Test]
        public void UpdateExcellenceStreak_CanGrowIndefinitely()
        {
            int streak = 0;

            // Simulate 100 consecutive excellent events
            for (int i = 0; i < 100; i++)
            {
                float satisfaction = 95f + (float)(_random.NextDouble() * 5); // 95-100
                var result = _referralSystem.UpdateExcellenceStreak(streak, satisfaction);
                streak = result.NewStreak;
            }

            Assert.AreEqual(100, streak,
                "Streak should reach 100 after 100 consecutive excellent events");
        }

        /// <summary>
        /// Property 21: Streak resets from any value.
        /// **Validates: Requirements R23.9**
        /// </summary>
        [Test]
        public void UpdateExcellenceStreak_ResetsFromAnyValue()
        {
            // Test that streak resets from various high values
            foreach (int highStreak in new[] { 1, 5, 10, 50, 100, 1000 })
            {
                float lowSatisfaction = (float)(_random.NextDouble() * 79.99f); // 0-79.99

                var result = _referralSystem.UpdateExcellenceStreak(highStreak, lowSatisfaction);

                Assert.AreEqual(0, result.NewStreak,
                    $"Streak should reset from {highStreak} to 0 on low satisfaction");
            }
        }

        /// <summary>
        /// Property 21: Exactly one flag is set per update.
        /// **Validates: Requirements R23**
        /// </summary>
        [Test]
        public void UpdateExcellenceStreak_ExactlyOneFlagSet()
        {
            for (int i = 0; i < 100; i++)
            {
                int currentStreak = _random.Next(0, 20);
                float satisfaction = (float)(_random.NextDouble() * 100);

                var result = _referralSystem.UpdateExcellenceStreak(currentStreak, satisfaction);

                int flagCount = (result.WasIncremented ? 1 : 0) +
                               (result.WasReset ? 1 : 0) +
                               (result.WasUnchanged ? 1 : 0);

                Assert.AreEqual(1, flagCount,
                    $"Exactly one flag should be set (got {flagCount} for satisfaction {satisfaction:F1}%)");
            }
        }

        /// <summary>
        /// Property 21: GetStreakBonus returns correct values.
        /// **Validates: Requirements R23.6-R23.8**
        /// </summary>
        [Test]
        public void GetStreakBonus_ReturnsCorrectValues()
        {
            var testCases = new[]
            {
                (streak: 0, expected: 0f),
                (streak: 1, expected: 0f),
                (streak: 2, expected: 0f),
                (streak: 3, expected: 0.10f),
                (streak: 4, expected: 0.10f),
                (streak: 5, expected: 0.20f),
                (streak: 6, expected: 0.20f),
                (streak: 10, expected: 0.20f),
                (streak: 100, expected: 0.20f)
            };

            foreach (var (streak, expected) in testCases)
            {
                float bonus = _referralSystem.GetStreakBonus(streak);

                Assert.AreEqual(expected, bonus, 0.001f,
                    $"Streak {streak} should have bonus {expected * 100}%");
            }
        }

        #endregion

        #region Property 26: Excellence Streak Referral Bonus Application

        /// <summary>
        /// Property 26: Excellence Streak Referral Bonus Application
        /// For any satisfaction S and streak E:
        ///   baseChance = S >= 95 ? 0.80 : (S >= 90 ? 0.50 : 0.00)
        ///   streakBonus = E >= 5 ? 0.20 : (E >= 3 ? 0.10 : 0.00)
        ///   finalChance = min(baseChance + streakBonus, 1.0)
        /// 
        /// CRITICAL: Streak bonus only applies when S >= 90
        /// **Validates: Requirements R23.6-R23.8**
        /// </summary>
        [Test]
        public void StreakBonusApplication_ExcellentSatisfaction_NoStreak()
        {
            // S=95+, E=0: 80% + 0% = 80%
            for (int i = 0; i < 100; i++)
            {
                float satisfaction = 95f + (float)(_random.NextDouble() * 5); // 95-100
                int streak = 0;

                float probability = _referralSystem.CalculateReferralProbability(satisfaction, streak);

                Assert.AreEqual(0.80f, probability, 0.001f,
                    $"Excellent satisfaction ({satisfaction:F1}%) with no streak should be 80%");
            }
        }

        /// <summary>
        /// Property 26: Excellent satisfaction with streak 3 gives 90%.
        /// **Validates: Requirements R23.7**
        /// </summary>
        [Test]
        public void StreakBonusApplication_ExcellentSatisfaction_Streak3()
        {
            // S=95+, E=3: 80% + 10% = 90%
            for (int i = 0; i < 100; i++)
            {
                float satisfaction = 95f + (float)(_random.NextDouble() * 5); // 95-100
                int streak = 3;

                float probability = _referralSystem.CalculateReferralProbability(satisfaction, streak);

                Assert.AreEqual(0.90f, probability, 0.001f,
                    $"Excellent satisfaction ({satisfaction:F1}%) with streak 3 should be 90%");
            }
        }

        /// <summary>
        /// Property 26: Excellent satisfaction with streak 5 gives 100% (capped).
        /// **Validates: Requirements R23.8**
        /// </summary>
        [Test]
        public void StreakBonusApplication_ExcellentSatisfaction_Streak5()
        {
            // S=95+, E=5: 80% + 20% = 100% (capped)
            for (int i = 0; i < 100; i++)
            {
                float satisfaction = 95f + (float)(_random.NextDouble() * 5); // 95-100
                int streak = 5 + _random.Next(0, 10); // 5+

                float probability = _referralSystem.CalculateReferralProbability(satisfaction, streak);

                Assert.AreEqual(1.0f, probability, 0.001f,
                    $"Excellent satisfaction ({satisfaction:F1}%) with streak {streak} should be 100% (capped)");
            }
        }

        /// <summary>
        /// Property 26: High satisfaction with streak 5 gives 70%.
        /// **Validates: Requirements R23.8**
        /// </summary>
        [Test]
        public void StreakBonusApplication_HighSatisfaction_Streak5()
        {
            // S=92, E=5: 50% + 20% = 70%
            for (int i = 0; i < 100; i++)
            {
                float satisfaction = 90f + (float)(_random.NextDouble() * 4.99f); // 90-94.99
                int streak = 5 + _random.Next(0, 10); // 5+

                float probability = _referralSystem.CalculateReferralProbability(satisfaction, streak);

                Assert.AreEqual(0.70f, probability, 0.001f,
                    $"High satisfaction ({satisfaction:F1}%) with streak {streak} should be 70%");
            }
        }

        /// <summary>
        /// Property 26: Below threshold satisfaction with high streak still gives 0%.
        /// **Validates: Requirements R23 (correction in design)**
        /// </summary>
        [Test]
        public void StreakBonusApplication_BelowThreshold_StreakIgnored()
        {
            // S=85, E=5: 0% (no referral possible below 90%)
            for (int i = 0; i < 100; i++)
            {
                float satisfaction = (float)(_random.NextDouble() * 89.99f); // 0-89.99
                int streak = 5 + _random.Next(0, 100); // Very high streak

                float probability = _referralSystem.CalculateReferralProbability(satisfaction, streak);

                Assert.AreEqual(0f, probability,
                    $"Below threshold satisfaction ({satisfaction:F1}%) should be 0% regardless of streak {streak}");
            }
        }

        /// <summary>
        /// Property 26: Comprehensive test of all satisfaction/streak combinations.
        /// **Validates: Requirements R23.6-R23.8**
        /// </summary>
        [Test]
        public void StreakBonusApplication_AllCombinations()
        {
            var testCases = new[]
            {
                // (satisfaction, streak, expectedProbability)
                // Excellent satisfaction (95%+)
                (satisfaction: 95f, streak: 0, expected: 0.80f),
                (satisfaction: 95f, streak: 1, expected: 0.80f),
                (satisfaction: 95f, streak: 2, expected: 0.80f),
                (satisfaction: 95f, streak: 3, expected: 0.90f),
                (satisfaction: 95f, streak: 4, expected: 0.90f),
                (satisfaction: 95f, streak: 5, expected: 1.00f),
                (satisfaction: 95f, streak: 10, expected: 1.00f),
                (satisfaction: 100f, streak: 5, expected: 1.00f),

                // High satisfaction (90-94%)
                (satisfaction: 90f, streak: 0, expected: 0.50f),
                (satisfaction: 90f, streak: 1, expected: 0.50f),
                (satisfaction: 90f, streak: 2, expected: 0.50f),
                (satisfaction: 90f, streak: 3, expected: 0.60f),
                (satisfaction: 90f, streak: 4, expected: 0.60f),
                (satisfaction: 90f, streak: 5, expected: 0.70f),
                (satisfaction: 90f, streak: 10, expected: 0.70f),
                (satisfaction: 94f, streak: 5, expected: 0.70f),

                // Below threshold (<90%)
                (satisfaction: 89f, streak: 0, expected: 0f),
                (satisfaction: 89f, streak: 5, expected: 0f),
                (satisfaction: 89f, streak: 10, expected: 0f),
                (satisfaction: 85f, streak: 5, expected: 0f),
                (satisfaction: 50f, streak: 5, expected: 0f),
                (satisfaction: 0f, streak: 5, expected: 0f)
            };

            foreach (var (satisfaction, streak, expected) in testCases)
            {
                float probability = _referralSystem.CalculateReferralProbability(satisfaction, streak);

                Assert.AreEqual(expected, probability, 0.001f,
                    $"Satisfaction {satisfaction}% with streak {streak} should have {expected * 100}% probability");
            }
        }

        /// <summary>
        /// Property 26: Streak bonus is additive with base chance.
        /// **Validates: Requirements R23.6-R23.8**
        /// </summary>
        [Test]
        public void StreakBonusApplication_IsAdditive()
        {
            // Verify that streak bonus is additive
            foreach (float satisfaction in new[] { 90f, 95f, 100f })
            {
                float baseChance = _referralSystem.GetBaseReferralChance(satisfaction);

                foreach (int streak in new[] { 0, 3, 5 })
                {
                    float streakBonus = _referralSystem.GetStreakBonus(streak);
                    float expectedProbability = Math.Min(1.0f, baseChance + streakBonus);
                    float actualProbability = _referralSystem.CalculateReferralProbability(satisfaction, streak);

                    Assert.AreEqual(expectedProbability, actualProbability, 0.001f,
                        $"Probability should be base ({baseChance * 100}%) + streak bonus ({streakBonus * 100}%) = {expectedProbability * 100}%");
                }
            }
        }

        /// <summary>
        /// Property 26: Probability is always between 0 and 1.
        /// **Validates: Requirements R23**
        /// </summary>
        [Test]
        public void StreakBonusApplication_ProbabilityInValidRange()
        {
            for (int i = 0; i < 100; i++)
            {
                float satisfaction = (float)(_random.NextDouble() * 100);
                int streak = _random.Next(0, 100);

                float probability = _referralSystem.CalculateReferralProbability(satisfaction, streak);

                Assert.GreaterOrEqual(probability, 0f,
                    $"Probability should be >= 0 (got {probability})");
                Assert.LessOrEqual(probability, 1.0f,
                    $"Probability should be <= 1 (got {probability})");
            }
        }

        #endregion
    }
}
