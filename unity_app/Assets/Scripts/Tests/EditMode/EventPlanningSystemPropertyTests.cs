using System;
using System.Collections.Generic;
using NUnit.Framework;
using EventPlannerSim.Core;
using EventPlannerSim.Data;
using EventPlannerSim.Systems;

namespace EventPlannerSim.Tests.EditMode
{
    /// <summary>
    /// Property-based tests for EventPlanningSystem.
    /// Feature: event-planner-simulator
    /// </summary>
    [TestFixture]
    public class EventPlanningSystemPropertyTests
    {
        private EventPlanningSystemImpl _system;
        private Random _random;

        [SetUp]
        public void Setup()
        {
            _random = new Random(42); // Fixed seed for reproducibility
            _system = new EventPlanningSystemImpl(42);
        }

        #region Property 3: Client Inquiry Completeness

        /// <summary>
        /// Property 3: Client Inquiry Completeness
        /// For any generated ClientInquiry, all required fields must be populated.
        /// **Validates: Requirements R5**
        /// </summary>
        [Test]
        public void GenerateInquiry_AllFieldsPopulated_ForAllStagesAndReputations()
        {
            // Test across all stages and various reputation levels
            for (int stage = 1; stage <= 5; stage++)
            {
                for (int rep = 0; rep <= 200; rep += 25)
                {
                    var currentDate = new GameDate(1, 1, 1);
                    
                    // Generate multiple inquiries per stage/rep combination
                    for (int i = 0; i < 20; i++)
                    {
                        var inquiry = _system.GenerateInquiry(stage, rep, currentDate);

                        // Verify non-empty clientName
                        Assert.IsFalse(string.IsNullOrEmpty(inquiry.clientName),
                            $"clientName should not be empty. Stage: {stage}, Rep: {rep}");

                        // Verify valid eventTypeId
                        Assert.IsFalse(string.IsNullOrEmpty(inquiry.eventTypeId),
                            $"eventTypeId should not be empty. Stage: {stage}, Rep: {rep}");

                        // Verify valid subCategory
                        Assert.IsFalse(string.IsNullOrEmpty(inquiry.subCategory),
                            $"subCategory should not be empty. Stage: {stage}, Rep: {rep}");

                        // Verify eventDisplayName format
                        Assert.IsFalse(string.IsNullOrEmpty(inquiry.eventDisplayName),
                            $"eventDisplayName should not be empty. Stage: {stage}, Rep: {rep}");
                        Assert.IsTrue(inquiry.eventDisplayName.Contains(inquiry.clientName),
                            $"eventDisplayName should contain clientName. Stage: {stage}, Rep: {rep}");
                        Assert.IsTrue(inquiry.eventDisplayName.Contains(inquiry.subCategory),
                            $"eventDisplayName should contain subCategory. Stage: {stage}, Rep: {rep}");

                        // Verify budget is positive and within reasonable range
                        Assert.Greater(inquiry.budget, 0,
                            $"budget should be positive. Stage: {stage}, Rep: {rep}");
                        Assert.LessOrEqual(inquiry.budget, 200000,
                            $"budget should be within reasonable range. Stage: {stage}, Rep: {rep}");

                        // Verify guestCount is positive
                        Assert.Greater(inquiry.guestCount, 0,
                            $"guestCount should be positive. Stage: {stage}, Rep: {rep}");

                        // Verify eventDate is in the future
                        Assert.IsTrue(inquiry.eventDate.IsAfter(currentDate),
                            $"eventDate should be in the future. Stage: {stage}, Rep: {rep}");

                        // Verify personality is valid for stage
                        Assert.IsTrue(IsPersonalityValidForStage(inquiry.personality, stage),
                            $"personality {inquiry.personality} should be valid for stage {stage}");

                        // Verify inquiryId is set
                        Assert.IsFalse(string.IsNullOrEmpty(inquiry.inquiryId),
                            $"inquiryId should not be empty. Stage: {stage}, Rep: {rep}");
                    }
                }
            }
        }

        /// <summary>
        /// Property 3: Client Inquiry Completeness - Budget within event type range
        /// **Validates: Requirements R5**
        /// </summary>
        [Test]
        public void GenerateInquiry_BudgetWithinEventTypeRange()
        {
            var currentDate = new GameDate(1, 1, 1);

            for (int i = 0; i < 100; i++)
            {
                int stage = _random.Next(1, 6);
                int rep = _random.Next(0, 201);

                var inquiry = _system.GenerateInquiry(stage, rep, currentDate);
                var budgetRange = GetExpectedBudgetRange(inquiry.eventTypeId);
                
                Assert.GreaterOrEqual(inquiry.budget, budgetRange.min,
                    $"Budget {inquiry.budget} should be >= {budgetRange.min} for {inquiry.eventTypeId}");
                Assert.LessOrEqual(inquiry.budget, budgetRange.max,
                    $"Budget {inquiry.budget} should be <= {budgetRange.max} for {inquiry.eventTypeId}");
            }
        }

        /// <summary>
        /// Property 3: Client Inquiry Completeness - Personality distribution by stage
        /// **Validates: Requirements R5, R14.13**
        /// </summary>
        [Test]
        public void GenerateInquiry_PersonalityDistributionByStage()
        {
            var currentDate = new GameDate(1, 1, 1);
            var stage1Counts = new Dictionary<ClientPersonality, int>
            {
                { ClientPersonality.EasyGoing, 0 },
                { ClientPersonality.BudgetConscious, 0 },
                { ClientPersonality.Perfectionist, 0 }
            };

            var testSystem = new EventPlanningSystemImpl(123);
            
            for (int i = 0; i < 1000; i++)
            {
                var inquiry = testSystem.GenerateInquiry(1, 0, currentDate);
                if (stage1Counts.ContainsKey(inquiry.personality))
                    stage1Counts[inquiry.personality]++;
            }

            Assert.AreEqual(1000, 
                stage1Counts[ClientPersonality.EasyGoing] + 
                stage1Counts[ClientPersonality.BudgetConscious] + 
                stage1Counts[ClientPersonality.Perfectionist],
                "Stage 1 should only have EasyGoing, BudgetConscious, or Perfectionist personalities");
        }

        #endregion

        #region Property 4: Event Creation from Inquiry

        /// <summary>
        /// Property 4: Event Creation from Inquiry
        /// **Validates: Requirements R5**
        /// </summary>
        [Test]
        public void AcceptInquiry_AllFieldsTransferCorrectly()
        {
            var currentDate = new GameDate(1, 1, 1);

            for (int i = 0; i < 100; i++)
            {
                int stage = _random.Next(1, 6);
                int rep = _random.Next(0, 201);

                var inquiry = _system.GenerateInquiry(stage, rep, currentDate);
                var eventData = _system.AcceptInquiry(inquiry, currentDate);

                string expectedTitle = $"{inquiry.clientName}'s {inquiry.subCategory}";
                Assert.AreEqual(expectedTitle, eventData.eventTitle);
                Assert.AreEqual(EventStatus.Accepted, eventData.status);
                Assert.AreEqual(inquiry.budget, eventData.budget.total);
                Assert.AreEqual(inquiry.eventDate, eventData.eventDate);
                Assert.AreEqual(inquiry.personality, eventData.personality);
                Assert.AreEqual(inquiry.guestCount, eventData.guestCount);
                Assert.AreEqual(inquiry.clientName, eventData.clientName);
                Assert.AreEqual(currentDate, eventData.acceptedDate);
                Assert.AreEqual(inquiry.isReferral, eventData.isReferral);
            }
        }

        /// <summary>
        /// Property 4: Event Creation from Inquiry - Referral info preserved
        /// **Validates: Requirements R5**
        /// </summary>
        [Test]
        public void AcceptInquiry_ReferralInfoPreserved()
        {
            var currentDate = new GameDate(1, 1, 1);
            var eventDate = currentDate.AddDays(7);

            var referralInquiry = ClientInquiry.CreateReferral(
                "TestClient", "KidsBirthday", "Princess Theme Birthday",
                ClientPersonality.EasyGoing, 1500, 25, eventDate, "ReferringClient");

            var eventData = _system.AcceptInquiry(referralInquiry, currentDate);

            Assert.IsTrue(eventData.isReferral);
            Assert.AreEqual("ReferringClient", eventData.referredByClientName);
        }

        #endregion

        #region Property 6: Event Title Format

        /// <summary>
        /// Property 6: Event Title Format
        /// **Validates: Requirements R6.18**
        /// </summary>
        [Test]
        public void GenerateEventTitle_CorrectFormat()
        {
            var testCases = new[]
            {
                ("Emma", "Princess Theme Birthday", "Emma's Princess Theme Birthday"),
                ("John", "Board Meeting", "John's Board Meeting"),
                ("Sarah", "Garden Party", "Sarah's Garden Party")
            };

            foreach (var (clientName, subCategory, expected) in testCases)
            {
                var result = _system.GenerateEventTitle(clientName, subCategory);
                Assert.AreEqual(expected, result);
            }
        }

        /// <summary>
        /// Property 6: Event Title Format - Handles edge cases
        /// **Validates: Requirements R6.18**
        /// </summary>
        [Test]
        public void GenerateEventTitle_HandlesEdgeCases()
        {
            Assert.AreEqual("Client's Birthday Party", _system.GenerateEventTitle("", "Birthday Party"));
            Assert.AreEqual("Emma's Event", _system.GenerateEventTitle("Emma", ""));
            Assert.AreEqual("Client's Event", _system.GenerateEventTitle("", ""));
            Assert.AreEqual("Client's Event", _system.GenerateEventTitle(null, null));
        }

        #endregion


        #region Property 5: Workload Capacity and Penalties

        /// <summary>
        /// Property 5: Workload Capacity and Penalties - Stage 1 simplified
        /// **Validates: Requirements R5.18a**
        /// </summary>
        [Test]
        public void GetWorkloadStatus_Stage1_SimplifiedSystem()
        {
            for (int eventCount = 0; eventCount <= 10; eventCount++)
            {
                var status = _system.GetWorkloadStatus(1, eventCount);
                
                if (eventCount <= 3)
                    Assert.AreEqual(WorkloadStatus.Optimal, status,
                        $"Stage 1 with {eventCount} events should be Optimal");
                else
                    Assert.AreEqual(WorkloadStatus.Comfortable, status,
                        $"Stage 1 with {eventCount} events should be Comfortable (warning)");
            }
        }

        /// <summary>
        /// Property 5: Workload Capacity and Penalties - Stage 1 no penalties
        /// **Validates: Requirements R5.18a**
        /// </summary>
        [Test]
        public void CalculateWorkloadPenalty_Stage1_AlwaysZero()
        {
            for (int eventCount = 0; eventCount <= 20; eventCount++)
            {
                float penalty = _system.CalculateWorkloadPenalty(1, eventCount);
                Assert.AreEqual(0f, penalty,
                    $"Stage 1 should have no workload penalty regardless of event count ({eventCount})");
            }
        }

        /// <summary>
        /// Property 5: Workload Capacity and Penalties - Stage 2+ tier system
        /// **Validates: Requirements R5.9-R5.15**
        /// </summary>
        [Test]
        public void GetWorkloadStatus_Stage2Plus_FullTierSystem()
        {
            var expectedThresholds = new[]
            {
                (stage: 2, optimal: 4, comfortable: 6, strained: 8),
                (stage: 3, optimal: 6, comfortable: 9, strained: 12),
                (stage: 4, optimal: 10, comfortable: 14, strained: 18),
                (stage: 5, optimal: 15, comfortable: 20, strained: 25)
            };

            foreach (var (stage, optimal, comfortable, strained) in expectedThresholds)
            {
                Assert.AreEqual(WorkloadStatus.Optimal, _system.GetWorkloadStatus(stage, optimal));
                Assert.AreEqual(WorkloadStatus.Comfortable, _system.GetWorkloadStatus(stage, optimal + 1));
                Assert.AreEqual(WorkloadStatus.Comfortable, _system.GetWorkloadStatus(stage, comfortable));
                Assert.AreEqual(WorkloadStatus.Strained, _system.GetWorkloadStatus(stage, comfortable + 1));
                Assert.AreEqual(WorkloadStatus.Strained, _system.GetWorkloadStatus(stage, strained));
                Assert.AreEqual(WorkloadStatus.Critical, _system.GetWorkloadStatus(stage, strained + 1));
            }
        }

        /// <summary>
        /// Property 5: Workload Capacity and Penalties - Penalty calculation
        /// **Validates: Requirements R5.12-R5.15**
        /// </summary>
        [Test]
        public void CalculateWorkloadPenalty_CorrectPenalties()
        {
            // Stage 2: optimal=4, comfortable=6, strained=8
            Assert.AreEqual(0f, _system.CalculateWorkloadPenalty(2, 4));
            Assert.AreEqual(3f, _system.CalculateWorkloadPenalty(2, 5), 0.01f);
            Assert.AreEqual(6f, _system.CalculateWorkloadPenalty(2, 6), 0.01f);
            Assert.AreEqual(13f, _system.CalculateWorkloadPenalty(2, 7), 0.01f);
            Assert.AreEqual(20f, _system.CalculateWorkloadPenalty(2, 8), 0.01f);
            Assert.AreEqual(32f, _system.CalculateWorkloadPenalty(2, 9), 0.01f);
            Assert.AreEqual(44f, _system.CalculateWorkloadPenalty(2, 10), 0.01f);
        }

        /// <summary>
        /// Property 5: Workload Capacity and Penalties - Task failure probability
        /// **Validates: Requirements R5.14-R5.15**
        /// </summary>
        [Test]
        public void CalculateTaskFailureProbabilityIncrease_CorrectValues()
        {
            // Stage 1: always 0%
            for (int i = 0; i <= 10; i++)
                Assert.AreEqual(0f, _system.CalculateTaskFailureProbabilityIncrease(1, i));
            
            // Stage 2: optimal=4, comfortable=6, strained=8
            Assert.AreEqual(0f, _system.CalculateTaskFailureProbabilityIncrease(2, 6));
            Assert.AreEqual(10f, _system.CalculateTaskFailureProbabilityIncrease(2, 7));
            Assert.AreEqual(10f, _system.CalculateTaskFailureProbabilityIncrease(2, 8));
            Assert.AreEqual(25f, _system.CalculateTaskFailureProbabilityIncrease(2, 9));
        }

        #endregion

        #region Property 25: Overlapping Event Preparation Penalty

        /// <summary>
        /// Property 25: Overlapping Event Preparation Penalty
        /// **Validates: Requirements R5.17**
        /// </summary>
        [Test]
        public void CalculateOverlappingPrepPenalty_CorrectPenalties()
        {
            Assert.AreEqual(0f, _system.CalculateOverlappingPrepPenalty(0));
            Assert.AreEqual(0f, _system.CalculateOverlappingPrepPenalty(1));
            Assert.AreEqual(5f, _system.CalculateOverlappingPrepPenalty(2));
            Assert.AreEqual(10f, _system.CalculateOverlappingPrepPenalty(3));
            Assert.AreEqual(15f, _system.CalculateOverlappingPrepPenalty(4));
            Assert.AreEqual(20f, _system.CalculateOverlappingPrepPenalty(5));
        }

        /// <summary>
        /// Property 25: Overlapping Event Preparation Penalty - Formula verification
        /// **Validates: Requirements R5.17**
        /// </summary>
        [Test]
        public void CalculateOverlappingPrepPenalty_FormulaVerification()
        {
            for (int i = 0; i < 100; i++)
            {
                int overlappingCount = _random.Next(0, 20);
                float expected = overlappingCount <= 1 ? 0f : (overlappingCount - 1) * 5f;
                float actual = _system.CalculateOverlappingPrepPenalty(overlappingCount);
                
                Assert.AreEqual(expected, actual, 0.01f,
                    $"Overlapping penalty for {overlappingCount} events should be {expected}%");
            }
        }

        #endregion


        #region Property 9: Vendor Booking Budget Deduction

        /// <summary>
        /// Property 9: Vendor Booking Budget Deduction
        /// **Validates: Requirements R8**
        /// </summary>
        [Test]
        public void BookVendor_DeductsBudgetCorrectly()
        {
            var eventData = CreateTestEvent(5000);
            var vendor = CreateTestVendor(VendorCategory.Caterer, 1000f);

            float initialSpent = eventData.budget.spent;
            var result = _system.BookVendor(eventData, vendor, null);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(1000f, result.AmountDeducted);
            Assert.AreEqual(initialSpent + 1000f, eventData.budget.spent);
        }

        /// <summary>
        /// Property 9: Vendor Booking Budget Deduction - Warns on overspend
        /// **Validates: Requirements R8**
        /// </summary>
        [Test]
        public void BookVendor_WarnsOnOverspend()
        {
            var eventData = CreateTestEvent(1000);
            eventData.budget.spent = 800f; // Only 200 remaining
            var vendor = CreateTestVendor(VendorCategory.Caterer, 500f);

            var result = _system.BookVendor(eventData, vendor, null);

            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.ShowWarning);
            Assert.IsTrue(result.WarningMessage.Contains("exceeds"));
        }

        /// <summary>
        /// Property 9: Vendor Booking Budget Deduction - Fails on unavailable
        /// **Validates: Requirements R8.5**
        /// </summary>
        [Test]
        public void BookVendor_FailsWhenUnavailable()
        {
            var eventData = CreateTestEvent(5000);
            var vendor = CreateTestVendor(VendorCategory.Caterer, 1000f);
            var bookedDates = new List<GameDate> { eventData.eventDate };

            var result = _system.BookVendor(eventData, vendor, bookedDates);

            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.Message.Contains("not available"));
        }

        #endregion

        #region Property 10: Venue Capacity Validation

        /// <summary>
        /// Property 10: Venue Capacity Validation
        /// **Validates: Requirements R9**
        /// </summary>
        [Test]
        public void BookVenue_ValidatesCapacity()
        {
            var eventData = CreateTestEvent(5000);
            eventData.guestCount = 50;
            var venue = CreateTestVenue(30, 40, 60); // comfortable=30, max=60

            var result = _system.BookVenue(eventData, venue, null);

            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.ShowWarning); // Guest count exceeds comfortable
        }

        /// <summary>
        /// Property 10: Venue Capacity Validation - Fails when exceeds max
        /// **Validates: Requirements R9.3**
        /// </summary>
        [Test]
        public void BookVenue_FailsWhenExceedsMaxCapacity()
        {
            var eventData = CreateTestEvent(5000);
            eventData.guestCount = 100;
            var venue = CreateTestVenue(30, 40, 60); // max=60

            var result = _system.BookVenue(eventData, venue, null);

            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.Message.Contains("cannot accommodate"));
        }

        /// <summary>
        /// Property 10: Venue Capacity Validation - No warning when under comfortable
        /// **Validates: Requirements R9.4**
        /// </summary>
        [Test]
        public void BookVenue_NoWarningWhenUnderComfortableCapacity()
        {
            var eventData = CreateTestEvent(5000);
            eventData.guestCount = 25;
            var venue = CreateTestVenue(30, 40, 60); // comfortable=30

            var result = _system.BookVenue(eventData, venue, null);

            Assert.IsTrue(result.Success);
            Assert.IsFalse(result.ShowWarning);
        }

        /// <summary>
        /// Property 10: Venue Capacity Validation - Fails when unavailable
        /// **Validates: Requirements R9.6**
        /// </summary>
        [Test]
        public void BookVenue_FailsWhenUnavailable()
        {
            var eventData = CreateTestEvent(5000);
            eventData.guestCount = 25;
            var venue = CreateTestVenue(30, 40, 60);
            var bookedDates = new List<GameDate> { eventData.eventDate };

            var result = _system.BookVenue(eventData, venue, bookedDates);

            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.Message.Contains("not available"));
        }

        #endregion

        #region Helper Methods

        private bool IsPersonalityValidForStage(ClientPersonality personality, int stage)
        {
            if (stage <= 2)
            {
                return personality == ClientPersonality.EasyGoing ||
                       personality == ClientPersonality.BudgetConscious ||
                       personality == ClientPersonality.Perfectionist;
            }
            return true;
        }

        private (int min, int max) GetExpectedBudgetRange(string eventTypeId)
        {
            return eventTypeId switch
            {
                "KidsBirthday" => (500, 2000),
                "FamilyGathering" => (300, 1500),
                "SchoolEvent" => (1000, 3000),
                "AdultBirthday" => (1000, 5000),
                "EngagementParty" => (2000, 8000),
                "CorporateMeeting" => (3000, 15000),
                "MilestoneBirthday" => (2000, 6000),
                "BabyShower" => (1000, 4000),
                _ => (300, 15000)
            };
        }

        private EventData CreateTestEvent(float budget)
        {
            return new EventData
            {
                id = Guid.NewGuid().ToString(),
                clientName = "TestClient",
                eventTitle = "TestClient's Test Event",
                eventDate = new GameDate(15, 1, 1),
                acceptedDate = new GameDate(1, 1, 1),
                guestCount = 50,
                budget = new EventBudget { total = budget }
            };
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

        #endregion
    }
}
