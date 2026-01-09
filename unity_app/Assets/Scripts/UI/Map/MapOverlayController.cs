using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EventPlannerSim.UI.Core;
using EventPlannerSim.Data;
using EventPlannerSim.Core;
using EventPlannerSim.Systems;

namespace EventPlannerSim.UI.Map
{
    /// <summary>
    /// Controls the Map overlay for city navigation.
    /// Displays zones, location pins, and preview cards.
    /// </summary>
    public class MapOverlayController : UIControllerBase
    {
        #region Serialized Fields

        [Header("Map Views")]
        [SerializeField] private GameObject cityOverviewView;
        [SerializeField] private GameObject zoneDetailView;

        [Header("Zone Buttons (Overview)")]
        [SerializeField] private Button neighborhoodButton;
        [SerializeField] private Button downtownButton;
        [SerializeField] private Button uptownButton;
        [SerializeField] private Button waterfrontButton;

        [Header("Zone Visuals")]
        [SerializeField] private Image neighborhoodZone;
        [SerializeField] private Image downtownZone;
        [SerializeField] private Image uptownZone;
        [SerializeField] private Image waterfrontZone;

        [Header("Zone Labels")]
        [SerializeField] private TextMeshProUGUI neighborhoodLabel;
        [SerializeField] private TextMeshProUGUI downtownLabel;
        [SerializeField] private TextMeshProUGUI uptownLabel;
        [SerializeField] private TextMeshProUGUI waterfrontLabel;

        [Header("Zone Detail View")]
        [SerializeField] private TextMeshProUGUI zoneNameText;
        [SerializeField] private Transform pinsContainer;
        [SerializeField] private GameObject locationPinPrefab;
        [SerializeField] private Button backToOverviewButton;

        [Header("Filter Bar")]
        [SerializeField] private Button filterAllButton;
        [SerializeField] private Button filterVenuesButton;
        [SerializeField] private Button filterVendorsButton;

        [Header("Preview Card")]
        [SerializeField] private GameObject previewCard;
        [SerializeField] private TextMeshProUGUI previewNameText;
        [SerializeField] private TextMeshProUGUI previewTypeText;
        [SerializeField] private TextMeshProUGUI previewDescriptionText;
        [SerializeField] private TextMeshProUGUI previewTierText;
        [SerializeField] private Button previewSelectButton;
        [SerializeField] private Button previewCloseButton;

        [Header("Close")]
        [SerializeField] private Button closeButton;

        #endregion

        #region State

        private IMapSystem _mapSystem;
        private MapZone? _currentZone = null;
        private LocationType _currentFilter = LocationType.All;
        private List<GameObject> _locationPins = new List<GameObject>();
        private string _selectedLocationId = null;

        // Callback for when a location is selected (used during event planning)
        public System.Action<string> OnLocationSelected;

        #endregion

        #region Unity Lifecycle

        protected override void Awake()
        {
            base.Awake();
            _mapSystem = GameManager?.MapSystem;
        }

        protected override void Start()
        {
            base.Start();
            HidePreviewCard();
            ShowCityOverview();
        }

        #endregion

        #region UIControllerBase Implementation

        protected override void SubscribeToEvents()
        {
            // Zone buttons
            if (neighborhoodButton != null)
                neighborhoodButton.onClick.AddListener(() => NavigateToZone(MapZone.Neighborhood));
            if (downtownButton != null)
                downtownButton.onClick.AddListener(() => NavigateToZone(MapZone.Downtown));
            if (uptownButton != null)
                uptownButton.onClick.AddListener(() => NavigateToZone(MapZone.Uptown));
            if (waterfrontButton != null)
                waterfrontButton.onClick.AddListener(() => NavigateToZone(MapZone.Waterfront));

            // Navigation
            if (backToOverviewButton != null)
                backToOverviewButton.onClick.AddListener(ShowCityOverview);
            if (closeButton != null)
                closeButton.onClick.AddListener(Close);

            // Filters
            if (filterAllButton != null)
                filterAllButton.onClick.AddListener(() => SetFilter(LocationType.All));
            if (filterVenuesButton != null)
                filterVenuesButton.onClick.AddListener(() => SetFilter(LocationType.Venue));
            if (filterVendorsButton != null)
                filterVendorsButton.onClick.AddListener(() => SetFilter(LocationType.Vendor));

            // Preview
            if (previewCloseButton != null)
                previewCloseButton.onClick.AddListener(HidePreviewCard);
            if (previewSelectButton != null)
                previewSelectButton.onClick.AddListener(SelectCurrentLocation);

            // Note: Map system event subscriptions will be added when IMapSystem is updated
        }

        protected override void UnsubscribeFromEvents()
        {
            if (neighborhoodButton != null)
                neighborhoodButton.onClick.RemoveAllListeners();
            if (downtownButton != null)
                downtownButton.onClick.RemoveAllListeners();
            if (uptownButton != null)
                uptownButton.onClick.RemoveAllListeners();
            if (waterfrontButton != null)
                waterfrontButton.onClick.RemoveAllListeners();

            if (backToOverviewButton != null)
                backToOverviewButton.onClick.RemoveAllListeners();
            if (closeButton != null)
                closeButton.onClick.RemoveAllListeners();

            if (filterAllButton != null)
                filterAllButton.onClick.RemoveAllListeners();
            if (filterVenuesButton != null)
                filterVenuesButton.onClick.RemoveAllListeners();
            if (filterVendorsButton != null)
                filterVendorsButton.onClick.RemoveAllListeners();

            if (previewCloseButton != null)
                previewCloseButton.onClick.RemoveAllListeners();
            if (previewSelectButton != null)
                previewSelectButton.onClick.RemoveAllListeners();

            // Note: Map system event unsubscriptions will be added when IMapSystem is updated
        }

        protected override void RefreshDisplay()
        {
            if (!HasValidGameState()) return;

            UpdateZoneVisuals();

            if (_currentZone.HasValue)
            {
                UpdateLocationPins();
            }
        }

        #endregion

        #region City Overview

        private void ShowCityOverview()
        {
            _currentZone = null;
            // Note: ClearZoneSelection will be added when IMapSystem is updated

            if (cityOverviewView != null) cityOverviewView.SetActive(true);
            if (zoneDetailView != null) zoneDetailView.SetActive(false);

            HidePreviewCard();
            UpdateZoneVisuals();
        }

        private void UpdateZoneVisuals()
        {
            var player = GameManager?.CurrentPlayer;
            int stage = player?.stage != null ? (int)player.stage : 1;

            // Get visible zones for current stage
            var visibleZones = _mapSystem?.GetVisibleZones(stage) ?? new List<MapZone>();

            // Update each zone's visual state
            UpdateZoneButton(MapZone.Neighborhood, neighborhoodButton, neighborhoodZone, neighborhoodLabel, visibleZones);
            UpdateZoneButton(MapZone.Downtown, downtownButton, downtownZone, downtownLabel, visibleZones);
            UpdateZoneButton(MapZone.Uptown, uptownButton, uptownZone, uptownLabel, visibleZones);
            UpdateZoneButton(MapZone.Waterfront, waterfrontButton, waterfrontZone, waterfrontLabel, visibleZones);
        }

        private void UpdateZoneButton(MapZone zone, Button button, Image zoneImage, TextMeshProUGUI label, List<MapZone> visibleZones)
        {
            bool isUnlocked = visibleZones.Contains(zone);

            if (button != null)
            {
                button.interactable = isUnlocked;
            }

            if (zoneImage != null)
            {
                Color zoneColor = GetZoneColor(zone);
                zoneImage.color = isUnlocked ? zoneColor : DesignTokens.Darken(zoneColor, 0.3f);
            }

            if (label != null)
            {
                label.text = GetZoneName(zone);
                label.color = isUnlocked ? DesignTokens.TextPrimary : DesignTokens.TextMuted;

                // Add location count if unlocked
                if (isUnlocked && _mapSystem != null)
                {
                    var locations = _mapSystem.GetLocationsInZone(zone);
                    int count = locations?.Count ?? 0;
                    if (count > 0)
                    {
                        label.text += $"\n{count} locations";
                    }
                }
                else if (!isUnlocked)
                {
                    label.text += "\nLocked";
                }
            }
        }

        private Color GetZoneColor(MapZone zone)
        {
            return zone switch
            {
                MapZone.Neighborhood => DesignTokens.ZoneNeighborhood,
                MapZone.Downtown => DesignTokens.ZoneDowntown,
                MapZone.Uptown => DesignTokens.ZoneUptown,
                MapZone.Waterfront => DesignTokens.ZoneWaterfront,
                _ => DesignTokens.Surface
            };
        }

        private string GetZoneName(MapZone zone)
        {
            return zone switch
            {
                MapZone.Neighborhood => "Neighborhood",
                MapZone.Downtown => "Downtown",
                MapZone.Uptown => "Uptown",
                MapZone.Waterfront => "Waterfront",
                _ => zone.ToString()
            };
        }

        #endregion

        #region Zone Detail

        private void NavigateToZone(MapZone zone)
        {
            _mapSystem?.NavigateToZone(zone);
        }

        private void HandleZoneNavigated(MapZone zone)
        {
            _currentZone = zone;

            if (cityOverviewView != null) cityOverviewView.SetActive(false);
            if (zoneDetailView != null) zoneDetailView.SetActive(true);

            if (zoneNameText != null)
            {
                zoneNameText.text = GetZoneName(zone);
            }

            HidePreviewCard();
            UpdateLocationPins();
        }

        private void UpdateLocationPins()
        {
            // Clear existing pins
            foreach (var pin in _locationPins)
            {
                if (pin != null) Destroy(pin);
            }
            _locationPins.Clear();

            if (!_currentZone.HasValue || pinsContainer == null || locationPinPrefab == null) return;

            // Get locations in current zone
            var locations = _mapSystem?.GetLocationsInZone(_currentZone.Value) ?? new List<LocationData>();

            // Apply filter
            if (_currentFilter != LocationType.All)
            {
                locations = locations.Where(l => l.locationType == _currentFilter).ToList();
            }

            // Create pins
            foreach (var location in locations)
            {
                CreateLocationPin(location);
            }
        }

        private void CreateLocationPin(LocationData location)
        {
            var pinObj = Instantiate(locationPinPrefab, pinsContainer);
            _locationPins.Add(pinObj);

            // Position pin based on map coordinates
            var rectTransform = pinObj.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchorMin = location.mapPosition;
                rectTransform.anchorMax = location.mapPosition;
                rectTransform.anchoredPosition = Vector2.zero;
            }

            var controller = pinObj.GetComponent<LocationPinController>();
            if (controller != null)
            {
                controller.SetLocation(location, () => ShowLocationPreview(location));
            }
        }

        #endregion

        #region Filter

        private void SetFilter(LocationType filter)
        {
            _currentFilter = filter;
            _mapSystem?.SetLocationFilter(filter);
            UpdateFilterButtons();
            UpdateLocationPins();
        }

        private void HandleFilterChanged(LocationType filter)
        {
            _currentFilter = filter;
            UpdateFilterButtons();
        }

        private void UpdateFilterButtons()
        {
            SetFilterButtonActive(filterAllButton, _currentFilter == LocationType.All);
            SetFilterButtonActive(filterVenuesButton, _currentFilter == LocationType.Venue);
            SetFilterButtonActive(filterVendorsButton, _currentFilter == LocationType.Vendor);
        }

        private void SetFilterButtonActive(Button button, bool isActive)
        {
            if (button == null) return;
            var img = button.GetComponent<Image>();
            if (img != null) img.color = isActive ? DesignTokens.Accent : DesignTokens.Surface;
        }

        #endregion

        #region Preview Card

        private void ShowLocationPreview(LocationData location)
        {
            _selectedLocationId = location.locationId;

            if (previewCard != null) previewCard.SetActive(true);

            if (previewNameText != null)
            {
                previewNameText.text = location.displayName;
            }

            if (previewTypeText != null)
            {
                previewTypeText.text = location.locationType.ToString();
                previewTypeText.color = location.locationType switch
                {
                    LocationType.Venue => DesignTokens.Accent,
                    LocationType.Vendor => DesignTokens.Money,
                    _ => DesignTokens.TextSecondary
                };
            }

            if (previewDescriptionText != null)
            {
                previewDescriptionText.text = location.description ?? "";
            }

            // Get additional preview data from map system
            var preview = _mapSystem?.GetLocationPreview(location.locationId);
            if (preview != null && previewTierText != null)
            {
                previewTierText.text = preview.tier.ToString();
                previewTierText.gameObject.SetActive(true);
            }
            else if (previewTierText != null)
            {
                previewTierText.gameObject.SetActive(false);
            }

            // Show select button only if there's a callback registered
            if (previewSelectButton != null)
            {
                previewSelectButton.gameObject.SetActive(OnLocationSelected != null);
            }
        }

        private void HidePreviewCard()
        {
            _selectedLocationId = null;
            if (previewCard != null) previewCard.SetActive(false);
        }

        private void SelectCurrentLocation()
        {
            if (_selectedLocationId != null && OnLocationSelected != null)
            {
                OnLocationSelected.Invoke(_selectedLocationId);
                Close();
            }
        }

        #endregion

        #region Public API

        /// <summary>
        /// Open the map for venue selection during event planning.
        /// </summary>
        public void OpenForVenueSelection(System.Action<string> onSelected)
        {
            OnLocationSelected = onSelected;
            SetFilter(LocationType.Venue);
            gameObject.SetActive(true);
            ShowCityOverview();
        }

        /// <summary>
        /// Open the map for vendor selection during event planning.
        /// </summary>
        public void OpenForVendorSelection(System.Action<string> onSelected)
        {
            OnLocationSelected = onSelected;
            SetFilter(LocationType.Vendor);
            gameObject.SetActive(true);
            ShowCityOverview();
        }

        /// <summary>
        /// Open the map for general browsing.
        /// </summary>
        public void OpenForBrowsing()
        {
            OnLocationSelected = null;
            SetFilter(LocationType.All);
            gameObject.SetActive(true);
            ShowCityOverview();
        }

        /// <summary>
        /// Close the map overlay.
        /// </summary>
        public void Close()
        {
            OnLocationSelected = null;
            gameObject.SetActive(false);
        }

        #endregion
    }

    /// <summary>
    /// Controls a single location pin on the map.
    /// </summary>
    public class LocationPinController : MonoBehaviour
    {
        [SerializeField] private Image pinIcon;
        [SerializeField] private Image pinBackground;
        [SerializeField] private Button pinButton;

        private LocationData _location;
        private System.Action _onTap;

        public void SetLocation(LocationData location, System.Action onTap)
        {
            _location = location;
            _onTap = onTap;

            if (pinBackground != null)
            {
                pinBackground.color = location.locationType switch
                {
                    LocationType.Venue => DesignTokens.Accent,
                    LocationType.Vendor => DesignTokens.Money,
                    LocationType.Office => DesignTokens.Reputation,
                    _ => DesignTokens.TextSecondary
                };
            }

            if (pinButton != null)
            {
                pinButton.onClick.RemoveAllListeners();
                pinButton.onClick.AddListener(() => _onTap?.Invoke());
            }
        }
    }
}
