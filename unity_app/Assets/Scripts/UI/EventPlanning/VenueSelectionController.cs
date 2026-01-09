using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EventPlannerSim.UI.Core;
using EventPlannerSim.Data;
using System;
using System.Collections.Generic;

namespace EventPlannerSim.UI.EventPlanning
{
    /// <summary>
    /// Controls the venue selection panel for event planning.
    /// </summary>
    public class VenueSelectionController : UIControllerBase
    {
        [Header("Display")]
        [SerializeField] private TextMeshProUGUI headerText;
        [SerializeField] private TextMeshProUGUI budgetText;
        [SerializeField] private TextMeshProUGUI capacityRequirementText;

        [Header("Venue List")]
        [SerializeField] private Transform venueListContainer;
        [SerializeField] private GameObject venueCardPrefab;

        [Header("Weather Warning")]
        [SerializeField] private GameObject weatherWarningPanel;
        [SerializeField] private TextMeshProUGUI weatherWarningText;

        [Header("Buttons")]
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button backButton;

        public event Action<VenueData> OnVenueSelected;
        public event Action OnBackRequested;

        private EventData _currentEvent;
        private VenueData _selectedVenue;
        private List<GameObject> _spawnedCards = new List<GameObject>();

        protected override void Awake()
        {
            base.Awake();

            if (confirmButton != null)
            {
                confirmButton.onClick.AddListener(HandleConfirm);
            }
            if (backButton != null)
            {
                backButton.onClick.AddListener(HandleBack);
            }
        }

        protected override void SubscribeToEvents() { }
        protected override void UnsubscribeFromEvents() { }

        protected override void RefreshDisplay()
        {
            if (_currentEvent == null) return;

            if (budgetText != null)
            {
                budgetText.text = $"Venue Budget: ${_currentEvent.budget.venueAllocation:N0}";
            }

            if (capacityRequirementText != null)
            {
                capacityRequirementText.text = $"Required Capacity: {_currentEvent.guestCount} guests";
            }

            UpdateConfirmButton();
        }

        public void SetEvent(EventData eventData)
        {
            _currentEvent = eventData;
            _selectedVenue = null;
            RefreshDisplay();
        }

        public void PopulateVenues(List<VenueData> venues)
        {
            ClearVenueCards();

            foreach (var venue in venues)
            {
                SpawnVenueCard(venue);
            }
        }

        private void SpawnVenueCard(VenueData venue)
        {
            if (venueCardPrefab == null || venueListContainer == null) return;

            var cardObj = Instantiate(venueCardPrefab, venueListContainer);
            _spawnedCards.Add(cardObj);

            // Try to get VenueCardController if it exists
            var cardController = cardObj.GetComponent<VenueCardController>();
            if (cardController != null)
            {
                cardController.SetVenue(venue);
                cardController.OnSelected += HandleVenueCardSelected;
            }
        }

        private void ClearVenueCards()
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

        private void HandleVenueCardSelected(VenueData venue)
        {
            _selectedVenue = venue;

            // Check for outdoor venue weather warning
            if (venue.hasOutdoorSpace && weatherWarningPanel != null)
            {
                weatherWarningPanel.SetActive(true);
                if (weatherWarningText != null)
                {
                    weatherWarningText.text = "Warning: Outdoor venue - weather may affect the event!";
                }
            }
            else if (weatherWarningPanel != null)
            {
                weatherWarningPanel.SetActive(false);
            }

            UpdateConfirmButton();
        }

        private void UpdateConfirmButton()
        {
            if (confirmButton != null)
            {
                confirmButton.interactable = _selectedVenue != null;
            }
        }

        private void HandleConfirm()
        {
            if (_selectedVenue != null)
            {
                OnVenueSelected?.Invoke(_selectedVenue);
            }
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
    /// Controller for individual venue cards in the selection list.
    /// </summary>
    public class VenueCardController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI venueNameText;
        [SerializeField] private TextMeshProUGUI capacityText;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private TextMeshProUGUI typeText;
        [SerializeField] private Button selectButton;
        [SerializeField] private Image backgroundImage;

        public event Action<VenueData> OnSelected;

        private VenueData _venue;
        private bool _isSelected;

        private void Awake()
        {
            if (selectButton != null)
            {
                selectButton.onClick.AddListener(HandleSelect);
            }
        }

        public void SetVenue(VenueData venue)
        {
            _venue = venue;

            if (venueNameText != null)
                venueNameText.text = venue.venueName;

            if (capacityText != null)
                capacityText.text = $"Capacity: {venue.capacityMin}-{venue.capacityMax}";

            if (priceText != null)
                priceText.text = $"${venue.basePrice:N0}";

            if (typeText != null)
                typeText.text = venue.venueType.ToString();
        }

        private void HandleSelect()
        {
            _isSelected = true;
            if (backgroundImage != null)
            {
                backgroundImage.color = DesignTokens.Accent;
            }
            OnSelected?.Invoke(_venue);
        }
    }
}
