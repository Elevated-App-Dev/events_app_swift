using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EventPlannerSim.UI.Core;
using EventPlannerSim.Core;
using EventPlannerSim.Data;

namespace EventPlannerSim.UI.Phone.Apps
{
    /// <summary>
    /// Controls the Bank app view.
    /// Displays current balance, pending transactions, and transaction history.
    /// </summary>
    public class BankAppController : UIControllerBase, IPhoneAppController
    {
        #region Serialized Fields

        [Header("Balance Display")]
        [SerializeField] private TextMeshProUGUI balanceText;

        [Header("Pending")]
        [SerializeField] private TextMeshProUGUI pendingInText;
        [SerializeField] private TextMeshProUGUI pendingOutText;

        [Header("Transactions List")]
        [SerializeField] private Transform transactionsContainer;
        [SerializeField] private GameObject transactionItemPrefab;
        [SerializeField] private GameObject daySeparatorPrefab;
        [SerializeField] private TextMeshProUGUI noTransactionsText;

        [Header("Emergency Funding")]
        [SerializeField] private GameObject emergencyFundingSection;
        [SerializeField] private Button emergencyFundingButton;
        [SerializeField] private TextMeshProUGUI remainingHelpText;

        #endregion

        #region State

        private List<GameObject> _transactionItems = new List<GameObject>();
        private float _displayedBalance;
        private Coroutine _balanceAnimation;

        #endregion

        #region Lifecycle

        protected override void Awake()
        {
            base.Awake();

            if (emergencyFundingButton != null)
            {
                emergencyFundingButton.onClick.AddListener(OnEmergencyFundingPressed);
            }
        }

        #endregion

        #region UIControllerBase Implementation

        protected override void SubscribeToEvents()
        {
            // Note: Event subscriptions will be added when GameContext exposes events
        }

        protected override void UnsubscribeFromEvents()
        {
            // Note: Event unsubscriptions will be added when GameContext exposes events
        }

        protected override void RefreshDisplay()
        {
            if (!HasValidGameState()) return;

            UpdateBalanceDisplay(false);
            UpdatePendingDisplay();
            UpdateTransactionsList();
            UpdateEmergencyFundingSection();
        }

        #endregion

        #region IPhoneAppController

        public void Refresh()
        {
            RefreshDisplay();
        }

        #endregion

        #region Event Handlers

        private void HandlePlayerDataChanged(PlayerData player)
        {
            if (gameObject.activeInHierarchy)
            {
                UpdateBalanceDisplay(true);
                UpdateEmergencyFundingSection();
            }
        }

        #endregion

        #region Balance Display

        private void UpdateBalanceDisplay(bool animate)
        {
            if (balanceText == null) return;

            var player = GameManager?.CurrentPlayer;
            if (player == null) return;

            float targetBalance = player.money;

            if (animate && gameObject.activeInHierarchy)
            {
                AnimateBalance(targetBalance);
            }
            else
            {
                _displayedBalance = targetBalance;
                balanceText.text = $"${targetBalance:N0}";
                balanceText.color = DesignTokens.Money;
            }
        }

        private void AnimateBalance(float targetBalance)
        {
            if (_balanceAnimation != null)
            {
                StopCoroutine(_balanceAnimation);
            }

            _balanceAnimation = StartCoroutine(
                UIAnimations.AnimateValue(
                    _displayedBalance,
                    targetBalance,
                    UIAnimations.Emphasis,
                    value =>
                    {
                        _displayedBalance = value;
                        if (balanceText != null)
                        {
                            balanceText.text = $"${value:N0}";
                        }
                    }
                )
            );
        }

        #endregion

        #region Pending Display

        private void UpdatePendingDisplay()
        {
            var saveData = GameManager?.CurrentSaveData;

            float pendingIn = CalculatePendingIncome(saveData);
            float pendingOut = CalculatePendingExpenses(saveData);

            if (pendingInText != null)
            {
                pendingInText.text = $"+${pendingIn:N0}";
                pendingInText.color = DesignTokens.Success;
            }

            if (pendingOutText != null)
            {
                pendingOutText.text = $"-${pendingOut:N0}";
                pendingOutText.color = DesignTokens.Error;
            }
        }

        private float CalculatePendingIncome(SaveData saveData)
        {
            float total = 0f;

            if (saveData?.activeEvents != null)
            {
                foreach (var evt in saveData.activeEvents)
                {
                    // Pending income from accepted/planning events
                    if (evt.status == EventStatus.Accepted ||
                        evt.status == EventStatus.Planning ||
                        evt.status == EventStatus.Executing)
                    {
                        total += evt.budget?.total ?? 0f;
                    }
                }
            }

            return total;
        }

        private float CalculatePendingExpenses(SaveData saveData)
        {
            float total = 0f;

            if (saveData?.activeEvents != null)
            {
                foreach (var evt in saveData.activeEvents)
                {
                    // Pending vendor payments - sum all allocations
                    if (evt.budget != null)
                    {
                        total += evt.budget.venueAllocation;
                        total += evt.budget.cateringAllocation;
                        total += evt.budget.entertainmentAllocation;
                        total += evt.budget.decorationsAllocation;
                        total += evt.budget.staffingAllocation;
                    }
                }
            }

            return total;
        }

        #endregion

        #region Transactions List

        private void UpdateTransactionsList()
        {
            if (transactionsContainer == null) return;

            // Clear existing items
            foreach (var item in _transactionItems)
            {
                if (item != null) Destroy(item);
            }
            _transactionItems.Clear();

            // Note: Transaction history will be added when SaveData includes transactionHistory
            // For now, show empty state
            if (noTransactionsText != null)
            {
                noTransactionsText.gameObject.SetActive(true);
                noTransactionsText.text = "No transactions yet";
            }
        }

        #endregion

        #region Emergency Funding

        private void UpdateEmergencyFundingSection()
        {
            if (emergencyFundingSection == null) return;

            var player = GameManager?.CurrentPlayer;

            if (player == null)
            {
                emergencyFundingSection.SetActive(false);
                return;
            }

            // Show if funds are low
            // Note: EmergencyFundingSystem will be added when IGameContext exposes it
            bool showEmergency = player.money < 500f;
            emergencyFundingSection.SetActive(showEmergency);

            if (showEmergency && remainingHelpText != null)
            {
                // Placeholder until EmergencyFundingSystem is available
                remainingHelpText.text = "Help available";
                remainingHelpText.color = DesignTokens.TextSecondary;
            }
        }

        private void OnEmergencyFundingPressed()
        {
            // Note: EmergencyFundingSystem integration will be added when IGameContext exposes it
            UIManager.Instance?.ShowNotification(
                "Family Help",
                "Emergency funding system coming soon",
                NotificationType.FinancialWarning
            );
        }

        #endregion
    }
}
