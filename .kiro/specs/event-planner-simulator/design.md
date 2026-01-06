# Design Document

## Overview

This design document describes the technical architecture for Event Planning Simulator, a Unity 6 mobile business simulation game. The design focuses on MVP + Launch-Simple scope (Stages 1-3) as the primary implementation, with Stages 4-5 designed as extension points.

### Design Philosophy

- **Pure Logic Separation**: Core game logic lives in plain C# classes, testable without Unity
- **Progressive Complexity**: Systems start simple in Stage 1, unlock depth through progression
- **Data-Driven Design**: ScriptableObjects define content; code defines behavior
- **Testable Properties**: Each system has verifiable correctness properties

### Scope Priorities

| Priority | Description |
|----------|-------------|
| **Primary** | MVP features, Stage 1 simplified versions |
| **Secondary** | Stage 2-3 full complexity unlocks |
| **Extension** | Stages 4-5 endgame content (designed as hooks, not detailed) |

## Architecture

### System Dependency Graph

```
┌─────────────────────────────────────────────────────────────────┐
│                        Game_Manager                              │
│  (Singleton - coordinates all systems, manages game state)       │
└─────────────────────────────────────────────────────────────────┘
                              │
        ┌─────────────────────┼─────────────────────┐
        ▼                     ▼                     ▼
┌───────────────┐    ┌───────────────┐    ┌───────────────┐
│  Save_System  │    │  Time_System  │    │  UI_Manager   │
│  (Persistence)│    │  (Day/Phase)  │    │  (All UI)     │
└───────────────┘    └───────────────┘    └───────────────┘
        │                     │                     │
        └─────────────────────┼─────────────────────┘
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                    Event_Planning_System                         │
│  (Event lifecycle, inquiries, tasks, vendor/venue booking)       │
└─────────────────────────────────────────────────────────────────┘
        │                     │                     │
        ▼                     ▼                     ▼
┌───────────────┐    ┌───────────────┐    ┌───────────────┐
│ Satisfaction  │    │  Consequence  │    │  Progression  │
│  Calculator   │    │    System     │    │    System     │
│  (Pure Logic) │    │ (Random Evts) │    │ (Stage/Rep)   │
└───────────────┘    └───────────────┘    └───────────────┘
```

### Initialization Order

1. `Save_System` - Load or create save data
2. `Time_System` - Initialize in-game clock
3. `Progression_System` - Set stage, reputation, unlocks
4. `Event_Planning_System` - Load active events, start inquiry generation
5. `Map_System` - Configure visible zones/locations
6. `Phone_System` - Initialize apps with current data
7. `UI_Manager` - Display appropriate screen
8. `Monetization_System` - Initialize Unity Ads/IAP
9. `Audio_Manager` - Start background music

### MonoBehaviour vs Pure C# Split

| Layer | Type | Purpose |
|-------|------|---------|
| **Managers** | MonoBehaviour | Unity lifecycle, coroutines, scene references |
| **Core Logic** | Pure C# | Calculations, rules, state transitions |
| **Data** | ScriptableObject | Content definitions (venues, vendors, events) |
| **Runtime Data** | Serializable C# | Player state, active events, save data |

## Components and Interfaces

### Core Interfaces

```csharp
/// <summary>
/// Pure logic calculator for client satisfaction scores.
/// No Unity dependencies - fully unit testable.
/// </summary>
public interface ISatisfactionCalculator
{
    /// <summary>
    /// Calculate final satisfaction score (0-100) for a completed event.
    /// </summary>
    SatisfactionResult Calculate(EventData eventData, ClientData client);
    
    /// <summary>
    /// Calculate individual category score (0-100).
    /// </summary>
    float CalculateCategoryScore(EventData eventData, BudgetCategory category);
}

/// <summary>
/// Manages event lifecycle from inquiry to results.
/// </summary>
public interface IEventPlanningSystem
{
    /// <summary>
    /// Generate a new client inquiry based on stage and reputation.
    /// </summary>
    ClientInquiry GenerateInquiry(int stage, int reputation);
    
    /// <summary>
    /// Accept an inquiry and create an active event.
    /// </summary>
    EventData AcceptInquiry(ClientInquiry inquiry);
    
    /// <summary>
    /// Get current workload status based on active event count.
    /// </summary>
    WorkloadStatus GetWorkloadStatus(int stage, int activeEventCount);
    
    /// <summary>
    /// Book a vendor for an event, deducting from budget.
    /// </summary>
    BookingResult BookVendor(EventData eventData, VendorData vendor);
    
    /// <summary>
    /// Book a venue for an event, validating capacity.
    /// </summary>
    BookingResult BookVenue(EventData eventData, VenueData venue);
}
```

```csharp
/// <summary>
/// Manages player progression through stages and reputation.
/// </summary>
public interface IProgressionSystem
{
    /// <summary>
    /// Apply reputation change based on event satisfaction.
    /// </summary>
    ReputationChange ApplyEventResult(float satisfaction, int currentReputation);
    
    /// <summary>
    /// Check if player meets requirements for next stage.
    /// </summary>
    bool CanAdvanceStage(PlayerData player);
    
    /// <summary>
    /// Get personality distribution for current stage.
    /// </summary>
    PersonalityDistribution GetPersonalityDistribution(int stage);
    
    /// <summary>
    /// Evaluate Stage 2 performance review.
    /// </summary>
    PerformanceReviewResult EvaluatePerformance(List<EventData> recentEvents);
}

/// <summary>
/// Manages in-game time passage and event scheduling.
/// </summary>
public interface ITimeSystem
{
    /// <summary>
    /// Current in-game date.
    /// </summary>
    GameDate CurrentDate { get; }
    
    /// <summary>
    /// Advance time by specified real-time seconds.
    /// </summary>
    void AdvanceTime(float realTimeSeconds, int stage);
    
    /// <summary>
    /// Get time passage rate for current stage (real minutes per game day).
    /// </summary>
    float GetTimeRate(int stage);
    
    /// <summary>
    /// Schedule an event for a future date based on complexity.
    /// </summary>
    GameDate ScheduleEvent(EventComplexity complexity, GameDate currentDate);
}

/// <summary>
/// Handles random events during event execution.
/// </summary>
public interface IConsequenceSystem
{
    /// <summary>
    /// Evaluate and trigger random events for an executing event.
    /// </summary>
    List<RandomEventResult> EvaluateRandomEvents(EventData eventData, int stage);

    /// <summary>
    /// Calculate satisfaction modifier from random events.
    /// </summary>
    float CalculateRandomEventModifier(List<RandomEventResult> events);

    /// <summary>
    /// Check if contingency can mitigate a random event.
    /// </summary>
    MitigationResult CheckMitigation(RandomEventResult randomEvent, float contingencyBudget);
}

/// <summary>
/// Manages weather forecasting and outdoor event impacts.
/// </summary>
public interface IWeatherSystem
{
    /// <summary>
    /// Get current 7-day forecast.
    /// </summary>
    List<WeatherForecast> GetForecast();

    /// <summary>
    /// Get weather forecast for a specific date.
    /// </summary>
    WeatherForecast GetForecastForDate(GameDate date);

    /// <summary>
    /// Get simplified weather risk for Stage 1 display.
    /// </summary>
    WeatherRisk GetSimplifiedRisk(GameDate date);

    /// <summary>
    /// Advance weather simulation and update forecasts.
    /// Called when game day advances.
    /// </summary>
    void AdvanceDay(GameDate newDate);

    /// <summary>
    /// Check if outdoor event on date has weather risk.
    /// Returns warning level and suggested action.
    /// </summary>
    WeatherWarning CheckOutdoorEventRisk(GameDate eventDate, int stage);
}

/// <summary>
/// Manages push notifications and local reminders.
/// </summary>
public interface INotificationSystem
{
    /// <summary>
    /// Schedule a notification for future delivery.
    /// </summary>
    void ScheduleNotification(NotificationType type, string title, string message, DateTime deliveryTime);

    /// <summary>
    /// Cancel a previously scheduled notification.
    /// </summary>
    void CancelNotification(string notificationId);

    /// <summary>
    /// Cancel all scheduled notifications.
    /// </summary>
    void CancelAllNotifications();

    /// <summary>
    /// Check if notification type is enabled by player.
    /// </summary>
    bool IsNotificationEnabled(NotificationType type);

    /// <summary>
    /// Update notification preferences.
    /// </summary>
    void SetNotificationEnabled(NotificationType type, bool enabled);

    /// <summary>
    /// Schedule event-related notifications (deadline reminders, etc.).
    /// </summary>
    void ScheduleEventNotifications(EventData eventData, GameDate currentDate);
}

/// <summary>
/// Manages player achievements and platform integration.
/// </summary>
public interface IAchievementSystem
{
    /// <summary>
    /// Check if achievement criteria are met and award if so.
    /// </summary>
    void CheckAndAward(AchievementType achievement);

    /// <summary>
    /// Get current progress for a trackable achievement.
    /// </summary>
    AchievementProgress GetProgress(AchievementType achievement);

    /// <summary>
    /// Get all earned achievements.
    /// </summary>
    List<AchievementData> GetEarnedAchievements();

    /// <summary>
    /// Get all available achievements with their status.
    /// </summary>
    List<AchievementData> GetAllAchievements();

    /// <summary>
    /// Sync achievements with platform services (Game Center, Google Play).
    /// </summary>
    void SyncWithPlatform();

    /// <summary>
    /// Increment progress for cumulative achievements.
    /// </summary>
    void IncrementProgress(AchievementType achievement, int amount = 1);
}
```

### Save System Interface

```csharp
/// <summary>
/// Handles persistent game state storage and retrieval.
/// </summary>
public interface ISaveSystem
{
    /// <summary>
    /// Save current game state to persistent storage.
    /// </summary>
    void Save(SaveData data);

    /// <summary>
    /// Load game state from persistent storage.
    /// Returns null if no save exists.
    /// </summary>
    SaveData Load();

    /// <summary>
    /// Check if a save file exists.
    /// </summary>
    bool HasSaveFile { get; }

    /// <summary>
    /// Delete the current save file.
    /// </summary>
    void DeleteSave();

    /// <summary>
    /// Create a backup of the current save before major operations.
    /// </summary>
    void CreateBackup();

    /// <summary>
    /// Restore from the most recent backup.
    /// Returns null if no backup exists.
    /// </summary>
    SaveData RestoreFromBackup();

    /// <summary>
    /// Check if save file is corrupted.
    /// </summary>
    bool ValidateSaveFile();

    /// <summary>
    /// Get the current save file version.
    /// </summary>
    string GetSaveVersion();

    /// <summary>
    /// Migrate save data from an older version to current.
    /// </summary>
    SaveData MigrateSaveData(SaveData oldData, string fromVersion);
}
```

### Map System Interface

```csharp
/// <summary>
/// Manages the city map UI, zone navigation, and location discovery.
/// </summary>
public interface IMapSystem
{
    /// <summary>
    /// Get zones currently visible/unlocked for the player.
    /// </summary>
    List<MapZone> GetVisibleZones(int stage);

    /// <summary>
    /// Get all locations (venues, vendors) in a specific zone.
    /// </summary>
    List<LocationData> GetLocationsInZone(MapZone zone);

    /// <summary>
    /// Navigate to a specific zone, triggering zoom animation.
    /// </summary>
    void NavigateToZone(MapZone zone);

    /// <summary>
    /// Navigate to a specific location within a zone.
    /// </summary>
    void NavigateToLocation(string locationId);

    /// <summary>
    /// Filter visible location pins by type.
    /// </summary>
    void SetLocationFilter(LocationType filter);

    /// <summary>
    /// Get the preview card data for a location.
    /// </summary>
    LocationPreviewData GetLocationPreview(string locationId);

    /// <summary>
    /// Check if a zone is unlocked for the current player.
    /// </summary>
    bool IsZoneUnlocked(MapZone zone);

    /// <summary>
    /// Get the player's current office location.
    /// </summary>
    LocationData GetPlayerOffice();
}

public enum LocationType { All, Venue, Vendor, Office, MeetingPoint }

[Serializable]
public class LocationData
{
    public string locationId;
    public string displayName;
    public LocationType locationType;
    public MapZone zone;
    public Vector2 mapPosition; // Position on map UI
    public string iconId;
}

[Serializable]
public class LocationPreviewData
{
    public string locationId;
    public string displayName;
    public string description;
    public string thumbnailPath;
    public LocationType locationType;
    // Venue-specific
    public int capacity;
    public float pricePerEvent;
    // Vendor-specific
    public VendorTier tier;
    public float rating;
}
```

### Phone System Interface

```csharp
/// <summary>
/// Manages the smartphone UI overlay and app navigation.
/// </summary>
public interface IPhoneSystem
{
    /// <summary>
    /// Open the phone overlay, displaying available apps.
    /// </summary>
    void OpenPhone();

    /// <summary>
    /// Close the phone overlay and return to previous screen.
    /// </summary>
    void ClosePhone();

    /// <summary>
    /// Open a specific app within the phone.
    /// </summary>
    void OpenApp(PhoneApp app);

    /// <summary>
    /// Get notification badge count for an app.
    /// </summary>
    int GetBadgeCount(PhoneApp app);

    /// <summary>
    /// Update badge count for an app.
    /// </summary>
    void SetBadgeCount(PhoneApp app, int count);

    /// <summary>
    /// Check if phone overlay is currently visible.
    /// </summary>
    bool IsPhoneOpen { get; }
}

public enum PhoneApp 
{ 
    Calendar, Messages, Bank, Contacts, Reviews, Tasks, Clients 
}
```

### Tutorial System Interface

```csharp
/// <summary>
/// Manages guided instruction for new players.
/// </summary>
public interface ITutorialSystem
{
    /// <summary>
    /// Start the tutorial sequence for a new player.
    /// </summary>
    void StartTutorial();

    /// <summary>
    /// Advance to the next tutorial step.
    /// </summary>
    void AdvanceStep();

    /// <summary>
    /// Skip the tutorial entirely.
    /// </summary>
    void SkipTutorial();

    /// <summary>
    /// Check if tutorial has been completed.
    /// </summary>
    bool IsTutorialComplete { get; }

    /// <summary>
    /// Get current tutorial step for UI highlighting.
    /// </summary>
    TutorialStep CurrentStep { get; }

    /// <summary>
    /// Highlight specific UI elements for current step.
    /// </summary>
    void HighlightElements(List<string> elementIds);

    /// <summary>
    /// Show contextual tip for a mechanic.
    /// </summary>
    void ShowContextualTip(string tipKey);
}

public enum TutorialStep
{
    Welcome,
    AcceptClient,
    SelectVenue,
    SelectCaterer,
    EventExecution,
    ViewResults,
    Complete
}
```

### Audio Manager Interface

```csharp
/// <summary>
/// Manages background music and sound effects.
/// </summary>
public interface IAudioManager
{
    /// <summary>
    /// Play background music for a screen/context.
    /// </summary>
    void PlayMusic(MusicTrack track);

    /// <summary>
    /// Stop current background music.
    /// </summary>
    void StopMusic();

    /// <summary>
    /// Play a sound effect.
    /// </summary>
    void PlaySFX(SoundEffect sfx);

    /// <summary>
    /// Set music volume (0-1).
    /// </summary>
    void SetMusicVolume(float volume);

    /// <summary>
    /// Set sound effects volume (0-1).
    /// </summary>
    void SetSFXVolume(float volume);

    /// <summary>
    /// Mute/unmute all audio.
    /// </summary>
    void SetMuteAll(bool muted);

    /// <summary>
    /// Pause audio when app loses focus.
    /// </summary>
    void PauseAudio();

    /// <summary>
    /// Resume audio when app regains focus.
    /// </summary>
    void ResumeAudio();
}

public enum MusicTrack { MainMenu, Planning, Execution, Results, Celebration }
public enum SoundEffect { ButtonClick, Success, Failure, Notification, CashRegister, Warning }
```

### Monetization System Interface

```csharp
/// <summary>
/// Handles in-app purchases and advertisements.
/// </summary>
public interface IMonetizationSystem
{
    /// <summary>
    /// Initialize Unity IAP and Unity Ads.
    /// </summary>
    void Initialize();

    /// <summary>
    /// Check if a rewarded ad is available.
    /// </summary>
    bool IsRewardedAdReady(AdPlacement placement);

    /// <summary>
    /// Show a rewarded ad and invoke callback on completion.
    /// </summary>
    void ShowRewardedAd(AdPlacement placement, Action<bool> onComplete);

    /// <summary>
    /// Show an interstitial ad at a natural break point.
    /// </summary>
    void ShowInterstitialAd(Action onComplete);

    /// <summary>
    /// Initiate a purchase for a product.
    /// </summary>
    void PurchaseProduct(string productId, Action<bool> onComplete);

    /// <summary>
    /// Restore previously purchased non-consumable items.
    /// </summary>
    void RestorePurchases(Action<bool> onComplete);

    /// <summary>
    /// Check if player has purchased "No Ads" or is VIP.
    /// </summary>
    bool IsAdFree { get; }

    /// <summary>
    /// Check if player has active VIP subscription.
    /// </summary>
    bool IsVIP { get; }

    /// <summary>
    /// Get remaining cooldown for a rewarded ad placement.
    /// </summary>
    float GetAdCooldown(AdPlacement placement);
}

public enum AdPlacement
{
    EmergencyFunding,
    OvertimeHours,
    RandomEventMitigation,
    TimeSkip
}
```

### Unity Gaming Services Interface

```csharp
/// <summary>
/// Manages Unity Gaming Services integration (Analytics, Remote Config, Cloud Diagnostics).
/// </summary>
public interface IUnityServicesManager
{
    /// <summary>
    /// Initialize Unity Gaming Services with project credentials.
    /// </summary>
    void Initialize();

    /// <summary>
    /// Track an analytics event.
    /// </summary>
    void TrackEvent(string eventName, Dictionary<string, object> parameters = null);

    /// <summary>
    /// Get a remote config value.
    /// </summary>
    T GetRemoteConfig<T>(string key, T defaultValue);

    /// <summary>
    /// Check if user has consented to analytics.
    /// </summary>
    bool HasAnalyticsConsent { get; }

    /// <summary>
    /// Set user analytics consent status.
    /// </summary>
    void SetAnalyticsConsent(bool consented);

    /// <summary>
    /// Log a custom exception for crash reporting.
    /// </summary>
    void LogException(Exception exception);
}
```

### Manager Classes (MonoBehaviour Wrappers)

```csharp
/// <summary>
/// Central game coordinator. Singleton MonoBehaviour.
/// Owns references to all systems and manages game state.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Core System references
    public ISaveSystem SaveSystem { get; private set; }
    public ITimeSystem TimeSystem { get; private set; }
    public IMapSystem MapSystem { get; private set; }
    public IEventPlanningSystem EventPlanningSystem { get; private set; }
    public IProgressionSystem ProgressionSystem { get; private set; }
    public IConsequenceSystem ConsequenceSystem { get; private set; }
    public IWeatherSystem WeatherSystem { get; private set; }

    // UI System references
    public IPhoneSystem PhoneSystem { get; private set; }
    public ITutorialSystem TutorialSystem { get; private set; }
    public IAudioManager AudioManager { get; private set; }

    // Platform System references
    public IMonetizationSystem MonetizationSystem { get; private set; }
    public IUnityServicesManager UnityServices { get; private set; }
    public INotificationSystem NotificationSystem { get; private set; }
    public IAchievementSystem AchievementSystem { get; private set; }

    // Current game state
    public PlayerData CurrentPlayer { get; private set; }
    public GameState State { get; private set; }

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeSystems();
    }

    /// <summary>
    /// Initialize all systems in dependency order.
    /// </summary>
    private void InitializeSystems()
    {
        // 1. Core systems (no dependencies)
        SaveSystem = new SaveSystemImpl();
        TimeSystem = new TimeSystemImpl();
        WeatherSystem = new WeatherSystemImpl();

        // 2. Game systems (depend on core)
        ProgressionSystem = new ProgressionSystemImpl();
        ConsequenceSystem = new ConsequenceSystemImpl();
        EventPlanningSystem = new EventPlanningSystemImpl();
        MapSystem = new MapSystemImpl();

        // 3. UI systems (depend on game systems)
        PhoneSystem = new PhoneSystemImpl();
        TutorialSystem = new TutorialSystemImpl();
        AudioManager = new AudioManagerImpl();

        // 4. Platform systems (can initialize async)
        MonetizationSystem = new MonetizationSystemImpl();
        UnityServices = new UnityServicesManagerImpl();
        NotificationSystem = new NotificationSystemImpl();
        AchievementSystem = new AchievementSystemImpl();

        // Initialize monetization and services
        MonetizationSystem.Initialize();
        UnityServices.Initialize();
    }

    /// <summary>
    /// Load existing save or start new game.
    /// </summary>
    public void StartGame()
    {
        if (SaveSystem.HasSaveFile && SaveSystem.ValidateSaveFile())
        {
            var saveData = SaveSystem.Load();
            CurrentPlayer = saveData.playerData;
            State = GameState.Playing;
        }
        else
        {
            StartNewGame();
        }
    }

    /// <summary>
    /// Initialize a new game with Stage 1 defaults.
    /// </summary>
    public void StartNewGame()
    {
        CurrentPlayer = new PlayerData
        {
            plannerName = "Alex",
            currentStage = BusinessStage.Solo,
            money = 500f,
            reputation = 0,
            unlockedZones = new List<MapZone> { MapZone.Neighborhood }
        };
        State = GameState.Tutorial;
    }
}
```

## Data Models

### GameDate Struct

```csharp
/// <summary>
/// Represents an in-game date. Serializable for save/load support.
/// Uses simplified 30-day months for easier calculations.
/// </summary>
[Serializable]
public struct GameDate : IComparable<GameDate>, IEquatable<GameDate>
{
    public int day;   // 1-30 (simplified month)
    public int month; // 1-12
    public int year;  // Starting year: 1

    public GameDate(int day, int month, int year)
    {
        this.day = day;
        this.month = month;
        this.year = year;
    }

    /// <summary>
    /// Total days since game start for easy comparison.
    /// </summary>
    public int TotalDays => (year - 1) * 360 + (month - 1) * 30 + day;

    /// <summary>
    /// Add days to current date, handling month/year overflow.
    /// </summary>
    public GameDate AddDays(int days)
    {
        int totalDays = TotalDays + days;
        int newYear = (totalDays - 1) / 360 + 1;
        int remainingDays = (totalDays - 1) % 360;
        int newMonth = remainingDays / 30 + 1;
        int newDay = remainingDays % 30 + 1;
        return new GameDate(newDay, newMonth, newYear);
    }

    /// <summary>
    /// Calculate days between two dates.
    /// </summary>
    public static int DaysBetween(GameDate from, GameDate to) => to.TotalDays - from.TotalDays;

    /// <summary>
    /// Check if this date is in the past relative to another date.
    /// </summary>
    public bool IsBefore(GameDate other) => TotalDays < other.TotalDays;

    /// <summary>
    /// Check if this date is in the future relative to another date.
    /// </summary>
    public bool IsAfter(GameDate other) => TotalDays > other.TotalDays;

    public int CompareTo(GameDate other) => TotalDays.CompareTo(other.TotalDays);
    public bool Equals(GameDate other) => TotalDays == other.TotalDays;
    public override bool Equals(object obj) => obj is GameDate other && Equals(other);
    public override int GetHashCode() => TotalDays;

    public static bool operator ==(GameDate left, GameDate right) => left.Equals(right);
    public static bool operator !=(GameDate left, GameDate right) => !left.Equals(right);
    public static bool operator <(GameDate left, GameDate right) => left.TotalDays < right.TotalDays;
    public static bool operator >(GameDate left, GameDate right) => left.TotalDays > right.TotalDays;
    public static bool operator <=(GameDate left, GameDate right) => left.TotalDays <= right.TotalDays;
    public static bool operator >=(GameDate left, GameDate right) => left.TotalDays >= right.TotalDays;

    public override string ToString() => $"Day {day}, Month {month}, Year {year}";

    /// <summary>
    /// Display format for UI (e.g., "Month 3, Day 15").
    /// </summary>
    public string ToDisplayString() => $"Month {month}, Day {day}";
}
```

### Core Enums

```csharp
// Game State
public enum GameState { Loading, MainMenu, Playing, Paused, Tutorial, Results, Settings }

// Business & Career
public enum BusinessStage { Solo = 1, Employee = 2, SmallCompany = 3, Established = 4, Premier = 5 }
public enum CareerPath { None, Entrepreneur, Corporate }
public enum EmployeeLevel { Junior = 1, Planner = 3, Senior = 5 } // Level 1-2 = Junior, 3-4 = Planner, 5 = Senior

// Client & Event
public enum ClientPersonality { EasyGoing, BudgetConscious, Perfectionist, Indecisive, Demanding, Celebrity }
public enum EventStatus { Inquiry, Accepted, Planning, Executing, Completed, Cancelled }
public enum EventComplexity { Low, Medium, High, VeryHigh }
public enum WorkloadStatus { Optimal, Comfortable, Strained, Critical }
public enum EventPhase { Booking, PrePlanning, ActivePlanning, FinalPrep, ExecutionDay, Results }

// Map & Locations
public enum MapZone { Neighborhood, Downtown, Uptown, Waterfront }

// Venue & Vendor
public enum VenueType
{
    // Neighborhood (Stage 1)
    Backyard, CommunityCenter, ParkPavilion,
    // Downtown (Stage 2+)
    Hotel, Restaurant, SmallBallroom,
    // Large (Stage 3+)
    ConventionCenter, ConferenceHotel,
    // Uptown (Stage 4+)
    LuxuryHotel, Estate, Rooftop,
    // Waterfront (Stage 5)
    Beach, GardenEstate
}

public enum VendorCategory
{
    Caterer, Entertainer, Decorator, Photographer,
    Florist, Baker, RentalCompany, AVTechnician,
    Transportation, Security // Post-MVP
}

public enum VendorTier { Budget, Standard, Premium, Luxury }

// Weather
public enum WeatherType { Clear, Cloudy, LightRain, HeavyRain, ExtremeHeat, ExtremeCold }
public enum WeatherRisk { Good, Risky, Bad } // Stage 1 simplified

// Notifications
public enum NotificationType
{
    EventDeadline, TaskDeadline, NewInquiry,
    Referral, FinancialWarning, WeatherAlert
}

// Achievements
public enum AchievementType
{
    // Progression
    FirstSteps, RisingStar, GoingPro, IndustryLeader, EventPlanningMogul,
    // Mastery
    PerfectPlanner, ConsistencyIsKey, ExcellenceStreak, BudgetMaster, VendorWhisperer, WeatherWatcher,
    // Challenge
    PerfectionistsPerfectionist, JugglingAct, CrisisManager, SelfMade, CorporateClimber, CelebrityHandler,
    // Secret
    AboveAndBeyond, FamilyFirst, ComebackKid
}

public enum AchievementCategory { Progression, Mastery, Challenge, Secret }
```

### Player Data

```csharp
[Serializable]
public class PlayerData
{
    // Identity
    public string playerName = "Alex";
    public AvatarData avatar;
    
    // Progression
    public BusinessStage stage = BusinessStage.Solo;
    public CareerPath careerPath = CareerPath.None;
    public int reputation = 0;
    public float money = 500f;
    
    // Stage 2 Employee Data
    public EmployeeData employeeData;
    
    // Tracking
    public List<string> activeEventIds = new();
    public List<string> completedEventIds = new();
    public List<VendorRelationship> vendorRelationships = new();
    
    // Hidden Metrics (not displayed to player)
    public int hiddenAboveAndBeyondCount = 0;
    public int hiddenExploitationCount = 0;
    
    // Emergency Funding
    public int familyHelpUsed = 0; // Max 3 in Stages 1-2
    
    // Unlocks
    public List<MapZone> unlockedZones = new() { MapZone.Neighborhood };
}

[Serializable]
public class EmployeeData
{
    public string companyName = "Premier Events Co.";
    public int employeeLevel = 1; // 1-5
    public int performanceScore = 0;
    public int consecutiveNegativeReviews = 0;
    public int eventsCompletedSinceReview = 0;
    public int activeSideGigs = 0;
    
    public string GetTitle() => employeeLevel switch
    {
        1 or 2 => "Junior Planner",
        3 or 4 => "Planner",
        5 => "Senior Planner",
        _ => "Planner"
    };
    
    public (float basePay, float commission) GetCompensation() => employeeLevel switch
    {
        1 or 2 => (500f, 0.05f),
        3 or 4 => (750f, 0.10f),
        5 => (1000f, 0.15f),
        _ => (500f, 0.05f)
    };
}
```

### Event Data

```csharp
[Serializable]
public class EventData
{
    public string id;
    public string clientId;
    public string clientName;
    public string eventTitle; // "[ClientName]'s [SubCategory]"
    public EventType eventType;
    public string subCategory;
    public EventStatus status = EventStatus.Inquiry;
    public EventPhase phase = EventPhase.Booking;
    
    // Scheduling
    public GameDate eventDate;
    public GameDate acceptedDate;
    
    // Client Info
    public ClientPersonality personality;
    public int guestCount;
    
    // Budget
    public EventBudget budget;
    
    // Assignments
    public string venueId;
    public List<VendorAssignment> vendors = new();
    public List<EventTask> tasks = new();
    
    // Results (populated after execution)
    public EventResults results;
    
    // Flags
    public bool isCompanyEvent; // Stage 2: company vs side gig
    public bool isReferral;
    public string referredByClientName;
}

[Serializable]
public class EventBudget
{
    public float total;
    public float venueAllocation;
    public float cateringAllocation;
    public float entertainmentAllocation;
    public float decorationsAllocation;
    public float staffingAllocation;
    public float contingencyAllocation;
    public float spent;
    
    public float Remaining => total - spent;
    public float OverageAmount => Mathf.Max(0, spent - total);
    public float OveragePercent => total > 0 ? (OverageAmount / total) * 100f : 0f;
}

[Serializable]
public class EventResults
{
    // Category scores (0-100)
    public float venueScore;
    public float foodScore;
    public float entertainmentScore;
    public float decorationScore;
    public float serviceScore;
    public float expectationScore;
    
    // Final calculation
    public float finalSatisfaction; // 0-100, clamped
    public float randomEventModifier = 1f;
    
    // Outcomes
    public float profit;
    public int reputationChange;
    public bool triggeredReferral;
    public string clientFeedback;
    
    // Random events that occurred
    public List<string> randomEventsOccurred = new();
}
```

### Vendor and Venue Data

```csharp
public enum VendorCategory 
{ 
    Caterer, Entertainer, Decorator, Photographer, 
    Florist, Baker, RentalCompany, AVTechnician,
    Transportation, Security // Post-MVP
}

[CreateAssetMenu(fileName = "Vendor", menuName = "EventPlanner/Vendor")]
public class VendorData : ScriptableObject
{
    public string id;
    public string vendorName;
    public VendorCategory category;
    public VendorTier tier;
    public float basePrice;
    
    // Visible attributes
    [Range(1f, 5f)] public float qualityRating;
    public string specialty;
    public MapZone zone;
    
    // Hidden attributes (revealed through relationship)
    [Range(0f, 1f)] public float reliability;
    [Range(0f, 1f)] public float flexibility;
}

[CreateAssetMenu(fileName = "Venue", menuName = "EventPlanner/Venue")]
public class VenueData : ScriptableObject
{
    public string id;
    public string venueName;
    public VenueType venueType;
    public VendorTier tier;
    
    // Capacity
    public int capacityMin;
    public int capacityMax;
    public int capacityComfortable;
    
    // Pricing
    public float basePrice;
    public float pricePerGuest;
    
    // Attributes
    public bool isIndoor;
    public bool hasOutdoorSpace;
    public bool weatherDependent;
    [Range(1f, 5f)] public float ambianceRating;
    
    // Unlock
    public MapZone zone;
    public BusinessStage requiredStage;
}

[Serializable]
public class VendorRelationship
{
    public string vendorId;
    public int timesHired = 0;
    public bool reliabilityRevealed = false; // At level 3
    public bool flexibilityRevealed = false; // At level 5
    public string playerNote = ""; // Player's personal notes about this vendor

    public int RelationshipLevel => timesHired switch
    {
        >= 5 => 5,
        >= 3 => 3,
        >= 1 => 1,
        _ => 0
    };

    /// <summary>
    /// Discount percentage unlocked through relationship (Post-MVP).
    /// Level 3 = 5%, Level 5 = 10%
    /// </summary>
    public float DiscountPercent => RelationshipLevel switch
    {
        >= 5 => 0.10f,
        >= 3 => 0.05f,
        _ => 0f
    };
}
```

### Work Hours and Tasks

```csharp
[Serializable]
public class WorkHoursData
{
    public int dailyCapacity = 8;
    public int hoursUsedToday = 0;
    public int overtimeUsedToday = 0; // Max 2 ads × 4 hours = 8
    
    public int RemainingHours => dailyCapacity + (overtimeUsedToday * 4) - hoursUsedToday;
    public bool CanUseOvertime => overtimeUsedToday < 2;
    
    public void ResetDaily()
    {
        hoursUsedToday = 0;
        overtimeUsedToday = 0;
    }
}

[Serializable]
public class EventTask
{
    public string id;
    public string taskName;
    public string description;
    public TaskStatus status = TaskStatus.Pending;
    public GameDate deadline;
    public int hoursRequired;
    public List<string> dependencyTaskIds = new();
    public string failureConsequence;
    
    // Stage 2 company help
    public bool usedCompanyHelp = false;
}

public enum TaskStatus { Locked, Pending, InProgress, Completed, Failed }
```

### Weather System

```csharp
[Serializable]
public class WeatherForecast
{
    public GameDate date;
    public WeatherType predictedWeather;
    public float accuracy; // 0.7 at 7 days, 0.9 at 2 days, 1.0 day-of
    public WeatherType actualWeather; // Revealed on day-of
    
    /// <summary>
    /// Stage 1 simplified display.
    /// </summary>
    public WeatherRisk GetSimplifiedRisk() => predictedWeather switch
    {
        WeatherType.Clear or WeatherType.Cloudy => WeatherRisk.Good,
        WeatherType.LightRain => WeatherRisk.Risky,
        _ => WeatherRisk.Bad
    };
}

[Serializable]
public class WeatherSystemData
{
    public List<WeatherForecast> forecasts = new(); // 7-day rolling
    public string currentSeason; // Affects probabilities
}

/// <summary>
/// Weather warning for outdoor event booking decisions.
/// </summary>
[Serializable]
public class WeatherWarning
{
    public WeatherRisk riskLevel;
    public string warningMessage;
    public string suggestedAction; // e.g., "Consider indoor backup" or "Book tent rental"
    public float satisfactionPenaltyIfIgnored; // Potential penalty if weather hits
}
```

### Random Event and Consequence Data

```csharp
/// <summary>
/// Types of random events that can occur during event execution.
/// </summary>
public enum RandomEventType
{
    // Vendor Issues
    VendorNoShow, VendorLate, VendorUnderperformance,
    // Equipment Issues
    EquipmentFailure, PowerOutage, AVMalfunction,
    // Guest Issues
    GuestConflict, UnexpectedGuests, GuestInjury,
    // Weather (outdoor events)
    WeatherChange, ExtremeWeather,
    // Client Issues
    LastMinuteChanges, ClientComplaint, BudgetDispute,
    // Positive Events (rare)
    UnexpectedCompliment, MediaCoverage, CelebrityAppearance
}

/// <summary>
/// Result of a random event evaluation during event execution.
/// </summary>
[Serializable]
public class RandomEventResult
{
    public RandomEventType eventType;
    public string eventDescription;
    public float baseSatisfactionImpact; // Negative for problems, positive for bonuses
    public float mitigationCost; // Cost to mitigate via contingency
    public bool canBeMitigated;
    public bool wasMitigated;
    public string mitigationDescription; // What happened if mitigated
    public string failureDescription; // What happened if not mitigated

    /// <summary>
    /// Calculate final satisfaction impact based on mitigation status.
    /// </summary>
    public float GetFinalImpact() => wasMitigated ? baseSatisfactionImpact * 0.25f : baseSatisfactionImpact;
}

/// <summary>
/// Result of attempting to mitigate a random event.
/// </summary>
[Serializable]
public class MitigationResult
{
    public bool canMitigate;
    public float requiredBudget;
    public float availableBudget;
    public string mitigationOption; // Description of what can be done
    public float reducedImpact; // Satisfaction impact if mitigated
}
```

### Achievement Data

```csharp
/// <summary>
/// Progress tracking for a single achievement.
/// </summary>
[Serializable]
public class AchievementProgress
{
    public AchievementType achievementType;
    public int currentProgress;
    public int targetProgress;
    public bool isEarned;
    public long earnedTimeTicks; // DateTime.Ticks when earned

    public DateTime EarnedTime
    {
        get => new DateTime(earnedTimeTicks);
        set => earnedTimeTicks = value.Ticks;
    }

    public float ProgressPercent => targetProgress > 0 ? (float)currentProgress / targetProgress : 0f;
    public bool IsTrackable => targetProgress > 1; // Multi-step achievements show progress
}

/// <summary>
/// Full achievement definition and status.
/// </summary>
[Serializable]
public class AchievementData
{
    public AchievementType achievementType;
    public AchievementCategory category;
    public string displayName;
    public string description;
    public string hiddenDescription; // Shown before earning secret achievements
    public string iconId;
    public int targetProgress; // 1 for one-time achievements
    public bool isSecret;

    // Rewards (Post-MVP)
    public int currencyReward;
    public string unlockId; // Cosmetic or feature unlock

    /// <summary>
    /// Get display description (handles secret achievements).
    /// </summary>
    public string GetDisplayDescription(bool isEarned)
    {
        if (isSecret && !isEarned) return hiddenDescription;
        return description;
    }
}
```

### Event Type and Subcategory Data

```csharp
/// <summary>
/// Definition of an event type with its subcategories.
/// </summary>
[CreateAssetMenu(fileName = "EventType", menuName = "EventPlanner/EventType")]
public class EventTypeData : ScriptableObject
{
    public string eventTypeId;
    public string displayName;
    public EventComplexity complexity;
    public int minBudget;
    public int maxBudget;
    public int minStageRequired;
    public List<string> subCategories;
    public List<VendorCategory> requiredVendors;
    public List<VendorCategory> optionalVendors;
    public float[] recommendedBudgetSplit; // Venue, Catering, Entertainment, Decorations, Staffing, Contingency
}

/// <summary>
/// Generated client inquiry awaiting player response.
/// </summary>
[Serializable]
public class ClientInquiry
{
    public string inquiryId;
    public string clientName;
    public string eventTypeId;
    public string subCategory;
    public string eventDisplayName; // "[clientName]'s [subCategory]"
    public ClientPersonality personality;
    public int budget;
    public int guestCount;
    public GameDate eventDate;
    public List<string> specialRequirements;
    public long createdTimeTicks;
    public long expiresTimeTicks; // 20 minutes after creation

    public DateTime CreatedTime
    {
        get => new DateTime(createdTimeTicks);
        set => createdTimeTicks = value.Ticks;
    }

    public DateTime ExpiresTime
    {
        get => new DateTime(expiresTimeTicks);
        set => expiresTimeTicks = value.Ticks;
    }

    public bool IsExpired => DateTime.UtcNow > ExpiresTime;
    public bool IsReferral; // True if generated from a referral
    public string referredByClientName; // Name of referring client
}
```

### Save Data

```csharp
/// <summary>
/// Serializable booking entry for vendors/venues.
/// Unity's JsonUtility doesn't serialize Dictionary, so we use List instead.
/// </summary>
[Serializable]
public class BookingEntry
{
    public string entityId; // vendorId or venueId
    public List<GameDate> bookedDates = new();

    public BookingEntry() { }
    public BookingEntry(string id) => entityId = id;
}

[Serializable]
public class SaveData
{
    public string saveVersion = "1.0";
    public long lastSavedTimestamp;

    public PlayerData playerData;
    public List<EventData> activeEvents = new();
    public List<EventData> eventHistory = new();
    public WorkHoursData workHours = new();
    public WeatherSystemData weather = new();
    public GameDate currentDate;

    // Booking calendars (serializable list format instead of Dictionary)
    public List<BookingEntry> vendorBookings = new();
    public List<BookingEntry> venueBookings = new();

    // Settings
    public GameSettings settings = new();

    // Referral tracking
    public int excellenceStreak = 0;

    /// <summary>
    /// Helper to find booking dates for a vendor.
    /// </summary>
    public List<GameDate> GetVendorBookedDates(string vendorId)
    {
        var entry = vendorBookings.Find(b => b.entityId == vendorId);
        return entry?.bookedDates ?? new List<GameDate>();
    }

    /// <summary>
    /// Helper to find booking dates for a venue.
    /// </summary>
    public List<GameDate> GetVenueBookedDates(string venueId)
    {
        var entry = venueBookings.Find(b => b.entityId == venueId);
        return entry?.bookedDates ?? new List<GameDate>();
    }

    /// <summary>
    /// Helper to add a vendor booking.
    /// </summary>
    public void AddVendorBooking(string vendorId, GameDate date)
    {
        var entry = vendorBookings.Find(b => b.entityId == vendorId);
        if (entry == null)
        {
            entry = new BookingEntry(vendorId);
            vendorBookings.Add(entry);
        }
        if (!entry.bookedDates.Contains(date))
            entry.bookedDates.Add(date);
    }

    /// <summary>
    /// Helper to add a venue booking.
    /// </summary>
    public void AddVenueBooking(string venueId, GameDate date)
    {
        var entry = venueBookings.Find(b => b.entityId == venueId);
        if (entry == null)
        {
            entry = new BookingEntry(venueId);
            venueBookings.Add(entry);
        }
        if (!entry.bookedDates.Contains(date))
            entry.bookedDates.Add(date);
    }
}

[Serializable]
public class GameSettings
{
    public float musicVolume = 1f;
    public float sfxVolume = 1f;
    public bool muteAll = false;
    public bool showTutorialTips = true;
    public NotificationSettings notifications = new();
    public PrivacySettings privacy = new();
    public AccessibilitySettings accessibility = new();
}

[Serializable]
public class NotificationSettings
{
    public bool eventDeadlines = false; // Default OFF
    public bool taskDeadlines = false;
    public bool newInquiries = false;
    public bool referrals = false;
    public bool financialWarnings = false;
}
```

### Avatar and Settings Data

```csharp
[Serializable]
public class AvatarData
{
    public int faceShapeIndex = 0;
    public int skinToneIndex = 0;
    public int hairStyleIndex = 0;
    public int hairColorIndex = 0;
    public int outfitIndex = 0;
}

[Serializable]
public class PrivacySettings
{
    public bool analyticsEnabled = false; // Default OFF, requires opt-in (GDPR/CCPA)
    public bool crashReportingEnabled = true;
    public DateTime consentTimestamp;
    public bool hasGivenConsent = false;
}

[Serializable]
public class AccessibilitySettings
{
    public TextSize textSize = TextSize.Medium;
    public bool reducedMotion = false;
    public ColorblindMode colorblindMode = ColorblindMode.None;
}

public enum TextSize { Small, Medium, Large }
public enum ColorblindMode { None, Deuteranopia, Protanopia, Tritanopia }
```

### Stage 3 Milestone Data

```csharp
[Serializable]
public class CareerSummaryData
{
    public int totalEventsCompleted;
    public string firstEventName;
    public string highestSatisfactionEventName;
    public float highestSatisfactionScore;
    public float totalMoneyEarned;
    public int currentReputation;
    public DateTime journeyStartDate;
    public DateTime stage3ReachedDate;
}

[Serializable]
public class MilestoneProgress
{
    public bool hasSeenStage3Milestone = false;
    public bool hasChosenPath = false;
    public CareerPath chosenPath = CareerPath.None;
    public bool canSkipMilestoneSequence = false; // True after first playthrough
}
```

### Marketing Data (Post-MVP Extension)

```csharp
public enum MarketingTier { None, Basic, Standard, Premium }
public enum MarketingFocus { General, Corporate, Wedding, Social } // Stage 4+

/// <summary>
/// Marketing system data for Stage 3+ Entrepreneur Path.
/// Post-MVP implementation - stub provided for extension.
/// </summary>
[Serializable]
public class MarketingData
{
    public MarketingTier currentTier = MarketingTier.None;
    public MarketingFocus focus = MarketingFocus.General;
    public GameDate lastPaymentDate;
    public float totalSpent = 0f;
    public int inquiriesGenerated = 0; // For ROI tracking

    /// <summary>
    /// Weekly cost by tier.
    /// </summary>
    public float GetWeeklyCost() => currentTier switch
    {
        MarketingTier.Basic => 500f,
        MarketingTier.Standard => 1500f,
        MarketingTier.Premium => 3000f,
        _ => 0f
    };

    /// <summary>
    /// Inquiry rate modifier (multiplier to base rate).
    /// Note: Stage 3+ entrepreneurs have 0.7x base rate without marketing.
    /// </summary>
    public float GetInquiryRateModifier() => currentTier switch
    {
        MarketingTier.Basic => 1.0f,    // Restores to normal
        MarketingTier.Standard => 1.25f, // +25% above normal
        MarketingTier.Premium => 1.5f,   // +50% above normal
        _ => 0.7f                         // No marketing = 70% of base
    };
}
```

### Monetization Data Models

```csharp
public enum IAPProductType { Consumable, NonConsumable, Subscription }

/// <summary>
/// Defines an in-app purchase product.
/// </summary>
[CreateAssetMenu(fileName = "IAPProduct", menuName = "EventPlanner/IAPProduct")]
public class IAPProductData : ScriptableObject
{
    public string productId;
    public string displayName;
    public string description;
    public IAPProductType productType;
    public float basePrice; // USD, actual price from store
    
    // Consumable rewards
    public int currencyAmount;
    
    // Non-consumable unlocks
    public List<string> unlockIds;
    
    // Subscription benefits
    public bool grantsVIP;
    public bool grantsNoAds;
    public bool grantsPremiumIdleMode;
}

/// <summary>
/// Defines a rewarded ad placement with cooldowns and limits.
/// </summary>
[CreateAssetMenu(fileName = "AdPlacement", menuName = "EventPlanner/AdPlacement")]
public class AdPlacementData : ScriptableObject
{
    public AdPlacement placementType;
    public string unityAdUnitId;
    public float cooldownSeconds = 300f; // 5 minutes default
    public int dailyLimit = 5;
    public string rewardDescription;
    
    // Reward values
    public int currencyReward;
    public int workHoursReward;
    public float timeSkipHours;
}

/// <summary>
/// Serializable ad tracking entry for a single placement.
/// Unity's JsonUtility doesn't serialize Dictionary, so we use List instead.
/// </summary>
[Serializable]
public class AdTrackingEntry
{
    public AdPlacement placement;
    public long lastWatchTimeTicks; // DateTime.Ticks for serialization
    public int dailyWatchCount;

    public AdTrackingEntry() { }
    public AdTrackingEntry(AdPlacement p) => placement = p;

    public DateTime LastWatchTime
    {
        get => new DateTime(lastWatchTimeTicks);
        set => lastWatchTimeTicks = value.Ticks;
    }
}

/// <summary>
/// Runtime tracking for monetization state.
/// </summary>
[Serializable]
public class MonetizationState
{
    public bool hasNoAdsPurchase = false;
    public bool hasVIPSubscription = false;
    public bool hasPremiumIdleMode = false;
    public List<string> purchasedProductIds = new();
    public List<AdTrackingEntry> adTracking = new(); // Replaces Dictionary for serialization
    public long lastDailyResetTicks;

    public DateTime LastDailyReset
    {
        get => new DateTime(lastDailyResetTicks);
        set => lastDailyResetTicks = value.Ticks;
    }

    /// <summary>
    /// Get or create tracking entry for a placement.
    /// </summary>
    private AdTrackingEntry GetOrCreateEntry(AdPlacement placement)
    {
        var entry = adTracking.Find(e => e.placement == placement);
        if (entry == null)
        {
            entry = new AdTrackingEntry(placement);
            adTracking.Add(entry);
        }
        return entry;
    }

    /// <summary>
    /// Check if ad placement is available (not on cooldown, under daily limit).
    /// </summary>
    public bool CanWatchAd(AdPlacement placement, AdPlacementData config)
    {
        var entry = adTracking.Find(e => e.placement == placement);
        if (entry == null) return true;

        // Check daily limit
        if (entry.dailyWatchCount >= config.dailyLimit)
            return false;

        // Check cooldown
        var elapsed = (DateTime.UtcNow - entry.LastWatchTime).TotalSeconds;
        if (elapsed < config.cooldownSeconds)
            return false;

        return true;
    }

    /// <summary>
    /// Record that an ad was watched.
    /// </summary>
    public void RecordAdWatch(AdPlacement placement)
    {
        var entry = GetOrCreateEntry(placement);
        entry.LastWatchTime = DateTime.UtcNow;
        entry.dailyWatchCount++;
    }

    /// <summary>
    /// Reset daily ad counts (call at start of each real day).
    /// </summary>
    public void ResetDailyCounts()
    {
        foreach (var entry in adTracking)
            entry.dailyWatchCount = 0;
        LastDailyReset = DateTime.UtcNow.Date;
    }
}
```

## Correctness Properties

These properties define testable invariants that must hold true throughout the game. Each property maps to specific requirements and can be verified through unit tests.

### Property 1: Save/Load Round Trip
**Requirements:** R2, R27

```
GIVEN any valid SaveData object S
WHEN S is serialized to JSON and deserialized back to S'
THEN S and S' are semantically equivalent
  AND all PlayerData fields match
  AND all EventData fields match
  AND all booking calendars match
```

**Test Approach:** Create SaveData with edge cases (empty lists, max values, special characters in names), serialize/deserialize, deep compare.

### Property 2: Zone Visibility by Stage
**Requirements:** R3

```
GIVEN player at stage N
WHEN Map_System determines visible zones
THEN:
  Stage 1: Only Neighborhood visible
  Stage 2: Neighborhood + employer network locations
  Stage 3+: Neighborhood + Downtown
  Stage 4+: + Uptown
  Stage 5: + Waterfront
```

**Test Approach:** For each stage 1-5, verify `GetVisibleZones(stage)` returns exactly the expected set.

### Property 3: Client Inquiry Completeness
**Requirements:** R5

```
GIVEN a generated ClientInquiry
THEN inquiry contains:
  - Non-empty clientName
  - Valid eventType for current stage
  - Valid subCategory for eventType
  - Budget within eventType range
  - GuestCount > 0
  - Valid personality for current stage
  - EventDate in future
```

**Test Approach:** Generate 100 inquiries per stage, validate all fields against requirements.

### Property 4: Event Creation from Inquiry
**Requirements:** R5

```
GIVEN a valid ClientInquiry I
WHEN player accepts inquiry
THEN EventData E is created where:
  - E.eventTitle == "[I.clientName]'s [I.subCategory]"
  - E.status == EventStatus.Accepted
  - E.budget.total == I.budget
  - E.eventDate == I.eventDate
  - E.personality == I.personality
```

**Test Approach:** Accept inquiry, verify all fields transferred correctly.

### Property 5: Workload Capacity and Penalties
**Requirements:** R5 (criteria 9-18)

```
GIVEN stage N and activeEventCount C
WHEN calculating workload penalties:

Stage 1 (Simplified):
  C <= 3: No penalty, no warning
  C > 3: Warning displayed, no percentage penalty

Stage 2+ (Full System):
  C <= optimal[N]: 0% penalty
  optimal[N] < C <= comfortable[N]: 3% per event over optimal
  comfortable[N] < C <= strained[N]: 7% per event over comfortable + 10% task failure increase
  C > strained[N]: 12% per event over strained + 25% task failure increase + critical warning

WHERE:
  optimal = [2, 4, 6, 10, 15]
  comfortable = [3, 6, 9, 14, 20]
  strained = [5, 8, 12, 18, 25]
```

**Test Approach:** Test boundary conditions for each stage and capacity tier.

### Property 6: Event Title Format
**Requirements:** R6 (criterion 18)

```
GIVEN clientName and subCategory
WHEN event title is generated
THEN title == "{clientName}'s {subCategory}"

EXAMPLES:
  ("Emma", "Princess Theme Birthday") → "Emma's Princess Theme Birthday"
  ("John", "Board Meeting") → "John's Board Meeting"
```

**Test Approach:** Verify format for all event type/subcategory combinations.

### Property 7: Budget Allocation Math
**Requirements:** R7

```
GIVEN EventBudget B
THEN:
  B.Remaining == B.total - B.spent
  B.OverageAmount == max(0, B.spent - B.total)
  B.OveragePercent == (B.OverageAmount / B.total) * 100 when total > 0
```

**Test Approach:** Test with various spent/total combinations including edge cases.

### Property 8: Overage Tolerance by Personality
**Requirements:** R7 (criteria 9-14)

```
GIVEN client personality P and overage percent O
WHEN evaluating budget overage:

Tolerance thresholds:
  EasyGoing: 15%
  BudgetConscious: 0%
  Perfectionist: 5%
  Indecisive: 10%
  Demanding: 5%

IF O <= tolerance[P]: Client absorbs overage, no player penalty
IF O > tolerance[P]: Player must cover overage or client demands payment
IF O > 25%: Event results in net loss
```

**Test Approach:** Test each personality at boundary (tolerance - 1%, tolerance, tolerance + 1%).

### Property 9: Vendor Booking Budget Deduction
**Requirements:** R8

```
GIVEN EventBudget B with category allocation A
AND vendor with price P
WHEN vendor is booked for that category
THEN:
  B.spent increases by P
  Booking succeeds if P <= remaining allocation
  Warning shown if P > remaining allocation (but still allowed)
```

**Test Approach:** Book vendors at various price points relative to allocation.

### Property 10: Venue Capacity Validation
**Requirements:** R9

```
GIVEN venue with capacityComfortable C and event with guestCount G
WHEN venue is selected:
  IF G <= C: Booking succeeds without warning
  IF G > C AND G <= capacityMax: Warning about cramped conditions
  IF G > capacityMax: Booking prevented
```

**Test Approach:** Test guest counts at capacity boundaries.

### Property 11: Work Hours Accumulation and Reset
**Requirements:** R10

```
GIVEN WorkHoursData W with dailyCapacity 8
WHEN tasks are started:
  W.hoursUsedToday increases by task.hoursRequired
  W.RemainingHours == 8 + (overtimeUsedToday * 4) - hoursUsedToday

WHEN new in-game day begins:
  W.hoursUsedToday resets to 0
  W.overtimeUsedToday resets to 0

WHEN overtime ad watched (max 2/day):
  W.overtimeUsedToday increases by 1
  W.RemainingHours increases by 4
```

**Test Approach:** Simulate day progression with various task/overtime combinations.

### Property 12: Time Passage by Stage
**Requirements:** R11

```
GIVEN stage N
WHEN calculating time passage rate:
  Stage 1: 1 game day per 3 real minutes
  Stage 2: 1 game day per 2.5 real minutes
  Stage 3: 1 game day per 2 real minutes
  Stage 4: 1 game day per 1.5 real minutes
  Stage 5: 1 game day per 1 real minute
```

**Test Approach:** Verify `GetTimeRate(stage)` returns correct values.

### Property 13: Satisfaction Weighted Calculation
**Requirements:** R13

```
GIVEN category scores V, F, E, D, S, X (venue, food, entertainment, decoration, service, expectation)
WHEN calculating base satisfaction:
  baseSatisfaction = V*0.20 + F*0.25 + E*0.20 + D*0.15 + S*0.10 + X*0.10
  finalSatisfaction = baseSatisfaction * randomEventModifier
```

**Test Approach:** Verify weighted sum with known inputs.

### Property 14: Satisfaction Clamping
**Requirements:** R13 (criterion 8)

```
GIVEN any calculated satisfaction score S
WHEN finalizing score:
  result = clamp(S, 0, 100)
  
INVARIANT: 0 <= finalSatisfaction <= 100
```

**Test Approach:** Test with extreme inputs (negative modifiers, stacked bonuses).

### Property 15: Reputation Change by Satisfaction
**Requirements:** R14

```
GIVEN final satisfaction S
WHEN calculating reputation change:
  90-100%: +15 to +25, referral triggered
  75-89%: +5 to +14, 50% referral chance
  60-74%: +1 to +4, no referral
  40-59%: -5 to -10
  <40%: -15 to -25, negative review
```

**Test Approach:** Test satisfaction at each boundary (39, 40, 59, 60, 74, 75, 89, 90, 100).

### Property 16: Personality Thresholds
**Requirements:** R15

```
GIVEN client personality P
WHEN determining satisfaction threshold:
  EasyGoing: 50
  BudgetConscious: 60
  Perfectionist: 85
  Indecisive: 65 (Stage 3+)
  Demanding: 80 (Stage 4+)
  Celebrity: varies (Stage 5)
```

**Test Approach:** Verify threshold lookup for each personality.

### Property 17: Employee Compensation by Level
**Requirements:** R16

```
GIVEN employee level L
WHEN calculating compensation:
  Level 1-2 (Junior): $500 base + 5% commission
  Level 3-4 (Planner): $750 base + 10% commission
  Level 5 (Senior): $1000 base + 15% commission
```

**Test Approach:** Verify `GetCompensation()` for each level.

### Property 18: Performance Review Evaluation
**Requirements:** R16

```
GIVEN last 3 company events with:
  avgSatisfaction and onTimeTaskRate

WHEN evaluating performance:
  IF avgSatisfaction >= 70 AND onTimeTaskRate >= 80%: Positive review
  IF avgSatisfaction < 60 OR onTimeTaskRate < 60%: Negative review
  OTHERWISE: Neutral review

Consequences:
  Positive: Progress toward next level
  Negative: Warning issued, consecutive negatives lead to demotion
  2 consecutive negatives: Demote one level (min Level 1)
  3 consecutive negatives at Level 1: Termination, return to Stage 1
```

**Test Approach:** Test boundary conditions for review outcomes.

### Property 19: Vendor Relationship Progression
**Requirements:** R20

```
GIVEN vendor hired N times
WHEN calculating relationship level:
  N >= 5: Level 5 (flexibility revealed)
  N >= 3: Level 3 (reliability revealed)
  N >= 1: Level 1
  N == 0: Level 0

Hidden attributes revealed at Stage 3+ only (invisible in Stages 1-2)
```

**Test Approach:** Verify level calculation and attribute reveal timing.

### Property 20: Referral Probability by Satisfaction
**Requirements:** R23

```
GIVEN final satisfaction S and excellence streak E
WHEN calculating referral chance:
  S >= 95: 80% base chance
  S >= 90: 50% base chance
  S < 90: 0% chance

Streak bonuses:
  E >= 3: +10% chance
  E >= 5: +20% chance (total)

Excellence streak resets when S < 80
```

**Test Approach:** Test referral generation at satisfaction boundaries with various streaks.

### Property 21: Excellence Streak Tracking
**Requirements:** R23

```
GIVEN current excellence streak E and event satisfaction S
WHEN event completes:
  IF S >= 90: E = E + 1
  IF S < 80: E = 0
  IF 80 <= S < 90: E unchanged
```

**Test Approach:** Simulate event sequences and verify streak values.

### Property 22: Weather Forecast Accuracy
**Requirements:** R32

```
GIVEN forecast for date D days in future
WHEN determining accuracy:
  D >= 7: 70% accuracy
  D >= 2: 90% accuracy
  D == 0: 100% accuracy (actual weather revealed)

Stage 1 simplified: Always 100% accuracy, displayed as Good/Risky/Bad
```

**Test Approach:** Verify accuracy values at day boundaries.

### Property 23: Profit Margin Calculation
**Requirements:** R33

```
GIVEN event budget B and satisfaction S
WHEN calculating profit:
  S >= 70: 20-30% of B
  50 <= S < 70: 10-15% of B
  S < 50: Break-even or loss
```

**Test Approach:** Verify profit ranges at satisfaction boundaries.

### Property 24: Family Help Diminishing Returns
**Requirements:** R34

```
GIVEN family help request count N (max 3 in Stages 1-2)
WHEN requesting family help:
  N == 0: Receive $500
  N == 1: Receive $400
  N == 2: Receive $300
  N >= 3: Option disabled

INVARIANT: familyHelpUsed <= 3
```

**Test Approach:** Verify amounts and option availability at each request count.

### Property 25: Overlapping Event Preparation Penalty
**Requirements:** R5 (criterion 17)

```
GIVEN multiple concurrent events with overlapping preparation periods
WHEN calculating task failure probability:
  baseFailureRate + (5% × numberOfOverlappingEvents)

WHERE overlapping events are those with:
  - Active preparation tasks during the same game day
  - Both events in Pre-Planning, Active Planning, or Final Prep phases

EXAMPLE:
  3 events with overlapping prep periods:
  baseFailureRate = 10%
  penalty = 5% × 2 = 10% (first event doesn't count against itself)
  totalFailureRate = 10% + 10% = 20%
```

**Test Approach:** Create scenarios with 1-5 overlapping events and verify failure rate calculations.

### Property 26: Excellence Streak Referral Bonus Application
**Requirements:** R23 (criteria 6-8)

```
GIVEN event satisfaction S and excellence streak E
WHEN calculating final referral chance:

  baseChance = S >= 95 ? 0.80 : (S >= 90 ? 0.50 : 0.00)
  streakBonus = E >= 5 ? 0.20 : (E >= 3 ? 0.10 : 0.00)
  finalChance = min(baseChance + streakBonus, 1.0)

EXAMPLES:
  S=95, E=0: 80% + 0% = 80%
  S=95, E=3: 80% + 10% = 90%
  S=95, E=5: 80% + 20% = 100% (capped)
  S=92, E=5: 50% + 20% = 70%
  S=85, E=5: 0% + 20% = 20% (NO - streak bonus only applies if base > 0)

CORRECTION: Streak bonus only applies when S >= 90:
  S=85, E=5: 0% (no referral possible below 90%)
```

**Test Approach:** Test combinations of satisfaction (85, 90, 95, 100) with streaks (0, 3, 5, 10).

### Property 27: Commission Calculation Formula
**Requirements:** R16 (criterion 3)

```
GIVEN employee level L and event budget B
WHEN calculating employee compensation for company event:

  (basePay, commissionRate) = GetCompensation(L)
  eventProfit = CalculateProfit(B, satisfaction)
  commission = eventProfit × commissionRate
  totalCompensation = basePay + commission

WHERE GetCompensation returns:
  L = 1-2: ($500, 0.05)
  L = 3-4: ($750, 0.10)
  L = 5:   ($1000, 0.15)

EXAMPLE:
  Level 3 planner, $10,000 budget event, 80% satisfaction
  eventProfit = $10,000 × 0.25 = $2,500 (25% margin for 80% satisfaction)
  commission = $2,500 × 0.10 = $250
  totalCompensation = $750 + $250 = $1,000
```

**Test Approach:** Test compensation calculations across all levels with various budget/satisfaction combinations.

### Property 28: Celebrity Reputation Loss Cap
**Requirements:** R15 (criteria 17-19)

```
GIVEN Celebrity event failure with press coverage status P
WHEN calculating reputation loss:

  baseReputationLoss = CalculateBaseLoss(satisfaction) // -15 to -25 for <40%
  multiplier = P == Positive ? 2.0 : (P == Neutral ? 2.5 : 3.0)
  uncappedLoss = baseReputationLoss × multiplier

  APPLY CAP:
  finalLoss = max(uncappedLoss, -50) // Loss cannot exceed -50

EXAMPLES:
  Base loss -25, Negative press: -25 × 3.0 = -75 → capped to -50
  Base loss -15, Positive press: -15 × 2.0 = -30 → no cap needed
  Base loss -20, Neutral press: -20 × 2.5 = -50 → exactly at cap

INVARIANT: Celebrity event reputation loss >= -50 (loss is negative)
```

**Test Approach:** Test worst-case scenarios (minimum satisfaction + negative press) and verify cap is applied.

## UI Animation Specifications

### Touch Feedback Animations

| Interaction | Animation | Duration | Easing |
|-------------|-----------|----------|--------|
| Button Press | Scale to 0.95 | 50ms | EaseOut |
| Button Release | Scale to 1.0 | 100ms | EaseOutBack |
| Toggle Switch | Slide + Color | 200ms | EaseInOut |
| Card Tap | Scale to 0.98 + Highlight | 100ms | EaseOut |

### Screen Transitions

| Transition | Animation | Duration | Notes |
|------------|-----------|----------|-------|
| Push Screen | Slide from right | 300ms | Previous screen slides left |
| Pop Screen | Slide from left | 300ms | Current screen slides right |
| Modal Open | Fade + Scale from 0.9 | 250ms | Background dims to 50% |
| Modal Close | Fade + Scale to 0.9 | 200ms | Background restores |
| Phone Overlay | Slide from bottom | 350ms | Slight bounce at end |

### Success/Failure Feedback

| Event | Animation | Duration | Additional |
|-------|-----------|----------|------------|
| Event Success | Confetti particles + Scale pop | 500ms | Celebratory SFX |
| Achievement Earned | Badge slide-in + Glow | 400ms | Achievement SFX |
| Task Complete | Checkmark draw + Fade | 300ms | Subtle success SFX |
| Error/Warning | Shake (3 cycles) | 300ms | Warning SFX |
| Budget Overage | Red flash + Shake | 400ms | Alert SFX |

### Loading States

| State | Animation | Notes |
|-------|-----------|-------|
| Data Loading | Pulsing opacity (0.5-1.0) | 1s cycle |
| Event Execution | Progress bar + Status text | Real-time updates |
| Save in Progress | Spinning icon | Top-right corner |

### Accessibility Considerations

- All animations respect `AccessibilitySettings.reducedMotion`
- When reduced motion enabled: instant transitions, no particles
- Minimum touch target: 44x44 points
- Color feedback always paired with icon/shape changes

## Error Handling

### Save System Errors

| Error | Detection | Recovery |
|-------|-----------|----------|
| Corrupted save file | JSON parse failure | Notify player, offer new game or backup restore |
| Missing save file | File not found | Initialize new game with defaults |
| Version mismatch | Version field check | Run migration, update version |
| Write failure | IO exception | Retry with exponential backoff, alert on persistent failure |

### Event System Errors

| Error | Detection | Recovery |
|-------|-----------|----------|
| Invalid event state transition | State machine validation | Log error, force to valid state |
| Booking conflict | Calendar check | Prevent booking, suggest alternatives |
| Budget overflow | Allocation sum check | Warn player, allow with consequences |
| Missing required vendor | Event validation | Block event execution, prompt player |

### Monetization Errors

| Error | Detection | Recovery |
|-------|-----------|----------|
| Ad load failure | Unity Ads callback | Hide ad option gracefully |
| Purchase failure | Unity IAP callback | Show error, don't grant items |
| Network timeout | Connection check | Retry with timeout, offline fallback |

### Graceful Degradation

- **Offline Mode**: Game fully playable without network; ads/IAP disabled
- **Missing Assets**: Placeholder sprites/audio; log warning
- **Invalid Data**: Clamp to valid ranges; log anomaly

## Testing Strategy

### Unit Testing (NUnit)

Pure C# logic classes are tested without Unity:

```csharp
[TestFixture]
public class SatisfactionCalculatorTests
{
    private SatisfactionCalculator _calculator;
    
    [SetUp]
    public void Setup() => _calculator = new SatisfactionCalculator();
    
    [Test]
    public void Calculate_AllPerfectScores_Returns100()
    {
        var eventData = CreateEventWithScores(100, 100, 100, 100, 100, 100);
        var client = CreateClient(ClientPersonality.EasyGoing);
        
        var result = _calculator.Calculate(eventData, client);
        
        Assert.AreEqual(100f, result.FinalSatisfaction);
    }
    
    [Test]
    public void Calculate_WeightedCorrectly()
    {
        // Venue=100, Food=0, others=50
        var eventData = CreateEventWithScores(100, 0, 50, 50, 50, 50);
        var result = _calculator.Calculate(eventData, CreateClient());
        
        // Expected: 100*0.20 + 0*0.25 + 50*0.20 + 50*0.15 + 50*0.10 + 50*0.10
        // = 20 + 0 + 10 + 7.5 + 5 + 5 = 47.5
        Assert.AreEqual(47.5f, result.FinalSatisfaction, 0.01f);
    }
}
```

### Property-Based Testing (FsCheck or custom)

For properties that must hold across all valid inputs:

```csharp
[TestFixture]
public class PropertyTests
{
    /// <summary>
    /// Property 1: Save/Load Round Trip
    /// For any valid SaveData, serialize then deserialize produces equivalent data.
    /// </summary>
    [Test]
    public void SaveData_RoundTrip_PreservesAllData()
    {
        // Generate 100 random SaveData instances
        for (int i = 0; i < 100; i++)
        {
            var original = GenerateRandomSaveData();
            var json = JsonUtility.ToJson(original);
            var restored = JsonUtility.FromJson<SaveData>(json);
            
            AssertSaveDataEquivalent(original, restored);
        }
    }
    
    /// <summary>
    /// Property 14: Satisfaction always clamped 0-100
    /// </summary>
    [Test]
    public void Satisfaction_AlwaysClamped_0To100()
    {
        var calculator = new SatisfactionCalculator();
        
        for (int i = 0; i < 100; i++)
        {
            var eventData = GenerateRandomEventData();
            var client = GenerateRandomClient();
            
            var result = calculator.Calculate(eventData, client);
            
            Assert.GreaterOrEqual(result.FinalSatisfaction, 0f);
            Assert.LessOrEqual(result.FinalSatisfaction, 100f);
        }
    }
}
```

### Integration Testing (Unity Test Framework)

Tests requiring Unity runtime:

```csharp
[UnityTest]
public IEnumerator GameManager_InitializesAllSystems_InCorrectOrder()
{
    // Load test scene
    yield return SceneManager.LoadSceneAsync("TestScene");
    
    var gm = GameManager.Instance;
    
    Assert.IsNotNull(gm.SaveSystem);
    Assert.IsNotNull(gm.TimeSystem);
    Assert.IsNotNull(gm.EventPlanningSystem);
    Assert.IsNotNull(gm.ProgressionSystem);
}
```

### Test Coverage Goals

| Component | Target Coverage | Priority |
|-----------|-----------------|----------|
| SatisfactionCalculator | 95% | High |
| ProgressionSystem | 90% | High |
| EventPlanningSystem | 85% | High |
| SaveSystem | 90% | High |
| UI Controllers | 60% | Medium |
| MonoBehaviour Managers | 50% | Low |

### Property Test Configuration

- Minimum 100 iterations per property test
- Each test tagged with: `Feature: event-planner-simulator, Property N: [description]`
- Tests run on CI for every commit

## Extension Points for Stages 4-5

The following systems are designed with extension hooks but not fully implemented in MVP:

### Staff System Hook

```csharp
public interface IStaffSystem
{
    // MVP: Returns empty list (no staff in Stages 1-2)
    // Extension: Full staff management in Stages 3+
    List<StaffData> GetAvailableStaff();
    void AssignStaffToEvent(string staffId, string eventId);
    float GetStaffEfficiencyBonus(string eventId);
}
```

### Marketing System Hook

```csharp
public interface IMarketingSystem
{
    // MVP: Returns 1.0 (no marketing effect)
    // Extension: Marketing tiers affect inquiry rate in Stage 3+
    float GetInquiryRateModifier();
    void SetMarketingTier(MarketingTier tier);
}
```

### Office Progression Hook

```csharp
public interface IOfficeSystem
{
    // MVP: Returns 0 (home office, no bonus)
    // Extension: Office upgrades provide efficiency bonuses
    float GetEfficiencyBonus();
    OfficeType CurrentOffice { get; }
}
```

## Project Structure

```
Assets/
├── Scripts/
│   ├── Core/                    # Pure C# logic (testable without Unity)
│   │   ├── SatisfactionCalculator.cs
│   │   ├── ProgressionCalculator.cs
│   │   ├── WorkloadCalculator.cs
│   │   ├── WeatherGenerator.cs
│   │   └── EconomicsCalculator.cs
│   │
│   ├── Data/                    # Data structures
│   │   ├── Enums.cs
│   │   ├── PlayerData.cs
│   │   ├── EventData.cs
│   │   ├── SaveData.cs
│   │   └── GameDate.cs
│   │
│   ├── Managers/                # MonoBehaviour managers
│   │   ├── GameManager.cs
│   │   ├── SaveManager.cs
│   │   ├── TimeManager.cs
│   │   ├── EventManager.cs
│   │   ├── UIManager.cs
│   │   └── AudioManager.cs
│   │
│   ├── Systems/                 # Game systems
│   │   ├── EventPlanningSystem.cs
│   │   ├── ProgressionSystem.cs
│   │   ├── ConsequenceSystem.cs
│   │   ├── WeatherSystem.cs
│   │   └── NotificationSystem.cs
│   │
│   ├── UI/                      # UI controllers
│   │   ├── Map/
│   │   ├── Phone/
│   │   ├── Event/
│   │   └── Common/
│   │
│   └── Tests/                   # Test assemblies
│       ├── EditMode/            # Unit tests
│       └── PlayMode/            # Integration tests
│
├── ScriptableObjects/
│   ├── Venues/
│   ├── Vendors/
│   ├── EventTypes/
│   └── Config/
│
└── Scenes/
    ├── MainMenu.unity
    ├── Game.unity
    └── Loading.unity
```
