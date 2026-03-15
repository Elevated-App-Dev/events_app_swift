# Design Landscape Analysis: From App to Game

## Purpose

This document captures research on mobile business simulator game design, SwiftUI capabilities for game-like interfaces, and a vision for evolving the Event Planner Simulator from a functional prototype into an engaging, game-feeling experience.

---

## The Problem

The current UI uses `List`, `NavigationStack`, and standard SwiftUI components — tabs, rows, buttons, sheets. It works for testing the gameplay loop, but it looks and feels like a business app, not a game. The goal is to identify what makes simulator games feel like *games* and how to achieve that natively in SwiftUI.

---

## Competitive Landscape

### Relevant Games for Inspiration

**Coffee Inc 2** (Side Labs) — [App Store](https://apps.apple.com/us/app/coffee-inc-2/id1573482724)
- Turn-based business sim, closest comp to our event planner concept
- Clean dashboards, store customization, realistic financials (income statements, balance sheets)
- Premium model ($4.99, no ads)
- Expand into real-world cities with multiple currencies
- Key takeaway: Proves business simulation depth works on mobile with clean UI

**Pocket City** (Codebrew Games) — [Website](https://pocketcitygame.com/)
- Low-poly minimalist city builder, premium ($4.99)
- Praised for mobile-first UI that doesn't feel like a settings app
- Fluid, intuitive mobile-specific design
- Key takeaway: Minimalist aesthetic + mobile-native interaction = great feel

**Game Dev Story** (Kairosoft) — [App Store](https://apps.apple.com/us/app/game-dev-story/id1557657042)
- Pixel art sim with enormous charm, 86/100 Metacritic
- Proves personality and character design matter more than graphical fidelity
- "More than a retro-styled mobile title; it's a love letter to the game industry wrapped in clever mechanics and witty design"
- Key takeaway: Character, personality, and charm create addictiveness

**The Westport Independent**
- Newspaper sim with Papers Please-style moral choices woven into business mechanics
- Key takeaway: Narrative choices embedded in business decisions create emotional investment

**Mini Metro**
- Already referenced in our design docs as aesthetic inspiration
- Pure minimalist geometric design, deeply satisfying interactions
- Key takeaway: Constraints breed elegance; less is more

### Genre Insights
- Business simulation games market is growing, with 90% of players reporting enhanced strategic planning skills (2025 survey)
- Premium mobile sims (Pocket City, Coffee Inc) prove players pay for ad-free, quality experiences
- The most successful mobile sims combine real-world experience themes with accessible gameplay

---

## What Separates "Game" from "App"

### 1. A Sense of Place (Not Screens)

Games have a *world*, not a tab bar. Instead of tabs switching between flat lists, the player inhabits a space.

**For our event planner, this means:**
- **Your desk/office as the home screen** — a stylized illustration of your workspace where items on the desk are interactive (phone, calendar, laptop, message board)
- **A city map as primary navigation** — not a secondary overlay, but the way you discover and visit venues/vendors
- Tapping the phone on your desk opens the phone overlay. Tapping the calendar shows events. The laptop shows financials.

This is the "diegetic UI" approach — interface elements exist *within* the game world rather than as abstract navigation chrome.

### 2. Character and Personality

Our design docs define client personalities (easyGoing, perfectionist, budgetConscious) but the current UI shows them as text labels. Games make characters *visible*:

- Simple illustrated client portraits (even stylized silhouettes with distinctive features)
- Client dialogue when they present their inquiry ("I need this to be PERFECT. Budget is no object." vs "Look, I just need something simple for the kids...")
- Vendor characters with personality (the budget caterer has a folksy quote, the luxury photographer is dramatic)
- A narrator/mentor character who comments on progress, reacts to failures, celebrates wins

### 3. Visual Feedback and "Juice"

Games constantly reward you with micro-animations:
- Money counter that **counts up** when you get paid (not just changing a number)
- Reputation stars that **pop and glow** on gain, **shake** on loss
- Event completion with a **satisfaction gauge** that fills dramatically
- Phase transitions with visual flair (calendar pages flipping for time passing)
- Button press feedback (scale, haptics)
- Celebration particles on achievements

### 4. Story Thread / Narrative Wrapper

Research on [narrative in simulation games](https://medium.com/@Tracey_Watson/storytelling-in-small-spaces-practical-narrative-design-for-mobile-games-1a080d0d3732) shows the most engaging sims weave story into mechanics:

- **A narrator/mentor character** who comments on your progress ("Your first wedding! No pressure..."), reacts to failures ("Well, the DJ didn't show but... you improvised."), and celebrates wins
- **Client stories that resolve** — not just "Event completed, 85% satisfaction" but "The birthday boy loved the magician. His mom left you a 5-star review."
- **Stage transitions as story beats** — Stage 2 isn't just "you now work for a company," it's a scene: your character gets a phone call, the offer is presented, you accept
- **Emergent narrative** — "Games are the only medium capable of truly generating stories as a function of interaction" ([Game Informer](https://gameinformer.com/b/features/archive/2016/11/25/letting-players-tell-the-story-simulation-games-as-narrative-machines.aspx))

### 5. Hybrid Visual Novel + Simulation

The intersection of tycoon mechanics with visual novel storytelling creates deeply engaging hybrid experiences:
- Dating sims with time-management express the protagonist's daily pressures more vividly than dialogue alone
- Mini-games thematically aligned with narrative amplify emotional investment
- Success or failure influences character relationships or future story routes
- This cross-pollination helps titles stand out in a saturated market

---

## SwiftUI Capabilities for Game-Like Interfaces

### What SwiftUI Uniquely Enables

**Native gestures:**
- Drag-to-arrange vendors on a planning board
- Swipe through client inquiries like cards (accept right, decline left)
- Long-press for contextual info
- Pinch-to-zoom on the city map

**Widget-like dashboard:**
- Instead of a traditional game HUD, use a layout inspired by iOS widgets — cards that show live event status, upcoming deadlines, weather, finances
- Feels *native* and modern, unlike ported Unity UIs

**Fluid transitions:**
- `matchedGeometryEffect` lets you tap an event card and have it expand into the detail view seamlessly, like opening an app on the home screen
- `NavigationTransition` for custom screen transitions

**Canvas + immediate mode drawing:**
- For the city map, event timeline, and satisfaction gauges
- SwiftUI's Canvas API provides 60fps custom rendering without SpriteKit
- Can combine with Metal for GPU-accelerated particle effects
- References: [Mastering Canvas in SwiftUI](https://swiftwithmajid.com/2023/04/11/mastering-canvas-in-swiftui/), [Canvas API Performance](https://ravi6997.medium.com/swiftuis-canvas-revolution-how-apple-s-new-drawing-api-is-transforming-ios-development-in-2025-ac0c1eb838df)

**Advanced animations:**
- Phase Animators for multi-step sequences (WWDC23)
- Keyframe Animations for coordinated effects
- Spring physics for natural motion
- 3D rotation effects (`rotation3DEffect`) for card flips
- Cascading delays for sequential reveals
- Reference: [WWDC23 Advanced Animations](https://developer.apple.com/videos/play/wwdc2023/10157/)

**Game dialogue boxes:**
- Typing effect for character dialogue
- Animated "next" arrow for conversation flow
- Reference: [SwiftUI Game Dialogue Tutorial](https://medium.com/appledeveloperacademy-ufpe/creating-an-animated-game-dialogue-box-using-swiftui-7a75b54f02d7)

### Architecture Notes
- Full games have been shipped using SwiftUI for the entire UI layer
- CADisplayLink can tie display refresh rate to update functions (30/60/120fps)
- All heavy logic should live in the ViewModel/@Observable object, keeping Views purely declarative
- SwiftUI forces cleaner architecture than UIKit — no massive view controllers

---

## A Concrete Vision for the Event Planner

### Home = Your Office Desk (illustrated, interactive)
- Phone buzzes on desk -> tap it -> client inquiry slides up as a message thread
- Calendar on wall -> tap -> shows your upcoming events with weather forecast
- Laptop screen -> financials dashboard
- Map pinned to wall -> tap -> zooms into city map
- Mail slot -> new referrals, messages
- The desk evolves as you progress (Stage 1: home desk, Stage 2: company office, Stage 3+: corner office)

### Accepting a Client = Conversation, Not a Form
- Client portrait slides in, dialogue box types out their request
- You see their personality, budget, and event details through the conversation
- "Accept" / "Pass" as dialogue choices, not buttons on a list row
- Personality affects tone of dialogue (perfectionist is demanding, easyGoing is casual)

### Planning an Event = A Planning Board, Not a Settings Screen
- Visual board with slots: Venue, Caterer, Entertainment, Decor, etc.
- Drag vendors onto slots from a tray, or tap to browse
- Budget bar fills visually as you assign items
- Weather warning stamp appears on outdoor venue slot
- Completion percentage ring shows planning progress

### Event Day = A Timeline That Plays Out
- Animated sequence of the event progressing through key moments
- Random events pop up as crisis cards you must resolve
- Satisfaction meter builds in real-time with each phase
- Contingency budget drains visually when you spend on mitigation

### Results = A Story Moment
- Client reaction dialogue ("This was everything we dreamed of!")
- Photo-style recap (stylized event snapshot)
- Stats appear with dramatic reveals (profit counts up, reputation pops, referral notification)
- Review quote from the client

### Stage Transitions = Narrative Beats
- Stage 1 -> 2: Phone call scene, company offers you a job
- Stage 2 -> 3: Performance review, path choice ceremony
- Each transition is a memorable story moment, not a settings change

---

## Recommended Design Spike

Before implementing all 15 feature areas from the conversion reference, build **one screen** that embodies the "game not app" philosophy as a proof of concept.

### Best candidates:

**Option A: Office Desk Home Screen**
- Replaces the tab bar with an interactive illustrated workspace
- Proves out Canvas/ZStack composition, tap targets, diegetic UI
- Immediately changes how the entire game feels
- Can be simple at first (colored shapes for desk items) and refined with art later

**Option B: Conversation-Style Client Inquiry**
- Replaces the flat InquiryListView with a dialogue-based experience
- Proves out typing animations, character display, dialogue choices
- Makes accepting clients feel like a story moment
- Directly addresses the "app vs game" gap

**Option C: Animated Event Results**
- Replaces the static results view with dramatic reveals
- Proves out count-up animations, phase animators, celebration effects
- The most emotionally impactful moment in the game loop
- Contained scope (one view, no navigation changes)

### Recommendation
Start with **Option A (Office Desk)** because it sets the foundation for the entire game's navigation paradigm. Everything else (phone, map, events, results) would be accessed *through* the desk, making it the architectural anchor.

---

## Sources

- [Coffee Inc 2 - App Store](https://apps.apple.com/us/app/coffee-inc-2/id1573482724)
- [Coffee Inc Developer Site](https://www.sidelabs.com/coffeeinc/)
- [Pocket City 2](https://pocketcitygame.com/)
- [Game Dev Story - App Store](https://apps.apple.com/us/app/game-dev-story/id1557657042)
- [Game UI Database](https://gameuidatabase.com/)
- [WWDC23: Advanced Animations in SwiftUI](https://developer.apple.com/videos/play/wwdc2023/10157/)
- [Animated Game Dialogue Box in SwiftUI](https://medium.com/appledeveloperacademy-ufpe/creating-an-animated-game-dialogue-box-using-swiftui-7a75b54f02d7)
- [Mastering Canvas in SwiftUI](https://swiftwithmajid.com/2023/04/11/mastering-canvas-in-swiftui/)
- [SwiftUI Canvas API - High Performance Graphics](https://ravi6997.medium.com/swiftuis-canvas-revolution-how-apple-s-new-drawing-api-is-transforming-ios-development-in-2025-ac0c1eb838df)
- [Narrative Design for Mobile Games](https://medium.com/@Tracey_Watson/storytelling-in-small-spaces-practical-narrative-design-for-mobile-games-1a080d0d3732)
- [Simulation Games as Narrative Machines - Game Informer](https://gameinformer.com/b/features/archive/2016/11/25/letting-players-tell-the-story-simulation-games-as-narrative-machines.aspx)
- [Open SwiftUI Animations Collection](https://github.com/amosgyamfi/open-swiftui-animations)
- [SwiftUI Games Collection](https://github.com/berikv/SwiftUIGames)
- [Top Business Simulation Games 2026](https://filmora.wondershare.com/game-recording/top-best-business-simulation-games.html)
- [Simulation Mobile Games Market Overview](https://maf.ad/en/blog/simulation-mobile-games-market/)
