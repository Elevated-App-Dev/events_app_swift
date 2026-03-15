# Session: March 15, 2026 — Decision-Point System & UI Planning

## What Was Done

### Design Discussion
- Analyzed the repo structure, architecture, and current state of the game
- Reviewed competitive landscape docs — Coffee Inc 2 identified as closest competitor
- Reviewed planned navigation UI (diegetic desk, phone overlay, 9-layer z-order system)
- Debated time progression mechanics: real-time vs turn-based vs decision-point advancement
- Decided on **turn-based, calendar-anchored decision-point system** inspired by Coffee Inc 2 but adapted for event planning's multi-vendor process complexity

### Key Design Decisions

**Time System**
- Replaced real-time timer (seconds → game days) with player-initiated advancement
- Player taps "Advance" to jump to the next calendar date with pending activities
- No dead air — every advance lands on a meaningful decision

**Vendor Process**
- Each vendor has a multi-step process: availability → options → negotiate → contract → deposit → final confirm
- Each step has a date offset, creating scheduled future activities on the calendar
- Offer/counteroffer negotiation (max 2 rounds, each costs +1 calendar day)
- Overdue tasks escalate through vendor messages, degrade vendor relationships behind the scenes

**Client Meetings**
- Client meeting scheduled 1-3 days after accepting an inquiry
- Dialogue transcripts generated per personality type — client drops hints about budget, priorities, and requirements (no explicit labels shown to player)
- The transcript becomes the player's "notepad" — they reference it when making vendor decisions later
- Future: interactive call flow where player chooses responses to client prompts

**Communication**
- Medium rotation: email (formal), text (quick), call (dialogue transcript), in-person (Stage 2+)
- All interactions surface through an inbox/messages system
- Real game dates shown (March 8), never abstract day numbers

**Contracts & Payment**
- Client contract signing + deposit collection (25%) before vendor planning begins
- Dual banking (client trust vs operating money) deferred to later

### Code Changes (branch: `feature/decision-point-system`)

**New files created:**
- `Models/Enums/PlanningEnums.swift` — ActivityType, ActivityStatus, CommunicationMedium, NegotiationOutcome
- `Models/Data/PlanningActivity.swift` — PlanningActivity, ActivityContent, DialogueLine, MenuOption, VendorProcessTemplate, DecisionPoint
- `Systems/Protocols/AdvanceSystemProtocol.swift` — protocol for turn-based engine
- `Systems/Implementations/AdvanceSystem.swift` — core advance engine (find next decision point, schedule activities, process overdue)
- `Views/Gameplay/InboxView.swift` — inbox with action buttons, medium icons, type badges, dialogue transcript display

**Files modified:**
- `App/GameManager.swift` — removed timer loop, added advanceToNextPoint(), initiateVendorContact(), sendNegotiationOffer(), client meeting dialogue generation, activity follow-up chains (meeting → contract → signed → deposit)
- `Systems/Implementations/TimeSystem.swift` — stripped to just event date scheduling
- `Systems/Protocols/TimeSystemProtocol.swift` — simplified to scheduleEvent() only
- `Systems/Protocols/GameContext.swift` — references AdvanceSystem instead of TimeSystem
- `Models/Data/ClientInquiry.swift` — removed real-time expiration, uses game date
- `Models/Data/VendorRelationship.swift` — expanded with relationship tiers, score tracking, negotiation/overdue impacts
- `Systems/Implementations/EventPlanningSystem.swift` — passes arrivedDate to inquiry factory
- `Systems/Implementations/EventPhaseCalculator.swift` — fixed pre-existing `goimport` typo
- `Systems/Implementations/TutorialSystem.swift` — updated instructions for turn-based flow
- `Views/GameplayView.swift` — added Advance button, Inbox tab, redesigned bottom bar
- `Views/Gameplay/ActiveEventsListView.swift` — uses gameManager.currentDate

### Current Playable Flow
1. New Game → tutorial starts, first inquiry appears
2. Accept inquiry → client meeting scheduled
3. Advance → meeting date → read personality-driven dialogue transcript → "End Call"
4. Contract sent → Advance → client signs → deposit received
5. Event moves to "planning" status
6. Advance to event day → execution → results

### Known Limitations
- Vendor multi-step process exists in backend but no UI calls it yet (old instant-book pickers still work)
- Client meeting dialogue is generated but not interactive (future: player chooses responses)
- No way to initiate vendor contact from the UI
- UI is still barebones — no visual design applied

---

## Next Steps: Phased UI & Feature Plan

### Phase 1: UI Foundation
1. **Dark theme + typography** — apply the visual style from design docs (dark background `#1A1A2E`, color palette, bold font hierarchy, max whitespace)
2. **Phone overlay shell** — slide-up phone interface with 7 app icons (Messages, Calendar, Bank, Contacts, Reviews, Tasks, Clients)
3. **HUD redesign** — match visual style, integrate Advance button properly into the flow

### Phase 2: Core Phone Apps
4. **Messages app** — replace current InboxView with threaded conversation UI. Client calls = conversation threads. Vendor emails = threads. All planning activity lives here
5. **Calendar app** — visual calendar showing scheduled activities, event dates, deadlines. The player's primary planning tool

### Phase 3: Vendor Process (built on the real UI)
6. **Wire vendor contact into Messages** — "Contact Vendor" sends a message in a thread, response arrives as reply, negotiation happens as back-and-forth conversation
7. **Bank app** — deposits in, vendor payments out, running balance

### Phase 4: Polish
8. **Desk/home screen** — diegetic navigation (phone on desk, calendar on wall, laptop for financials, map on wall)
9. **"Juice"** — animations, haptics, celebration particles, money counter animations, reputation star effects

### Rationale
The phone's Messages app IS the inbox. The Calendar IS the planning view. Building the UI shell first means the vendor negotiation flow (Phase 3) gets built directly into the final UI rather than being built twice. The diegetic desk comes last because the phone overlay works standalone.
