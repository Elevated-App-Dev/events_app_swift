using System;
using System.Collections.Generic;
using EventPlannerSim.Core;

namespace EventPlannerSim.Data
{
    /// <summary>
    /// Data class representing a tutorial step's configuration.
    /// Requirements: R25.2-R25.4
    /// </summary>
    [Serializable]
    public class TutorialStepData
    {
        /// <summary>
        /// The tutorial step this data represents.
        /// </summary>
        public TutorialStep Step { get; set; }

        /// <summary>
        /// Display title for this step.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Instruction text shown to the player.
        /// Requirements: R25.4
        /// </summary>
        public string Instruction { get; set; }

        /// <summary>
        /// UI element IDs to highlight during this step.
        /// Requirements: R25.2
        /// </summary>
        public List<string> HighlightElementIds { get; set; }

        /// <summary>
        /// UI element IDs to disable during this step.
        /// Requirements: R25.2
        /// </summary>
        public List<string> DisabledElementIds { get; set; }

        /// <summary>
        /// Optional tip key to show during this step.
        /// </summary>
        public string TipKey { get; set; }

        /// <summary>
        /// Whether this step requires player action to advance.
        /// </summary>
        public bool RequiresPlayerAction { get; set; }

        /// <summary>
        /// The action that triggers advancement to the next step.
        /// </summary>
        public string TriggerAction { get; set; }

        /// <summary>
        /// Minimum stage required for this step to be shown.
        /// Requirements: R25.4a, R25.5
        /// </summary>
        public int MinimumStage { get; set; }

        public TutorialStepData()
        {
            HighlightElementIds = new List<string>();
            DisabledElementIds = new List<string>();
            RequiresPlayerAction = true;
            MinimumStage = 1;
        }

        public TutorialStepData(TutorialStep step, string title, string instruction)
            : this()
        {
            Step = step;
            Title = title;
            Instruction = instruction;
        }
    }

    /// <summary>
    /// Data class for persisting tutorial state in save data.
    /// Requirements: R25.12, R25.13
    /// </summary>
    [Serializable]
    public class TutorialSaveData
    {
        /// <summary>
        /// Whether the tutorial has been completed.
        /// Requirements: R25.12
        /// </summary>
        public bool IsComplete { get; set; }

        /// <summary>
        /// Whether the tutorial was skipped.
        /// Requirements: R25.14
        /// </summary>
        public bool WasSkipped { get; set; }

        /// <summary>
        /// The current step if tutorial is in progress.
        /// </summary>
        public TutorialStep CurrentStep { get; set; }

        /// <summary>
        /// List of tip keys that have been shown to the player.
        /// </summary>
        public List<string> ShownTips { get; set; }

        public TutorialSaveData()
        {
            IsComplete = false;
            WasSkipped = false;
            CurrentStep = TutorialStep.Welcome;
            ShownTips = new List<string>();
        }
    }

    /// <summary>
    /// Static helper class for tutorial step progression logic.
    /// Requirements: R25.3, R25.4
    /// </summary>
    public static class TutorialStepProgression
    {
        /// <summary>
        /// Get the next step in the tutorial sequence.
        /// Requirements: R25.3
        /// </summary>
        public static TutorialStep GetNextStep(TutorialStep current)
        {
            return current switch
            {
                TutorialStep.Welcome => TutorialStep.AcceptClient,
                TutorialStep.AcceptClient => TutorialStep.SelectVenue,
                TutorialStep.SelectVenue => TutorialStep.SelectCaterer,
                TutorialStep.SelectCaterer => TutorialStep.EventExecution,
                TutorialStep.EventExecution => TutorialStep.ViewResults,
                TutorialStep.ViewResults => TutorialStep.Complete,
                TutorialStep.Complete => TutorialStep.Complete,
                _ => TutorialStep.Complete
            };
        }

        /// <summary>
        /// Get the previous step in the tutorial sequence.
        /// </summary>
        public static TutorialStep GetPreviousStep(TutorialStep current)
        {
            return current switch
            {
                TutorialStep.Welcome => TutorialStep.Welcome,
                TutorialStep.AcceptClient => TutorialStep.Welcome,
                TutorialStep.SelectVenue => TutorialStep.AcceptClient,
                TutorialStep.SelectCaterer => TutorialStep.SelectVenue,
                TutorialStep.EventExecution => TutorialStep.SelectCaterer,
                TutorialStep.ViewResults => TutorialStep.EventExecution,
                TutorialStep.Complete => TutorialStep.ViewResults,
                _ => TutorialStep.Welcome
            };
        }

        /// <summary>
        /// Check if a step is the first step.
        /// </summary>
        public static bool IsFirstStep(TutorialStep step)
        {
            return step == TutorialStep.Welcome;
        }

        /// <summary>
        /// Check if a step is the last step.
        /// </summary>
        public static bool IsLastStep(TutorialStep step)
        {
            return step == TutorialStep.Complete;
        }

        /// <summary>
        /// Get the step index (0-based).
        /// </summary>
        public static int GetStepIndex(TutorialStep step)
        {
            return step switch
            {
                TutorialStep.Welcome => 0,
                TutorialStep.AcceptClient => 1,
                TutorialStep.SelectVenue => 2,
                TutorialStep.SelectCaterer => 3,
                TutorialStep.EventExecution => 4,
                TutorialStep.ViewResults => 5,
                TutorialStep.Complete => 6,
                _ => 0
            };
        }

        /// <summary>
        /// Get the total number of steps (excluding Complete).
        /// </summary>
        public static int GetTotalSteps()
        {
            return 6; // Welcome through ViewResults
        }

        /// <summary>
        /// Get progress percentage (0-100).
        /// </summary>
        public static float GetProgressPercent(TutorialStep step)
        {
            int index = GetStepIndex(step);
            int total = GetTotalSteps();
            return (float)index / total * 100f;
        }

        /// <summary>
        /// Get the step from an index.
        /// </summary>
        public static TutorialStep GetStepFromIndex(int index)
        {
            return index switch
            {
                0 => TutorialStep.Welcome,
                1 => TutorialStep.AcceptClient,
                2 => TutorialStep.SelectVenue,
                3 => TutorialStep.SelectCaterer,
                4 => TutorialStep.EventExecution,
                5 => TutorialStep.ViewResults,
                6 => TutorialStep.Complete,
                _ => TutorialStep.Welcome
            };
        }

        /// <summary>
        /// Get all steps in order.
        /// </summary>
        public static IReadOnlyList<TutorialStep> GetAllSteps()
        {
            return new List<TutorialStep>
            {
                TutorialStep.Welcome,
                TutorialStep.AcceptClient,
                TutorialStep.SelectVenue,
                TutorialStep.SelectCaterer,
                TutorialStep.EventExecution,
                TutorialStep.ViewResults,
                TutorialStep.Complete
            };
        }

        /// <summary>
        /// Get the default step data for all steps.
        /// Requirements: R25.4
        /// </summary>
        public static Dictionary<TutorialStep, TutorialStepData> GetDefaultStepData()
        {
            return new Dictionary<TutorialStep, TutorialStepData>
            {
                {
                    TutorialStep.Welcome,
                    new TutorialStepData(
                        TutorialStep.Welcome,
                        "Welcome to Event Planning!",
                        "Welcome to your new career as an event planner! Let's walk through your first event together."
                    )
                    {
                        HighlightElementIds = new List<string> { "welcome_panel" },
                        RequiresPlayerAction = true,
                        TriggerAction = "continue_button_click"
                    }
                },
                {
                    TutorialStep.AcceptClient,
                    new TutorialStepData(
                        TutorialStep.AcceptClient,
                        "Accept Your First Client",
                        "A client is waiting! Review their request and tap 'Accept' to take on your first event."
                    )
                    {
                        HighlightElementIds = new List<string> { "client_inquiry_panel", "accept_button" },
                        RequiresPlayerAction = true,
                        TriggerAction = "accept_inquiry"
                    }
                },
                {
                    TutorialStep.SelectVenue,
                    new TutorialStepData(
                        TutorialStep.SelectVenue,
                        "Choose a Venue",
                        "Every event needs a location. Browse the available venues and select one that fits the budget and guest count."
                    )
                    {
                        HighlightElementIds = new List<string> { "venue_list", "venue_select_button" },
                        RequiresPlayerAction = true,
                        TriggerAction = "book_venue"
                    }
                },
                {
                    TutorialStep.SelectCaterer,
                    new TutorialStepData(
                        TutorialStep.SelectCaterer,
                        "Book a Caterer",
                        "Good food makes happy guests! Choose a caterer that matches your remaining budget."
                    )
                    {
                        HighlightElementIds = new List<string> { "vendor_list", "caterer_category", "book_button" },
                        RequiresPlayerAction = true,
                        TriggerAction = "book_vendor"
                    }
                },
                {
                    TutorialStep.EventExecution,
                    new TutorialStepData(
                        TutorialStep.EventExecution,
                        "Event Day!",
                        "The big day is here! Watch as your planning comes together. Sometimes surprises happen - that's part of the job!"
                    )
                    {
                        HighlightElementIds = new List<string> { "event_execution_panel" },
                        RequiresPlayerAction = false, // Auto-advances after execution
                        TriggerAction = "event_complete"
                    }
                },
                {
                    TutorialStep.ViewResults,
                    new TutorialStepData(
                        TutorialStep.ViewResults,
                        "See Your Results",
                        "Let's see how you did! Your client's satisfaction score determines your reputation and earnings."
                    )
                    {
                        HighlightElementIds = new List<string> { "results_panel", "satisfaction_score" },
                        RequiresPlayerAction = true,
                        TriggerAction = "close_results"
                    }
                },
                {
                    TutorialStep.Complete,
                    new TutorialStepData(
                        TutorialStep.Complete,
                        "Tutorial Complete!",
                        "Congratulations! You've completed your first event. You're ready to build your event planning empire!"
                    )
                    {
                        HighlightElementIds = new List<string>(),
                        RequiresPlayerAction = true,
                        TriggerAction = "start_free_play"
                    }
                }
            };
        }
    }
}
