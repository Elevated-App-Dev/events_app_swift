using System;
using NUnit.Framework;
using EventPlannerSim.Core;
using EventPlannerSim.Systems;

namespace EventPlannerSim.Tests.EditMode
{
    /// <summary>
    /// Property-based tests for EmergencyFundingSystem.
    /// Feature: event-planner-simulator
    /// Property 24: Family Help Diminishing Returns
    /// **Validates: Requirements R34**
    /// </summary>
    [TestFixture]
    public class EmergencyFundingPropertyTests
    {
        private EmergencyFundingSystemImpl _emergencyFundingSystem;
        private Random _random;

        [SetUp]
        public void Setup()
        {
            _random = new Random(42);
            _emergencyFundingSystem = new EmergencyFundingSystemImpl();
        }

        #region Property 24: Family Help Diminishing Returns

        /// <summary>
        /// Property 24: Family Help Diminishing Returns
        /// GIVEN family help request count N (max 3 in Stages 1-2)
        /// WHEN requesting family help:
        ///   N == 0: Receive $500
        ///   N == 1: Receive $400
        ///   N == 2: Receive $300
        ///   N >= 3: Option disabled
        /// INVARIANT: familyHelpUsed <= 3
        /// **Validates: Requirements R34**
        /// </summary>
        [Test]
        public void GetFamilyHelpAmount_FirstRequest_Returns500()
        {
            // R34.3: First request provides $500
            float amount = _emergencyFundingSystem.GetFamilyHelpAmount(0);
            Assert.AreEqual(500f, amount, "First family help request should provide $500");
        }

        /// <summary>
        /// Property 24: Second request returns $400.
        /// **Validates: Requirements R34.4**
        /// </summary>
        [Test]
        public void GetFamilyHelpAmount_SecondRequest_Returns400()
        {
            // R34.4: Second request provides $400
            float amount = _emergencyFundingSystem.GetFamilyHelpAmount(1);
            Assert.AreEqual(400f, amount, "Second family help request should provide $400");
        }

        /// <summary>
        /// Property 24: Third request returns $300.
        /// **Validates: Requirements R34.5**
        /// </summary>
        [Test]
        public void GetFamilyHelpAmount_ThirdRequest_Returns300()
        {
            // R34.5: Third request provides $300
            float amount = _emergencyFundingSystem.GetFamilyHelpAmount(2);
            Assert.AreEqual(300f, amount, "Third family help request should provide $300");
        }

        /// <summary>
        /// Property 24: After 3 requests, no more help available.
        /// **Validates: Requirements R34.6**
        /// </summary>
        [Test]
        public void GetFamilyHelpAmount_AfterThreeRequests_ReturnsZero()
        {
            // R34.6: No more help after 3 requests
            for (int i = 0; i < 100; i++)
            {
                int exhaustedCount = 3 + _random.Next(0, 100); // 3 or more
                float amount = _emergencyFundingSystem.GetFamilyHelpAmount(exhaustedCount);
                Assert.AreEqual(0f, amount, 
                    $"Family help should return $0 when {exhaustedCount} requests have been used");
            }
        }

        /// <summary>
        /// Property 24: Family help is only available in Stages 1-2.
        /// **Validates: Requirements R34.1**
        /// </summary>
        [Test]
        public void IsFamilyHelpAvailable_Stage1_ReturnsTrue()
        {
            // R34.1: Family help available in Stage 1
            for (int helpUsed = 0; helpUsed < 3; helpUsed++)
            {
                bool available = _emergencyFundingSystem.IsFamilyHelpAvailable(BusinessStage.Solo, helpUsed);
                Assert.IsTrue(available, 
                    $"Family help should be available in Stage 1 with {helpUsed} requests used");
            }
        }

        /// <summary>
        /// Property 24: Family help is available in Stage 2.
        /// **Validates: Requirements R34.1**
        /// </summary>
        [Test]
        public void IsFamilyHelpAvailable_Stage2_ReturnsTrue()
        {
            // R34.1: Family help available in Stage 2
            for (int helpUsed = 0; helpUsed < 3; helpUsed++)
            {
                bool available = _emergencyFundingSystem.IsFamilyHelpAvailable(BusinessStage.Employee, helpUsed);
                Assert.IsTrue(available, 
                    $"Family help should be available in Stage 2 with {helpUsed} requests used");
            }
        }

        /// <summary>
        /// Property 24: Family help is NOT available in Stages 3-5.
        /// **Validates: Requirements R34.1**
        /// </summary>
        [Test]
        public void IsFamilyHelpAvailable_Stage3Plus_ReturnsFalse()
        {
            // R34.1: Family help only in Stages 1-2
            var laterStages = new[] { BusinessStage.SmallCompany, BusinessStage.Established, BusinessStage.Premier };
            
            foreach (var stage in laterStages)
            {
                for (int helpUsed = 0; helpUsed < 3; helpUsed++)
                {
                    bool available = _emergencyFundingSystem.IsFamilyHelpAvailable(stage, helpUsed);
                    Assert.IsFalse(available, 
                        $"Family help should NOT be available in {stage} with {helpUsed} requests used");
                }
            }
        }

        /// <summary>
        /// Property 24: Family help is limited to 3 requests total.
        /// **Validates: Requirements R34.2**
        /// </summary>
        [Test]
        public void IsFamilyHelpAvailable_AfterThreeRequests_ReturnsFalse()
        {
            // R34.2: Limited to 3 requests total
            var validStages = new[] { BusinessStage.Solo, BusinessStage.Employee };
            
            foreach (var stage in validStages)
            {
                for (int i = 0; i < 100; i++)
                {
                    int exhaustedCount = 3 + _random.Next(0, 100); // 3 or more
                    bool available = _emergencyFundingSystem.IsFamilyHelpAvailable(stage, exhaustedCount);
                    Assert.IsFalse(available, 
                        $"Family help should NOT be available in {stage} after {exhaustedCount} requests");
                }
            }
        }

        /// <summary>
        /// Property 24: Low funds threshold is $500.
        /// **Validates: Requirements R34.1**
        /// </summary>
        [Test]
        public void IsLowOnFunds_BelowThreshold_ReturnsTrue()
        {
            // R34.1: Funds below $500 enables family help
            for (int i = 0; i < 100; i++)
            {
                float lowMoney = (float)(_random.NextDouble() * 499.99f); // 0-499.99
                bool isLow = _emergencyFundingSystem.IsLowOnFunds(lowMoney);
                Assert.IsTrue(isLow, $"${lowMoney:F2} should be considered low on funds");
            }
        }

        /// <summary>
        /// Property 24: Funds at or above $500 are not low.
        /// **Validates: Requirements R34.1**
        /// </summary>
        [Test]
        public void IsLowOnFunds_AtOrAboveThreshold_ReturnsFalse()
        {
            // R34.1: Funds at or above $500 does not enable family help
            for (int i = 0; i < 100; i++)
            {
                float normalMoney = 500f + (float)(_random.NextDouble() * 10000f); // 500+
                bool isLow = _emergencyFundingSystem.IsLowOnFunds(normalMoney);
                Assert.IsFalse(isLow, $"${normalMoney:F2} should NOT be considered low on funds");
            }
        }

        /// <summary>
        /// Property 24: Boundary test at $500 threshold.
        /// **Validates: Requirements R34.1**
        /// </summary>
        [Test]
        public void IsLowOnFunds_BoundaryValues()
        {
            // Test exact boundary
            Assert.IsFalse(_emergencyFundingSystem.IsLowOnFunds(500f), "$500 exactly should NOT be low");
            Assert.IsTrue(_emergencyFundingSystem.IsLowOnFunds(499.99f), "$499.99 should be low");
            Assert.IsTrue(_emergencyFundingSystem.IsLowOnFunds(0f), "$0 should be low");
            Assert.IsFalse(_emergencyFundingSystem.IsLowOnFunds(500.01f), "$500.01 should NOT be low");
        }

        /// <summary>
        /// Property 24: Remaining requests calculation is correct.
        /// **Validates: Requirements R34.7**
        /// </summary>
        [Test]
        public void GetRemainingFamilyHelpRequests_ReturnsCorrectCount()
        {
            // R34.7: Track remaining requests
            Assert.AreEqual(3, _emergencyFundingSystem.GetRemainingFamilyHelpRequests(0), "0 used = 3 remaining");
            Assert.AreEqual(2, _emergencyFundingSystem.GetRemainingFamilyHelpRequests(1), "1 used = 2 remaining");
            Assert.AreEqual(1, _emergencyFundingSystem.GetRemainingFamilyHelpRequests(2), "2 used = 1 remaining");
            Assert.AreEqual(0, _emergencyFundingSystem.GetRemainingFamilyHelpRequests(3), "3 used = 0 remaining");
        }

        /// <summary>
        /// Property 24: Remaining requests never goes negative.
        /// **Validates: Requirements R34.7**
        /// </summary>
        [Test]
        public void GetRemainingFamilyHelpRequests_NeverNegative()
        {
            for (int i = 0; i < 100; i++)
            {
                int helpUsed = _random.Next(0, 1000);
                int remaining = _emergencyFundingSystem.GetRemainingFamilyHelpRequests(helpUsed);
                Assert.GreaterOrEqual(remaining, 0, 
                    $"Remaining requests should never be negative (got {remaining} for {helpUsed} used)");
            }
        }

        /// <summary>
        /// Property 24: RequestFamilyHelp returns correct result for first request.
        /// **Validates: Requirements R34.3**
        /// </summary>
        [Test]
        public void RequestFamilyHelp_FirstRequest_ReturnsCorrectResult()
        {
            var result = _emergencyFundingSystem.RequestFamilyHelp(BusinessStage.Solo, 0);
            
            Assert.IsTrue(result.Success, "First request should succeed");
            Assert.AreEqual(500f, result.AmountReceived, "First request should provide $500");
            Assert.AreEqual(1, result.NewFamilyHelpUsed, "Help used should be 1 after first request");
            Assert.AreEqual(2, result.RemainingRequests, "Should have 2 remaining after first request");
            Assert.AreEqual(1, result.RequestNumber, "Request number should be 1");
            Assert.IsNotNull(result.FamilyMessage, "Should have a family message");
        }

        /// <summary>
        /// Property 24: RequestFamilyHelp returns correct result for second request.
        /// **Validates: Requirements R34.4**
        /// </summary>
        [Test]
        public void RequestFamilyHelp_SecondRequest_ReturnsCorrectResult()
        {
            var result = _emergencyFundingSystem.RequestFamilyHelp(BusinessStage.Solo, 1);
            
            Assert.IsTrue(result.Success, "Second request should succeed");
            Assert.AreEqual(400f, result.AmountReceived, "Second request should provide $400");
            Assert.AreEqual(2, result.NewFamilyHelpUsed, "Help used should be 2 after second request");
            Assert.AreEqual(1, result.RemainingRequests, "Should have 1 remaining after second request");
            Assert.AreEqual(2, result.RequestNumber, "Request number should be 2");
        }

        /// <summary>
        /// Property 24: RequestFamilyHelp returns correct result for third request.
        /// **Validates: Requirements R34.5**
        /// </summary>
        [Test]
        public void RequestFamilyHelp_ThirdRequest_ReturnsCorrectResult()
        {
            var result = _emergencyFundingSystem.RequestFamilyHelp(BusinessStage.Employee, 2);
            
            Assert.IsTrue(result.Success, "Third request should succeed");
            Assert.AreEqual(300f, result.AmountReceived, "Third request should provide $300");
            Assert.AreEqual(3, result.NewFamilyHelpUsed, "Help used should be 3 after third request");
            Assert.AreEqual(0, result.RemainingRequests, "Should have 0 remaining after third request");
            Assert.AreEqual(3, result.RequestNumber, "Request number should be 3");
        }

        /// <summary>
        /// Property 24: RequestFamilyHelp fails when help is exhausted.
        /// **Validates: Requirements R34.6**
        /// </summary>
        [Test]
        public void RequestFamilyHelp_Exhausted_ReturnsFailed()
        {
            var result = _emergencyFundingSystem.RequestFamilyHelp(BusinessStage.Solo, 3);
            
            Assert.IsFalse(result.Success, "Request should fail when help is exhausted");
            Assert.AreEqual(0f, result.AmountReceived, "Should receive $0 when exhausted");
            Assert.AreEqual(3, result.NewFamilyHelpUsed, "Help used should remain at 3");
            Assert.AreEqual(0, result.RemainingRequests, "Should have 0 remaining");
            Assert.AreEqual("Family cannot help further.", result.FailureReason, "Should have correct failure reason");
        }

        /// <summary>
        /// Property 24: RequestFamilyHelp fails in Stage 3+.
        /// **Validates: Requirements R34.1**
        /// </summary>
        [Test]
        public void RequestFamilyHelp_Stage3Plus_ReturnsFailed()
        {
            var laterStages = new[] { BusinessStage.SmallCompany, BusinessStage.Established, BusinessStage.Premier };
            
            foreach (var stage in laterStages)
            {
                var result = _emergencyFundingSystem.RequestFamilyHelp(stage, 0);
                
                Assert.IsFalse(result.Success, $"Request should fail in {stage}");
                Assert.AreEqual(0f, result.AmountReceived, $"Should receive $0 in {stage}");
                Assert.IsNotNull(result.FailureReason, $"Should have failure reason in {stage}");
            }
        }

        /// <summary>
        /// Property 24: Family messages are appropriate for each request.
        /// **Validates: Requirements R34.3, R34.4, R34.5**
        /// </summary>
        [Test]
        public void GetFamilyMessage_ReturnsAppropriateMessages()
        {
            // First message should be supportive (R34.3)
            string firstMessage = _emergencyFundingSystem.GetFamilyMessage(0);
            Assert.IsNotNull(firstMessage, "First message should not be null");
            Assert.IsTrue(firstMessage.Contains("500"), "First message should mention $500");

            // Second message should be worried (R34.4)
            string secondMessage = _emergencyFundingSystem.GetFamilyMessage(1);
            Assert.IsNotNull(secondMessage, "Second message should not be null");
            Assert.IsTrue(secondMessage.Contains("400"), "Second message should mention $400");

            // Third message should indicate last time (R34.5)
            string thirdMessage = _emergencyFundingSystem.GetFamilyMessage(2);
            Assert.IsNotNull(thirdMessage, "Third message should not be null");
            Assert.IsTrue(thirdMessage.Contains("300"), "Third message should mention $300");
            Assert.IsTrue(thirdMessage.ToLower().Contains("last"), "Third message should indicate this is the last time");

            // Exhausted message (R34.6)
            string exhaustedMessage = _emergencyFundingSystem.GetFamilyMessage(3);
            Assert.IsNotNull(exhaustedMessage, "Exhausted message should not be null");
            Assert.IsTrue(exhaustedMessage.ToLower().Contains("cannot"), "Exhausted message should indicate family cannot help");
        }

        /// <summary>
        /// Property 24: INVARIANT - familyHelpUsed never exceeds 3 through normal operations.
        /// **Validates: Requirements R34.2**
        /// </summary>
        [Test]
        public void FamilyHelpUsed_Invariant_NeverExceedsThree()
        {
            // Simulate multiple requests and verify invariant
            int helpUsed = 0;
            
            for (int i = 0; i < 10; i++)
            {
                var result = _emergencyFundingSystem.RequestFamilyHelp(BusinessStage.Solo, helpUsed);
                
                if (result.Success)
                {
                    helpUsed = result.NewFamilyHelpUsed;
                }
                
                Assert.LessOrEqual(helpUsed, 3, 
                    $"familyHelpUsed should never exceed 3 (got {helpUsed} after {i + 1} attempts)");
            }
        }

        /// <summary>
        /// Property 24: Diminishing returns are strictly decreasing.
        /// **Validates: Requirements R34.3, R34.4, R34.5**
        /// </summary>
        [Test]
        public void FamilyHelpAmounts_StrictlyDecreasing()
        {
            float firstAmount = _emergencyFundingSystem.GetFamilyHelpAmount(0);
            float secondAmount = _emergencyFundingSystem.GetFamilyHelpAmount(1);
            float thirdAmount = _emergencyFundingSystem.GetFamilyHelpAmount(2);
            
            Assert.Greater(firstAmount, secondAmount, "First amount should be greater than second");
            Assert.Greater(secondAmount, thirdAmount, "Second amount should be greater than third");
            Assert.Greater(thirdAmount, 0f, "Third amount should be greater than zero");
        }

        /// <summary>
        /// Property 24: Total family help across all requests equals $1200.
        /// **Validates: Requirements R34.3, R34.4, R34.5**
        /// </summary>
        [Test]
        public void FamilyHelpAmounts_TotalEquals1200()
        {
            float total = _emergencyFundingSystem.GetFamilyHelpAmount(0) +
                         _emergencyFundingSystem.GetFamilyHelpAmount(1) +
                         _emergencyFundingSystem.GetFamilyHelpAmount(2);
            
            Assert.AreEqual(1200f, total, "Total family help should be $1200 ($500 + $400 + $300)");
        }

        /// <summary>
        /// Property 24: Full sequence of family help requests works correctly.
        /// **Validates: Requirements R34.1-R34.7**
        /// </summary>
        [Test]
        public void FamilyHelp_FullSequence_WorksCorrectly()
        {
            int helpUsed = 0;
            float totalReceived = 0f;
            
            // First request
            var result1 = _emergencyFundingSystem.RequestFamilyHelp(BusinessStage.Solo, helpUsed);
            Assert.IsTrue(result1.Success);
            Assert.AreEqual(500f, result1.AmountReceived);
            helpUsed = result1.NewFamilyHelpUsed;
            totalReceived += result1.AmountReceived;
            
            // Second request
            var result2 = _emergencyFundingSystem.RequestFamilyHelp(BusinessStage.Employee, helpUsed);
            Assert.IsTrue(result2.Success);
            Assert.AreEqual(400f, result2.AmountReceived);
            helpUsed = result2.NewFamilyHelpUsed;
            totalReceived += result2.AmountReceived;
            
            // Third request
            var result3 = _emergencyFundingSystem.RequestFamilyHelp(BusinessStage.Solo, helpUsed);
            Assert.IsTrue(result3.Success);
            Assert.AreEqual(300f, result3.AmountReceived);
            helpUsed = result3.NewFamilyHelpUsed;
            totalReceived += result3.AmountReceived;
            
            // Fourth request should fail
            var result4 = _emergencyFundingSystem.RequestFamilyHelp(BusinessStage.Solo, helpUsed);
            Assert.IsFalse(result4.Success);
            Assert.AreEqual(0f, result4.AmountReceived);
            
            // Verify totals
            Assert.AreEqual(3, helpUsed, "Should have used all 3 requests");
            Assert.AreEqual(1200f, totalReceived, "Should have received $1200 total");
        }

        #endregion
    }
}
