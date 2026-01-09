using System;
using System.Collections.Generic;
using EventPlannerSim.Core;
using EventPlannerSim.Data;

namespace EventPlannerSim.Systems
{
    /// <summary>
    /// Implementation of the Event Planning System.
    /// Manages event lifecycle from inquiry to results.
    /// Requirements: R5.1-R5.8, R6.18, R8.1-R8.7, R9.1-R9.7
    /// </summary>
    public class EventPlanningSystemImpl : IEventPlanningSystem
    {
        // Workload capacity thresholds by stage (index 0 = Stage 1, etc.)
        // Requirements: R5.9-R5.11
        private static readonly int[] OptimalCapacity = { 2, 4, 6, 10, 15 };
        private static readonly int[] ComfortableCapacity = { 3, 6, 9, 14, 20 };
        private static readonly int[] StrainedCapacity = { 5, 8, 12, 18, 25 };

        // Inquiry interval ranges in minutes by stage (index 0 = Stage 1, etc.)
        // Requirements: R5.6
        private static readonly (float min, float max)[] InquiryIntervals = 
        {
            (8f, 12f),   // Stage 1
            (6f, 10f),   // Stage 2
            (5f, 8f),    // Stage 3
            (4f, 7f),    // Stage 4
            (3f, 6f)     // Stage 5
        };

        // Stage minimum reputation thresholds for inquiry interval calculation
        // Requirements: R5.7
        private static readonly int[] StageMinReputation = { 0, 25, 50, 100, 200 };

        // Client names pool for inquiry generation
        private static readonly string[] ClientNames = 
        {
            "Emma", "Liam", "Olivia", "Noah", "Ava", "Ethan", "Sophia", "Mason",
            "Isabella", "William", "Mia", "James", "Charlotte", "Benjamin", "Amelia",
            "Lucas", "Harper", "Henry", "Evelyn", "Alexander", "Abigail", "Michael",
            "Emily", "Daniel", "Elizabeth", "Jacob", "Sofia", "Logan", "Avery", "Jackson"
        };

        private readonly Random _random;

        /// <summary>
        /// Creates a new EventPlanningSystemImpl with optional random seed for testing.
        /// </summary>
        public EventPlanningSystemImpl(int? seed = null)
        {
            _random = seed.HasValue ? new Random(seed.Value) : new Random();
        }

        /// <inheritdoc/>
        public ClientInquiry GenerateInquiry(int stage, int reputation, GameDate currentDate)
        {
            // Validate stage
            stage = Math.Clamp(stage, 1, 5);

            // Select a random client name
            string clientName = ClientNames[_random.Next(ClientNames.Length)];

            // Generate personality based on stage distribution (R14.13)
            ClientPersonality personality = GeneratePersonalityForStage(stage);

            // Generate event type and sub-category based on stage
            // For now, use placeholder values - in full implementation, this would use EventTypeData
            string eventTypeId = GetRandomEventTypeForStage(stage);
            string subCategory = GetRandomSubCategoryForEventType(eventTypeId);

            // Generate budget within event type range
            var budgetRange = GetBudgetRangeForEventType(eventTypeId);
            int budget = _random.Next(budgetRange.min, budgetRange.max + 1);

            // Generate guest count based on event type
            int guestCount = GenerateGuestCount(eventTypeId);

            // Schedule event date based on complexity (R11.3)
            var complexity = GetComplexityForEventType(eventTypeId);
            var schedulingRange = GetSchedulingRange(complexity);
            int daysUntilEvent = _random.Next(schedulingRange.min, schedulingRange.max + 1);
            GameDate eventDate = currentDate.AddDays(daysUntilEvent);

            // Create the inquiry
            return ClientInquiry.Create(
                clientName,
                eventTypeId,
                subCategory,
                personality,
                budget,
                guestCount,
                eventDate
            );
        }

        /// <inheritdoc/>
        public EventData AcceptInquiry(ClientInquiry inquiry, GameDate currentDate)
        {
            if (inquiry == null)
                throw new ArgumentNullException(nameof(inquiry));

            return new EventData
            {
                id = Guid.NewGuid().ToString(),
                clientId = inquiry.inquiryId,
                clientName = inquiry.clientName,
                eventTitle = GenerateEventTitle(inquiry.clientName, inquiry.subCategory),
                eventTypeId = inquiry.eventTypeId,
                subCategory = inquiry.subCategory,
                status = EventStatus.Accepted,
                phase = EventPhase.Booking,
                eventDate = inquiry.eventDate,
                acceptedDate = currentDate,
                personality = inquiry.personality,
                guestCount = inquiry.guestCount,
                budget = new EventBudget { total = inquiry.budget },
                isReferral = inquiry.isReferral,
                referredByClientName = inquiry.referredByClientName
            };
        }

        /// <inheritdoc/>
        public void DeclineInquiry(ClientInquiry inquiry)
        {
            // No penalty for declining - just remove from queue
            // The actual removal is handled by the caller
        }

        /// <inheritdoc/>
        public WorkloadStatus GetWorkloadStatus(int stage, int activeEventCount)
        {
            stage = Math.Clamp(stage, 1, 5);
            int stageIndex = stage - 1;

            // Stage 1 simplified: soft cap at 3 with warning (R5.18a)
            if (stage == 1)
            {
                return activeEventCount <= 3 ? WorkloadStatus.Optimal : WorkloadStatus.Comfortable;
            }

            // Stage 2+ full tier system (R5.9-R5.15)
            if (activeEventCount <= OptimalCapacity[stageIndex])
                return WorkloadStatus.Optimal;
            if (activeEventCount <= ComfortableCapacity[stageIndex])
                return WorkloadStatus.Comfortable;
            if (activeEventCount <= StrainedCapacity[stageIndex])
                return WorkloadStatus.Strained;
            
            return WorkloadStatus.Critical;
        }

        /// <inheritdoc/>
        public float CalculateWorkloadPenalty(int stage, int activeEventCount)
        {
            stage = Math.Clamp(stage, 1, 5);
            int stageIndex = stage - 1;

            // Stage 1 simplified: no percentage penalties (R5.18a)
            if (stage == 1)
                return 0f;

            // Stage 2+ full penalty system
            int optimal = OptimalCapacity[stageIndex];
            int comfortable = ComfortableCapacity[stageIndex];
            int strained = StrainedCapacity[stageIndex];

            // At or below optimal: no penalty (R5.12)
            if (activeEventCount <= optimal)
                return 0f;

            float penalty = 0f;

            // Between optimal and comfortable: 3% per event over optimal (R5.13)
            if (activeEventCount <= comfortable)
            {
                int eventsOverOptimal = activeEventCount - optimal;
                penalty = eventsOverOptimal * 3f;
            }
            // Between comfortable and strained: 7% per event over comfortable (R5.14)
            else if (activeEventCount <= strained)
            {
                int eventsOverOptimal = comfortable - optimal;
                int eventsOverComfortable = activeEventCount - comfortable;
                penalty = (eventsOverOptimal * 3f) + (eventsOverComfortable * 7f);
            }
            // Beyond strained: 12% per event over strained (R5.15)
            else
            {
                int eventsOverOptimal = comfortable - optimal;
                int eventsOverComfortable = strained - comfortable;
                int eventsOverStrained = activeEventCount - strained;
                penalty = (eventsOverOptimal * 3f) + (eventsOverComfortable * 7f) + (eventsOverStrained * 12f);
            }

            return penalty;
        }

        /// <inheritdoc/>
        public float CalculateTaskFailureProbabilityIncrease(int stage, int activeEventCount)
        {
            stage = Math.Clamp(stage, 1, 5);
            int stageIndex = stage - 1;

            // Stage 1 simplified: no task failure increase (R5.18a)
            if (stage == 1)
                return 0f;

            int comfortable = ComfortableCapacity[stageIndex];
            int strained = StrainedCapacity[stageIndex];

            // At or below comfortable: no increase
            if (activeEventCount <= comfortable)
                return 0f;

            // Between comfortable and strained: 10% increase (R5.14)
            if (activeEventCount <= strained)
                return 10f;

            // Beyond strained: 25% increase (R5.15)
            return 25f;
        }

        /// <inheritdoc/>
        public float CalculateOverlappingPrepPenalty(int overlappingEventCount)
        {
            // 5% per overlapping event (R5.17)
            // First event doesn't count against itself
            if (overlappingEventCount <= 1)
                return 0f;

            return (overlappingEventCount - 1) * 5f;
        }

        /// <inheritdoc/>
        public BookingResult BookVendor(EventData eventData, VendorData vendor, List<GameDate> bookedDates)
        {
            if (eventData == null)
                return BookingResult.Failed("Event data is required");
            if (vendor == null)
                return BookingResult.Failed("Vendor data is required");

            // Check availability (R8.5)
            if (bookedDates != null && bookedDates.Contains(eventData.eventDate))
            {
                return BookingResult.Failed($"{vendor.vendorName} is not available on {eventData.eventDate.ToDisplayString()}");
            }

            float price = vendor.basePrice;

            // Check if price exceeds remaining budget
            float remainingBudget = eventData.budget.Remaining;
            
            // Deduct from budget (R8.4)
            eventData.budget.spent += price;

            // Add vendor assignment (R8.7)
            eventData.vendors.Add(new VendorAssignment
            {
                vendorId = vendor.id,
                category = vendor.category,
                agreedPrice = price,
                isConfirmed = true,
                bookingDate = eventData.acceptedDate
            });

            // Check if this causes overspend - warn but allow (R7.4)
            if (price > remainingBudget)
            {
                return BookingResult.SuccessfulWithWarning(price, 
                    $"Booking {vendor.vendorName} exceeds remaining budget. Current overage: {eventData.budget.OveragePercent:F1}%");
            }

            return BookingResult.Successful(price, $"Successfully booked {vendor.vendorName}");
        }

        /// <inheritdoc/>
        public BookingResult BookVenue(EventData eventData, VenueData venue, List<GameDate> bookedDates)
        {
            if (eventData == null)
                return BookingResult.Failed("Event data is required");
            if (venue == null)
                return BookingResult.Failed("Venue data is required");

            // Check availability (R9.6)
            if (bookedDates != null && bookedDates.Contains(eventData.eventDate))
            {
                return BookingResult.Failed($"{venue.venueName} is not available on {eventData.eventDate.ToDisplayString()}");
            }

            // Validate capacity (R9.3, R9.4)
            if (eventData.guestCount > venue.capacityMax)
            {
                return BookingResult.Failed(
                    $"{venue.venueName} cannot accommodate {eventData.guestCount} guests. Maximum capacity: {venue.capacityMax}");
            }

            // Calculate price
            float price = venue.basePrice + (venue.pricePerGuest * eventData.guestCount);

            // Check remaining budget
            float remainingBudget = eventData.budget.Remaining;

            // Deduct from budget (R9.5)
            eventData.budget.spent += price;

            // Assign venue (R9.7)
            eventData.venueId = venue.id;

            // Check for cramped conditions warning (R9.4)
            if (eventData.guestCount > venue.capacityComfortable)
            {
                return BookingResult.SuccessfulWithWarning(price,
                    $"Guest count ({eventData.guestCount}) exceeds comfortable capacity ({venue.capacityComfortable}). This may affect satisfaction.");
            }

            // Check if this causes overspend
            if (price > remainingBudget)
            {
                return BookingResult.SuccessfulWithWarning(price,
                    $"Booking {venue.venueName} exceeds remaining budget. Current overage: {eventData.budget.OveragePercent:F1}%");
            }

            return BookingResult.Successful(price, $"Successfully booked {venue.venueName}");
        }

        /// <inheritdoc/>
        public string GenerateEventTitle(string clientName, string subCategory)
        {
            if (string.IsNullOrEmpty(clientName))
                clientName = "Client";
            if (string.IsNullOrEmpty(subCategory))
                subCategory = "Event";

            return $"{clientName}'s {subCategory}";
        }

        /// <inheritdoc/>
        public (float min, float max) GetInquiryIntervalRange(int stage)
        {
            stage = Math.Clamp(stage, 1, 5);
            return InquiryIntervals[stage - 1];
        }

        /// <inheritdoc/>
        public float CalculateAdjustedInquiryInterval(int stage, int reputation)
        {
            stage = Math.Clamp(stage, 1, 5);
            var (min, max) = GetInquiryIntervalRange(stage);
            
            // Base interval is the midpoint
            float baseInterval = (min + max) / 2f;

            // Calculate reputation above stage minimum (R5.7)
            int stageMinRep = StageMinReputation[stage - 1];
            int reputationAboveMin = Math.Max(0, reputation - stageMinRep);

            // Reduce interval by 5% per 25 reputation points above minimum
            float reductionPercent = (reputationAboveMin / 25f) * 5f;
            float reduction = baseInterval * (reductionPercent / 100f);

            // Don't reduce below minimum interval
            return Math.Max(min, baseInterval - reduction);
        }

        #region Private Helper Methods

        /// <summary>
        /// Generate personality based on stage distribution (R14.13).
        /// Stage 1: 50% Easy-Going, 30% Budget-Conscious, 20% Perfectionist
        /// Stage 2: 40% Easy-Going, 35% Budget-Conscious, 25% Perfectionist
        /// Stage 3+: 33% Easy-Going, 33% Budget-Conscious, 34% Perfectionist
        /// </summary>
        private ClientPersonality GeneratePersonalityForStage(int stage)
        {
            int roll = _random.Next(100);

            if (stage == 1)
            {
                if (roll < 50) return ClientPersonality.EasyGoing;
                if (roll < 80) return ClientPersonality.BudgetConscious;
                return ClientPersonality.Perfectionist;
            }
            else if (stage == 2)
            {
                if (roll < 40) return ClientPersonality.EasyGoing;
                if (roll < 75) return ClientPersonality.BudgetConscious;
                return ClientPersonality.Perfectionist;
            }
            else // Stage 3+
            {
                if (roll < 33) return ClientPersonality.EasyGoing;
                if (roll < 66) return ClientPersonality.BudgetConscious;
                return ClientPersonality.Perfectionist;
            }
        }

        /// <summary>
        /// Get a random event type available at the given stage.
        /// </summary>
        private string GetRandomEventTypeForStage(int stage)
        {
            // Stage 1 event types (R6.1-R6.3, R6.8)
            var stage1Types = new[] { "KidsBirthday", "FamilyGathering", "SchoolEvent", "BabyShower" };
            
            // Stage 2+ adds more types (R6.4-R6.7)
            var stage2Types = new[] { "AdultBirthday", "EngagementParty", "CorporateMeeting", "MilestoneBirthday" };

            var availableTypes = new List<string>(stage1Types);
            if (stage >= 2)
                availableTypes.AddRange(stage2Types);

            return availableTypes[_random.Next(availableTypes.Count)];
        }

        /// <summary>
        /// Get a random sub-category for the given event type.
        /// </summary>
        private string GetRandomSubCategoryForEventType(string eventTypeId)
        {
            // Sub-categories from R6.19-R6.26
            var subCategories = eventTypeId switch
            {
                "KidsBirthday" => new[] { "Princess Theme Birthday", "Superhero Theme Birthday", "Pool Party", "Bounce House Party", "Arts & Crafts Party", "Sports Party" },
                "FamilyGathering" => new[] { "Graduation Celebration", "4th of July BBQ", "Thanksgiving Dinner", "Christmas Party", "New Year's Eve Party", "Easter Brunch", "Family Reunion" },
                "SchoolEvent" => new[] { "Science Fair", "Talent Show", "Prom", "Sports Banquet", "Graduation Ceremony", "PTA Fundraiser" },
                "AdultBirthday" => new[] { "Surprise Party", "Cocktail Party", "Dinner Party", "Themed Costume Party", "Outdoor Adventure Party" },
                "EngagementParty" => new[] { "Garden Party", "Rooftop Celebration", "Intimate Dinner", "Brunch Engagement", "Cocktail Reception" },
                "CorporateMeeting" => new[] { "Board Meeting", "Team Building Retreat", "Training Workshop", "Quarterly Review", "Client Presentation", "Staff Appreciation Luncheon" },
                "MilestoneBirthday" => new[] { "Sweet 16", "21st Birthday", "30th Birthday Bash", "40th Birthday", "50th Golden Birthday", "Quinceañera" },
                "BabyShower" => new[] { "Traditional Baby Shower", "Gender Reveal Party", "Couples Baby Shower", "Virtual Baby Shower", "Sprinkle" },
                _ => new[] { "Event" }
            };

            return subCategories[_random.Next(subCategories.Length)];
        }

        /// <summary>
        /// Get budget range for the given event type.
        /// </summary>
        private (int min, int max) GetBudgetRangeForEventType(string eventTypeId)
        {
            // Budget ranges from R6.1-R6.8
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
                _ => (1000, 5000)
            };
        }

        /// <summary>
        /// Generate guest count based on event type.
        /// </summary>
        private int GenerateGuestCount(string eventTypeId)
        {
            var (min, max) = eventTypeId switch
            {
                "KidsBirthday" => (10, 30),
                "FamilyGathering" => (15, 50),
                "SchoolEvent" => (50, 200),
                "AdultBirthday" => (20, 75),
                "EngagementParty" => (30, 100),
                "CorporateMeeting" => (20, 100),
                "MilestoneBirthday" => (25, 100),
                "BabyShower" => (15, 50),
                _ => (20, 50)
            };

            return _random.Next(min, max + 1);
        }

        /// <summary>
        /// Get complexity for the given event type.
        /// </summary>
        private EventComplexity GetComplexityForEventType(string eventTypeId)
        {
            return eventTypeId switch
            {
                "KidsBirthday" => EventComplexity.Low,
                "FamilyGathering" => EventComplexity.Low,
                "SchoolEvent" => EventComplexity.Medium,
                "AdultBirthday" => EventComplexity.Medium,
                "EngagementParty" => EventComplexity.Medium,
                "CorporateMeeting" => EventComplexity.Medium,
                "MilestoneBirthday" => EventComplexity.Medium,
                "BabyShower" => EventComplexity.Medium,
                _ => EventComplexity.Medium
            };
        }

        /// <summary>
        /// Get scheduling range in days based on complexity (R11.3).
        /// </summary>
        private (int min, int max) GetSchedulingRange(EventComplexity complexity)
        {
            return complexity switch
            {
                EventComplexity.Low => (3, 7),
                EventComplexity.Medium => (7, 14),
                EventComplexity.High => (14, 21),
                EventComplexity.VeryHigh => (21, 30),
                _ => (7, 14)
            };
        }

        #endregion
    }
}
