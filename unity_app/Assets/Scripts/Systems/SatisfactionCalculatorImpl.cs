using System;
using EventPlannerSim.Core;
using EventPlannerSim.Data;

namespace EventPlannerSim.Systems
{
    /// <summary>
    /// Implementation of satisfaction calculation logic.
    /// Pure C# class with no Unity dependencies for testability.
    /// Requirements: R13.1-R13.3, R13.8, R15.2-R15.6, R7.9-R7.14
    /// </summary>
    public class SatisfactionCalculatorImpl : ISatisfactionCalculator
    {
        // Category weights as per R13.2
        // Venue 20%, Food 25%, Entertainment 20%, Decorations 15%, Service 10%, Expectations 10%
        private const float VenueWeight = 0.20f;
        private const float FoodWeight = 0.25f;
        private const float EntertainmentWeight = 0.20f;
        private const float DecorationWeight = 0.15f;
        private const float ServiceWeight = 0.10f;
        private const float ExpectationWeight = 0.10f;

        /// <summary>
        /// Calculate final satisfaction score for a completed event.
        /// Requirements: R13.1-R13.3, R13.8
        /// </summary>
        public SatisfactionResult Calculate(EventData eventData, ClientData client)
        {
            if (eventData == null)
                throw new ArgumentNullException(nameof(eventData));
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            var results = eventData.results ?? new EventResults();
            
            // Get category scores from event results
            float venueScore = results.venueScore;
            float foodScore = results.foodScore;
            float entertainmentScore = results.entertainmentScore;
            float decorationScore = results.decorationScore;
            float serviceScore = results.serviceScore;
            float expectationScore = results.expectationScore;

            // Calculate weighted base satisfaction (R13.2)
            float baseSatisfaction = CalculateWeightedSatisfaction(
                venueScore, foodScore, entertainmentScore,
                decorationScore, serviceScore, expectationScore);

            // Apply random event modifier (R13.3)
            float randomEventModifier = results.randomEventModifier;
            float modifiedSatisfaction = baseSatisfaction * randomEventModifier;

            // Clamp final satisfaction to 0-100 (R13.8)
            float finalSatisfaction = ClampSatisfaction(modifiedSatisfaction);

            // Get client threshold based on personality (R15.2-R15.6)
            float clientThreshold = GetPersonalityThreshold(client.personality);

            return new SatisfactionResult
            {
                VenueScore = venueScore,
                FoodScore = foodScore,
                EntertainmentScore = entertainmentScore,
                DecorationScore = decorationScore,
                ServiceScore = serviceScore,
                ExpectationScore = expectationScore,
                BaseSatisfaction = baseSatisfaction,
                RandomEventModifier = randomEventModifier,
                FinalSatisfaction = finalSatisfaction,
                ClientThreshold = clientThreshold,
                MeetsClientThreshold = finalSatisfaction >= clientThreshold
            };
        }

        /// <summary>
        /// Calculate weighted satisfaction from category scores.
        /// </summary>
        private float CalculateWeightedSatisfaction(
            float venue, float food, float entertainment,
            float decoration, float service, float expectation)
        {
            return venue * VenueWeight +
                   food * FoodWeight +
                   entertainment * EntertainmentWeight +
                   decoration * DecorationWeight +
                   service * ServiceWeight +
                   expectation * ExpectationWeight;
        }

        /// <summary>
        /// Clamp satisfaction score to valid range 0-100.
        /// Requirements: R13.8
        /// </summary>
        public static float ClampSatisfaction(float satisfaction)
        {
            return Math.Max(0f, Math.Min(100f, satisfaction));
        }

        /// <summary>
        /// Calculate individual category score.
        /// This is a simplified implementation - actual scoring would depend on
        /// vendor quality, budget allocation, and other factors.
        /// </summary>
        public float CalculateCategoryScore(EventData eventData, BudgetCategory category)
        {
            if (eventData?.results == null)
                return 0f;

            return category switch
            {
                BudgetCategory.Venue => eventData.results.venueScore,
                BudgetCategory.Catering => eventData.results.foodScore,
                BudgetCategory.Entertainment => eventData.results.entertainmentScore,
                BudgetCategory.Decorations => eventData.results.decorationScore,
                _ => 50f // Default score for other categories
            };
        }

        /// <summary>
        /// Get satisfaction threshold for a client personality.
        /// Requirements: R15.2-R15.6
        /// </summary>
        public float GetPersonalityThreshold(ClientPersonality personality)
        {
            return personality switch
            {
                ClientPersonality.EasyGoing => 50f,      // R15.2
                ClientPersonality.BudgetConscious => 60f, // R15.3
                ClientPersonality.Perfectionist => 85f,   // R15.4
                ClientPersonality.Indecisive => 65f,      // R15.7 (Stage 3+)
                ClientPersonality.Demanding => 80f,       // R15.8 (Stage 4+)
                ClientPersonality.Celebrity => 70f,       // R15.9 (Stage 5) - varies, using average
                _ => 50f // Default to EasyGoing threshold
            };
        }

        /// <summary>
        /// Get budget overage tolerance percentage for a client personality.
        /// Requirements: R7.9
        /// </summary>
        public float GetOverageTolerance(ClientPersonality personality)
        {
            return personality switch
            {
                ClientPersonality.EasyGoing => 15f,       // R7.9
                ClientPersonality.BudgetConscious => 0f,  // R7.9
                ClientPersonality.Perfectionist => 5f,    // R7.9
                ClientPersonality.Indecisive => 10f,      // R7.9
                ClientPersonality.Demanding => 5f,        // R7.9
                ClientPersonality.Celebrity => 10f,       // Not specified, using moderate tolerance
                _ => 0f // Default to no tolerance
            };
        }

        /// <summary>
        /// Check if budget overage is within client's tolerance.
        /// Requirements: R7.10
        /// </summary>
        public bool IsOverageWithinTolerance(float overagePercent, ClientPersonality personality)
        {
            float tolerance = GetOverageTolerance(personality);
            return overagePercent <= tolerance;
        }
    }
}
