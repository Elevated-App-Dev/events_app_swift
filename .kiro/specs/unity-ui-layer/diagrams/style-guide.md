# Visual Style Guide

## Design Philosophy

Inspired by **Mini Metro's** radical minimalism, **Coffee Inc 2 / Pocket City's** charming map aesthetics, and **Two Point Hospital's** personality-driven simulation feel.

### Core Principles

1. **Only What's Needed** - If it doesn't help the player make a decision or understand state, remove it
2. **Breathing Room** - Generous whitespace, elements don't crowd each other
3. **Clarity Over Decoration** - Information hierarchy through size and position, not ornament
4. **Subtle Personality** - Charm through animation and micro-interactions, not visual clutter
5. **Confident Color** - Bold, purposeful color choices; no gradients or shadows

---

## Color Palette

### Primary Palette (Mini Metro Inspired)

```
┌─────────────────────────────────────────────────────────────┐
│                                                             │
│  BACKGROUND         SURFACE           ELEVATED              │
│  ┌─────────┐       ┌─────────┐       ┌─────────┐           │
│  │         │       │         │       │         │           │
│  │ #0D0D0D │       │ #1A1A1A │       │ #252525 │           │
│  │         │       │         │       │         │           │
│  └─────────┘       └─────────┘       └─────────┘           │
│  Pure black        Cards/Panels      Hover/Active          │
│                                                             │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│                                                             │
│  TEXT PRIMARY      TEXT SECONDARY    TEXT MUTED             │
│  ┌─────────┐       ┌─────────┐       ┌─────────┐           │
│  │         │       │         │       │         │           │
│  │ #FFFFFF │       │ #B0B0B0 │       │ #606060 │           │
│  │         │       │         │       │         │           │
│  └─────────┘       └─────────┘       └─────────┘           │
│  Headlines         Body text         Disabled/Hints        │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

### Accent Colors (Transit Line Inspired)

```
┌─────────────────────────────────────────────────────────────┐
│                                                             │
│  Each color has ONE meaning. Used sparingly, purposefully.  │
│                                                             │
│  MONEY             REPUTATION        SUCCESS                │
│  ┌─────────┐       ┌─────────┐       ┌─────────┐           │
│  │         │       │         │       │         │           │
│  │ #4ECDC4 │       │ #FFE66D │       │ #7BC950 │           │
│  │         │       │         │       │         │           │
│  └─────────┘       └─────────┘       └─────────┘           │
│  Teal              Gold/Yellow       Green                  │
│  Finances          Stars/Rating      Completed/Positive     │
│                                                             │
│  WARNING           ERROR             ACCENT                 │
│  ┌─────────┐       ┌─────────┐       ┌─────────┐           │
│  │         │       │         │       │         │           │
│  │ #F7931A │       │ #E84855 │       │ #667EEA │           │
│  │         │       │         │       │         │           │
│  └─────────┘       └─────────┘       └─────────┘           │
│  Orange            Red               Purple/Blue            │
│  Attention         Negative/Loss     Interactive/Links      │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

### Map Zone Colors (Pocket City / Coffee Inc Inspired)

```
┌─────────────────────────────────────────────────────────────┐
│                                                             │
│  Soft, muted tones that don't compete with UI elements      │
│                                                             │
│  NEIGHBORHOOD      DOWNTOWN          UPTOWN                 │
│  ┌─────────┐       ┌─────────┐       ┌─────────┐           │
│  │         │       │         │       │         │           │
│  │ #2D4739 │       │ #3D3D5C │       │ #4A3D5C │           │
│  │         │       │         │       │         │           │
│  └─────────┘       └─────────┘       └─────────┘           │
│  Soft Green        Muted Blue        Soft Purple            │
│                                                             │
│  WATERFRONT        LOCKED                                   │
│  ┌─────────┐       ┌─────────┐                             │
│  │         │       │         │                             │
│  │ #2D4A5C │       │ #1A1A1A │                             │
│  │         │       │  + 50%  │                             │
│  └─────────┘       └─────────┘                             │
│  Ocean Blue        Darkened base                           │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

---

## Typography

### Font Stack

```
Primary:    Inter (or SF Pro on iOS)
Monospace:  JetBrains Mono (for numbers/money)
```

### Type Scale

```
┌─────────────────────────────────────────────────────────────┐
│                                                             │
│  DISPLAY        48px   Bold      Game title only            │
│  ─────────────────────────────────────────────────────────  │
│                                                             │
│  H1             32px   Semibold  Screen titles              │
│  ─────────────────────────────────────────────────────────  │
│                                                             │
│  H2             24px   Semibold  Section headers            │
│  ─────────────────────────────────────────────────────────  │
│                                                             │
│  H3             20px   Medium    Card titles                │
│  ─────────────────────────────────────────────────────────  │
│                                                             │
│  Body           16px   Regular   Primary content            │
│  ─────────────────────────────────────────────────────────  │
│                                                             │
│  Caption        14px   Regular   Secondary info             │
│  ─────────────────────────────────────────────────────────  │
│                                                             │
│  Micro          12px   Medium    Badges, labels             │
│  ─────────────────────────────────────────────────────────  │
│                                                             │
│  NUMBERS        Tabular numerals (monospace) for alignment  │
│  $2,450         Always use monospace for financial data     │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

### Line Height

- Headlines: 1.1 (tight)
- Body: 1.5 (comfortable reading)
- UI Labels: 1.2 (compact but readable)

---

## Spacing System

### Base Unit: 8px

All spacing uses multiples of 8px for consistency.

```
┌─────────────────────────────────────────────────────────────┐
│                                                             │
│  XS      8px     Tight spacing within components            │
│  SM      16px    Default element spacing                    │
│  MD      24px    Section spacing                            │
│  LG      32px    Major section breaks                       │
│  XL      48px    Screen-level padding                       │
│  2XL     64px    Hero spacing                               │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

### Screen Margins

```
┌────────────────────────────────────────┐
│◄──────────── 24px ────────────────────►│
│ ┌────────────────────────────────────┐ │
│ │                                    │ │
│ │         Content Area               │ │
│ │                                    │ │
│ └────────────────────────────────────┘ │
│◄──────────── 24px ────────────────────►│
└────────────────────────────────────────┘

Safe area insets are IN ADDITION to margins.
```

---

## Components

### Cards

```
┌─────────────────────────────────────────────────────────────┐
│                                                             │
│  MINIMAL CARD (Default)                                     │
│  ┌────────────────────────────────┐                        │
│  │                                │  Background: #1A1A1A    │
│  │  Content                       │  Border: none           │
│  │                                │  Border-radius: 12px    │
│  │                                │  Padding: 16px          │
│  └────────────────────────────────┘  Shadow: none           │
│                                                             │
│  INTERACTIVE CARD (Tappable)                                │
│  ┌────────────────────────────────┐                        │
│  │                                │  Same as above          │
│  │  Content                       │  + Hover: #252525       │
│  │                                │  + Active: scale(0.98)  │
│  │                                │                         │
│  └────────────────────────────────┘                        │
│                                                             │
│  ELEVATED CARD (Modal/Important)                            │
│  ┌────────────────────────────────┐                        │
│  │                                │  Background: #252525    │
│  │  Content                       │  Border: 1px #333       │
│  │                                │  Border-radius: 16px    │
│  │                                │                         │
│  └────────────────────────────────┘                        │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

### Buttons

```
┌─────────────────────────────────────────────────────────────┐
│                                                             │
│  PRIMARY BUTTON                                             │
│  ┌────────────────────────────────┐                        │
│  │          ACCEPT                │  Background: #667EEA    │
│  └────────────────────────────────┘  Text: #FFFFFF          │
│                                       Height: 56px          │
│                                       Border-radius: 12px   │
│                                       Font: 16px Semibold   │
│                                                             │
│  SECONDARY BUTTON                                           │
│  ┌────────────────────────────────┐                        │
│  │          DECLINE               │  Background: transparent│
│  └────────────────────────────────┘  Border: 1px #606060    │
│                                       Text: #B0B0B0         │
│                                       Height: 56px          │
│                                                             │
│  TEXT BUTTON (Minimal)                                      │
│                                                             │
│          Skip Tutorial               Background: none       │
│                                       Text: #667EEA         │
│                                       Underline on hover    │
│                                                             │
│  ICON BUTTON                                                │
│       ┌────┐                                               │
│       │ ✕  │                         Size: 44x44 min       │
│       └────┘                         Background: none       │
│                                       Hover: #252525 circle │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

### Input Controls

```
┌─────────────────────────────────────────────────────────────┐
│                                                             │
│  SLIDER (Mini Metro style - simple line)                    │
│                                                             │
│  Venue Budget                                               │
│  ●━━━━━━━━━━━━━━━━━━━○──────────────  $360                 │
│                                                             │
│  Track: 2px line, #333                                      │
│  Fill: 2px line, #667EEA                                    │
│  Thumb: 16px circle, #FFFFFF                                │
│  No tick marks - clean                                      │
│                                                             │
│                                                             │
│  TOGGLE                                                     │
│                                                             │
│  Notifications  ┌──────────●│                              │
│                 └───────────┘  ON                           │
│                                                             │
│  Track: 24px tall, rounded pill                             │
│  OFF: #333 background                                       │
│  ON: #667EEA background                                     │
│  Thumb: #FFFFFF circle                                      │
│                                                             │
│                                                             │
│  SEGMENTED CONTROL                                          │
│                                                             │
│  ┌─────────┬─────────┬─────────┐                           │
│  │   All   │ Venues  │ Vendors │                           │
│  └─────────┴─────────┴─────────┘                           │
│                                                             │
│  Background: #1A1A1A                                        │
│  Selected: #252525 with subtle indicator line below         │
│  Text: #B0B0B0 inactive, #FFFFFF active                     │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

### Progress Indicators

```
┌─────────────────────────────────────────────────────────────┐
│                                                             │
│  PROGRESS BAR (Thin, Mini Metro style)                      │
│                                                             │
│  ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━────────────  75%     │
│                                                             │
│  Height: 4px                                                │
│  Track: #333                                                │
│  Fill: #667EEA (or contextual color)                        │
│  Border-radius: 2px                                         │
│  No percentage text ON bar - beside it if needed            │
│                                                             │
│                                                             │
│  SATISFACTION SCORE (Large, Centered)                       │
│                                                             │
│                   87                                        │
│                                                             │
│  Just the number. Big. Bold. Color indicates quality.       │
│  90+ : #7BC950 (green)                                      │
│  70-89: #4ECDC4 (teal)                                      │
│  50-69: #F7931A (orange)                                    │
│  <50 : #E84855 (red)                                        │
│                                                             │
│                                                             │
│  CATEGORY BARS (Results breakdown)                          │
│                                                             │
│  Venue         ━━━━━━━━━━━━━━━━━━━━━━━━━━────  85          │
│  Food          ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━  90          │
│  Entertainment ━━━━━━━━━━━━━━━━━━────────────  65          │
│                                                             │
│  Labels left-aligned, bars expand right, number at end      │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

### Badges & Status Indicators

```
┌─────────────────────────────────────────────────────────────┐
│                                                             │
│  NOTIFICATION BADGE                                         │
│        ┌───┐                                               │
│        │ 3 │   Min-width: 20px                             │
│        └───┘   Height: 20px                                │
│                Background: #E84855                          │
│                Text: #FFFFFF, 12px Bold                     │
│                Border-radius: 10px (pill)                   │
│                                                             │
│                                                             │
│  STATUS DOT                                                 │
│                                                             │
│  ● Active     8px circle, color indicates status            │
│  ● Pending    #F7931A                                       │
│  ● Complete   #7BC950                                       │
│  ● Failed     #E84855                                       │
│                                                             │
│                                                             │
│  TAG/CHIP                                                   │
│  ┌──────────────┐                                          │
│  │ Easy-Going   │   Background: #252525                    │
│  └──────────────┘   Text: #B0B0B0, 12px                    │
│                     Padding: 4px 12px                       │
│                     Border-radius: 16px (pill)              │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

---

## Iconography

### Style Guidelines

```
┌─────────────────────────────────────────────────────────────┐
│                                                             │
│  • Line icons, not filled (matches Mini Metro)              │
│  • 2px stroke weight                                        │
│  • Rounded caps and joins                                   │
│  • 24x24 base size (scale to 20, 28, 32 as needed)         │
│  • Single color (inherits from text color)                  │
│  • Geometric, not organic                                   │
│                                                             │
│  EXAMPLES:                                                  │
│                                                             │
│   ╭───╮      ╭─────╮      ○──○      ┌─┬─┐                  │
│   │ ☰ │      │ ╳   │      │  │      ├─┼─┤   Calendar       │
│   ╰───╯      ╰─────╯      ○──○      └─┴─┘                  │
│   Menu       Close        Map       Grid                    │
│                                                             │
│   $          ★            ●──       ⚙                      │
│   Money      Rep          Progress  Settings                │
│                                                             │
│  Use Phosphor Icons, Feather Icons, or similar              │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

---

## Animation

### Principles

1. **Quick and Subtle** - Animations should feel snappy, not sluggish
2. **Purposeful** - Only animate when it aids understanding
3. **Consistent Timing** - Use standard durations throughout

### Timing

```
┌─────────────────────────────────────────────────────────────┐
│                                                             │
│  INSTANT       0ms        State changes (color, opacity)    │
│  FAST          100ms      Button feedback, small changes    │
│  NORMAL        200ms      Panel transitions, reveals        │
│  SLOW          300ms      Full-screen transitions           │
│  EMPHASIS      400ms      Celebration moments               │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

### Easing

```
┌─────────────────────────────────────────────────────────────┐
│                                                             │
│  EASE-OUT      Elements entering (decelerates in)           │
│                cubic-bezier(0, 0, 0.2, 1)                   │
│                                                             │
│  EASE-IN       Elements exiting (accelerates out)           │
│                cubic-bezier(0.4, 0, 1, 1)                   │
│                                                             │
│  EASE-IN-OUT   Moving between states                        │
│                cubic-bezier(0.4, 0, 0.2, 1)                 │
│                                                             │
│  LINEAR        Progress bars, loading indicators            │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

### Standard Animations

```
┌─────────────────────────────────────────────────────────────┐
│                                                             │
│  BUTTON PRESS                                               │
│  scale(1.0) → scale(0.97) → scale(1.0)                     │
│  Duration: 100ms down, 150ms up                             │
│                                                             │
│  PANEL SLIDE (Phone overlay)                                │
│  translateY(100%) → translateY(0)                           │
│  Duration: 250ms, Ease-out                                  │
│                                                             │
│  FADE IN                                                    │
│  opacity(0) → opacity(1)                                    │
│  Duration: 200ms, Ease-out                                  │
│                                                             │
│  VALUE CHANGE (Money, reputation)                           │
│  Count up/down numerically                                  │
│  Duration: 400ms                                            │
│  Brief color flash: gain = green, loss = red                │
│                                                             │
│  NOTIFICATION                                               │
│  translateY(-100%) → translateY(0) → hold → translateY(-100%)│
│  Duration: 200ms in, 4000ms hold, 200ms out                 │
│                                                             │
│  SCORE REVEAL (Results screen)                              │
│  Scale from 0.8 → 1.0 + opacity 0 → 1                       │
│  Duration: 300ms, Ease-out                                  │
│  Slight overshoot: scale to 1.05 then 1.0                   │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

---

## Map Design (Coffee Inc / Pocket City Inspired)

### Visual Style

```
┌─────────────────────────────────────────────────────────────┐
│                                                             │
│  • Flat, geometric shapes - no 3D or isometric              │
│  • Soft, muted zone colors (see palette above)              │
│  • Simple road network: 2-3px lines, #333 or slightly       │
│    lighter than zone                                        │
│  • Location pins: simple circles or rounded rectangles      │
│  • No texture fills - solid colors only                     │
│  • Subtle zone boundaries - 1px lighter line or gap         │
│                                                             │
│  ZONE STYLE:                                                │
│  ┌────────────────────────────────┐                        │
│  │ ╭─────────────────────────────╮│                        │
│  │ │                             ││                        │
│  │ │    ●        ●               ││ Rounded corners        │
│  │ │        ●         ●          ││ 8-12px radius          │
│  │ │    ●      [ZONE NAME]       ││                        │
│  │ │               ●    ●        ││ Name: subtle,          │
│  │ │                             ││ bottom-left            │
│  │ ╰─────────────────────────────╯│                        │
│  └────────────────────────────────┘                        │
│                                                             │
│  LOCATION PIN:                                              │
│      ╭───╮                                                 │
│      │ 🏠 │   24x24px                                       │
│      ╰───╯   Background: #252525 or zone-contrasting        │
│              Icon: 16px, white                              │
│              Border-radius: 6px                             │
│              Selected: #667EEA background                   │
│                                                             │
│  PIN TYPES:                                                 │
│  🏛  Venue (building icon)                                  │
│  👤 Vendor (person icon)                                    │
│  🏠 Office (home/office icon)                               │
│  ●  Meeting point (simple dot)                              │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

---

## Sound Design Notes

Audio should match the visual minimalism:

```
┌─────────────────────────────────────────────────────────────┐
│                                                             │
│  • Soft, subtle UI sounds - not attention-grabbing          │
│  • Muted tones for button presses                           │
│  • Gentle chime for notifications                           │
│  • Satisfying but brief success sound                       │
│  • Background music: lo-fi, ambient, unobtrusive            │
│                                                             │
│  Reference: Mini Metro's audio - functional, not distracting│
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

---

## Accessibility

```
┌─────────────────────────────────────────────────────────────┐
│                                                             │
│  CONTRAST                                                   │
│  • All text meets WCAG AA (4.5:1 for body, 3:1 for large)  │
│  • Critical info never conveyed by color alone              │
│                                                             │
│  TOUCH TARGETS                                              │
│  • Minimum 44x44pt for all interactive elements             │
│  • 8px minimum spacing between targets                      │
│                                                             │
│  MOTION                                                     │
│  • Respect "Reduce Motion" system setting                   │
│  • Provide alternative feedback when motion disabled        │
│                                                             │
│  TEXT                                                       │
│  • Support Dynamic Type / text scaling                      │
│  • Test at 200% scale                                       │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

---

## Do's and Don'ts

### DO

- Use generous whitespace
- Let content breathe
- Use color sparingly and meaningfully
- Keep animations quick and subtle
- Show only essential information
- Use consistent spacing multiples
- Trust the user's intelligence

### DON'T

- Add drop shadows
- Use gradients
- Add decorative elements
- Use more than 2 accent colors per screen
- Animate everything
- Show all information at once
- Use icons without purpose
- Add borders where spacing works
