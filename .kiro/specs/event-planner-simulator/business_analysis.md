# Event Planning Simulator - Business Analysis

## Monetization Strategy Recommendation

### Recommendation: Free-to-Play (F2P) with Optional Purchases

For a mobile simulation/tycoon game targeting casual-to-midcore players, **F2P with cosmetic and convenience IAP** is the optimal strategy.

### Why F2P Over Paid

| Factor | Paid ($2.99-$4.99) | F2P + IAP |
|--------|-------------------|-----------|
| **Downloads** | Lower (price barrier) | 10-50x higher |
| **Discoverability** | Harder to rank | Easier organic growth |
| **Revenue ceiling** | Fixed per user | Unlimited (whales) |
| **Retention pressure** | Lower | Higher (must engage) |
| **Market expectation** | Niche | Industry standard |

### Recommended IAP Tiers

| Tier | Price | Content |
|------|-------|---------|
| **Starter Pack** | $1.99 | 500 premium currency + 1 exclusive decor item (one-time) |
| **Event Boost** | $2.99 | Skip 1 event cooldown + bonus reputation multiplier |
| **Decor Bundle** | $4.99 | 5 themed decoration packs |
| **VIP Pass** | $9.99/month | Daily currency, early venue unlocks, ad-free |
| **Premium Currency** | $0.99-$19.99 | Scaling gem packs |

### Why This Works for Event Planning Simulator

1. **Decoration-heavy games monetize well** - Players love expressing creativity
2. **Time-skip IAP fits naturally** - Event cooldowns create organic monetization moments
3. **No pay-to-win concerns** - All gameplay achievable F2P; IAP is convenience/cosmetic
4. **Broader audience** - Casual players try free games; converts happen over time

### Risk Mitigation

- **Generous F2P path**: Ensure all content is earnable without payment (just slower)
- **No energy gates at launch**: Avoid frustration; add later if retention is strong
- **Transparent pricing**: Show value clearly; avoid predatory patterns

---

## Revenue Projections (First 6 Months)

### Key Assumptions

| Metric | Conservative | Moderate | Optimistic |
|--------|-------------|----------|------------|
| **Organic downloads** | 5,000 | 15,000 | 40,000 |
| **Day 1 retention** | 30% | 40% | 50% |
| **Day 30 retention** | 5% | 10% | 15% |
| **Payer conversion** | 1.5% | 3% | 5% |
| **ARPPU** (avg rev/paying user) | $8 | $12 | $18 |

### Revenue Calculations

#### Conservative Scenario
- Downloads: 5,000
- Payers: 5,000 × 1.5% = 75
- Revenue: 75 × $8 = **$600**
- Ad revenue (if enabled): ~$200
- **Total: ~$800**

#### Moderate Scenario
- Downloads: 15,000
- Payers: 15,000 × 3% = 450
- Revenue: 450 × $12 = **$5,400**
- Ad revenue: ~$1,000
- **Total: ~$6,400**

#### Optimistic Scenario
- Downloads: 40,000
- Payers: 40,000 × 5% = 2,000
- Revenue: 2,000 × $18 = **$36,000**
- Ad revenue: ~$4,000
- **Total: ~$40,000**

### Revenue Summary Table

| Scenario | 6-Month Revenue | Monthly Average |
|----------|----------------|-----------------|
| Conservative | $800 | $133 |
| Moderate | $6,400 | $1,067 |
| Optimistic | $40,000 | $6,667 |

---

## Publishing & Operational Costs

### One-Time Publishing Costs

| Item | iOS | Android | Notes |
|------|-----|---------|-------|
| **Developer account** | $99/year | $25 (one-time) | Apple annual, Google lifetime |
| **App review** | $0 | $0 | Both free |
| **Privacy policy hosting** | ~$0-12/year | Same | Required; can use free hosting |
| **DUNS number** (if LLC/Corp) | $0 | N/A | Free from D&B |

**Total Year 1 Setup: ~$125-150**

### Ongoing Operational Costs

| Item | Monthly Cost | Notes |
|------|-------------|-------|
| **Apple Developer Program** | $8.25 | ($99/year) |
| **Backend/Analytics** | $0-50 | Firebase free tier covers most indie needs |
| **Cloud saves** (if implemented) | $0-20 | Firebase/PlayFab free tiers |
| **Ad mediation** | $0 | Revenue share only |
| **Push notifications** | $0-10 | Firebase free tier |

**Total Monthly Ops: $10-90** (realistically $20-40 for a solo dev)

### Platform Revenue Cuts

| Platform | Cut | Your Share |
|----------|-----|------------|
| Apple App Store | 30% (15% if <$1M/year) | 70-85% |
| Google Play | 30% (15% if <$1M/year) | 70-85% |

As a new developer under $1M annual revenue, you qualify for the **15% reduced commission** on both platforms.

---

## Maintenance Costs

### Time Investment (Solo AI-Assisted Dev)

| Task | Hours/Month | Notes |
|------|-------------|-------|
| Bug fixes | 5-15 | Depends on issue severity |
| OS compatibility updates | 2-5 | iOS/Android version bumps |
| Store listing updates | 1-2 | Screenshots, descriptions |
| Community/support | 5-10 | Reviews, emails, social |
| Content updates (optional) | 10-30 | New events, venues, items |

**Minimum maintenance: 15-30 hours/month**

### Dollar Costs

If you value your time at $50/hour:
- Minimum maintenance: $750-1,500/month
- With content updates: $1,500-2,500/month

### Actual Out-of-Pocket

| Item | Cost | Frequency |
|------|------|-----------|
| Apple Developer renewal | $99 | Annual |
| Backend services | $0-50 | Monthly |
| Asset purchases (if any) | $0-200 | As needed |
| Testing devices | $0 | Use existing |

**Realistic annual out-of-pocket: $200-800**

---

## Paid User Acquisition (UA) Analysis

### The Economics of Paid UA

For mobile games, paid advertising success depends on:
- **LTV (Lifetime Value)**: Total revenue a user generates
- **CPI (Cost Per Install)**: What you pay to acquire one user
- **Profit = LTV - CPI**

### Current LTV Estimates for Event Planning Simulator

| Scenario | Payer % | ARPPU | LTV/User |
|----------|---------|-------|----------|
| Conservative | 1.5% | $8 | $0.12 |
| Moderate | 3% | $12 | $0.36 |
| Optimistic | 5% | $18 | $0.90 |

### Current CPI Benchmarks (Casual/Simulation Games)

| Platform | CPI Range | Notes |
|----------|-----------|-------|
| Facebook/Meta | $1.50-4.00 | Highly competitive |
| TikTok | $1.00-3.00 | Younger audience |
| Google UAC | $0.80-2.50 | Broad but variable |
| Unity Ads | $0.50-2.00 | Gamer audience |
| Apple Search Ads | $2.00-5.00 | High intent |

### The Problem: LTV < CPI

Even in the **optimistic scenario** (LTV = $0.90), you're paying $1-4 per install. This means:

**Every dollar spent on ads loses money initially.**

To break even at $2 CPI, you need:
- LTV ≥ $2.00
- Which requires: 10% payer rate at $20 ARPPU, or 5% at $40 ARPPU

### What "Triple Revenue" Would Require

To triple the **Moderate scenario** ($6,400 → $19,200):
- Need ~30,000 additional downloads
- At $2 CPI = **$60,000 ad spend**
- Revenue increase: $12,800
- **Net loss: -$47,200**

This is why most indie games cannot profitably run paid UA.

---

## Big-Budget Mobile Game Marketing (Whiteout Survival, Last War)

### Who Makes These Games

These are developed by **Century Games** (Chinese publisher), part of a trend of "SLG" (strategy) games with massive UA budgets.

### Their Marketing Budgets

| Metric | Estimated Range |
|--------|----------------|
| **Monthly UA spend** | $20-50 million |
| **Annual marketing** | $200-500 million |
| **Cost per install** | $5-15 (premium markets) |
| **Daily ad impressions** | Billions globally |

### How They Make It Work

1. **Extremely high LTV**:
   - Whales spend $1,000-100,000+
   - ARPPU for payers: $100-500
   - LTV/install: $5-20 (justifies high CPI)

2. **Aggressive monetization**:
   - Pay-to-win mechanics
   - Limited-time offers creating FOMO
   - VIP systems with escalating benefits
   - Server-based competition (spend or lose)

3. **Scale economics**:
   - Millions of installs = data for optimization
   - Can A/B test thousands of ad creatives
   - Negotiate better rates with platforms

4. **Backed by massive capital**:
   - VC/publisher funding
   - Can sustain losses for months while optimizing

### Why You Can't (and Shouldn't) Compete

| Their Advantage | Your Reality |
|----------------|--------------|
| $50M/month budget | $500-5,000 budget |
| 200-person UA team | Solo developer |
| Aggressive P2W monetization | Ethical F2P design |
| 3-year runway to profitability | Need sustainable model |

---

## Realistic Marketing Budget Recommendations

### Phase 1: Pre-Launch (Cost: $0-200)

| Tactic | Cost | Expected Impact |
|--------|------|-----------------|
| Reddit community engagement | $0 | Build wishlist, get feedback |
| TikTok/YouTube devlogs | $0 | Organic reach, algorithm favors new creators |
| Press kit + outreach | $0 | Potential coverage |
| Discord server | $0 | Community building |
| Indie game festivals (digital) | $0-50 | Visibility among enthusiasts |

### Phase 2: Launch Month (Cost: $200-500)

| Tactic | Cost | Expected Impact |
|--------|------|-----------------|
| App Store Optimization (ASO) | $0 | Critical - keywords, screenshots, description |
| Small influencer outreach | $100-300 | 5-10 micro-influencers (1K-50K followers) |
| Reddit/Discord ads (test) | $50-100 | Targeted gaming communities |
| Cross-promotion | $0 | Partner with other indie devs |

### Phase 3: Post-Launch (Monthly: $100-500)

| Tactic | Cost | Expected Impact |
|--------|------|-----------------|
| Content updates (PR value) | $0 | Each update = news cycle |
| Community management | $0 (time) | Retention + word of mouth |
| Seasonal ASO updates | $0 | Capture seasonal search traffic |
| Micro-influencer partnerships | $100-300 | Ongoing visibility |
| Retargeting ads (if LTV improves) | $100-200 | Re-engage lapsed users |

### Budget Summary

| Phase | Budget | Timeline |
|-------|--------|----------|
| Pre-launch | $0-200 | 1-2 months before |
| Launch | $200-500 | Launch month |
| Ongoing | $100-500/month | Post-launch |

**Total Year 1 Marketing: $1,500-6,000**

### When to Scale Up

Only increase paid UA spending when:
1. **LTV exceeds $2-3**: You've optimized monetization
2. **Organic retention is strong**: D7 > 15%, D30 > 5%
3. **You have creative assets that convert**: Tested video ads
4. **You can measure attribution**: Proper analytics in place

---

## Paid Model Alternative ($2.99)

### Revenue Projections at $2.99

With a paid model, download volume drops significantly (10-15x lower than F2P) but each download generates guaranteed revenue.

| Scenario | Downloads | Net per Sale | 6-Month Revenue |
|----------|-----------|--------------|-----------------|
| Conservative | 500 | $2.54 | **$1,270** |
| Moderate | 1,500 | $2.54 | **$3,810** |
| Optimistic | 4,000 | $2.54 | **$10,160** |

*Net per sale assumes 15% platform cut (small developer program)*

### F2P vs Paid Comparison

| Scenario | F2P Revenue | Paid ($2.99) | Winner |
|----------|-------------|--------------|--------|
| Conservative | $800 | $1,270 | **Paid (+59%)** |
| Moderate | $6,400 | $3,810 | **F2P (+68%)** |
| Optimistic | $40,000 | $10,160 | **F2P (+294%)** |

### Analysis

**Paid wins in the conservative scenario** because:
- Even with 10x fewer downloads, guaranteed $2.54/user beats low F2P conversion
- No need to convince players to spend after download
- Simpler monetization (no IAP/ad integration needed)

**F2P wins in moderate/optimistic scenarios** because:
- Volume advantage compounds with payer conversion
- Whales can spend $50-200+ (impossible in paid model)
- Word-of-mouth spreads faster with free downloads

### Hybrid Option: $2.99 + Light IAP

Some paid games add cosmetic IAP for additional revenue:
- Base revenue: $2.54/download
- Optional IAP (decorations, time-skips): adds ~$0.50-1.00 ARPU
- Moderate scenario: 1,500 × $3.25 = **$4,875**

### Recommendation by Risk Profile

| If You Are... | Choose | Why |
|---------------|--------|-----|
| **Risk-averse** | $2.99 Paid | Guaranteed revenue per download, simpler implementation |
| **Growth-focused** | F2P | Higher ceiling, better discoverability |
| **Testing market** | F2P | More data from larger user base |
| **Time-constrained** | $2.99 Paid | Skip IAP/ad implementation |

---

## Key Takeaways

1. **F2P has higher ceiling** but requires monetization optimization
2. **Paid ($2.99) is safer** for conservative expectations
3. **Realistic 6-month revenue**: $800-$6,400 (F2P) or $1,270-$3,810 (Paid)
4. **Operational costs are low**: $200-800/year out-of-pocket
5. **Paid UA is not viable initially**: LTV must improve first
6. **Big-budget games operate differently**: Don't try to compete on their terms
7. **Focus on organic growth**: Community, content, and word-of-mouth
8. **Reinvest early revenue into ASO and micro-influencers**, not paid ads

---

*Analysis prepared for Event Planning Simulator - January 2026*
