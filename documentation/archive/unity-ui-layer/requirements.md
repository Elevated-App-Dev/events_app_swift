# Requirements Document: Unity UI Layer

## Introduction

This document defines requirements for implementing the visual/UI layer of the Event Planning Simulator. The game's backend systems (22 game systems, data models, and business logic) are fully implemented. This spec covers the Unity scenes, prefabs, UI controllers, and visual assets needed to make the game playable.

The UI layer bridges the existing C# systems to Unity's visual components, creating an interactive mobile experience.

## Scope

This spec covers:
- Unity scenes and canvas hierarchy
- UI prefabs and their MonoBehaviour controllers
- Wiring UI to existing backend systems
- ScriptableObject instances for test data
- Basic placeholder art (production art is out of scope)

## Dependencies

This spec depends on the completed systems from `event-planner-simulator`:
- GameManager and all 22 game systems
- All data models and enums
- ISaveSystem, ITimeSystem, IEventPlanningSystem, etc.

## Glossary

- **UIController**: MonoBehaviour that manages a UI prefab and connects it to backend systems
- **Canvas**: Unity's UI rendering system
- **Prefab**: Reusable Unity GameObject template
- **ScriptableObject Instance**: Configured data asset (venue, vendor, event type definitions)

## Requirements

### Requirement UI-1: Scene Structure

**User Story:** As a player, I want properly structured game scenes, so that the game loads and runs correctly.

#### Acceptance Criteria

1. THE Project SHALL contain a MainMenu scene with title screen, new game, continue, and settings buttons [MVP]
2. THE Project SHALL contain a GameplayMain scene containing all gameplay UI layers [MVP]
3. WHEN the game launches, THE SceneManager SHALL load MainMenu scene first [MVP]
4. WHEN the player starts/continues a game, THE SceneManager SHALL load GameplayMain scene [MVP]
5. THE GameplayMain scene SHALL contain a single root Canvas configured for Screen Space - Overlay [MVP]
6. THE Canvas SHALL use Canvas Scaler with "Scale With Screen Size" and reference resolution 1080x1920 [MVP]
7. THE Canvas SHALL have match width/height set to 0.5 for balanced scaling [MVP]

### Requirement UI-2: Canvas Layer Hierarchy

**User Story:** As a player, I want UI elements to layer correctly, so that overlays appear above gameplay and nothing is hidden incorrectly.

#### Acceptance Criteria

1. THE Canvas SHALL organize UI into ordered layers: GameplayLayer (0), PhoneOverlay (10), MapOverlay (20), TutorialOverlay (30), NotificationLayer (40), SettingsOverlay (50), PauseMenuOverlay (60), MilestoneOverlay (70), LoadingOverlay (80) [MVP]
2. WHEN the PhoneOverlay is opened, IT SHALL appear above GameplayLayer elements [MVP]
3. WHEN a notification appears, IT SHALL appear above all gameplay and overlay elements except Loading [MVP]
4. WHEN multiple overlays are active, THE higher sort order SHALL render on top [MVP]
5. THE Canvas SHALL use Unity's sorting order or sibling order to manage layer priority [MVP]

### Requirement UI-3: HUD (Heads-Up Display)

**User Story:** As a player, I want to see my current status at a glance, so that I know my money, reputation, and date without opening menus.

#### Acceptance Criteria

1. THE HUD SHALL display current in-game date from ITimeSystem.CurrentDate [MVP]
2. THE HUD SHALL display current player money with currency formatting from PlayerData.money [MVP]
3. THE HUD SHALL display current reputation score from PlayerData.reputation [MVP]
4. THE HUD SHALL provide a Phone button that opens PhoneOverlay [MVP]
5. THE HUD SHALL provide a Map button that opens MapOverlay [MVP]
6. THE HUD SHALL provide a Settings/Pause button that opens PauseMenuOverlay [MVP]
7. WHEN player money changes, THE HUD SHALL animate the value change [MVP]
8. WHEN player reputation changes, THE HUD SHALL animate the value change with color (green up, red down) [MVP]
9. THE HUD SHALL display workload status indicator (Stage 2+ only) from IEventPlanningSystem [MVP]
10. THE HUD SHALL be visible during all gameplay states except when covered by full-screen overlays [MVP]

### Requirement UI-4: Phone System UI

**User Story:** As a player, I want to access my phone to manage my business, so that I can check messages, calendar, finances, and contacts.

#### Acceptance Criteria

1. WHEN the Phone button is pressed, THE PhoneOverlay SHALL animate in from the bottom of the screen [MVP]
2. THE PhoneOverlay SHALL display a home screen with 7 app icons: Calendar, Messages, Bank, Contacts, Reviews, Tasks, Clients [MVP]
3. EACH app icon SHALL display a badge count from IPhoneSystem.GetBadgeCount() when count > 0 [MVP]
4. WHEN an app icon is tapped, THE PhoneOverlay SHALL transition to that app's content view [MVP]
5. THE PhoneOverlay SHALL provide a back button to return to home screen from any app [MVP]
6. THE PhoneOverlay SHALL provide a close button to dismiss the overlay [MVP]
7. WHEN the close button is pressed, THE PhoneOverlay SHALL animate out [MVP]

#### Calendar App
8. THE CalendarApp SHALL display upcoming events sorted by date [MVP]
9. THE CalendarApp SHALL display task deadlines with urgency indicators [MVP]
10. THE CalendarApp SHALL highlight the current in-game date [MVP]
11. WHEN an event entry is tapped, THE CalendarApp SHALL navigate to event details [MVP]

#### Messages App
12. THE MessagesApp SHALL display message threads grouped by contact [MVP]
13. THE MessagesApp SHALL show unread indicators on threads with new messages [MVP]
14. THE MessagesApp SHALL display contact type icons (Client, Vendor, Company, System) [MVP]
15. WHILE in Stage 2, THE MessagesApp SHALL show separate sections for Company vs Personal [MVP]

#### Bank App
16. THE BankApp SHALL display current balance prominently [MVP]
17. THE BankApp SHALL display recent transactions with type icons [MVP]
18. THE BankApp SHALL display pending income and expenses [MVP]
19. WHILE in Stage 2, THE BankApp SHALL show salary vs commission vs side gig earnings separately [MVP]
20. WHEN funds are low, THE BankApp SHALL highlight the Emergency Funding option [MVP]

#### Contacts App
21. THE ContactsApp SHALL display vendor contacts with category filters [MVP]
22. THE ContactsApp SHALL show relationship level indicators for each vendor [MVP]
23. THE ContactsApp SHALL show quality ratings and revealed hidden attributes [MVP]
24. WHEN a contact is tapped, THE ContactsApp SHALL show full vendor details [MVP]

#### Reviews App
25. THE ReviewsApp SHALL display current reputation with progress bar [MVP]
26. THE ReviewsApp SHALL display recent client reviews with satisfaction scores [MVP]
27. THE ReviewsApp SHALL display excellence streak counter [MVP]
28. THE ReviewsApp SHALL show reputation milestones for next stage [MVP]

#### Tasks App
29. THE TasksApp SHALL display tasks grouped by event [MVP]
30. THE TasksApp SHALL show task status (Pending, In Progress, Completed, Failed) [MVP]
31. THE TasksApp SHALL show deadline countdown for each task [MVP]
32. THE TasksApp SHALL show work hours required (Stage 2+) [MVP]
33. WHEN a task is tapped, THE TasksApp SHALL show task details with action button [MVP]

#### Clients App
34. THE ClientsApp SHALL display client history with satisfaction scores [MVP]
35. THE ClientsApp SHALL show client personality types [MVP]
36. THE ClientsApp SHALL show referral sources [MVP]
37. THE ClientsApp SHALL calculate and display average satisfaction [MVP]

### Requirement UI-5: Map System UI

**User Story:** As a player, I want to navigate a visual city map to find venues and vendors, so that planning feels tangible.

#### Acceptance Criteria

1. WHEN the Map button is pressed, THE MapOverlay SHALL display the city map [MVP]
2. THE MapOverlay SHALL show zone boundaries with unlocked/locked visual states [MVP]
3. THE MapOverlay SHALL display location pins for venues, vendors, office, and meeting points [MVP]
4. THE MapOverlay SHALL provide filter buttons to show: All, Venues, Vendors, Office [MVP]
5. WHEN a zone is tapped, THE MapOverlay SHALL zoom into that zone [MVP]
6. WHEN a location pin is tapped, THE MapOverlay SHALL display a preview card [MVP]
7. THE preview card SHALL show location name, description, and key attributes [MVP]
8. THE preview card SHALL show a "Visit" button to view full details [MVP]
9. WHEN "Visit" is pressed, THE MapOverlay SHALL navigate to the location detail screen [MVP]
10. THE MapOverlay SHALL indicate the player's current office location [MVP]
11. WHEN a locked zone is tapped, THE MapOverlay SHALL show unlock requirements [MVP]
12. THE MapOverlay SHALL provide a close button to dismiss the overlay [MVP]

### Requirement UI-6: Event Planning Flow UI

**User Story:** As a player, I want intuitive screens to plan events, so that I can accept clients, allocate budgets, and book vendors.

#### Acceptance Criteria

#### Client Inquiry Panel
1. WHEN a new inquiry arrives, THE UI SHALL display a notification [MVP]
2. THE ClientInquiryPanel SHALL display client name, event type, budget, guest count, date, and personality [MVP]
3. THE ClientInquiryPanel SHALL display special requirements if any [MVP]
4. THE ClientInquiryPanel SHALL show expiration countdown [MVP]
5. THE ClientInquiryPanel SHALL provide Accept and Decline buttons [MVP]
6. WHEN Accept is pressed, THE UI SHALL transition to Budget Allocation [MVP]
7. WHEN Decline is pressed, THE UI SHALL dismiss the inquiry [MVP]

#### Budget Allocation Panel
8. THE BudgetAllocationPanel SHALL display total budget with category sliders [MVP]
9. THE BudgetAllocationPanel SHALL show recommended allocation percentages [MVP]
10. THE BudgetAllocationPanel SHALL update remaining budget in real-time as sliders change [MVP]
11. THE BudgetAllocationPanel SHALL warn when allocations are below recommended minimums [MVP]
12. THE BudgetAllocationPanel SHALL provide a Confirm button to lock allocations [MVP]
13. WHEN Confirm is pressed, THE UI SHALL transition to Venue Selection [MVP]

#### Venue Selection Panel
14. THE VenueSelectionPanel SHALL display available venues filtered by event requirements [MVP]
15. THE VenueSelectionPanel SHALL show venue capacity, price, type, and availability [MVP]
16. THE VenueSelectionPanel SHALL warn if venue capacity is insufficient for guest count [MVP]
17. THE VenueSelectionPanel SHALL show weather warnings for outdoor venues [MVP]
18. WHEN a venue is selected, THE UI SHALL confirm booking and transition to Vendor Selection [MVP]

#### Vendor Selection Panel
19. THE VendorSelectionPanel SHALL display vendor categories with required/optional indicators [MVP]
20. THE VendorSelectionPanel SHALL show available vendors per category [MVP]
21. THE VendorSelectionPanel SHALL display vendor tier, price, quality, and availability [MVP]
22. THE VendorSelectionPanel SHALL show relationship level and revealed attributes [MVP]
23. THE VendorSelectionPanel SHALL track remaining budget per category [MVP]
24. WHEN all required vendors are booked, THE UI SHALL enable completion [MVP]
25. THE VendorSelectionPanel SHALL provide a Complete Planning button [MVP]

### Requirement UI-7: Event Execution UI

**User Story:** As a player, I want to see my event unfold with potential surprises, so that I experience the consequences of my planning.

#### Acceptance Criteria

1. WHEN an event date arrives, THE EventExecutionPanel SHALL display event progress [MVP]
2. THE EventExecutionPanel SHALL show event status updates as execution progresses [MVP]
3. WHEN a random event occurs, THE EventExecutionPanel SHALL display it with impact description [MVP]
4. IF the random event can be mitigated, THE EventExecutionPanel SHALL show mitigation options [MVP]
5. THE EventExecutionPanel SHALL show contingency budget status [MVP]
6. WHEN the player chooses to mitigate, THE UI SHALL deduct from contingency and show result [MVP]
7. WHEN the player declines mitigation, THE UI SHALL show the consequence [MVP]
8. WHEN execution completes, THE UI SHALL transition to Results [MVP]

### Requirement UI-8: Results Screen UI

**User Story:** As a player, I want detailed results of my events, so that I understand what went well and what to improve.

#### Acceptance Criteria

1. THE ResultsPanel SHALL display final satisfaction score prominently [MVP]
2. THE ResultsPanel SHALL display category breakdown (Stage 2+ only) with animations [MVP]
3. THE ResultsPanel SHALL show profit/loss calculation with line items [MVP]
4. THE ResultsPanel SHALL display reputation change with animation [MVP]
5. THE ResultsPanel SHALL show client feedback text [MVP]
6. IF a referral was triggered, THE ResultsPanel SHALL highlight it [MVP]
7. THE ResultsPanel SHALL list random events that occurred and their impacts [MVP]
8. THE ResultsPanel SHALL provide a Continue button to return to gameplay [MVP]
9. WHEN Continue is pressed, THE UI SHALL return to the main gameplay view [MVP]

### Requirement UI-9: Tutorial System UI

**User Story:** As a new player, I want guided instruction through my first event, so that I understand core mechanics.

#### Acceptance Criteria

1. THE TutorialOverlay SHALL display tutorial step instructions [MVP]
2. THE TutorialOverlay SHALL highlight relevant UI elements per ITutorialSystem.CurrentStep [MVP]
3. THE TutorialOverlay SHALL dim non-highlighted elements [MVP]
4. THE TutorialOverlay SHALL display contextual tips from ShowContextualTip() [MVP]
5. THE TutorialOverlay SHALL provide a Skip Tutorial button with confirmation [MVP]
6. THE TutorialOverlay SHALL provide a Next/Continue button to advance steps [MVP]
7. WHEN the tutorial completes, THE TutorialOverlay SHALL dismiss automatically [MVP]
8. THE TutorialOverlay SHALL support highlighting by element ID matching [MVP]

### Requirement UI-10: Notification System UI

**User Story:** As a player, I want to see important alerts without interrupting gameplay, so that I stay informed.

#### Acceptance Criteria

1. WHEN a notification is triggered, THE NotificationPopup SHALL appear at the top of the screen [MVP]
2. THE NotificationPopup SHALL display notification type icon, title, and message [MVP]
3. THE NotificationPopup SHALL auto-dismiss after 4 seconds [MVP]
4. THE NotificationPopup SHALL provide a dismiss button for immediate dismissal [MVP]
5. WHEN tapped, THE NotificationPopup SHALL navigate to the relevant screen [MVP]
6. THE NotificationPopup SHALL queue multiple notifications and show sequentially [MVP]
7. THE NotificationPopup SHALL use priority ordering from INotificationSystem [MVP]

### Requirement UI-11: Settings and Pause Menu UI

**User Story:** As a player, I want to adjust settings and pause the game, so that I can customize my experience.

#### Acceptance Criteria

#### Pause Menu
1. WHEN the pause button is pressed, THE PauseMenuOverlay SHALL appear [MVP]
2. THE PauseMenuOverlay SHALL pause game time via ITimeSystem [MVP]
3. THE PauseMenuOverlay SHALL provide Resume, Settings, and Quit buttons [MVP]
4. WHEN Resume is pressed, THE PauseMenuOverlay SHALL dismiss and resume time [MVP]
5. WHEN Quit is pressed, THE PauseMenuOverlay SHALL save and return to MainMenu [MVP]

#### Settings Panel
6. THE SettingsPanel SHALL display audio sliders (Music, SFX) bound to IAudioManager [MVP]
7. THE SettingsPanel SHALL display notification toggles bound to INotificationSystem [MVP]
8. THE SettingsPanel SHALL display accessibility options (text size) [MVP]
9. THE SettingsPanel SHALL provide Restore Purchases button [MVP]
10. THE SettingsPanel SHALL provide Reset Game button with confirmation [MVP]
11. THE SettingsPanel SHALL provide Privacy Settings access [MVP]
12. THE SettingsPanel SHALL display game version [MVP]

### Requirement UI-12: Milestone Sequence UI

**User Story:** As a player reaching Stage 3, I want to experience a satisfying narrative moment, so that my journey feels meaningful.

#### Acceptance Criteria

1. WHEN Stage 3 milestone triggers, THE MilestoneOverlay SHALL display Career Summary [MVP]
2. THE CareerSummary SHALL show total events, first event, highest satisfaction, total earnings [MVP]
3. THE MilestoneOverlay SHALL display path choice (Entrepreneur vs Corporate) with descriptions [MVP]
4. WHEN a path is chosen, THE MilestoneOverlay SHALL play the path-specific narrative [MVP]
5. THE narrative SHALL display images and text for each narrative scene [MVP]
6. THE MilestoneOverlay SHALL display Credits Sequence [MVP]
7. THE MilestoneOverlay SHALL provide Skip button for subsequent playthroughs [MVP]
8. WHEN the sequence completes, THE MilestoneOverlay SHALL dismiss and continue gameplay [MVP]

### Requirement UI-13: Main Menu UI

**User Story:** As a player, I want a polished main menu, so that the game feels professional.

#### Acceptance Criteria

1. THE MainMenu SHALL display game title/logo [MVP]
2. THE MainMenu SHALL provide New Game button [MVP]
3. IF a save exists, THE MainMenu SHALL provide Continue button [MVP]
4. THE MainMenu SHALL provide Settings button [MVP]
5. THE MainMenu SHALL provide Credits button [MVP]
6. WHEN New Game is pressed with existing save, THE MainMenu SHALL confirm overwrite [MVP]
7. WHEN Continue is pressed, THE MainMenu SHALL load save and transition to GameplayMain [MVP]
8. THE MainMenu SHALL play background music via IAudioManager [MVP]

### Requirement UI-14: Loading Screen UI

**User Story:** As a player, I want visual feedback during loading, so that I know the game is working.

#### Acceptance Criteria

1. THE LoadingScreen SHALL display during scene transitions [MVP]
2. THE LoadingScreen SHALL show loading progress indicator [MVP]
3. THE LoadingScreen SHALL display loading tips/hints [MVP]
4. WHEN loading completes, THE LoadingScreen SHALL fade out [MVP]

### Requirement UI-15: ScriptableObject Test Data

**User Story:** As a developer, I want test data to make the game playable, so that we can test the UI implementation.

#### Acceptance Criteria

1. THE Project SHALL contain at least 5 VenueData ScriptableObject instances for Stage 1 [MVP]
2. THE Project SHALL contain at least 10 VendorData ScriptableObject instances across categories [MVP]
3. THE Project SHALL contain EventTypeData instances for all Stage 1 event types [MVP]
4. THE ScriptableObjects SHALL have realistic test values matching design requirements [MVP]
5. THE Project SHALL contain IAPProductData instances for basic IAP products [MVP]
6. THE Project SHALL contain AdPlacementData instances for ad placements [MVP]

### Requirement UI-16: UI Controller Wiring

**User Story:** As a developer, I want UI controllers properly connected to backend systems, so that the UI reflects game state.

#### Acceptance Criteria

1. ALL UIControllers SHALL obtain system references from GameManager.Instance [MVP]
2. ALL UIControllers SHALL subscribe to relevant system events on Enable [MVP]
3. ALL UIControllers SHALL unsubscribe from events on Disable [MVP]
4. ALL UIControllers SHALL update display when underlying data changes [MVP]
5. ALL UIControllers SHALL call appropriate system methods for user actions [MVP]
6. ALL UIControllers SHALL handle null/missing data gracefully [MVP]

### Requirement UI-17: Animation and Feedback

**User Story:** As a player, I want responsive visual feedback, so that the game feels polished.

#### Acceptance Criteria

1. ALL buttons SHALL animate on press (scale to 0.95) and release (scale to 1.0) [MVP]
2. ALL screen transitions SHALL use fade or slide animations (250-400ms) [MVP]
3. WHEN values change (money, reputation), THE UI SHALL animate the transition [MVP]
4. WHEN success occurs, THE UI SHALL play success feedback (particles, sound) [MVP]
5. WHEN error occurs, THE UI SHALL play error feedback (shake, sound) [MVP]
6. ALL animations SHALL respect AccessibilitySettings.reducedMotion [MVP]

### Requirement UI-18: Mobile Input Handling

**User Story:** As a mobile player, I want responsive touch controls, so that the game is easy to use.

#### Acceptance Criteria

1. ALL interactive elements SHALL have minimum 44x44 point touch targets [MVP]
2. THE UI SHALL support swipe gestures for dismissing overlays [MVP]
3. THE UI SHALL provide haptic feedback on supported devices for key actions [MVP]
4. THE UI SHALL handle multi-touch appropriately (prevent accidental double-taps) [MVP]
5. THE UI SHALL adapt to screen safe areas (notches, home indicators) [MVP]
