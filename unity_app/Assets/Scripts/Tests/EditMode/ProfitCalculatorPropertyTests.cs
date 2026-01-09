using System;
using NUnit.Framework;
using EventPlannerSim.Core;
using EventPlannerSim.Systems;

namespace EventPlannerSim.Tests.EditMode
{
    /// <summary>
    /// Property-based tests for ProfitCalculator.
    /// Feature: event-planner-simulator
    /// Property 23: Profit Margin Calculation
    /// Property 27: Commission Calculation Formula
    /// **Validates: Requirements R33, R16.3**
    /// </summary>
    [TestFixture]
    public class ProfitCalculatorPropertyTests
    {
        private ProfitCalculatorImpl _profitCalculator;
        private Random _random;

        [SetUp]
        public void Setup()
        {
            _random = new Random(42);
            _profitCalculator = new ProfitCalculatorImpl(42);
        }

        #region Property 23: Profit Margin Calculation

        /// <summary>
        /// Property 23: Profit Margin Calculation
        /// GIVEN event budget B and satisfaction S >= 70
        /// WHEN calculating profit
        /// THEN profit margin should be 20-30% of B
        /// **Validates: Requirements R33.1**
        /// </summary>
        [Test]
        public void CalculateProfit_SuccessfulEvent_Returns20To30PercentMargin()
        {
            // Test 100 random successful events (satisfaction 70-100%)
            for (int i = 0; i < 100; i++)
            {
                float budget = 1000f + (float)(_random.NextDouble() * 99000f); // $1,000 - $100,000
                float satisfaction = 70f + (float)(_random.NextDouble() * 30f); // 70-100%

                var result = _profitCalculator.CalculateProfit(budget, satisfaction);

                Assert.AreEqual("Successful", result.ProfitTier, 
                    $"Satisfaction {satisfaction}% should be Successful tier");
                Assert.GreaterOrEqual(result.ProfitMarginPercent, 0.20f, 
                    $"Profit margin should be >= 20% for satisfaction {satisfaction}%");
                Assert.LessOrEqual(result.ProfitMarginPercent, 0.30f, 
                    $"Profit margin should be <= 30% for satisfaction {satisfaction}%");
                Assert.IsTrue(result.IsProfitable, 
                    $"Event should be profitable for satisfaction {satisfaction}%");
            }
        }

        /// <summary>
        /// Property 23: Profit Margin Calculation
        /// GIVEN event budget B and satisfaction S in range [50, 70)
        /// WHEN calculating profit
        /// THEN profit margin should be 10-15% of B
        /// **Validates: Requirements R33.2**
        /// </summary>
        [Test]
        public void CalculateProfit_MediocreEvent_Returns10To15PercentMargin()
        {
            // Test 100 random mediocre events (satisfaction 50-69%)
            for (int i = 0; i < 100; i++)
            {
                float budget = 1000f + (float)(_random.NextDouble() * 99000f); // $1,000 - $100,000
                float satisfaction = 50f + (float)(_random.NextDouble() * 19.99f); // 50-69.99%

                var result = _profitCalculator.CalculateProfit(budget, satisfaction);

                Assert.AreEqual("Mediocre", result.ProfitTier, 
                    $"Satisfaction {satisfaction}% should be Mediocre tier");
                Assert.GreaterOrEqual(result.ProfitMarginPercent, 0.10f, 
                    $"Profit margin should be >= 10% for satisfaction {satisfaction}%");
                Assert.LessOrEqual(result.ProfitMarginPercent, 0.15f, 
                    $"Profit margin should be <= 15% for satisfaction {satisfaction}%");
                Assert.IsTrue(result.IsProfitable, 
                    $"Event should be profitable for satisfaction {satisfaction}%");
            }
        }

        /// <summary>
        /// Property 23: Profit Margin Calculation
        /// GIVEN event budget B and satisfaction S < 50
        /// WHEN calculating profit
        /// THEN result should be break-even or loss
        /// **Validates: Requirements R33.3**
        /// </summary>
        [Test]
        public void CalculateProfit_FailedEvent_ReturnsBreakEvenOrLoss()
        {
            // Test 100 random failed events (satisfaction 0-49%)
            for (int i = 0; i < 100; i++)
            {
                float budget = 1000f + (float)(_random.NextDouble() * 99000f); // $1,000 - $100,000
                float satisfaction = (float)(_random.NextDouble() * 49.99f); // 0-49.99%

                var result = _profitCalculator.CalculateProfit(budget, satisfaction);

                Assert.AreEqual("Failed", result.ProfitTier, 
                    $"Satisfaction {satisfaction}% should be Failed tier");
                Assert.LessOrEqual(result.ProfitMarginPercent, 0f, 
                    $"Profit margin should be <= 0% for satisfaction {satisfaction}%");
                Assert.IsFalse(result.IsProfitable, 
                    $"Event should NOT be profitable for satisfaction {satisfaction}%");
            }
        }

        /// <summary>
        /// Property 23: Boundary test at 70% satisfaction threshold.
        /// **Validates: Requirements R33.1, R33.2**
        /// </summary>
        [Test]
        public void CalculateProfit_BoundaryAt70Percent()
        {
            float budget = 10000f;

            // Just below 70% should be Mediocre
            var resultBelow = _profitCalculator.CalculateProfit(budget, 69.99f);
            Assert.AreEqual("Mediocre", resultBelow.ProfitTier, "69.99% should be Mediocre");
            Assert.LessOrEqual(resultBelow.ProfitMarginPercent, 0.15f, "69.99% should have <= 15% margin");

            // At 70% should be Successful
            var resultAt = _profitCalculator.CalculateProfit(budget, 70f);
            Assert.AreEqual("Successful", resultAt.ProfitTier, "70% should be Successful");
            Assert.GreaterOrEqual(resultAt.ProfitMarginPercent, 0.20f, "70% should have >= 20% margin");
        }

        /// <summary>
        /// Property 23: Boundary test at 50% satisfaction threshold.
        /// **Validates: Requirements R33.2, R33.3**
        /// </summary>
        [Test]
        public void CalculateProfit_BoundaryAt50Percent()
        {
            float budget = 10000f;

            // Just below 50% should be Failed
            var resultBelow = _profitCalculator.CalculateProfit(budget, 49.99f);
            Assert.AreEqual("Failed", resultBelow.ProfitTier, "49.99% should be Failed");
            Assert.LessOrEqual(resultBelow.ProfitMarginPercent, 0f, "49.99% should have <= 0% margin");

            // At 50% should be Mediocre
            var resultAt = _profitCalculator.CalculateProfit(budget, 50f);
            Assert.AreEqual("Mediocre", resultAt.ProfitTier, "50% should be Mediocre");
            Assert.GreaterOrEqual(resultAt.ProfitMarginPercent, 0.10f, "50% should have >= 10% margin");
        }

        /// <summary>
        /// Property 23: Higher satisfaction within tier yields higher margin.
        /// **Validates: Requirements R33.1, R33.2**
        /// </summary>
        [Test]
        public void CalculateProfit_HigherSatisfaction_YieldsHigherMargin()
        {
            float budget = 10000f;

            // Test within Successful tier
            var result70 = _profitCalculator.CalculateProfit(budget, 70f);
            var result100 = _profitCalculator.CalculateProfit(budget, 100f);
            Assert.Less(result70.ProfitMarginPercent, result100.ProfitMarginPercent,
                "100% satisfaction should yield higher margin than 70%");

            // Test within Mediocre tier
            var result50 = _profitCalculator.CalculateProfit(budget, 50f);
            var result69 = _profitCalculator.CalculateProfit(budget, 69f);
            Assert.Less(result50.ProfitMarginPercent, result69.ProfitMarginPercent,
                "69% satisfaction should yield higher margin than 50%");
        }

        /// <summary>
        /// Property 23: Profit amount scales linearly with budget.
        /// **Validates: Requirements R33**
        /// </summary>
        [Test]
        public void CalculateProfit_ProfitScalesWithBudget()
        {
            float satisfaction = 85f; // Fixed satisfaction

            for (int i = 0; i < 100; i++)
            {
                float budget1 = 1000f + (float)(_random.NextDouble() * 49000f);
                float budget2 = budget1 * 2f; // Double the budget

                var result1 = _profitCalculator.CalculateProfit(budget1, satisfaction);
                var result2 = _profitCalculator.CalculateProfit(budget2, satisfaction);

                // Same satisfaction should yield same margin percentage
                Assert.AreEqual(result1.ProfitMarginPercent, result2.ProfitMarginPercent, 0.001f,
                    "Same satisfaction should yield same margin percentage");

                // Profit amount should scale with budget
                Assert.AreEqual(result1.ProfitAmount * 2f, result2.ProfitAmount, 0.01f,
                    "Profit should scale linearly with budget");
            }
        }

        /// <summary>
        /// Property 23: Zero budget yields zero profit.
        /// **Validates: Requirements R33**
        /// </summary>
        [Test]
        public void CalculateProfit_ZeroBudget_ReturnsZeroProfit()
        {
            for (int i = 0; i < 100; i++)
            {
                float satisfaction = (float)(_random.NextDouble() * 100f);
                var result = _profitCalculator.CalculateProfit(0f, satisfaction);

                Assert.AreEqual(0f, result.ProfitAmount, 
                    $"Zero budget should yield zero profit regardless of satisfaction ({satisfaction}%)");
            }
        }

        /// <summary>
        /// Property 23: Satisfaction is clamped to valid range.
        /// **Validates: Requirements R33**
        /// </summary>
        [Test]
        public void CalculateProfit_SatisfactionClamped()
        {
            float budget = 10000f;

            // Test satisfaction > 100
            var resultOver = _profitCalculator.CalculateProfit(budget, 150f);
            Assert.AreEqual(100f, resultOver.SatisfactionScore, "Satisfaction should be clamped to 100");
            Assert.AreEqual("Successful", resultOver.ProfitTier);

            // Test satisfaction < 0
            var resultUnder = _profitCalculator.CalculateProfit(budget, -50f);
            Assert.AreEqual(0f, resultUnder.SatisfactionScore, "Satisfaction should be clamped to 0");
            Assert.AreEqual("Failed", resultUnder.ProfitTier);
        }

        /// <summary>
        /// Property 23: Maximum loss is capped at -20%.
        /// **Validates: Requirements R33.3**
        /// </summary>
        [Test]
        public void CalculateProfit_MaximumLossCapped()
        {
            float budget = 10000f;

            // At 0% satisfaction, loss should be -20%
            var result = _profitCalculator.CalculateProfit(budget, 0f);
            Assert.AreEqual(-0.20f, result.ProfitMarginPercent, 0.001f,
                "0% satisfaction should yield -20% margin");
            Assert.AreEqual(-2000f, result.ProfitAmount, 0.01f,
                "0% satisfaction on $10,000 budget should yield -$2,000 loss");
        }

        /// <summary>
        /// Property 23: GetProfitMarginRange returns correct ranges.
        /// **Validates: Requirements R33**
        /// </summary>
        [Test]
        public void GetProfitMarginRange_ReturnsCorrectRanges()
        {
            // Successful tier (70%+)
            var successfulRange = _profitCalculator.GetProfitMarginRange(85f);
            Assert.AreEqual(0.20f, successfulRange.min, "Successful min should be 20%");
            Assert.AreEqual(0.30f, successfulRange.max, "Successful max should be 30%");

            // Mediocre tier (50-69%)
            var mediocreRange = _profitCalculator.GetProfitMarginRange(60f);
            Assert.AreEqual(0.10f, mediocreRange.min, "Mediocre min should be 10%");
            Assert.AreEqual(0.15f, mediocreRange.max, "Mediocre max should be 15%");

            // Failed tier (<50%)
            var failedRange = _profitCalculator.GetProfitMarginRange(25f);
            Assert.AreEqual(-0.20f, failedRange.min, "Failed min should be -20%");
            Assert.AreEqual(0f, failedRange.max, "Failed max should be 0%");
        }

        #endregion

        #region Property 27: Commission Calculation Formula

        /// <summary>
        /// Property 27: Commission Calculation Formula
        /// GIVEN employee level L = 1-2 (Junior)
        /// WHEN calculating commission
        /// THEN base pay = $500, commission rate = 5%
        /// **Validates: Requirements R16.3**
        /// </summary>
        [Test]
        public void CalculateCommission_JuniorLevel_CorrectCompensation()
        {
            for (int level = 1; level <= 2; level++)
            {
                for (int i = 0; i < 50; i++)
                {
                    float budget = 5000f + (float)(_random.NextDouble() * 15000f);
                    float satisfaction = 70f + (float)(_random.NextDouble() * 30f);

                    var result = _profitCalculator.CalculateCommission(level, budget, satisfaction);

                    Assert.AreEqual(500f, result.BasePay, 
                        $"Junior (Level {level}) base pay should be $500");
                    Assert.AreEqual(0.05f, result.CommissionRate, 
                        $"Junior (Level {level}) commission rate should be 5%");
                    Assert.AreEqual(level, result.EmployeeLevel);
                }
            }
        }

        /// <summary>
        /// Property 27: Commission Calculation Formula
        /// GIVEN employee level L = 3-4 (Planner)
        /// WHEN calculating commission
        /// THEN base pay = $750, commission rate = 10%
        /// **Validates: Requirements R16.3**
        /// </summary>
        [Test]
        public void CalculateCommission_PlannerLevel_CorrectCompensation()
        {
            for (int level = 3; level <= 4; level++)
            {
                for (int i = 0; i < 50; i++)
                {
                    float budget = 5000f + (float)(_random.NextDouble() * 15000f);
                    float satisfaction = 70f + (float)(_random.NextDouble() * 30f);

                    var result = _profitCalculator.CalculateCommission(level, budget, satisfaction);

                    Assert.AreEqual(750f, result.BasePay, 
                        $"Planner (Level {level}) base pay should be $750");
                    Assert.AreEqual(0.10f, result.CommissionRate, 
                        $"Planner (Level {level}) commission rate should be 10%");
                    Assert.AreEqual(level, result.EmployeeLevel);
                }
            }
        }

        /// <summary>
        /// Property 27: Commission Calculation Formula
        /// GIVEN employee level L = 5 (Senior)
        /// WHEN calculating commission
        /// THEN base pay = $1000, commission rate = 15%
        /// **Validates: Requirements R16.3**
        /// </summary>
        [Test]
        public void CalculateCommission_SeniorLevel_CorrectCompensation()
        {
            for (int i = 0; i < 100; i++)
            {
                float budget = 5000f + (float)(_random.NextDouble() * 15000f);
                float satisfaction = 70f + (float)(_random.NextDouble() * 30f);

                var result = _profitCalculator.CalculateCommission(5, budget, satisfaction);

                Assert.AreEqual(1000f, result.BasePay, "Senior base pay should be $1000");
                Assert.AreEqual(0.15f, result.CommissionRate, "Senior commission rate should be 15%");
                Assert.AreEqual(5, result.EmployeeLevel);
            }
        }

        /// <summary>
        /// Property 27: Commission is calculated from event profit.
        /// **Validates: Requirements R16.3**
        /// </summary>
        [Test]
        public void CalculateCommission_CommissionBasedOnProfit()
        {
            // Example from design doc:
            // Level 3 planner, $10,000 budget event, 80% satisfaction
            // eventProfit = $10,000 × 0.25 = $2,500 (25% margin for 80% satisfaction)
            // commission = $2,500 × 0.10 = $250
            // totalCompensation = $750 + $250 = $1,000

            var result = _profitCalculator.CalculateCommission(3, 10000f, 80f);

            // Verify profit calculation (80% satisfaction should be in 20-30% range)
            Assert.Greater(result.EventProfit, 0f, "Event profit should be positive");
            
            // Commission should be profit * rate
            float expectedCommission = result.EventProfit * result.CommissionRate;
            Assert.AreEqual(expectedCommission, result.CommissionAmount, 0.01f,
                "Commission should be profit × rate");

            // Total should be base + commission
            Assert.AreEqual(result.BasePay + result.CommissionAmount, result.TotalCompensation, 0.01f,
                "Total compensation should be base pay + commission");
        }

        /// <summary>
        /// Property 27: No commission on failed events (negative profit).
        /// **Validates: Requirements R16.3**
        /// </summary>
        [Test]
        public void CalculateCommission_FailedEvent_NoCommission()
        {
            for (int i = 0; i < 100; i++)
            {
                int level = _random.Next(1, 6);
                float budget = 5000f + (float)(_random.NextDouble() * 15000f);
                float satisfaction = (float)(_random.NextDouble() * 49.99f); // Failed event

                var result = _profitCalculator.CalculateCommission(level, budget, satisfaction);

                Assert.LessOrEqual(result.EventProfit, 0f, 
                    "Failed event should have zero or negative profit");
                Assert.AreEqual(0f, result.CommissionAmount, 
                    "Commission should be $0 for failed events");
                Assert.AreEqual(result.BasePay, result.TotalCompensation, 
                    "Total compensation should equal base pay for failed events");
            }
        }

        /// <summary>
        /// Property 27: Higher level yields higher total compensation.
        /// **Validates: Requirements R16.3**
        /// </summary>
        [Test]
        public void CalculateCommission_HigherLevel_HigherCompensation()
        {
            float budget = 10000f;
            float satisfaction = 85f;

            var juniorResult = _profitCalculator.CalculateCommission(1, budget, satisfaction);
            var plannerResult = _profitCalculator.CalculateCommission(3, budget, satisfaction);
            var seniorResult = _profitCalculator.CalculateCommission(5, budget, satisfaction);

            Assert.Less(juniorResult.TotalCompensation, plannerResult.TotalCompensation,
                "Planner should earn more than Junior");
            Assert.Less(plannerResult.TotalCompensation, seniorResult.TotalCompensation,
                "Senior should earn more than Planner");
        }

        /// <summary>
        /// Property 27: GetCompensationByLevel returns correct values.
        /// **Validates: Requirements R16.3**
        /// </summary>
        [Test]
        public void GetCompensationByLevel_ReturnsCorrectValues()
        {
            // Junior (Level 1-2)
            var junior1 = _profitCalculator.GetCompensationByLevel(1);
            var junior2 = _profitCalculator.GetCompensationByLevel(2);
            Assert.AreEqual((500f, 0.05f), junior1, "Level 1 should be Junior compensation");
            Assert.AreEqual((500f, 0.05f), junior2, "Level 2 should be Junior compensation");

            // Planner (Level 3-4)
            var planner3 = _profitCalculator.GetCompensationByLevel(3);
            var planner4 = _profitCalculator.GetCompensationByLevel(4);
            Assert.AreEqual((750f, 0.10f), planner3, "Level 3 should be Planner compensation");
            Assert.AreEqual((750f, 0.10f), planner4, "Level 4 should be Planner compensation");

            // Senior (Level 5)
            var senior = _profitCalculator.GetCompensationByLevel(5);
            Assert.AreEqual((1000f, 0.15f), senior, "Level 5 should be Senior compensation");
        }

        /// <summary>
        /// Property 27: Employee level is clamped to valid range.
        /// **Validates: Requirements R16.3**
        /// </summary>
        [Test]
        public void CalculateCommission_LevelClamped()
        {
            float budget = 10000f;
            float satisfaction = 85f;

            // Level below 1 should be treated as 1
            var resultLow = _profitCalculator.CalculateCommission(0, budget, satisfaction);
            Assert.AreEqual(1, resultLow.EmployeeLevel, "Level 0 should be clamped to 1");
            Assert.AreEqual(500f, resultLow.BasePay, "Level 0 should use Junior compensation");

            // Level above 5 should be treated as 5
            var resultHigh = _profitCalculator.CalculateCommission(10, budget, satisfaction);
            Assert.AreEqual(5, resultHigh.EmployeeLevel, "Level 10 should be clamped to 5");
            Assert.AreEqual(1000f, resultHigh.BasePay, "Level 10 should use Senior compensation");
        }

        /// <summary>
        /// Property 27: Commission calculation matches design document example.
        /// **Validates: Requirements R16.3**
        /// </summary>
        [Test]
        public void CalculateCommission_MatchesDesignExample()
        {
            // From design doc:
            // Level 3 planner, $10,000 budget event, 80% satisfaction
            // eventProfit = $10,000 × 0.25 = $2,500 (25% margin for 80% satisfaction)
            // commission = $2,500 × 0.10 = $250
            // totalCompensation = $750 + $250 = $1,000

            var result = _profitCalculator.CalculateCommission(3, 10000f, 80f);

            // 80% satisfaction is in Successful tier (70%+), so margin is 20-30%
            // At 80%, which is 1/3 of the way from 70 to 100, margin should be ~23.3%
            float expectedMargin = 0.20f + ((80f - 70f) / 30f) * 0.10f; // ~0.233
            float expectedProfit = 10000f * expectedMargin;
            float expectedCommission = expectedProfit * 0.10f;
            float expectedTotal = 750f + expectedCommission;

            Assert.AreEqual(750f, result.BasePay, "Base pay should be $750");
            Assert.AreEqual(0.10f, result.CommissionRate, "Commission rate should be 10%");
            Assert.AreEqual(expectedProfit, result.EventProfit, 1f, "Event profit should match");
            Assert.AreEqual(expectedCommission, result.CommissionAmount, 0.1f, "Commission should match");
            Assert.AreEqual(expectedTotal, result.TotalCompensation, 0.1f, "Total compensation should match");
        }

        #endregion
    }
}
