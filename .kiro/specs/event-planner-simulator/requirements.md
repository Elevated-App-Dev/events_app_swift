# Requirements Document

## Introduction

Event Planning Simulator is a mobile business simulation/tycoon game for iOS and Android built in Unity 6. Players start as aspiring event planners organizing small family and school events, progressing through five career stages to eventually run a premier event planning company. The game emphasizes meaningful decisions with trade-offs, visual map-based navigation, and a consequence system that creates emergent storytelling.

This document covers the complete game scope (Stages 1-5) with MVP items marked. The project starts from scratch with all assets built during development. Save data is local device only.

## Progressive Complexity Model

This game uses a **progressive complexity approach** where game systems start simple and unlock depth as players advance. This reduces cognitive load for new players while rewarding engaged players with strategic depth.

### Design Philosophy

- **Stage 1** presents simplified versions of systems - players learn core loops without overwhelming detail
- **Complexity unlocks as rewards** - advanced mechanics feel like progression, not burden
- **Hidden mechanics remain hidden** - players discover depth through experience, not tutorials
- **Satisfying midpoint at Stage 3** - players who stop here feel they've completed a full game

### Phase Definitions

Requirements are tagged with implementation phases:

| Tag | Description |
|-----|-------------|
| **[MVP]** | Core launch features - required for initial release |
| **[Post-MVP]** | Features for future updates after launch |
| **[Phase:Launch-Simple]** | Implemented at launch but in simplified form for Stage 1 |
| **[Phase:Stage2-Unlock]** | Full complexity unlocks when player reaches Stage 2 |
| **[Phase:Stage3-Unlock]** | Full complexity unlocks when player reaches Stage 3 |

### Stage 1 Simplifications

The following systems are **simplified in Stage 1** and unlock full complexity later:

| System | Stage 1 (Simplified) | Full Version Unlocks |
|--------|---------------------|---------------------|
| **Workload Capacity** | Soft cap at 3 events with warning | Stage 2: Full tier system (optimal/comfortable/strained) |
| **Satisfaction Display** | Single final score only | Stage 2: Category breakdown visible |
| **Weather System** | Simple "Good/Risky/Bad" indicator | Stage 2: 7-day forecast with accuracy percentages |
| **Work Hours** | Tasks have deadlines only | Stage 2: Daily 8-hour work capacity system |
| **Vendor Attributes** | Price, quality, style visible | Stage 3: Hidden reliability/flexibility revealed through relationship |

### Stage 3 Midpoint Milestone

When players reach Stage 3, they experience a **narrative completion moment**:

1. **Path Choice Ceremony** - Meaningful choice between Entrepreneur and Corporate paths
2. **Career Summary Screen** - Stats from their journey (first event, biggest success, total earnings)
3. **Credits Sequence** - Acknowledges player achievement; signals "you beat the game"
4. **"Your Story Continues"** - Clear messaging that Stages 4-5 are expansion/endgame content

This ensures players who complete Stage 3 (~8-12 hours) feel satisfied, while dedicated players have substantial endgame content.

### Tutorial Simplification

The tutorial teaches only essential mechanics:
- Accept client → Pick venue → Pick caterer → Event happens → See results

Advanced systems (work hours, weather forecasting, category scores, workload management) are introduced contextually as players encounter them or unlock them through progression.

## Glossary

- **Game_Manager**: The central system coordinating game state, progression, and cross-system communication
- **Map_System**: The visual navigation interface displaying city zones, location pins, and enabling player exploration
- **Phone_System**: The smartphone UI overlay providing access to apps for managing tasks, communications, and business operations
- **Event_Planning_System**: The system managing the complete event lifecycle from client intake through results
- **Satisfaction_Calculator**: The pure logic component calculating client satisfaction scores based on event outcomes
- **Consequence_System**: The system determining outcomes based on player decisions and random events
- **Progression_System**: The system managing player advancement through business stages and unlocks
- **Save_System**: The system handling persistent game state storage and retrieval
- **UI_Manager**: The system coordinating all user interface elements, animations, and transitions
- **Time_System**: The system managing in-game time passage, event scheduling, and phase transitions
- **Marketing_System**: The system managing marketing investments and their effects on client inquiry rates
- **Monetization_System**: The system handling in-app purchases, advertisements, and premium features
- **Staff_System**: The system managing hired employees, their assignments, and performance (Post-MVP)
- **Client**: An NPC requesting event planning services with specific personality, budget, and requirements
- **Vendor**: A service provider (caterer, entertainer, decorator, etc.) that can be hired for events
- **Venue**: A location where events can be held with specific capacity, amenities, and pricing
- **Event**: A planned occasion with client requirements, budget allocation, vendor assignments, and outcome scoring
- **Reputation**: A numeric value representing the player's standing in the event planning industry
- **Business_Stage**: The player's current career level determining available content and mechanics
- **Zone**: A geographic area of the city map containing related venues and vendors

## Requirements

### Requirement 1: Project Architecture

**User Story:** As a developer, I want a properly structured Unity 6 project, so that the codebase is maintainable and AI-assistable.

#### Acceptance Criteria

1. THE Game_Manager SHALL be implemented as a singleton MonoBehaviour managing game state [MVP]
2. THE Game_Manager SHALL separate pure C# logic classes from Unity MonoBehaviour wrappers [MVP]
3. THE Game_Manager SHALL organize scripts into Core, Data, Managers, UI, and Systems folders [MVP]
4. THE Game_Manager SHALL use ScriptableObjects for venue, vendor, client, and event type definitions [MVP]
5. THE Game_Manager SHALL follow naming conventions: PascalCase for classes/methods, _camelCase for private fields [MVP]
6. THE Game_Manager SHALL implement all data structures as serializable classes for save/load support [MVP]

### Requirement 2: Game Initialization and State Management

**User Story:** As a player, I want the game to properly initialize and manage my progress, so that I can have a seamless gaming experience across sessions.

#### Acceptance Criteria

1. WHEN the game launches, THE Game_Manager SHALL initialize all core systems in the correct dependency order [MVP]
2. WHEN a save file exists, THE Save_System SHALL load the player's progress and restore game state [MVP]
3. WHEN no save file exists, THE Game_Manager SHALL initialize a new game with default Stage 1 values [MVP]
4. WHEN the player's data changes, THE Save_System SHALL persist changes to local storage [MVP]
5. IF the save file is corrupted, THEN THE Save_System SHALL notify the player and offer to start a new game [MVP]
6. WHEN the game loses focus or closes, THE Save_System SHALL automatically save current progress [MVP]
7. THE Game_Manager SHALL support full offline play for all core gameplay features (event planning, execution, progression) [MVP]
8. WHEN the device is offline, THE Game_Manager SHALL disable network-dependent features (ads, IAP, cloud save) gracefully without impacting core gameplay [MVP]
9. WHEN the device transitions from offline to online, THE Game_Manager SHALL re-enable network features without requiring app restart [MVP]

### Requirement 3: City Map Navigation

**User Story:** As a player, I want to navigate a visual city map to discover and visit locations, so that planning feels tangible and immersive.

#### Acceptance Criteria

1. WHEN the map screen loads, THE Map_System SHALL display unlocked zones with all accessible location pins [MVP]
2. WHEN a player taps a zone, THE Map_System SHALL zoom into that zone showing filterable location pins [MVP]
3. WHEN a player taps a location pin, THE Map_System SHALL display a preview card with basic information [MVP]
4. WHEN a player taps 'Visit' on a preview card, THE Map_System SHALL navigate to the full location detail screen [MVP]
5. WHILE the player is in Stage 1, THE Map_System SHALL only show the Neighborhood zone as accessible [MVP]
6. WHEN the player reaches Stage 2, THE Map_System SHALL unlock access to employer's vendor network locations [MVP]
7. THE Map_System SHALL visually distinguish between venue pins, vendor pins, office pins, and meeting location pins [MVP]
8. WHEN the player reaches Stage 3, THE Map_System SHALL unlock the Downtown zone [Post-MVP]
9. WHEN the player reaches Stage 4, THE Map_System SHALL unlock the Uptown zone [Post-MVP]
10. WHEN the player reaches Stage 5, THE Map_System SHALL unlock the Waterfront zone [Post-MVP]

### Requirement 4: Phone Interface System

**User Story:** As a player, I want to access a phone interface to manage my business without leaving my current location, so that I can efficiently handle communications and tasks.

#### Acceptance Criteria

1. WHEN the player opens the phone overlay, THE Phone_System SHALL display available apps with notification badges [MVP]
2. WHEN the Calendar app is opened, THE Phone_System SHALL display upcoming events, deadlines, and prep tasks [MVP]
3. WHEN the Messages app is opened, THE Phone_System SHALL display client and vendor communications [MVP]
4. WHEN the Bank app is opened, THE Phone_System SHALL display current finances, pending payments, and transaction history [MVP]
5. WHEN the Contacts app is opened, THE Phone_System SHALL display vendor rolodex with ratings, notes, and relationship status [MVP]
6. WHEN a new message arrives, THE Phone_System SHALL update the Messages app badge count [MVP]
7. WHEN a deadline approaches, THE Phone_System SHALL update the Calendar app badge count [MVP]
8. THE Phone_System SHALL allow the player to close the overlay and return to the previous screen [MVP]
9. WHEN the Reviews app is opened, THE Phone_System SHALL display reputation tracking and testimonials [MVP]
10. WHEN the Tasks app is opened, THE Phone_System SHALL display event to-do checklists with pending counts [MVP]
11. WHEN the Clients app is opened, THE Phone_System SHALL display client database and history [MVP]
12. WHILE in Stage 2, THE Phone_System SHALL visually distinguish company events from personal side gigs in all apps [MVP]
13. WHILE in Stage 2, THE Phone_System SHALL display separate sections or filters for company vs personal communications in Messages [MVP]
14. WHILE in Stage 2, THE Bank app SHALL show separate income tracking for salary/commission vs side gig earnings [MVP]

### Requirement 5: Client Intake and Event Creation

**User Story:** As a player, I want to receive and evaluate client requests, so that I can decide which events to accept based on risk and reward.

#### Acceptance Criteria

1. WHEN a new client inquiry arrives, THE Event_Planning_System SHALL display the client's event type, budget range, guest count, date, and personality type [MVP]
2. WHEN the player reviews a client request, THE Event_Planning_System SHALL show special requirements and constraints [MVP]
3. WHEN the player accepts a client request, THE Event_Planning_System SHALL create a new event in Planning status [MVP]
4. WHEN the player declines a client request, THE Event_Planning_System SHALL remove the inquiry with no penalty [MVP]
5. THE Event_Planning_System SHALL generate new client inquiries at random intervals based on player reputation and stage [MVP]
6. THE Event_Planning_System SHALL use base inquiry intervals of: Stage 1 = 8-12 minutes, Stage 2 = 6-10 minutes, Stage 3 = 5-8 minutes, Stage 4 = 4-7 minutes, Stage 5 = 3-6 minutes [MVP]
7. THE Event_Planning_System SHALL reduce inquiry interval by 5% per 25 reputation points above stage minimum [MVP]
8. THE Event_Planning_System SHALL queue up to 5 pending inquiries, expiring oldest inquiries after 20 minutes if not reviewed [MVP]
9. THE Event_Planning_System SHALL define optimal concurrent event capacity as: Stage 1 = 2 events, Stage 2 = 4 events, Stage 3 = 6 events, Stage 4 = 10 events, Stage 5 = 15 events [MVP] [Phase:Stage2-Unlock]
10. THE Event_Planning_System SHALL define comfortable concurrent event capacity as: Stage 1 = 3 events, Stage 2 = 6 events, Stage 3 = 9 events, Stage 4 = 14 events, Stage 5 = 20 events [MVP] [Phase:Stage2-Unlock]
11. THE Event_Planning_System SHALL define strained concurrent event capacity as: Stage 1 = 5 events, Stage 2 = 8 events, Stage 3 = 12 events, Stage 4 = 18 events, Stage 5 = 25 events [MVP] [Phase:Stage2-Unlock]
12. WHEN the player has concurrent active events at or below optimal capacity, THE Event_Planning_System SHALL apply no penalties [MVP] [Phase:Stage2-Unlock]
13. WHEN the player has concurrent active events between optimal and comfortable capacity, THE Event_Planning_System SHALL apply 3% satisfaction reduction per event over optimal [MVP] [Phase:Stage2-Unlock]
14. WHEN the player has concurrent active events between comfortable and strained capacity, THE Event_Planning_System SHALL apply 7% satisfaction reduction per event over comfortable AND 10% increased task failure probability [MVP] [Phase:Stage2-Unlock]
15. WHEN the player has concurrent active events beyond strained capacity, THE Event_Planning_System SHALL apply 12% satisfaction reduction per event over strained AND 25% increased task failure probability AND display critical workload warning [MVP] [Phase:Stage2-Unlock]
16. THE Event_Planning_System SHALL NOT impose a hard cap on concurrent events, allowing players to accept unlimited events at their own risk [MVP] [Phase:Stage2-Unlock]
17. WHEN multiple concurrent events have overlapping preparation periods, THE Event_Planning_System SHALL increase task failure probability by 5% per overlapping event [MVP] [Phase:Stage2-Unlock]
18. THE Event_Planning_System SHALL display current workload status (Optimal/Comfortable/Strained/Critical) in the UI [MVP] [Phase:Stage2-Unlock]
18a. WHILE in Stage 1, THE Event_Planning_System SHALL use simplified workload display: soft cap at 3 events with warning message, no percentage penalties [MVP] [Phase:Launch-Simple]
19. THE Event_Planning_System SHALL count an event as "active" from acceptance until results are displayed, then remove it from concurrent count [MVP]
20. WHILE in Stage 2, THE Event_Planning_System SHALL generate two separate inquiry streams: company assignments and personal side gig opportunities [MVP]
21. WHILE in Stage 2, THE Event_Planning_System SHALL generate company assignments at the Stage 2 interval rate [MVP]
22. WHILE in Stage 2, THE Event_Planning_System SHALL generate side gig inquiries at 50% of the Stage 1 interval rate (less frequent) [MVP]
23. WHILE in Stage 2, THE Event_Planning_System SHALL visually distinguish company assignments from side gig inquiries in the UI [MVP]

### Requirement 6: Event Types and Characteristics

**User Story:** As a player, I want different event types with unique requirements and complexity, so that I experience variety and progression.

#### Acceptance Criteria

1. WHEN in Stage 1, THE Event_Planning_System SHALL offer Kids' Birthday events with budget $500-$2,000, Low complexity, requiring entertainment focus and kid-friendly vendors [MVP]
2. WHEN in Stage 1, THE Event_Planning_System SHALL offer Family Gathering events with budget $300-$1,500, Low complexity, emphasizing catering and casual atmosphere [MVP]
3. WHEN in Stage 1, THE Event_Planning_System SHALL offer School Event events with budget $1,000-$3,000, Low-Medium complexity, requiring venue capacity planning and educational themes [MVP]
4. WHEN in Stage 1+, THE Event_Planning_System SHALL offer Adult Birthday events with budget $1,000-$5,000, Medium complexity, with theme customization and entertainment variety [MVP]
5. WHEN in Stage 2+, THE Event_Planning_System SHALL offer Engagement Party events with budget $2,000-$8,000, Medium complexity, requiring romantic themes and photography focus [MVP]
6. WHEN in Stage 2+, THE Event_Planning_System SHALL offer Corporate Meeting events with budget $3,000-$15,000, Medium complexity, emphasizing AV equipment, catering, and professional atmosphere [MVP]
7. WHEN in Stage 2+, THE Event_Planning_System SHALL offer Milestone Birthday events with budget $2,000-$6,000, Medium complexity, requiring personalization and guest management [MVP]
8. WHEN in Stage 1+, THE Event_Planning_System SHALL offer Baby Shower events with budget $1,000-$4,000, Medium complexity [MVP]
9. WHEN in Stage 3+, THE Event_Planning_System SHALL offer Anniversary Party events with budget $2,000-$10,000, Medium complexity [Post-MVP]
10. WHEN in Stage 3+, THE Event_Planning_System SHALL offer Small Corporate Event events with budget $5,000-$20,000, Medium complexity [Post-MVP]
11. WHEN in Stage 4+, THE Event_Planning_System SHALL offer Small Wedding events with budget $10,000-$30,000, High complexity [Post-MVP]
12. WHEN in Stage 4+, THE Event_Planning_System SHALL offer Charity Gala events with budget $15,000-$50,000, High complexity [Post-MVP]
13. WHEN in Stage 4+, THE Event_Planning_System SHALL offer Conference events with budget $20,000-$100,000, High complexity [Post-MVP]
14. WHEN in Stage 5, THE Event_Planning_System SHALL offer Large Wedding events with budget $30,000-$150,000, Very High complexity [Post-MVP]
15. WHEN in Stage 5, THE Event_Planning_System SHALL offer Product Launch events with budget $50,000-$200,000, Very High complexity [Post-MVP]
16. WHEN in Stage 5, THE Event_Planning_System SHALL offer Festival events with budget $100,000+, Very High complexity [Post-MVP]
17. FOR EACH event type, THE Event_Planning_System SHALL define required vendor categories, recommended budget splits, and unique random events [MVP]
18. WHEN generating an event inquiry, THE Event_Planning_System SHALL randomly select a sub-category and generate a unique client name, displaying as "[Client Name]'s [Sub-Category]" (e.g., "Emma's Princess Theme Birthday") [MVP]
19. THE Event_Planning_System SHALL define Kids' Birthday sub-categories: Princess/Superhero Theme, Pool Party, Bounce House Party, Arts & Crafts Party, Sports Party [MVP]
20. THE Event_Planning_System SHALL define Family Gathering sub-categories: Graduation Celebration, 4th of July BBQ, Thanksgiving Dinner, Christmas Party, New Year's Eve Party, Easter Brunch, Family Reunion [MVP]
21. THE Event_Planning_System SHALL define School Event sub-categories: Science Fair, Talent Show, Prom/Dance, Sports Banquet, Graduation Ceremony, PTA Fundraiser [MVP]
22. THE Event_Planning_System SHALL define Adult Birthday sub-categories: Surprise Party, Cocktail Party, Dinner Party, Themed Costume Party, Outdoor Adventure Party [MVP]
23. THE Event_Planning_System SHALL define Engagement Party sub-categories: Garden Party, Rooftop Celebration, Intimate Dinner, Brunch Engagement, Cocktail Reception [MVP]
24. THE Event_Planning_System SHALL define Corporate Meeting sub-categories: Board Meeting, Team Building Retreat, Training Workshop, Quarterly Review, Client Presentation, Staff Appreciation Luncheon, Golf Tournament, Corporate Fundraising Reception [MVP]
25. THE Event_Planning_System SHALL define Milestone Birthday sub-categories: Sweet 16, 21st Birthday, 30th Birthday Bash, 40th Birthday, 50th Golden Birthday, Quinceañera [MVP]
26. THE Event_Planning_System SHALL define Baby Shower sub-categories: Traditional Baby Shower, Gender Reveal Party, Couples Baby Shower, Virtual Baby Shower, Sprinkle (2nd+ baby) [MVP]
27. THE Event_Planning_System SHALL define Anniversary Party sub-categories: Silver Anniversary (25th), Golden Anniversary (50th), Vow Renewal Ceremony, Romantic Dinner for Two, Surprise Anniversary Party [Post-MVP]
28. THE Event_Planning_System SHALL define Small Corporate Event sub-categories: Product Demo Day, Networking Mixer, Employee Award Ceremony, Holiday Office Party, Client Appreciation Dinner [Post-MVP]
29. THE Event_Planning_System SHALL define Small Wedding sub-categories: Intimate Garden Ceremony, Destination Beach Wedding, Courthouse Wedding Reception, Backyard Wedding, Elopement Celebration [Post-MVP]
30. THE Event_Planning_System SHALL define Charity Gala sub-categories: Black Tie Fundraiser Gala, Silent Auction Evening, Benefit Concert Night, Charity Run/Walk Event, Masquerade Ball [Post-MVP]
31. THE Event_Planning_System SHALL define Conference sub-categories: Industry Leadership Summit, Tech Innovation Conference, Medical Research Symposium, Educational Workshop Series, Trade Show Exhibition [Post-MVP]
32. THE Event_Planning_System SHALL define Large Wedding sub-categories: Traditional Grand Wedding, Luxury Destination Wedding, Cultural Heritage Wedding, Celebrity-Style Wedding, Multi-Day Wedding Celebration [Post-MVP]
33. THE Event_Planning_System SHALL define Product Launch sub-categories: Tech Product Unveiling, Fashion Collection Debut, Restaurant Grand Opening, Author Book Launch, Mobile App Launch Party [Post-MVP]
34. THE Event_Planning_System SHALL define Festival sub-categories: Summer Music Festival, Food & Wine Festival, Cultural Heritage Festival, Art & Design Festival, Community Street Fair [Post-MVP]

### Requirement 7: Budget Allocation

**User Story:** As a player, I want to allocate the client's budget across categories, so that I can balance quality and cost for each aspect of the event.

#### Acceptance Criteria

1. WHEN the player enters budget allocation, THE Event_Planning_System SHALL display all budget categories with recommended percentages [MVP]
2. WHEN the player adjusts a category allocation, THE Event_Planning_System SHALL update the remaining budget in real-time [MVP]
3. WHEN the player allocates below recommended minimums, THE Event_Planning_System SHALL display a warning about potential consequences [MVP]
4. THE Event_Planning_System SHALL allow free allocation across categories within the total client budget (overspend one category, underspend another) [MVP]
5. WHEN the player confirms budget allocation, THE Event_Planning_System SHALL lock the allocations and enable vendor selection [MVP]
6. THE Event_Planning_System SHALL track contingency allocation separately for emergency use during events [MVP]
7. THE Event_Planning_System SHALL recommend Venue 25-40%, Catering 25-35%, Entertainment 10-20%, Decorations 10-20%, Staffing 5-15%, Contingency 5-10% [MVP]
8. WHEN total spending exceeds the client's budget, THE Event_Planning_System SHALL calculate overage percentage and evaluate client response based on personality [MVP]
9. THE Event_Planning_System SHALL define overage tolerance by personality: Easy-Going = 15% overage tolerance, Budget-Conscious = 0% tolerance, Perfectionist = 5% tolerance, Indecisive = 10% tolerance, Demanding = 5% tolerance [MVP]
10. WHEN overage is within client's tolerance, THE Event_Planning_System SHALL absorb the overage into the event cost without player penalty [MVP]
11. WHEN overage exceeds client's tolerance, THE Event_Planning_System SHALL present the player with a choice at event completion: cover the overage from personal funds OR have the client demand payment [MVP]
12. WHEN the client demands overage payment, THE Event_Planning_System SHALL deduct the overage from the player's profit [MVP]
13. IF the player covers overage voluntarily before client demands it, THEN THE Event_Planning_System SHALL apply a hidden satisfaction bonus (going above and beyond) [MVP]
14. IF total spending exceeds the client's budget significantly (>25%), THEN THE Event_Planning_System SHALL result in a net loss for the player on that event [MVP]
15. WHEN a random event occurs and contingency is insufficient, THE Event_Planning_System SHALL offer the player the option to use personal funds to cover the shortfall [MVP]
16. WHEN a random event occurs and contingency is insufficient, THE Event_Planning_System SHALL offer a rewarded ad option for emergency funds [MVP]
17. IF the player chooses not to cover a shortfall, THEN THE Consequence_System SHALL apply the full satisfaction penalty for the unmitigated event [MVP]
18. THE Event_Planning_System SHALL allow players to make suboptimal decisions (skip contingency, mismatch vendors to theme, underspend categories) with consequences reflected in satisfaction scores [MVP]

### Requirement 8: Vendor Selection and Booking

**User Story:** As a player, I want to browse and hire vendors for my events, so that I can assemble the right team within my budget.

#### Acceptance Criteria

1. WHEN the player browses vendors, THE Event_Planning_System SHALL display available vendors filtered by category [MVP]
2. WHEN viewing a vendor, THE Event_Planning_System SHALL show visible attributes: price, quality rating, style/specialty, and availability [MVP]
3. WHEN the player has worked with a vendor multiple times, THE Event_Planning_System SHALL reveal hidden attributes: reliability, flexibility [MVP]
4. WHEN the player books a vendor, THE Event_Planning_System SHALL deduct the cost from the appropriate budget category [MVP]
5. IF a vendor is unavailable on the event date, THEN THE Event_Planning_System SHALL prevent booking and suggest alternatives [MVP]
6. WHILE in Stage 2, THE Event_Planning_System SHALL provide access to employer's vendor network with discounted rates [MVP]
7. WHEN a vendor is booked, THE Event_Planning_System SHALL add them to the event's vendor assignments [MVP]
8. THE Event_Planning_System SHALL offer Budget, Standard, and Premium vendor tiers in MVP [MVP]
9. WHEN in Stage 4+, THE Event_Planning_System SHALL unlock Luxury tier vendors [Post-MVP]
10. WHEN the player builds relationship with a vendor, THE Event_Planning_System SHALL unlock discount rates [Post-MVP]

**Vendor Categories:**
11. THE Event_Planning_System SHALL define vendor category Caterer: provides food and beverage services, required for all events [MVP]
12. THE Event_Planning_System SHALL define vendor category Entertainer: provides entertainment (DJ, band, magician, face painter, etc.), required for most events [MVP]
13. THE Event_Planning_System SHALL define vendor category Decorator: provides decorations, centerpieces, and theme elements [MVP]
14. THE Event_Planning_System SHALL define vendor category Photographer: provides photo and video documentation services [MVP]
15. THE Event_Planning_System SHALL define vendor category Florist: provides floral arrangements and botanical decor [MVP]
16. THE Event_Planning_System SHALL define vendor category Baker: provides cakes and specialty desserts [MVP]
17. THE Event_Planning_System SHALL define vendor category Rental Company: provides tables, chairs, linens, and equipment rentals [MVP]
18. THE Event_Planning_System SHALL define vendor category AV Technician: provides audio/visual equipment and technical support, required for corporate events [MVP]
19. THE Event_Planning_System SHALL define vendor category Transportation: provides guest transportation, valet, or shuttle services [Post-MVP]
20. THE Event_Planning_System SHALL define vendor category Security: provides event security personnel for large events [Post-MVP]
21. FOR EACH event type, THE Event_Planning_System SHALL specify which vendor categories are required vs optional [MVP]

### Requirement 9: Venue Selection and Booking

**User Story:** As a player, I want to select and book venues for events, so that I can find appropriate spaces that match client needs and budget.

#### Acceptance Criteria

1. WHEN the player browses venues, THE Event_Planning_System SHALL display available venues with capacity, price, and amenities [MVP]
2. WHEN viewing a venue, THE Event_Planning_System SHALL show photos, ambiance rating, accessibility rating, and restrictions [MVP]
3. WHEN the player selects a venue, THE Event_Planning_System SHALL validate guest count against venue capacity [MVP]
4. IF the guest count exceeds comfortable capacity, THEN THE Event_Planning_System SHALL warn about cramped conditions affecting satisfaction [MVP]
5. WHEN the player books a venue, THE Event_Planning_System SHALL deduct the cost from the venue budget allocation [MVP]
6. IF a venue is booked on the event date, THEN THE Event_Planning_System SHALL prevent booking and show alternative dates [MVP]
7. WHEN a venue is booked, THE Event_Planning_System SHALL update the event with the venue assignment [MVP]
8. WHEN in Stage 1, THE Event_Planning_System SHALL offer Neighborhood venues: Backyard, Community Center, Park Pavilion [MVP]
9. WHEN in Stage 2+, THE Event_Planning_System SHALL unlock Downtown venues: Hotels, Restaurants, Small Ballrooms [MVP]
10. WHEN in Stage 3+, THE Event_Planning_System SHALL unlock Large venues: Convention Centers, Conference Center Hotels [Post-MVP]
11. WHEN in Stage 4+, THE Event_Planning_System SHALL unlock Uptown venues: Luxury Hotels, Estates, Rooftops [Post-MVP]
12. WHEN in Stage 5, THE Event_Planning_System SHALL unlock Waterfront venues: Beach, Garden estates [Post-MVP]

### Requirement 10: Event Task Management

**User Story:** As a player, I want to manage preparation tasks leading up to an event, so that I can ensure everything is ready on time.

#### Acceptance Criteria

1. WHEN an event enters Planning status, THE Event_Planning_System SHALL generate required preparation tasks with deadlines [MVP]
2. WHEN viewing tasks, THE Event_Planning_System SHALL display task name, deadline, work hours required, and dependencies [MVP]
3. WHEN a task deadline passes without completion, THE Event_Planning_System SHALL mark it as Failed with consequences [MVP]
4. WHEN the player completes a task, THE Event_Planning_System SHALL update task status and unlock dependent tasks [MVP]
5. WHILE in Stage 2, THE Event_Planning_System SHALL allow some tasks to be handled by employer resources [MVP]
6. IF critical tasks are incomplete at event time, THEN THE Event_Planning_System SHALL apply negative modifiers to satisfaction [MVP]
7. THE Event_Planning_System SHALL give the player a daily work capacity of 8 work hours per in-game day [MVP] [Phase:Stage2-Unlock]
8. WHEN the player starts a task, THE Event_Planning_System SHALL deduct the task's work hours from the daily capacity [MVP] [Phase:Stage2-Unlock]
9. WHEN the daily work capacity reaches 0, THE Event_Planning_System SHALL prevent starting new tasks until the next in-game day [MVP] [Phase:Stage2-Unlock]
10. WHEN a new in-game day begins, THE Event_Planning_System SHALL reset the player's daily work capacity to 8 hours [MVP] [Phase:Stage2-Unlock]
11. THE Event_Planning_System SHALL display remaining daily work hours in the UI [MVP] [Phase:Stage2-Unlock]
11a. WHILE in Stage 1, THE Event_Planning_System SHALL use simplified task system: tasks have deadlines only, no work hour tracking [MVP] [Phase:Launch-Simple]
12. WHILE in Stage 2, THE Event_Planning_System SHALL allow the player to request company help on tasks for a 10% profit reduction on that event [MVP]
13. WHEN company help is requested, THE Event_Planning_System SHALL complete the task without consuming player work hours [MVP]
14. WHEN company help is used, THE Event_Planning_System SHALL have a 25% chance to grant a small reputation bonus (mentor noticed your delegation skills) [MVP]
15. THE Event_Planning_System SHALL limit company help requests to 2 per event [MVP]
16. WHEN the player has exhausted daily work hours, THE Monetization_System SHALL offer a rewarded ad for overtime hours [MVP]
17. WHEN the player watches an overtime ad, THE Monetization_System SHALL grant 4 bonus work hours for that day [MVP]
18. THE Monetization_System SHALL limit overtime ads to 2 per in-game day [MVP]
19. THE Event_Planning_System SHALL allow players to let tasks fail rather than use overtime, accepting the satisfaction consequences [MVP]

### Requirement 11: In-Game Time System

**User Story:** As a player, I want events to have realistic timelines with in-game days passing, so that I can plan ahead and manage multiple events at different phases.

#### Acceptance Criteria

1. THE Time_System SHALL track in-game days that pass during gameplay sessions [MVP]
2. THE Time_System SHALL define time passage rates by stage: Stage 1 = 1 in-game day per 3 real minutes, Stage 2 = 1 day per 2.5 minutes, Stage 3 = 1 day per 2 minutes, Stage 4 = 1 day per 1.5 minutes, Stage 5 = 1 day per 1 minute [MVP]
3. WHEN a client inquiry is accepted, THE Event_Planning_System SHALL schedule the event date based on complexity: Low = 3-7 days, Medium = 7-14 days, High = 14-21 days, Very High = 21-30 days [MVP]
4. THE Event_Planning_System SHALL define event phases: Booking (days 1-2), Pre-Planning (25% of remaining time), Active Planning (50% of remaining time), Final Prep (20% of remaining time), Execution Day (last day), Results (post-event) [MVP]
5. THE Event_Planning_System SHALL assign stress weights to phases: Booking = 0.5x, Pre-Planning = 0.75x, Active Planning = 1.0x, Final Prep = 1.5x, Execution Day = 2.0x [MVP]
6. WHEN calculating workload penalties, THE Event_Planning_System SHALL multiply base penalty by the stress weight of each event's current phase [MVP]
7. WHEN two or more events are in Final Prep or Execution Day simultaneously, THE Event_Planning_System SHALL apply an additional 10% stress penalty per overlapping high-stress event [MVP]
8. THE Time_System SHALL pause when the app is backgrounded and resume when foregrounded [MVP]
9. THE Time_System SHALL display current in-game date and upcoming event dates in the Calendar app [MVP]
10. WHEN the player has no active events, THE Time_System SHALL offer a "Skip to Next Inquiry" option to advance time [MVP]
11. THE Time_System SHALL support time-skip via rewarded ads or premium currency for waiting periods [MVP]
12. WHEN Premium Idle Mode is active, THE Time_System SHALL continue time passage while the app is backgrounded [Post-MVP]
13. WHEN Premium Idle Mode is active and time passes in background, THE Event_Planning_System SHALL auto-complete tasks in queue order without consuming work hours [Post-MVP]
14. WHEN Premium Idle Mode is active, THE Event_Planning_System SHALL NOT apply satisfaction penalties for tasks that miss deadlines during background time [Post-MVP]
15. WHEN Premium Idle Mode is active and an event reaches a decision point (vendor selection, budget allocation, random event), THE Event_Planning_System SHALL pause that event until the player returns [Post-MVP]
16. WHEN the player returns from background with Premium Idle Mode, THE Time_System SHALL display a summary of time passed, tasks completed, and pending decisions [Post-MVP]

### Requirement 12: Event Execution and Random Events

**User Story:** As a player, I want to see my event unfold with potential surprises, so that I can experience the consequences of my planning decisions.

#### Acceptance Criteria

1. WHEN an event date arrives, THE Event_Planning_System SHALL transition the event to Executing status [MVP]
2. WHEN the event executes, THE Consequence_System SHALL evaluate vendor reliability and trigger potential issues [MVP]
3. WHEN a random event occurs, THE Consequence_System SHALL apply modifiers based on player preparation [MVP]
4. IF a vendor no-shows and no backup exists, THEN THE Consequence_System SHALL apply severe satisfaction penalties [MVP]
5. IF weather affects an outdoor venue without backup, THEN THE Consequence_System SHALL apply satisfaction penalties [MVP]
6. WHEN contingency budget exists, THE Consequence_System SHALL allow spending to mitigate random event impacts [MVP]
7. WHEN the event completes, THE Event_Planning_System SHALL transition to results calculation [MVP]
8. WHEN equipment failure occurs, THE Consequence_System SHALL check for contingency budget or backup rentals [MVP]
9. WHEN guest conflicts arise, THE Consequence_System SHALL evaluate seating plan and staff quality [MVP]
10. WHEN client makes last-minute changes, THE Consequence_System SHALL evaluate vendor flexibility and contingency budget [MVP]

### Requirement 13: Satisfaction Calculation and Results

**User Story:** As a player, I want to see detailed results of my events, so that I can understand what went well and what to improve.

#### Acceptance Criteria

1. WHEN an event completes, THE Satisfaction_Calculator SHALL compute scores for venue, food, entertainment, decorations, service, and expectations [MVP]
2. THE Satisfaction_Calculator SHALL weight scores as: Venue 20%, Food 25%, Entertainment 20%, Decorations 15%, Service 10%, Expectations 10% [MVP]
3. WHEN random events occurred, THE Satisfaction_Calculator SHALL apply the random event modifier to the base score [MVP]
4. WHEN displaying results, THE Event_Planning_System SHALL show individual category scores and final satisfaction as a 0-100 score [MVP] [Phase:Stage2-Unlock]
4a. WHILE in Stage 1, THE Event_Planning_System SHALL show only the final satisfaction score (0-100), not individual category breakdowns [MVP] [Phase:Launch-Simple]
5. WHEN satisfaction meets client threshold, THE Event_Planning_System SHALL mark the event as successful [MVP]
6. WHEN displaying results, THE Event_Planning_System SHALL show profit/loss calculation and reputation change [MVP]
7. THE Event_Planning_System SHALL generate client feedback text based on satisfaction level and notable issues [MVP]
8. THE Satisfaction_Calculator SHALL clamp the final score to minimum 0 and maximum 100, preventing overflow from stacked bonuses or penalties [MVP]

### Requirement 14: Reputation and Progression

**User Story:** As a player, I want my reputation to reflect my performance and unlock new opportunities, so that I feel a sense of progression with increasing challenge.

#### Acceptance Criteria

1. WHEN an event completes, THE Progression_System SHALL adjust reputation based on satisfaction level [MVP]
2. WHEN satisfaction is 90-100%, THE Progression_System SHALL add 15-25 reputation and trigger referral chance [MVP]
3. WHEN satisfaction is 75-89%, THE Progression_System SHALL add 5-14 reputation with 50% referral chance [MVP]
4. WHEN satisfaction is 60-74%, THE Progression_System SHALL add 1-4 reputation with no referral [MVP]
5. WHEN satisfaction is 40-59%, THE Progression_System SHALL subtract 5-10 reputation [MVP]
6. WHEN satisfaction is below 40%, THE Progression_System SHALL subtract 15-25 reputation and generate negative review [MVP]
7. WHEN the player reaches 25 reputation and $5,000 savings in Stage 1, THE Progression_System SHALL unlock Stage 2 [MVP]
8. WHEN Stage 2 unlocks, THE Progression_System SHALL present the option to join an established event planning company [MVP]
9. WHILE in Stage 2, THE Progression_System SHALL track employee level and performance reviews [MVP]
10. WHEN the player reaches Senior Planner (Level 5), 50 personal reputation, and $25,000 savings in Stage 2, THE Progression_System SHALL unlock Stage 3 choice [MVP]
11. WHEN the player reaches 100 reputation and $75,000 savings in Stage 3, THE Progression_System SHALL unlock Stage 4 [Post-MVP]
12. WHEN the player reaches 200 reputation and $250,000 savings in Stage 4, THE Progression_System SHALL unlock Stage 5 [Post-MVP]
13. THE Progression_System SHALL apply personality distribution: Stage 1 = 50% Easy-Going, 30% Budget-Conscious, 20% Perfectionist; Stage 2 = 40% Easy-Going, 35% Budget-Conscious, 25% Perfectionist; Stage 3+ = 33% Easy-Going, 33% Budget-Conscious, 34% Perfectionist [MVP]
14. THE Progression_System SHALL increase random event frequency by stage: Stage 1 = 20% chance, Stage 2 = 35% chance, Stage 3 = 50% chance, Stage 4 = 65% chance, Stage 5 = 80% chance [MVP]
15. IF player reputation drops below 0, THEN THE Progression_System SHALL trigger game over with option to restart or load save [MVP]
16. THE Progression_System SHALL require maintaining minimum reputation thresholds: Stage 2 requires 10+, Stage 3 requires 30+, Stage 4 requires 75+, Stage 5 requires 150+ [MVP]
17. IF player reputation drops below stage minimum threshold for 3 consecutive failed events, THEN THE Progression_System SHALL demote player to previous stage after warning [MVP]
18. THE Progression_System SHALL apply hidden reputation bonuses when the player uses personal funds to cover client-driven issues or unforeseen problems (not displayed to player) [MVP]
19. THE Progression_System SHALL apply hidden reputation penalties when the player significantly overcharges or takes advantage of Easy-Going clients (not displayed to player) [MVP]
20. THE Progression_System SHALL track hidden "going above and beyond" metrics that influence referral rates and future client quality (not displayed to player) [MVP]
21. THE Progression_System SHALL allow players to discover these hidden mechanics through gameplay experience rather than explicit tutorials [MVP]

### Requirement 15: Client Personality System

**User Story:** As a player, I want clients to have distinct personalities affecting their expectations, so that I must adapt my approach to each client.

#### Acceptance Criteria

1. WHEN generating a client in Stage 1-2, THE Event_Planning_System SHALL assign one of three personalities: Easy-Going, Budget-Conscious, or Perfectionist [MVP]
2. WHEN a client is Easy-Going, THE Satisfaction_Calculator SHALL use a 50 satisfaction threshold [MVP]
3. WHEN a client is Budget-Conscious, THE Satisfaction_Calculator SHALL use a 60 threshold and penalize overspending [MVP]
4. WHEN a client is Perfectionist, THE Satisfaction_Calculator SHALL use an 85 threshold and weight quality scores higher [MVP]
5. WHEN displaying client information, THE Event_Planning_System SHALL show personality type and communication style [MVP]
6. THE Event_Planning_System SHALL vary client behavior in messages based on personality type [MVP]
7. WHEN the player reaches Stage 3, THE Progression_System SHALL unlock the Indecisive personality type (65 threshold, requires flexibility, frequent change requests) [Post-MVP]
8. WHEN the player reaches Stage 4, THE Progression_System SHALL unlock the Demanding personality type (80 threshold, high pay but very difficult to please) [Post-MVP]
9. WHEN the player reaches Stage 5, THE Progression_System SHALL unlock the Celebrity personality type (wildcard behavior, very high pay, reputation multiplier) [Post-MVP]
10. WHEN a client is Celebrity, THE Event_Planning_System SHALL randomly assign behavior traits from other personality types, changing unpredictably during the event [Post-MVP]
11. THE Event_Planning_System SHALL pay Celebrity clients at 3x the normal budget range for their event type [Post-MVP]
12. WHEN a Celebrity inquiry arrives, THE Event_Planning_System SHALL display current press coverage status: Positive, Neutral, or Negative [Post-MVP]
13. THE Event_Planning_System SHALL generate Celebrity press coverage with probability: 40% Positive, 35% Neutral, 25% Negative [Post-MVP]
14. WHEN a Celebrity event succeeds with Positive press coverage, THE Progression_System SHALL apply 3x reputation gain AND generate 2 guaranteed referral opportunities [Post-MVP]
15. WHEN a Celebrity event succeeds with Neutral press coverage, THE Progression_System SHALL apply 2x reputation gain AND generate 1 guaranteed referral opportunity [Post-MVP]
16. WHEN a Celebrity event succeeds with Negative press coverage, THE Progression_System SHALL apply 1.5x reputation gain with no referral (damage control success) [Post-MVP]
17. WHEN a Celebrity event fails with Positive press coverage, THE Progression_System SHALL apply 2x reputation loss (wasted opportunity), capped at -50 maximum loss [Post-MVP]
18. WHEN a Celebrity event fails with Neutral press coverage, THE Progression_System SHALL apply 2.5x reputation loss, capped at -50 maximum loss [Post-MVP]
19. WHEN a Celebrity event fails with Negative press coverage, THE Progression_System SHALL apply 3x reputation loss (capped at -50 maximum loss) AND generate negative media coverage affecting future inquiries [Post-MVP]
19a. THE Progression_System SHALL cap all Celebrity event reputation losses at -50 to prevent single-event catastrophic reputation damage [Post-MVP]
20. WHEN a Celebrity client's satisfaction exceeds 90%, THE Event_Planning_System SHALL mark them as a retained client with 50% chance of repeat booking [Post-MVP]
21. WHEN a retained Celebrity books again, THE Event_Planning_System SHALL improve their press coverage probability by one tier [Post-MVP]
22. THE Event_Planning_System SHALL NOT generate Indecisive, Demanding, or Celebrity clients before their respective stage unlocks [MVP]

### Requirement 16: Stage 2 Employee Mechanics

**User Story:** As a player in Stage 2, I want to work for an established company with salary and oversight, so that I can learn and build savings before starting my own business.

#### Acceptance Criteria

1. WHEN the player enters Stage 2, THE Progression_System SHALL assign them to "Premier Events Co." as Junior Planner (Level 1) [MVP]
2. THE Progression_System SHALL define Stage 2 employee levels: Level 1-2 = Junior Planner, Level 3-4 = Planner, Level 5 = Senior Planner [MVP]
3. THE Event_Planning_System SHALL pay company event compensation by level: Junior Planner = $500 base + 5% commission, Planner = $750 base + 10% commission, Senior Planner = $1,000 base + 15% commission [MVP]
4. WHILE in Stage 2, THE Event_Planning_System SHALL provide access to company vendor network with better rates [MVP]
5. THE Progression_System SHALL conduct performance reviews after every 3 completed company events [MVP]
6. THE Progression_System SHALL evaluate performance reviews based on: average satisfaction score, on-time task completion rate, and budget management [MVP]
7. WHEN a performance review is positive (avg satisfaction 70%+, 80%+ tasks on time), THE Progression_System SHALL grant progress toward next employee level [MVP]
8. WHEN a performance review is negative (avg satisfaction below 60% OR less than 60% tasks on time), THE Progression_System SHALL issue a warning [MVP]
9. IF the player receives 2 consecutive negative performance reviews, THEN THE Progression_System SHALL demote the player one employee level (minimum Level 1) [MVP]
10. IF the player receives 3 consecutive negative performance reviews at Level 1, THEN THE Progression_System SHALL terminate employment and return player to Stage 1 [MVP]
11. THE Event_Planning_System SHALL assign company events with guest count and budget scaled by employee level: Junior = 50-75% of event type max, Planner = 75-100% of max, Senior = 100-150% of max [MVP]
12. WHILE at Junior Planner level, THE Event_Planning_System SHALL provide basic mentor tips ("Remember to allocate contingency budget") [MVP]
13. WHILE at Planner level, THE Event_Planning_System SHALL provide intermediate mentor tips ("This client type prefers X") [MVP]
14. WHILE at Senior Planner level, THE Event_Planning_System SHALL provide strategic mentor tips ("This vendor has reliability issues, consider backup") [MVP]
15. WHEN the player reaches Senior Planner (Level 5), 50 personal reputation, and $25,000 savings, THE Progression_System SHALL unlock Stage 3 choice [MVP]
16. WHEN Stage 3 unlocks, THE Progression_System SHALL offer choice: Accept Director promotion OR Leave to start own company [MVP]
17. IF the player chooses Director promotion, THEN THE Progression_System SHALL enter Stage 3 as company Director with $1,500 base + 20% commission and continued company resources [MVP]
18. IF the player chooses to start own company, THEN THE Progression_System SHALL enter Stage 3 as independent business owner keeping 100% profit but losing company vendor discounts [MVP]
19. WHILE in Stage 2, THE Event_Planning_System SHALL allow the player to accept personal side gigs (Stage 1 event types) in addition to company assignments [MVP]
20. WHEN the player accepts a side gig, THE Event_Planning_System SHALL keep 100% of side gig profits but not provide company vendor discounts [MVP]
21. WHEN the player has active side gigs, THE Event_Planning_System SHALL count them toward concurrent event capacity alongside company events [MVP]
22. IF a side gig conflicts with a company event deadline, THEN THE Event_Planning_System SHALL apply performance review penalties [MVP]
23. THE Event_Planning_System SHALL limit side gigs to 2 concurrent events maximum while employed [MVP]
24. WHEN a side gig succeeds, THE Progression_System SHALL add to personal reputation but not company reputation [MVP]

### Requirement 17: Stage 3 Midpoint Milestone

**User Story:** As a player reaching Stage 3, I want to experience a satisfying narrative conclusion, so that I feel my journey has been meaningful even if I don't continue to later stages.

#### Acceptance Criteria

1. WHEN the player meets Stage 3 unlock requirements, THE Progression_System SHALL trigger the Stage 3 Milestone sequence before presenting the path choice [MVP]
2. THE Progression_System SHALL display a Career Summary Screen showing: total events completed, first event name, highest satisfaction event, total money earned, and current reputation [MVP]
3. THE Progression_System SHALL display a narrative moment based on chosen path: office lease signing (Entrepreneur) or Director announcement ceremony (Corporate) [MVP]
4. FOR the Entrepreneur path narrative, THE UI_Manager SHALL show: signing lease on first office, former client sending congratulatory flowers, family phone call expressing pride, small newspaper feature about the new business [MVP]
5. FOR the Corporate path narrative, THE UI_Manager SHALL show: company meeting announcement, mentor speech acknowledging growth, reserved parking spot reveal, name plaque on office door [MVP]
6. WHEN the narrative moment completes, THE Progression_System SHALL display a Credits Sequence acknowledging the player's achievement [MVP]
7. THE Credits Sequence SHALL include game credits followed by "Your story continues..." messaging [MVP]
8. AFTER the Credits Sequence, THE UI_Manager SHALL clearly label Stages 4-5 content as "Expansion Mode" or "Endgame Content" [MVP]
9. THE Achievement_System SHALL award "Going Pro" achievement during the Stage 3 Milestone sequence [MVP]
10. THE Progression_System SHALL allow players to skip the narrative/credits sequence on subsequent playthroughs [MVP]

### Requirement 18: Stages 3-5 Business Progression (Endgame)

**User Story:** As a business owner or company executive, I want to grow my career through either entrepreneurship or corporate advancement, so that I can experience different paths to success.

#### Acceptance Criteria

1. THE Progression_System SHALL track two parallel career paths from Stage 3: Entrepreneur Path and Corporate Path [MVP]

**Entrepreneur Path (Left company in Stage 3):**
2. WHEN the player enters Stage 3 as entrepreneur, THE Progression_System SHALL enable small office purchase with +10% efficiency bonus [Post-MVP]
3. WHEN in Stage 3 as entrepreneur, THE Staff_System SHALL allow hiring first assistant [Post-MVP]
4. WHEN the player enters Stage 4 as entrepreneur, THE Progression_System SHALL enable agency office upgrade with +20% efficiency [Post-MVP]
5. WHEN in Stage 4 as entrepreneur, THE Staff_System SHALL allow hiring multiple specialists (coordinator, designer, vendor manager) [Post-MVP]
6. WHEN the player enters Stage 5 as entrepreneur, THE Progression_System SHALL enable flagship office/showroom with +30% efficiency [Post-MVP]
7. WHEN in Stage 5 as entrepreneur, THE Event_Planning_System SHALL enable delegation of most work to focus on key decisions [Post-MVP]
8. THE Staff_System SHALL allow hiring large team with department heads in Stage 5 entrepreneur path [Post-MVP]

**Corporate Path (Stayed with company in Stage 3):**
9. WHEN the player enters Stage 3 as Director, THE Progression_System SHALL provide company office with +10% efficiency bonus [Post-MVP]
10. WHEN in Stage 3 as Director, THE Staff_System SHALL allow hiring one company-funded assistant [Post-MVP]
11. WHEN the player reaches Stage 4 requirements as Director, THE Progression_System SHALL promote to Vice President opening a new regional office [Post-MVP]
12. WHEN in Stage 4 as VP, THE Progression_System SHALL provide regional office with +20% efficiency and company-funded specialists [Post-MVP]
13. WHEN the player reaches Stage 5 requirements as VP, THE Progression_System SHALL promote to Partner/Executive with flagship division [Post-MVP]
14. WHEN in Stage 5 as Partner, THE Event_Planning_System SHALL enable delegation with company support infrastructure [Post-MVP]

**Path Differences:**
15. WHILE on Entrepreneur Path, THE Event_Planning_System SHALL keep 100% of event profits but require paying all staff salaries [Post-MVP]
16. WHILE on Corporate Path, THE Event_Planning_System SHALL pay higher base salary + commission but cap maximum earnings per event [Post-MVP]
17. WHILE on Corporate Path, THE Progression_System SHALL provide company-funded staff and resources but require meeting company performance targets [Post-MVP]

### Requirement 19: Staff Management System (Endgame)

**User Story:** As a player, I want to hire and manage staff, so that I can handle more events and delegate tasks.

#### Acceptance Criteria

1. WHEN in Stage 3+ on Entrepreneur Path, THE Staff_System SHALL allow hiring staff with defined skills, roles, and salaries paid by the player [Post-MVP]
2. WHEN in Stage 3+ on Corporate Path, THE Staff_System SHALL provide company-funded staff assigned to the player [Post-MVP]
3. WHEN assigning staff to events, THE Staff_System SHALL match staff specialties to event types [Post-MVP]
4. WHEN staff completes tasks, THE Staff_System SHALL apply their efficiency rating to task completion [Post-MVP]
5. WHEN staff reliability is low, THE Staff_System SHALL occasionally cause task failures [Post-MVP]
6. WHILE on Entrepreneur Path, THE Staff_System SHALL track staff salary as ongoing expenses deducted from player funds [Post-MVP]
7. WHILE on Corporate Path, THE Staff_System SHALL track staff performance which affects player's corporate standing [Post-MVP]
8. WHEN staff successfully completes events, THE Staff_System SHALL improve their skill level over time [Post-MVP]
9. WHILE on Entrepreneur Path, THE Staff_System SHALL allow the player to hire and fire staff freely [Post-MVP]
10. WHILE on Corporate Path, THE Staff_System SHALL require company approval for staff changes [Post-MVP]

### Requirement 20: Vendor Relationship System

**User Story:** As a player, I want to build relationships with vendors over time, so that I can unlock benefits and hidden information.

#### Acceptance Criteria

1. WHEN the player hires a vendor, THE Event_Planning_System SHALL increment the relationship counter [MVP] 
2. WHEN relationship level reaches 3, THE Event_Planning_System SHALL reveal vendor's hidden reliability attribute [MVP] [Phase:Stage3-Unlock]
3. WHEN relationship level reaches 5, THE Event_Planning_System SHALL reveal vendor's hidden flexibility attribute [MVP] [Phase:Stage3-Unlock]
3a. WHILE in Stage 1-2, THE Event_Planning_System SHALL track vendor relationships but not reveal hidden attributes (system operates invisibly) [MVP] [Phase:Launch-Simple]
4. WHEN relationship level reaches certain thresholds, THE Event_Planning_System SHALL unlock discount rates [Post-MVP]
5. WHEN a vendor performs poorly, THE Event_Planning_System SHALL allow the player to leave a note for future reference [Post-MVP]
6. WHILE on Corporate Path, THE Event_Planning_System SHALL provide access to company vendor network with pre-negotiated rates [MVP]
7. WHILE on Entrepreneur Path, THE Event_Planning_System SHALL require the player to build vendor relationships from scratch after leaving company [MVP]
8. WHEN transitioning from Corporate to Entrepreneur Path, THE Event_Planning_System SHALL retain personal vendor relationships built during employment [MVP]

### Requirement 21: Office Progression

**User Story:** As a player, I want to see my workspace evolve as I progress, so that I can see tangible evidence of my career growth.

#### Acceptance Criteria

1. WHILE in Stage 1, THE Progression_System SHALL show home office as player's work base [MVP]
2. WHEN the player enters Stage 2, THE Progression_System SHALL show "Premier Events Co." office as work location [MVP]
3. THE Map_System SHALL display the player's current office location on the map appropriately [MVP]

**Entrepreneur Path Offices:**
4. WHEN the player enters Stage 3 as entrepreneur, THE Progression_System SHALL enable small office purchase with +10% efficiency [Post-MVP]
5. WHEN the player enters Stage 4 as entrepreneur, THE Progression_System SHALL enable agency office upgrade with +20% efficiency [Post-MVP]
6. WHEN the player enters Stage 5 as entrepreneur, THE Progression_System SHALL unlock flagship office/showroom with +30% efficiency [Post-MVP]
7. WHILE on Entrepreneur Path, THE Progression_System SHALL require the player to pay office rent/mortgage as ongoing expense [Post-MVP]

**Corporate Path Offices:**
8. WHEN the player enters Stage 3 as Director, THE Progression_System SHALL provide company Director's office with +10% efficiency [Post-MVP]
9. WHEN the player enters Stage 4 as VP, THE Progression_System SHALL provide regional office headquarters with +20% efficiency [Post-MVP]
10. WHEN the player enters Stage 5 as Partner, THE Progression_System SHALL provide executive suite in flagship building with +30% efficiency [Post-MVP]
11. WHILE on Corporate Path, THE Progression_System SHALL provide office space at no cost to the player [Post-MVP]

### Requirement 22: Marketing System (Endgame)

**User Story:** As a business owner, I want to invest in marketing to attract more clients, so that I can grow my business faster.

#### Acceptance Criteria

1. WHEN the player enters Stage 3 on Entrepreneur Path, THE Marketing_System SHALL unlock marketing investment options [Post-MVP]
2. WHEN the player enters Stage 3 on Entrepreneur Path, THE Event_Planning_System SHALL silently reduce base inquiry rate by 30% (not displayed to player) [Post-MVP]
3. THE Marketing_System SHALL offer marketing tiers: Basic ($500/week, restores base inquiry rate), Standard ($1,500/week, +25% above base), Premium ($3,000/week, +50% above base) [Post-MVP]
4. WHEN no marketing is active in Stage 3+, THE Event_Planning_System SHALL use the reduced 70% base inquiry rate [Post-MVP]
5. WHEN marketing is active, THE Event_Planning_System SHALL apply the marketing tier bonus to the original (non-reduced) base inquiry rate [Post-MVP]
6. THE Marketing_System SHALL deduct marketing costs weekly from player funds [Post-MVP]
7. WHEN player funds are insufficient, THE Marketing_System SHALL automatically downgrade or cancel marketing [Post-MVP]
8. THE Marketing_System SHALL track marketing ROI and display it in the Bank app [Post-MVP]
9. WHEN in Stage 4+, THE Marketing_System SHALL unlock targeted marketing options (Corporate Focus, Wedding Focus, etc.) that increase specific event type inquiries [Post-MVP]
10. THE Marketing_System SHALL allow players to pause or cancel marketing at any time [Post-MVP]
11. WHILE on Corporate Path, THE Event_Planning_System SHALL NOT apply the 30% inquiry reduction (company handles marketing) [Post-MVP]

### Requirement 23: Referral Opportunities

**User Story:** As a successful event planner, I want to receive referral clients from satisfied customers, so that I am rewarded for maintaining high quality.

#### Acceptance Criteria

1. WHEN a client's satisfaction exceeds 90%, THE Event_Planning_System SHALL have a chance to generate a referral opportunity [MVP]
2. THE Event_Planning_System SHALL define referral chance as: 95-100% satisfaction = 80% referral chance, 90-94% satisfaction = 50% referral chance [MVP]
3. WHEN a referral opportunity is generated, THE Event_Planning_System SHALL mark it distinctly as "Referral from [Previous Client Name]" [MVP]
4. THE Event_Planning_System SHALL give referral opportunities a 15% profit bonus (client pre-trusts the planner) [MVP]
5. THE Event_Planning_System SHALL give referral clients a higher base satisfaction starting point (+10% to all category scores) [MVP]
6. THE Event_Planning_System SHALL track cumulative "excellence streak" - consecutive events with 90%+ satisfaction [MVP]
7. WHEN excellence streak reaches 3 events, THE Event_Planning_System SHALL increase referral chance by 10% [MVP]
8. WHEN excellence streak reaches 5 events, THE Event_Planning_System SHALL increase referral chance by an additional 10% (total +20%) [MVP]
9. WHEN an event scores below 80% satisfaction, THE Event_Planning_System SHALL reset the excellence streak to 0 [MVP]
10. THE Event_Planning_System SHALL treat 80% satisfaction as the optimal target baseline for normal gameplay [MVP]
11. THE Event_Planning_System SHALL limit referral opportunities to 1 per successful event (no chain referrals from referrals) [MVP]
12. WHEN a referral opportunity expires without acceptance, THE Event_Planning_System SHALL not penalize the player but the opportunity is lost [MVP]
13. THE Event_Planning_System SHALL make referrals progressively harder to maintain as concurrent event count increases (natural difficulty scaling) [MVP]

### Requirement 24: Modern Mobile UI

**User Story:** As a player, I want a polished, modern mobile interface, so that the game feels professional and enjoyable to use.

#### Acceptance Criteria

1. THE UI_Manager SHALL ensure all touch targets are minimum 44x44 points [MVP]
2. WHEN a button is pressed, THE UI_Manager SHALL provide immediate visual feedback with scale animation [MVP]
3. WHEN transitioning between screens, THE UI_Manager SHALL animate with slide or fade over 300-400ms [MVP]
4. WHEN an action succeeds, THE UI_Manager SHALL provide positive feedback with particles, scale pop, or sound [MVP]
5. WHEN an error occurs, THE UI_Manager SHALL provide feedback with shake animation and visual indicator [MVP]
6. THE UI_Manager SHALL adapt layouts for various screen sizes from iPhone SE to iPad [MVP]
7. THE UI_Manager SHALL maintain 60fps performance on iPhone 8 and equivalent Android devices [MVP]
8. THE UI_Manager SHALL use consistent color palette, typography, and icon style throughout [MVP]

### Requirement 25: Tutorial System

**User Story:** As a new player, I want guided instruction through my first event, so that I understand core game mechanics before playing independently.

#### Acceptance Criteria

1. WHEN a new game starts, THE Tutorial_System SHALL begin the guided first event sequence with an Easy-Going client [MVP]
2. WHEN teaching a mechanic, THE Tutorial_System SHALL highlight relevant UI elements and disable others [MVP]
3. WHEN the player completes a tutorial step, THE Tutorial_System SHALL advance to the next instruction [MVP]
4. THE Tutorial_System SHALL guide the player through simplified core loop: accept client, pick venue, pick caterer, event executes, see results [MVP] [Phase:Launch-Simple]
4a. THE Tutorial_System SHALL teach full mechanics (budget allocation, vendor selection, task management) only when those systems unlock in Stage 2 [MVP] [Phase:Stage2-Unlock]
5. THE Tutorial_System SHALL defer teaching daily work hours until the player reaches Stage 2 when the system activates [MVP] [Phase:Stage2-Unlock]
6. THE Tutorial_System SHALL introduce essential Phone apps only: Calendar, Messages, Bank [MVP] [Phase:Launch-Simple]
6a. THE Tutorial_System SHALL introduce Contacts, Tasks, and Clients apps contextually as players use them [MVP]
7. THE Tutorial_System SHALL demonstrate weather awareness with simplified "Good/Risky/Bad" indicator in Stage 1 [MVP] [Phase:Launch-Simple]
7a. THE Tutorial_System SHALL teach full weather forecast checking when the system unlocks in Stage 2 [MVP] [Phase:Stage2-Unlock]
8. THE Tutorial_System SHALL explain contingency budget allocation with a vague hint: "Experienced planners always keep something in reserve for surprises" [MVP]
9. THE Tutorial_System SHALL NOT explicitly teach hidden mechanics (reputation bonuses, going above and beyond, client exploitation penalties) [MVP]
10. THE Tutorial_System SHALL include subtle hints that encourage discovery: "Some say the best planners develop a sixth sense for vendor reliability over time" [MVP]
11. WHEN the tutorial event completes, THE Tutorial_System SHALL show the results screen explaining satisfaction scoring and reputation [MVP]
12. WHEN the tutorial event completes, THE Tutorial_System SHALL mark the tutorial as complete and enable free play [MVP]
13. IF the player has completed the tutorial, THEN THE Tutorial_System SHALL not show tutorial prompts on subsequent sessions [MVP]
14. THE Tutorial_System SHALL allow the player to skip the tutorial if they choose, with a confirmation warning [MVP]
15. THE Tutorial_System SHALL provide a "Tips" section in the Phone that players can revisit for basic guidance [MVP]

### Requirement 26: Audio System

**User Story:** As a player, I want appropriate music and sound effects, so that the game feels polished and engaging.

#### Acceptance Criteria

1. WHEN the game is running, THE Audio_Manager SHALL play background music appropriate to the current screen [MVP]
2. WHEN the player interacts with UI elements, THE Audio_Manager SHALL play appropriate sound effects [MVP]
3. WHEN an event succeeds, THE Audio_Manager SHALL play celebratory audio feedback [MVP]
4. WHEN an event fails, THE Audio_Manager SHALL play appropriate negative audio feedback [MVP]
5. THE Audio_Manager SHALL respect device volume settings and allow muting music and SFX independently [MVP]
6. WHEN the game loses focus, THE Audio_Manager SHALL pause audio playback [MVP]

### Requirement 27: Data Persistence and Serialization

**User Story:** As a player, I want my game data to be reliably saved and loaded, so that I never lose progress.

#### Acceptance Criteria

1. WHEN saving game state, THE Save_System SHALL serialize all player data, active events, and event history to JSON [MVP]
2. WHEN loading game state, THE Save_System SHALL deserialize saved data and restore all game objects [MVP]
3. THE Save_System SHALL include a version number in save files for migration support [MVP]
4. WHEN a save file version is outdated, THE Save_System SHALL migrate data to the current format [MVP]
5. THE Save_System SHALL store save files in the platform-appropriate persistent data path [MVP]
6. IF serialization fails, THEN THE Save_System SHALL log the error and attempt recovery from backup [MVP]
7. THE Save_System SHALL create automatic backups before major save operations [MVP]
8. THE Save_System SHALL support cloud save synchronization via Unity Cloud Save or platform services (Game Center, Google Play Games) [Post-MVP]
9. WHEN cloud save is enabled, THE Save_System SHALL sync local saves to cloud on major progress milestones (stage completion, significant purchases) [Post-MVP]
10. THE Save_System SHALL support optional player account creation for cross-device sync [Post-MVP]
11. WHEN a player logs into an account on a new device, THE Save_System SHALL offer to download cloud save or keep local save [Post-MVP]
12. THE Save_System SHALL handle cloud save conflicts by presenting both options to the player with timestamps [Post-MVP]

### Requirement 28: Monetization Foundation

**User Story:** As a player, I want optional ways to enhance my experience through purchases, so that I can support the game while gaining benefits.

#### Acceptance Criteria

1. THE Monetization_System SHALL integrate Unity IAP for in-app purchase handling [MVP]
2. THE Monetization_System SHALL integrate Unity Ads for rewarded and interstitial ad placements [MVP]
3. WHEN the player watches a rewarded ad, THE Monetization_System SHALL grant the specified bonus (extra money, reputation boost, time skip, overtime hours, etc.) [MVP]
4. WHEN the player initiates a purchase, THE Monetization_System SHALL process through Unity IAP and grant items on success [MVP]
5. IF a purchase fails, THEN THE Monetization_System SHALL display an error message and not grant items [MVP]
6. THE Monetization_System SHALL validate purchases and prevent exploitation [MVP]
7. WHEN restoring purchases, THE Monetization_System SHALL restore all previously purchased non-consumable items [MVP]
8. THE Monetization_System SHALL support consumable IAPs (currency packs), non-consumable IAPs (permanent unlocks), and subscription IAPs (VIP benefits) [MVP]
9. THE Monetization_System SHALL integrate Unity Gaming Services for analytics and remote configuration [MVP]
10. THE Monetization_System SHALL track all ad views and purchases for analytics [MVP]

### Requirement 29: In-App Purchase Products

**User Story:** As a player, I want various purchase options at different price points, so that I can choose how much to invest in the game.

#### Acceptance Criteria

1. THE Monetization_System SHALL offer currency packs: Small ($0.99), Medium ($4.99), Large ($9.99), and Mega ($19.99) [MVP]
2. THE Monetization_System SHALL offer a Starter Pack one-time purchase with currency, premium vendor unlock, and cosmetic item [MVP]
3. THE Monetization_System SHALL offer permanent unlocks: Premium Venue Pack, Elite Vendor Network, No Ads [MVP]
4. THE Monetization_System SHALL offer a VIP Subscription with daily rewards, exclusive content access, ad-free experience, and Premium Idle Mode [MVP]
5. WHEN displaying IAP options, THE Monetization_System SHALL show localized pricing from the app store [MVP]
6. THE Monetization_System SHALL track purchase history and prevent duplicate non-consumable purchases [MVP]
7. THE Monetization_System SHALL offer Premium Idle Mode as a standalone permanent unlock enabling background time progression [Post-MVP]
8. WHEN Premium Idle Mode is purchased, THE Monetization_System SHALL unlock background time passage and remove task failure penalties during idle time [Post-MVP]
9. THE Monetization_System SHALL offer Business Upgrade Bundles: Office Starter Kit (furniture + basic equipment), Professional Bundle (AV suite + vehicle), Executive Package (all upgrades at 20% discount) [Post-MVP]
10. THE Monetization_System SHALL offer Training Fast-Track pack that unlocks all certifications [Post-MVP]

### Requirement 30: Advertisement Integration

**User Story:** As a player, I want to optionally watch ads for bonuses, so that I can progress without spending money.

#### Acceptance Criteria

1. WHEN a random event causes problems and contingency is insufficient, THE Monetization_System SHALL offer a rewarded ad to unlock emergency mitigation options [MVP]
2. WHEN the player exhausts daily work hours, THE Monetization_System SHALL offer a rewarded ad for 4 overtime hours (max 2 per day) [MVP]
3. WHEN the player is low on funds (below $500), THE Monetization_System SHALL offer a rewarded ad to unlock the Emergency Funding request option [MVP]
4. THE Monetization_System SHALL limit rewarded ad availability to prevent abuse (cooldown timers) [MVP]
5. THE Monetization_System SHALL show interstitial ads at natural break points (between events, after results screen) for non-VIP players [MVP]
6. THE Monetization_System SHALL respect player choice and never force ad viewing for core gameplay [MVP]
7. IF ads fail to load, THEN THE Monetization_System SHALL gracefully hide ad options without breaking gameplay [MVP]
8. THE Monetization_System SHALL offer time-skip ads when waiting for event dates or inquiry cooldowns [MVP]
9. WHILE on Corporate Path in Stage 2, THE Monetization_System SHALL NOT show interstitial ads (company benefit) [MVP]
10. THE Monetization_System SHALL support both free-to-play (with ads) and premium (no ads) distribution models [MVP]

### Requirement 31: Unity Gaming Services Integration

**User Story:** As a developer, I want to leverage Unity's backend services, so that I can track analytics, configure the game remotely, and manage player engagement.

#### Acceptance Criteria

1. THE Game_Manager SHALL integrate Unity Analytics for tracking player behavior and game events [MVP]
2. THE Game_Manager SHALL integrate Unity Remote Config for server-side game balance adjustments [MVP]
3. THE Game_Manager SHALL integrate Unity Cloud Diagnostics for crash reporting and performance monitoring [MVP]
4. WHEN the game launches, THE Game_Manager SHALL initialize Unity Gaming Services with appropriate project credentials [MVP]
5. THE Game_Manager SHALL track key analytics events as defined in the Analytics Events List below [MVP]
6. THE Game_Manager SHALL support A/B testing through Remote Config for monetization optimization [Post-MVP]

**Analytics Events List (MVP):**
- `tutorial_started`: When tutorial begins
- `tutorial_completed`: When tutorial finishes
- `tutorial_skipped`: When player skips tutorial
- `event_accepted`: When player accepts a client inquiry (with event_type, budget_range, stage)
- `event_completed`: When event finishes (with satisfaction_score, profit, event_type)
- `event_failed`: When satisfaction drops below threshold (with satisfaction_score, event_type)
- `stage_advanced`: When player advances to next stage (with new_stage, total_playtime)
- `stage_3_path_chosen`: When player chooses Entrepreneur or Corporate (with path_choice)
- `vendor_booked`: When vendor is hired (with vendor_category, vendor_tier, relationship_level)
- `venue_booked`: When venue is selected (with venue_type, capacity_utilization)
- `purchase_initiated`: When IAP flow starts (with product_id)
- `purchase_completed`: When IAP succeeds (with product_id, price)
- `purchase_failed`: When IAP fails (with product_id, error_type)
- `ad_watched`: When rewarded ad completes (with ad_placement, reward_type)
- `ad_skipped`: When player dismisses ad early (with ad_placement)
- `family_help_used`: When emergency family funding is requested (with request_number)
- `financial_crisis`: When player hits $0 (with stage, total_events_completed)
- `session_start`: When app opens (with time_since_last_session)
- `session_end`: When app closes (with session_duration)

**Free Tier Notes:**
7. THE Game_Manager SHALL operate within Unity Gaming Services free tier limits: Analytics up to 50,000 MAU, Cloud Save up to 5GB/month and 1M operations, Crash Reporting included with Unity license [MVP]
8. WHEN the game exceeds free tier limits, THE Game_Manager SHALL alert developers via Unity Dashboard for cost planning [Post-MVP]

### Requirement 32: Weather System

**User Story:** As a player, I want to check weather forecasts before booking outdoor venues, so that I can plan for inclement weather and avoid event disasters.

#### Acceptance Criteria

1. THE Weather_System SHALL generate a 7-day weather forecast for the game world [MVP] [Phase:Stage2-Unlock]
2. THE Weather_System SHALL display the forecast in the Phone Calendar app [MVP] [Phase:Stage2-Unlock]
3. WHEN the player is booking an outdoor venue, THE Weather_System SHALL display the forecasted weather for the event date [MVP]
4. THE Weather_System SHALL define weather types: Clear, Cloudy, Light Rain, Heavy Rain, Extreme Heat, Extreme Cold [MVP]
5. WHEN an outdoor event is scheduled on a day with Light Rain forecast, THE Weather_System SHALL display a warning during venue booking [MVP]
6. WHEN an outdoor event is scheduled on a day with Heavy Rain or Extreme weather forecast, THE Weather_System SHALL strongly warn the player and suggest indoor alternatives [MVP]
7. THE Weather_System SHALL allow forecasts to change as the event date approaches (70% accuracy 7 days out, 90% accuracy 2 days out, 100% accuracy day-of) [MVP] [Phase:Stage2-Unlock]
7a. WHILE in Stage 1, THE Weather_System SHALL display simplified weather as "Good", "Risky", or "Bad" indicator only, with 100% accuracy [MVP] [Phase:Launch-Simple]
8. WHEN weather changes unfavorably after booking, THE Event_Planning_System SHALL notify the player via Messages app [MVP]
9. WHEN inclement weather occurs on an outdoor event day, THE Consequence_System SHALL check for backup venue or tent rental in contingency [MVP]
10. IF no weather backup exists for an outdoor event with bad weather, THEN THE Consequence_System SHALL apply 20-40% satisfaction penalty based on severity [MVP]
11. WHEN the player books a backup indoor venue or tent rental, THE Event_Planning_System SHALL deduct from contingency budget [MVP]
12. THE Weather_System SHALL make weather patterns learnable (e.g., summer has more extreme heat risk, spring has more rain) [MVP]

### Requirement 33: Business Economics and Money Management

**User Story:** As a player, I want meaningful ways to spend my earnings on business growth and upgrades, so that financial success feels rewarding and strategic.

#### Acceptance Criteria

**Profit Margins:**
1. THE Event_Planning_System SHALL calculate player profit as 20-30% of event budget for successful events (satisfaction 70%+) [MVP]
2. THE Event_Planning_System SHALL calculate player profit as 10-15% of event budget for mediocre events (satisfaction 50-69%) [MVP]
3. THE Event_Planning_System SHALL result in break-even or loss for failed events (satisfaction below 50%) [MVP]

**Stage 3+ Ongoing Expenses (Entrepreneur Path - Weekly):**
4. THE Progression_System SHALL charge office rent: Stage 3 = $300/week, Stage 4 = $800/week, Stage 5 = $1,500/week [Post-MVP]
5. THE Staff_System SHALL charge staff salaries: Assistant = $250/week, Specialist = $400/week, Department Head = $600/week [Post-MVP]
6. THE Event_Planning_System SHALL deduct ongoing expenses automatically at the start of each in-game week [Post-MVP]
7. IF player funds drop below $1,000, THEN THE Event_Planning_System SHALL display financial warning and offer emergency options [Post-MVP]

**Optional Business Upgrades (One-Time Purchases):**
8. THE Progression_System SHALL offer Office Cosmetic Upgrades: Furniture ($2,000), Art & Decor ($3,000), Premium Renovation ($10,000) - visual progression only [Post-MVP]
9. THE Event_Planning_System SHALL offer Equipment Purchases: Basic AV Kit ($5,000, -10% AV rental costs), Professional AV Suite ($15,000, -25% AV rental costs), Tent & Outdoor Kit ($8,000, -20% outdoor equipment rentals) [Post-MVP]
10. THE Event_Planning_System SHALL offer Premium Vendor Retainers: $2,000/month per vendor guarantees availability and 10% discount [Post-MVP]
11. THE Progression_System SHALL offer Vehicle Upgrades: Reliable Car ($5,000, no travel delays), Premium Vehicle ($15,000, +5% client impression bonus) [Post-MVP]
12. THE Progression_System SHALL offer Training & Certifications: Event Type Certification ($3,000, unlocks event type one stage early), Efficiency Training ($5,000, +1 daily work hour) [Post-MVP]

**Savings and Investment:**
13. THE Bank_System SHALL offer a Business Savings Account that earns 2% weekly interest on balances over $10,000 [Post-MVP]
14. THE Bank_System SHALL display net worth tracking including cash, equipment value, and business assets [Post-MVP]

**Economic Balance Goals:**
15. THE Event_Planning_System SHALL balance economics so Stage 3 entrepreneurs can afford Basic marketing within 2-3 successful events after covering initial expenses [Post-MVP]
16. THE Event_Planning_System SHALL balance economics so players have discretionary income for optional upgrades after covering necessities [Post-MVP]
17. THE Event_Planning_System SHALL ensure ongoing expenses consume approximately 30-40% of typical weekly earnings, leaving 60-70% for savings and upgrades [Post-MVP]

### Requirement 34: Emergency Funding System

**User Story:** As a player facing financial hardship, I want realistic options to get emergency funds, so that I can recover from setbacks without breaking immersion.

#### Acceptance Criteria

**Stage 1-2: Family Support**
1. WHEN the player's funds drop below $500 in Stage 1-2, THE Bank_System SHALL enable the "Ask Family for Help" option (after watching ad if free-to-play) [MVP]
2. THE Bank_System SHALL limit family help to 3 requests total across Stage 1-2 [MVP]
3. WHEN family help is requested, THE Bank_System SHALL provide $500 with a supportive but concerned message from a family member [MVP]
4. WHEN family help is requested a second time, THE Bank_System SHALL provide $400 with a worried message [MVP]
5. WHEN family help is requested a third time, THE Bank_System SHALL provide $300 with a message indicating this is the last time they can help [MVP]
6. WHEN all family help is exhausted, THE Bank_System SHALL display "Family cannot help further" and disable the option [MVP]
7. THE Bank_System SHALL track family help usage and display remaining requests in the Bank app [MVP]

**Stage 3+: Bank Loans (Entrepreneur Path)**
8. WHEN the player's funds drop below $1,000 in Stage 3+ on Entrepreneur Path, THE Bank_System SHALL enable the "Request Bank Loan" option (after watching ad if free-to-play) [Post-MVP]
9. THE Bank_System SHALL offer loan amounts: Small ($2,000), Medium ($5,000), Large ($10,000) [Post-MVP]
10. THE Bank_System SHALL charge 15% interest on loans, due within 4 in-game weeks [Post-MVP]
11. THE Bank_System SHALL automatically deduct loan payments from event profits until repaid [Post-MVP]
12. IF a loan payment is missed, THEN THE Bank_System SHALL increase interest rate to 25% and flag credit rating [Post-MVP]
13. IF the player has a flagged credit rating, THEN THE Bank_System SHALL only offer Small loans at 30% interest [Post-MVP]
14. THE Bank_System SHALL limit active loans to 1 at a time [Post-MVP]
15. WHEN a loan is fully repaid, THE Bank_System SHALL restore credit rating after 2 successful on-time payments [Post-MVP]

**Stage 3+: Company Advance (Corporate Path)**
16. WHEN the player's funds drop below $1,000 in Stage 3+ on Corporate Path, THE Bank_System SHALL enable the "Request Company Advance" option [Post-MVP]
17. THE Bank_System SHALL offer company advances up to $3,000 with no interest but deducted from next 3 event commissions [Post-MVP]
18. THE Bank_System SHALL limit company advances to 1 per month [Post-MVP]
19. IF the player requests too many advances, THEN THE Progression_System SHALL note it in performance reviews [Post-MVP]

**Consequences and Recovery**
20. IF the player's funds reach $0 with no emergency options available, THEN THE Progression_System SHALL trigger a "Financial Crisis" event with recovery options [MVP]
21. THE Progression_System SHALL offer Financial Crisis recovery: restart stage with reduced reputation OR watch ad to receive one-time emergency grant [MVP]

### Requirement 35: Settings and Player Profile

**User Story:** As a player, I want to customize my avatar and adjust game settings, so that I can personalize my experience.

#### Acceptance Criteria

**Player Avatar:**
1. WHEN starting a new game, THE UI_Manager SHALL prompt the player to create their avatar [MVP]
2. THE UI_Manager SHALL allow the player to choose avatar appearance: face shape, skin tone, hair style, hair color, and outfit [MVP]
3. THE UI_Manager SHALL allow the player to enter their planner's name (default: "Alex") [MVP]
4. THE UI_Manager SHALL display the player's avatar on the home screen, business cards, and client-facing screens [MVP]
5. THE UI_Manager SHALL allow the player to edit avatar appearance and name from the Settings screen at any time [MVP]

**Audio Settings:**
6. THE Settings_Screen SHALL provide a Music Volume slider (0-100%) [MVP]
7. THE Settings_Screen SHALL provide a Sound Effects Volume slider (0-100%) [MVP]
8. THE Settings_Screen SHALL provide a Mute All toggle [MVP]
9. THE Audio_Manager SHALL persist audio settings across sessions [MVP]

**Display Settings:**
10. THE Settings_Screen SHALL provide notification preferences (enable/disable by type) [MVP]
11. THE Settings_Screen SHALL provide a "Show Tutorial Tips" toggle for returning players [MVP]
12. THE Settings_Screen SHALL display current game version number [MVP]

**Account and Data:**
13. THE Settings_Screen SHALL provide access to "Restore Purchases" for IAP recovery [MVP]
14. THE Settings_Screen SHALL provide a "Reset Game" option with confirmation dialog [MVP]
15. THE Settings_Screen SHALL provide links to Privacy Policy and Terms of Service [MVP]
16. THE Settings_Screen SHALL provide account login/logout options when cloud save is enabled [Post-MVP]

**Accessibility:**
17. THE Settings_Screen SHALL provide text size options (Small, Medium, Large) [MVP]
18. THE UI_Manager SHALL scale all text elements according to the selected text size setting [MVP]
19. THE Settings_Screen SHALL provide colorblind mode options (Deuteranopia, Protanopia, Tritanopia) [Post-MVP]
20. THE Settings_Screen SHALL provide a reduced motion toggle for players sensitive to animations [Post-MVP]

**Localization (Post-MVP):**
21. THE Settings_Screen SHALL provide language selection when multiple languages are supported [Post-MVP]

**Privacy and Data Compliance:**
22. THE Settings_Screen SHALL provide a "Privacy Settings" option allowing players to manage data collection preferences [MVP]
23. THE Game_Manager SHALL request user consent before enabling analytics data collection (GDPR/CCPA compliance) [MVP]
24. WHEN a player opts out of analytics, THE Game_Manager SHALL disable Unity Analytics data collection while maintaining core functionality [MVP]
25. THE Settings_Screen SHALL provide a "Delete My Data" option that clears all local save data and resets the game [MVP]
26. THE Game_Manager SHALL NOT collect or transmit personally identifiable information without explicit user consent [MVP]
27. WHEN displaying Privacy Policy or Terms of Service, THE Settings_Screen SHALL open the documents in an in-app browser or system browser [MVP]

### Requirement 36: Push Notifications

**User Story:** As a player, I want optional reminders about important game events, so that I don't miss deadlines or opportunities when I'm away from the game.

#### Acceptance Criteria

**Notification Types:**
1. THE Notification_System SHALL support Event Deadline notifications: "Your event '[Event Name]' is tomorrow! Make sure everything is ready." [MVP]
2. THE Notification_System SHALL support Task Deadline notifications: "[Task Name] is due in 2 hours" (only for critical tasks) [MVP]
3. THE Notification_System SHALL support New Inquiry notifications: "A new client is interested in your services!" (max 1 per day) [MVP]
4. THE Notification_System SHALL support Referral notifications: "You received a referral from [Client Name]!" [MVP]
5. THE Notification_System SHALL support Financial Warning notifications: "Your funds are running low. Check your Bank app." [MVP]

**Player Control:**
6. THE Settings_Screen SHALL allow players to enable/disable each notification type independently [MVP]
7. THE Notification_System SHALL default all notifications to OFF, requiring player opt-in [MVP]
8. THE Notification_System SHALL respect device Do Not Disturb settings [MVP]
9. THE Notification_System SHALL NOT send notifications between 10 PM and 8 AM local time [MVP]

**Notification Limits:**
10. THE Notification_System SHALL limit total notifications to maximum 3 per day [MVP]
11. THE Notification_System SHALL prioritize notifications by urgency: Event Deadline > Task Deadline > Financial Warning > Referral > New Inquiry [MVP]
12. THE Notification_System SHALL NOT send promotional or re-engagement notifications ("Come back and play!") [MVP]

**Technical:**
13. THE Notification_System SHALL integrate with iOS APNs and Android FCM for push delivery [MVP]
14. THE Notification_System SHALL schedule notifications locally based on in-game time calculations [MVP]
15. WHEN the player opens the app from a notification, THE Notification_System SHALL navigate to the relevant screen [MVP]

### Requirement 37: Achievements and Trophies

**User Story:** As a player, I want to earn achievements for my accomplishments, so that I feel recognized for my progress and have additional goals to pursue.

#### Acceptance Criteria

**Achievement System:**
1. THE Achievement_System SHALL track player accomplishments and award achievements when criteria are met [MVP]
2. THE Achievement_System SHALL display achievements in a dedicated "Trophies" section accessible from the Phone [MVP]
3. WHEN an achievement is earned, THE Achievement_System SHALL display a celebratory notification with the achievement name and icon [MVP]
4. THE Achievement_System SHALL categorize achievements as: Progression, Mastery, Challenge, and Secret [MVP]
5. THE Achievement_System SHALL sync achievements with platform services (Game Center, Google Play Games) when available [MVP]

**Progression Achievements (Stage-based):**
6. THE Achievement_System SHALL award "First Steps" when the player completes their first event [MVP]
7. THE Achievement_System SHALL award "Rising Star" when the player reaches Stage 2 [MVP]
8. THE Achievement_System SHALL award "Going Pro" when the player reaches Stage 3 [MVP]
9. THE Achievement_System SHALL award "Industry Leader" when the player reaches Stage 4 [Post-MVP]
10. THE Achievement_System SHALL award "Event Planning Mogul" when the player reaches Stage 5 [Post-MVP]

**Mastery Achievements (Skill-based):**
11. THE Achievement_System SHALL award "Perfect Planner" when the player achieves 100% satisfaction on any event [MVP]
12. THE Achievement_System SHALL award "Consistency is Key" when the player completes 10 events with 80%+ satisfaction [MVP]
13. THE Achievement_System SHALL award "Excellence Streak" when the player achieves 5 consecutive 90%+ satisfaction events [MVP]
14. THE Achievement_System SHALL award "Budget Master" when the player completes 10 events under budget [MVP]
15. THE Achievement_System SHALL award "Vendor Whisperer" when the player reaches relationship level 5 with 5 different vendors [MVP]
16. THE Achievement_System SHALL award "Weather Watcher" when the player successfully handles 5 weather-related challenges [MVP]

**Challenge Achievements (Difficulty-based):**
17. THE Achievement_System SHALL award "Perfectionist's Perfectionist" when the player satisfies a Perfectionist client with 95%+ [MVP]
18. THE Achievement_System SHALL award "Juggling Act" when the player successfully completes 3 concurrent events [MVP]
19. THE Achievement_System SHALL award "Crisis Manager" when the player recovers from 3 random events using contingency [MVP]
20. THE Achievement_System SHALL award "Self-Made" when the player reaches Stage 3 via Entrepreneur Path [Post-MVP]
21. THE Achievement_System SHALL award "Corporate Climber" when the player reaches Director on Corporate Path [Post-MVP]
22. THE Achievement_System SHALL award "Celebrity Handler" when the player successfully completes a Celebrity event [Post-MVP]

**Secret Achievements (Hidden until earned):**
23. THE Achievement_System SHALL award "Above and Beyond" when the player triggers hidden reputation bonuses 10 times (description hidden until earned) [MVP]
24. THE Achievement_System SHALL award "Family First" when the player never uses family emergency funds (description hidden until earned) [MVP]
25. THE Achievement_System SHALL award "Comeback Kid" when the player recovers from a Financial Crisis (description hidden until earned) [MVP]

**Rewards:**
26. WHEN certain achievements are earned, THE Achievement_System SHALL grant in-game rewards (currency, cosmetic items, or unlocks) [Post-MVP]
27. THE Achievement_System SHALL display achievement progress for trackable achievements (e.g., "3/10 events completed") [MVP]
