# Event Planning Simulator
## Game Design Document

**A strategic planning simulation game where players build an event planning career from solo side-hustle, to employee at an established firm, to running their own premier agency.**

- **Genre:** Business Simulation / Tycoon / Strategy
- **Platform:** Mobile (iOS/Android)
- **Engine:** Godot
- **Target Audience:** Casual to mid-core gamers who enjoy management sims

---

## Table of Contents

1. [Game Overview](#1-game-overview)
2. [Core Gameplay Loop](#2-core-gameplay-loop)
3. [Visual Navigation System](#3-visual-navigation-system)
4. [Planning Phase Mechanics](#4-planning-phase-mechanics)
5. [Decision Consequence System](#5-decision-consequence-system)
6. [Progression System](#6-progression-system)
7. [Event Types & Unlocks](#7-event-types--unlocks)
8. [Data Structures](#8-data-structures)
9. [MVP Scope](#9-mvp-scope-version-10)
10. [Future Features (v2.0)](#10-future-features-v20)

---

## 1. Game Overview

Event Planning Simulator puts players in the role of an aspiring event planner who starts by organizing small family and school events, then grows their reputation to open a full event planning company. The game emphasizes strategic decision-making where every choice has meaningful consequences on event outcomes, client satisfaction, and business growth.

### Core Pillars

- **Meaningful Decisions:** Every choice affects outcomes - no optimal path, only trade-offs
- **Business Growth:** Progress from solo planner to company owner with staff and resources
- **Visual Exploration:** Navigate a map/phone interface to make decisions, not just menus
- **Replayability:** Different decisions lead to different outcomes and strategies

### Unique Selling Points

- Deeper business simulation than existing wedding/party dress-up games
- Consequence system creates emergent storytelling
- Map-based navigation makes planning feel tangible and immersive

---

## 2. Core Gameplay Loop

The game follows a repeating loop that becomes more complex as players progress:

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

Instead of static menus, players navigate visually through two main interfaces: the City Map and the Planner's Phone. This creates an immersive experience where decisions feel tangible.

### 3.1 City Map Interface

A stylized 2D map of the city showing locations players can visit. Locations unlock as the player progresses.

| Location Type | Examples | Purpose |
|---------------|----------|---------|
| Venues | Community Center, Park Pavilion, Hotel Ballroom, Estate | Visit to preview, check availability, book |
| Vendor Shops | Bakery Row, Catering District, Party Supply Store | Browse and hire vendors |
| Services | DJ Studio, Photo Booth Rental, Florist | Book entertainment and extras |
| Your Office | Home Office → Small Office → Agency HQ | Manage business, view stats, hire staff |
| Client Locations | Client's home, Coffee shop | Meet clients, pitch services |
| Conference Room | Rented meeting space, Hotel business center | Formal client meetings, progress updates |
| Virtual Meeting | Video call from any location | Remote client updates, quick check-ins |

#### Map Interaction Flow

1. Player taps a location category (e.g., 'Venues')
2. Available venues in that category appear as pins on map
3. Tapping a pin shows a preview card with key stats
4. Player can 'Visit' to see full details, photos, reviews
5. From the visit screen, player can book, place a hold for a future event, or hire

### 3.2 Planner's Phone Interface

A smartphone UI overlay for managing tasks, communications, and quick actions without leaving current location.

| App | Function |
|-----|----------|
| Calendar | View upcoming events, deadlines, prep tasks |
| Messages | Client communications, vendor confirmations |
| Contacts | Saved vendors with ratings and notes |
| Bank | View finances, pay vendors, receive payments |
| Reviews | See your reputation, client testimonials |
| Tasks | Checklist for current event prep |
| Camera | Take notes/photos when visiting venues (flavor) |

---

## 4. Planning Phase Mechanics

### 4.1 Client Intake

New clients appear as notifications or meetings. Each client request includes:

- Event type and theme preferences
- Budget range (fixed or negotiable)
- Guest count
- Date and timeline
- Client personality type (affects satisfaction thresholds)
- Special requests or constraints

#### Client Personality Types

| Type | Behavior | Strategy |
|------|----------|----------|
| Easy-Going | Low expectations, satisfied easily | Good for learning, lower pay |
| Budget-Conscious | Wants maximum value, scrutinizes costs | Must justify every expense |
| Perfectionist | High standards, notices every detail | Requires premium choices |
| Indecisive | Changes mind, unclear requirements | Build in flexibility |
| Demanding | Difficult but pays well | High risk, high reward |

### 4.2 Budget Allocation

Players must allocate the client's budget across categories. Each category has minimum thresholds that affect outcomes.

| Category | % Range | Impact of Underspending |
|----------|---------|-------------------------|
| Venue | 25-40% | Capacity issues, ambiance complaints |
| Catering | 25-35% | Food quality/quantity complaints |
| Entertainment | 10-20% | Bored guests, low energy |
| Decorations | 10-20% | Underwhelming atmosphere |
| Staffing | 5-15% | Service problems, chaos |
| Contingency | 5-10% | No buffer for emergencies |

### 4.3 Vendor Selection (Map-Based)

Players visit vendor locations on the map to browse options. Each vendor has visible and hidden attributes:

- **Visible:** Price, quality rating, style/specialty, availability
- **Hidden (revealed over time):** Reliability, flexibility, hidden fees
- **Relationship:** Repeat business unlocks discounts and priority booking

---

## 5. Decision Consequence System

The heart of the game. Every decision feeds into outcome calculations. This creates emergent stories and meaningful choices.

### 5.1 Consequence Matrix

| Decision | Possible Outcomes | Probability Factors |
|----------|-------------------|-------------------|
| Hire cheap caterer | Fine / Food runs out / Quality complaints | Caterer reliability stat, guest count |
| Skip backup entertainment | OK if main shows / Disaster if no-show | Entertainment reliability stat |
| No contingency budget | Fine if no problems / Can't handle crises | Random event chance |
| Outdoor venue, no tent | Beautiful day / Rain ruins event | Season, weather RNG |
| Overbook schedule | Complete all / Quality suffers / Miss deadline | Staff count, event complexity |
| Ignore client preferences | They don't notice / Major dissatisfaction | Client personality type |

### 5.2 Satisfaction Calculation

Final satisfaction score combines multiple factors:

| Component | Weight | Calculation |
|-----------|--------|-------------|
| Venue Match | 20% | Venue capacity/style vs event needs |
| Food Quality | 25% | Caterer quality × budget adequacy |
| Entertainment | 20% | Entertainment quality × guest preferences |
| Decorations | 15% | Decoration quality × client expectations |
| Service | 10% | Staff performance, problem resolution |
| Expectations | 10% | Did reality match client vision? |

### 5.3 Random Events

Events that can occur during execution, modified by player preparation:

- **Vendor no-show** (mitigated by: backup plans, reliable vendors)
- **Weather problems** (mitigated by: indoor backup, tent rental)
- **Equipment failure** (mitigated by: contingency budget, quality rentals)
- **Guest conflicts** (mitigated by: good seating plan, experienced staff)
- **Client last-minute changes** (mitigated by: contingency budget, flexible vendors)

---

## 6. Progression System

Players progress through distinct business stages, each unlocking new mechanics and content.

### Stage 1: Solo Side Hustle

- Work from home, 1 event at a time
- **Event types:** Kids' birthdays, small family gatherings, school events
- Limited vendor access (local, budget-friendly only)
- **Goal:** Reach reputation level 10 and save $2,000
- **Unlock:** Get hired by an established event planning company

### Stage 2: Company Employee

- Work for "Premier Events Co." as a junior planner
- Learn from experienced mentors, access company resources
- Handle 1-2 events with company support and oversight
- **Event types:** Add engagement parties, corporate meetings, milestone birthdays
- Access to company's vendor network (better rates, reliable contacts)
- Receive salary + commission instead of keeping full profit
- **New mechanics:** Performance reviews, learning from senior planners, company reputation affects your events
- **Goal:** Reach employee level 5, save $10,000, and build personal reputation to 20
- **Unlock:** Option to leave and start your own company (or stay for stability)

### Stage 3: Small Company (Your Own Business)

- Small office (provides planning bonuses)
- Hire first employee (assistant)
- Handle 2 events simultaneously
- **Event types:** Add baby showers, anniversary parties, small corporate
- Vendor relationships begin (loyalty discounts)
- Keep contacts from your employee days (some vendors give loyalty rates)
- **Goal:** Reach reputation level 30 and save $30,000

### Stage 4: Established Agency

- Larger office with specialized rooms
- Multiple staff (coordinator, designer, vendor manager)
- Handle 3-4 events simultaneously
- **Event types:** Add weddings, galas, conferences
- Premium vendors unlock
- Staff assignment becomes strategic

### Stage 5: Premier Event Company

- Flagship office/showroom
- Large team with department heads
- Celebrity and high-profile clients
- **Event types:** Add festivals, product launches, destination events
- Exclusive venue and vendor partnerships
- Delegate most work, focus on key decisions

---

## 7. Event Types & Unlocks

| Event Type | Unlock Stage | Complexity | Typical Budget |
|------------|--------------|------------|----------------|
| Kids' Birthday | Stage 1 | Low | $500-2,000 |
| Family Gathering | Stage 1 | Low | $300-1,500 |
| School Event | Stage 1 | Low-Med | $1,000-3,000 |
| Adult Birthday | Stage 1+ | Medium | $1,000-5,000 |
| Engagement Party | Stage 2 | Medium | $2,000-8,000 |
| Corporate Meeting | Stage 2 | Medium | $3,000-15,000 |
| Milestone Birthday | Stage 2 | Medium | $2,000-6,000 |
| Baby Shower | Stage 3 | Medium | $1,000-4,000 |
| Anniversary Party | Stage 3 | Medium | $2,000-10,000 |
| Small Corporate Event | Stage 3 | Medium | $5,000-20,000 |
| Small Wedding | Stage 4 | High | $10,000-30,000 |
| Charity Gala | Stage 4 | High | $15,000-50,000 |
| Conference | Stage 4 | High | $20,000-100,000 |
| Large Wedding | Stage 5 | Very High | $30,000-150,000 |
| Product Launch | Stage 5 | Very High | $50,000-200,000 |
| Festival | Stage 5 | Very High | $100,000+ |

---

## 8. Data Structures

Core data structures for implementing the game mechanics. These define how game objects store and interact with data.

### 8.1 Client Data

```javascript
Client {
  id: string
  name: string
  personality_type: enum [EASY_GOING, BUDGET_CONSCIOUS, PERFECTIONIST, INDECISIVE, DEMANDING]
  satisfaction_threshold: float (0.0-1.0) // Minimum score to be satisfied
  budget_flexibility: float // How much they'll negotiate
  communication_style: enum [RESPONSIVE, SLOW, MICROMANAGER]
  relationship_level: int // Increases with successful events
  referral_chance: float // Chance to refer new clients
}
```

### 8.2 Event Data

```javascript
Event {
  id: string
  client_id: string
  event_type: enum [KIDS_BIRTHDAY, FAMILY_GATHERING, WEDDING, ...]
  status: enum [INQUIRY, ACCEPTED, PLANNING, EXECUTING, COMPLETED, CANCELLED]
  date: datetime
  guest_count: int
  budget: {
    total: float
    allocated: { venue: float, catering: float, entertainment: float, ... }
    spent: float
  }
  venue_id: string
  vendors: [Vendor_Assignment]
  tasks: [Task]
  requirements: { indoor: bool, outdoor_backup: bool, dietary: [...], ... }
  satisfaction_scores: { venue: float, food: float, entertainment: float, ... }
  final_score: float
  profit: float
}
```

### 8.3 Vendor Data

```javascript
Vendor {
  id: string
  name: string
  category: enum [CATERING, ENTERTAINMENT, DECORATIONS, PHOTOGRAPHY, ...]
  tier: enum [BUDGET, STANDARD, PREMIUM, LUXURY]
  base_price: float
  quality_rating: float (1-5) // Visible to player
  reliability: float (0-1) // Hidden, revealed over time
  flexibility: float (0-1) // Hidden, handles last-minute changes
  hidden_fees_chance: float // Hidden
  specialty: string // e.g., 'Kid-friendly', 'Upscale', 'Rustic'
  availability: [date] // Booked dates
  relationship_level: int // With player, affects discounts
  discount_rate: float // Based on relationship
}
```

### 8.4 Venue Data

```javascript
Venue {
  id: string
  name: string
  type: enum [BACKYARD, COMMUNITY_CENTER, RESTAURANT, HOTEL, ESTATE, ...]
  tier: enum [BUDGET, STANDARD, PREMIUM, LUXURY]
  capacity: { min: int, max: int, comfortable: int }
  base_price: float
  amenities: [string] // 'kitchen', 'AV_equipment', 'parking', ...
  indoor: bool
  outdoor_space: bool
  weather_dependent: bool
  ambiance_rating: float (1-5)
  accessibility: float (1-5)
  restrictions: [string] // 'no_alcohol', 'noise_curfew', ...
  availability: [date]
  location: { x: float, y: float } // Map position
}
```

### 8.5 Player/Business Data

```javascript
Player {
  name: string
  business_name: string
  stage: enum [SOLO, EMPLOYEE, SMALL_COMPANY, ESTABLISHED, PREMIER]
  reputation: int (0-100)
  money: float
  office: Office
  staff: [Staff]
  active_events: [Event]
  completed_events: [Event]
  vendor_relationships: { vendor_id: relationship_level }
  unlocked_event_types: [enum]
  unlocked_venues: [venue_id]
  unlocked_vendors: [vendor_id]
  employer: Employer // Only used in EMPLOYEE stage
  stats: {
    total_events: int
    successful_events: int
    total_revenue: float
    average_satisfaction: float
  }
}

// New structure for Employee stage
Employer {
  company_name: string
  reputation: int // Affects event difficulty and client expectations
  vendor_network: [vendor_id] // Pre-approved vendors with discounts
  salary: float // Base pay per event
  commission_rate: float // Percentage of profit as bonus
  employee_level: int (1-5) // Player's rank within company
  mentor: Staff // Senior planner who provides tips/bonuses
}
```

### 8.6 Staff Data

```javascript
Staff {
  id: string
  name: string
  role: enum [ASSISTANT, COORDINATOR, DESIGNER, VENDOR_MANAGER, ...]
  salary: float // Per event or per month
  skill_level: float (1-5)
  specialties: [event_type] // Event types they're best at
  efficiency: float // Reduces task time
  reliability: float // Chance of errors
  assigned_event: event_id
}
```

### 8.7 Task Data

```javascript
Task {
  id: string
  event_id: string
  name: string
  category: enum [VENUE, CATERING, ENTERTAINMENT, LOGISTICS, ...]
  status: enum [PENDING, IN_PROGRESS, COMPLETED, FAILED]
  deadline: datetime
  time_required: int // In-game hours
  assigned_to: staff_id or 'player'
  dependencies: [task_id] // Tasks that must complete first
  failure_consequence: string // What happens if missed
}
```

### 8.8 Outcome Calculation Structure

```javascript
OutcomeCalculator {
  calculate_satisfaction(event: Event) -> float {
    venue_score = calc_venue_score(event.venue, event.requirements)
    food_score = calc_food_score(event.vendors['catering'], event.budget.catering)
    entertainment_score = calc_entertainment_score(...)
    decoration_score = calc_decoration_score(...)
    service_score = calc_service_score(event.staff, event.random_events)
    
    // Apply weights based on event type and client
    weights = get_weights(event.event_type, event.client.personality)
    
    // Apply random event modifiers
    modifiers = calc_random_event_impact(event.random_events, event.contingency)
    
    final = weighted_average(scores, weights) * modifiers
    return clamp(final, 0.0, 1.0)
  }
}
```

---

## 9. MVP Scope (Version 1.0)

Minimum features for a testable, launchable first version:

### Include in MVP

- Stage 1 and Stage 2 progression (Solo Planner → Company Employee)
- 6 event types: Kids' birthday, family gathering, school event, adult birthday, engagement party, corporate meeting
- 5 planning categories: Venue, catering, entertainment, decorations, extras
- 3 vendor options per category (budget, standard, premium)
- 5 venues to choose from
- Simplified city map (fewer locations)
- Basic phone interface (calendar, messages, bank)
- Core consequence system (satisfaction calculations)
- 3 client personality types
- Results screen with detailed breakdown
- Employee mechanics: salary, commission, performance reviews
- Basic monetization (rewarded ads for bonuses, IAP for currency)

### Defer to Later Versions

- Diner Dash execution mini-game (v2.0)
- Stages 3-5 progression (own company through premier agency)
- Staff hiring and management
- Multiple simultaneous events
- Complex vendor relationship system
- Seasonal events and limited-time content
- Battle pass / subscription features

---

## 10. Future Features (v2.0+)

### 10.1 Diner Dash Execution Mode

Real-time mini-game during event execution. Quality of planning determines difficulty:

- **Good planning** = smooth event, minor issues to handle
- **Poor planning** = chaos, multiple crises at once
- Player taps to resolve issues, keep guests happy, manage staff
- Optional: Can auto-complete with reduced rewards

### 10.2 Advanced Business Features

- Office customization and upgrades
- Marketing and advertising to attract clients
- Competitor events (other planners in town)
- Specialization paths (wedding specialist, corporate expert)
- Franchise system (open offices in new cities)

### 10.3 Social Features

- Share event photos/results
- Friend leaderboards
- Cooperative events (plan together)

### 10.4 Live Events

- Seasonal themes (Halloween parties, holiday galas)
- Limited-time event types
- Special client campaigns