using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EventPlannerSim.UI.Core;
using EventPlannerSim.Data;
using System;
using System.Collections.Generic;

namespace EventPlannerSim.UI.EventExecution
{
    /// <summary>
    /// Controls the event execution panel during live events.
    /// </summary>
    public class EventExecutionController : UIControllerBase
    {
        [Header("Progress")]
        [SerializeField] private TextMeshProUGUI headerText;
        [SerializeField] private Slider progressBar;
        [SerializeField] private TextMeshProUGUI progressText;

        [Header("Status Updates")]
        [SerializeField] private Transform statusContainer;
        [SerializeField] private TextMeshProUGUI statusText;

        [Header("Contingency")]
        [SerializeField] private TextMeshProUGUI contingencyText;

        [Header("Random Event Card")]
        [SerializeField] private GameObject randomEventCardArea;
        [SerializeField] private TextMeshProUGUI randomEventTitle;
        [SerializeField] private TextMeshProUGUI randomEventDescription;
        [SerializeField] private TextMeshProUGUI randomEventImpact;
        [SerializeField] private Button mitigateButton;
        [SerializeField] private Button ignoreButton;
        [SerializeField] private TextMeshProUGUI mitigateCostText;

        public event Action OnEventComplete;
        public event Action<bool> OnRandomEventResolved; // true = mitigated, false = ignored

        private EventData _currentEvent;
        private float _currentProgress;
        private float _contingencyRemaining;
        private List<string> _statusMessages = new List<string>();

        protected override void Awake()
        {
            base.Awake();

            if (mitigateButton != null)
            {
                mitigateButton.onClick.AddListener(() => HandleRandomEventChoice(true));
            }
            if (ignoreButton != null)
            {
                ignoreButton.onClick.AddListener(() => HandleRandomEventChoice(false));
            }
        }

        protected override void SubscribeToEvents() { }
        protected override void UnsubscribeFromEvents() { }

        protected override void RefreshDisplay()
        {
            UpdateProgressDisplay();
            UpdateContingencyDisplay();
            UpdateStatusDisplay();
        }

        public void SetEvent(EventData eventData)
        {
            _currentEvent = eventData;
            _currentProgress = 0f;
            _contingencyRemaining = eventData.budget.contingencyAllocation;
            _statusMessages.Clear();

            if (headerText != null)
            {
                headerText.text = $"{eventData.eventTitle} - In Progress";
            }

            HideRandomEventCard();
            RefreshDisplay();
        }

        public void UpdateProgress(float progress)
        {
            _currentProgress = Mathf.Clamp01(progress);
            UpdateProgressDisplay();

            if (_currentProgress >= 1f)
            {
                OnEventComplete?.Invoke();
            }
        }

        public void AddStatusMessage(string message)
        {
            _statusMessages.Add(message);
            UpdateStatusDisplay();
        }

        public void ShowRandomEvent(string title, string description, int impactAmount, float mitigateCost)
        {
            if (randomEventCardArea != null)
            {
                randomEventCardArea.SetActive(true);
            }

            if (randomEventTitle != null)
                randomEventTitle.text = title;

            if (randomEventDescription != null)
                randomEventDescription.text = description;

            if (randomEventImpact != null)
            {
                randomEventImpact.text = $"Impact: {impactAmount:+#;-#;0} satisfaction";
                randomEventImpact.color = impactAmount < 0 ? DesignTokens.Error : DesignTokens.Success;
            }

            if (mitigateCostText != null)
                mitigateCostText.text = $"Spend ${mitigateCost:N0} to fix";

            // Disable mitigate if not enough contingency
            if (mitigateButton != null)
            {
                mitigateButton.interactable = _contingencyRemaining >= mitigateCost;
            }
        }

        public void HideRandomEventCard()
        {
            if (randomEventCardArea != null)
            {
                randomEventCardArea.SetActive(false);
            }
        }

        public void UseContingency(float amount)
        {
            _contingencyRemaining = Mathf.Max(0, _contingencyRemaining - amount);
            UpdateContingencyDisplay();
        }

        private void UpdateProgressDisplay()
        {
            if (progressBar != null)
            {
                progressBar.value = _currentProgress;
            }

            if (progressText != null)
            {
                progressText.text = $"{_currentProgress * 100:F0}% Complete";
            }
        }

        private void UpdateContingencyDisplay()
        {
            if (contingencyText != null)
            {
                contingencyText.text = $"Contingency: ${_contingencyRemaining:N0}";
                contingencyText.color = _contingencyRemaining < 50
                    ? DesignTokens.Warning
                    : DesignTokens.Success;
            }
        }

        private void UpdateStatusDisplay()
        {
            if (statusText != null && _statusMessages.Count > 0)
            {
                // Show last 5 messages
                int startIndex = Mathf.Max(0, _statusMessages.Count - 5);
                var recentMessages = _statusMessages.GetRange(startIndex, _statusMessages.Count - startIndex);
                statusText.text = string.Join("\n", recentMessages);
            }
        }

        private void HandleRandomEventChoice(bool mitigate)
        {
            HideRandomEventCard();
            OnRandomEventResolved?.Invoke(mitigate);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
