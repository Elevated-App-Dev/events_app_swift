# Design Document: Unity UI Layer

## Overview

This design document describes the technical architecture for implementing the Event Planning Simulator's visual/UI layer in Unity. The backend systems are complete; this document focuses on Unity scenes, prefabs, UI controllers, and wiring to existing systems.

### Design Philosophy

- **Controller Pattern**: Each UI prefab has a corresponding MonoBehaviour controller
- **Event-Driven Updates**: UI subscribes to system events, not polling
- **Separation of Concerns**: Controllers handle UI logic; systems handle game logic
- **Prefab-Based**: Reusable prefabs for consistent UI components

### Integration Points

The UI layer integrates with existing systems via GameManager:

```csharp
// Example: UI Controller accessing systems
public class HUDController : MonoBehaviour
{
    private void Start()
    {
        var gm = GameManager.Instance;
        _timeSystem = gm.TimeSystem;
        _playerData = gm.CurrentPlayer;
        // Subscribe to events
    }
}
```

## Architecture

### Scene Structure

```
Scenes/
├── MainMenu.unity          # Title screen, new/continue game
└── GameplayMain.unity      # All gameplay with overlay system
```

### Canvas Hierarchy (GameplayMain)

```
GameplayCanvas (Screen Space - Overlay, Sort Order 0)
│
├── GameplayLayer (Sort Order 0)
│   ├── HUD
│   │   ├── TopBar (Date, Money, Reputation)
│   │   └── BottomBar (Phone, Map, Settings buttons)
│   ├── EventPlanningPanel (hidden by default)
│   │   ├── ClientInquiryView
│   │   ├── BudgetAllocationView
│   │   ├── VenueSelectionView
│   │   └── VendorSelectionView
│   ├── EventExecutionPanel (hidden by default)
│   └── ResultsPanel (hidden by default)
│
├── PhoneOverlay (Sort Order 10, hidden by default)
│   ├── PhoneFrame
│   ├── HomeScreen (App Grid)
│   └── AppContentArea
│       ├── CalendarAppView
│       ├── MessagesAppView
│       ├── BankAppView
│       ├── ContactsAppView
│       ├── ReviewsAppView
│       ├── TasksAppView
│       └── ClientsAppView
│
├── MapOverlay (Sort Order 20, hidden by default)
│   ├── MapView
│   ├── ZoneButtons
│   ├── LocationPins
│   ├── FilterBar
│   └── PreviewCard
│
├── TutorialOverlay (Sort Order 30, hidden by default)
│   ├── DimBackground
│   ├── HighlightMask
│   ├── InstructionPanel
│   └── TipBubble
│
├── NotificationLayer (Sort Order 40)
│   └── NotificationQueue
│
├── SettingsOverlay (Sort Order 50, hidden by default)
│   └── SettingsPanel
│
├── PauseMenuOverlay (Sort Order 60, hidden by default)
│   └── PausePanel
│
├── MilestoneOverlay (Sort Order 70, hidden by default)
│   ├── CareerSummaryView
│   ├── PathChoiceView
│   ├── NarrativeView
│   └── CreditsView
│
└── LoadingOverlay (Sort Order 80, hidden by default)
    └── LoadingPanel
```

### Prefab Organization

```
Assets/Prefabs/
├── UI/
│   ├── HUD/
│   │   ├── HUD.prefab
│   │   ├── TopBar.prefab
│   │   └── BottomBar.prefab
│   ├── Phone/
│   │   ├── PhoneOverlay.prefab
│   │   ├── AppIcon.prefab
│   │   ├── CalendarApp.prefab
│   │   ├── MessagesApp.prefab
│   │   ├── BankApp.prefab
│   │   ├── ContactsApp.prefab
│   │   ├── ReviewsApp.prefab
│   │   ├── TasksApp.prefab
│   │   └── ClientsApp.prefab
│   ├── Map/
│   │   ├── MapOverlay.prefab
│   │   ├── LocationPin.prefab
│   │   └── PreviewCard.prefab
│   ├── EventPlanning/
│   │   ├── ClientInquiryPanel.prefab
│   │   ├── BudgetAllocationPanel.prefab
│   │   ├── VenueSelectionPanel.prefab
│   │   ├── VendorSelectionPanel.prefab
│   │   ├── VenueCard.prefab
│   │   └── VendorCard.prefab
│   ├── EventExecution/
│   │   ├── EventExecutionPanel.prefab
│   │   └── RandomEventCard.prefab
│   ├── Results/
│   │   └── ResultsPanel.prefab
│   ├── Tutorial/
│   │   ├── TutorialOverlay.prefab
│   │   └── TipBubble.prefab
│   ├── Notifications/
│   │   └── NotificationPopup.prefab
│   ├── Settings/
│   │   ├── PauseMenu.prefab
│   │   └── SettingsPanel.prefab
│   ├── Milestone/
│   │   └── MilestoneOverlay.prefab
│   ├── MainMenu/
│   │   └── MainMenuPanel.prefab
│   └── Common/
│       ├── Button.prefab
│       ├── Slider.prefab
│       ├── Toggle.prefab
│       ├── Card.prefab
│       └── Badge.prefab
```

## Components and Interfaces

### Base UI Controller

```csharp
/// <summary>
/// Base class for all UI controllers providing common functionality.
/// </summary>
public abstract class UIControllerBase : MonoBehaviour
{
    protected GameManager GameManager => GameManager.Instance;

    protected virtual void OnEnable()
    {
        SubscribeToEvents();
    }

    protected virtual void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    protected abstract void SubscribeToEvents();
    protected abstract void UnsubscribeFromEvents();
    protected abstract void RefreshDisplay();
}
```

### HUD Controller

```csharp
/// <summary>
/// Controls the always-visible heads-up display.
/// </summary>
public class HUDController : UIControllerBase
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI dateText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI reputationText;
    [SerializeField] private Button phoneButton;
    [SerializeField] private Button mapButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private GameObject workloadIndicator;

    [Header("Animation")]
    [SerializeField] private float valueAnimationDuration = 0.5f;

    private ITimeSystem _timeSystem;
    private float _displayedMoney;
    private int _displayedReputation;

    protected override void SubscribeToEvents()
    {
        // Subscribe to time changes, money changes, reputation changes
    }

    protected override void UnsubscribeFromEvents() { }

    protected override void RefreshDisplay()
    {
        var player = GameManager.CurrentPlayer;
        dateText.text = _timeSystem.CurrentDate.ToDisplayString();
        UpdateMoneyDisplay(player.money);
        UpdateReputationDisplay(player.reputation);
        UpdateWorkloadIndicator();
    }

    private void UpdateMoneyDisplay(float newMoney)
    {
        // Animate from _displayedMoney to newMoney
        moneyText.text = $"${newMoney:N0}";
    }

    private void UpdateReputationDisplay(int newReputation)
    {
        // Animate with color (green if up, red if down)
        reputationText.text = newReputation.ToString();
    }

    public void OnPhoneButtonPressed()
    {
        // Open PhoneOverlay
        UIManager.Instance.OpenOverlay<PhoneOverlayController>();
    }

    public void OnMapButtonPressed()
    {
        // Open MapOverlay
        UIManager.Instance.OpenOverlay<MapOverlayController>();
    }

    public void OnSettingsButtonPressed()
    {
        // Open PauseMenu
        UIManager.Instance.OpenOverlay<PauseMenuController>();
    }
}
```

### UI Manager

```csharp
/// <summary>
/// Central coordinator for UI overlays and panels.
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Overlay References")]
    [SerializeField] private PhoneOverlayController phoneOverlay;
    [SerializeField] private MapOverlayController mapOverlay;
    [SerializeField] private TutorialOverlayController tutorialOverlay;
    [SerializeField] private PauseMenuController pauseMenu;
    [SerializeField] private SettingsController settingsPanel;
    [SerializeField] private MilestoneOverlayController milestoneOverlay;
    [SerializeField] private NotificationController notificationController;
    [SerializeField] private LoadingController loadingOverlay;

    [Header("Panel References")]
    [SerializeField] private ClientInquiryController clientInquiryPanel;
    [SerializeField] private BudgetAllocationController budgetPanel;
    [SerializeField] private VenueSelectionController venuePanel;
    [SerializeField] private VendorSelectionController vendorPanel;
    [SerializeField] private EventExecutionController executionPanel;
    [SerializeField] private ResultsController resultsPanel;

    private Stack<UIControllerBase> _overlayStack = new();

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void OpenOverlay<T>() where T : UIControllerBase
    {
        // Find overlay of type T, show it, add to stack
    }

    public void CloseTopOverlay()
    {
        if (_overlayStack.Count > 0)
        {
            var overlay = _overlayStack.Pop();
            overlay.gameObject.SetActive(false);
        }
    }

    public void ShowClientInquiry(ClientInquiry inquiry)
    {
        clientInquiryPanel.SetInquiry(inquiry);
        clientInquiryPanel.gameObject.SetActive(true);
    }

    public void TransitionToBudgetAllocation(EventData eventData)
    {
        clientInquiryPanel.gameObject.SetActive(false);
        budgetPanel.SetEvent(eventData);
        budgetPanel.gameObject.SetActive(true);
    }

    // Additional transition methods...

    public void ShowNotification(string title, string message, NotificationType type)
    {
        notificationController.QueueNotification(title, message, type);
    }
}
```

### Phone Overlay Controller

```csharp
/// <summary>
/// Controls the phone overlay and app navigation.
/// </summary>
public class PhoneOverlayController : UIControllerBase
{
    [Header("References")]
    [SerializeField] private GameObject homeScreen;
    [SerializeField] private GameObject appContentArea;
    [SerializeField] private AppIconController[] appIcons;
    [SerializeField] private Button backButton;
    [SerializeField] private Button closeButton;

    [Header("App Views")]
    [SerializeField] private CalendarAppController calendarApp;
    [SerializeField] private MessagesAppController messagesApp;
    [SerializeField] private BankAppController bankApp;
    [SerializeField] private ContactsAppController contactsApp;
    [SerializeField] private ReviewsAppController reviewsApp;
    [SerializeField] private TasksAppController tasksApp;
    [SerializeField] private ClientsAppController clientsApp;

    [Header("Animation")]
    [SerializeField] private float slideAnimationDuration = 0.35f;

    private IPhoneSystem _phoneSystem;
    private PhoneApp? _currentApp;

    protected override void SubscribeToEvents()
    {
        _phoneSystem = GameManager.PhoneSystem;
        // Subscribe to badge count changes
    }

    public void Show()
    {
        gameObject.SetActive(true);
        // Animate slide from bottom
        ShowHomeScreen();
    }

    public void Hide()
    {
        // Animate slide to bottom
        gameObject.SetActive(false);
    }

    private void ShowHomeScreen()
    {
        _currentApp = null;
        homeScreen.SetActive(true);
        appContentArea.SetActive(false);
        backButton.gameObject.SetActive(false);
        RefreshBadges();
    }

    private void RefreshBadges()
    {
        foreach (var icon in appIcons)
        {
            icon.SetBadgeCount(_phoneSystem.GetBadgeCount(icon.App));
        }
    }

    public void OpenApp(PhoneApp app)
    {
        _currentApp = app;
        homeScreen.SetActive(false);
        appContentArea.SetActive(true);
        backButton.gameObject.SetActive(true);

        // Hide all apps, show selected
        HideAllApps();
        GetAppController(app).gameObject.SetActive(true);
        GetAppController(app).Refresh();
    }

    private UIControllerBase GetAppController(PhoneApp app) => app switch
    {
        PhoneApp.Calendar => calendarApp,
        PhoneApp.Messages => messagesApp,
        PhoneApp.Bank => bankApp,
        PhoneApp.Contacts => contactsApp,
        PhoneApp.Reviews => reviewsApp,
        PhoneApp.Tasks => tasksApp,
        PhoneApp.Clients => clientsApp,
        _ => throw new ArgumentOutOfRangeException()
    };

    public void OnBackButtonPressed()
    {
        if (_currentApp.HasValue)
        {
            ShowHomeScreen();
        }
    }

    public void OnCloseButtonPressed()
    {
        Hide();
    }
}
```

### Map Overlay Controller

```csharp
/// <summary>
/// Controls the city map navigation.
/// </summary>
public class MapOverlayController : UIControllerBase
{
    [Header("References")]
    [SerializeField] private RectTransform mapContainer;
    [SerializeField] private Transform locationPinContainer;
    [SerializeField] private PreviewCardController previewCard;
    [SerializeField] private Button[] zoneButtons;
    [SerializeField] private Toggle[] filterToggles;
    [SerializeField] private Button closeButton;

    [Header("Prefabs")]
    [SerializeField] private LocationPinController locationPinPrefab;

    [Header("Zone Sprites")]
    [SerializeField] private Sprite[] zoneSprites;
    [SerializeField] private Sprite[] zoneLockedSprites;

    private IMapSystem _mapSystem;
    private MapZone _currentZone = MapZone.Neighborhood;
    private LocationType _currentFilter = LocationType.All;
    private List<LocationPinController> _activePins = new();

    protected override void SubscribeToEvents()
    {
        _mapSystem = GameManager.MapSystem;
    }

    public void Show()
    {
        gameObject.SetActive(true);
        RefreshZoneButtons();
        ShowZone(_currentZone);
    }

    private void RefreshZoneButtons()
    {
        var visibleZones = _mapSystem.GetVisibleZones(GameManager.CurrentPlayer.stage);
        foreach (MapZone zone in Enum.GetValues(typeof(MapZone)))
        {
            var button = zoneButtons[(int)zone];
            var isUnlocked = visibleZones.Contains(zone);
            // Update button visual state
        }
    }

    public void ShowZone(MapZone zone)
    {
        if (!_mapSystem.IsZoneUnlocked(zone))
        {
            ShowLockedZoneMessage(zone);
            return;
        }

        _currentZone = zone;
        ClearPins();

        var locations = _mapSystem.GetLocationsInZone(zone);
        foreach (var location in locations)
        {
            if (MatchesFilter(location.locationType))
            {
                SpawnPin(location);
            }
        }

        // Animate zoom to zone
    }

    private void SpawnPin(LocationData location)
    {
        var pin = Instantiate(locationPinPrefab, locationPinContainer);
        pin.SetLocation(location);
        pin.OnTapped += ShowPreviewCard;
        _activePins.Add(pin);
    }

    private void ShowPreviewCard(LocationData location)
    {
        var previewData = _mapSystem.GetLocationPreview(location.locationId);
        previewCard.SetData(previewData);
        previewCard.gameObject.SetActive(true);
    }

    public void OnFilterChanged(LocationType filter)
    {
        _currentFilter = filter;
        ShowZone(_currentZone);
    }
}
```

### Event Planning Controllers

```csharp
/// <summary>
/// Controls client inquiry display and acceptance.
/// </summary>
public class ClientInquiryController : UIControllerBase
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI clientNameText;
    [SerializeField] private TextMeshProUGUI eventTypeText;
    [SerializeField] private TextMeshProUGUI budgetText;
    [SerializeField] private TextMeshProUGUI guestCountText;
    [SerializeField] private TextMeshProUGUI eventDateText;
    [SerializeField] private TextMeshProUGUI personalityText;
    [SerializeField] private TextMeshProUGUI expirationText;
    [SerializeField] private TextMeshProUGUI specialRequirementsText;
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button declineButton;

    private ClientInquiry _inquiry;
    private IEventPlanningSystem _eventSystem;

    public void SetInquiry(ClientInquiry inquiry)
    {
        _inquiry = inquiry;
        RefreshDisplay();
    }

    protected override void RefreshDisplay()
    {
        clientNameText.text = _inquiry.clientName;
        eventTypeText.text = _inquiry.eventDisplayName;
        budgetText.text = $"${_inquiry.budget:N0}";
        guestCountText.text = $"{_inquiry.guestCount} guests";
        eventDateText.text = _inquiry.eventDate.ToDisplayString();
        personalityText.text = _inquiry.personality.ToString();
        // Update expiration countdown
    }

    public void OnAcceptPressed()
    {
        var eventData = _eventSystem.AcceptInquiry(_inquiry);
        UIManager.Instance.TransitionToBudgetAllocation(eventData);
    }

    public void OnDeclinePressed()
    {
        gameObject.SetActive(false);
    }
}

/// <summary>
/// Controls budget allocation with sliders.
/// </summary>
public class BudgetAllocationController : UIControllerBase
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI totalBudgetText;
    [SerializeField] private TextMeshProUGUI remainingBudgetText;
    [SerializeField] private Slider venueSlider;
    [SerializeField] private Slider cateringSlider;
    [SerializeField] private Slider entertainmentSlider;
    [SerializeField] private Slider decorationsSlider;
    [SerializeField] private Slider staffingSlider;
    [SerializeField] private Slider contingencySlider;
    [SerializeField] private TextMeshProUGUI[] allocationTexts;
    [SerializeField] private TextMeshProUGUI[] warningTexts;
    [SerializeField] private Button confirmButton;

    private EventData _eventData;

    public void SetEvent(EventData eventData)
    {
        _eventData = eventData;
        totalBudgetText.text = $"${eventData.budget.total:N0}";
        // Initialize sliders with recommended percentages
        RefreshDisplay();
    }

    public void OnSliderChanged()
    {
        // Recalculate allocations
        // Check for warnings
        // Update remaining budget
        RefreshDisplay();
    }

    public void OnConfirmPressed()
    {
        // Apply allocations to _eventData.budget
        UIManager.Instance.TransitionToVenueSelection(_eventData);
    }
}
```

### Notification Controller

```csharp
/// <summary>
/// Manages notification popup queue and display.
/// </summary>
public class NotificationController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NotificationPopup popupPrefab;
    [SerializeField] private Transform popupContainer;

    [Header("Settings")]
    [SerializeField] private float displayDuration = 4f;
    [SerializeField] private float animationDuration = 0.3f;

    private Queue<NotificationData> _notificationQueue = new();
    private NotificationPopup _activePopup;
    private bool _isDisplaying;

    public void QueueNotification(string title, string message, NotificationType type)
    {
        _notificationQueue.Enqueue(new NotificationData { title = title, message = message, type = type });

        if (!_isDisplaying)
        {
            ShowNextNotification();
        }
    }

    private void ShowNextNotification()
    {
        if (_notificationQueue.Count == 0)
        {
            _isDisplaying = false;
            return;
        }

        _isDisplaying = true;
        var data = _notificationQueue.Dequeue();

        _activePopup = Instantiate(popupPrefab, popupContainer);
        _activePopup.SetData(data);
        _activePopup.OnDismissed += OnPopupDismissed;
        _activePopup.Show(animationDuration);

        StartCoroutine(AutoDismissAfterDelay());
    }

    private IEnumerator AutoDismissAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        if (_activePopup != null)
        {
            _activePopup.Dismiss();
        }
    }

    private void OnPopupDismissed()
    {
        if (_activePopup != null)
        {
            Destroy(_activePopup.gameObject);
            _activePopup = null;
        }
        ShowNextNotification();
    }
}
```

## Data Flow

### UI Update Flow

```
Game System Event (e.g., TimeSystem.OnDateChanged)
         │
         ▼
UIController.OnSystemEvent() (subscribed in OnEnable)
         │
         ▼
UIController.RefreshDisplay()
         │
         ▼
Update UI Components (TextMeshPro, Images, etc.)
```

### User Action Flow

```
User Input (Button Press)
         │
         ▼
UIController.OnButtonPressed()
         │
         ▼
GameManager.Instance.{System}.{Method}()
         │
         ▼
System fires event with result
         │
         ▼
UIController.OnSystemEvent() - updates display
```

## Animation Specifications

### Overlay Transitions

| Overlay | Open Animation | Close Animation | Duration |
|---------|---------------|-----------------|----------|
| Phone | Slide from bottom | Slide to bottom | 350ms |
| Map | Fade + Scale from 0.9 | Fade + Scale to 0.9 | 300ms |
| Pause | Fade in + Dim background | Fade out | 250ms |
| Tutorial | Fade in highlights | Fade out | 200ms |
| Notification | Slide from top | Slide to top | 300ms |

### Value Change Animations

| Element | Animation | Duration | Notes |
|---------|-----------|----------|-------|
| Money | Count up/down + Flash | 500ms | Green for gain, red for loss |
| Reputation | Count + Color flash | 500ms | Scale pop on change |
| Progress bars | Smooth fill | 300ms | Eased |
| Badge counts | Scale pop | 200ms | When value changes |

### Button Feedback

```csharp
public class ButtonFeedback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float pressScale = 0.95f;
    [SerializeField] private float pressDuration = 0.05f;
    [SerializeField] private float releaseDuration = 0.1f;

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.DOScale(pressScale, pressDuration).SetEase(Ease.OutQuad);
        // Play button sound
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.DOScale(1f, releaseDuration).SetEase(Ease.OutBack);
    }
}
```

## ScriptableObject Test Data

### Venue Data (Stage 1)

```yaml
# Backyard Venue
id: "venue_backyard_001"
venueName: "Smith Family Backyard"
venueType: Backyard
tier: Budget
capacityMin: 10
capacityMax: 50
capacityComfortable: 30
basePrice: 200
pricePerGuest: 5
isIndoor: false
hasOutdoorSpace: true
weatherDependent: true
ambianceRating: 3.0
zone: Neighborhood
requiredStage: Solo

# Community Center
id: "venue_community_001"
venueName: "Riverside Community Center"
venueType: CommunityCenter
tier: Standard
capacityMin: 20
capacityMax: 100
capacityComfortable: 75
basePrice: 500
pricePerGuest: 8
isIndoor: true
hasOutdoorSpace: false
weatherDependent: false
ambianceRating: 3.5
zone: Neighborhood
requiredStage: Solo
```

### Vendor Data (Sample)

```yaml
# Budget Caterer
id: "vendor_caterer_001"
vendorName: "Mom's Home Cooking"
category: Caterer
tier: Budget
basePrice: 15  # per person
qualityRating: 3.0
specialty: "Comfort Food"
zone: Neighborhood
reliability: 0.85  # hidden
flexibility: 0.7   # hidden

# Standard Entertainer
id: "vendor_entertainer_001"
vendorName: "DJ Mike"
category: Entertainer
tier: Standard
basePrice: 300  # flat fee
qualityRating: 4.0
specialty: "All Occasions"
zone: Neighborhood
reliability: 0.9
flexibility: 0.8
```

## Error Handling

### Null Reference Protection

```csharp
protected override void RefreshDisplay()
{
    if (GameManager == null || GameManager.CurrentPlayer == null)
    {
        Debug.LogWarning($"{GetType().Name}: GameManager or player data is null");
        return;
    }

    // Safe to proceed
}
```

### System Event Safety

```csharp
protected override void SubscribeToEvents()
{
    if (_timeSystem != null)
    {
        _timeSystem.OnDateChanged += HandleDateChanged;
    }
}

protected override void UnsubscribeFromEvents()
{
    if (_timeSystem != null)
    {
        _timeSystem.OnDateChanged -= HandleDateChanged;
    }
}
```

## Accessibility

### Reduced Motion Mode

```csharp
public static class UIAnimations
{
    public static float GetAnimationDuration(float normalDuration)
    {
        if (GameManager.Instance?.Settings?.accessibility?.reducedMotion == true)
        {
            return 0f; // Instant
        }
        return normalDuration;
    }

    public static void AnimateScale(Transform target, float toScale, float duration)
    {
        duration = GetAnimationDuration(duration);
        if (duration <= 0)
        {
            target.localScale = Vector3.one * toScale;
        }
        else
        {
            target.DOScale(toScale, duration);
        }
    }
}
```

### Text Scaling

```csharp
public class AccessibleText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float baseFontSize = 24f;

    private void Start()
    {
        ApplyTextSizeSettings();
    }

    public void ApplyTextSizeSettings()
    {
        var multiplier = GameManager.Instance?.Settings?.accessibility?.textSize switch
        {
            TextSize.Small => 0.85f,
            TextSize.Medium => 1.0f,
            TextSize.Large => 1.25f,
            _ => 1.0f
        };

        text.fontSize = baseFontSize * multiplier;
    }
}
```

## Testing Approach

### UI Integration Tests

```csharp
[UnityTest]
public IEnumerator HUD_UpdatesWhenMoneyChanges()
{
    // Arrange
    var hudController = FindObjectOfType<HUDController>();
    var initialMoney = GameManager.Instance.CurrentPlayer.money;

    // Act
    GameManager.Instance.CurrentPlayer.money += 100;
    yield return null; // Wait for event propagation

    // Assert
    Assert.That(hudController.MoneyText.text, Contains.Substring("100"));
}
```

### Manual Test Checklist

- [ ] All overlays open/close with correct animations
- [ ] Phone apps display correct data
- [ ] Map shows correct zones and pins
- [ ] Event planning flow completes end-to-end
- [ ] Notifications appear and dismiss correctly
- [ ] Settings persist across sessions
- [ ] Tutorial highlights correct elements
- [ ] Reduced motion mode disables animations

## Project Structure

```
Assets/
├── Scripts/
│   ├── UI/
│   │   ├── Core/
│   │   │   ├── UIControllerBase.cs
│   │   │   ├── UIManager.cs
│   │   │   └── UIAnimations.cs
│   │   ├── HUD/
│   │   │   └── HUDController.cs
│   │   ├── Phone/
│   │   │   ├── PhoneOverlayController.cs
│   │   │   ├── AppIconController.cs
│   │   │   ├── CalendarAppController.cs
│   │   │   ├── MessagesAppController.cs
│   │   │   ├── BankAppController.cs
│   │   │   ├── ContactsAppController.cs
│   │   │   ├── ReviewsAppController.cs
│   │   │   ├── TasksAppController.cs
│   │   │   └── ClientsAppController.cs
│   │   ├── Map/
│   │   │   ├── MapOverlayController.cs
│   │   │   ├── LocationPinController.cs
│   │   │   └── PreviewCardController.cs
│   │   ├── EventPlanning/
│   │   │   ├── ClientInquiryController.cs
│   │   │   ├── BudgetAllocationController.cs
│   │   │   ├── VenueSelectionController.cs
│   │   │   └── VendorSelectionController.cs
│   │   ├── EventExecution/
│   │   │   └── EventExecutionController.cs
│   │   ├── Results/
│   │   │   └── ResultsController.cs
│   │   ├── Tutorial/
│   │   │   └── TutorialOverlayController.cs
│   │   ├── Notifications/
│   │   │   └── NotificationController.cs
│   │   ├── Settings/
│   │   │   ├── PauseMenuController.cs
│   │   │   └── SettingsController.cs
│   │   ├── Milestone/
│   │   │   └── MilestoneOverlayController.cs
│   │   ├── MainMenu/
│   │   │   └── MainMenuController.cs
│   │   └── Common/
│   │       ├── ButtonFeedback.cs
│   │       ├── AccessibleText.cs
│   │       └── SafeAreaHandler.cs
│   └── Tests/
│       └── UI/
│           └── UIIntegrationTests.cs
├── Prefabs/
│   └── UI/
│       └── [as described above]
├── Scenes/
│   ├── MainMenu.unity
│   └── GameplayMain.unity
└── ScriptableObjects/
    ├── Venues/
    │   └── [VenueData instances]
    ├── Vendors/
    │   └── [VendorData instances]
    ├── EventTypes/
    │   └── [EventTypeData instances]
    └── Monetization/
        ├── IAPProducts/
        └── AdPlacements/
```
