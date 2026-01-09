using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EventPlannerSim.UI.Core;
using EventPlannerSim.Data;
using System;

namespace EventPlannerSim.UI.EventPlanning
{
    /// <summary>
    /// Controls the budget allocation panel where players distribute event budget.
    /// </summary>
    public class BudgetAllocationController : UIControllerBase
    {
        [Header("Budget Display")]
        [SerializeField] private TextMeshProUGUI totalBudgetText;
        [SerializeField] private TextMeshProUGUI remainingBudgetText;

        [Header("Category Sliders")]
        [SerializeField] private Slider venueSlider;
        [SerializeField] private Slider cateringSlider;
        [SerializeField] private Slider entertainmentSlider;
        [SerializeField] private Slider decorationsSlider;
        [SerializeField] private Slider staffingSlider;
        [SerializeField] private Slider contingencySlider;

        [Header("Category Amount Labels")]
        [SerializeField] private TextMeshProUGUI venueAmountText;
        [SerializeField] private TextMeshProUGUI cateringAmountText;
        [SerializeField] private TextMeshProUGUI entertainmentAmountText;
        [SerializeField] private TextMeshProUGUI decorationsAmountText;
        [SerializeField] private TextMeshProUGUI staffingAmountText;
        [SerializeField] private TextMeshProUGUI contingencyAmountText;

        [Header("Warnings")]
        [SerializeField] private TextMeshProUGUI warningText;

        [Header("Buttons")]
        [SerializeField] private Button confirmButton;

        public event Action<EventBudget> OnBudgetConfirmed;

        private EventData _currentEvent;
        private float _totalBudget;

        protected override void Awake()
        {
            base.Awake();

            // Set up slider listeners
            SetupSliderListener(venueSlider);
            SetupSliderListener(cateringSlider);
            SetupSliderListener(entertainmentSlider);
            SetupSliderListener(decorationsSlider);
            SetupSliderListener(staffingSlider);
            SetupSliderListener(contingencySlider);

            if (confirmButton != null)
            {
                confirmButton.onClick.AddListener(HandleConfirm);
            }
        }

        private void SetupSliderListener(Slider slider)
        {
            if (slider != null)
            {
                slider.onValueChanged.AddListener(_ => OnSliderChanged());
            }
        }

        protected override void SubscribeToEvents() { }
        protected override void UnsubscribeFromEvents() { }

        protected override void RefreshDisplay()
        {
            UpdateAllocationDisplay();
        }

        public void SetEvent(EventData eventData)
        {
            _currentEvent = eventData;
            _totalBudget = eventData.budget.total;

            if (totalBudgetText != null)
            {
                totalBudgetText.text = $"Total Budget: ${_totalBudget:N0}";
            }

            // Initialize sliders with recommended percentages
            InitializeSliders();
            RefreshDisplay();
        }

        private void InitializeSliders()
        {
            // Default recommended allocations
            SetSliderValue(venueSlider, 0.25f);      // 25% venue
            SetSliderValue(cateringSlider, 0.30f);   // 30% catering
            SetSliderValue(entertainmentSlider, 0.15f); // 15% entertainment
            SetSliderValue(decorationsSlider, 0.10f);   // 10% decorations
            SetSliderValue(staffingSlider, 0.10f);      // 10% staffing
            SetSliderValue(contingencySlider, 0.10f);   // 10% contingency
        }

        private void SetSliderValue(Slider slider, float value)
        {
            if (slider != null)
            {
                slider.value = value;
            }
        }

        private void OnSliderChanged()
        {
            UpdateAllocationDisplay();
        }

        private void UpdateAllocationDisplay()
        {
            float venueAlloc = GetSliderValue(venueSlider) * _totalBudget;
            float cateringAlloc = GetSliderValue(cateringSlider) * _totalBudget;
            float entertainmentAlloc = GetSliderValue(entertainmentSlider) * _totalBudget;
            float decorationsAlloc = GetSliderValue(decorationsSlider) * _totalBudget;
            float staffingAlloc = GetSliderValue(staffingSlider) * _totalBudget;
            float contingencyAlloc = GetSliderValue(contingencySlider) * _totalBudget;

            float totalAllocated = venueAlloc + cateringAlloc + entertainmentAlloc +
                                   decorationsAlloc + staffingAlloc + contingencyAlloc;
            float remaining = _totalBudget - totalAllocated;

            // Update amount labels
            UpdateAmountLabel(venueAmountText, venueAlloc);
            UpdateAmountLabel(cateringAmountText, cateringAlloc);
            UpdateAmountLabel(entertainmentAmountText, entertainmentAlloc);
            UpdateAmountLabel(decorationsAmountText, decorationsAlloc);
            UpdateAmountLabel(staffingAmountText, staffingAlloc);
            UpdateAmountLabel(contingencyAmountText, contingencyAlloc);

            // Update remaining
            if (remainingBudgetText != null)
            {
                remainingBudgetText.text = $"Remaining: ${remaining:N0}";
                remainingBudgetText.color = remaining < 0 ? DesignTokens.Error : DesignTokens.TextPrimary;
            }

            // Update warnings
            UpdateWarnings(remaining, contingencyAlloc);

            // Enable/disable confirm button
            if (confirmButton != null)
            {
                confirmButton.interactable = remaining >= 0;
            }
        }

        private float GetSliderValue(Slider slider)
        {
            return slider != null ? slider.value : 0f;
        }

        private void UpdateAmountLabel(TextMeshProUGUI label, float amount)
        {
            if (label != null)
            {
                label.text = $"${amount:N0}";
            }
        }

        private void UpdateWarnings(float remaining, float contingency)
        {
            if (warningText == null) return;

            if (remaining < 0)
            {
                warningText.text = "Budget exceeded!";
                warningText.color = DesignTokens.Error;
                warningText.gameObject.SetActive(true);
            }
            else if (contingency < _totalBudget * 0.05f)
            {
                warningText.text = "Warning: Low contingency budget";
                warningText.color = DesignTokens.Warning;
                warningText.gameObject.SetActive(true);
            }
            else
            {
                warningText.gameObject.SetActive(false);
            }
        }

        private void HandleConfirm()
        {
            if (_currentEvent == null) return;

            var budget = new EventBudget
            {
                total = _totalBudget,
                venueAllocation = GetSliderValue(venueSlider) * _totalBudget,
                cateringAllocation = GetSliderValue(cateringSlider) * _totalBudget,
                entertainmentAllocation = GetSliderValue(entertainmentSlider) * _totalBudget,
                decorationsAllocation = GetSliderValue(decorationsSlider) * _totalBudget,
                staffingAllocation = GetSliderValue(staffingSlider) * _totalBudget,
                contingencyAllocation = GetSliderValue(contingencySlider) * _totalBudget
            };

            OnBudgetConfirmed?.Invoke(budget);
            Hide();
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
