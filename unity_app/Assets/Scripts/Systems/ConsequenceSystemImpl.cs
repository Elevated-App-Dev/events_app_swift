using System;
using System.Collections.Generic;
using EventPlannerSim.Core;
using EventPlannerSim.Data;

namespace EventPlannerSim.Systems
{
    /// <summary>
    /// Implementation of the consequence system handling random events during event execution.
    /// Requirements: R12.1-R12.10, R14.14
    /// </summary>
    public class ConsequenceSystemImpl : IConsequenceSystem
    {
        private readonly Random _random;

        // Random event frequency by stage (R14.14)
        private static readonly Dictionary<int, float> StageEventFrequency = new Dictionary<int, float>
        {
            { 1, 0.20f }, // Stage 1 = 20% chance
            { 2, 0.35f }, // Stage 2 = 35% chance
            { 3, 0.50f }, // Stage 3 = 50% chance
            { 4, 0.65f }, // Stage 4 = 65% chance
            { 5, 0.80f }  // Stage 5 = 80% chance
        };

        // Base satisfaction impacts for each event type
        private static readonly Dictionary<RandomEventType, float> BaseImpacts = new Dictionary<RandomEventType, float>
        {
            // Vendor Issues (R12.2, R12.4)
            { RandomEventType.VendorNoShow, -25f },
            { RandomEventType.VendorLate, -10f },
            { RandomEventType.VendorUnderperformance, -15f },
            
            // Equipment Issues (R12.8)
            { RandomEventType.EquipmentFailure, -15f },
            { RandomEventType.PowerOutage, -20f },
            { RandomEventType.AVMalfunction, -12f },
            
            // Guest Issues (R12.9)
            { RandomEventType.GuestConflict, -8f },
            { RandomEventType.UnexpectedGuests, -5f },
            { RandomEventType.GuestInjury, -18f },
            
            // Weather (R12.5)
            { RandomEventType.WeatherChange, -10f },
            { RandomEventType.ExtremeWeather, -25f },
            
            // Client Issues (R12.10)
            { RandomEventType.LastMinuteChanges, -12f },
            { RandomEventType.ClientComplaint, -8f },
            { RandomEventType.BudgetDispute, -10f },
            
            // Positive Events
            { RandomEventType.UnexpectedCompliment, 5f },
            { RandomEventType.MediaCoverage, 8f },
            { RandomEventType.CelebrityAppearance, 10f }
        };

        // Mitigation costs as percentage of base impact magnitude
        private static readonly Dictionary<RandomEventType, float> MitigationCostMultiplier = new Dictionary<RandomEventType, float>
        {
            { RandomEventType.VendorNoShow, 150f },
            { RandomEventType.VendorLate, 50f },
            { RandomEventType.VendorUnderperformance, 75f },
            { RandomEventType.EquipmentFailure, 100f },
            { RandomEventType.PowerOutage, 120f },
            { RandomEventType.AVMalfunction, 80f },
            { RandomEventType.GuestConflict, 30f },
            { RandomEventType.UnexpectedGuests, 40f },
            { RandomEventType.GuestInjury, 100f },
            { RandomEventType.WeatherChange, 80f },
            { RandomEventType.ExtremeWeather, 200f },
            { RandomEventType.LastMinuteChanges, 100f },
            { RandomEventType.ClientComplaint, 50f },
            { RandomEventType.BudgetDispute, 75f }
        };

        public ConsequenceSystemImpl() : this(new Random()) { }

        public ConsequenceSystemImpl(Random random)
        {
            _random = random ?? throw new ArgumentNullException(nameof(random));
        }

        /// <inheritdoc/>
        public List<RandomEventResult> EvaluateRandomEvents(EventData eventData, int stage)
        {
            if (eventData == null)
                throw new ArgumentNullException(nameof(eventData));

            var results = new List<RandomEventResult>();
            float eventFrequency = GetRandomEventFrequency(stage);

            // Check if a random event occurs based on stage frequency
            if (_random.NextDouble() >= eventFrequency)
                return results; // No random event this time

            // Determine which type of random event occurs
            var possibleEvents = GetPossibleEventsForEvent(eventData);
            if (possibleEvents.Count == 0)
                return results;

            // Select a random event type
            var selectedEventType = possibleEvents[_random.Next(possibleEvents.Count)];
            var randomEvent = CreateRandomEvent(selectedEventType, eventData);
            
            results.Add(randomEvent);

            // Small chance of multiple events at higher stages
            if (stage >= 3 && _random.NextDouble() < 0.15f)
            {
                possibleEvents.Remove(selectedEventType);
                if (possibleEvents.Count > 0)
                {
                    var secondEventType = possibleEvents[_random.Next(possibleEvents.Count)];
                    results.Add(CreateRandomEvent(secondEventType, eventData));
                }
            }

            return results;
        }

        /// <inheritdoc/>
        public float CalculateRandomEventModifier(List<RandomEventResult> events)
        {
            if (events == null || events.Count == 0)
                return 0f;

            float totalModifier = 0f;
            foreach (var evt in events)
            {
                totalModifier += evt.GetFinalImpact();
            }

            return totalModifier;
        }

        /// <inheritdoc/>
        public MitigationResult CheckMitigation(RandomEventResult randomEvent, float contingencyBudget)
        {
            if (randomEvent == null)
                throw new ArgumentNullException(nameof(randomEvent));

            var result = new MitigationResult
            {
                availableBudget = contingencyBudget,
                requiredBudget = randomEvent.mitigationCost,
                canMitigate = false,
                reducedImpact = randomEvent.baseSatisfactionImpact
            };

            // Positive events don't need mitigation
            if (randomEvent.baseSatisfactionImpact >= 0)
            {
                result.canMitigate = false;
                result.mitigationOption = "No mitigation needed for positive events.";
                return result;
            }

            // Check if event can be mitigated
            if (!randomEvent.canBeMitigated)
            {
                result.canMitigate = false;
                result.mitigationOption = "This event cannot be mitigated.";
                return result;
            }

            // Check if we have enough contingency budget
            if (contingencyBudget >= randomEvent.mitigationCost)
            {
                result.canMitigate = true;
                result.reducedImpact = randomEvent.baseSatisfactionImpact * 0.25f;
                result.mitigationOption = randomEvent.mitigationDescription;
            }
            else
            {
                result.canMitigate = false;
                result.mitigationOption = $"Insufficient contingency budget. Need ${randomEvent.mitigationCost:F0}, have ${contingencyBudget:F0}.";
            }

            return result;
        }

        /// <inheritdoc/>
        public float GetRandomEventFrequency(int stage)
        {
            if (StageEventFrequency.TryGetValue(stage, out float frequency))
                return frequency;
            
            // Default to Stage 1 frequency for invalid stages
            return stage < 1 ? 0.20f : 0.80f;
        }

        /// <summary>
        /// Get list of possible random events based on event characteristics.
        /// </summary>
        private List<RandomEventType> GetPossibleEventsForEvent(EventData eventData)
        {
            var possibleEvents = new List<RandomEventType>
            {
                // Always possible vendor issues
                RandomEventType.VendorLate,
                RandomEventType.VendorUnderperformance,
                
                // Equipment issues
                RandomEventType.EquipmentFailure,
                RandomEventType.AVMalfunction,
                
                // Guest issues
                RandomEventType.GuestConflict,
                RandomEventType.UnexpectedGuests,
                
                // Client issues
                RandomEventType.LastMinuteChanges,
                RandomEventType.ClientComplaint
            };

            // Vendor no-show is less common but possible
            if (_random.NextDouble() < 0.3f)
                possibleEvents.Add(RandomEventType.VendorNoShow);

            // Power outage is rare
            if (_random.NextDouble() < 0.2f)
                possibleEvents.Add(RandomEventType.PowerOutage);

            // Guest injury is rare
            if (_random.NextDouble() < 0.1f)
                possibleEvents.Add(RandomEventType.GuestInjury);

            // Budget dispute based on personality
            if (eventData.personality == ClientPersonality.BudgetConscious ||
                eventData.personality == ClientPersonality.Demanding)
            {
                possibleEvents.Add(RandomEventType.BudgetDispute);
            }

            // Positive events are rare
            if (_random.NextDouble() < 0.1f)
            {
                possibleEvents.Add(RandomEventType.UnexpectedCompliment);
            }
            if (_random.NextDouble() < 0.05f)
            {
                possibleEvents.Add(RandomEventType.MediaCoverage);
            }

            return possibleEvents;
        }

        /// <summary>
        /// Create a RandomEventResult for the given event type.
        /// </summary>
        private RandomEventResult CreateRandomEvent(RandomEventType eventType, EventData eventData)
        {
            float baseImpact = BaseImpacts.TryGetValue(eventType, out float impact) ? impact : -10f;
            float mitigationCost = CalculateMitigationCost(eventType, eventData);
            bool canMitigate = baseImpact < 0 && MitigationCostMultiplier.ContainsKey(eventType);

            return new RandomEventResult
            {
                eventType = eventType,
                eventDescription = GetEventDescription(eventType),
                baseSatisfactionImpact = baseImpact,
                mitigationCost = mitigationCost,
                canBeMitigated = canMitigate,
                wasMitigated = false,
                mitigationDescription = GetMitigationDescription(eventType),
                failureDescription = GetFailureDescription(eventType)
            };
        }

        /// <summary>
        /// Calculate mitigation cost based on event type and event budget.
        /// </summary>
        private float CalculateMitigationCost(RandomEventType eventType, EventData eventData)
        {
            if (!MitigationCostMultiplier.TryGetValue(eventType, out float multiplier))
                return 0f;

            // Base cost is a percentage of the event's total budget
            float baseCost = eventData.budget.total * 0.05f; // 5% of total budget as base
            return baseCost * (multiplier / 100f);
        }

        /// <summary>
        /// Get description for a random event type.
        /// </summary>
        private string GetEventDescription(RandomEventType eventType)
        {
            return eventType switch
            {
                RandomEventType.VendorNoShow => "A vendor failed to show up for the event!",
                RandomEventType.VendorLate => "A vendor arrived late, causing delays.",
                RandomEventType.VendorUnderperformance => "A vendor's service quality was below expectations.",
                RandomEventType.EquipmentFailure => "Equipment malfunctioned during the event.",
                RandomEventType.PowerOutage => "A power outage disrupted the event.",
                RandomEventType.AVMalfunction => "Audio/visual equipment experienced issues.",
                RandomEventType.GuestConflict => "A conflict arose between guests.",
                RandomEventType.UnexpectedGuests => "More guests arrived than expected.",
                RandomEventType.GuestInjury => "A guest was injured during the event.",
                RandomEventType.WeatherChange => "Weather conditions changed unexpectedly.",
                RandomEventType.ExtremeWeather => "Extreme weather severely impacted the event.",
                RandomEventType.LastMinuteChanges => "The client requested last-minute changes.",
                RandomEventType.ClientComplaint => "The client raised concerns during the event.",
                RandomEventType.BudgetDispute => "A budget disagreement arose with the client.",
                RandomEventType.UnexpectedCompliment => "Guests gave unexpected praise for the event!",
                RandomEventType.MediaCoverage => "The event received positive media attention!",
                RandomEventType.CelebrityAppearance => "A celebrity made a surprise appearance!",
                _ => "An unexpected event occurred."
            };
        }

        /// <summary>
        /// Get mitigation description for a random event type.
        /// </summary>
        private string GetMitigationDescription(RandomEventType eventType)
        {
            return eventType switch
            {
                RandomEventType.VendorNoShow => "Emergency backup vendor was called in.",
                RandomEventType.VendorLate => "Adjusted schedule to accommodate the delay.",
                RandomEventType.VendorUnderperformance => "Provided additional support to improve service.",
                RandomEventType.EquipmentFailure => "Replacement equipment was quickly sourced.",
                RandomEventType.PowerOutage => "Backup generators were activated.",
                RandomEventType.AVMalfunction => "Technical support resolved the issue.",
                RandomEventType.GuestConflict => "Staff diplomatically resolved the situation.",
                RandomEventType.UnexpectedGuests => "Additional catering and seating arranged.",
                RandomEventType.GuestInjury => "First aid provided and situation handled professionally.",
                RandomEventType.WeatherChange => "Contingency plans were activated.",
                RandomEventType.ExtremeWeather => "Event was moved to backup location.",
                RandomEventType.LastMinuteChanges => "Team adapted quickly to new requirements.",
                RandomEventType.ClientComplaint => "Concerns were addressed promptly.",
                RandomEventType.BudgetDispute => "Transparent cost breakdown provided.",
                _ => "The situation was handled professionally."
            };
        }

        /// <summary>
        /// Get failure description for a random event type.
        /// </summary>
        private string GetFailureDescription(RandomEventType eventType)
        {
            return eventType switch
            {
                RandomEventType.VendorNoShow => "No backup was available, leaving a gap in services.",
                RandomEventType.VendorLate => "The delay caused noticeable disruption.",
                RandomEventType.VendorUnderperformance => "Service quality disappointed guests.",
                RandomEventType.EquipmentFailure => "The malfunction impacted the event experience.",
                RandomEventType.PowerOutage => "The outage significantly disrupted activities.",
                RandomEventType.AVMalfunction => "Technical issues affected presentations.",
                RandomEventType.GuestConflict => "The conflict created an uncomfortable atmosphere.",
                RandomEventType.UnexpectedGuests => "Resources were stretched thin.",
                RandomEventType.GuestInjury => "The incident overshadowed the event.",
                RandomEventType.WeatherChange => "Weather conditions affected outdoor activities.",
                RandomEventType.ExtremeWeather => "Severe weather ruined outdoor elements.",
                RandomEventType.LastMinuteChanges => "Changes couldn't be fully accommodated.",
                RandomEventType.ClientComplaint => "Client concerns went unresolved.",
                RandomEventType.BudgetDispute => "Financial disagreement soured the relationship.",
                _ => "The situation was not handled well."
            };
        }
    }
}
