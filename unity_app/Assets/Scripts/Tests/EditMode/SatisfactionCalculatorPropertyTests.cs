using System;
using NUnit.Framework;
using EventPlannerSim.Core;
using EventPlannerSim.Data;
using EventPlannerSim.Systems;

namespace EventPlannerSim.Tests.EditMode
{
    /// <summary>
    /// Property-based tests for SatisfactionCalculator.
    /// Feature: event-planner-simulator
    /// </summary>
    [TestFixture]
    public class SatisfactionCalculatorPropertyTests
    {
        private SatisfactionCalculatorImpl _calculator;
        private Random _random;

        // Category weights as per R13.2
        private const float VenueWeight = 0.20f;
        private const float FoodWeight = 0.25f;
        private const float EntertainmentWeight = 0.20f;
        private const float DecorationWeight = 0.15f;
        private const float ServiceWeight = 0.10f;
        private const float ExpectationWeight = 0.10f;

        [SetUp]
        public void Setup()
        {
            _calculator = new SatisfactionCalculatorImpl();
            _random = new Random(42); // Fixed seed for reproducibility
        }

        #region Property 13: Satisfaction Weighted Calculation

        /// <summary>
        /// Property 13: Satisfaction Weighted Calculation
        /// For any category scores V, F, E, D, S, X:
        /// baseSatisfaction = V*0.20 + F*0.25 + E*0.20 + D*0.15 + S*0.10 + X*0.10
        /// **Validates: Requirements R13**
        /// </summary>
        [Test]
        public void Calculate_WeightedSatisfaction_MatchesFormula()
        {
            for (int i = 0; i < 100; i++)
            {
                // Generate random category scores (0-100)
                float venueScore = (float)(_random.NextDouble() * 100);
                float foodScore = (float)(_random.NextDouble() * 100);
                float entertainmentScore = (float)(_random.NextDouble() * 100);
                float decorationScore = (float)(_random.NextDouble() * 100);
                float serviceScore = (float)(_random.NextDouble() * 100);
                float expectationScore = (float)(_random.NextDouble() * 100);

                var eventData = CreateEventWithScores(
                    venueScore, foodScore, entertainmentScore,
                    decorationScore, serviceScore, expectationScore);
                var client = CreateClient(ClientPersonality.EasyGoing);

                var result = _calculator.Calculate(eventData, client);

                // Calculate expected weighted sum
                float expected = venueScore * VenueWeight +
                                 foodScore * FoodWeight +
                                 entertainmentScore * EntertainmentWeight +
                                 decorationScore * DecorationWeight +
                                 serviceScore * ServiceWeight +
                                 expectationScore * ExpectationWeight;

                Assert.AreEqual(expected, result.BaseSatisfaction, 0.01f,
                    $"Weighted calculation failed. Scores: V={venueScore:F2}, F={foodScore:F2}, " +
                    $"E={entertainmentScore:F2}, D={decorationScore:F2}, S={serviceScore:F2}, X={expectationScore:F2}");
            }
        }

        /// <summary>
        /// Property 13: All perfect scores should result in 100 satisfaction.
        /// **Validates: Requirements R13**
        /// </summary>
        [Test]
        public void Calculate_AllPerfectScores_Returns100()
        {
            var eventData = CreateEventWithScores(100, 100, 100, 100, 100, 100);
            var client = CreateClient(ClientPersonality.EasyGoing);

            var result = _calculator.Calculate(eventData, client);

            Assert.AreEqual(100f, result.FinalSatisfaction, 0.01f);
        }

        /// <summary>
        /// Property 13: All zero scores should result in 0 satisfaction.
        /// **Validates: Requirements R13**
        /// </summary>
        [Test]
        public void Calculate_AllZeroScores_Returns0()
        {
            var eventData = CreateEventWithScores(0, 0, 0, 0, 0, 0);
            var client = CreateClient(ClientPersonality.EasyGoing);

            var result = _calculator.Calculate(eventData, client);

            Assert.AreEqual(0f, result.FinalSatisfaction, 0.01f);
        }

        /// <summary>
        /// Property 13: Random event modifier is applied correctly.
        /// finalSatisfaction = baseSatisfaction * randomEventModifier
        /// **Validates: Requirements R13.3**
        /// </summary>
        [Test]
        public void Calculate_RandomEventModifier_AppliedCorrectly()
        {
            for (int i = 0; i < 100; i++)
            {
                float baseScore = (float)(_random.NextDouble() * 100);
                float modifier = (float)(0.5 + _random.NextDouble()); // 0.5 to 1.5

                var eventData = CreateEventWithScores(baseScore, baseScore, baseScore, baseScore, baseScore, baseScore);
                eventData.results.randomEventModifier = modifier;
                var client = CreateClient(ClientPersonality.EasyGoing);

                var result = _calculator.Calculate(eventData, client);

                float expectedBase = baseScore; // All scores equal, so weighted sum = baseScore
                float expectedFinal = SatisfactionCalculatorImpl.ClampSatisfaction(expectedBase * modifier);

                Assert.AreEqual(expectedFinal, result.FinalSatisfaction, 0.01f,
                    $"Random event modifier not applied correctly. Base: {expectedBase}, Modifier: {modifier}");
            }
        }

        /// <summary>
        /// Property 13: Weights sum to 1.0 (100%).
        /// **Validates: Requirements R13.2**
        /// </summary>
        [Test]
        public void Weights_SumToOne()
        {
            float totalWeight = VenueWeight + FoodWeight + EntertainmentWeight +
                               DecorationWeight + ServiceWeight + ExpectationWeight;

            Assert.AreEqual(1.0f, totalWeight, 0.001f, "Category weights must sum to 1.0");
        }

        #endregion

        #region Property 14: Satisfaction Clamping

        /// <summary>
        /// Property 14: Satisfaction Clamping
        /// For any calculated satisfaction score S: result = clamp(S, 0, 100)
        /// INVARIANT: 0 <= finalSatisfaction <= 100
        /// **Validates: Requirements R13.8**
        /// </summary>
        [Test]
        public void Calculate_FinalSatisfaction_AlwaysClamped_0To100()
        {
            for (int i = 0; i < 100; i++)
            {
                // Generate random scores that could produce extreme values
                float venueScore = (float)(_random.NextDouble() * 200 - 50); // -50 to 150
                float foodScore = (float)(_random.NextDouble() * 200 - 50);
                float entertainmentScore = (float)(_random.NextDouble() * 200 - 50);
                float decorationScore = (float)(_random.NextDouble() * 200 - 50);
                float serviceScore = (float)(_random.NextDouble() * 200 - 50);
                float expectationScore = (float)(_random.NextDouble() * 200 - 50);

                // Random modifier that could push values extreme
                float modifier = (float)(_random.NextDouble() * 3); // 0 to 3

                var eventData = CreateEventWithScores(
                    venueScore, foodScore, entertainmentScore,
                    decorationScore, serviceScore, expectationScore);
                eventData.results.randomEventModifier = modifier;
                var client = CreateClient(ClientPersonality.EasyGoing);

                var result = _calculator.Calculate(eventData, client);

                Assert.GreaterOrEqual(result.FinalSatisfaction, 0f,
                    $"FinalSatisfaction should be >= 0. Got: {result.FinalSatisfaction}");
                Assert.LessOrEqual(result.FinalSatisfaction, 100f,
                    $"FinalSatisfaction should be <= 100. Got: {result.FinalSatisfaction}");
            }
        }

        /// <summary>
        /// Property 14: Negative modifiers should clamp to 0.
        /// **Validates: Requirements R13.8**
        /// </summary>
        [Test]
        public void Calculate_NegativeModifier_ClampsToZero()
        {
            var eventData = CreateEventWithScores(100, 100, 100, 100, 100, 100);
            eventData.results.randomEventModifier = -1f; // Negative modifier
            var client = CreateClient(ClientPersonality.EasyGoing);

            var result = _calculator.Calculate(eventData, client);

            Assert.AreEqual(0f, result.FinalSatisfaction,
                "Negative satisfaction should be clamped to 0");
        }

        /// <summary>
        /// Property 14: Stacked bonuses should clamp to 100.
        /// **Validates: Requirements R13.8**
        /// </summary>
        [Test]
        public void Calculate_StackedBonuses_ClampsTo100()
        {
            var eventData = CreateEventWithScores(100, 100, 100, 100, 100, 100);
            eventData.results.randomEventModifier = 2f; // Double the score
            var client = CreateClient(ClientPersonality.EasyGoing);

            var result = _calculator.Calculate(eventData, client);

            Assert.AreEqual(100f, result.FinalSatisfaction,
                "Satisfaction above 100 should be clamped to 100");
        }

        /// <summary>
        /// Property 14: ClampSatisfaction static method works correctly.
        /// **Validates: Requirements R13.8**
        /// </summary>
        [Test]
        public void ClampSatisfaction_HandlesExtremeValues()
        {
            for (int i = 0; i < 100; i++)
            {
                // Generate extreme values
                float input = (float)(_random.NextDouble() * 1000 - 500); // -500 to 500

                float result = SatisfactionCalculatorImpl.ClampSatisfaction(input);

                Assert.GreaterOrEqual(result, 0f, $"Clamped value should be >= 0. Input: {input}");
                Assert.LessOrEqual(result, 100f, $"Clamped value should be <= 100. Input: {input}");

                // Verify correct clamping behavior
                if (input < 0)
                    Assert.AreEqual(0f, result, $"Negative input should clamp to 0. Input: {input}");
                else if (input > 100)
                    Assert.AreEqual(100f, result, $"Input > 100 should clamp to 100. Input: {input}");
                else
                    Assert.AreEqual(input, result, 0.001f, $"Input in range should be unchanged. Input: {input}");
            }
        }

        /// <summary>
        /// Property 14: Boundary values are handled correctly.
        /// **Validates: Requirements R13.8**
        /// </summary>
        [Test]
        public void ClampSatisfaction_BoundaryValues()
        {
            // Test exact boundaries
            Assert.AreEqual(0f, SatisfactionCalculatorImpl.ClampSatisfaction(0f));
            Assert.AreEqual(100f, SatisfactionCalculatorImpl.ClampSatisfaction(100f));
            Assert.AreEqual(0f, SatisfactionCalculatorImpl.ClampSatisfaction(-0.001f));
            Assert.AreEqual(100f, SatisfactionCalculatorImpl.ClampSatisfaction(100.001f));
            Assert.AreEqual(50f, SatisfactionCalculatorImpl.ClampSatisfaction(50f));
        }

        #endregion

        #region Property 16: Personality Thresholds

        /// <summary>
        /// Property 16: Personality Thresholds
        /// Each personality type has a specific satisfaction threshold.
        /// **Validates: Requirements R15.2-R15.6**
        /// </summary>
        [Test]
        public void GetPersonalityThreshold_ReturnsCorrectThresholds()
        {
            // R15.2: EasyGoing = 50
            Assert.AreEqual(50f, _calculator.GetPersonalityThreshold(ClientPersonality.EasyGoing),
                "EasyGoing threshold should be 50");

            // R15.3: BudgetConscious = 60
            Assert.AreEqual(60f, _calculator.GetPersonalityThreshold(ClientPersonality.BudgetConscious),
                "BudgetConscious threshold should be 60");

            // R15.4: Perfectionist = 85
            Assert.AreEqual(85f, _calculator.GetPersonalityThreshold(ClientPersonality.Perfectionist),
                "Perfectionist threshold should be 85");

            // R15.7: Indecisive = 65 (Stage 3+)
            Assert.AreEqual(65f, _calculator.GetPersonalityThreshold(ClientPersonality.Indecisive),
                "Indecisive threshold should be 65");

            // R15.8: Demanding = 80 (Stage 4+)
            Assert.AreEqual(80f, _calculator.GetPersonalityThreshold(ClientPersonality.Demanding),
                "Demanding threshold should be 80");
        }

        /// <summary>
        /// Property 16: All personality types have valid thresholds.
        /// **Validates: Requirements R15**
        /// </summary>
        [Test]
        public void GetPersonalityThreshold_AllPersonalities_HaveValidThresholds()
        {
            foreach (ClientPersonality personality in Enum.GetValues(typeof(ClientPersonality)))
            {
                float threshold = _calculator.GetPersonalityThreshold(personality);

                Assert.GreaterOrEqual(threshold, 0f,
                    $"Threshold for {personality} should be >= 0");
                Assert.LessOrEqual(threshold, 100f,
                    $"Threshold for {personality} should be <= 100");
            }
        }

        /// <summary>
        /// Property 16: Perfectionist has highest threshold among base personalities.
        /// **Validates: Requirements R15**
        /// </summary>
        [Test]
        public void GetPersonalityThreshold_Perfectionist_HasHighestBaseThreshold()
        {
            float easyGoing = _calculator.GetPersonalityThreshold(ClientPersonality.EasyGoing);
            float budgetConscious = _calculator.GetPersonalityThreshold(ClientPersonality.BudgetConscious);
            float perfectionist = _calculator.GetPersonalityThreshold(ClientPersonality.Perfectionist);

            Assert.Greater(perfectionist, easyGoing,
                "Perfectionist threshold should be higher than EasyGoing");
            Assert.Greater(perfectionist, budgetConscious,
                "Perfectionist threshold should be higher than BudgetConscious");
        }

        /// <summary>
        /// Property 16: EasyGoing has lowest threshold.
        /// **Validates: Requirements R15**
        /// </summary>
        [Test]
        public void GetPersonalityThreshold_EasyGoing_HasLowestThreshold()
        {
            float easyGoing = _calculator.GetPersonalityThreshold(ClientPersonality.EasyGoing);
            float budgetConscious = _calculator.GetPersonalityThreshold(ClientPersonality.BudgetConscious);
            float perfectionist = _calculator.GetPersonalityThreshold(ClientPersonality.Perfectionist);
            float indecisive = _calculator.GetPersonalityThreshold(ClientPersonality.Indecisive);
            float demanding = _calculator.GetPersonalityThreshold(ClientPersonality.Demanding);

            Assert.LessOrEqual(easyGoing, budgetConscious,
                "EasyGoing threshold should be <= BudgetConscious");
            Assert.LessOrEqual(easyGoing, perfectionist,
                "EasyGoing threshold should be <= Perfectionist");
            Assert.LessOrEqual(easyGoing, indecisive,
                "EasyGoing threshold should be <= Indecisive");
            Assert.LessOrEqual(easyGoing, demanding,
                "EasyGoing threshold should be <= Demanding");
        }

        /// <summary>
        /// Property 16: MeetsClientThreshold is calculated correctly.
        /// **Validates: Requirements R15**
        /// </summary>
        [Test]
        public void Calculate_MeetsClientThreshold_CalculatedCorrectly()
        {
            foreach (ClientPersonality personality in Enum.GetValues(typeof(ClientPersonality)))
            {
                float threshold = _calculator.GetPersonalityThreshold(personality);

                // Test at threshold - should meet
                var eventAtThreshold = CreateEventWithScores(threshold, threshold, threshold, threshold, threshold, threshold);
                var clientAtThreshold = CreateClient(personality);
                var resultAtThreshold = _calculator.Calculate(eventAtThreshold, clientAtThreshold);

                Assert.IsTrue(resultAtThreshold.MeetsClientThreshold,
                    $"Score at threshold ({threshold}) should meet {personality} threshold");

                // Test below threshold - should not meet
                if (threshold > 1)
                {
                    float belowThreshold = threshold - 1;
                    var eventBelow = CreateEventWithScores(belowThreshold, belowThreshold, belowThreshold, belowThreshold, belowThreshold, belowThreshold);
                    var clientBelow = CreateClient(personality);
                    var resultBelow = _calculator.Calculate(eventBelow, clientBelow);

                    Assert.IsFalse(resultBelow.MeetsClientThreshold,
                        $"Score below threshold ({belowThreshold}) should not meet {personality} threshold ({threshold})");
                }
            }
        }

        #endregion

        #region Property 8: Overage Tolerance by Personality

        /// <summary>
        /// Property 8: Overage Tolerance by Personality
        /// Each personality type has a specific overage tolerance percentage.
        /// **Validates: Requirements R7.9-R7.14**
        /// </summary>
        [Test]
        public void GetOverageTolerance_ReturnsCorrectTolerances()
        {
            // R7.9: EasyGoing = 15%
            Assert.AreEqual(15f, _calculator.GetOverageTolerance(ClientPersonality.EasyGoing),
                "EasyGoing overage tolerance should be 15%");

            // R7.9: BudgetConscious = 0%
            Assert.AreEqual(0f, _calculator.GetOverageTolerance(ClientPersonality.BudgetConscious),
                "BudgetConscious overage tolerance should be 0%");

            // R7.9: Perfectionist = 5%
            Assert.AreEqual(5f, _calculator.GetOverageTolerance(ClientPersonality.Perfectionist),
                "Perfectionist overage tolerance should be 5%");

            // R7.9: Indecisive = 10%
            Assert.AreEqual(10f, _calculator.GetOverageTolerance(ClientPersonality.Indecisive),
                "Indecisive overage tolerance should be 10%");

            // R7.9: Demanding = 5%
            Assert.AreEqual(5f, _calculator.GetOverageTolerance(ClientPersonality.Demanding),
                "Demanding overage tolerance should be 5%");
        }

        /// <summary>
        /// Property 8: Overage within tolerance is accepted.
        /// **Validates: Requirements R7.10**
        /// </summary>
        [Test]
        public void IsOverageWithinTolerance_WithinTolerance_ReturnsTrue()
        {
            foreach (ClientPersonality personality in Enum.GetValues(typeof(ClientPersonality)))
            {
                float tolerance = _calculator.GetOverageTolerance(personality);

                // Test at exactly tolerance
                Assert.IsTrue(_calculator.IsOverageWithinTolerance(tolerance, personality),
                    $"Overage at tolerance ({tolerance}%) should be within tolerance for {personality}");

                // Test below tolerance
                if (tolerance > 0)
                {
                    Assert.IsTrue(_calculator.IsOverageWithinTolerance(tolerance - 0.1f, personality),
                        $"Overage below tolerance should be within tolerance for {personality}");
                }

                // Test at zero
                Assert.IsTrue(_calculator.IsOverageWithinTolerance(0f, personality),
                    $"Zero overage should always be within tolerance for {personality}");
            }
        }

        /// <summary>
        /// Property 8: Overage exceeding tolerance is rejected.
        /// **Validates: Requirements R7.11**
        /// </summary>
        [Test]
        public void IsOverageWithinTolerance_ExceedsTolerance_ReturnsFalse()
        {
            foreach (ClientPersonality personality in Enum.GetValues(typeof(ClientPersonality)))
            {
                float tolerance = _calculator.GetOverageTolerance(personality);

                // Test above tolerance
                Assert.IsFalse(_calculator.IsOverageWithinTolerance(tolerance + 0.1f, personality),
                    $"Overage above tolerance ({tolerance + 0.1f}%) should exceed tolerance for {personality}");

                // Test significantly above tolerance
                Assert.IsFalse(_calculator.IsOverageWithinTolerance(tolerance + 10f, personality),
                    $"Overage significantly above tolerance should exceed tolerance for {personality}");
            }
        }

        /// <summary>
        /// Property 8: BudgetConscious has zero tolerance.
        /// **Validates: Requirements R7.9**
        /// </summary>
        [Test]
        public void IsOverageWithinTolerance_BudgetConscious_ZeroTolerance()
        {
            // Any overage should exceed tolerance
            Assert.IsFalse(_calculator.IsOverageWithinTolerance(0.01f, ClientPersonality.BudgetConscious),
                "BudgetConscious should not tolerate any overage");
            Assert.IsFalse(_calculator.IsOverageWithinTolerance(1f, ClientPersonality.BudgetConscious),
                "BudgetConscious should not tolerate 1% overage");

            // Zero overage should be fine
            Assert.IsTrue(_calculator.IsOverageWithinTolerance(0f, ClientPersonality.BudgetConscious),
                "BudgetConscious should accept zero overage");
        }

        /// <summary>
        /// Property 8: EasyGoing has highest tolerance.
        /// **Validates: Requirements R7.9**
        /// </summary>
        [Test]
        public void GetOverageTolerance_EasyGoing_HasHighestTolerance()
        {
            float easyGoing = _calculator.GetOverageTolerance(ClientPersonality.EasyGoing);
            float budgetConscious = _calculator.GetOverageTolerance(ClientPersonality.BudgetConscious);
            float perfectionist = _calculator.GetOverageTolerance(ClientPersonality.Perfectionist);
            float indecisive = _calculator.GetOverageTolerance(ClientPersonality.Indecisive);
            float demanding = _calculator.GetOverageTolerance(ClientPersonality.Demanding);

            Assert.GreaterOrEqual(easyGoing, budgetConscious,
                "EasyGoing tolerance should be >= BudgetConscious");
            Assert.GreaterOrEqual(easyGoing, perfectionist,
                "EasyGoing tolerance should be >= Perfectionist");
            Assert.GreaterOrEqual(easyGoing, indecisive,
                "EasyGoing tolerance should be >= Indecisive");
            Assert.GreaterOrEqual(easyGoing, demanding,
                "EasyGoing tolerance should be >= Demanding");
        }

        /// <summary>
        /// Property 8: Boundary testing for overage tolerance.
        /// Tests at tolerance - 1%, tolerance, tolerance + 1%
        /// **Validates: Requirements R7.9-R7.14**
        /// </summary>
        [Test]
        public void IsOverageWithinTolerance_BoundaryTests()
        {
            var testCases = new[]
            {
                (ClientPersonality.EasyGoing, 15f),
                (ClientPersonality.BudgetConscious, 0f),
                (ClientPersonality.Perfectionist, 5f),
                (ClientPersonality.Indecisive, 10f),
                (ClientPersonality.Demanding, 5f)
            };

            foreach (var (personality, tolerance) in testCases)
            {
                // At tolerance - should be within
                Assert.IsTrue(_calculator.IsOverageWithinTolerance(tolerance, personality),
                    $"{personality}: At tolerance ({tolerance}%) should be within tolerance");

                // Just above tolerance - should exceed
                Assert.IsFalse(_calculator.IsOverageWithinTolerance(tolerance + 0.01f, personality),
                    $"{personality}: Just above tolerance ({tolerance + 0.01f}%) should exceed tolerance");

                // Below tolerance - should be within
                if (tolerance > 0)
                {
                    Assert.IsTrue(_calculator.IsOverageWithinTolerance(tolerance - 1f, personality),
                        $"{personality}: Below tolerance ({tolerance - 1f}%) should be within tolerance");
                }
            }
        }

        /// <summary>
        /// Property 8: All personality types have non-negative tolerance.
        /// **Validates: Requirements R7.9**
        /// </summary>
        [Test]
        public void GetOverageTolerance_AllPersonalities_NonNegative()
        {
            foreach (ClientPersonality personality in Enum.GetValues(typeof(ClientPersonality)))
            {
                float tolerance = _calculator.GetOverageTolerance(personality);

                Assert.GreaterOrEqual(tolerance, 0f,
                    $"Overage tolerance for {personality} should be non-negative");
            }
        }

        #endregion

        #region Helper Methods

        private EventData CreateEventWithScores(
            float venue, float food, float entertainment,
            float decoration, float service, float expectation)
        {
            return new EventData
            {
                id = Guid.NewGuid().ToString(),
                clientId = "test-client",
                clientName = "Test Client",
                eventTitle = "Test Event",
                results = new EventResults
                {
                    venueScore = venue,
                    foodScore = food,
                    entertainmentScore = entertainment,
                    decorationScore = decoration,
                    serviceScore = service,
                    expectationScore = expectation,
                    randomEventModifier = 1f
                }
            };
        }

        private ClientData CreateClient(ClientPersonality personality = ClientPersonality.EasyGoing)
        {
            return new ClientData
            {
                clientId = "test-client",
                clientName = "Test Client",
                personality = personality,
                guestCount = 50,
                budgetTotal = 5000f
            };
        }

        #endregion
    }
}
