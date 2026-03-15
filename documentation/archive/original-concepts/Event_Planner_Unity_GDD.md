# Event Planning Simulator
## Game Design Document for Unity (C#)

**Engine:** Unity 2022 LTS or Unity 6
**Platform:** iOS / Android
**Genre:** Business Simulation / Tycoon
**Development Approach:** AI-Assisted (Claude Code)

---

## Table of Contents

1. [Game Overview](#1-game-overview)
2. [Core Gameplay Loop](#2-core-gameplay-loop)
3. [Visual Navigation System](#3-visual-navigation-system)
4. [Planning Phase Mechanics](#4-planning-phase-mechanics)
5. [Decision Consequence System](#5-decision-consequence-system)
6. [Progression System](#6-progression-system)
7. [Event Types & Unlocks](#7-event-types--unlocks)
8. [Data Structures (C#)](#8-data-structures-c)
9. [Unity Project Structure](#9-unity-project-structure)
10. [UI/UX Requirements](#10-uiux-requirements)
11. [MVP Scope](#11-mvp-scope-version-10)
12. [Future Features](#12-future-features-v20)
13. [AI Development Guidelines](#13-ai-development-guidelines)

---

## 1. Game Overview

Event Planning Simulator puts players in the role of an aspiring event planner who starts by organizing small family and school events, then grows their reputation to open a full event planning company.

### Core Pillars

| Pillar | Description |
|--------|-------------|
| **Meaningful Decisions** | Every choice affects outcomes — no optimal path, only trade-offs |
| **Business Growth** | Progress from solo planner to company owner with staff and resources |
| **Visual Exploration** | Navigate a map/phone interface to make decisions, not just menus |
| **Replayability** | Different decisions lead to different outcomes and strategies |

### Target Audience

- Casual to mid-core mobile gamers
- Fans of management/tycoon games
- Players who enjoy games like Two Point Hospital, Pizza Ready, Magic Sort

### Unique Selling Points

- Deeper business simulation than existing wedding/party dress-up games
- Consequence system creates emergent storytelling
- Map-based navigation makes planning feel tangible and immersive
- Modern, polished mobile UI

---

## 2. Core Gameplay Loop

```
┌─────────────────────────────────────────────────────────────┐
│  1. CLIENT INTAKE                                           │
│     Review requests → Accept/Decline → Risk vs Reward       │
└─────────────────────┬───────────────────────────────────────┘
                      ▼
┌─────────────────────────────────────────────────────────────┐
│  2. PLANNING PHASE                                          │
│     Navigate map → Select venue/vendors → Allocate budget   │
└─────────────────────┬───────────────────────────────────────┘
                      ▼
┌─────────────────────────────────────────────────────────────┐
│  3. PREPARATION                                             │
│     Manage tasks → Handle timeline → Contingency planning   │
└─────────────────────┬───────────────────────────────────────┘
                      ▼
┌─────────────────────────────────────────────────────────────┐
│  4. EVENT EXECUTION                                         │
│     Event unfolds → Random events → Crisis response         │
└─────────────────────┬───────────────────────────────────────┘
                      ▼
┌─────────────────────────────────────────────────────────────┐
│  5. RESULTS                                                 │
│     Satisfaction score → Earnings → Reputation change       │
└─────────────────────┬───────────────────────────────────────┘
                      ▼
┌─────────────────────────────────────────────────────────────┐
│  6. GROWTH                                                  │
│     Upgrade business → Unlock content → New opportunities   │
└─────────────────────────────────────────────────────────────┘
```

| Phase | Player Action | Key Decisions |
|-------|---------------|---------------|
| 1. Client Intake | Review and accept/decline event requests | Risk vs reward, workload management |
| 2. Planning | Navigate map to select venues, vendors, services | Budget allocation, quality vs cost |
| 3. Preparation | Manage timeline tasks leading to event | Prioritization, contingency planning |
| 4. Event Execution | Watch event unfold (v2: Diner Dash mini-game) | Crisis response, improvisation |
| 5. Results | Review satisfaction scores and earnings | Learn from outcomes |
| 6. Growth | Upgrade business, unlock new content | Investment priorities |

---

## 3. Visual Navigation System

### 3.1 City Map Interface

A stylized 2D illustrated map with zones that unlock as the player progresses.

| Zone | Locations | Unlock Condition |
|------|-----------|------------------|
| **Neighborhood** | Home office, park, community center, bakery | Start |
| **Downtown** | Hotels, caterer row, rental shops, small office | Stage 3 |
| **Uptown** | Luxury venues, premium vendors | Stage 4 |
| **Waterfront** | Destination venues, exclusive vendors | Stage 5 |

#### Map Interaction Flow

```
1. Tap zone to zoom in
       ↓
2. See location pins (filterable by category)
       ↓
3. Tap pin for preview card
       ↓
4. Tap 'Visit' for full details
       ↓
5. Book, place hold, or hire from detail screen
```

#### Location Types

| Type | Examples | Purpose |
|------|----------|---------|
| Venues | Community Center, Park Pavilion, Hotel Ballroom, Estate | Preview, check availability, book or hold |
| Vendor Shops | Bakery Row, Catering District, Party Supply Store | Browse and hire vendors |
| Services | DJ Studio, Photo Booth Rental, Florist | Book entertainment and extras |
| Your Office | Home Office → Small Office → Agency HQ | Manage business, view stats, hire staff |
| Client Locations | Client's home, Coffee shop | Meet clients, pitch services |
| Conference Room | Rented meeting space, Hotel business center | Formal client meetings, progress updates |
| Virtual Meeting | Video call from any location | Remote client updates, quick check-ins |

### 3.2 Phone Interface

Smartphone UI overlay for managing tasks and communications without leaving current location.

| App | Function | Badge Shows |
|-----|----------|-------------|
| Calendar | View events, deadlines, prep tasks | Upcoming deadlines |
| Messages | Client/vendor communications | Unread messages |
| Contacts | Vendor rolodex with ratings/notes | — |
| Bank | Finances, payments, receive payments | — |
| Reviews | Reputation tracking, testimonials | New reviews |
| Tasks | Event to-do checklist | Pending tasks |
| Clients | Client database and history | New inquiries |
| Camera | Take notes/photos when visiting venues | — (flavor feature) |

---

## 4. Planning Phase Mechanics

### 4.1 Client Intake

New clients appear as notifications or meetings. Each client request includes:

- Event type and theme preferences
- Budget range (fixed or negotiable)
- Guest count
- Date and timeline
- Client personality type
- Special requests or constraints

### 4.2 Client Personality Types

| Type | Behavior | Strategy | Satisfaction Threshold |
|------|----------|----------|------------------------|
| **Easy-Going** | Low expectations, satisfied easily | Good for learning, lower pay | 0.5 |
| **Budget-Conscious** | Scrutinizes costs, wants maximum value | Must justify every expense | 0.6 |
| **Perfectionist** | High standards, notices every detail | Requires premium choices | 0.85 |
| **Indecisive** | Changes mind, unclear requirements | Build in flexibility | 0.65 |
| **Demanding** | Difficult but pays well | High risk, high reward | 0.8 |

### 4.3 Budget Allocation

Players must allocate the client's budget across categories:

| Category | Recommended % | Impact of Underspending |
|----------|---------------|-------------------------|
| Venue | 25-40% | Capacity issues, cramped feeling, ambiance complaints |
| Catering | 25-35% | Food quality/quantity complaints |
| Entertainment | 10-20% | Bored guests, low energy |
| Decorations | 10-20% | Underwhelming atmosphere |
| Staffing | 5-15% | Service problems, chaos |
| Contingency | 5-10% | Can't handle emergencies |

### 4.4 Vendor Selection

Each vendor has visible and hidden attributes:

| Attribute | Visibility | Description |
|-----------|------------|-------------|
| Price | Visible | Base cost |
| Quality Rating | Visible | 1-5 stars |
| Style/Specialty | Visible | e.g., "Kid-friendly", "Upscale" |
| Availability | Visible | Booked dates |
| Reliability | Hidden (revealed over time) | Chance of issues |
| Flexibility | Hidden (revealed over time) | Handles last-minute changes |
| Hidden Fees | Hidden | Surprise costs |

**Vendor Tiers:**
- Budget: Lower cost, lower quality, less reliable
- Standard: Balanced option
- Premium: Higher cost, higher quality, more reliable
- Luxury: Exclusive, expensive, top-tier (later stages)

---

## 5. Decision Consequence System

### 5.1 Consequence Matrix

| Decision | Possible Outcomes | Probability Factors |
|----------|-------------------|---------------------|
| Hire cheap caterer | Fine / Food runs out / Quality complaints | Caterer reliability, guest count |
| Skip backup entertainment | OK if main shows / Disaster if no-show | Entertainment reliability |
| No contingency budget | Fine if no problems / Can't handle crises | Random event chance |
| Outdoor venue, no tent | Beautiful day / Rain ruins event | Season, weather RNG |
| Rush booking | Available / Booked / Premium price | Lead time |
| Overbook schedule | Complete all / Quality suffers / Miss deadline | Staff count, event complexity |
| Ignore client preferences | They don't notice / Major dissatisfaction | Client personality type |

### 5.2 Satisfaction Calculation

```csharp
// Pseudocode for satisfaction calculation
float CalculateSatisfaction(EventData eventData, ClientData client) {
    float venueScore = CalculateVenueScore(eventData);      // Weight: 20%
    float foodScore = CalculateFoodScore(eventData);        // Weight: 25%
    float entertainmentScore = CalculateEntertainment();    // Weight: 20%
    float decorationScore = CalculateDecorations();         // Weight: 15%
    float serviceScore = CalculateService();                // Weight: 10%
    float expectationScore = CalculateExpectations();       // Weight: 10%

    float baseScore = (venueScore * 0.20f) +
                      (foodScore * 0.25f) +
                      (entertainmentScore * 0.20f) +
                      (decorationScore * 0.15f) +
                      (serviceScore * 0.10f) +
                      (expectationScore * 0.10f);

    // Apply random event modifiers
    baseScore *= randomEventModifier;

    return Mathf.Clamp01(baseScore);
}
```

### 5.3 Reputation Impact

| Satisfaction | Rep Change | Effects |
|--------------|------------|---------|
| 90-100% | +15 to +25 | Client referral, bonus tip |
| 75-89% | +5 to +14 | 50% referral chance |
| 60-74% | +1 to +4 | Neutral |
| 40-59% | -5 to -10 | No referral, won't return |
| Below 40% | -15 to -25 | Negative review, vendor relations hurt |

### 5.4 Random Events

Events that can occur during execution, modified by player preparation:

| Event | Trigger | Mitigation |
|-------|---------|------------|
| Vendor no-show | Low reliability vendor | Backup plans, reliable vendors |
| Weather problems | Outdoor venue | Indoor backup, tent rental |
| Equipment failure | Random | Contingency budget, quality rentals |
| Guest conflicts | Random | Good seating plan, experienced staff |
| Client last-minute changes | Indecisive personality | Contingency budget, flexible vendors |

---

## 6. Progression System

Players progress through 5 distinct business stages, each unlocking new mechanics and content.

### Stage 1: Solo Side Hustle

| Aspect | Details |
|--------|---------|
| **Location** | Work from home |
| **Capacity** | 1 event at a time |
| **Event Types** | Kids' birthdays, small family gatherings, school events |
| **Map Zones** | Neighborhood only |
| **Vendors** | Local, budget-friendly only |
| **Goal** | Reputation 10 + $2,000 savings |
| **Unlock** | Get hired by an established event planning company |

### Stage 2: Company Employee

| Aspect | Details |
|--------|---------|
| **Location** | Work for "Premier Events Co." as junior planner |
| **Capacity** | 1-2 events with company support and oversight |
| **Event Types** | Add engagement parties, corporate meetings, milestone birthdays |
| **Map Zones** | Access to company's locations |
| **Vendors** | Access to company's vendor network (better rates, reliable contacts) |
| **Pay Structure** | Salary + commission instead of keeping full profit |
| **New Mechanics** | Performance reviews, learning from senior planners, company reputation affects your events |
| **Goal** | Employee level 5 + $10,000 savings + Personal reputation 20 |
| **Unlock** | Option to leave and start your own company (or stay for stability) |

### Stage 3: Small Company (Your Own Business)

| Aspect | Details |
|--------|---------|
| **Location** | Small office (+10% efficiency) |
| **Capacity** | 2 events simultaneously |
| **Staff** | Hire first assistant |
| **Event Types** | Add baby showers, anniversary parties, small corporate events |
| **Map Zones** | + Downtown |
| **Vendors** | Standard tier unlocks, keep contacts from employee days (loyalty rates) |
| **Goal** | Reputation 30 + $30,000 savings |
| **Unlock** | Stage 4: Established Agency |

### Stage 4: Established Agency

| Aspect | Details |
|--------|---------|
| **Location** | Agency office (+20% efficiency) |
| **Capacity** | 3-4 events simultaneously |
| **Staff** | Multiple specialists (coordinator, designer, vendor manager) |
| **Event Types** | Add weddings, galas, conferences |
| **Map Zones** | + Uptown |
| **Vendors** | Premium tier unlocks |
| **Goal** | Reputation 60 + $100,000 savings |
| **Unlock** | Stage 5: Premier Company |

### Stage 5: Premier Event Company

| Aspect | Details |
|--------|---------|
| **Location** | Flagship office/showroom (+30% efficiency) |
| **Capacity** | 5+ events simultaneously |
| **Staff** | Large team with department heads |
| **Event Types** | Add festivals, product launches, destination events |
| **Map Zones** | + Waterfront |
| **Vendors** | Luxury/Exclusive venue and vendor partnerships |
| **Clients** | Celebrity and high-profile clients |
| **Gameplay** | Delegate most work, focus on key decisions |

---

## 7. Event Types & Unlocks

| Event Type | Unlock Stage | Complexity | Budget Range |
|------------|--------------|------------|--------------|
| Kids' Birthday | Stage 1 | Low | $500 - $2,000 |
| Family Gathering | Stage 1 | Low | $300 - $1,500 |
| School Event | Stage 1 | Low-Med | $1,000 - $3,000 |
| Adult Birthday | Stage 1+ | Medium | $1,000 - $5,000 |
| Engagement Party | Stage 2 | Medium | $2,000 - $8,000 |
| Corporate Meeting | Stage 2 | Medium | $3,000 - $15,000 |
| Milestone Birthday | Stage 2 | Medium | $2,000 - $6,000 |
| Baby Shower | Stage 3 | Medium | $1,000 - $4,000 |
| Anniversary Party | Stage 3 | Medium | $2,000 - $10,000 |
| Small Corporate Event | Stage 3 | Medium | $5,000 - $20,000 |
| Small Wedding | Stage 4 | High | $10,000 - $30,000 |
| Charity Gala | Stage 4 | High | $15,000 - $50,000 |
| Conference | Stage 4 | High | $20,000 - $100,000 |
| Large Wedding | Stage 5 | Very High | $30,000 - $150,000 |
| Product Launch | Stage 5 | Very High | $50,000 - $200,000 |
| Festival | Stage 5 | Very High | $100,000+ |

---

## 8. Data Structures (C#)

### 8.1 Core Enums

```csharp
public enum BusinessStage
{
    Solo,
    Employee,
    SmallCompany,
    Established,
    Premier
}

public enum VenueType
{
    Backyard,
    CommunityCenter,
    ParkPavilion,
    Restaurant,
    Hotel,
    Ballroom,
    Estate,
    Rooftop,
    Garden,
    Beach
}

public enum CommunicationStyle
{
    Responsive,
    Slow,
    Micromanager
}

public enum EventType
{
    KidsBirthday,
    FamilyGathering,
    SchoolEvent,
    AdultBirthday,
    EngagementParty,
    CorporateMeeting,
    MilestoneBirthday,
    BabyShower,
    AnniversaryParty,
    SmallCorporateEvent,
    SmallWedding,
    CharityGala,
    Conference,
    LargeWedding,
    ProductLaunch,
    Festival
}

public enum EventStatus
{
    Inquiry,
    Accepted,
    Planning,
    Executing,
    Completed,
    Cancelled
}

public enum ClientPersonality
{
    EasyGoing,
    BudgetConscious,
    Perfectionist,
    Indecisive,
    Demanding
}

public enum VendorCategory
{
    Catering,
    Entertainment,
    Decorations,
    Photography,
    Florist,
    Rentals,
    Staffing
}

public enum VendorTier
{
    Budget,
    Standard,
    Premium,
    Luxury
}

public enum MapZone
{
    Neighborhood,
    Downtown,
    Uptown,
    Waterfront
}

public enum TaskStatus
{
    Locked,
    Pending,
    InProgress,
    Completed,
    Failed
}
```

### 8.2 Client Data

```csharp
[System.Serializable]
public class ClientData
{
    public string id;
    public string clientName;
    public Sprite portrait;
    public ClientPersonality personality;
    public CommunicationStyle communicationStyle;

    [Range(0f, 1f)]
    public float satisfactionThreshold; // Minimum score to be satisfied

    [Range(0f, 1f)]
    public float budgetFlexibility; // How much they'll negotiate

    public int relationshipLevel; // Increases with successful events

    [Range(0f, 1f)]
    public float referralChance; // Chance to refer new clients

    public List<string> completedEventIds;
}
```

### 8.3 Event Data

```csharp
[System.Serializable]
public class EventData
{
    public string id;
    public string clientId;
    public string eventName;
    public EventType eventType;
    public EventStatus status;
    public DateTime eventDate;
    public int guestCount;
    public EventBudget budget;
    public string venueId;
    public List<VendorAssignment> vendors;
    public List<EventTask> tasks;
    public EventRequirements requirements;
    public EventResults results;
}

[System.Serializable]
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

    public float GetAllocationPercentage(string category)
    {
        // Returns percentage of total for given category
    }
}

[System.Serializable]
public class EventRequirements
{
    public bool indoorRequired;
    public bool outdoorBackupNeeded;
    public List<string> dietaryRestrictions;
    public bool accessibilityRequired;
    public string themePreference;
    public List<string> mustHaveServices;
}

[System.Serializable]
public class EventResults
{
    public float venueScore;
    public float foodScore;
    public float entertainmentScore;
    public float decorationScore;
    public float serviceScore;
    public float expectationScore;
    public float finalSatisfaction;
    public float profit;
    public int reputationChange;
    public List<string> randomEventsOccurred;
    public string clientFeedback;
}

[System.Serializable]
public class VendorAssignment
{
    public string vendorId;
    public VendorCategory category;
    public float agreedPrice;
    public bool confirmed;
}

[System.Serializable]
public class EventTask
{
    public string id;
    public string taskName;
    public string description;
    public TaskStatus status;
    public DateTime deadline;
    public int hoursRequired;
    public string assignedToStaffId; // null = player
    public List<string> dependencyTaskIds;
    public string failureConsequence;
}
```

### 8.4 Vendor Data

```csharp
[CreateAssetMenu(fileName = "Vendor", menuName = "EventPlanner/Vendor")]
public class VendorData : ScriptableObject
{
    public string id;
    public string vendorName;
    public string specialty;
    public string description;
    public Sprite icon;
    public Sprite[] photos;

    public VendorCategory category;
    public VendorTier tier;
    public float basePrice;

    [Range(1f, 5f)]
    public float qualityRating; // Visible to player

    [Range(0f, 1f)]
    public float reliability; // Hidden initially

    [Range(0f, 1f)]
    public float flexibility; // Hidden initially

    [Range(0f, 1f)]
    public float hiddenFeesChance; // Hidden

    public MapZone zone;
    public Vector2 mapPosition;

    public UnlockRequirement unlockRequirement;
}
```

### 8.5 Venue Data

```csharp
[CreateAssetMenu(fileName = "Venue", menuName = "EventPlanner/Venue")]
public class VenueData : ScriptableObject
{
    public string id;
    public string venueName;
    public VenueType venueType;
    public string address;
    public string description;
    public Sprite[] photos;

    public VendorTier tier;
    public int capacityMin;
    public int capacityMax;
    public int capacityComfortable;

    public float basePrice;
    public float pricePerGuest;

    public List<string> amenities; // "kitchen", "av_equipment", "parking"
    public bool isIndoor;
    public bool hasOutdoorSpace;
    public bool weatherDependent;

    [Range(1f, 5f)]
    public float ambianceRating;

    [Range(1f, 5f)]
    public float accessibilityRating;

    public List<string> restrictions; // "no_alcohol", "noise_curfew"
    public List<DateTime> availability; // Booked dates

    public MapZone zone;
    public Vector2 mapPosition;

    public UnlockRequirement unlockRequirement;
}
```

### 8.6 Player Data

```csharp
[System.Serializable]
public class PlayerData
{
    public string playerName;
    public string businessName;
    public BusinessStage stage;
    public int reputation;
    public float money;

    public OfficeData office;
    public List<StaffData> staff;

    public List<string> activeEventIds;
    public List<string> completedEventIds;

    public List<VendorRelationship> vendorRelationships;
    public List<EventType> unlockedEventTypes;
    public List<MapZone> unlockedZones;
    public List<string> unlockedVenueIds;
    public List<string> unlockedVendorIds;

    // Only used during EMPLOYEE stage
    public EmployerData employer;

    public PlayerStats stats;
}

/// <summary>
/// Data structure for the Company Employee stage (Stage 2).
/// Tracks the player's relationship with their employer.
/// </summary>
[System.Serializable]
public class EmployerData
{
    public string companyName; // e.g., "Premier Events Co."
    public int companyReputation; // Affects event difficulty and client expectations

    public List<string> vendorNetworkIds; // Pre-approved vendors with discounts
    public float salary; // Base pay per event
    public float commissionRate; // Percentage of profit as bonus

    public int employeeLevel; // Player's rank within company (1-5)
    public string mentorId; // Senior planner who provides tips/bonuses

    public int performanceScore; // Accumulated from performance reviews
}

[System.Serializable]
public class PlayerStats
{
    public int totalEventsCompleted;
    public int successfulEvents;
    public int failedEvents;
    public float totalRevenue;
    public float totalProfit;
    public float averageSatisfaction;
    public int clientsServed;
    public int referralsReceived;
}

[System.Serializable]
public class VendorRelationship
{
    public string vendorId;
    public int relationshipLevel;
    public float discountRate;
    public bool hiddenAttributesRevealed;
    public int timesHired;
}

[System.Serializable]
public class StaffData
{
    public string id;
    public string staffName;
    public Sprite portrait;
    public StaffRole role;
    public float salary;

    [Range(1f, 5f)]
    public float skillLevel;

    public List<EventType> specialties;

    [Range(0f, 1f)]
    public float efficiency;

    [Range(0f, 1f)]
    public float reliability;

    public string assignedEventId;
}

public enum StaffRole
{
    Assistant,
    Coordinator,
    Designer,
    VendorManager,
    SeniorPlanner
}
```

### 8.7 Map Location Data

```csharp
[CreateAssetMenu(fileName = "MapLocation", menuName = "EventPlanner/MapLocation")]
public class MapLocationData : ScriptableObject
{
    public string id;
    public string locationName;
    public LocationType locationType;
    public MapZone zone;
    public Vector2 mapPosition;
    public Sprite icon;
    public Sprite previewImage;
    public UnlockRequirement unlockRequirement;
    public string linkedEntityId; // venueId or vendorId
}

public enum LocationType
{
    Venue,
    Vendor,
    Office,
    ClientMeeting
}

[System.Serializable]
public class UnlockRequirement
{
    public BusinessStage requiredStage;
    public int requiredReputation;

    public bool IsMet(PlayerData player)
    {
        return player.stage >= requiredStage &&
               player.reputation >= requiredReputation;
    }
}
```

### 8.8 Save Data

```csharp
[System.Serializable]
public class SaveData
{
    public string saveVersion;
    public DateTime lastSaved;
    public PlayerData playerData;
    public List<EventData> activeEvents;
    public List<EventData> eventHistory;
    public GameSettings settings;

    // For vendor availability tracking
    public Dictionary<string, List<DateTime>> vendorBookings;
    public Dictionary<string, List<DateTime>> venueBookings;
}
```

---

## 9. Unity Project Structure

```
Assets/
├── Scripts/
│   ├── Core/                           # Pure C# logic (no MonoBehaviour)
│   │   ├── SatisfactionCalculator.cs
│   │   ├── ConsequenceSystem.cs
│   │   ├── ProgressionManager.cs
│   │   ├── EconomyCalculator.cs
│   │   ├── RandomEventResolver.cs
│   │   └── SaveSystem.cs
│   │
│   ├── Data/                           # Data structures
│   │   ├── Enums.cs
│   │   ├── ClientData.cs
│   │   ├── EventData.cs
│   │   ├── VendorData.cs
│   │   ├── VenueData.cs
│   │   ├── PlayerData.cs
│   │   └── SaveData.cs
│   │
│   ├── Managers/                       # MonoBehaviour managers
│   │   ├── GameManager.cs
│   │   ├── MapManager.cs
│   │   ├── PhoneManager.cs
│   │   ├── EventPlanningManager.cs
│   │   ├── UIManager.cs
│   │   ├── AudioManager.cs
│   │   └── AnalyticsManager.cs
│   │
│   ├── UI/                             # UI controllers
│   │   ├── Map/
│   │   │   ├── MapController.cs
│   │   │   ├── LocationPinUI.cs
│   │   │   ├── LocationPreviewCard.cs
│   │   │   └── ZoneController.cs
│   │   ├── Phone/
│   │   │   ├── PhoneController.cs
│   │   │   ├── CalendarAppUI.cs
│   │   │   ├── MessagesAppUI.cs
│   │   │   ├── BankAppUI.cs
│   │   │   └── TasksAppUI.cs
│   │   ├── Event/
│   │   │   ├── ClientIntakeUI.cs
│   │   │   ├── BudgetAllocationUI.cs
│   │   │   ├── VendorSelectionUI.cs
│   │   │   └── ResultsScreenUI.cs
│   │   └── Common/
│   │       ├── ButtonAnimator.cs
│   │       ├── PopupManager.cs
│   │       └── NotificationBadge.cs
│   │
│   ├── Systems/                        # Game systems
│   │   ├── TutorialSystem.cs
│   │   ├── NotificationSystem.cs
│   │   └── AchievementSystem.cs
│   │
│   └── Debug/                          # Development tools
│       ├── DebugConsole.cs
│       ├── CheatCommands.cs
│       └── BalanceSimulator.cs
│
├── ScriptableObjects/
│   ├── Venues/
│   ├── Vendors/
│   ├── Clients/
│   ├── EventTemplates/
│   └── GameConfig/
│
├── Prefabs/
│   ├── UI/
│   │   ├── LocationPin.prefab
│   │   ├── PreviewCard.prefab
│   │   ├── VendorCard.prefab
│   │   └── TaskItem.prefab
│   ├── Map/
│   └── Common/
│
├── Art/
│   ├── Sprites/
│   │   ├── Map/
│   │   ├── Icons/
│   │   ├── Characters/
│   │   └── Venues/
│   ├── UI/
│   └── Fonts/
│
├── Audio/
│   ├── Music/
│   └── SFX/
│
├── Scenes/
│   ├── MainMenu.unity
│   ├── Game.unity
│   └── Loading.unity
│
└── Resources/
    └── (Runtime-loaded assets)
```

---

## 10. UI/UX Requirements

### 10.1 Visual Style

| Element | Requirement |
|---------|-------------|
| **Overall Feel** | Modern, clean, "juicy" mobile UI |
| **Color Palette** | Bright, friendly, professional |
| **Typography** | Clear, readable, mobile-optimized |
| **Icons** | Consistent style, immediately recognizable |
| **Animations** | Smooth transitions, satisfying feedback |

### 10.2 UI Animation Requirements

Every interactive element should have:

- **Press feedback** — Scale down slightly on touch
- **Release feedback** — Bounce back with overshoot
- **Transitions** — Slide/fade between screens (300-400ms)
- **Success feedback** — Particles, scale pop, sound
- **Error feedback** — Shake, red flash

### 10.3 Touch Targets

- Minimum touch target: **44x44 points**
- Comfortable spacing between interactive elements
- Clear visual affordances for tappable items

### 10.4 Responsive Design

Must work on:
- iPhone SE (small)
- iPhone 15 Pro Max (large)
- iPad
- Various Android aspect ratios (16:9, 19.5:9, 20:9)

### 10.5 Recommended Asset Store Packages

| Package | Purpose |
|---------|---------|
| **DOTween Pro** | UI animations, tweening |
| **Modern UI Pack** | Pre-built modern UI elements |
| **GUI PRO Kit** | Professional UI kit |
| **TextMeshPro** | Text rendering (built-in) |

---

## 11. MVP Scope (Version 1.0)

### Include in MVP

| Feature | Scope |
|---------|-------|
| **Progression** | Stage 1 and Stage 2 (Solo Planner → Company Employee) |
| **Event Types** | 6 types: Kids' birthday, family gathering, school event, adult birthday, engagement party, corporate meeting |
| **Map** | Simplified city map (fewer locations), 5 venues |
| **Vendors** | 5 planning categories, 3 options each (budget/standard/premium) |
| **Phone Apps** | Basic phone interface: Calendar, Messages, Bank |
| **Client Types** | 3 personalities: Easy-Going, Budget-Conscious, Perfectionist |
| **Consequence System** | Core satisfaction calculations |
| **Employee Mechanics** | Salary, commission, performance reviews |
| **UI** | Polished, modern mobile UI |
| **Results Screen** | Detailed breakdown of event outcome |
| **Tutorial** | Guided first event |
| **Monetization** | Rewarded ads for bonuses, basic IAP for currency |

### MVP Success Metrics

- Complete first event in under 10 minutes
- Clear understanding of all mechanics after tutorial
- 60fps on iPhone 8 / mid-range Android
- < 100MB initial download

### Defer to v2.0+

- Stages 3-5 progression (own company through premier agency)
- Staff hiring and management
- Multiple simultaneous events
- Additional zones (Downtown, Uptown, Waterfront)
- Diner Dash execution mini-game
- Complex vendor relationship system
- Seasonal events and limited-time content
- Battle pass / subscription features
- Social features

---

## 12. Future Features (v2.0+)

### 12.1 Diner Dash Execution Mode

Real-time mini-game during event execution:
- Good planning = smooth event, minor issues
- Poor planning = chaos, multiple crises
- Player taps to resolve issues, manage staff
- Optional auto-complete with reduced rewards

### 12.2 Advanced Business Features

- Office customization and upgrades
- Marketing and advertising system
- Competitor events
- Specialization paths (wedding specialist, corporate expert)
- Franchise expansion

### 12.3 Social Features

- Share event photos/results
- Friend leaderboards
- Cooperative events

### 12.4 Live Operations

- Seasonal themes (Halloween, holidays)
- Limited-time event types
- Special client campaigns
- Weekly challenges

### 12.5 Monetization Expansion

- Event Pass (battle pass)
- Premium venue/vendor packs
- VIP subscription

---

## 13. AI Development Guidelines

### 13.1 Code Architecture for AI Assistance

**Principle:** Separate pure logic from Unity-specific code.

```csharp
// GOOD: Pure C# class - easy for AI to reason about
public class SatisfactionCalculator
{
    public float Calculate(EventData eventData, ClientData client)
    {
        // Pure logic, no Unity dependencies
    }
}

// GOOD: Thin MonoBehaviour wrapper
public class EventManager : MonoBehaviour
{
    private SatisfactionCalculator _calculator = new();

    public void CompleteEvent(EventData eventData)
    {
        float satisfaction = _calculator.Calculate(eventData, client);
        // Unity-specific stuff here
    }
}
```

### 13.2 Naming Conventions

| Type | Convention | Example |
|------|------------|---------|
| Classes | PascalCase | `SatisfactionCalculator` |
| Public Methods | PascalCase | `CalculateScore()` |
| Private Fields | _camelCase | `_playerData` |
| Properties | PascalCase | `CurrentEvent` |
| Constants | UPPER_SNAKE | `MAX_REPUTATION` |
| Enums | PascalCase | `ClientPersonality.EasyGoing` |

### 13.3 File Organization

- One class per file
- File name matches class name
- Group related classes in folders
- Keep files under 300 lines when possible

### 13.4 Comments for AI Context

```csharp
/// <summary>
/// Calculates final satisfaction score for a completed event.
/// Score is 0.0 to 1.0, where 1.0 is perfect satisfaction.
///
/// Weights:
/// - Venue: 20%
/// - Food: 25%
/// - Entertainment: 20%
/// - Decorations: 15%
/// - Service: 10%
/// - Expectations: 10%
/// </summary>
public float Calculate(EventData eventData, ClientData client)
```

### 13.5 ScriptableObject Usage

Use ScriptableObjects for:
- Venue definitions
- Vendor definitions
- Client templates
- Event type configurations
- Balance parameters

This allows AI to generate data definitions while designers tweak values in Unity Editor.

### 13.6 Testing Entry Points

Create clear entry points for testing:

```csharp
public class DebugCommands
{
    public static void SetMoney(float amount) { }
    public static void SetReputation(int amount) { }
    public static void UnlockAllVenues() { }
    public static void ForceRandomEvent(string eventType) { }
    public static void CompleteCurrentEvent(float satisfaction) { }
}
```

---

## Appendix: Quick Reference

### Satisfaction Formula

```
Final Score = (Venue × 0.20) + (Food × 0.25) + (Entertainment × 0.20)
            + (Decorations × 0.15) + (Service × 0.10) + (Expectations × 0.10)
            × RandomEventModifier
```

### Reputation Thresholds

| Stage Transition | Required Rep | Required Money | Additional Requirements |
|------------------|--------------|----------------|------------------------|
| Stage 1 → 2 (Employee) | 10 | $2,000 | — |
| Stage 2 → 3 (Own Company) | 20 (personal) | $10,000 | Employee level 5 |
| Stage 3 → 4 (Established) | 30 | $30,000 | — |
| Stage 4 → 5 (Premier) | 60 | $100,000 | — |

### Budget Allocation Guidelines

| Category | Min % | Max % | Sweet Spot |
|----------|-------|-------|------------|
| Venue | 20% | 45% | 30% |
| Catering | 20% | 40% | 30% |
| Entertainment | 5% | 25% | 15% |
| Decorations | 5% | 25% | 15% |
| Staffing | 0% | 20% | 5% |
| Contingency | 0% | 15% | 5% |
