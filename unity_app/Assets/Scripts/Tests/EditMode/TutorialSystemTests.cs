using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using EventPlannerSim.Core;
using EventPlannerSim.Data;
using EventPlannerSim.Systems;

namespace EventPlannerSim.Tests.EditMode
{
    /// <summary>
    /// Unit tests for TutorialSystem.
    /// Validates: Requirements R25.1-R25.15
    /// </summary>
    [TestFixture]
    public class TutorialSystemTests
    {
        private TutorialSystemImpl _tutorialSystem;

        [SetUp]
        public void Setup()
        {
            _tutorialSystem = new TutorialSystemImpl();
        }

        #region Tutorial Start Tests (R25.1)

        /// <summary>
        /// Test that StartTutorial initializes the tutorial correctly.
        /// **Validates: Requirements R25.1**
        /// </summary>
        [Test]
        public void StartTutorial_InitializesTutorialCorrectly()
        {
            Assert.IsFalse(_tutorialSystem.IsTutorialActive, "Tutorial should not be active initially");
            Assert.IsFalse(_tutorialSystem.IsTutorialComplete, "Tutorial should not be complete initially");

            _tutorialSystem.StartTutorial();

            Assert.IsTrue(_tutorialSystem.IsTutorialActive, "Tutorial should be active after start");
            Assert.IsFalse(_tutorialSystem.IsTutorialComplete, "Tutorial should not be complete after start");
            Assert.AreEqual(TutorialStep.Welcome, _tutorialSystem.CurrentStep, "Should start at Welcome step");
        }

        /// <summary>
        /// Test that StartTutorial fires the OnTutorialStarted event.
        /// **Validates: Requirements R25.1**
        /// </summary>
        [Test]
        public void StartTutorial_FiresOnTutorialStartedEvent()
        {
            bool eventFired = false;
            _tutorialSystem.OnTutorialStarted += () => eventFired = true;

            _tutorialSystem.StartTutorial();

            Assert.IsTrue(eventFired, "OnTutorialStarted event should fire");
        }

        /// <summary>
        /// Test that StartTutorial does nothing if tutorial is already active.
        /// **Validates: Requirements R25.1**
        /// </summary>
        [Test]
        public void StartTutorial_DoesNothingIfAlreadyActive()
        {
            _tutorialSystem.StartTutorial();
            _tutorialSystem.AdvanceStep(); // Move to AcceptClient

            int eventCount = 0;
            _tutorialSystem.OnTutorialStarted += () => eventCount++;

            _tutorialSystem.StartTutorial(); // Try to start again

            Assert.AreEqual(0, eventCount, "OnTutorialStarted should not fire again");
            Assert.AreEqual(TutorialStep.AcceptClient, _tutorialSystem.CurrentStep, "Step should not reset");
        }

        /// <summary>
        /// Test that StartTutorial does nothing if tutorial is already complete.
        /// **Validates: Requirements R25.13**
        /// </summary>
        [Test]
        public void StartTutorial_DoesNothingIfAlreadyComplete()
        {
            _tutorialSystem.SkipTutorial();

            int eventCount = 0;
            _tutorialSystem.OnTutorialStarted += () => eventCount++;

            _tutorialSystem.StartTutorial();

            Assert.AreEqual(0, eventCount, "OnTutorialStarted should not fire if already complete");
            Assert.IsTrue(_tutorialSystem.IsTutorialComplete, "Tutorial should remain complete");
        }

        #endregion

        #region Step Advancement Tests (R25.3)

        /// <summary>
        /// Test that AdvanceStep progresses through all steps correctly.
        /// **Validates: Requirements R25.3, R25.4**
        /// </summary>
        [Test]
        public void AdvanceStep_ProgressesThroughAllSteps()
        {
            _tutorialSystem.StartTutorial();

            var expectedSteps = new[]
            {
                TutorialStep.Welcome,
                TutorialStep.AcceptClient,
                TutorialStep.SelectVenue,
                TutorialStep.SelectCaterer,
                TutorialStep.EventExecution,
                TutorialStep.ViewResults,
                TutorialStep.Complete
            };

            Assert.AreEqual(expectedSteps[0], _tutorialSystem.CurrentStep);

            for (int i = 1; i < expectedSteps.Length; i++)
            {
                _tutorialSystem.AdvanceStep();
                Assert.AreEqual(expectedSteps[i], _tutorialSystem.CurrentStep,
                    $"Step {i} should be {expectedSteps[i]}");
            }
        }

        /// <summary>
        /// Test that AdvanceStep fires OnStepAdvanced event.
        /// **Validates: Requirements R25.3**
        /// </summary>
        [Test]
        public void AdvanceStep_FiresOnStepAdvancedEvent()
        {
            _tutorialSystem.StartTutorial();

            TutorialStep? advancedStep = null;
            _tutorialSystem.OnStepAdvanced += step => advancedStep = step;

            _tutorialSystem.AdvanceStep();

            Assert.AreEqual(TutorialStep.AcceptClient, advancedStep, "OnStepAdvanced should fire with correct step");
        }

        /// <summary>
        /// Test that AdvanceStep does nothing if tutorial is not active.
        /// **Validates: Requirements R25.3**
        /// </summary>
        [Test]
        public void AdvanceStep_DoesNothingIfNotActive()
        {
            int eventCount = 0;
            _tutorialSystem.OnStepAdvanced += _ => eventCount++;

            _tutorialSystem.AdvanceStep();

            Assert.AreEqual(0, eventCount, "OnStepAdvanced should not fire if tutorial not active");
            Assert.AreEqual(TutorialStep.Welcome, _tutorialSystem.CurrentStep, "Step should remain at Welcome");
        }

        /// <summary>
        /// Test that advancing to Complete step completes the tutorial.
        /// **Validates: Requirements R25.12**
        /// </summary>
        [Test]
        public void AdvanceStep_CompletesWhenReachingComplete()
        {
            _tutorialSystem.StartTutorial();

            bool completedEventFired = false;
            _tutorialSystem.OnTutorialCompleted += () => completedEventFired = true;

            // Advance through all steps
            for (int i = 0; i < 6; i++)
            {
                _tutorialSystem.AdvanceStep();
            }

            Assert.IsTrue(completedEventFired, "OnTutorialCompleted should fire");
            Assert.IsTrue(_tutorialSystem.IsTutorialComplete, "Tutorial should be complete");
            Assert.IsFalse(_tutorialSystem.IsTutorialActive, "Tutorial should not be active");
            Assert.AreEqual(TutorialStep.Complete, _tutorialSystem.CurrentStep);
        }

        #endregion

        #region Skip Tutorial Tests (R25.14)

        /// <summary>
        /// Test that SkipTutorial completes the tutorial immediately.
        /// **Validates: Requirements R25.14**
        /// </summary>
        [Test]
        public void SkipTutorial_CompletesImmediately()
        {
            _tutorialSystem.StartTutorial();

            _tutorialSystem.SkipTutorial();

            Assert.IsTrue(_tutorialSystem.IsTutorialComplete, "Tutorial should be complete after skip");
            Assert.IsFalse(_tutorialSystem.IsTutorialActive, "Tutorial should not be active after skip");
            Assert.AreEqual(TutorialStep.Complete, _tutorialSystem.CurrentStep);
        }

        /// <summary>
        /// Test that SkipTutorial fires OnTutorialSkipped event.
        /// **Validates: Requirements R25.14**
        /// </summary>
        [Test]
        public void SkipTutorial_FiresOnTutorialSkippedEvent()
        {
            bool eventFired = false;
            _tutorialSystem.OnTutorialSkipped += () => eventFired = true;

            _tutorialSystem.SkipTutorial();

            Assert.IsTrue(eventFired, "OnTutorialSkipped event should fire");
        }

        /// <summary>
        /// Test that SkipTutorial clears highlights and tips.
        /// **Validates: Requirements R25.14**
        /// </summary>
        [Test]
        public void SkipTutorial_ClearsHighlightsAndTips()
        {
            _tutorialSystem.StartTutorial();
            _tutorialSystem.HighlightElements(new List<string> { "test_element" });
            _tutorialSystem.ShowContextualTip("contingency_budget");

            _tutorialSystem.SkipTutorial();

            Assert.AreEqual(0, _tutorialSystem.HighlightedElements.Count, "Highlights should be cleared");
            Assert.IsNull(_tutorialSystem.CurrentTip, "Current tip should be null");
        }

        #endregion

        #region Highlight Tests (R25.2)

        /// <summary>
        /// Test that HighlightElements sets the highlighted elements correctly.
        /// **Validates: Requirements R25.2**
        /// </summary>
        [Test]
        public void HighlightElements_SetsHighlightedElementsCorrectly()
        {
            var elements = new List<string> { "element1", "element2", "element3" };

            _tutorialSystem.HighlightElements(elements);

            Assert.AreEqual(3, _tutorialSystem.HighlightedElements.Count);
            CollectionAssert.AreEquivalent(elements, _tutorialSystem.HighlightedElements);
        }

        /// <summary>
        /// Test that HighlightElements fires OnElementsHighlighted event.
        /// **Validates: Requirements R25.2**
        /// </summary>
        [Test]
        public void HighlightElements_FiresOnElementsHighlightedEvent()
        {
            List<string> highlightedElements = null;
            _tutorialSystem.OnElementsHighlighted += elements => highlightedElements = elements;

            var elements = new List<string> { "test_element" };
            _tutorialSystem.HighlightElements(elements);

            Assert.IsNotNull(highlightedElements, "OnElementsHighlighted should fire");
            CollectionAssert.AreEquivalent(elements, highlightedElements);
        }

        /// <summary>
        /// Test that HighlightElements replaces previous highlights.
        /// **Validates: Requirements R25.2**
        /// </summary>
        [Test]
        public void HighlightElements_ReplacesPreviousHighlights()
        {
            _tutorialSystem.HighlightElements(new List<string> { "old_element" });
            _tutorialSystem.HighlightElements(new List<string> { "new_element" });

            Assert.AreEqual(1, _tutorialSystem.HighlightedElements.Count);
            Assert.AreEqual("new_element", _tutorialSystem.HighlightedElements[0]);
        }

        /// <summary>
        /// Test that ClearHighlights removes all highlights.
        /// **Validates: Requirements R25.2**
        /// </summary>
        [Test]
        public void ClearHighlights_RemovesAllHighlights()
        {
            _tutorialSystem.HighlightElements(new List<string> { "element1", "element2" });

            bool eventFired = false;
            _tutorialSystem.OnHighlightsCleared += () => eventFired = true;

            _tutorialSystem.ClearHighlights();

            Assert.AreEqual(0, _tutorialSystem.HighlightedElements.Count);
            Assert.IsTrue(eventFired, "OnHighlightsCleared should fire");
        }

        /// <summary>
        /// Test that HighlightElements handles null gracefully.
        /// **Validates: Requirements R25.2**
        /// </summary>
        [Test]
        public void HighlightElements_HandlesNullGracefully()
        {
            _tutorialSystem.HighlightElements(new List<string> { "element" });

            _tutorialSystem.HighlightElements(null);

            // Should not throw and should keep previous highlights
            Assert.AreEqual(1, _tutorialSystem.HighlightedElements.Count);
        }

        #endregion

        #region Contextual Tip Tests (R25.8, R25.10, R25.15)

        /// <summary>
        /// Test that ShowContextualTip displays the correct tip.
        /// **Validates: Requirements R25.8, R25.10, R25.15**
        /// </summary>
        [Test]
        public void ShowContextualTip_DisplaysCorrectTip()
        {
            string tipKey = null;
            string tipText = null;
            _tutorialSystem.OnTipShown += (key, text) => { tipKey = key; tipText = text; };

            _tutorialSystem.ShowContextualTip("contingency_budget");

            Assert.AreEqual("contingency_budget", tipKey);
            Assert.AreEqual("contingency_budget", _tutorialSystem.CurrentTip);
            Assert.IsNotNull(tipText);
            Assert.IsTrue(tipText.Contains("reserve") || tipText.Contains("surprises"),
                "Contingency tip should mention reserves or surprises");
        }

        /// <summary>
        /// Test that GetTipText returns correct text for known tips.
        /// **Validates: Requirements R25.8, R25.10**
        /// </summary>
        [Test]
        public void GetTipText_ReturnsCorrectTextForKnownTips()
        {
            // Test contingency budget tip (R25.8)
            var contingencyTip = _tutorialSystem.GetTipText("contingency_budget");
            Assert.IsNotNull(contingencyTip);
            Assert.IsTrue(contingencyTip.Contains("reserve") || contingencyTip.Contains("surprises"));

            // Test vendor reliability tip (R25.10)
            var vendorTip = _tutorialSystem.GetTipText("vendor_reliability");
            Assert.IsNotNull(vendorTip);
            Assert.IsTrue(vendorTip.Contains("vendor") || vendorTip.Contains("reliability"));
        }

        /// <summary>
        /// Test that GetTipText returns null for unknown tips.
        /// **Validates: Requirements R25.15**
        /// </summary>
        [Test]
        public void GetTipText_ReturnsNullForUnknownTips()
        {
            Assert.IsNull(_tutorialSystem.GetTipText("unknown_tip"));
            Assert.IsNull(_tutorialSystem.GetTipText(null));
            Assert.IsNull(_tutorialSystem.GetTipText(""));
        }

        /// <summary>
        /// Test that HideContextualTip clears the current tip.
        /// **Validates: Requirements R25.15**
        /// </summary>
        [Test]
        public void HideContextualTip_ClearsCurrentTip()
        {
            _tutorialSystem.ShowContextualTip("contingency_budget");

            bool eventFired = false;
            _tutorialSystem.OnTipHidden += () => eventFired = true;

            _tutorialSystem.HideContextualTip();

            Assert.IsNull(_tutorialSystem.CurrentTip);
            Assert.IsTrue(eventFired, "OnTipHidden should fire");
        }

        /// <summary>
        /// Test that ShowContextualTip marks tip as shown.
        /// **Validates: Requirements R25.15**
        /// </summary>
        [Test]
        public void ShowContextualTip_MarksTipAsShown()
        {
            Assert.IsFalse(_tutorialSystem.HasTipBeenShown("contingency_budget"));

            _tutorialSystem.ShowContextualTip("contingency_budget");

            Assert.IsTrue(_tutorialSystem.HasTipBeenShown("contingency_budget"));
        }

        #endregion

        #region Step Instruction Tests (R25.4)

        /// <summary>
        /// Test that GetCurrentStepInstruction returns correct instructions.
        /// **Validates: Requirements R25.4**
        /// </summary>
        [Test]
        public void GetCurrentStepInstruction_ReturnsCorrectInstructions()
        {
            _tutorialSystem.StartTutorial();

            // Welcome step
            var welcomeInstruction = _tutorialSystem.GetCurrentStepInstruction();
            Assert.IsNotNull(welcomeInstruction);
            Assert.IsTrue(welcomeInstruction.Contains("Welcome") || welcomeInstruction.Contains("event planner"));

            // AcceptClient step
            _tutorialSystem.AdvanceStep();
            var acceptInstruction = _tutorialSystem.GetCurrentStepInstruction();
            Assert.IsNotNull(acceptInstruction);
            Assert.IsTrue(acceptInstruction.Contains("client") || acceptInstruction.Contains("Accept"));
        }

        /// <summary>
        /// Test that GetCurrentStepTitle returns correct titles.
        /// **Validates: Requirements R25.4**
        /// </summary>
        [Test]
        public void GetCurrentStepTitle_ReturnsCorrectTitles()
        {
            _tutorialSystem.StartTutorial();

            var welcomeTitle = _tutorialSystem.GetCurrentStepTitle();
            Assert.IsNotNull(welcomeTitle);
            Assert.IsTrue(welcomeTitle.Contains("Welcome"));

            _tutorialSystem.AdvanceStep();
            var acceptTitle = _tutorialSystem.GetCurrentStepTitle();
            Assert.IsNotNull(acceptTitle);
            Assert.IsTrue(acceptTitle.Contains("Client") || acceptTitle.Contains("Accept"));
        }

        #endregion

        #region Stage-Specific Content Tests (R25.4a, R25.5, R25.7a)

        /// <summary>
        /// Test that CurrentStage is clamped to valid range.
        /// **Validates: Requirements R25.4a, R25.5**
        /// </summary>
        [Test]
        public void CurrentStage_ClampedToValidRange()
        {
            _tutorialSystem.CurrentStage = 0;
            Assert.AreEqual(1, _tutorialSystem.CurrentStage, "Stage 0 should clamp to 1");

            _tutorialSystem.CurrentStage = -5;
            Assert.AreEqual(1, _tutorialSystem.CurrentStage, "Negative stage should clamp to 1");

            _tutorialSystem.CurrentStage = 6;
            Assert.AreEqual(5, _tutorialSystem.CurrentStage, "Stage 6 should clamp to 5");

            _tutorialSystem.CurrentStage = 100;
            Assert.AreEqual(5, _tutorialSystem.CurrentStage, "Stage 100 should clamp to 5");
        }

        /// <summary>
        /// Test that ShouldShowStage2Content returns correct value.
        /// **Validates: Requirements R25.4a, R25.5**
        /// </summary>
        [Test]
        public void ShouldShowStage2Content_ReturnsCorrectValue()
        {
            _tutorialSystem.CurrentStage = 1;
            Assert.IsFalse(_tutorialSystem.ShouldShowStage2Content(), "Stage 1 should not show Stage 2 content");

            _tutorialSystem.CurrentStage = 2;
            Assert.IsTrue(_tutorialSystem.ShouldShowStage2Content(), "Stage 2 should show Stage 2 content");

            _tutorialSystem.CurrentStage = 3;
            Assert.IsTrue(_tutorialSystem.ShouldShowStage2Content(), "Stage 3+ should show Stage 2 content");
        }

        /// <summary>
        /// Test that GetWeatherTipKey returns stage-appropriate tip.
        /// **Validates: Requirements R25.7, R25.7a**
        /// </summary>
        [Test]
        public void GetWeatherTipKey_ReturnsStageAppropriateTip()
        {
            _tutorialSystem.CurrentStage = 1;
            Assert.AreEqual("weather_simple", _tutorialSystem.GetWeatherTipKey(),
                "Stage 1 should use simple weather tip");

            _tutorialSystem.CurrentStage = 2;
            Assert.AreEqual("weather_forecast", _tutorialSystem.GetWeatherTipKey(),
                "Stage 2+ should use forecast weather tip");
        }

        #endregion

        #region State Persistence Tests (R25.12, R25.13)

        /// <summary>
        /// Test that LoadState restores tutorial state correctly.
        /// **Validates: Requirements R25.12, R25.13**
        /// </summary>
        [Test]
        public void LoadState_RestoresStateCorrectly()
        {
            var shownTips = new HashSet<string> { "tip1", "tip2" };

            _tutorialSystem.LoadState(true, shownTips);

            Assert.IsTrue(_tutorialSystem.IsTutorialComplete, "Tutorial should be complete");
            Assert.IsFalse(_tutorialSystem.IsTutorialActive, "Tutorial should not be active");
            Assert.AreEqual(TutorialStep.Complete, _tutorialSystem.CurrentStep);
            Assert.IsTrue(_tutorialSystem.HasTipBeenShown("tip1"));
            Assert.IsTrue(_tutorialSystem.HasTipBeenShown("tip2"));
        }

        /// <summary>
        /// Test that GetShownTips returns correct tips.
        /// **Validates: Requirements R25.13**
        /// </summary>
        [Test]
        public void GetShownTips_ReturnsCorrectTips()
        {
            _tutorialSystem.ShowContextualTip("contingency_budget");
            _tutorialSystem.ShowContextualTip("vendor_reliability");

            var shownTips = _tutorialSystem.GetShownTips();

            Assert.AreEqual(2, shownTips.Count);
            Assert.IsTrue(shownTips.Contains("contingency_budget"));
            Assert.IsTrue(shownTips.Contains("vendor_reliability"));
        }

        /// <summary>
        /// Test that ResetTutorial clears all state.
        /// **Validates: Requirements R25.1**
        /// </summary>
        [Test]
        public void ResetTutorial_ClearsAllState()
        {
            _tutorialSystem.StartTutorial();
            _tutorialSystem.AdvanceStep();
            _tutorialSystem.ShowContextualTip("contingency_budget");
            _tutorialSystem.SkipTutorial();

            _tutorialSystem.ResetTutorial();

            Assert.IsFalse(_tutorialSystem.IsTutorialComplete);
            Assert.IsFalse(_tutorialSystem.IsTutorialActive);
            Assert.AreEqual(TutorialStep.Welcome, _tutorialSystem.CurrentStep);
            Assert.AreEqual(0, _tutorialSystem.HighlightedElements.Count);
            Assert.IsNull(_tutorialSystem.CurrentTip);
            Assert.IsFalse(_tutorialSystem.HasTipBeenShown("contingency_budget"));
        }

        #endregion

        #region TutorialStepProgression Tests

        /// <summary>
        /// Test that GetNextStep returns correct next steps.
        /// **Validates: Requirements R25.3**
        /// </summary>
        [Test]
        public void TutorialStepProgression_GetNextStep_ReturnsCorrectSteps()
        {
            Assert.AreEqual(TutorialStep.AcceptClient, TutorialStepProgression.GetNextStep(TutorialStep.Welcome));
            Assert.AreEqual(TutorialStep.SelectVenue, TutorialStepProgression.GetNextStep(TutorialStep.AcceptClient));
            Assert.AreEqual(TutorialStep.SelectCaterer, TutorialStepProgression.GetNextStep(TutorialStep.SelectVenue));
            Assert.AreEqual(TutorialStep.EventExecution, TutorialStepProgression.GetNextStep(TutorialStep.SelectCaterer));
            Assert.AreEqual(TutorialStep.ViewResults, TutorialStepProgression.GetNextStep(TutorialStep.EventExecution));
            Assert.AreEqual(TutorialStep.Complete, TutorialStepProgression.GetNextStep(TutorialStep.ViewResults));
            Assert.AreEqual(TutorialStep.Complete, TutorialStepProgression.GetNextStep(TutorialStep.Complete));
        }

        /// <summary>
        /// Test that GetPreviousStep returns correct previous steps.
        /// **Validates: Requirements R25.3**
        /// </summary>
        [Test]
        public void TutorialStepProgression_GetPreviousStep_ReturnsCorrectSteps()
        {
            Assert.AreEqual(TutorialStep.Welcome, TutorialStepProgression.GetPreviousStep(TutorialStep.Welcome));
            Assert.AreEqual(TutorialStep.Welcome, TutorialStepProgression.GetPreviousStep(TutorialStep.AcceptClient));
            Assert.AreEqual(TutorialStep.AcceptClient, TutorialStepProgression.GetPreviousStep(TutorialStep.SelectVenue));
            Assert.AreEqual(TutorialStep.SelectVenue, TutorialStepProgression.GetPreviousStep(TutorialStep.SelectCaterer));
            Assert.AreEqual(TutorialStep.SelectCaterer, TutorialStepProgression.GetPreviousStep(TutorialStep.EventExecution));
            Assert.AreEqual(TutorialStep.EventExecution, TutorialStepProgression.GetPreviousStep(TutorialStep.ViewResults));
            Assert.AreEqual(TutorialStep.ViewResults, TutorialStepProgression.GetPreviousStep(TutorialStep.Complete));
        }

        /// <summary>
        /// Test that GetStepIndex returns correct indices.
        /// **Validates: Requirements R25.3**
        /// </summary>
        [Test]
        public void TutorialStepProgression_GetStepIndex_ReturnsCorrectIndices()
        {
            Assert.AreEqual(0, TutorialStepProgression.GetStepIndex(TutorialStep.Welcome));
            Assert.AreEqual(1, TutorialStepProgression.GetStepIndex(TutorialStep.AcceptClient));
            Assert.AreEqual(2, TutorialStepProgression.GetStepIndex(TutorialStep.SelectVenue));
            Assert.AreEqual(3, TutorialStepProgression.GetStepIndex(TutorialStep.SelectCaterer));
            Assert.AreEqual(4, TutorialStepProgression.GetStepIndex(TutorialStep.EventExecution));
            Assert.AreEqual(5, TutorialStepProgression.GetStepIndex(TutorialStep.ViewResults));
            Assert.AreEqual(6, TutorialStepProgression.GetStepIndex(TutorialStep.Complete));
        }

        /// <summary>
        /// Test that GetProgressPercent returns correct percentages.
        /// **Validates: Requirements R25.3**
        /// </summary>
        [Test]
        public void TutorialStepProgression_GetProgressPercent_ReturnsCorrectPercentages()
        {
            Assert.AreEqual(0f, TutorialStepProgression.GetProgressPercent(TutorialStep.Welcome), 0.01f);
            Assert.AreEqual(100f, TutorialStepProgression.GetProgressPercent(TutorialStep.Complete), 0.01f);

            // Middle steps should be between 0 and 100
            float acceptProgress = TutorialStepProgression.GetProgressPercent(TutorialStep.AcceptClient);
            Assert.Greater(acceptProgress, 0f);
            Assert.Less(acceptProgress, 100f);
        }

        /// <summary>
        /// Test that GetAllSteps returns all steps in order.
        /// **Validates: Requirements R25.3, R25.4**
        /// </summary>
        [Test]
        public void TutorialStepProgression_GetAllSteps_ReturnsAllStepsInOrder()
        {
            var allSteps = TutorialStepProgression.GetAllSteps();

            Assert.AreEqual(7, allSteps.Count);
            Assert.AreEqual(TutorialStep.Welcome, allSteps[0]);
            Assert.AreEqual(TutorialStep.AcceptClient, allSteps[1]);
            Assert.AreEqual(TutorialStep.SelectVenue, allSteps[2]);
            Assert.AreEqual(TutorialStep.SelectCaterer, allSteps[3]);
            Assert.AreEqual(TutorialStep.EventExecution, allSteps[4]);
            Assert.AreEqual(TutorialStep.ViewResults, allSteps[5]);
            Assert.AreEqual(TutorialStep.Complete, allSteps[6]);
        }

        /// <summary>
        /// Test that GetDefaultStepData returns data for all steps.
        /// **Validates: Requirements R25.4**
        /// </summary>
        [Test]
        public void TutorialStepProgression_GetDefaultStepData_ReturnsDataForAllSteps()
        {
            var stepData = TutorialStepProgression.GetDefaultStepData();

            Assert.AreEqual(7, stepData.Count);

            foreach (TutorialStep step in Enum.GetValues(typeof(TutorialStep)))
            {
                Assert.IsTrue(stepData.ContainsKey(step), $"Should have data for {step}");
                Assert.IsNotNull(stepData[step].Title, $"{step} should have a title");
                Assert.IsNotNull(stepData[step].Instruction, $"{step} should have an instruction");
            }
        }

        #endregion
    }
}
