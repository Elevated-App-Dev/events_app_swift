# Wireframes v2 - Minimalist Aesthetic

Based on Mini Metro minimalism + Coffee Inc/Pocket City map style.

**Design Principles Applied:**
- Maximum whitespace
- Only essential information
- No decorative elements
- Bold typography for hierarchy
- Color used sparingly and meaningfully

---

## 1. Main Menu

```
┌────────────────────────────────────────┐
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│           EVENT PLANNER                │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│           New Game                     │
│                                        │
│           Continue                     │
│                                        │
│           Settings                     │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                              v1.0.0    │
│                                        │
└────────────────────────────────────────┘

• Title: 48px, Bold, White
• Menu items: 20px, Regular, #B0B0B0
• Menu items highlight #FFFFFF on hover/focus
• No borders, no boxes, just text
• Massive breathing room
```

---

## 2. HUD

```
┌────────────────────────────────────────┐
│                                        │
│   Mar 15              47 ★      $2,450 │
│                                        │
├────────────────────────────────────────┤
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│          [ Content Area ]              │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
├────────────────────────────────────────┤
│                                        │
│      ○            ○            ○       │
│    Phone         Map        Settings   │
│                                        │
└────────────────────────────────────────┘

• Top bar: 16px text, single line
• Date: white | Rep: gold #FFE66D | Money: teal #4ECDC4
• Bottom: 24px icons, 12px labels below
• Icons: simple line art (circle placeholders shown)
• Background: pure black #0D0D0D
```

---

## 3. Phone - Home Screen

```
┌────────────────────────────────────────┐
│                                        │
│                                   ✕    │
│                                        │
│                                        │
│                                        │
│       ○           ○           ○        │
│    Calendar    Messages     Bank       │
│                   3                    │
│                                        │
│                                        │
│       ○           ○           ○        │
│    Contacts    Reviews      Tasks      │
│                               5        │
│                                        │
│                                        │
│       ○                                │
│    Clients                             │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
└────────────────────────────────────────┘

• Background: #0D0D0D
• Icons: 32px, line style, #B0B0B0
• Labels: 14px, #606060
• Badge: small red circle with number, positioned top-right of icon
• Close (✕): top-right, 44px touch target
• Grid layout with generous 48px gaps
• No frames, no cards, just icons floating
```

---

## 4. Phone - Calendar

```
┌────────────────────────────────────────┐
│                                        │
│   ←  Calendar                     ✕    │
│                                        │
│                                        │
│            ◄  March  ►                 │
│                                        │
│   Su   Mo   Tu   We   Th   Fr   Sa    │
│                                        │
│                   1    2    3    4     │
│                                        │
│    5    6    7    8    9   10   11     │
│                                        │
│   12   13   14  [15]  16   17   18     │
│                                        │
│   19   20   21   22   23   24   25     │
│                        ●               │
│   26   27   28   29   30   31          │
│             ●                          │
│                                        │
│   ─────────────────────────────────    │
│                                        │
│   19  Emma's Princess Birthday         │
│       20 guests · $1,200               │
│                                        │
│   27  Smith Family BBQ                 │
│       40 guests · $800                 │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
└────────────────────────────────────────┘

• [15] = Today, white circle outline
• ● = Days with events, small 6px dot below number
• Month navigation: simple arrows
• Event list: minimal, just date + name + key info
• No cards, just rows with subtle separator line
• Numbers: monospace for alignment
```

---

## 5. Phone - Bank

```
┌────────────────────────────────────────┐
│                                        │
│   ←  Bank                         ✕    │
│                                        │
│                                        │
│                                        │
│                                        │
│               $2,450                   │
│                                        │
│                                        │
│                                        │
│      +$1,200            -$350          │
│      pending in         pending out    │
│                                        │
│   ─────────────────────────────────    │
│                                        │
│   Today                                │
│                                        │
│   +  Event Payment           +$800     │
│      Johnson Wedding                   │
│                                        │
│   −  Vendor Payment          -$200     │
│      DJ Mike                           │
│                                        │
│   Yesterday                            │
│                                        │
│   +  Side Gig                +$150     │
│      Birthday Party                    │
│                                        │
│   −  Venue Deposit           -$300     │
│      Community Center                  │
│                                        │
└────────────────────────────────────────┘

• Balance: 48px, Bold, Teal #4ECDC4
• Pending: 16px, secondary color
• Transactions: grouped by day
• + in green #7BC950, - in red #E84855
• Amount right-aligned, monospace
• Description in #606060
```

---

## 6. Phone - Tasks

```
┌────────────────────────────────────────┐
│                                        │
│   ←  Tasks                        ✕    │
│                                        │
│                                        │
│   Emma's Princess Birthday             │
│   Mar 19 · 3 of 5                      │
│   ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━───    │
│                                        │
│                                        │
│   ✓  Confirm venue                     │
│                                        │
│   ✓  Book caterer                      │
│                                        │
│   ✓  Order decorations                 │
│                                        │
│   ○  Confirm entertainment             │
│      Due in 1 day · 2h                 │
│                                        │
│   ○  Final walkthrough                 │
│      Due in 3 days · 1h                │
│                                        │
│                                        │
│   ─────────────────────────────────    │
│                                        │
│   Smith Family BBQ                     │
│   Mar 27 · 0 of 4                      │
│   ━━━━━━━───────────────────────────   │
│                                        │
│   ...                                  │
│                                        │
└────────────────────────────────────────┘

• Event name: 20px semibold
• Progress bar: 4px, minimal
• ✓ = completed (#7BC950)
• ○ = pending (outline only)
• Due warning in #F7931A if urgent
• Work hours shown as "2h" - minimal
```

---

## 7. Map

```
┌────────────────────────────────────────┐
│                                        │
│                                   ✕    │
│                                        │
│   ╭─────────────────────────────────╮  │
│   │                                 │  │
│   │         WATERFRONT              │  │
│   │            🔒                   │  │
│   │                                 │  │
│   ├─────────────────────────────────┤  │
│   │                                 │  │
│   │          UPTOWN                 │  │
│   │            🔒                   │  │
│   │                                 │  │
│   ├─────────────────────────────────┤  │
│   │                                 │  │
│   │         DOWNTOWN                │  │
│   │            🔒                   │  │
│   │                                 │  │
│   ├─────────────────────────────────┤  │
│   │       ●            ●            │  │
│   │              ●                  │  │
│   │    ●    ◆         ●      ●     │  │
│   │              ●                  │  │
│   │       NEIGHBORHOOD              │  │
│   ╰─────────────────────────────────╯  │
│                                        │
│      All      Venues     Vendors       │
│       ●                                │
│                                        │
└────────────────────────────────────────┘

• Zones: muted colors, flat, rounded corners
• Locked zones: darker, lock icon centered
• ● = Location pins (small circles, 12px)
• ◆ = Player's office (diamond shape)
• Zone names: 14px, bottom-left of zone
• Filters: simple text toggle, dot under active
• No roads/details in locked zones
```

---

## 8. Map - Preview Card

```
┌────────────────────────────────────────┐
│                                        │
│   (Map visible, slightly dimmed)       │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│   ╭─────────────────────────────────╮  │
│   │                                 │  │
│   │   Riverside Community Center    │  │
│   │   Venue · Neighborhood          │  │
│   │                                 │  │
│   │   20-100 guests                 │  │
│   │   $500 + $8/guest               │  │
│   │   ★ 3.5 ambiance                │  │
│   │   Indoor                        │  │
│   │                                 │  │
│   │              Visit →            │  │
│   │                                 │  │
│   ╰─────────────────────────────────╯  │
│                                        │
│                                        │
└────────────────────────────────────────┘

• Card: #1A1A1A, slides up from bottom
• Title: 20px semibold
• Subtitle: 14px #606060
• Details: 16px, one line each
• Visit: text button with arrow, #667EEA
• Tap outside card to dismiss
```

---

## 9. Client Inquiry

```
┌────────────────────────────────────────┐
│                                        │
│   Mar 15              47 ★      $2,450 │
│                                        │
│                                        │
│                                        │
│                                        │
│   New Inquiry                          │
│   Expires in 18:32                     │
│                                        │
│                                        │
│   Sarah Johnson                        │
│   Easy-Going                           │
│                                        │
│                                        │
│   Princess Theme Birthday              │
│   Kids' Birthday                       │
│                                        │
│   Apr 5                                │
│   20 guests                            │
│   $1,200                               │
│                                        │
│                                        │
│   Requests                             │
│   Loves pink & purple                  │
│   Nut-free catering                    │
│                                        │
│                                        │
│        Decline              Accept     │
│                                        │
│                                        │
│                                        │
└────────────────────────────────────────┘

• "New Inquiry" header: 14px, #606060
• Expiration: 14px, #F7931A (warning color)
• Client name: 24px semibold
• Personality: 14px tag/chip
• Event details: clear hierarchy
• Requests: simple bullet list
• Buttons: Decline (secondary), Accept (primary)
• Maximum whitespace between sections
```

---

## 10. Budget Allocation

```
┌────────────────────────────────────────┐
│                                        │
│   ←  Budget                            │
│                                        │
│                                        │
│   Emma's Princess Birthday             │
│   $1,200 total                         │
│                                        │
│                                        │
│                                        │
│   Venue                         $360   │
│   ●━━━━━━━━━━━━━━━━━━━━○────────────   │
│                                        │
│   Catering                      $300   │
│   ●━━━━━━━━━━━━━━━━○────────────────   │
│                                        │
│   Entertainment                 $180   │
│   ●━━━━━━━━━━━○──────────────────────  │
│                                        │
│   Decorations                   $180   │
│   ●━━━━━━━━━━━○──────────────────────  │
│                                        │
│   Staffing                       $60   │
│   ●━━━○───────────────────────────────  │
│   Below recommended                    │
│                                        │
│   Contingency                   $120   │
│   ●━━━━━━━━○────────────────────────   │
│                                        │
│                                        │
│              Confirm                   │
│                                        │
└────────────────────────────────────────┘

• Category name left, amount right (monospace)
• Slider: thin 2px line, white thumb
• Warning text: 12px, #F7931A, appears below slider
• No recommended percentages cluttering - just warn if too low
• Confirm button: full width, primary style
```

---

## 11. Venue Selection

```
┌────────────────────────────────────────┐
│                                        │
│   ←  Select Venue                      │
│                                        │
│                                        │
│   20 guests · $360 budget              │
│                                        │
│                                        │
│   ╭─────────────────────────────────╮  │
│   │                                 │  │
│   │   Smith Family Backyard         │  │
│   │                                 │  │
│   │   10-50 · $300 · ★ 3.0         │  │
│   │   Outdoor                       │  │
│   │                                 │  │
│   │   ⚠ Weather risk on Apr 5      │  │
│   │                                 │  │
│   ╰─────────────────────────────────╯  │
│                                        │
│   ╭─────────────────────────────────╮  │
│   │                                 │  │
│   │   Riverside Community Center    │  │
│   │                                 │  │
│   │   20-100 · $660 · ★ 3.5        │  │
│   │   Indoor                        │  │
│   │                                 │  │
│   │   ⚠ $300 over budget           │  │
│   │                                 │  │
│   ╰─────────────────────────────────╯  │
│                                        │
│                                        │
└────────────────────────────────────────┘

• Context line: guest count + budget
• Cards: simple, tappable
• Key info condensed: capacity · price · rating
• One tag: Indoor/Outdoor
• Warning: 12px, orange, only when relevant
• Tap card to select
```

---

## 12. Vendor Selection

```
┌────────────────────────────────────────┐
│                                        │
│   ←  Select Vendors                    │
│                                        │
│                                        │
│   Caterer   Entertain   Decor   More   │
│      ●                                 │
│   ─────────────────────────────────    │
│                                        │
│   Required · $300 budget               │
│                                        │
│                                        │
│   ╭─────────────────────────────────╮  │
│   │                                 │  │
│   │   Mom's Home Cooking            │  │
│   │   Budget · ★ 3.0                │  │
│   │                                 │  │
│   │   $300                          │  │
│   │                                 │  │
│   ╰─────────────────────────────────╯  │
│                                        │
│   ╭─────────────────────────────────╮  │
│   │                                 │  │
│   │   Delicious Bites               │  │
│   │   Standard · ★ 4.0              │  │
│   │                                 │  │
│   │   $500                          │  │
│   │   ⚠ $200 over budget           │  │
│   │                                 │  │
│   ╰─────────────────────────────────╯  │
│                                        │
│                                        │
│         Complete Planning  0/3         │
│                                        │
└────────────────────────────────────────┘

• Category tabs: text only, dot under active
• Required/Optional indicator
• Vendor cards: name, tier, rating, price
• Warning only when over budget
• Bottom: complete button with counter
```

---

## 13. Event Execution

```
┌────────────────────────────────────────┐
│                                        │
│                                        │
│                                        │
│   Event Day                            │
│   Emma's Princess Birthday             │
│                                        │
│                                        │
│   ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━────   │
│                                        │
│                                        │
│   2:00   Guests arriving       ✓       │
│   2:30   Food service          ✓       │
│   3:00   Entertainment         ✓       │
│   3:30   Cake ceremony         ·       │
│                                        │
│                                        │
│   ─────────────────────────────────    │
│                                        │
│   ⚠  Equipment Issue                   │
│                                        │
│   Speaker malfunction                  │
│   -15% satisfaction                    │
│                                        │
│                                        │
│   ╭─────────────────────────────────╮  │
│   │   Use contingency  $80    →    │  │
│   ╰─────────────────────────────────╯  │
│                                        │
│              Skip                      │
│                                        │
│   Contingency: $120 remaining          │
│                                        │
└────────────────────────────────────────┘

• Progress bar at top, thin
• Timeline: simple list, checkmarks for complete
• Random event: warning color header
• Impact stated clearly
• Mitigation: card with cost and arrow
• Skip: text button below
• Contingency status: small, bottom
```

---

## 14. Results

```
┌────────────────────────────────────────┐
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                  87                    │
│                                        │
│                                        │
│                                        │
│   Venue          ━━━━━━━━━━━━━━━━━ 85  │
│   Food           ━━━━━━━━━━━━━━━━━━ 90 │
│   Entertainment  ━━━━━━━━━━━━━━━── 65  │
│   Decorations    ━━━━━━━━━━━━━━━━━━ 95 │
│   Service        ━━━━━━━━━━━━━━━━━ 88  │
│                                        │
│   ─────────────────────────────────    │
│                                        │
│   Budget         $1,200                │
│   Spent          $1,080                │
│   Contingency       $80                │
│                                        │
│   Profit           +$288               │
│                                        │
│   Reputation        +18                │
│                                        │
│   ─────────────────────────────────    │
│                                        │
│   "Emma had the best birthday!"        │
│                                        │
│   🎁  Referral received                │
│                                        │
│              Continue                  │
│                                        │
└────────────────────────────────────────┘

• Score: 64px, bold, centered
  - Color: 90+ green, 70-89 teal, 50-69 orange, <50 red
• Category bars: thin, minimal labels
• Financials: right-aligned numbers, monospace
• Profit: green if positive, red if negative
• Reputation: gold color
• Quote: italic, smaller
• Referral: simple highlight
• Continue: primary button
```

---

## 15. Tutorial Highlight

```
┌────────────────────────────────────────┐
│░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░│
│░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░│
│░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░│
│░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░│
│░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░│
│░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░│
│░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░│
│░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░│
│░░░░░░░░                      ░░░░░░░░░░│
│░░░░░░░░       Accept         ░░░░░░░░░░│
│░░░░░░░░                      ░░░░░░░░░░│
│░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░│
│░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░│
│░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░│
│░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░│
│░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░│
│░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░│
│░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░│
│                                        │
│   Tap to accept this client            │
│   and start planning.                  │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│              Skip tutorial             │
│                                        │
└────────────────────────────────────────┘

• Dark overlay: #0D0D0D at 85% opacity
• Highlighted element: clear cutout
• Instruction: bottom of screen, 16px
• Keep it simple - one sentence
• Skip: text button, subtle
```

---

## 16. Notification

```
┌────────────────────────────────────────┐
│                                        │
│   ╭─────────────────────────────────╮  │
│   │                                 │  │
│   │   New client inquiry       ✕    │  │
│   │   Sarah wants to book you       │  │
│   │                                 │  │
│   ╰─────────────────────────────────╯  │
│                                        │
│                                        │
│                                        │
│          (Game content below)          │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
└────────────────────────────────────────┘

• Card: #1A1A1A, slides from top
• Title: 16px semibold
• Body: 14px, #B0B0B0
• Dismiss: ✕ right side
• Auto-dismiss after 4 seconds
• Tap card to navigate to relevant screen
```

---

## 17. Settings

```
┌────────────────────────────────────────┐
│                                        │
│   ←  Settings                          │
│                                        │
│                                        │
│   Sound                                │
│                                        │
│   Music                                │
│   ━━━━━━━━━━━━━━━━━━━━●────────────    │
│                                        │
│   Effects                              │
│   ━━━━━━━━━━━━━━━━━━━━━━━━●────────    │
│                                        │
│                                        │
│   Notifications                        │
│                                        │
│   Event reminders              ●───    │
│   Task deadlines               ●───    │
│   New inquiries                ───○    │
│                                        │
│                                        │
│   Display                              │
│                                        │
│   Text size         S    M    L        │
│                          ●             │
│                                        │
│                                        │
│   ─────────────────────────────────    │
│                                        │
│   Restore purchases                    │
│   Privacy                              │
│   Reset game                           │
│                                        │
│                              v1.0.0    │
│                                        │
└────────────────────────────────────────┘

• Section headers: 12px, #606060, uppercase
• Sliders: thin, minimal
• Toggles: simple pill shape
• Text size: segmented control
• Bottom links: text buttons
• Version: 12px, #606060, bottom-right
```

---

## 18. Empty States

```
┌────────────────────────────────────────┐
│                                        │
│   ←  Calendar                     ✕    │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│           No upcoming events           │
│                                        │
│       Accept a client to get started   │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
│                                        │
└────────────────────────────────────────┘

• Primary text: 18px, white
• Secondary text: 14px, #606060
• Centered vertically
• No illustrations - just text
• Keep it helpful, not cute
```

---

## Design Tokens Summary

```
┌─────────────────────────────────────────────────────────────┐
│                                                             │
│  COLORS                                                     │
│  Background:    #0D0D0D                                     │
│  Surface:       #1A1A1A                                     │
│  Elevated:      #252525                                     │
│  Text:          #FFFFFF / #B0B0B0 / #606060                │
│  Money:         #4ECDC4                                     │
│  Reputation:    #FFE66D                                     │
│  Success:       #7BC950                                     │
│  Warning:       #F7931A                                     │
│  Error:         #E84855                                     │
│  Accent:        #667EEA                                     │
│                                                             │
│  TYPOGRAPHY                                                 │
│  Display:       48px Bold                                   │
│  H1:            32px Semibold                               │
│  H2:            24px Semibold                               │
│  H3:            20px Semibold                               │
│  Body:          16px Regular                                │
│  Caption:       14px Regular                                │
│  Micro:         12px Medium                                 │
│                                                             │
│  SPACING                                                    │
│  Base unit:     8px                                         │
│  XS: 8 / SM: 16 / MD: 24 / LG: 32 / XL: 48                 │
│                                                             │
│  RADIUS                                                     │
│  Small:         8px  (buttons, inputs)                      │
│  Medium:        12px (cards)                                │
│  Large:         16px (modals)                               │
│  Full:          9999px (pills, badges)                      │
│                                                             │
│  ANIMATION                                                  │
│  Fast:          100ms                                       │
│  Normal:        200ms                                       │
│  Slow:          300ms                                       │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```
