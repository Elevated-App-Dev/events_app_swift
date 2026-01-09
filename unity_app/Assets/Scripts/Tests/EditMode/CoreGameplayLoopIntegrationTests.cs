using System;
using System.Collections.Generic;
using NUnit.Framework;
using EventPlannerSim.Core;
using EventPlannerSim.Data;
using EventPlannerSim.Systems;

namespace EventPlannerSim.Tests.EditMode
{
    /// <summary>
    /// Integration tests for the core gameplay loop.
    /// Tests the full flow: Accept inquiry → Plan event → Execute → Results
    /// Requirements: R5, R13, R14
    /// </summary>
    [TestFixture]
    public class CoreGameplayLoopIntegrationTests
    {
        private IEventPlanningSystem _eventPlanningSystem;
        private ISatisfactionCalculator _satisfactionCalculator;
        private IProgressionSystem _progressionSystem;
        private IReferralSystem _referralSystem;
        private IProfitCalculator _profitCalculator;
        private Random _random;

        [SetUp]
        public void Setup()
        {
            _random = new Random(42);
            _eventPlanningSystem = new EventPlanningSystemImpl(42);
            _satisfactionCalculator = new SatisfactionCalculatorImpl();
            _progressionSystem = new ProgressionSystemImpl();
            _referralSystem = new ReferralSystemImpl();
            _profitCalculator = new ProfitCalculatorImpl();
        }

        #region Full Gameplay Loop Tests

        /// <summary>
        /// Test complete gameplay loop from inquiry to results with positive outcome.
        /// Requirements: R5, R13, R14
        /// </summary>
        [Test]
        public void FullGameplayLoop_SuccessfulEvent_PositiveOutcome()
        {
            // Arrange - Stage 1 player
            var currentDate = new GameDate(1, 1, 1);
            int stage = 1;
            int reputation = 0;
            int excellenceStreak = 0;

            // Step 1: Generate and accept inquiry
            var inquiry = _eventPlanningSystem.GenerateInquiry(stage, reputation, currentDate);
            Assert.IsNotNull(inquiry);
            Assert.IsFalse(string.IsNullOrEmpty(inquiry.clientName));

            var eventData = _eventPlanningSystem.AcceptInquiry(inquiry, currentDate);
            Assert.IsNotNull(eventData);
            Assert.AreEqual(EventStatus.Accepted, eventData.status);
            Assert.AreEqual(inquiry.budget, eventData.budget.total);

            // Step 2: Plan event - book venue and vendor
            var venue = CreateTestVenue(50, 30, 100);
            var venueResult = _eventPlanningSystem.BookVenue(eventData, venue, null);
            Assert.IsTrue(venueResult.Success);

            var caterer = CreateTestVendor(VendorCategory.Caterer, 500f);
            var catererResult = _eventPlanningSystem.BookVendor(eventData, caterer, null);
            Assert.IsTrue(catererResult.Success);

            // Step 3: Execute event - simulate completion with good scores
            eventData.status = EventStatus.Completed;
            eventData.results = new EventResults
            {
                venueScore = 85f,
                foodScore = 80f,
                entertainmentScore = 75f,
                decorationScore = 70f,
                serviceScore = 80f,
                expectationScore = 78f
            };

            // Step 4: Calculate results
            var client = new ClientData { personality = inquiry.personality };
            var satisfactionResult = _satisfactionCalculator.Calculate(eventData, client);
            
            Assert.GreaterOrEqual(satisfactionResult.FinalSatisfaction, 0f);
            Assert.LessOrEqual(satisfactionResult.FinalSatisfaction, 100f);

            // Step 5: Apply reputation change
            var reputationChange = _progressionSystem.ApplyEventResult(
                satisfactionResult.FinalSatisfaction, reputation, excellenceStreak);
            
            Assert.IsNotNull(reputationChange);
            
            // With good scores, reputation should increase
            if (satisfactionResult.FinalSatisfaction >= 70f)
            {
                Assert.GreaterOrEqual(reputationChange.Amount, 0);
            }

            // Step 6: Calculate profit
            var profitResult = _profitCalculator.CalculateProfit(
                eventData.budget.total, satisfactionResult.FinalSatisfaction);
            
            Assert.IsNotNull(profitResult);
        }

        /// <summary>
        /// Test complete gameplay loop with poor performance leading to negative outcome.
        /// Requirements: R5, R13, R14
        /// </summary>
        [Test]
        public void FullGameplayLoop_PoorEvent_NegativeOutcome()
        {
            // Arrange
            var currentDate = new GameDate(1, 1, 1);
            int stage = 1;
            int reputation = 50;

            // Step 1: Generate and accept inquiry
            var inquiry = _eventPlanningSystem.GenerateInquiry(stage, reputation, currentDate);
            var eventData = _eventPlanningSystem.AcceptInquiry(inquiry, currentDate);

            // Step 2: Skip proper planning (no venue/vendor booking)
            // Step 3: Execute with poor scores
            eventData.status = EventStatus.Completed;
            eventData.results = new EventResults
            {
                venueScore = 30f,
                foodScore = 25f,
                entertainmentScore = 20f,
                decorationScore = 35f,
                serviceScore = 30f,
                expectationScore = 28f
            };

            // Step 4: Calculate results
            var client = new ClientData { personality = inquiry.personality };
            var satisfactionResult = _satisfactionCalculator.Calculate(eventData, client);
            
            // Poor scores should result in low satisfaction
            Assert.Less(satisfactionResult.FinalSatisfaction, 50f);

            // Step 5: Apply reputation change - should be negative
            var reputationChange = _progressionSystem.ApplyEventResult(
                satisfactionResult.FinalSatisfaction, reputation, 0);
            
            Assert.Less(reputationChange.Amount, 0);
        }

        /// <summary>
        /// Test gameplay loop with referral generation on excellent performance.
        /// Requirements: R5, R13, R14, R23
        /// </summary>
        [Test]
        public void FullGameplayLoop_ExcellentEvent_GeneratesReferral()
        {
            // Arrange
            var currentDate = new GameDate(1, 1, 1);
            int stage = 1;
            int reputation = 50;
            int excellenceStreak = 2; // Already have a streak

            // Step 1: Generate and accept inquiry
            var inquiry = _eventPlanningSystem.GenerateInquiry(stage, reputation, currentDate);
            var eventData = _eventPlanningSystem.AcceptInquiry(inquiry, currentDate);

            // Step 2: Plan with excellent vendors
            var venue = CreateTestVenue(50, 30, 100);
            _eventPlanningSystem.BookVenue(eventData, venue, null);

            var caterer = CreateTestVendor(VendorCategory.Caterer, 800f);
            _eventPlanningSystem.BookVendor(eventData, caterer, null);

            // Step 3: Execute with excellent scores
            eventData.status = EventStatus.Completed;
            eventData.results = new EventResults
            {
                venueScore = 95f,
                foodScore = 92f,
                entertainmentScore = 90f,
                decorationScore = 88f,
                serviceScore = 93f,
                expectationScore = 91f
            };

            // Step 4: Calculate results
            var client = new ClientData { personality = ClientPersonality.EasyGoing };
            var satisfactionResult = _satisfactionCalculator.Calculate(eventData, client);
            
            Assert.GreaterOrEqual(satisfactionResult.FinalSatisfaction, 90f);

            // Step 5: Check referral probability
            var referralProbability = _referralSystem.CalculateReferralProbability(
                satisfactionResult.FinalSatisfaction, excellenceStreak);
            
            // With 90%+ satisfaction, should have positive referral chance
            Assert.Greater(referralProbability, 0f);

            // Step 6: Update excellence streak
            var streakResult = _referralSystem.UpdateExcellenceStreak(
                excellenceStreak, satisfactionResult.FinalSatisfaction);
            
            Assert.AreEqual(excellenceStreak + 1, streakResult.NewStreak);
        }

        #endregion

        #region Stage Progression Integration Tests

        /// <summary>
        /// Test that completing events contributes to stage progression.
        /// Requirements: R14.7-R14.12
        /// </summary>
        [Test]
        public void StageProgression_MultipleSuccessfulEvents_BuildsReputation()
        {
            var currentDate = new GameDate(1, 1, 1);
            int stage = 1;
            int reputation = 0;
            int excellenceStreak = 0;

            // Simulate multiple successful events
            for (int i = 0; i < 10; i++)
            {
                var inquiry = _eventPlanningSystem.GenerateInquiry(stage, reputation, currentDate);
                var eventData = _eventPlanningSystem.AcceptInquiry(inquiry, currentDate);
                
                // Simulate good performance
                eventData.status = EventStatus.Completed;
                eventData.results = new EventResults
                {
                    venueScore = 80f + _random.Next(0, 15),
                    foodScore = 75f + _random.Next(0, 20),
                    entertainmentScore = 70f + _random.Next(0, 25),
                    decorationScore = 75f + _random.Next(0, 20),
                    serviceScore = 80f + _random.Next(0, 15),
                    expectationScore = 78f + _random.Next(0, 17)
                };

                var client = new ClientData { personality = inquiry.personality };
                var satisfactionResult = _satisfactionCalculator.Calculate(eventData, client);
                
                var reputationChange = _progressionSystem.ApplyEventResult(
                    satisfactionResult.FinalSatisfaction, reputation, excellenceStreak);
                
                reputation = reputationChange.NewReputation;
                
                // Update excellence streak
                if (satisfactionResult.FinalSatisfaction >= 90f)
                    excellenceStreak++;
                else if (satisfactionResult.FinalSatisfaction < 80f)
                    excellenceStreak = 0;

                currentDate = currentDate.AddDays(7);
            }

            // After 10 successful events, reputation should have increased
            Assert.Greater(reputation, 0);
        }

        /// <summary>
        /// Test workload management across multiple concurrent events.
        /// Requirements: R5.9-R5.18
        /// </summary>
        [Test]
        public void WorkloadManagement_MultipleConcurrentEvents_AffectsSatisfaction()
        {
            int stage = 2; // Stage 2 has full workload system

            // Test workload status at different event counts
            var optimalStatus = _eventPlanningSystem.GetWorkloadStatus(stage, 4);
            Assert.AreEqual(WorkloadStatus.Optimal, optimalStatus);

            var comfortableStatus = _eventPlanningSystem.GetWorkloadStatus(stage, 5);
            Assert.AreEqual(WorkloadStatus.Comfortable, comfortableStatus);

            var strainedStatus = _eventPlanningSystem.GetWorkloadStatus(stage, 7);
            Assert.AreEqual(WorkloadStatus.Strained, strainedStatus);

            var criticalStatus = _eventPlanningSystem.GetWorkloadStatus(stage, 9);
            Assert.AreEqual(WorkloadStatus.Critical, criticalStatus);

            // Verify penalties increase with workload
            var optimalPenalty = _eventPlanningSystem.CalculateWorkloadPenalty(stage, 4);
            var strainedPenalty = _eventPlanningSystem.CalculateWorkloadPenalty(stage, 7);
            var criticalPenalty = _eventPlanningSystem.CalculateWorkloadPenalty(stage, 9);

            Assert.AreEqual(0f, optimalPenalty);
            Assert.Greater(strainedPenalty, optimalPenalty);
            Assert.Greater(criticalPenalty, strainedPenalty);
        }

        #endregion

        #region Budget and Profit Integration Tests

        /// <summary>
        /// Test budget allocation affects satisfaction and profit.
        /// Requirements: R7, R13, R33
        /// </summary>
        [Test]
        public void BudgetAllocation_AffectsSatisfactionAndProfit()
        {
            var currentDate = new GameDate(1, 1, 1);
            
            // Create event with specific budget
            var inquiry = _eventPlanningSystem.GenerateInquiry(1, 50, currentDate);
            var eventData = _eventPlanningSystem.AcceptInquiry(inquiry, currentDate);
            
            float totalBudget = eventData.budget.total;

            // Book venue and vendor within budget
            var venue = CreateTestVenue(50, 30, 100);
            venue.basePrice = totalBudget * 0.3f; // 30% of budget
            _eventPlanningSystem.BookVenue(eventData, venue, null);

            var caterer = CreateTestVendor(VendorCategory.Caterer, totalBudget * 0.25f);
            _eventPlanningSystem.BookVendor(eventData, caterer, null);

            // Verify budget tracking
            Assert.Greater(eventData.budget.spent, 0);
            Assert.LessOrEqual(eventData.budget.spent, totalBudget);

            // Complete event with good scores
            eventData.status = EventStatus.Completed;
            eventData.results = new EventResults
            {
                venueScore = 85f,
                foodScore = 80f,
                entertainmentScore = 75f,
                decorationScore = 70f,
                serviceScore = 80f,
                expectationScore = 78f
            };

            var client = new ClientData { personality = inquiry.personality };
            var satisfactionResult = _satisfactionCalculator.Calculate(eventData, client);

            // Calculate profit based on satisfaction
            var profitResult = _profitCalculator.CalculateProfit(totalBudget, satisfactionResult.FinalSatisfaction);

            // Verify profit calculation returns valid result
            Assert.IsNotNull(profitResult);
            Assert.GreaterOrEqual(profitResult.ProfitMarginPercent, 0f);
        }

        /// <summary>
        /// Test overage handling based on client personality.
        /// Requirements: R7.9-R7.14
        /// </summary>
        [Test]
        public void OverageHandling_VariesByPersonality()
        {
            // Test overage tolerance for different personalities
            var easyGoingTolerance = _satisfactionCalculator.GetOverageTolerance(ClientPersonality.EasyGoing);
            var budgetConsciousTolerance = _satisfactionCalculator.GetOverageTolerance(ClientPersonality.BudgetConscious);
            var perfectionistTolerance = _satisfactionCalculator.GetOverageTolerance(ClientPersonality.Perfectionist);

            Assert.AreEqual(15f, easyGoingTolerance);
            Assert.AreEqual(0f, budgetConsciousTolerance);
            Assert.AreEqual(5f, perfectionistTolerance);

            // Test overage within tolerance
            Assert.IsTrue(_satisfactionCalculator.IsOverageWithinTolerance(10f, ClientPersonality.EasyGoing));
            Assert.IsFalse(_satisfactionCalculator.IsOverageWithinTolerance(10f, ClientPersonality.BudgetConscious));
            Assert.IsFalse(_satisfactionCalculator.IsOverageWithinTolerance(10f, ClientPersonality.Perfectionist));
        }

        #endregion

        #region Personality and Satisfaction Integration Tests

        /// <summary>
        /// Test that different personalities have different satisfaction thresholds.
        /// Requirements: R15.2-R15.6
        /// </summary>
        [Test]
        public void PersonalityThresholds_AffectSatisfactionOutcome()
        {
            var currentDate = new GameDate(1, 1, 1);

            // Create event with moderate scores
            var inquiry = _eventPlanningSystem.GenerateInquiry(1, 50, currentDate);
            var eventData = _eventPlanningSystem.AcceptInquiry(inquiry, currentDate);
            
            eventData.status = EventStatus.Completed;
            eventData.results = new EventResults
            {
                venueScore = 70f,
                foodScore = 68f,
                entertainmentScore = 65f,
                decorationScore = 70f,
                serviceScore = 72f,
                expectationScore = 69f
            };

            // Test with different personalities
            var easyGoingClient = new ClientData { personality = ClientPersonality.EasyGoing };
            var perfectionistClient = new ClientData { personality = ClientPersonality.Perfectionist };

            var easyGoingResult = _satisfactionCalculator.Calculate(eventData, easyGoingClient);
            var perfectionistResult = _satisfactionCalculator.Calculate(eventData, perfectionistClient);

            // Get thresholds
            var easyGoingThreshold = _satisfactionCalculator.GetPersonalityThreshold(ClientPersonality.EasyGoing);
            var perfectionistThreshold = _satisfactionCalculator.GetPersonalityThreshold(ClientPersonality.Perfectionist);

            // EasyGoing has lower threshold (60), Perfectionist has higher (85)
            Assert.Less(easyGoingThreshold, perfectionistThreshold);

            // Same scores may meet EasyGoing threshold but not Perfectionist
            if (easyGoingResult.FinalSatisfaction >= 60f && easyGoingResult.FinalSatisfaction < 85f)
            {
                Assert.IsTrue(easyGoingResult.MeetsClientThreshold);
                Assert.IsFalse(perfectionistResult.MeetsClientThreshold);
            }
        }

        #endregion

        #region Event Type Variety Tests

        /// <summary>
        /// Test that different stages generate appropriate event types.
        /// Requirements: R6.1-R6.17
        /// </summary>
        [Test]
        public void EventTypeGeneration_VariesByStage()
        {
            var currentDate = new GameDate(1, 1, 1);
            var stage1EventTypes = new HashSet<string>();
            var stage3EventTypes = new HashSet<string>();

            // Generate multiple inquiries for Stage 1
            for (int i = 0; i < 50; i++)
            {
                var inquiry = _eventPlanningSystem.GenerateInquiry(1, 0, currentDate);
                stage1EventTypes.Add(inquiry.eventTypeId);
            }

            // Generate multiple inquiries for Stage 3
            var stage3System = new EventPlanningSystemImpl(123);
            for (int i = 0; i < 50; i++)
            {
                var inquiry = stage3System.GenerateInquiry(3, 100, currentDate);
                stage3EventTypes.Add(inquiry.eventTypeId);
            }

            // Stage 1 should have limited event types
            Assert.Greater(stage1EventTypes.Count, 0);
            
            // Stage 3 should have more variety
            Assert.GreaterOrEqual(stage3EventTypes.Count, stage1EventTypes.Count);
        }

        #endregion

        #region Inquiry to Event Transition Tests

        /// <summary>
        /// Test that inquiry data is correctly transferred to event data.
        /// Requirements: R5.3
        /// </summary>
        [Test]
        public void InquiryToEvent_DataTransferredCorrectly()
        {
            var currentDate = new GameDate(1, 1, 1);

            for (int i = 0; i < 20; i++)
            {
                var inquiry = _eventPlanningSystem.GenerateInquiry(1, 50, currentDate);
                var eventData = _eventPlanningSystem.AcceptInquiry(inquiry, currentDate);

                // Verify all data transferred correctly
                Assert.AreEqual(inquiry.clientName, eventData.clientName);
                Assert.AreEqual(inquiry.budget, eventData.budget.total);
                Assert.AreEqual(inquiry.eventDate, eventData.eventDate);
                Assert.AreEqual(inquiry.personality, eventData.personality);
                Assert.AreEqual(inquiry.guestCount, eventData.guestCount);
                Assert.AreEqual(inquiry.eventTypeId, eventData.eventTypeId);
                Assert.AreEqual(inquiry.subCategory, eventData.subCategory);
                Assert.AreEqual(inquiry.isReferral, eventData.isReferral);
                Assert.AreEqual(currentDate, eventData.acceptedDate);
                Assert.AreEqual(EventStatus.Accepted, eventData.status);
            }
        }

        /// <summary>
        /// Test that declining an inquiry has no side effects.
        /// Requirements: R5.4
        /// </summary>
        [Test]
        public void DeclineInquiry_NoSideEffects()
        {
            var currentDate = new GameDate(1, 1, 1);
            int reputation = 50;

            var inquiry = _eventPlanningSystem.GenerateInquiry(1, reputation, currentDate);
            
            // Decline should not throw and have no side effects
            Assert.DoesNotThrow(() => _eventPlanningSystem.DeclineInquiry(inquiry));
        }

        #endregion

        #region Helper Methods

        private VenueData CreateTestVenue(int comfortable, int min, int max)
        {
            var venue = UnityEngine.ScriptableObject.CreateInstance<VenueData>();
            venue.id = Guid.NewGuid().ToString();
            venue.venueName = "Test Venue";
            venue.capacityComfortable = comfortable;
            venue.capacityMin = min;
            venue.capacityMax = max;
            venue.basePrice = 500f;
            venue.pricePerGuest = 10f;
            return venue;
        }

        private VendorData CreateTestVendor(VendorCategory category, float price)
        {
            var vendor = UnityEngine.ScriptableObject.CreateInstance<VendorData>();
            vendor.id = Guid.NewGuid().ToString();
            vendor.vendorName = "Test Vendor";
            vendor.category = category;
            vendor.basePrice = price;
            return vendor;
        }

        #endregion
    }
}
