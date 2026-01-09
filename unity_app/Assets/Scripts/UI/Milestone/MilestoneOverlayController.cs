using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EventPlannerSim.UI.Core;
using EventPlannerSim.Data;
using EventPlannerSim.Core;
using System;
using System.Collections;

namespace EventPlannerSim.UI.Milestone
{
    /// <summary>
    /// Controls the milestone/stage transition overlay.
    /// </summary>
    public class MilestoneOverlayController : UIControllerBase
    {
        [Header("Views")]
        [SerializeField] private GameObject careerSummaryView;
        [SerializeField] private GameObject pathChoiceView;
        [SerializeField] private GameObject narrativeView;
        [SerializeField] private GameObject creditsView;

        [Header("Career Summary")]
        [SerializeField] private TextMeshProUGUI stageCompleteText;
        [SerializeField] private TextMeshProUGUI eventsCompletedText;
        [SerializeField] private TextMeshProUGUI totalRevenueText;
        [SerializeField] private TextMeshProUGUI avgSatisfactionText;
        [SerializeField] private TextMeshProUGUI reputationText;

        [Header("Path Choice")]
        [SerializeField] private TextMeshProUGUI pathChoiceHeader;
        [SerializeField] private Button pathOption1Button;
        [SerializeField] private Button pathOption2Button;
        [SerializeField] private TextMeshProUGUI pathOption1Text;
        [SerializeField] private TextMeshProUGUI pathOption2Text;

        [Header("Narrative")]
        [SerializeField] private Image narrativeImage;
        [SerializeField] private TextMeshProUGUI narrativeText;

        [Header("Navigation")]
        [SerializeField] private Button continueButton;
        [SerializeField] private Button skipButton;

        public event Action OnContinue;
        public event Action OnSkip;
        public event Action<int> OnPathChosen;

        private MilestoneState _currentState = MilestoneState.Summary;

        protected override void Awake()
        {
            base.Awake();

            if (continueButton != null)
            {
                continueButton.onClick.AddListener(HandleContinue);
            }
            if (skipButton != null)
            {
                skipButton.onClick.AddListener(HandleSkip);
            }
            if (pathOption1Button != null)
            {
                pathOption1Button.onClick.AddListener(() => HandlePathChoice(0));
            }
            if (pathOption2Button != null)
            {
                pathOption2Button.onClick.AddListener(() => HandlePathChoice(1));
            }
        }

        protected override void SubscribeToEvents() { }
        protected override void UnsubscribeFromEvents() { }

        protected override void RefreshDisplay()
        {
            UpdateViewVisibility();
        }

        public void ShowCareerSummary(CareerSummary summary)
        {
            _currentState = MilestoneState.Summary;

            if (stageCompleteText != null)
            {
                stageCompleteText.text = $"{summary.completedStage} Complete!";
            }
            if (eventsCompletedText != null)
            {
                eventsCompletedText.text = $"Events Completed: {summary.eventsCompleted}";
            }
            if (totalRevenueText != null)
            {
                totalRevenueText.text = $"Total Revenue: ${summary.totalRevenue:N0}";
            }
            if (avgSatisfactionText != null)
            {
                avgSatisfactionText.text = $"Avg Satisfaction: {summary.averageSatisfaction:F0}%";
            }
            if (reputationText != null)
            {
                reputationText.text = $"Reputation: {summary.currentReputation}";
            }

            UpdateViewVisibility();
            Show();
        }

        public void ShowPathChoice(string header, string option1, string option2)
        {
            _currentState = MilestoneState.PathChoice;

            if (pathChoiceHeader != null)
            {
                pathChoiceHeader.text = header;
            }
            if (pathOption1Text != null)
            {
                pathOption1Text.text = option1;
            }
            if (pathOption2Text != null)
            {
                pathOption2Text.text = option2;
            }

            UpdateViewVisibility();
        }

        public void ShowNarrative(Sprite image, string text)
        {
            _currentState = MilestoneState.Narrative;

            if (narrativeImage != null && image != null)
            {
                narrativeImage.sprite = image;
                narrativeImage.gameObject.SetActive(true);
            }
            else if (narrativeImage != null)
            {
                narrativeImage.gameObject.SetActive(false);
            }

            if (narrativeText != null)
            {
                narrativeText.text = text;
            }

            UpdateViewVisibility();
        }

        public void ShowCredits()
        {
            _currentState = MilestoneState.Credits;
            UpdateViewVisibility();
        }

        private void UpdateViewVisibility()
        {
            if (careerSummaryView != null)
                careerSummaryView.SetActive(_currentState == MilestoneState.Summary);

            if (pathChoiceView != null)
                pathChoiceView.SetActive(_currentState == MilestoneState.PathChoice);

            if (narrativeView != null)
                narrativeView.SetActive(_currentState == MilestoneState.Narrative);

            if (creditsView != null)
                creditsView.SetActive(_currentState == MilestoneState.Credits);

            // Hide continue button during path choice
            if (continueButton != null)
            {
                continueButton.gameObject.SetActive(_currentState != MilestoneState.PathChoice);
            }
        }

        private void HandleContinue()
        {
            OnContinue?.Invoke();
        }

        private void HandleSkip()
        {
            OnSkip?.Invoke();
            Hide();
        }

        private void HandlePathChoice(int pathIndex)
        {
            OnPathChosen?.Invoke(pathIndex);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private enum MilestoneState
        {
            Summary,
            PathChoice,
            Narrative,
            Credits
        }
    }

    public struct CareerSummary
    {
        public BusinessStage completedStage;
        public int eventsCompleted;
        public float totalRevenue;
        public float averageSatisfaction;
        public int currentReputation;
    }
}
