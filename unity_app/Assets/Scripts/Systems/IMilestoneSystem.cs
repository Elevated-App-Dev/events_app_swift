using System.Collections.Generic;
using EventPlannerSim.Core;
using EventPlannerSim.Data;

namespace EventPlannerSim.Systems
{
    /// <summary>
    /// Result of triggering the Stage 3 milestone sequence.
    /// </summary>
    public class MilestoneSequenceResult
    {
        /// <summary>
        /// Whether the milestone sequence should be shown.
        /// </summary>
        public bool ShouldShowSequence { get; set; }

        /// <summary>
        /// The career summary data to display.
        /// </summary>
        public CareerSummaryData CareerSummary { get; set; }

        /// <summary>
        /// Whether the player can skip the sequence (subsequent playthrough).
        /// </summary>
        public bool CanSkip { get; set; }

        /// <summary>
        /// Message explaining why sequence was not triggered (if applicable).
        /// </summary>
        public string Message { get; set; }
    }

    /// <summary>
    /// Result of choosing a career path at Stage 3.
    /// </summary>
    public class PathChoiceResult
    {
        /// <summary>
        /// The chosen career path.
        /// </summary>
        public CareerPath ChosenPath { get; set; }

        /// <summary>
        /// Narrative elements to display for the chosen path.
        /// </summary>
        public List<NarrativeElement> NarrativeElements { get; set; } = new List<NarrativeElement>();

        /// <summary>
        /// Whether the "Going Pro" achievement should be awarded.
        /// </summary>
        public bool ShouldAwardAchievement { get; set; }
    }

    /// <summary>
    /// Represents a narrative element in the milestone sequence.
    /// </summary>
    public class NarrativeElement
    {
        /// <summary>
        /// Type of narrative element.
        /// </summary>
        public NarrativeElementType Type { get; set; }

        /// <summary>
        /// Title or heading for the element.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Description or body text.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Optional image/icon identifier.
        /// </summary>
        public string ImageId { get; set; }
    }

    /// <summary>
    /// Types of narrative elements in the milestone sequence.
    /// </summary>
    public enum NarrativeElementType
    {
        CareerSummary,
        PathChoice,
        EntrepreneurNarrative,
        CorporateNarrative,
        Credits,
        StoryContines
    }

    /// <summary>
    /// Manages Stage 3 milestone sequence and career path choice.
    /// Requirements: R17.1-R17.10
    /// </summary>
    public interface IMilestoneSystem
    {
        /// <summary>
        /// Check if the Stage 3 milestone sequence should be triggered.
        /// Requirements: R17.1
        /// </summary>
        /// <param name="player">The player data.</param>
        /// <param name="milestoneProgress">Current milestone progress.</param>
        /// <returns>True if milestone sequence should be triggered.</returns>
        bool ShouldTriggerMilestone(PlayerData player, MilestoneProgress milestoneProgress);

        /// <summary>
        /// Generate career summary data from player history.
        /// Requirements: R17.2
        /// </summary>
        /// <param name="player">The player data.</param>
        /// <param name="eventHistory">List of completed events.</param>
        /// <param name="journeyStartDate">When the player started their journey.</param>
        /// <returns>CareerSummaryData with journey statistics.</returns>
        CareerSummaryData GenerateCareerSummary(
            PlayerData player,
            List<EventData> eventHistory,
            System.DateTime journeyStartDate);

        /// <summary>
        /// Trigger the Stage 3 milestone sequence.
        /// Requirements: R17.1-R17.2
        /// </summary>
        /// <param name="player">The player data.</param>
        /// <param name="eventHistory">List of completed events.</param>
        /// <param name="milestoneProgress">Current milestone progress.</param>
        /// <param name="journeyStartDate">When the player started their journey.</param>
        /// <returns>MilestoneSequenceResult with sequence data.</returns>
        MilestoneSequenceResult TriggerMilestoneSequence(
            PlayerData player,
            List<EventData> eventHistory,
            MilestoneProgress milestoneProgress,
            System.DateTime journeyStartDate);

        /// <summary>
        /// Process the player's career path choice.
        /// Requirements: R17.3-R17.5
        /// </summary>
        /// <param name="chosenPath">The path chosen by the player.</param>
        /// <param name="milestoneProgress">Milestone progress to update.</param>
        /// <returns>PathChoiceResult with narrative elements.</returns>
        PathChoiceResult ProcessPathChoice(CareerPath chosenPath, MilestoneProgress milestoneProgress);

        /// <summary>
        /// Get the narrative elements for the Entrepreneur path.
        /// Requirements: R17.4
        /// </summary>
        /// <returns>List of narrative elements for Entrepreneur path.</returns>
        List<NarrativeElement> GetEntrepreneurNarrative();

        /// <summary>
        /// Get the narrative elements for the Corporate path.
        /// Requirements: R17.5
        /// </summary>
        /// <returns>List of narrative elements for Corporate path.</returns>
        List<NarrativeElement> GetCorporateNarrative();

        /// <summary>
        /// Get the credits sequence elements.
        /// Requirements: R17.6-R17.7
        /// </summary>
        /// <returns>List of narrative elements for credits.</returns>
        List<NarrativeElement> GetCreditsSequence();

        /// <summary>
        /// Mark the milestone sequence as completed.
        /// Requirements: R17.10
        /// </summary>
        /// <param name="milestoneProgress">Milestone progress to update.</param>
        void CompleteMilestoneSequence(MilestoneProgress milestoneProgress);

        /// <summary>
        /// Check if the player can skip the milestone sequence.
        /// Requirements: R17.10
        /// </summary>
        /// <param name="milestoneProgress">Current milestone progress.</param>
        /// <returns>True if player can skip.</returns>
        bool CanSkipSequence(MilestoneProgress milestoneProgress);
    }
}
