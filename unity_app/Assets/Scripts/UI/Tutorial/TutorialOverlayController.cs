using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EventPlannerSim.UI.Core;
using System;

namespace EventPlannerSim.UI.Tutorial
{
    /// <summary>
    /// Controls the tutorial overlay that guides new players.
    /// </summary>
    public class TutorialOverlayController : UIControllerBase
    {
        [Header("Background")]
        [SerializeField] private Image dimBackground;
        [SerializeField] private float dimAlpha = 0.8f;

        [Header("Instruction Panel")]
        [SerializeField] private GameObject instructionPanel;
        [SerializeField] private TextMeshProUGUI instructionText;
        [SerializeField] private TextMeshProUGUI stepCounterText;

        [Header("Highlight")]
        [SerializeField] private RectTransform highlightFrame;

        [Header("Tip Bubble")]
        [SerializeField] private GameObject tipBubble;
        [SerializeField] private TextMeshProUGUI tipText;

        [Header("Buttons")]
        [SerializeField] private Button continueButton;
        [SerializeField] private Button skipButton;

        [Header("Animation")]
        [SerializeField] private float fadeInDuration = 0.3f;

        public event Action OnContinue;
        public event Action OnSkip;

        private int _currentStep;
        private int _totalSteps;

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
        }

        protected override void SubscribeToEvents() { }
        protected override void UnsubscribeFromEvents() { }

        protected override void RefreshDisplay()
        {
            UpdateStepCounter();
        }

        public void StartTutorial(int totalSteps)
        {
            _totalSteps = totalSteps;
            _currentStep = 0;
            Show();
        }

        public void ShowStep(int step, string instruction, string tip = null)
        {
            _currentStep = step;

            if (instructionText != null)
            {
                instructionText.text = instruction;
            }

            // Show or hide tip bubble
            if (tipBubble != null)
            {
                tipBubble.SetActive(!string.IsNullOrEmpty(tip));
                if (tipText != null && !string.IsNullOrEmpty(tip))
                {
                    tipText.text = tip;
                }
            }

            UpdateStepCounter();
        }

        public void HighlightElement(RectTransform targetElement)
        {
            if (highlightFrame == null || targetElement == null) return;

            // Position highlight frame over target element
            highlightFrame.gameObject.SetActive(true);

            // Match position and size of target
            highlightFrame.position = targetElement.position;
            highlightFrame.sizeDelta = targetElement.sizeDelta + new Vector2(20, 20); // Add padding
        }

        public void ClearHighlight()
        {
            if (highlightFrame != null)
            {
                highlightFrame.gameObject.SetActive(false);
            }
        }

        private void UpdateStepCounter()
        {
            if (stepCounterText != null)
            {
                stepCounterText.text = $"Step {_currentStep + 1} of {_totalSteps}";
            }
        }

        private void HandleContinue()
        {
            OnContinue?.Invoke();
        }

        private void HandleSkip()
        {
            // Could show confirmation dialog here
            OnSkip?.Invoke();
            Hide();
        }

        public void Show()
        {
            gameObject.SetActive(true);

            if (dimBackground != null)
            {
                // Fade in background
                StartCoroutine(FadeInBackground());
            }
        }

        public void Hide()
        {
            ClearHighlight();
            gameObject.SetActive(false);
        }

        private System.Collections.IEnumerator FadeInBackground()
        {
            if (dimBackground == null) yield break;

            float elapsed = 0f;
            Color color = dimBackground.color;
            color.a = 0f;
            dimBackground.color = color;

            while (elapsed < fadeInDuration)
            {
                elapsed += Time.deltaTime;
                color.a = Mathf.Lerp(0f, dimAlpha, elapsed / fadeInDuration);
                dimBackground.color = color;
                yield return null;
            }

            color.a = dimAlpha;
            dimBackground.color = color;
        }
    }
}
