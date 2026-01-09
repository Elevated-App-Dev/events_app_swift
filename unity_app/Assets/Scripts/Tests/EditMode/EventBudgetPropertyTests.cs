using System;
using NUnit.Framework;
using EventPlannerSim.Data;

namespace EventPlannerSim.Tests.EditMode
{
    /// <summary>
    /// Property-based tests for EventBudget calculations.
    /// Feature: event-planner-simulator, Property 7: Budget Allocation Math
    /// Validates: Requirements R7
    /// </summary>
    [TestFixture]
    public class EventBudgetPropertyTests
    {
        private Random _random;

        [SetUp]
        public void Setup()
        {
            _random = new Random(42); // Fixed seed for reproducibility
        }

        /// <summary>
        /// Property 7: Budget Allocation Math - Remaining calculation
        /// For any EventBudget B: B.Remaining == B.total - B.spent
        /// **Validates: Requirements R7**
        /// </summary>
        [Test]
        public void EventBudget_Remaining_EqualsTotal_MinusSpent()
        {
            // Run 100 iterations as per testing strategy
            for (int i = 0; i < 100; i++)
            {
                var budget = new EventBudget
                {
                    total = (float)(_random.NextDouble() * 100000), // 0 to 100,000
                    spent = (float)(_random.NextDouble() * 150000)  // 0 to 150,000 (can overspend)
                };

                float expected = budget.total - budget.spent;
                float actual = budget.Remaining;

                // Use larger tolerance (0.1) due to float precision with large numbers
                // Float precision degrades with larger values, so we need a proportionally larger tolerance
                Assert.AreEqual(expected, actual, 0.1f,
                    $"Remaining calculation failed. Total: {budget.total}, Spent: {budget.spent}");
            }
        }

        /// <summary>
        /// Property 7: Budget Allocation Math - OverageAmount calculation
        /// For any EventBudget B: B.OverageAmount == max(0, B.spent - B.total)
        /// **Validates: Requirements R7**
        /// </summary>
        [Test]
        public void EventBudget_OverageAmount_EqualsMax_ZeroOrOverspend()
        {
            for (int i = 0; i < 100; i++)
            {
                var budget = new EventBudget
                {
                    total = (float)(_random.NextDouble() * 100000),
                    spent = (float)(_random.NextDouble() * 150000)
                };

                float expected = Math.Max(0, budget.spent - budget.total);
                float actual = budget.OverageAmount;

                Assert.AreEqual(expected, actual, 0.001f,
                    $"OverageAmount calculation failed. Total: {budget.total}, Spent: {budget.spent}");
            }
        }

        /// <summary>
        /// Property 7: Budget Allocation Math - OveragePercent calculation
        /// For any EventBudget B with total > 0: B.OveragePercent == (B.OverageAmount / B.total) * 100
        /// **Validates: Requirements R7**
        /// </summary>
        [Test]
        public void EventBudget_OveragePercent_CalculatesCorrectly()
        {
            for (int i = 0; i < 100; i++)
            {
                var budget = new EventBudget
                {
                    total = (float)(_random.NextDouble() * 100000) + 1, // 1 to 100,001 (avoid zero)
                    spent = (float)(_random.NextDouble() * 150000)
                };

                float overageAmount = Math.Max(0, budget.spent - budget.total);
                float expected = (overageAmount / budget.total) * 100f;
                float actual = budget.OveragePercent;

                Assert.AreEqual(expected, actual, 0.001f,
                    $"OveragePercent calculation failed. Total: {budget.total}, Spent: {budget.spent}, OverageAmount: {overageAmount}");
            }
        }

        /// <summary>
        /// Property 7: Budget Allocation Math - OveragePercent with zero total
        /// When total is 0, OveragePercent should return 0 (avoid division by zero)
        /// **Validates: Requirements R7**
        /// </summary>
        [Test]
        public void EventBudget_OveragePercent_ReturnsZero_WhenTotalIsZero()
        {
            for (int i = 0; i < 100; i++)
            {
                var budget = new EventBudget
                {
                    total = 0,
                    spent = (float)(_random.NextDouble() * 10000)
                };

                Assert.AreEqual(0f, budget.OveragePercent,
                    $"OveragePercent should be 0 when total is 0. Spent: {budget.spent}");
            }
        }

        /// <summary>
        /// Property 7: Budget Allocation Math - OverageAmount is never negative
        /// For any EventBudget B: B.OverageAmount >= 0
        /// **Validates: Requirements R7**
        /// </summary>
        [Test]
        public void EventBudget_OverageAmount_IsNeverNegative()
        {
            for (int i = 0; i < 100; i++)
            {
                var budget = new EventBudget
                {
                    total = (float)(_random.NextDouble() * 100000),
                    spent = (float)(_random.NextDouble() * 100000)
                };

                Assert.GreaterOrEqual(budget.OverageAmount, 0f,
                    $"OverageAmount should never be negative. Total: {budget.total}, Spent: {budget.spent}");
            }
        }

        /// <summary>
        /// Property 7: Budget Allocation Math - Under budget means zero overage
        /// When spent <= total, OverageAmount should be 0
        /// **Validates: Requirements R7**
        /// </summary>
        [Test]
        public void EventBudget_UnderBudget_HasZeroOverage()
        {
            for (int i = 0; i < 100; i++)
            {
                float total = (float)(_random.NextDouble() * 100000) + 100; // At least 100
                float spent = (float)(_random.NextDouble() * total); // Always under or at budget

                var budget = new EventBudget
                {
                    total = total,
                    spent = spent
                };

                Assert.AreEqual(0f, budget.OverageAmount, 0.001f,
                    $"OverageAmount should be 0 when under budget. Total: {total}, Spent: {spent}");
                Assert.AreEqual(0f, budget.OveragePercent, 0.001f,
                    $"OveragePercent should be 0 when under budget. Total: {total}, Spent: {spent}");
            }
        }
    }
}
