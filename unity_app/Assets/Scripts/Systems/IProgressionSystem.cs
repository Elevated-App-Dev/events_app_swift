using System.Collections.Generic;
using EventPlannerSim.Core;
using EventPlannerSim.Data;

namespace EventPlannerSim.Systems
{
    /// <summary>
    /// Result of applying an event result to player reputation.
    /// </summary>
    public class ReputationChange
    {
        /// <summary>
        /// The amount of reputation change (positive or negative).
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// The new reputation value after the change.
        /// </summary>
        public int NewReputation { get; set; }

        /// <summary>
        /// Whether a referral was triggered by this event.
        /// </summary>
        public bool TriggeredReferral { get; set; }

        /// <summary>
        /// The probability that was used for referral calculation.
        /// </summary>
        public float ReferralProbability { get; set; }

        /// <summary>
        /// Whether a negative review was generated.
        /// </summary>
        public bool GeneratedNegativeReview { get; set; }

        /// <summary>
        /// Description of the reputation change for UI display.
        /// </summary>
        public string Description { get; set; }
    }

    /// <summary>
    /// Distribution of client personalities for a given stage.
    /// </summary>
    public class PersonalityDistribution
    {
        /// <summary>
        /// Probability of EasyGoing personality (0-1).
        /// </summary>
        public float EasyGoingChance { get; set; }

        /// <summary>
        /// Probability of BudgetConscious personality (0-1).
        /// </summary>
        public float BudgetConsciousChance { get; set; }

        /// <summary>
        /// Probability of Perfectionist personality (0-1).
        /// </summary>
        public float PerfectionistChance { get; set; }

        /// <summary>
        /// Probability of Indecisive personality (0-1). Only available Stage 3+.
        /// </summary>
        public float IndecisiveChance { get; set; }

        /// <summary>
        /// Probability of Demanding personality (0-1). Only available Stage 4+.
        /// </summary>
        public float DemandingChance { get; set; }

        /// <summary>
        /// Probability of Celebrity personality (0-1). Only available Stage 5.
        /// </summary>
        public float CelebrityChance { get; set; }
    }

    /// <summary>
    /// Result of a Stage 2 performance review.
    /// </summary>
    public class PerformanceReviewResult
    {
        /// <summary>
        /// The outcome of the review.
        /// </summary>
        public PerformanceReviewOutcome Outcome { get; set; }

        /// <summary>
        /// Average satisfaction score from reviewed events.
        /// </summary>
        public float AverageSatisfaction { get; set; }

        /// <summary>
        /// On-time task completion rate (0-100).
        /// </summary>
        public float OnTimeTaskRate { get; set; }

        /// <summary>
        /// Whether the player was promoted.
        /// </summary>
        public bool WasPromoted { get; set; }

        /// <summary>
        /// Whether the player was demoted.
        /// </summary>
        public bool WasDemoted { get; set; }

        /// <summary>
        /// Whether the player was terminated.
        /// </summary>
        public bool WasTerminated { get; set; }

        /// <summary>
        /// New employee level after the review.
        /// </summary>
        public int NewEmployeeLevel { get; set; }

        /// <summary>
        /// Number of consecutive negative reviews.
        /// </summary>
        public int ConsecutiveNegativeReviews { get; set; }

        /// <summary>
        /// Feedback message for the player.
        /// </summary>
        public string FeedbackMessage { get; set; }
    }

    /// <summary>
    /// Outcome of a performance review.
    /// </summary>
    public enum PerformanceReviewOutcome
    {
        Positive,
        Neutral,
        Negative
    }

    /// <summary>
    /// Requirements for advancing to the next stage.
    /// </summary>
    public class StageRequirements
    {
        public int MinReputation { get; set; }
        public float MinMoney { get; set; }
        public int MinEmployeeLevel { get; set; } // For Stage 2 -> 3
        public string Description { get; set; }
    }

    /// <summary>
    /// Manages player progression through stages and reputation.
    /// Requirements: R14.1-R14.12
    /// </summary>
    public interface IProgressionSystem
    {
        /// <summary>
        /// Apply reputation change based on event satisfaction.
        /// Requirements: R14.1-R14.6
        /// </summary>
        /// <param name="satisfaction">Final satisfaction score (0-100).</param>
        /// <param name="currentReputation">Current player reputation.</param>
        /// <param name="excellenceStreak">Current excellence streak for referral bonus.</param>
        /// <returns>ReputationChange containing the change amount and outcomes.</returns>
        ReputationChange ApplyEventResult(float satisfaction, int currentReputation, int excellenceStreak = 0);

        /// <summary>
        /// Check if player meets requirements for next stage.
        /// Requirements: R14.7-R14.12
        /// </summary>
        /// <param name="player">The player data to check.</param>
        /// <returns>True if player can advance to next stage.</returns>
        bool CanAdvanceStage(PlayerData player);

        /// <summary>
        /// Get the requirements for advancing from current stage.
        /// </summary>
        /// <param name="currentStage">The player's current stage.</param>
        /// <returns>StageRequirements for the next stage.</returns>
        StageRequirements GetStageRequirements(BusinessStage currentStage);

        /// <summary>
        /// Get personality distribution for current stage.
        /// Requirements: R14.13
        /// </summary>
        /// <param name="stage">The business stage (1-5).</param>
        /// <returns>PersonalityDistribution with probabilities for each personality.</returns>
        PersonalityDistribution GetPersonalityDistribution(int stage);

        /// <summary>
        /// Evaluate Stage 2 performance review.
        /// Requirements: R16.5-R16.10
        /// </summary>
        /// <param name="recentEvents">List of recent company events (last 3).</param>
        /// <param name="currentEmployeeLevel">Current employee level (1-5).</param>
        /// <param name="consecutiveNegativeReviews">Number of consecutive negative reviews.</param>
        /// <returns>PerformanceReviewResult with outcome and consequences.</returns>
        PerformanceReviewResult EvaluatePerformance(
            List<EventData> recentEvents,
            int currentEmployeeLevel,
            int consecutiveNegativeReviews);

        /// <summary>
        /// Get the random event frequency for a stage.
        /// Requirements: R14.14
        /// </summary>
        /// <param name="stage">The business stage (1-5).</param>
        /// <returns>Probability of random event (0-1).</returns>
        float GetRandomEventFrequency(int stage);

        /// <summary>
        /// Get the minimum reputation threshold for a stage.
        /// Requirements: R14.16
        /// </summary>
        /// <param name="stage">The business stage (1-5).</param>
        /// <returns>Minimum reputation required to stay in stage.</returns>
        int GetMinimumReputationThreshold(BusinessStage stage);

        /// <summary>
        /// Calculate reputation change for a celebrity event.
        /// Requirements: R15.17-R15.19a
        /// </summary>
        /// <param name="satisfaction">Final satisfaction score.</param>
        /// <param name="pressCoverage">Press coverage status.</param>
        /// <returns>Reputation change (capped at -50 for failures).</returns>
        int CalculateCelebrityReputationChange(float satisfaction, PressCoverage pressCoverage);

        /// <summary>
        /// Check if the Stage 3 milestone sequence should be triggered.
        /// Requirements: R17.1
        /// </summary>
        /// <param name="player">The player data.</param>
        /// <param name="milestoneProgress">Current milestone progress.</param>
        /// <returns>True if milestone sequence should be triggered.</returns>
        bool ShouldTriggerStage3Milestone(PlayerData player, MilestoneProgress milestoneProgress);

        /// <summary>
        /// Advance the player to the next stage.
        /// </summary>
        /// <param name="player">The player data to advance.</param>
        /// <returns>True if advancement was successful.</returns>
        bool AdvanceToNextStage(PlayerData player);
    }

    /// <summary>
    /// Press coverage status for celebrity events.
    /// </summary>
    public enum PressCoverage
    {
        Positive,
        Neutral,
        Negative
    }
}
