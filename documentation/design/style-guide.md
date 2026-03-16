# Style Guide — Event Planning Simulator (SwiftUI)

Adapted from the visual style guide for native SwiftUI implementation. See [design-landscape-analysis.md](design-landscape-analysis.md) for the design philosophy and competitive context.

---

## Design Philosophy

Inspired by **Mini Metro's** radical minimalism, **Coffee Inc 2's** clean business sim dashboards, and **Pocket City's** mobile-native feel.

### Core Principles

1. **Game, Not App** — the player inhabits a world (office desk, city map), not a tab bar
2. **Only What's Needed** — if it doesn't help the player decide or understand state, remove it
3. **Breathing Room** — generous whitespace, elements don't crowd
4. **Charm Through Motion** — personality comes from animation and micro-interactions, not visual clutter
5. **Confident Color** — bold, purposeful; no gradients or shadows

---

## Color Palette

### Backgrounds & Surfaces

| Token | Hex | Usage | SwiftUI |
|-------|-----|-------|---------|
| background | `#0D0D0D` | Main screen bg | `Color("background")` |
| surface | `#1A1A1A` | Cards, panels | `Color("surface")` |
| elevated | `#252525` | Active/pressed states | `Color("elevated")` |

### Text

| Token | Hex | Usage |
|-------|-----|-------|
| textPrimary | `#FFFFFF` | Headlines, important |
| textSecondary | `#B0B0B0` | Body, descriptions |
| textMuted | `#606060` | Hints, disabled, labels |

### Accent Colors — each has ONE meaning

| Token | Hex | Meaning |
|-------|-----|---------|
| money | `#4ECDC4` | Teal — finances, payments |
| reputation | `#FFE66D` | Gold — stars, rating |
| success | `#7BC950` | Green — completed, positive |
| warning | `#F7931A` | Orange — attention needed |
| error | `#E84855` | Red — negative, loss, overdue |
| accent | `#667EEA` | Purple/blue — interactive, buttons |

### Map Zones

| Zone | Hex | Tone |
|------|-----|------|
| Neighborhood | `#2D4739` | Soft green |
| Downtown | `#3D3D5C` | Muted blue |
| Uptown | `#4A3D5C` | Soft purple |
| Waterfront | `#2D4A5C` | Ocean blue |
| Locked | `#1A1A1A` + 50% darkening | |

---

## Typography

### Fonts

- **Primary:** SF Pro (system font — `.body`, `.headline`, etc.)
- **Monospace:** SF Mono or system `.monospacedDigit()` for financial values

### Scale

| Style | Size | Weight | Usage | SwiftUI |
|-------|------|--------|-------|---------|
| Display | 48pt | Bold | Game title only | `.font(.system(size: 48, weight: .bold))` |
| H1 | 32pt | Semibold | Screen titles | `.font(.system(size: 32, weight: .semibold))` |
| H2 | 24pt | Semibold | Section headers | `.font(.system(size: 24, weight: .semibold))` |
| H3 | 20pt | Medium | Card titles | `.font(.system(size: 20, weight: .medium))` |
| Body | 16pt | Regular | Primary content | `.font(.body)` |
| Caption | 14pt | Regular | Secondary info | `.font(.caption)` |
| Micro | 12pt | Medium | Badges, labels | `.font(.system(size: 12, weight: .medium))` |

Financial values always use `.monospacedDigit()` for alignment.

---

## Spacing

### Base Unit: 8pt

| Token | Value | Usage |
|-------|-------|-------|
| xs | 8pt | Tight spacing within components |
| sm | 16pt | Default element spacing |
| md | 24pt | Section spacing, screen margins |
| lg | 32pt | Major section breaks |
| xl | 48pt | Screen-level padding |

Screen margins: 24pt horizontal (+ safe area insets).

---

## Components

### Cards

- **Default:** `#1A1A1A` bg, 12pt corner radius, 16pt padding, no border, no shadow
- **Interactive:** same + pressed state `#252525` + `scaleEffect(0.98)`
- **Elevated:** `#252525` bg, 1pt `#333333` border, 16pt corner radius

### Buttons

- **Primary:** `#667EEA` bg, white text, 56pt height, 12pt radius, semibold
- **Secondary:** transparent bg, 1pt `#606060` border, `#B0B0B0` text, 56pt height
- **Text:** no bg, `#667EEA` text
- **Icon:** 44x44pt minimum, no bg

### Progress

- **Bar:** 4pt height, `#333333` track, accent fill, 2pt radius
- **Satisfaction score:** just the number, large, bold, color by range (90+ green, 70-89 teal, 50-69 orange, <50 red)

### Badges & Status

- **Notification badge:** 20pt, `#E84855` bg, white text, pill shape
- **Status dot:** 8pt circle (pending=orange, complete=green, failed=red)
- **Tag/chip:** `#252525` bg, `#B0B0B0` text, 12pt, pill shape

---

## Icons

- SF Symbols — use line-weight variants where available
- 24pt base size
- Single color, inherits from text
- Geometric over organic

---

## Animation

### Timing

| Speed | Duration | Usage |
|-------|----------|-------|
| Fast | 0.1s | Button feedback |
| Normal | 0.2s | Panel transitions, reveals |
| Slow | 0.3s | Full-screen transitions |
| Emphasis | 0.4s | Celebrations, score reveals |

### Standard Animations

- **Button press:** `scaleEffect` 1.0 → 0.97 → 1.0, spring
- **Phone overlay slide:** offset from bottom, 0.25s, `.easeOut`
- **Value change (money/rep):** count up/down numerically, 0.4s, brief color flash
- **Score reveal:** scale 0.8 → 1.05 → 1.0, 0.3s, `.easeOut`

### SwiftUI Notes

- Use `.animation(.spring())` for natural motion
- Phase Animators for multi-step sequences
- Respect `@Environment(\.accessibilityReduceMotion)` — provide instant fallbacks

---

## Accessibility

- All text meets WCAG AA contrast (4.5:1 body, 3:1 large)
- Critical info never conveyed by color alone
- 44pt minimum touch targets, 8pt spacing between
- Respect Reduce Motion setting
- Support Dynamic Type

---

## Do / Don't

**Do:** generous whitespace, meaningful color, quick animations, trust the player

**Don't:** shadows, gradients, decorative elements, >2 accent colors per screen, borders where spacing works
