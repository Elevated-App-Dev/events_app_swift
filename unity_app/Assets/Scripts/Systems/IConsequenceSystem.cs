using System.Collections.Generic;
using EventPlannerSim.Data;

namespace EventPlannerSim.Systems
{
    /// <summary>
    /// Handles random events during event execution.
    /// Requirements: R12.1-R12.10, R14.14
    /// </summary>
    public interface IConsequenceSystem
    {
        /// <summary>
        /// Evaluate and trigger random events for an executing event.
        /// Random event frequency increases by stage:
        /// Stage 1 = 20%, Stage 2 = 35%, Stage 3 = 50%, Stage 4 = 65%, Stage 5 = 80%
        /// Requirements: R12.2, R14.14
        /// </summary>
        /// <param name="eventData">The event being executed</param>
        /// <param name="stage">Current business stage (1-5)</param>
        /// <returns>List of random events that occurred</returns>
        List<RandomEventResult> EvaluateRandomEvents(EventData eventData, int stage);

        /// <summary>
        /// Calculate satisfaction modifier from random events.
        /// Aggregates all event impacts into a single modifier.
        /// Requirements: R12.3
        /// </summary>
        /// <param name="events">List of random events that occurred</param>
        /// <returns>Total satisfaction modifier (can be positive or negative)</returns>
        float CalculateRandomEventModifier(List<RandomEventResult> events);

        /// <summary>
        /// Check if contingency budget can mitigate a random event.
        /// Requirements: R7.15-R7.17, R12.6
        /// </summary>
        /// <param name="randomEvent">The random event to potentially mitigate</param>
        /// <param name="contingencyBudget">Available contingency budget</param>
        /// <returns>Result indicating if mitigation is possible and details</returns>
        MitigationResult CheckMitigation(RandomEventResult randomEvent, float contingencyBudget);

        /// <summary>
        /// Get the random event frequency for a given stage.
        /// Requirements: R14.14
        /// </summary>
        /// <param name="stage">Business stage (1-5)</param>
        /// <returns>Probability of random event occurring (0.0-1.0)</returns>
        float GetRandomEventFrequency(int stage);
    }
}
