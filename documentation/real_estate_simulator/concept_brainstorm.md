# Real Estate Agent Simulator - Concept Brainstorm

## Game Vision

**"The Gran Turismo of Real Estate"** - A mobile simulation game that's both entertaining AND genuinely teaches players what it's like to be a real estate agent. Built with 10+ years of real industry experience.

---

## Core Gameplay Loop (Mobile-Optimized)

### Single Session (5-15 minutes)
```
Start Day
    ↓
Check Messages/Leads (1-2 min)
    ↓
Choose Activity:
├── Show property to buyer
├── Meet with seller for listing
├── Host open house
├── Follow up with leads
├── Handle transaction task
└── Prospect for new business
    ↓
Complete Activity (3-8 min)
    ↓
See Results (income, reputation, progress)
    ↓
End Session or Continue
```

---

## Career Progression (Stages)

### Stage 1: New Agent (Months 1-6)
**Theme**: Survival and learning the basics

| Aspect | Details |
|--------|---------|
| **Income** | Commission only, no salary |
| **Leads** | Floor duty, open houses, sphere of influence |
| **Clients** | Mostly first-time buyers, smaller transactions |
| **Challenges** | Pay for licensing, desk fees, marketing |
| **Skills** | Basic showing, simple negotiations |
| **Unlock at** | Game start |

### Stage 2: Established Agent (Year 1-2)
**Theme**: Building reputation and pipeline

| Aspect | Details |
|--------|---------|
| **Income** | Steadier commission flow |
| **Leads** | Referrals start coming, past client business |
| **Clients** | Mix of buyers and sellers |
| **Challenges** | Time management, multiple transactions |
| **Skills** | Listing presentations, CMAs, negotiation tactics |
| **Unlock at** | 10 closed transactions |

### Stage 3: Top Producer (Year 3+)
**Theme**: Scaling and efficiency

| Aspect | Details |
|--------|---------|
| **Income** | Consistent high income |
| **Leads** | Reputation generates leads, premium clients |
| **Clients** | Investors, luxury, relocation |
| **Challenges** | Hire assistant?, team vs solo, work-life balance |
| **Skills** | Advanced negotiation, market analysis |
| **Unlock at** | 50 closed transactions, reputation milestone |

### Stage 4: Team Leader / Broker (Endgame)
**Theme**: Business building

| Aspect | Details |
|--------|---------|
| **Income** | Override from team + personal production |
| **Leads** | Lead generation systems, brand recognition |
| **Clients** | High-net-worth, commercial (intro) |
| **Challenges** | Recruiting, training, office management |
| **Skills** | Leadership, business planning |
| **Unlock at** | Revenue milestone + choice |

---

## Client Types & Personalities

### Buyer Types
| Type | Characteristics | Challenge |
|------|----------------|-----------|
| **First-Time Buyer** | Nervous, needs education, emotional | Hand-holding, realistic expectations |
| **Move-Up Buyer** | Has to sell to buy, timeline pressure | Coordination, contingencies |
| **Investor** | Numbers-focused, quick decisions | Finding deals, ROI analysis |
| **Relocating Buyer** | Remote, time-crunched, trusting | Video tours, quick turnaround |
| **Luxury Buyer** | High expectations, privacy concerns | Discretion, white-glove service |
| **Downsizer** | Emotional attachment, overwhelmed | Patience, empathy, decluttering help |

### Seller Types
| Type | Characteristics | Challenge |
|------|----------------|-----------|
| **FSBO Convert** | Tried alone, skeptical of agents | Prove value, handle ego |
| **Motivated Seller** | Job loss, divorce, estate | Speed, sensitivity, realistic pricing |
| **Overpriced Seller** | Unrealistic expectations | Price reduction conversations |
| **Investor Seller** | Multiple properties, business-minded | Volume, efficiency |
| **Estate Sale** | Multiple decision-makers, emotional | Coordination, patience |
| **Luxury Seller** | High expectations, demanding | Premium marketing, exclusivity |

### Personality Modifiers
| Personality | Effect on Gameplay |
|-------------|-------------------|
| **Easy-Going** | Flexible, trusting, forgiving of mistakes |
| **Demanding** | High expectations, frequent check-ins required |
| **Indecisive** | Multiple showings, needs reassurance |
| **Analytical** | Wants data, CMAs, market reports |
| **Emotional** | Attachment to home, needs patience |
| **Distrustful** | Questions everything, needs proof |

---

## Transaction Lifecycle (Core Mechanic)

### Buyer Side
```
1. Lead Received
   ├── Source: Open house, referral, online, floor duty
   └── Action: Respond within time window (affects conversion)

2. Buyer Consultation
   ├── Discovery: Budget, timeline, needs vs wants
   └── Action: Pre-approval check, sign buyer agreement

3. Property Search
   ├── Show properties matching criteria
   └── Action: Schedule showings, provide feedback

4. Offer Preparation
   ├── Found "the one"
   └── Action: Write offer, explain terms, set strategy

5. Negotiation
   ├── Counter-offers, multiple offer situations
   └── Action: Advise client, present options

6. Under Contract
   ├── Inspection, appraisal, loan approval
   └── Action: Manage timeline, handle issues

7. Closing
   ├── Final walkthrough, signing
   └── Action: Attend closing, hand over keys
```

### Seller Side
```
1. Lead Received
   ├── Source: Referral, sign call, online inquiry
   └── Action: Schedule listing appointment

2. Listing Presentation
   ├── CMA, marketing plan, pricing strategy
   └── Action: Win the listing, set expectations

3. Pre-Market Prep
   ├── Photos, staging, repairs
   └── Action: Coordinate vendors, manage timeline

4. Active Marketing
   ├── Showings, open houses, feedback
   └── Action: Schedule, collect feedback, adjust

5. Offer Review
   ├── Single or multiple offers
   └── Action: Present offers, advise on strategy

6. Negotiation
   ├── Counter-offers, inspection requests
   └── Action: Negotiate terms, protect client

7. Under Contract → Closing
   ├── Appraisal, buyer contingencies
   └── Action: Monitor progress, problem-solve

8. Post-Closing
   ├── Referral request, review request
   └── Action: Stay in touch, ask for business
```

---

## Key Systems

### 1. Lead Generation System
| Source | Stage Available | Conversion Rate | Cost |
|--------|-----------------|-----------------|------|
| Sphere of Influence | Stage 1 | High | Free (time) |
| Open Houses | Stage 1 | Medium | Time + materials |
| Floor Duty | Stage 1 | Low | Time |
| Online Leads (Zillow, etc.) | Stage 2 | Low | $$ |
| Referrals | Stage 2+ | High | Referral fee |
| Past Clients | Stage 2+ | High | Free |
| Marketing/Ads | Stage 3+ | Medium | $$$ |

### 2. Time Management System
- **Daily hours**: 8-10 available (expandable with upgrades)
- **Activities cost time**: Showings (2hr), Listing appt (1.5hr), Open house (4hr)
- **Urgency matters**: Hot leads cool off, deadlines have consequences
- **Overtime**: Possible but affects satisfaction/burnout

### 3. Financial System
| Income | Expense |
|--------|---------|
| Commission (2.5-3% typical) | Desk fees (monthly) |
| Referral fees received | MLS dues |
| Bonuses (volume, company) | Marketing costs |
| | E&O insurance |
| | Continuing education |
| | Gas/mileage |

### 4. Reputation System
| Factor | Affects |
|--------|---------|
| Closed transactions | Referral likelihood |
| Client reviews | Lead quality |
| Time to respond | Conversion rate |
| Negotiation wins | Word of mouth |
| Failed transactions | Negative reputation |

### 5. Skill Tree (Progression)
| Category | Skills |
|----------|--------|
| **Sales** | Listing presentation, objection handling, closing techniques |
| **Negotiation** | Counter-offer strategy, multiple offer management, repair negotiations |
| **Marketing** | Social media, photography, staging advice, open house hosting |
| **Technical** | CMA accuracy, contract knowledge, financing understanding |
| **Relationship** | Follow-up systems, referral asking, client retention |

---

## Random Events / Challenges

### Negative Events
| Event | Consequence | Mitigation |
|-------|-------------|------------|
| **Inspection issues** | Buyer wants repairs/credits | Negotiation skill |
| **Low appraisal** | Deal at risk | Renegotiate or appeal |
| **Financing falls through** | Lost deal | Backup offers, pre-qual verification |
| **Client ghosts** | Wasted time | Better qualification |
| **Competing agent poaches** | Lost listing | Relationship building |
| **Market shift** | Prices drop, listings sit | Pricing strategy, communication |
| **Difficult showing** | Tenant won't cooperate | Coordination skills |

### Positive Events
| Event | Benefit |
|-------|---------|
| **Referral surprise** | New quality lead |
| **Multiple offers** | Higher sale price, happy seller |
| **Quick close** | Faster commission |
| **Media coverage** | Reputation boost |
| **Award/recognition** | Unlock new opportunities |

---

## Monetization Ideas

### F2P + IAP Model

| IAP Type | Example | Price |
|----------|---------|-------|
| **Time Skip** | Skip waiting periods | $0.99 |
| **Premium Leads** | Higher quality clients | $1.99 |
| **Office Upgrade** | Better desk, more prestige | $2.99 |
| **Marketing Boost** | More lead generation | $2.99 |
| **Premium Agent Pack** | Exclusive outfits, car | $4.99 |
| **VIP Subscription** | Ad-free, daily bonuses | $9.99/mo |

### Educational Angle
- Partner with real estate schools?
- "Recommended by agents" endorsement
- Study mode with real exam questions?

---

## Unique Features (Differentiators)

### 1. "Real Talk" Mode
- Toggle realistic scenarios on/off
- When ON: Deals fall through, clients are difficult, market changes
- When OFF: Easier, more arcade-like experience

### 2. Market Simulation
- Dynamic market conditions (buyer's market, seller's market)
- Interest rate changes affect buyers
- Seasonal patterns (spring rush, holiday slowdown)

### 3. Actual Learning Content
- Pop-up tips explaining real concepts
- "Did you know?" facts about real estate
- Optional deep-dive explanations

### 4. Scenario Mode
- "Handle this multiple offer situation"
- "Navigate this low appraisal"
- "Convert this FSBO"
- Bite-sized learning challenges

---

## Technical Architecture (Reuse from Event Planner)

Many systems from Event Planning Simulator can be adapted:

| Event Planner System | Real Estate Equivalent |
|---------------------|------------------------|
| ClientInquiry | LeadInquiry |
| EventData | TransactionData |
| VendorData | ServiceProviderData (inspectors, lenders, etc.) |
| VenueData | PropertyData |
| SatisfactionCalculator | ClientSatisfactionCalculator |
| ProgressionSystem | CareerProgressionSystem |
| TimeSystem | TimeSystem (same concept) |
| ReputationSystem | ReputationSystem (similar) |

---

## Domain Expert Insights (From 10+ Years Experience)

### 1. What surprised you most when you started in real estate?

**The license is just the bare minimum.** Getting licensed is the easy part - brokers will hire almost anyone because they just want bodies. But you're not getting a job, you're starting a business.

**Key Insights:**
- Brokers don't really interview - they just want agents who might produce
- You'll get about 3 "freebie" transactions from your sphere of influence (friends/family)
- After those 3, if you haven't built business systems, you're done
- Real estate is a BUSINESS, not a job - you need:
  - Marketing systems
  - Lead generation
  - Lead evaluation
  - Lead conversion
- **"What you don't build, you pay for"** - if you can't generate leads, you'll pay someone else for them

**Game Implication:** Stage 1 should feel like survival mode. Players get a few easy sphere-of-influence deals, then hit a wall if they haven't invested in systems.

---

### 2. What do agents get wrong that costs them deals?

**Poor follow-up is the #1 killer.** 70% of people don't use the same real estate agent again - not because agents are bad at transactions, but because they're forgettable.

**Key Insights:**
- Agents have horrible systems (or no systems at all)
- Most agents optimize for "right place, right time" instead of building relationships
- The majority of client experiences are forgettable or actively negative
- The part-time nature of many agents contributes to poor service
- Too many agents competing for too few clients
- Other mistakes fall under "poor client experience":
  - Pricing mistakes (too high, too low)
  - Not qualifying buyers properly
  - Lying or pressuring clients
  - Poor negotiation skills

**Game Implication:** Follow-up system should be a core mechanic. Players who don't follow up lose repeat business and referrals. The 70% stat could be shown: "Sarah bought a house 2 years ago but used a different agent - you never followed up."

---

### 3. What's the most satisfying part of closing a deal?

**Managing the transaction to meet a client's goals.** It's not about the commission check - it's about being part of major life moments.

**Key Insights:**
- Helping someone secure the place where their family will make memories
- Helping clients manage family wealth through investment property
- Helping someone become a landlord and meet financial goals
- Being part of selling the family home with all its memories and stories
- The fulfillment comes from genuinely helping people achieve meaningful goals

**Game Implication:** Rewards shouldn't just be "you earned $X commission" - they should highlight human impact: "The Martinez family just moved into their first home" or "The Johnsons hit their goal of owning 3 rental properties." Show the client's story, not just the numbers.

---

### 4. What's the most frustrating part that games could simulate?

**Dealing with people and personalities - it's a human business.** Most people think only clients are headaches, but EVERYONE in a transaction has a personality.

**Key Insights:**
- Managing the entire transaction ecosystem:
  - Lenders (slow, unresponsive)
  - Appraisers (come in low, nitpicky)
  - Title agents (lose paperwork)
  - Other real estate agents (don't return calls, incompetent)
- Dealing with incompetence from professionals who should know better
- The ethical tightrope: You must advise clients AND follow their orders, even when their orders go against the best outcome
- You're the one clients blame when things go wrong, even if it's not your fault

**Game Implication:** Random events should include "lender drops the ball" or "other agent is unresponsive." Add scenarios where player must advise against client's wishes but ultimately follow their decision.

---

### 5. What would you want new agents to learn before they start?

**Three key lessons:**

1. **Pay attention in licensing class** - the information is actually valid and useful
2. **It's a competitive business** - even the agents in your office are your competitors
3. **"You're only as good as your last transaction"** - service to clients is the most important thing

**Game Implication:** Tutorial should emphasize these realities. Maybe show "office colleagues" who are actually competing for the same leads. The reputation system should heavily weight recent transactions.

---

### 6. What separates top producers from average agents?

**Ranked in order of importance:**

1. **Work ethic + Systems** (the foundation)
2. **Follow-up execution** (enabled by good systems)
3. **Marketing spend** (amplifies everything else)
4. **Niche focus**
5. **Personality** (last - surprisingly)

**Key Insights:**
- Top producers pay attention to the tasks that matter
- They develop follow-up systems and execute relentlessly
- They always call people back, and they do it QUICKLY
- Confidence matters more than being "nice"
- Some of the top producers are not the nicest people
- It's a weird dynamic: sales requires confidence, not necessarily warmth

**Game Implication:** Skill tree should emphasize systems and follow-up over "charisma." A player could build a successful agent who's efficient and confident but not warm - and that should work if their systems are solid.

---

### 7. What does a "day in the life" really look like?

**The 70/30 Rule:** 70% planned, 30% reactive

**Typical Day Structure:**

| Time Block | Activity |
|------------|----------|
| **Morning** | Follow-up - combing through leads, nurturing conversations (largest time block) |
| **Mid-Morning** | Transaction tasks - following up with lenders, title, loose ends on under-contract deals |
| **Lunch** | Marketing lunch - eating with prospective client or business owner for mutual referrals |
| **Mid-Afternoon** | Confirm evening appointments, prepare showing materials, plan driving route, gather supplies |
| **Late Afternoon/Evening** | Active appointments - buyer consultations, listing presentations, property showings |

**Critical Rule:** "Your effort today shows up on your calendar in 3 months"

**If Calendar is Empty:**
- Engage in marketing activities
- Door knocking
- Networking events
- Lead generation activities

**Reactive Triggers:**
- Transaction team member drops the ball
- Urgent client/lead call that needs immediate response

**Game Implication:** The 3-month delay between effort and results is a key mechanic. Players should see cause-and-effect: "You did marketing 3 months ago → new lead appeared." Empty calendar = danger zone.

---

## The Transaction Puzzle (Financial & Preference Matching)

This is the "puzzle layer" that makes real estate intellectually challenging - fitting together financial reality with client dreams.

### Buyer Qualification Puzzle

Every buyer has a financial profile that determines what they can actually afford:

| Factor | What It Affects | Game Decision Point |
|--------|-----------------|---------------------|
| **Credit Score** | Loan eligibility, interest rate | Can they even get a loan? What rate? |
| **Debt-to-Income Ratio** | Maximum loan amount | How much house can they afford? |
| **Down Payment Cash** | Loan type options, PMI | What programs are they eligible for? |
| **Income Stability** | Loan approval risk | Will the lender approve them? |

### Loan Type Complexity

| Loan Type | Down Payment | Credit Min | Best For | Tradeoffs |
|-----------|--------------|------------|----------|-----------|
| **Conventional** | 5-20% | 620+ | Strong credit buyers | Higher down, stricter requirements |
| **FHA** | 3.5% | 580+ | First-time buyers | Lower down, but PMI for life |
| **VA** | 0% | None | Veterans | No down payment, but VA funding fee |
| **203(k)** | 3.5% | 640+ | Fixer-uppers | Rehab costs rolled in, complex process |
| **Down Payment Assistance** | Varies | Varies | Income-qualified | Free money, but restrictions |

**Game Mechanic:** Player must match buyer to right loan type. Wrong choice = deal falls through or unhappy client.

### The Affordability Calculation

```
Monthly Payment = Principal + Interest + Taxes + Insurance + PMI (if applicable)

Affordability Check:
├── Monthly Payment ≤ 28% of Gross Monthly Income (front-end ratio)
├── All Debt Payments ≤ 36-43% of Gross Monthly Income (back-end ratio)
└── Cash for: Down payment + Closing costs + Reserves
```

**Game Mechanic:** Show player the math. "Your buyer can afford $350K at current rates, but they want a $425K house. What do you do?"

### Preference vs. Budget Matching

| Buyer Wants | Budget Reality | Agent Decision |
|-------------|----------------|----------------|
| 4 bedrooms in nice area | Can only afford 3 bed or worse area | Educate on tradeoffs |
| Move-in ready | Budget only allows fixer-upper | Discuss 203(k) or adjust expectations |
| Specific school district | Prices 20% higher there | Find edge-of-district or reconsider |
| New construction | Budget fits only resale | Show value of existing homes |

**Game Mechanic:** Player must have "expectation conversations" with clients. Success depends on:
- Skill in presenting data (CMAs, market reports)
- Timing of conversation (too early = client leaves, too late = wasted showings)
- Client personality (analytical vs. emotional)

### Lender Selection Strategy

| Lender Type | Speed | Flexibility | Cost | Best For |
|-------------|-------|-------------|------|----------|
| **Big Bank** | Slow | Rigid | Average | Simple deals |
| **Credit Union** | Medium | Medium | Lower | Members |
| **Mortgage Broker** | Fast | High | Varies | Complex situations |
| **Online Lender** | Fast | Low | Lowest | Rate shoppers |
| **Local Lender** | Medium | High | Higher | Relationship, complex deals |

**Game Mechanic:** Player recommends lenders. Wrong match = delays, failed deals. Build relationships with reliable lenders over time.

### Why This Is Fun (The Puzzle Satisfaction)

**Making all the pieces fit:**
- Credit score + debt ratio + down payment + loan type + interest rate = what they can afford
- What they can afford + what they want + what's available = the right house
- The right house + inspection + appraisal + closing timeline = closed deal

**When it works:** Deeply satisfying - you solved a complex puzzle AND helped a family
**When it fails:** Usually because an agent dropped a ball somewhere in this chain

### Bad/Average Agent Failure Points

| Where They Drop Ball | Consequence |
|----------------------|-------------|
| Don't verify pre-approval | Buyer can't actually afford what they offered on |
| Wrong loan type recommended | Buyer gets worse rate or deal falls through |
| Don't calculate DTI properly | Buyer denied at underwriting |
| Skip affordability conversation | Waste time showing wrong houses |
| Don't understand down payment assistance | Buyer misses free money |
| Pick unreliable lender | Delays cause deal to fall apart |

**Game Implication:** These are all decision points where player skill matters. Tutorials should teach the math; gameplay should test the application.

---

## Decision Trees & Consequence System

Every decision in the transaction puzzle should have **cost/risk tradeoffs** that change how the game plays out. This isn't a linear path - player choices branch the experience.

### Example Decision Trees

#### Buyer Can't Afford Dream Home
```
Buyer wants $425K house, can only afford $350K
    │
    ├── Option A: Show $425K homes anyway
    │   ├── Risk: Waste time, client gets frustrated
    │   ├── Risk: Client makes offer, gets denied
    │   └── Possible: Client finds more down payment
    │
    ├── Option B: Have affordability conversation early
    │   ├── Risk: Client leaves for agent who "believes in them"
    │   ├── Benefit: Build trust through honesty
    │   └── Benefit: Save time, show right homes
    │
    └── Option C: Suggest creative solutions
        ├── 203(k) loan for fixer-upper?
        ├── Different neighborhood?
        ├── Wait 6 months to save more?
        └── Risk: Complex solutions can fall apart
```

#### Seller Wants to Overprice
```
Seller wants to list at $500K, market says $425K
    │
    ├── Option A: List at seller's price
    │   ├── Risk: Home sits, DOM grows, stigma
    │   ├── Risk: Price reductions hurt reputation
    │   └── Benefit: Keep the listing (short term)
    │
    ├── Option B: Refuse unless priced correctly
    │   ├── Risk: Lose listing entirely
    │   ├── Benefit: Protect your reputation
    │   └── Benefit: Avoid wasted marketing spend
    │
    └── Option C: Compromise with timeline
        ├── "Let's try $475K for 2 weeks, then reassess"
        ├── Risk: Still overpriced, may not work
        └── Benefit: Client feels heard
```

#### Lender Choice
```
Buyer needs lender recommendation
    │
    ├── Option A: Big Bank (your buyer's existing bank)
    │   ├── Pro: Client comfortable, existing relationship
    │   ├── Con: Slow, rigid, may delay closing
    │   └── Risk: If deal falls through, you look bad
    │
    ├── Option B: Your preferred local lender
    │   ├── Pro: Fast, flexible, you have relationship
    │   ├── Con: Slightly higher rate
    │   └── Benefit: They'll call you back, solve problems
    │
    └── Option C: Let buyer shop around
        ├── Pro: Buyer feels in control
        ├── Con: Unknown lender reliability
        └── Risk: Lender you can't influence
```

### Cost/Risk Tradeoff Examples

| Decision | Short-Term Benefit | Long-Term Risk |
|----------|-------------------|----------------|
| Take overpriced listing | Commission eventually | DOM hurts reputation, price cuts |
| Skip buyer qualification | More showings booked | Deals fall through at financing |
| Use cheap inspector | Client saves money | Issues missed, client blames you |
| Pressure hesitant buyer | Close faster | Bad review, no referral |
| Hide property issues | Easier sale | Legal liability, reputation damage |

---

## Public Reputation & Truth Obligation

Real estate agents are **public personas with legal and ethical obligations** to tell the truth. This creates unique gameplay dynamics.

### Why Reputation is Different in Real Estate

| Factor | Impact |
|--------|--------|
| **License is public record** | Anyone can verify you're licensed |
| **Reviews are everywhere** | Zillow, Google, Realtor.com, Yelp |
| **Transactions are public** | MLS history, county records |
| **You ARE your brand** | No hiding behind company name |
| **Word travels fast** | Agents talk, clients talk, everyone knows |

### Truth Obligations (Legal/Ethical)

| Situation | You MUST | You CAN'T |
|-----------|----------|-----------|
| Material defects | Disclose known issues | Hide problems you know about |
| Property condition | Represent accurately | Lie about square footage, etc. |
| Offers | Present all offers to seller | Hide offers to benefit yourself |
| Dual agency | Disclose and get consent | Secretly represent both sides |
| Pricing | Base CMAs on real data | Inflate values to win listings |

### Reputation Consequences in Game

**Positive Actions → Reputation Gains:**
- Honest affordability conversations (even if client leaves)
- Disclosing issues proactively
- Accurate pricing recommendations
- Following through on commitments
- Responsive communication

**Negative Actions → Reputation Damage:**
- Overpricing listings that sit
- Deals that fall through due to poor qualification
- Complaints to licensing board
- Bad reviews from clients
- Other agents warning people about you

### The Public Persona Mechanic

```
Your Reputation Score affects:
├── Lead quality (good reputation = better leads)
├── Referral rate (past clients recommend you)
├── Agent cooperation (other agents want to work with you)
├── Broker support (broker gives you better leads)
└── Client trust (clients follow your advice)

Reputation is VISIBLE:
├── Other agents can see your track record
├── Clients can read your reviews
├── Your DOM average is public
└── Your sales volume is public
```

### Ethical Dilemma Examples

| Scenario | Honest Choice | Tempting Choice |
|----------|---------------|-----------------|
| Client wants to hide defect | Disclose, lose listing | Stay quiet, risk license |
| Buyer loves house with major issues | Point out problems | Let them "discover" later |
| Seller asking if offers are coming | "Interest is low" | "I expect offers soon" |
| Client making bad financial decision | "I advise against this" | "It's your money" |

**Game Mechanic:** Each ethical choice affects short-term success vs. long-term reputation. Players who always take shortcuts eventually hit a wall when reputation tanks.

---

## Gameplay Insights from Domain Expertise

### Core Mechanics Validated

| Mechanic | Expert Validation |
|----------|-------------------|
| Follow-up system | **Critical** - #1 differentiator, 70% don't reuse agents |
| Systems/automation | **Critical** - what separates top producers |
| 3-month effort delay | **Confirmed** - effort today = calendar in 3 months |
| Transaction team management | **Important** - lenders, appraisers, title cause problems |
| Personality diversity | **Important** - not just clients, everyone has personalities |
| Reputation = recent work | **Confirmed** - "only as good as your last transaction" |

### New Mechanics to Add

1. **Sphere of Influence Depletion** - 3 "freebie" deals, then you must generate
2. **Transaction Team NPCs** - Lenders, appraisers, title agents with their own reliability ratings
3. **Ethical Dilemmas** - Advise client vs. follow bad orders
4. **Marketing Lunch System** - Relationship building with other professionals
5. **Office Competition** - Your "teammates" compete for same leads
6. **Client Story Payoffs** - Show emotional outcomes, not just commission
7. **Financial Puzzle System** - Credit score, DTI, down payment, loan type matching
8. **Affordability Calculator Mini-Game** - Player does the math to qualify buyers
9. **Expectation Conversation System** - Timing and skill in adjusting client dreams to budget
10. **Lender Relationship Building** - Reliable lenders as strategic assets
11. **Loan Type Matching** - FHA vs Conventional vs VA vs 203(k) decision tree
12. **Market Knowledge System** - Neighborhood pricing, school districts, local expertise
13. **Branching Decision Trees** - Every major decision has cost/risk tradeoffs that change gameplay
14. **Public Reputation System** - Visible stats (DOM, volume, reviews) that affect lead quality
15. **Ethical Dilemma System** - Short-term gain vs. long-term reputation choices
16. **Truth Obligation Mechanic** - Legal/ethical requirements that can't be violated without consequences

### What NOT to Over-Emphasize

- Personality/charisma (ranked last by expert)
- "Being nice" (confidence matters more)
- Single transaction focus (systems and follow-up matter more)

---

## Next Steps

1. [x] ~~Answer domain expert questions~~ - COMPLETED
2. [ ] Review insights and validate game mechanics
3. [ ] Prioritize: What's MVP vs nice-to-have?
4. [ ] Decide: Start fresh project or build alongside Event Planner?
5. [ ] Draft formal requirements document

---

*Brainstorm created: January 6, 2026*
*Domain expert interview completed: January 6, 2026*
