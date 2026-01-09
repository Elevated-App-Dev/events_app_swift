using System;
using System.Collections.Generic;
using EventPlannerSim.Core;

namespace EventPlannerSim.Systems
{
    /// <summary>
    /// Implementation of the Tutorial System for guided instruction.
    /// Requirements: R25.1-R25.15
    /// </summary>
    public class TutorialSystemImpl : ITutorialSystem
    {
        private bool _isTutorialComplete;
        private bool _isTutorialActive;
        private TutorialStep _currentStep;
        private readonly List<string> _highlightedElements;
        private string _currentTip;
        private readonly HashSet<string> _shownTips;
        private int _currentStage;

        // Tip text dictionary for contextual tips
        private readonly Dictionary<string, string> _tipTexts;

        // Step instructions for the simplified core loop (Stage 1)
        private readonly Dictionary<TutorialStep, (string title, string instruction)> _stepInstructions;

        // Events
        public event Action OnTutorialStarted;
        public event Action<TutorialStep> OnStepAdvanced;
        public event Action OnTutorialCompleted;
        public event Action OnTutorialSkipped;
        public event Action<List<string>> OnElementsHighlighted;
        public event Action OnHighlightsCleared;
        public event Action<string, string> OnTipShown;
        public event Action OnTipHidden;

        // Properties
        public bool IsTutorialComplete => _isTutorialComplete;
        public bool IsTutorialActive => _isTutorialActive;
        public TutorialStep CurrentStep => _currentStep;
        public IReadOnlyList<string> HighlightedElements => _highlightedElements.AsReadOnly();
        public string CurrentTip => _currentTip;

        public int CurrentStage
        {
            get => _currentStage;
            set => _currentStage = Math.Clamp(value, 1, 5);
        }

        public TutorialSystemImpl()
        {
            _isTutorialComplete = false;
            _isTutorialActive = false;
            _currentStep = TutorialStep.Welcome;
            _highlightedElements = new List<string>();
            _currentTip = null;
            _shownTips = new HashSet<string>();
            _currentStage = 1;

            // Initialize tip texts
            // Requirements: R25.8, R25.10, R25.15
            _tipTexts = new Dictionary<string, string>
            {
                // Contingency budget hint (R25.8)
                { "contingency_budget", "Experienced planners always keep something in reserve for surprises." },
                
                // Vendor reliability hint (R25.10)
                { "vendor_reliability", "Some say the best planners develop a sixth sense for vendor reliability over time." },
                
                // Weather awareness (R25.7)
                { "weather_simple", "Check the weather indicator before booking outdoor venues. Good, Risky, or Bad - it's that simple for now!" },
                { "weather_forecast", "The 7-day forecast shows weather predictions. Accuracy improves as the event date approaches." },
                
                // Essential apps (R25.6)
                { "calendar_app", "Your Calendar shows upcoming events and deadlines. Keep an eye on it!" },
                { "messages_app", "Messages is where clients and vendors will contact you. Don't leave them waiting!" },
                { "bank_app", "The Bank app tracks your finances. Watch your balance grow as you succeed!" },
                
                // Contextual app tips (R25.6a)
                { "contacts_app", "Your Contacts app stores vendor information. Build relationships for better deals!" },
                { "tasks_app", "The Tasks app shows what needs to be done for each event." },
                { "clients_app", "The Clients app keeps track of all your past and current clients." },
                
                // Stage 2 specific tips (R25.4a, R25.5)
                { "work_hours", "You have 8 work hours per day. Plan your tasks wisely!" },
                { "budget_allocation", "Allocate your budget across categories. Balance is key to client satisfaction." },
                { "task_management", "Complete tasks before their deadlines to avoid satisfaction penalties." },
                
                // Hidden mechanics hints (R25.9 - subtle, not explicit)
                { "above_and_beyond", "Going the extra mile for clients might have unexpected benefits..." },
                { "client_treatment", "How you treat clients matters more than you might think." },
                
                // General tips (R25.15)
                { "satisfaction_basics", "Client satisfaction depends on venue, food, entertainment, decorations, and service." },
                { "reputation_growth", "Higher satisfaction means better reputation and more referrals!" },
                { "event_types", "Different event types have different requirements. Pay attention to what each client needs." }
            };

            // Initialize step instructions for simplified core loop (R25.4)
            _stepInstructions = new Dictionary<TutorialStep, (string title, string instruction)>
            {
                { 
                    TutorialStep.Welcome, 
                    ("Welcome to Event Planning!", 
                     "Welcome to your new career as an event planner! Let's walk through your first event together.") 
                },
                { 
                    TutorialStep.AcceptClient, 
                    ("Accept Your First Client", 
                     "A client is waiting! Review their request and tap 'Accept' to take on your first event.") 
                },
                { 
                    TutorialStep.SelectVenue, 
                    ("Choose a Venue", 
                     "Every event needs a location. Browse the available venues and select one that fits the budget and guest count.") 
                },
                { 
                    TutorialStep.SelectCaterer, 
                    ("Book a Caterer", 
                     "Good food makes happy guests! Choose a caterer that matches your remaining budget.") 
                },
                { 
                    TutorialStep.EventExecution, 
                    ("Event Day!", 
                     "The big day is here! Watch as your planning comes together. Sometimes surprises happen - that's part of the job!") 
                },
                { 
                    TutorialStep.ViewResults, 
                    ("See Your Results", 
                     "Let's see how you did! Your client's satisfaction score determines your reputation and earnings.") 
                },
                { 
                    TutorialStep.Complete, 
                    ("Tutorial Complete!", 
                     "Congratulations! You've completed your first event. You're ready to build your event planning empire!") 
                }
            };
        }


        /// <summary>
        /// Start the tutorial sequence for a new player.
        /// Requirements: R25.1
        /// </summary>
        public void StartTutorial()
        {
            if (_isTutorialComplete || _isTutorialActive)
                return;

            _isTutorialActive = true;
            _currentStep = TutorialStep.Welcome;
            ClearHighlights();
            HideContextualTip();

            OnTutorialStarted?.Invoke();
            
            // Set up initial highlights for welcome step
            SetupStepHighlights(_currentStep);
        }

        /// <summary>
        /// Advance to the next tutorial step.
        /// Requirements: R25.3
        /// </summary>
        public void AdvanceStep()
        {
            if (!_isTutorialActive || _isTutorialComplete)
                return;

            // Clear current highlights before advancing
            ClearHighlights();
            HideContextualTip();

            // Advance to next step
            TutorialStep nextStep = GetNextStep(_currentStep);
            
            if (nextStep == TutorialStep.Complete)
            {
                _currentStep = TutorialStep.Complete;
                CompleteTutorial();
            }
            else
            {
                _currentStep = nextStep;
                OnStepAdvanced?.Invoke(_currentStep);
                
                // Set up highlights for new step
                SetupStepHighlights(_currentStep);
            }
        }

        /// <summary>
        /// Skip the tutorial entirely.
        /// Requirements: R25.14
        /// </summary>
        public void SkipTutorial()
        {
            if (_isTutorialComplete)
                return;

            ClearHighlights();
            HideContextualTip();
            
            _isTutorialActive = false;
            _isTutorialComplete = true;
            _currentStep = TutorialStep.Complete;

            OnTutorialSkipped?.Invoke();
        }

        /// <summary>
        /// Highlight specific UI elements for current step.
        /// Requirements: R25.2
        /// </summary>
        public void HighlightElements(List<string> elementIds)
        {
            if (elementIds == null)
                return;

            _highlightedElements.Clear();
            _highlightedElements.AddRange(elementIds);

            OnElementsHighlighted?.Invoke(new List<string>(_highlightedElements));
        }

        /// <summary>
        /// Clear all highlighted elements.
        /// </summary>
        public void ClearHighlights()
        {
            if (_highlightedElements.Count == 0)
                return;

            _highlightedElements.Clear();
            OnHighlightsCleared?.Invoke();
        }

        /// <summary>
        /// Show contextual tip for a mechanic.
        /// Requirements: R25.8, R25.10, R25.15
        /// </summary>
        public void ShowContextualTip(string tipKey)
        {
            if (string.IsNullOrEmpty(tipKey))
                return;

            string tipText = GetTipText(tipKey);
            if (string.IsNullOrEmpty(tipText))
                return;

            _currentTip = tipKey;
            MarkTipAsShown(tipKey);

            OnTipShown?.Invoke(tipKey, tipText);
        }

        /// <summary>
        /// Hide the currently displayed contextual tip.
        /// </summary>
        public void HideContextualTip()
        {
            if (_currentTip == null)
                return;

            _currentTip = null;
            OnTipHidden?.Invoke();
        }

        /// <summary>
        /// Get the tip text for a given tip key.
        /// Requirements: R25.8, R25.10, R25.15
        /// </summary>
        public string GetTipText(string tipKey)
        {
            if (string.IsNullOrEmpty(tipKey))
                return null;

            return _tipTexts.TryGetValue(tipKey, out string text) ? text : null;
        }

        /// <summary>
        /// Get the instruction text for the current tutorial step.
        /// Requirements: R25.4
        /// </summary>
        public string GetCurrentStepInstruction()
        {
            if (_stepInstructions.TryGetValue(_currentStep, out var stepInfo))
            {
                return stepInfo.instruction;
            }
            return string.Empty;
        }

        /// <summary>
        /// Get the title for the current tutorial step.
        /// </summary>
        public string GetCurrentStepTitle()
        {
            if (_stepInstructions.TryGetValue(_currentStep, out var stepInfo))
            {
                return stepInfo.title;
            }
            return string.Empty;
        }

        /// <summary>
        /// Check if a specific tip has been shown before.
        /// </summary>
        public bool HasTipBeenShown(string tipKey)
        {
            return _shownTips.Contains(tipKey);
        }

        /// <summary>
        /// Mark a tip as shown.
        /// </summary>
        public void MarkTipAsShown(string tipKey)
        {
            if (!string.IsNullOrEmpty(tipKey))
            {
                _shownTips.Add(tipKey);
            }
        }

        /// <summary>
        /// Get the next tutorial step in sequence.
        /// </summary>
        private TutorialStep GetNextStep(TutorialStep current)
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
        /// Set up highlights for a specific tutorial step.
        /// Requirements: R25.2
        /// </summary>
        private void SetupStepHighlights(TutorialStep step)
        {
            var highlights = GetHighlightsForStep(step);
            if (highlights.Count > 0)
            {
                HighlightElements(highlights);
            }
        }

        /// <summary>
        /// Get the UI element IDs to highlight for a given step.
        /// </summary>
        private List<string> GetHighlightsForStep(TutorialStep step)
        {
            return step switch
            {
                TutorialStep.Welcome => new List<string> { "welcome_panel" },
                TutorialStep.AcceptClient => new List<string> { "client_inquiry_panel", "accept_button" },
                TutorialStep.SelectVenue => new List<string> { "venue_list", "venue_select_button" },
                TutorialStep.SelectCaterer => new List<string> { "vendor_list", "caterer_category", "book_button" },
                TutorialStep.EventExecution => new List<string> { "event_execution_panel" },
                TutorialStep.ViewResults => new List<string> { "results_panel", "satisfaction_score" },
                TutorialStep.Complete => new List<string>(),
                _ => new List<string>()
            };
        }

        /// <summary>
        /// Complete the tutorial and mark it as finished.
        /// Requirements: R25.12
        /// </summary>
        private void CompleteTutorial()
        {
            _isTutorialActive = false;
            _isTutorialComplete = true;

            OnTutorialCompleted?.Invoke();
        }

        /// <summary>
        /// Reset the tutorial state (for testing or new game).
        /// </summary>
        public void ResetTutorial()
        {
            _isTutorialComplete = false;
            _isTutorialActive = false;
            _currentStep = TutorialStep.Welcome;
            _highlightedElements.Clear();
            _currentTip = null;
            _shownTips.Clear();
        }

        /// <summary>
        /// Load tutorial state from save data.
        /// Requirements: R25.13
        /// </summary>
        public void LoadState(bool isComplete, HashSet<string> shownTips)
        {
            _isTutorialComplete = isComplete;
            _isTutorialActive = false;
            _currentStep = isComplete ? TutorialStep.Complete : TutorialStep.Welcome;
            
            _shownTips.Clear();
            if (shownTips != null)
            {
                foreach (var tip in shownTips)
                {
                    _shownTips.Add(tip);
                }
            }
        }

        /// <summary>
        /// Get the set of shown tips for saving.
        /// </summary>
        public HashSet<string> GetShownTips()
        {
            return new HashSet<string>(_shownTips);
        }

        /// <summary>
        /// Check if the tutorial should show stage-specific content.
        /// Requirements: R25.4a, R25.5, R25.7a
        /// </summary>
        public bool ShouldShowStage2Content()
        {
            return _currentStage >= 2;
        }

        /// <summary>
        /// Get stage-appropriate weather tip key.
        /// Requirements: R25.7, R25.7a
        /// </summary>
        public string GetWeatherTipKey()
        {
            return _currentStage >= 2 ? "weather_forecast" : "weather_simple";
        }

        /// <summary>
        /// Add a custom tip text (for extensibility).
        /// </summary>
        public void AddTipText(string tipKey, string tipText)
        {
            if (!string.IsNullOrEmpty(tipKey) && !string.IsNullOrEmpty(tipText))
            {
                _tipTexts[tipKey] = tipText;
            }
        }

        /// <summary>
        /// Check if a tip key exists.
        /// </summary>
        public bool HasTipKey(string tipKey)
        {
            return _tipTexts.ContainsKey(tipKey);
        }

        /// <summary>
        /// Get all available tip keys.
        /// </summary>
        public IReadOnlyCollection<string> GetAllTipKeys()
        {
            return _tipTexts.Keys;
        }
    }
}
