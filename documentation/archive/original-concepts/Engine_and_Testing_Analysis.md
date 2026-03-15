# Engine Selection & Testing Strategy Analysis

## Final Decision: Unity (C#)

**Decision Date:** January 2025

After thorough analysis comparing Godot and Unity, **Unity has been selected** as the engine for Event Planning Simulator based on:

1. **Modern UI Requirements** — The game needs polished, "juicy" mobile UI to compete with games like Magic Sort and Pizza Ready. Unity's Asset Store has extensive professional UI kits.
2. **Target Market** — Casual mobile simulation games are dominated by Unity. The visual polish standard is set by Unity games.
3. **AI-Assisted Development** — C# has significantly more training data than GDScript, improving AI code generation quality.
4. **Monetization** — Built-in Unity IAP and Unity Ads support simplifies implementation.
5. **Industry Validation** — Similar successful games (Two Point Hospital, Cities: Skylines, Pizza Ready) use Unity.

---

## 1. Engine Comparison (For Reference)

### Overview Table

| Factor | **Godot 4.x** | **Unity** | **Winner for This Project** |
|--------|---------------|-----------|----------------------------|
| **Cost** | 100% free, MIT license | Free under $200K revenue | Godot |
| **Mobile Export** | Good, improving | Mature, battle-tested | **Unity** |
| **2D UI Systems** | Excellent Control nodes | Canvas + Asset Store kits | **Unity** (ecosystem) |
| **Modern UI Polish** | Limited assets/examples | Extensive UI kits available | **Unity** |
| **Learning Curve** | GDScript is easier | C# more verbose | Godot |
| **Asset Store** | Smaller ecosystem | Massive marketplace | **Unity** |
| **AI Code Generation** | Less training data | More C# examples | **Unity** |
| **Monetization** | Manual integration | Built-in IAP/Ads | **Unity** |
| **Community/Hiring** | Growing | Much larger | **Unity** |
| **Successful Mobile Sims** | Few examples | Many examples | **Unity** |

### Why Not Godot?

Godot is an excellent engine, but for this specific project:

1. **UI Polish Gap** — Most Godot games have a "programmer art" or basic UI feel. The casual mobile market requires highly polished, animated UI.
2. **Fewer Mobile Sim Examples** — Hard to find successful Godot tycoon/simulation games on iOS App Store to reference.
3. **Asset Ecosystem** — Would need to build UI systems from scratch rather than leveraging proven kits.
4. **AI Training Data** — Claude and other AI assistants have less GDScript training data than C#.

### Unity Licensing Notes

- **Revenue Cap**: Free (Personal) tier allows up to **$200,000 USD** in trailing 12-month revenue
- **Splash Screen**: Optional as of Unity 6 (no longer required for Personal tier)
- **No Runtime Fees**: Unity reversed the controversial 2023 runtime fee policy

Sources: [Unity Pricing Updates](https://unity.com/products/pricing-updates) | [Unity License Compliance](https://unity.com/pages/license-compliance)

---

## 2. AI-Assisted Development Strategy

### Why AI-Assisted Development Works for This Project

Grand Games (Magic Sort) uses AI for ~30% of their codebase and AI-generated art concepts, validating this approach for casual mobile games.

### Claude Code + Unity Considerations

| Aspect | Approach |
|--------|----------|
| **Code Generation** | C# has extensive training data — high quality output |
| **Scene Files** | Unity scenes are YAML-based — readable but complex |
| **Architecture** | Keep logic in pure C# classes for better AI comprehension |
| **Prefabs** | Describe structure clearly; AI can generate scripts but not prefab configs |
| **UI** | AI excels at generating UI controller scripts; layout done in Unity Editor |

### Best Practices for AI-Assisted Unity Development

1. **Separate Logic from MonoBehaviour** — Pure C# classes are easier for AI to reason about
2. **Use ScriptableObjects for Data** — AI can generate these effectively
3. **Document Intent** — Clear comments help AI understand context
4. **Small, Focused Scripts** — Easier for AI to generate and modify
5. **Consistent Naming** — AI learns patterns from your codebase

---

## 3. Testing Strategies During Development

For a simulation game with interconnected systems (satisfaction calculation, consequences, progression), comprehensive testing is critical to ensure balance and prevent game-breaking bugs.

### 3.1 Architecture for Testability

Structure the codebase to separate **game logic** from **engine/UI code**:

```
project/
├── core/                    # Pure logic, no engine dependencies
│   ├── satisfaction_calculator
│   ├── consequence_system
│   ├── progression_manager
│   ├── economy_calculator
│   └── random_event_resolver
├── data/                    # Data structures
│   ├── client_data
│   ├── event_data
│   ├── vendor_data
│   └── venue_data
├── managers/                # Engine-dependent managers
│   ├── game_manager
│   ├── save_manager
│   └── audio_manager
└── ui/                      # All UI/presentation
    ├── map/
    ├── phone/
    └── events/
```

This separation allows core systems to be tested without running the full game.

### 3.2 Testing Approaches

#### A. Unit Testing (Automated)

Test core calculations independently:

```
# Satisfaction Calculation Tests
test_perfect_event_scores_100()
test_underspent_catering_reduces_food_score()
test_perfectionist_client_has_higher_threshold()
test_weather_event_impacts_outdoor_venue()

# Progression Tests
test_stage1_to_stage2_requires_rep_and_money()
test_unlocking_downtown_zone_at_stage2()

# Economy Tests
test_profit_calculation_with_vendor_discounts()
test_budget_allocation_percentages_sum_to_100()
```

**Unity**: Use Unity Test Framework (NUnit-based)

#### B. Debug/Cheat Panel (In-Game)

Build a developer console accessible during development (hidden in release builds):

| Command | Function |
|---------|----------|
| `set_money <amount>` | Set player money |
| `set_reputation <amount>` | Set reputation level |
| `set_stage <1-5>` | Jump to progression stage |
| `force_event <type>` | Trigger specific random event |
| `complete_event` | Instantly complete current event |
| `set_satisfaction <0-100>` | Override satisfaction score |
| `spawn_client <personality>` | Create client with specific personality |
| `unlock_all` | Unlock all zones/vendors/venues |
| `reset_save` | Clear save data |

#### C. Simulation Mode (Balance Testing)

Run automated "playthroughs" to validate game balance:

```
function run_simulation(num_events: int, strategy: Strategy):
    results = []
    for i in range(num_events):
        event = generate_random_event()
        choices = strategy.make_choices(event)
        outcome = simulate_event(event, choices)
        results.append(outcome)

    return analyze_results(results)
    # → Average profit, failure rate, time to stage 2, etc.
```

Questions simulation testing can answer:
- How many events to reach Stage 2 on average?
- What's the failure rate for budget-tier vendors?
- Is any vendor combination strictly dominant?
- Can a player go bankrupt? Under what conditions?

#### D. Save State Snapshots

Create preset save files for testing specific scenarios:

| Save File | Scenario |
|-----------|----------|
| `debug_tutorial_complete.json` | Just finished tutorial |
| `debug_stage1_broke.json` | Stage 1, very low money |
| `debug_stage1_rich.json` | Stage 1, high money |
| `debug_stage2_start.json` | Just unlocked Stage 2 |
| `debug_demanding_client.json` | Mid-event with demanding client |
| `debug_multiple_events.json` | 3 simultaneous events (Stage 3+) |

Load these instantly to test specific situations without replaying.

#### E. Visual/Manual Testing Checklist

For UI and feel, automated tests aren't enough:

**Map Testing**
- [ ] All location pins appear in correct positions
- [ ] Zone transitions animate smoothly
- [ ] Locked locations show lock indicator
- [ ] Preview cards display correct data
- [ ] Filtering works for all categories

**Phone Testing**
- [ ] All apps open/close correctly
- [ ] Notification badges update
- [ ] Calendar shows correct events
- [ ] Messages thread correctly
- [ ] Bank balance updates in real-time

**Event Flow Testing**
- [ ] Client intake shows all required info
- [ ] Budget allocation UI prevents overspending
- [ ] Vendor booking updates availability
- [ ] Task list updates on completion
- [ ] Results screen shows accurate breakdown

### 3.3 Testing Phases by Development Stage

| Dev Phase | Focus | Testing Method |
|-----------|-------|----------------|
| **Prototype** | Core loop feels good | Manual playtesting |
| **Alpha** | All systems functional | Unit tests + debug panel |
| **Beta** | Balance and polish | Simulation + user playtesting |
| **Release** | Edge cases, crashes | Full regression + device testing |

### 3.4 Mobile-Specific Testing

#### Device Testing Matrix

| Category | Devices to Test |
|----------|-----------------|
| **Low-end Android** | 2GB RAM, older GPU |
| **Mid-range Android** | 4GB RAM, recent budget phone |
| **High-end Android** | Flagship device |
| **Older iPhone** | iPhone 8 / SE |
| **Current iPhone** | iPhone 12+ |
| **Tablets** | iPad, Android tablet |

#### Mobile Testing Checklist

- [ ] Touch targets are at least 44x44 points
- [ ] UI scales correctly on different aspect ratios
- [ ] Game handles interruptions (calls, notifications)
- [ ] Save system handles app being killed
- [ ] Performance maintains 60fps on target devices
- [ ] Battery usage is reasonable
- [ ] Works offline (if applicable)

### 3.5 Recommended Testing Tools (Unity)

- **Unity Test Framework**: Unit/integration tests (NUnit-based)
- **Unity Remote**: Quick mobile preview on device
- **Unity Profiler**: Performance analysis
- **Unity Cloud Build**: Automated builds for iOS/Android
- **Firebase Test Lab**: Automated device testing across many devices
- **Unity Game Simulation**: AI-driven playtesting (if available)

---

## 4. Next Steps

1. **Set up Unity project** with recommended folder structure
2. **Choose UI kit** from Asset Store for modern mobile look
3. **Implement debug panel early** — it will save significant time
4. **Create first unit tests** for satisfaction calculation (the core system)
5. **Build save state system** with snapshot support from the start
6. **Reference the unified GDD** (Event_Planner_Unity_GDD.md) for implementation details
