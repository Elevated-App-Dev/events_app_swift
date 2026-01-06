# Requirements Analysis: Monetization & Game Design Review

## Overview

This analysis compares the requirements document against the monetization strategy PDF and evaluates the overall game design for mobile market fit.

---

## Monetization Comparison

### What's Covered ✅

| PDF Strategy | Requirements Coverage |
|--------------|----------------------|
| **Rewarded Video Ads** | ✅ Covered in Req 27-29: emergency mitigation, overtime hours, emergency funding, time-skip |
| **Interstitial Ads** | ✅ Req 29.5: "interstitial ads at natural break points" for non-VIP players |
| **Currency Packs** | ⚠️ Partial - Req 28.1 offers 4 tiers ($0.99-$19.99), PDF recommends 6 tiers up to $49.99 |
| **Bundles/Starter Packs** | ✅ Req 28.2-3: Starter Pack, Premium Venue Pack, Elite Vendor Network |
| **Subscription (VIP)** | ✅ Req 28.4: VIP Subscription with daily rewards, ad-free, Premium Idle Mode |
| **One-Time Ad Removal** | ✅ Req 28.3: "No Ads" as permanent unlock |
| **Avoid Pay-to-Win** | ✅ Req 29.6: "never force ad viewing for core gameplay" |
| **Patience Monetization (timers)** | ✅ Req 11.11: time-skip via rewarded ads, Req 10.16-18: overtime ads |

### Gaps Identified 🔴

1. **Currency Tier Count Mismatch**
   - PDF recommends: 6 currency tiers ranging $0.99 to $49.99
   - Requirements have: Only 4 tiers (Small $0.99, Medium $4.99, Large $9.99, Mega $19.99)
   - Missing: 2 additional tiers, especially a high-value option ($49.99)

2. **Limited-Time Sales / Seasonal Content (20% of IAP revenue)**
   - PDF recommends: Seasonal decoration packs, special event types as limited-time offers
   - Requirements: No mention of limited-time sales, seasonal content, or rotating offers

3. **Double Rewards Ad Option**
   - PDF recommends: "Double rewards for watching an ad after successful events"
   - Requirements: Not present - events complete with standard rewards only

4. **Temporary Premium Access via Ads**
   - PDF recommends: "Offer temporary access to premium decorations or venues" via ads
   - Requirements: Ads only grant currency, time, or emergency help - not temporary premium unlocks

5. **Multi-Tier Subscription Structure**
   - PDF recommends: 3 tiers (Basic, Pro, Elite) with escalating benefits including exclusive event types
   - Requirements have: Single VIP subscription tier

6. **Bonus Tips/Currency After Events**
   - PDF recommends: "Provide bonus tips/currency after completing events" via ads
   - Requirements: Post-event ad opportunity not specified

7. **Value Communication UI**
   - PDF recommends: "Clearly show what players get; use discounts and 'best value' tags"
   - Requirements: No specific UI requirements for IAP presentation, discount displays, or value indicators

### Monetization Conclusion

The missing strategies (limited-time sales, double rewards, extra currency tiers, multi-tier subscriptions) are **not critical** for this game. These tactics work best for casual/idle games with shallow engagement loops.

This game is designed for players who enjoy **strategy and planning depth** - closer to Pocket City or Kairosoft titles than My Perfect Hotel. The existing monetization is appropriate for the target audience. Adding aggressive monetization would feel incongruent with the experience.

---

## Target Audience Assessment

### Size Estimate

**Niche but dedicated: 100,000-500,000 potential players**

Target players who enjoy:
- Business simulation with actual depth (Kairosoft fans, Game Dev Tycoon, Project Highrise)
- Management games where decisions have consequences
- Mobile games that respect their time and intelligence

For context:
- Kairosoft games: 100K-1M downloads per title
- Pocket City: ~1M paid downloads (major indie success)
- Game Dev Tycoon: ~2M copies across all platforms over many years

---

## Complexity Assessment: Is It Too Cerebral?

### Areas of Concern

1. **Hidden mechanics system** (Req 14.18-14.21) - Players discovering reputation bonuses through gameplay works on PC/console where players invest hours. Mobile sessions are 5-15 minutes.

2. **Workload capacity math** - Optimal/comfortable/strained with percentage penalties per event over threshold is genuinely complex. Most players won't internalize "7% satisfaction reduction per event over comfortable AND 10% increased task failure probability."

3. **Stage 2's dual income streams** - Company assignments vs side gigs with separate tracking, profit splits, and performance reviews adds cognitive load that might overwhelm casual players.

4. **Satisfaction calculation** - Weighted categories, random event modifiers, personality thresholds, and hidden bonuses may leave players confused about why they got 73% vs 78%.

### Recommendation: Option B - Simplified Surface, Optional Depth

Hide complexity until players are ready for it. Current design front-loads systems that could be introduced gradually.

---

## Option B: Progressive Complexity Unlocks

### Stage 1 Simplifications

- Remove workload capacity tiers entirely. Just let players take 2-3 events max with a soft warning at 3.
- Satisfaction calculation shows only the final score, not category breakdowns.
- Remove daily work hours system initially. Tasks just have deadlines.
- Weather system exists but only as "good day" or "bad day" - no forecast checking required.

### Introduce Complexity as Rewards

| Stage | Unlocks |
|-------|---------|
| Stage 2 | Work hours system, category score breakdowns, side gigs |
| Stage 3 | Workload capacity tiers, detailed weather forecasts, vendor hidden attributes |
| Stage 4 | Full satisfaction formula visibility, advanced analytics in Bank app |
| Stage 5 | All systems fully exposed, "pro mode" optional toggle |

### Tutorial Simplification

**Current tutorial teaches:** client intake, budget allocation, venue selection, vendor selection, task management, phone apps, weather checking, contingency budgets.

**Simplified tutorial:** Accept client, pick venue, pick caterer, event happens, see results. Done in 3 minutes. Everything else discovered naturally or unlocked later.

### Features That Could Be Deferred to Post-Launch Updates

| Current MVP | Could Be Update 1 | Could Be Update 2 |
|-------------|-------------------|-------------------|
| 3 personalities | Indecisive personality | Demanding, Celebrity |
| Basic workload warnings | Capacity tier system | Stress weight multipliers |
| Simple weather (good/bad) | 7-day forecasts | Accuracy percentages |
| Single satisfaction score | Category breakdowns | Weighted formula visibility |
| Stage 1-2 content | Stage 3 | Stages 4-5 |
| Basic vendor info | Hidden attributes reveal | Relationship discounts |
| Family emergency funds | - | Bank loans system |

---

## Game Length Analysis

### Current Design Completion Time

| Stage | Requirements | Estimated Time |
|-------|--------------|----------------|
| Stage 1 | 25 reputation + $5,000 savings | 2-4 hours |
| Stage 2 | Senior Planner (Level 5) + 50 reputation + $25,000 | 6-10 hours |
| Stage 3+ | Progressively longer requirements | 15-25+ hours |
| **Total** | | **25-40+ hours** |

### Mobile Sim Benchmarks

| Game | Completion Time | Session Length |
|------|-----------------|----------------|
| My Perfect Hotel | 10-15 hours | 5-10 min |
| Idle games generally | 20-50 hours (mostly idle) | 2-5 min |
| Kairosoft games | 8-15 hours | 15-30 min |
| Pocket City | 6-10 hours | 15-30 min |

### Assessment

Current design asks for Kairosoft-level session engagement but with longer total playtime. **Recommendation:** Stage 1-2 should feel complete at 8-12 hours. Stages 3-5 become "endgame content" for dedicated players.

---

## Satisfying Midpoint at Stage 3

### The Problem

Stage 3 is where the player makes a major choice (Entrepreneur vs Corporate path), but there's no narrative payoff - it's just a gate to more content. The game has no real ending, just progressively harder stages.

### Recommended Solution

When a player reaches Stage 3, implement:

1. **Narrative moment** - The office signing (Entrepreneur) or Director announcement (Corporate)
   - Entrepreneur path: Sign lease on first real office, former client sends flowers, family calls to say they're proud, small newspaper feature
   - Corporate path: Announced as Director at company meeting, mentor gives speech, reserved parking spot, name on door

2. **Career summary screen** - Stats from their journey (first event, biggest success, worst disaster, total events, money earned)

3. **Credits roll** - Yes, actual credits. Signals "you beat the game." Followed by "Your story continues..." button.

4. **Clear UI labeling** - Stages 4-5 labeled as "Expansion Mode" or "Endgame Content"

### Why This Works

- Players who stop at Stage 3 feel satisfied
- Players who continue feel like they're in "post-game" bonus content
- No one feels like they quit halfway
- Mobile players who drop off at 10 hours leave satisfied
- Dedicated players who want Stages 4-5 have content waiting

---

## Web Conversion Assessment

### Technical Feasibility

Unity to Web is straightforward in theory:
- Unity has WebGL export built-in
- Game has no platform-specific features
- Local save system translates to browser localStorage or IndexedDB

### Challenges

| Challenge | Difficulty | Notes |
|-----------|------------|-------|
| Performance | Medium | Unity WebGL builds are larger and slower. UI-heavy game should be fine. |
| Monetization | High | Unity Ads and IAP don't work on web. Need alternative solutions. |
| Save Data | Medium | localStorage has size limits (~5-10MB). May need cloud backend. |
| Mobile Web | Medium-High | Touch controls work but WebGL on mobile browsers is inconsistent. |
| Distribution | Different | No App Store discovery. Rely on portals or self-hosting. |

### Effort Estimate

- **If planned from start:** 15-20% additional development time
- **If converted after mobile launch:** 25-35% of original development time

### Recommendation

Build with web export in mind (abstract platform-specific layers), but launch mobile-first. Web version can come later as marketing tool or secondary platform.

---

## Summary of Recommendations

1. **Monetization:** Current coverage is appropriate. Don't add aggressive tactics that conflict with the game's identity.

2. **Complexity:** Implement Option B - simplify Stage 1-2, introduce complexity as unlocks, defer some systems to post-launch updates.

3. **Game Length:** Add satisfying midpoint at Stage 3 with narrative closure, credits, and clear "expansion mode" labeling for Stages 4-5.

4. **Web Version:** Plan for it in architecture but prioritize mobile launch.
