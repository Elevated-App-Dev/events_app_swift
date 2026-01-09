using System;
using System.Collections.Generic;
using EventPlannerSim.Core;

namespace EventPlannerSim.Systems
{
    /// <summary>
    /// Manages guided instruction for new players.
    /// Requirements: R25.1-R25.15
    /// </summary>
    public interface ITutorialSystem
    {
        /// <summary>
        /// Start the tutorial sequence for a new player.
        /// Requirements: R25.1
        /// </summary>
        void StartTutorial();

        /// <summary>
        /// Advance to the next tutorial step.
        /// Requirements: R25.3
        /// </summary>
        void AdvanceStep();

        /// <summary>
        /// Skip the tutorial entirely.
        /// Requirements: R25.14
        /// </summary>
        void SkipTutorial();

        /// <summary>
        /// Check if tutorial has been completed.
        /// Requirements: R25.12, R25.13
        /// </summary>
        bool IsTutorialComplete { get; }

        /// <summary>
        /// Check if tutorial is currently active.
        /// </summary>
        bool IsTutorialActive { get; }

        /// <summary>
        /// Get current tutorial step for UI highlighting.
        /// Requirements: R25.2
        /// </summary>
        TutorialStep CurrentStep { get; }

        /// <summary>
        /// Highlight specific UI elements for current step.
        /// Requirements: R25.2
        /// </summary>
        void HighlightElements(List<string> elementIds);

        /// <summary>
        /// Clear all highlighted elements.
        /// </summary>
        void ClearHighlights();

        /// <summary>
        /// Show contextual tip for a mechanic.
        /// Requirements: R25.8, R25.10, R25.15
        /// </summary>
        void ShowContextualTip(string tipKey);

        /// <summary>
        /// Hide the currently displayed contextual tip.
        /// </summary>
        void HideContextualTip();

        /// <summary>
        /// Get the list of currently highlighted element IDs.
        /// </summary>
        IReadOnlyList<string> HighlightedElements { get; }

        /// <summary>
        /// Get the current contextual tip being displayed, or null if none.
        /// </summary>
        string CurrentTip { get; }

        /// <summary>
        /// Get the tip text for a given tip key.
        /// Requirements: R25.8, R25.10, R25.15
        /// </summary>
        string GetTipText(string tipKey);

        /// <summary>
        /// Get the instruction text for the current tutorial step.
        /// Requirements: R25.4
        /// </summary>
        string GetCurrentStepInstruction();

        /// <summary>
        /// Get the title for the current tutorial step.
        /// </summary>
        string GetCurrentStepTitle();

        /// <summary>
        /// Check if a specific tip has been shown before.
        /// </summary>
        bool HasTipBeenShown(string tipKey);

        /// <summary>
        /// Mark a tip as shown.
        /// </summary>
        void MarkTipAsShown(string tipKey);

        /// <summary>
        /// Get the current business stage for stage-specific tutorial content.
        /// Requirements: R25.4a, R25.5, R25.7a
        /// </summary>
        int CurrentStage { get; set; }

        /// <summary>
        /// Event fired when tutorial starts.
        /// </summary>
        event Action OnTutorialStarted;

        /// <summary>
        /// Event fired when tutorial step advances.
        /// </summary>
        event Action<TutorialStep> OnStepAdvanced;

        /// <summary>
        /// Event fired when tutorial is completed.
        /// Requirements: R25.12
        /// </summary>
        event Action OnTutorialCompleted;

        /// <summary>
        /// Event fired when tutorial is skipped.
        /// Requirements: R25.14
        /// </summary>
        event Action OnTutorialSkipped;

        /// <summary>
        /// Event fired when elements are highlighted.
        /// </summary>
        event Action<List<string>> OnElementsHighlighted;

        /// <summary>
        /// Event fired when highlights are cleared.
        /// </summary>
        event Action OnHighlightsCleared;

        /// <summary>
        /// Event fired when a contextual tip is shown.
        /// </summary>
        event Action<string, string> OnTipShown; // tipKey, tipText

        /// <summary>
        /// Event fired when a contextual tip is hidden.
        /// </summary>
        event Action OnTipHidden;
    }
}
