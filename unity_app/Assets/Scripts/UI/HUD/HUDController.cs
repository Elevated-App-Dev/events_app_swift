using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EventPlannerSim.UI.Core;
using EventPlannerSim.Core;
using EventPlannerSim.Data;
using EventPlannerSim.Systems;

namespace EventPlannerSim.UI.HUD
{
    /// <summary>
    /// Controls the always-visible heads-up display.
    /// Displays date, money, reputation, and navigation buttons.
    /// </summary>
    public class HUDController : UIControllerBase
    {
        #region Serialized Fields

        [Header("Top Bar - Date")]
        [SerializeField] private TextMeshProUGUI dateText;

        [Header("Top Bar - Reputation")]
        [SerializeField] private TextMeshProUGUI reputationText;
        [SerializeField] private TextMeshProUGUI reputationLabel;

        [Header("Top Bar - Money")]
        [SerializeField] private TextMeshProUGUI moneyText;

        [Header("Bottom Bar - Buttons")]
        [SerializeField] private Button phoneButton;
        [SerializeField] private Button mapButton;
        [SerializeField] private Button settingsButton;

        [Header("Optional - Workload Indicator")]
        [SerializeField] private GameObject workloadIndicator;
        [SerializeField] private TextMeshProUGUI workloadText;

        [Header("Animation")]
        [SerializeField] private float valueAnimationDuration = 0.4f;

        #endregion

        #region State

        private float _displayedMoney;
        private int _displayedReputation;
        private ITimeSystem _timeSystem;
        private Coroutine _moneyAnimation;
        private Coroutine _reputationAnimation;

        #endregion

        #region Lifecycle

        protected override void Awake()
        {
            base.Awake();

            // Set up button listeners
            if (phoneButton != null) phoneButton.onClick.AddListener(OnPhonePressed);
            if (mapButton != null) mapButton.onClick.AddListener(OnMapPressed);
            if (settingsButton != null) settingsButton.onClick.AddListener(OnSettingsPressed);
        }

        #endregion

        #region UIControllerBase Implementation

        protected override void SubscribeToEvents()
        {
            if (GameManager == null) return;

            _timeSystem = GameManager.TimeSystem;

            // Note: Event subscriptions will be added when GameManager exposes events
        }

        protected override void UnsubscribeFromEvents()
        {
            // Note: Event unsubscriptions will be added when GameManager exposes events
        }

        protected override void RefreshDisplay()
        {
            if (!HasValidGameState()) return;

            var player = GameManager.CurrentPlayer;

            // Update date
            UpdateDateDisplay();

            // Set money and reputation without animation (initial load)
            _displayedMoney = player.money;
            _displayedReputation = player.reputation;
            UpdateMoneyDisplay(_displayedMoney, false);
            UpdateReputationDisplay(_displayedReputation, false);

            // Update workload indicator
            UpdateWorkloadIndicator();
        }

        #endregion

        #region Event Handlers

        private void HandleDateChanged(GameDate newDate)
        {
            UpdateDateDisplay();
        }

        private void HandlePlayerDataChanged(PlayerData player)
        {
            if (player == null) return;

            // Animate money change
            if (!Mathf.Approximately(_displayedMoney, player.money))
            {
                AnimateMoney(_displayedMoney, player.money);
            }

            // Animate reputation change
            if (_displayedReputation != player.reputation)
            {
                AnimateReputation(_displayedReputation, player.reputation);
            }

            // Update workload
            UpdateWorkloadIndicator();
        }

        #endregion

        #region Display Updates

        private void UpdateDateDisplay()
        {
            if (dateText == null || _timeSystem == null) return;

            var date = _timeSystem.CurrentDate;
            // Format: "Mar 15" (short month + day)
            dateText.text = date.ToShortDisplayString();
        }

        private void UpdateMoneyDisplay(float amount, bool showChange = true)
        {
            if (moneyText == null) return;

            // Format with monospace-friendly number formatting
            moneyText.text = $"${amount:N0}";

            // Apply color
            moneyText.color = DesignTokens.Money;
        }

        private void UpdateReputationDisplay(int amount, bool showChange = true)
        {
            if (reputationText == null) return;

            reputationText.text = amount.ToString();
            reputationText.color = DesignTokens.Reputation;
        }

        private void UpdateWorkloadIndicator()
        {
            if (workloadIndicator == null) return;

            var player = GameManager?.CurrentPlayer;
            if (player == null) return;

            // Only show workload for Stage 2+
            bool showWorkload = player.stage >= BusinessStage.Employee;
            workloadIndicator.SetActive(showWorkload);

            if (showWorkload && workloadText != null)
            {
                // Note: Workload calculation will be added when IGameContext exposes EventPlanningSystem
                // For now, default to Comfortable
                workloadText.text = WorkloadStatus.Comfortable.ToString();
                workloadText.color = GetWorkloadColor(WorkloadStatus.Comfortable);
            }
        }

        private Color GetWorkloadColor(WorkloadStatus tier)
        {
            return tier switch
            {
                WorkloadStatus.Optimal => DesignTokens.Success,
                WorkloadStatus.Comfortable => DesignTokens.TextPrimary,
                WorkloadStatus.Strained => DesignTokens.Warning,
                WorkloadStatus.Critical => DesignTokens.Error,
                _ => DesignTokens.TextSecondary
            };
        }

        #endregion

        #region Animations

        private void AnimateMoney(float from, float to)
        {
            // Stop any existing animation
            if (_moneyAnimation != null)
            {
                StopCoroutine(_moneyAnimation);
            }

            _moneyAnimation = StartCoroutine(
                UIAnimations.AnimateValue(
                    from,
                    to,
                    valueAnimationDuration,
                    value =>
                    {
                        _displayedMoney = value;
                        UpdateMoneyDisplay(value);
                    }
                )
            );

            // Flash color based on change direction
            StartCoroutine(FlashColor(moneyText, DesignTokens.GetMoneyChangeColor(to - from)));
        }

        private void AnimateReputation(int from, int to)
        {
            // Stop any existing animation
            if (_reputationAnimation != null)
            {
                StopCoroutine(_reputationAnimation);
            }

            _reputationAnimation = StartCoroutine(
                UIAnimations.AnimateIntValue(
                    from,
                    to,
                    valueAnimationDuration,
                    value =>
                    {
                        _displayedReputation = value;
                        UpdateReputationDisplay(value);
                    }
                )
            );

            // Flash color based on change direction
            StartCoroutine(FlashColor(reputationText, DesignTokens.GetReputationChangeColor(to - from)));
        }

        private System.Collections.IEnumerator FlashColor(TextMeshProUGUI text, Color flashColor)
        {
            if (text == null) yield break;

            Color originalColor = text.color;

            // Flash to highlight color
            text.color = flashColor;

            // Wait
            yield return new WaitForSeconds(0.2f);

            // Fade back to original
            float duration = 0.3f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                text.color = Color.Lerp(flashColor, originalColor, elapsed / duration);
                yield return null;
            }

            text.color = originalColor;
        }

        #endregion

        #region Button Handlers

        private void OnPhonePressed()
        {
            UIManager.Instance?.OpenPhone();
        }

        private void OnMapPressed()
        {
            UIManager.Instance?.OpenMap();
        }

        private void OnSettingsPressed()
        {
            UIManager.Instance?.OpenPauseMenu();
        }

        #endregion
    }
}
