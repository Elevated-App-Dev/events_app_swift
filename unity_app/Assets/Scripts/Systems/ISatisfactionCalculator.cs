using EventPlannerSim.Core;
using EventPlannerSim.Data;

namespace EventPlannerSim.Systems
{
    /// <summary>
    /// Result of satisfaction calculation containing all scores and outcomes.
    /// </summary>
    public class SatisfactionResult
    {
        /// <summary>
        /// Individual category scores (0-100).
        /// </summary>
        public float VenueScore { get; set; }
        public float FoodScore { get; set; }
        public float EntertainmentScore { get; set; }
        public float DecorationScore { get; set; }
        public float ServiceScore { get; set; }
        public float ExpectationScore { get; set; }

        /// <summary>
        /// Base satisfaction before random event modifier.
        /// </summary>
        public float BaseSatisfaction { get; set; }

        /// <summary>
        /// Final satisfaction score (0-100, clamped).
        /// </summary>
        public float FinalSatisfaction { get; set; }

        /// <summary>
        /// Random event modifier applied to base satisfaction.
        /// </summary>
        public float RandomEventModifier { get; set; } = 1f;

        /// <summary>
        /// Whether the event met the client's satisfaction threshold.
        /// </summary>
        public bool MeetsClientThreshold { get; set; }

        /// <summary>
        /// The client's satisfaction threshold based on personality.
        /// </summary>
        public float ClientThreshold { get; set; }
    }

    /// <summary>
    /// Pure logic calculator for client satisfaction scores.
    /// No Unity dependencies - fully unit testable.
    /// Requirements: R13.1-R13.3, R13.8
    /// </summary>
    public interface ISatisfactionCalculator
    {
        /// <summary>
        /// Calculate final satisfaction score (0-100) for a completed event.
        /// </summary>
        /// <param name="eventData">The completed event data with category scores.</param>
        /// <param name="client">The client data including personality.</param>
        /// <returns>SatisfactionResult containing all scores and outcomes.</returns>
        SatisfactionResult Calculate(EventData eventData, ClientData client);

        /// <summary>
        /// Calculate individual category score (0-100).
        /// </summary>
        /// <param name="eventData">The event data.</param>
        /// <param name="category">The budget category to score.</param>
        /// <returns>Score from 0-100 for the category.</returns>
        float CalculateCategoryScore(EventData eventData, BudgetCategory category);

        /// <summary>
        /// Get the satisfaction threshold for a client personality.
        /// Requirements: R15.2-R15.6
        /// </summary>
        /// <param name="personality">The client's personality type.</param>
        /// <returns>The satisfaction threshold (0-100).</returns>
        float GetPersonalityThreshold(ClientPersonality personality);

        /// <summary>
        /// Get the budget overage tolerance percentage for a client personality.
        /// Requirements: R7.9-R7.14
        /// </summary>
        /// <param name="personality">The client's personality type.</param>
        /// <returns>The overage tolerance as a percentage (0-100).</returns>
        float GetOverageTolerance(ClientPersonality personality);

        /// <summary>
        /// Check if the budget overage is within the client's tolerance.
        /// </summary>
        /// <param name="overagePercent">The overage percentage.</param>
        /// <param name="personality">The client's personality type.</param>
        /// <returns>True if overage is within tolerance.</returns>
        bool IsOverageWithinTolerance(float overagePercent, ClientPersonality personality);
    }
}
