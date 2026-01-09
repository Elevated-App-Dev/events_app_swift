# Implementation Plan: Event Planning Simulator

## Overview

This implementation plan breaks down the Event Planning Simulator into discrete coding tasks, organized by system dependency. Tasks focus on MVP + Launch-Simple scope (Stages 1-3), with extension hooks for Stages 4-5. Each task builds incrementally on previous work.

## Tasks

- [x] 1. Project Setup and Core Infrastructure
  - [x] 1.1 Create Unity 6 project with folder structure (Core, Data, Managers, Systems, UI, Tests)
    - Set up Assembly Definitions for testability
    - Configure .gitignore for Unity
    - _Requirements: R1.3_
  - [x] 1.2 Implement GameDate struct with date arithmetic
    - AddDays, DaysBetween, comparison operators
    - TotalDays calculation for simplified 30-day months
    - _Requirements: R11.1_
  - [x] 1.3 Write property test for GameDate round-trip
    - **Property: GameDate serialization round-trip**
    - **Validates: Requirements R27.1**
  - [x] 1.4 Implement core enums (BusinessStage, ClientPersonality, EventStatus, etc.)
    - All enums from design document
    - _Requirements: R1.4_

- [x] 2. Data Models Implementation
  - [x] 2.1 Implement PlayerData and EmployeeData classes
    - All fields from design, serializable
    - GetTitle() and GetCompensation() methods
    - _Requirements: R16.2, R16.3_
  - [x] 2.2 Implement EventData, EventBudget, and EventResults classes
    - Budget calculations (Remaining, OverageAmount, OveragePercent)
    - _Requirements: R7.1-R7.8_
  - [x] 2.3 Write property test for EventBudget calculations
    - **Property 7: Budget Allocation Math**
    - **Validates: Requirements R7**
  - [x] 2.4 Implement VendorData and VenueData ScriptableObjects
    - All visible and hidden attributes
    - _Requirements: R8.2, R9.1_
  - [x] 2.5 Implement VendorRelationship with level progression
    - RelationshipLevel calculation, DiscountPercent
    - _Requirements: R20.1-R20.3_
  - [x] 2.6 Write property test for VendorRelationship progression
    - **Property 19: Vendor Relationship Progression**
    - **Validates: Requirements R20**
  - [x] 2.7 Implement WorkHoursData with daily reset
    - RemainingHours, CanUseOvertime, ResetDaily
    - _Requirements: R10.7-R10.11_
  - [x] 2.8 Write property test for WorkHours accumulation and reset
    - **Property 11: Work Hours Accumulation and Reset**
    - **Validates: Requirements R10**
  - [x] 2.9 Implement EventTask and TaskStatus
    - Dependencies, deadlines, company help tracking
    - _Requirements: R10.1-R10.6_
  - [x] 2.10 Implement ClientInquiry and ClientData classes
    - Expiration logic, referral tracking
    - _Requirements: R5.1-R5.8_
  - [x] 2.11 Implement EventTypeData ScriptableObject
    - Event type definitions with subcategories
    - Complexity, budget ranges, required/optional vendors
    - Recommended budget split percentages
    - _Requirements: R6.1-R6.17_

- [x] 3. Checkpoint - Core Data Models
  - Ensure all tests pass, ask the user if questions arise.

- [x] 4. Save System Implementation
  - [x] 4.1 Implement SaveData class with all fields
    - BookingEntry list format for serialization
    - Helper methods for vendor/venue bookings
    - _Requirements: R27.1-R27.5_
  - [x] 4.2 Implement ISaveSystem interface and SaveSystemImpl
    - Save, Load, HasSaveFile, DeleteSave
    - CreateBackup, RestoreFromBackup
    - ValidateSaveFile, version migration
    - _Requirements: R2.2-R2.6, R27.1-R27.7_
  - [x] 4.3 Write property test for Save/Load round-trip
    - **Property 1: Save/Load Round Trip**
    - **Validates: Requirements R2, R27**
  - [x] 4.4 Implement GameSettings with NotificationSettings, PrivacySettings, AccessibilitySettings
    - Default values per requirements
    - _Requirements: R35.6-R35.27, R36.6-R36.7_

- [x] 5. Time System Implementation
  - [x] 5.1 Implement ITimeSystem interface and TimeSystemImpl
    - CurrentDate, AdvanceTime, GetTimeRate
    - ScheduleEvent based on complexity
    - _Requirements: R11.1-R11.11_
  - [x] 5.2 Write property test for time passage by stage
    - **Property 12: Time Passage by Stage**
    - **Validates: Requirements R11**
  - [x] 5.3 Implement event phase calculations
    - Phase durations, stress weights
    - _Requirements: R11.4-R11.7_

- [x] 6. Satisfaction Calculator Implementation
  - [x] 6.1 Implement ISatisfactionCalculator interface and SatisfactionCalculatorImpl
    - Calculate method with weighted scores
    - CalculateCategoryScore for individual categories
    - _Requirements: R13.1-R13.3_
  - [x] 6.2 Write property test for satisfaction weighted calculation
    - **Property 13: Satisfaction Weighted Calculation**
    - **Validates: Requirements R13**
  - [x] 6.3 Implement satisfaction clamping (0-100)
    - Handle extreme inputs
    - _Requirements: R13.8_
  - [x] 6.4 Write property test for satisfaction clamping
    - **Property 14: Satisfaction Clamping**
    - **Validates: Requirements R13.8**
  - [x] 6.5 Implement personality threshold lookup
    - Thresholds for each personality type
    - _Requirements: R15.2-R15.6_
  - [x] 6.6 Write property test for personality thresholds
    - **Property 16: Personality Thresholds**
    - **Validates: Requirements R15**
  - [x] 6.7 Implement overage tolerance by personality
    - Tolerance percentages, overage handling
    - _Requirements: R7.9-R7.14_
  - [x] 6.8 Write property test for overage tolerance
    - **Property 8: Overage Tolerance by Personality**
    - **Validates: Requirements R7.9-R7.14**

- [ ] 7. Checkpoint - Core Calculators
  - Ensure all tests pass, ask the user if questions arise.

- [ ] 8. Progression System Implementation
  - [ ] 8.1 Implement IProgressionSystem interface and ProgressionSystemImpl
    - ApplyEventResult with reputation changes
    - CanAdvanceStage with stage requirements
    - _Requirements: R14.1-R14.12_
  - [ ] 8.2 Write property test for reputation change by satisfaction
    - **Property 15: Reputation Change by Satisfaction**
    - **Validates: Requirements R14**
  - [ ] 8.3 Implement personality distribution by stage
    - GetPersonalityDistribution method
    - _Requirements: R14.13_
  - [ ] 8.4 Implement Stage 2 performance review evaluation
    - EvaluatePerformance method
    - Positive/negative/neutral outcomes
    - _Requirements: R16.5-R16.10_
  - [ ] 8.5 Write property test for performance review evaluation
    - **Property 18: Performance Review Evaluation**
    - **Validates: Requirements R16**
  - [ ] 8.6 Implement employee compensation calculation
    - GetCompensation by level
    - _Requirements: R16.3_
  - [ ] 8.7 Write property test for employee compensation
    - **Property 17: Employee Compensation by Level**
    - **Validates: Requirements R16**

- [ ] 9. Event Planning System Implementation
  - [ ] 9.1 Implement IEventPlanningSystem interface and EventPlanningSystemImpl
    - GenerateInquiry with stage/reputation modifiers
    - AcceptInquiry creating EventData
    - _Requirements: R5.1-R5.8_
  - [ ] 9.2 Write property test for client inquiry completeness
    - **Property 3: Client Inquiry Completeness**
    - **Validates: Requirements R5**
  - [ ] 9.3 Write property test for event creation from inquiry
    - **Property 4: Event Creation from Inquiry**
    - **Validates: Requirements R5**
  - [ ] 9.4 Implement event title generation
    - "[ClientName]'s [SubCategory]" format
    - _Requirements: R6.18_
  - [ ] 9.5 Write property test for event title format
    - **Property 6: Event Title Format**
    - **Validates: Requirements R6.18**
  - [ ] 9.6 Implement workload capacity calculation
    - GetWorkloadStatus with stage-specific thresholds
    - Stage 1 simplified (soft cap at 3)
    - Stage 2+ full tier system
    - _Requirements: R5.9-R5.19_
  - [ ] 9.7 Write property test for workload capacity and penalties
    - **Property 5: Workload Capacity and Penalties**
    - **Validates: Requirements R5.9-R5.18**
  - [ ] 9.8 Implement overlapping event preparation penalty
    - 5% per overlapping event
    - _Requirements: R5.17_
  - [ ] 9.9 Write property test for overlapping preparation penalty
    - **Property 25: Overlapping Event Preparation Penalty**
    - **Validates: Requirements R5.17**
  - [ ] 9.10 Implement vendor booking with budget deduction
    - BookVendor method
    - _Requirements: R8.1-R8.7_
  - [ ] 9.11 Write property test for vendor booking budget deduction
    - **Property 9: Vendor Booking Budget Deduction**
    - **Validates: Requirements R8**
  - [ ] 9.12 Implement venue booking with capacity validation
    - BookVenue method
    - _Requirements: R9.1-R9.7_
  - [ ] 9.13 Write property test for venue capacity validation
    - **Property 10: Venue Capacity Validation**
    - **Validates: Requirements R9**

- [ ] 10. Checkpoint - Event Planning Core
  - Ensure all tests pass, ask the user if questions arise.

- [ ] 11. Referral System Implementation
  - [ ] 11.1 Implement referral probability calculation
    - Base chance by satisfaction (90%+)
    - Excellence streak bonuses
    - _Requirements: R23.1-R23.8_
  - [ ] 11.2 Write property test for referral probability
    - **Property 20: Referral Probability by Satisfaction**
    - **Validates: Requirements R23**
  - [ ] 11.3 Implement excellence streak tracking
    - Increment on 90%+, reset on <80%
    - _Requirements: R23.6-R23.9_
  - [ ] 11.4 Write property test for excellence streak tracking
    - **Property 21: Excellence Streak Tracking**
    - **Validates: Requirements R23**
  - [ ] 11.5 Write property test for excellence streak referral bonus
    - **Property 26: Excellence Streak Referral Bonus Application**
    - **Validates: Requirements R23.6-R23.8**

- [ ] 12. Consequence System Implementation
  - [ ] 12.1 Implement IConsequenceSystem interface and ConsequenceSystemImpl
    - EvaluateRandomEvents with stage-based frequency
    - CalculateRandomEventModifier
    - _Requirements: R12.1-R12.10, R14.14_
  - [ ] 12.2 Implement RandomEventResult and MitigationResult classes
    - GetFinalImpact based on mitigation
    - _Requirements: R12.3, R12.6_
  - [ ] 12.3 Implement CheckMitigation for contingency budget
    - _Requirements: R7.15-R7.17_

- [ ] 13. Weather System Implementation
  - [ ] 13.1 Implement IWeatherSystem interface and WeatherSystemImpl
    - GetForecast, GetForecastForDate
    - GetSimplifiedRisk for Stage 1
    - _Requirements: R32.1-R32.7_
  - [ ] 13.2 Implement WeatherForecast with accuracy by days out
    - 70% at 7 days, 90% at 2 days, 100% day-of
    - _Requirements: R32.7_
  - [ ] 13.3 Write property test for weather forecast accuracy
    - **Property 22: Weather Forecast Accuracy**
    - **Validates: Requirements R32**
  - [ ] 13.4 Implement CheckOutdoorEventRisk with warnings
    - WeatherWarning generation
    - _Requirements: R32.5-R32.6, R32.9-R32.11_

- [ ] 14. Checkpoint - Game Systems Complete
  - Ensure all tests pass, ask the user if questions arise.

- [ ] 15. Map System Implementation
  - [ ] 15.1 Implement IMapSystem interface and MapSystemImpl
    - GetVisibleZones by stage
    - GetLocationsInZone, NavigateToZone
    - _Requirements: R3.1-R3.10_
  - [ ] 15.2 Write property test for zone visibility by stage
    - **Property 2: Zone Visibility by Stage**
    - **Validates: Requirements R3**
  - [ ] 15.3 Implement LocationData and LocationPreviewData
    - Pin types, preview card data
    - _Requirements: R3.3-R3.4, R3.7_

- [ ] 16. Phone System Implementation
  - [ ] 16.1 Implement IPhoneSystem interface and PhoneSystemImpl
    - OpenPhone, ClosePhone, OpenApp
    - Badge count management
    - _Requirements: R4.1-R4.14_
  - [ ] 16.2 Implement PhoneApp enum and app data structures
    - Calendar, Messages, Bank, Contacts, Reviews, Tasks, Clients
    - _Requirements: R4.2-R4.11_

- [ ] 17. Tutorial System Implementation
  - [ ] 17.1 Implement ITutorialSystem interface and TutorialSystemImpl
    - StartTutorial, AdvanceStep, SkipTutorial
    - HighlightElements, ShowContextualTip
    - _Requirements: R25.1-R25.15_
  - [ ] 17.2 Implement TutorialStep enum and step progression
    - Simplified core loop for Stage 1
    - _Requirements: R25.4-R25.7_

- [ ] 18. Audio System Implementation
  - [ ] 18.1 Implement IAudioManager interface and AudioManagerImpl
    - PlayMusic, PlaySFX, volume controls
    - PauseAudio, ResumeAudio
    - _Requirements: R26.1-R26.6_
  - [ ] 18.2 Implement MusicTrack and SoundEffect enums
    - Context-appropriate audio selection
    - _Requirements: R26.1-R26.4_

- [ ] 19. Notification System Implementation
  - [ ] 19.1 Implement INotificationSystem interface and NotificationSystemImpl
    - ScheduleNotification, CancelNotification
    - IsNotificationEnabled, SetNotificationEnabled
    - _Requirements: R36.1-R36.15_
  - [ ] 19.2 Implement notification scheduling logic
    - Priority system, daily limits, quiet hours
    - _Requirements: R36.9-R36.12_

- [ ] 20. Achievement System Implementation
  - [ ] 20.1 Implement IAchievementSystem interface and AchievementSystemImpl
    - CheckAndAward, GetProgress, IncrementProgress
    - SyncWithPlatform
    - _Requirements: R37.1-R37.5_
  - [ ] 20.2 Implement AchievementData and AchievementProgress classes
    - All achievement types from requirements
    - _Requirements: R37.6-R37.16_

- [ ] 21. Checkpoint - UI Systems Complete
  - Ensure all tests pass, ask the user if questions arise.

- [ ] 22. Monetization System Implementation
  - [ ] 22.1 Implement IMonetizationSystem interface and MonetizationSystemImpl
    - Initialize Unity IAP and Unity Ads
    - IsRewardedAdReady, ShowRewardedAd
    - PurchaseProduct, RestorePurchases
    - _Requirements: R28.1-R28.10_
  - [ ] 22.2 Implement IAPProductData ScriptableObject
    - Product types, rewards, unlocks
    - _Requirements: R29.1-R29.10_
  - [ ] 22.3 Implement AdPlacementData ScriptableObject
    - Cooldowns, daily limits, rewards
    - _Requirements: R30.1-R30.10_
  - [ ] 22.4 Implement MonetizationState with ad tracking
    - CanWatchAd, RecordAdWatch, ResetDailyCounts
    - _Requirements: R30.4_

- [ ] 23. Unity Gaming Services Implementation
  - [ ] 23.1 Implement IUnityServicesManager interface and UnityServicesManagerImpl
    - Initialize, TrackEvent, GetRemoteConfig
    - Analytics consent management
    - _Requirements: R31.1-R31.8_
  - [ ] 23.2 Implement analytics event tracking
    - All events from Analytics Events List
    - _Requirements: R31.5_

- [ ] 24. Emergency Funding Implementation
  - [ ] 24.1 Implement family help system (Stages 1-2)
    - Diminishing returns ($500, $400, $300)
    - Max 3 requests tracking
    - _Requirements: R34.1-R34.7_
  - [ ] 24.2 Write property test for family help diminishing returns
    - **Property 24: Family Help Diminishing Returns**
    - **Validates: Requirements R34**

- [ ] 25. Profit Calculation Implementation
  - [ ] 25.1 Implement profit margin calculation
    - 20-30% for 70%+ satisfaction
    - 10-15% for 50-69%
    - Break-even/loss for <50%
    - _Requirements: R33.1-R33.3_
  - [ ] 25.2 Write property test for profit margin calculation
    - **Property 23: Profit Margin Calculation**
    - **Validates: Requirements R33**
  - [ ] 25.3 Implement commission calculation for Stage 2
    - Base pay + commission by level
    - _Requirements: R16.3_
  - [ ] 25.4 Write property test for commission calculation
    - **Property 27: Commission Calculation Formula**
    - **Validates: Requirements R16.3**

- [ ] 26. Celebrity System Implementation (Post-MVP Stub)
  - [ ] 26.1 Implement celebrity reputation loss cap
    - Cap at -50 maximum loss
    - Press coverage multipliers
    - _Requirements: R15.17-R15.19a_
  - [ ] 26.2 Write property test for celebrity reputation loss cap
    - **Property 28: Celebrity Reputation Loss Cap**
    - **Validates: Requirements R15.17-R15.19**

- [ ] 27. Checkpoint - Platform Systems Complete
  - Ensure all tests pass, ask the user if questions arise.

- [ ] 28. GameManager Integration
  - [ ] 28.1 Implement GameManager singleton with all system references
    - InitializeSystems in dependency order
    - StartGame, StartNewGame methods
    - _Requirements: R1.1, R2.1-R2.9_
  - [ ] 28.2 Implement game state management
    - GameState enum transitions
    - Offline mode handling
    - _Requirements: R2.7-R2.9_

- [ ] 29. Stage 3 Milestone Implementation
  - [ ] 29.1 Implement CareerSummaryData and MilestoneProgress
    - Track journey statistics
    - _Requirements: R17.1-R17.2_
  - [ ] 29.2 Implement milestone sequence trigger
    - Path choice ceremony
    - Credits sequence
    - _Requirements: R17.3-R17.10_

- [ ] 30. Extension Hooks for Stages 4-5
  - [ ] 30.1 Implement IStaffSystem stub interface
    - Empty implementations for MVP
    - _Requirements: R19_
  - [ ] 30.2 Implement IMarketingSystem stub interface
    - Returns 1.0 modifier for MVP
    - _Requirements: R22_
  - [ ] 30.3 Implement IOfficeSystem stub interface
    - Returns 0 bonus for MVP
    - _Requirements: R21_

- [ ] 31. Final Integration and Testing
  - [ ] 31.1 Wire all systems together in GameManager
    - Verify initialization order
    - Test system interactions
    - _Requirements: R1, R2_
  - [ ] 31.2 Run full property test suite
    - All 28 properties passing
    - Minimum 100 iterations each
  - [ ] 31.3 Create integration tests for core gameplay loop
    - Accept inquiry → Plan event → Execute → Results
    - _Requirements: R5, R13, R14_

- [ ] 32. Final Checkpoint
  - Ensure all tests pass, ask the user if questions arise.

## Notes

- All tasks are required for comprehensive implementation
- Each task references specific requirements for traceability
- Checkpoints ensure incremental validation
- Property tests validate universal correctness properties (28 total)
- Unit tests validate specific examples and edge cases
- Extension hooks (Tasks 30.1-30.3) provide stubs for Stages 4-5 without full implementation
