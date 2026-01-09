# Event Planning Simulator

A Unity 6 mobile business simulation game where players build an event planning career.

## Project Structure

```
Assets/
├── Art/              # Sprites, textures, UI assets
├── Audio/            # Music and sound effects
├── Fonts/            # Custom fonts
├── Plugins/          # Third-party plugins (Unity IAP, Ads, etc.)
├── Prefabs/          # Reusable game objects
├── Resources/        # Runtime-loaded assets
├── Scenes/           # Game scenes
├── ScriptableObjects/# Data assets (vendors, venues, events)
└── Scripts/
    ├── Core/         # Pure C# logic (no Unity deps, fully testable)
    ├── Data/         # Data structures, enums, ScriptableObjects
    ├── Managers/     # MonoBehaviour managers (GameManager, etc.)
    ├── Systems/      # Game system implementations
    ├── UI/           # UI controllers and views
    └── Tests/
        ├── EditMode/ # Unit tests (run without Play mode)
        └── PlayMode/ # Integration tests (require Play mode)
```

## Assembly Definitions

The project uses Assembly Definitions for:
- Clean dependency management
- Faster compilation (only recompile changed assemblies)
- Testability (Core has no Unity dependencies)

Dependency graph:
```
Data (base) ← Core ← Systems ← UI ← Managers
                 ↑       ↑
                 └───────┴─── Tests
```

## Requirements

- Unity 6 (6000.0 or later)
- Unity IAP package
- Unity Ads package
- Unity Analytics package

## Getting Started

1. Open the project in Unity 6
2. Install required packages via Package Manager
3. Open the main scene in Assets/Scenes/
4. Press Play to test

## Testing

Run tests via Window > General > Test Runner:
- EditMode: Pure logic tests (fast, no Play mode needed)
- PlayMode: Integration tests (require Play mode)

## Prefab & Controller Wiring

### Automatic Wiring

The project includes an automated wiring system that attaches controller scripts to prefabs and connects SerializeField references.

**Run via Unity Editor:**
1. Open Unity
2. Go to `Tools > Wire Controllers to Prefabs`
3. Check the Console for wiring results

**Run via Command Line:**
```bash
Unity -batchmode -projectPath . -executeMethod EventPlannerSim.Editor.PrefabControllerWirer.WireAll -quit
```

The wirer automatically:
- Adds missing controller components to prefabs
- Finds child objects by name and wires them to matching SerializeField references
- Handles common naming patterns (camelCase, PascalCase, with/without suffixes)

### Manual Wiring Steps

If automatic wiring fails or you need to wire references manually:

#### 1. Add Controller Script to Prefab
1. Select the prefab in `Assets/Prefabs/UI/`
2. Click "Open Prefab" to edit
3. Select the root GameObject
4. In Inspector, click "Add Component"
5. Search for and add the matching controller (e.g., `HUDController` for `HUD.prefab`)

#### 2. Wire SerializeField References
For each `[SerializeField]` field in the controller:

1. In the Inspector, find the field (under the controller component)
2. Drag the matching child GameObject from the Hierarchy to the field slot

**Field Naming Convention:**
| Field Name | Expected Child Object |
|------------|----------------------|
| `_headerText` | Child named "Header" or "HeaderText" with TextMeshProUGUI |
| `_confirmButton` | Child named "Confirm" or "ConfirmButton" with Button |
| `_contentPanel` | Child named "Content" or "ContentPanel" (GameObject) |
| `_iconImage` | Child named "Icon" or "IconImage" with Image |

#### 3. Required Child Structure by Prefab

**HUD/TopBar.prefab** → `HUDController`
- `Header` (TextMeshProUGUI) - Day/date display
- `MoneyText` (TextMeshProUGUI) - Currency display
- `ReputationText` (TextMeshProUGUI) - Rep score

**EventPlanning/ClientInquiryPanel.prefab** → `ClientInquiryController`
- `ClientName` (TextMeshProUGUI)
- `EventType` (TextMeshProUGUI)
- `Budget` (TextMeshProUGUI)
- `GuestCount` (TextMeshProUGUI)
- `Deadline` (TextMeshProUGUI)
- `Description` (TextMeshProUGUI)
- `AcceptButton` (Button)
- `DeclineButton` (Button)

**EventPlanning/BudgetAllocationPanel.prefab** → `BudgetAllocationController`
- `TotalBudget` (TextMeshProUGUI)
- `RemainingBudget` (TextMeshProUGUI)
- `VenueSlider`, `CateringSlider`, etc. (Slider)
- `VenueAmount`, `CateringAmount`, etc. (TextMeshProUGUI)
- `Warning` (TextMeshProUGUI)
- `ConfirmButton` (Button)

**EventPlanning/VenueSelectionPanel.prefab** → `VenueSelectionController`
- `Header` (TextMeshProUGUI)
- `VenueListContainer` (Transform) - Parent for spawned cards
- `VenueCardPrefab` reference (set in Inspector)
- `ConfirmButton` (Button)
- `BackButton` (Button)

**EventPlanning/VendorSelectionPanel.prefab** → `VendorSelectionController`
- `Header` (TextMeshProUGUI)
- `CategoryBudget` (TextMeshProUGUI)
- Category tab buttons array (Button[])
- `VendorListContainer` (Transform)
- `VendorCardPrefab` reference
- `RequiredVendors` (TextMeshProUGUI)
- `CompleteButton`, `BackButton` (Button)

**Results/ResultsPanel.prefab** → `ResultsController`
- `SatisfactionScore` (TextMeshProUGUI)
- `SatisfactionLabel` (TextMeshProUGUI)
- `Profit`, `Revenue`, `Expenses` (TextMeshProUGUI)
- `ReputationChange` (TextMeshProUGUI)
- `Feedback` (TextMeshProUGUI)
- `ReferralPanel` (GameObject)
- `ReferralText` (TextMeshProUGUI)
- `ContinueButton` (Button)

**Tutorial/TutorialOverlay.prefab** → `TutorialOverlayController`
- `Title` (TextMeshProUGUI)
- `Message` (TextMeshProUGUI)
- `HighlightTarget` (RectTransform)
- `NextButton`, `SkipButton` (Button)
- `StepIndicator` (TextMeshProUGUI)

**Notifications/NotificationPopup.prefab** → `NotificationController`
- `NotificationContainer` (Transform) - For spawned popups
- `NotificationPrefab` reference
- Or single-popup mode: `Icon` (Image), `Title`, `Message` (TextMeshProUGUI), `DismissButton` (Button)

**Settings/PauseMenu.prefab** → `PauseMenuController`
- `ResumeButton`, `SettingsButton`, `MainMenuButton`, `QuitButton` (Button)

**Settings/SettingsPanel.prefab** → `SettingsController`
- `MusicSlider`, `SFXSlider` (Slider)
- `MusicVolume`, `SFXVolume` (TextMeshProUGUI)
- `NotificationsToggle` (Toggle)
- `CloseButton` (Button)

**MainMenu/MainMenuPanel.prefab** → `MainMenuController`
- `Title` (TextMeshProUGUI)
- `NewGameButton`, `ContinueButton`, `SettingsButton`, `QuitButton` (Button)
- `Version` (TextMeshProUGUI)

**Loading/LoadingScreen.prefab** → `LoadingController`
- `ProgressBar` (Slider)
- `ProgressText` (TextMeshProUGUI)
- `Tip` (TextMeshProUGUI)
- `Spinner` (Image, optional)

#### 4. Verify Wiring
After wiring, test by:
1. Entering Play mode
2. Checking Console for null reference warnings
3. Interacting with UI elements to confirm functionality
