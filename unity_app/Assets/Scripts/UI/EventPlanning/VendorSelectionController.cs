using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EventPlannerSim.UI.Core;
using EventPlannerSim.Data;
using EventPlannerSim.Core;
using System;
using System.Collections.Generic;

namespace EventPlannerSim.UI.EventPlanning
{
    /// <summary>
    /// Controls the vendor selection panel for event planning.
    /// </summary>
    public class VendorSelectionController : UIControllerBase
    {
        [Header("Display")]
        [SerializeField] private TextMeshProUGUI headerText;
        [SerializeField] private TextMeshProUGUI categoryBudgetText;

        [Header("Category Tabs")]
        [SerializeField] private Button[] categoryButtons;
        [SerializeField] private TextMeshProUGUI[] categoryLabels;

        [Header("Vendor List")]
        [SerializeField] private Transform vendorListContainer;
        [SerializeField] private GameObject vendorCardPrefab;

        [Header("Required Vendors")]
        [SerializeField] private TextMeshProUGUI requiredVendorsText;

        [Header("Buttons")]
        [SerializeField] private Button completeButton;
        [SerializeField] private Button backButton;

        public event Action OnPlanningComplete;
        public event Action OnBackRequested;

        private EventData _currentEvent;
        private VendorCategory _currentCategory = VendorCategory.Caterer;
        private Dictionary<VendorCategory, VendorData> _selectedVendors = new Dictionary<VendorCategory, VendorData>();
        private List<GameObject> _spawnedCards = new List<GameObject>();

        protected override void Awake()
        {
            base.Awake();

            if (completeButton != null)
            {
                completeButton.onClick.AddListener(HandleComplete);
            }
            if (backButton != null)
            {
                backButton.onClick.AddListener(HandleBack);
            }

            // Set up category button listeners
            for (int i = 0; i < categoryButtons?.Length; i++)
            {
                int index = i;
                if (categoryButtons[i] != null)
                {
                    categoryButtons[i].onClick.AddListener(() => SelectCategory((VendorCategory)index));
                }
            }
        }

        protected override void SubscribeToEvents() { }
        protected override void UnsubscribeFromEvents() { }

        protected override void RefreshDisplay()
        {
            UpdateCategoryBudget();
            UpdateRequiredVendorsStatus();
            UpdateCompleteButton();
        }

        public void SetEvent(EventData eventData)
        {
            _currentEvent = eventData;
            _selectedVendors.Clear();
            SelectCategory(VendorCategory.Caterer);
            RefreshDisplay();
        }

        public void PopulateVendors(List<VendorData> vendors)
        {
            ClearVendorCards();

            foreach (var vendor in vendors)
            {
                if (vendor.category == _currentCategory)
                {
                    SpawnVendorCard(vendor);
                }
            }
        }

        private void SelectCategory(VendorCategory category)
        {
            _currentCategory = category;
            UpdateCategoryBudget();

            // Update tab visuals
            for (int i = 0; i < categoryButtons?.Length; i++)
            {
                if (categoryButtons[i] != null)
                {
                    var colors = categoryButtons[i].colors;
                    colors.normalColor = (VendorCategory)i == category
                        ? DesignTokens.Accent
                        : DesignTokens.Surface;
                    categoryButtons[i].colors = colors;
                }
            }
        }

        private void UpdateCategoryBudget()
        {
            if (categoryBudgetText == null || _currentEvent == null) return;

            float budget = GetBudgetForCategory(_currentCategory);
            categoryBudgetText.text = $"{_currentCategory} Budget: ${budget:N0}";
        }

        private float GetBudgetForCategory(VendorCategory category)
        {
            if (_currentEvent == null) return 0;

            return category switch
            {
                VendorCategory.Caterer => _currentEvent.budget.cateringAllocation,
                VendorCategory.Entertainer => _currentEvent.budget.entertainmentAllocation,
                VendorCategory.Decorator => _currentEvent.budget.decorationsAllocation,
                _ => 0
            };
        }

        private void SpawnVendorCard(VendorData vendor)
        {
            if (vendorCardPrefab == null || vendorListContainer == null) return;

            var cardObj = Instantiate(vendorCardPrefab, vendorListContainer);
            _spawnedCards.Add(cardObj);

            var cardController = cardObj.GetComponent<VendorCardController>();
            if (cardController != null)
            {
                cardController.SetVendor(vendor);
                cardController.OnBooked += HandleVendorBooked;

                // Mark as selected if already booked
                if (_selectedVendors.TryGetValue(vendor.category, out var selectedVendor) &&
                    selectedVendor.id == vendor.id)
                {
                    cardController.SetSelected(true);
                }
            }
        }

        private void ClearVendorCards()
        {
            foreach (var card in _spawnedCards)
            {
                if (card != null)
                {
                    Destroy(card);
                }
            }
            _spawnedCards.Clear();
        }

        private void HandleVendorBooked(VendorData vendor)
        {
            _selectedVendors[vendor.category] = vendor;
            RefreshDisplay();
        }

        private void UpdateRequiredVendorsStatus()
        {
            if (requiredVendorsText == null) return;

            int required = 2; // Caterer and one other typically required
            int booked = _selectedVendors.Count;

            requiredVendorsText.text = $"Vendors Booked: {booked}/{required}";
            requiredVendorsText.color = booked >= required
                ? DesignTokens.Success
                : DesignTokens.TextSecondary;
        }

        private void UpdateCompleteButton()
        {
            if (completeButton != null)
            {
                // Require at least a caterer to complete
                completeButton.interactable = _selectedVendors.ContainsKey(VendorCategory.Caterer);
            }
        }

        private void HandleComplete()
        {
            OnPlanningComplete?.Invoke();
            Hide();
        }

        private void HandleBack()
        {
            OnBackRequested?.Invoke();
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

    /// <summary>
    /// Controller for individual vendor cards in the selection list.
    /// </summary>
    public class VendorCardController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI vendorNameText;
        [SerializeField] private TextMeshProUGUI tierText;
        [SerializeField] private TextMeshProUGUI qualityText;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private Button bookButton;
        [SerializeField] private Image backgroundImage;

        public event Action<VendorData> OnBooked;

        private VendorData _vendor;
        private bool _isSelected;

        private void Awake()
        {
            if (bookButton != null)
            {
                bookButton.onClick.AddListener(HandleBook);
            }
        }

        public void SetVendor(VendorData vendor)
        {
            _vendor = vendor;

            if (vendorNameText != null)
                vendorNameText.text = vendor.vendorName;

            if (tierText != null)
                tierText.text = vendor.tier.ToString();

            if (qualityText != null)
                qualityText.text = $"Quality: {vendor.qualityRating:F1}";

            if (priceText != null)
                priceText.text = $"${vendor.basePrice:N0}";
        }

        public void SetSelected(bool selected)
        {
            _isSelected = selected;
            if (backgroundImage != null)
            {
                backgroundImage.color = selected
                    ? DesignTokens.Accent
                    : DesignTokens.Surface;
            }
        }

        private void HandleBook()
        {
            SetSelected(true);
            OnBooked?.Invoke(_vendor);
        }
    }
}
