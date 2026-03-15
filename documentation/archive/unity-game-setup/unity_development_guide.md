# Unity Development Guide - Event Planning Simulator

This guide explains how to set up Unity and work with Kiro/Claude to build the Event Planning Simulator.

---

## Table of Contents

1. [How the Tools Work Together](#how-the-tools-work-together)
2. [One-Time Setup](#one-time-setup)
3. [Daily Development Workflow](#daily-development-workflow)
4. [Testing Your App](#testing-your-app)
5. [What Each Tool Does](#what-each-tool-does)
6. [Troubleshooting](#troubleshooting)

---

## How the Tools Work Together

```
┌─────────────────────────────────────────────────────────────────┐
│                     YOUR WORKFLOW                                │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│   ┌──────────┐      ┌──────────┐      ┌──────────────┐         │
│   │  Kiro/   │ ───► │  VS Code │ ───► │  Unity Hub   │         │
│   │  Claude  │      │  (Edit)  │      │  (Run/Test)  │         │
│   └──────────┘      └──────────┘      └──────────────┘         │
│        │                 │                   │                  │
│        │                 ▼                   │                  │
│        │           ┌──────────┐              │                  │
│        └─────────► │   Git    │ ◄────────────┘                  │
│                    │  (Save)  │                                 │
│                    └──────────┘                                 │
│                         │                                       │
│                         ▼                                       │
│                  /Users/d/github/                               │
│                    events_app/                                  │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

### Key Point

**Unity Hub and Kiro/VS Code both look at the SAME folder** (`/Users/d/github/events_app/`)

- **Kiro/Claude** writes `.cs` files (C# code) to the folder
- **Unity Hub** sees those files and compiles/runs them
- **Git** tracks all changes

---

## One-Time Setup

### Step 1: Download Unity Hub

1. Go to https://unity.com/download
2. Download Unity Hub for Mac
3. Install and open Unity Hub

### Step 2: Create Unity Account (Free)

1. Unity Hub will prompt you to sign in
2. Create account with email + password
3. Activate **Unity Personal** license (free)

| License | Cost | For You? |
|---------|------|----------|
| **Personal** | Free | Yes - use this |
| Plus | $399/year | No |
| Pro | $2,040/year | No |

*Personal is free if your revenue/funding is under $100K/year*

### Step 3: Install Unity 6

In Unity Hub:
1. Go to **Installs** tab
2. Click **Install Editor**
3. Select **Unity 6** (latest LTS version)
4. Check these modules:
   - ✅ **Android Build Support**
   - ✅ **iOS Build Support**
   - ✅ **Visual Studio Code Editor** (or your preferred IDE)
5. Click Install (this takes a while - ~5-10GB)

### Step 4: Open the Project

In Unity Hub:
1. Go to **Projects** tab
2. Click **Open**
3. Navigate to `/Users/d/github/events_app/`
4. Select the folder
5. Unity will create project files automatically

---

## Daily Development Workflow

### Your Two Windows

You'll have two applications open side by side:

```
┌─────────────────────────────────┬─────────────────────────────────┐
│          VS Code + Kiro         │          Unity Hub              │
│    (where you talk to Claude)   │    (where you run the game)     │
└─────────────────────────────────┴─────────────────────────────────┘
```

### Typical Session

#### 1. Start Your Session
```
Open VS Code → Open events_app folder
Open Unity Hub → Open events_app project
```

#### 2. Ask Claude to Write Code
```
You: "Implement Task 1.2 - GameDate struct"
Claude: *writes GameDate.cs file to Assets/Scripts/Data/*
```

#### 3. Unity Auto-Detects Changes
- Unity sees new `.cs` files automatically
- Console window shows any errors
- If errors appear, copy them back to Claude

#### 4. Test in Unity
- Click **Play ▶** button to run the game
- Game runs in the editor window
- Click Play again to stop

#### 5. Fix Issues
```
You: "Unity shows this error: NullReferenceException at line 42"
Claude: *fixes the code*
Unity: *auto-recompiles*
```

#### 6. Save Progress
```
You: "Commit the changes"
Claude: *runs git commit*
```

---

## Testing Your App

### Level 1: Unity Editor (Easiest)
- Just click **Play ▶** in Unity
- Game runs in a window on your Mac
- Good for testing logic and flow

### Level 2: Unity Remote (Quick Device Test)
- Download "Unity Remote" app on your phone (free)
- Connect phone via USB
- Game streams to your phone
- Good for testing touch controls

### Level 3: Build to Device

#### Android (Easier)
1. Enable Developer Mode on Android phone
2. Connect via USB
3. In Unity: File → Build Settings → Android → Build and Run
4. App installs on phone

#### iOS (Requires More Setup)
1. Install Xcode from Mac App Store (free)
2. In Unity: File → Build Settings → iOS → Build
3. Open generated Xcode project
4. Run on Simulator (free) or device ($99/year Apple Developer)

### Testing Checklist

| What to Test | Where | Why |
|--------------|-------|-----|
| Game logic works | Unity Editor | Quick iteration |
| Touch controls feel right | Unity Remote / Device | Can't test touch on computer |
| Performance is smooth | Device Build | Editor is slower than real device |
| Save/Load works | Device Build | Tests real file system |
| Notifications work | Device Build | Requires real OS |

---

## What Each Tool Does

### Kiro/Claude

| Can Do | Cannot Do |
|--------|-----------|
| ✅ Write all C# code files | ❌ Click buttons in Unity |
| ✅ Create folder structure | ❌ Run the game for you |
| ✅ Edit scripts when you report errors | ❌ See the Unity visual editor |
| ✅ Help debug issues | ❌ Create visual assets (sprites, UI) |
| ✅ Explain Unity concepts | ❌ Configure Unity project settings visually |

### VS Code

| What It Does |
|--------------|
| View code files Claude wrote |
| Make small manual edits |
| See file structure |
| Run Kiro/Claude extension |

### Unity Hub

| What It Does |
|--------------|
| Compile C# code |
| Run the game (Play button) |
| Build app for phone |
| Visual scene editor |
| Import assets (images, sounds) |
| Project settings |

### Git

| What It Does |
|--------------|
| Save snapshots of your code |
| Undo mistakes (go back in time) |
| Backup to GitHub |

---

## Project Structure

After setup, your folder will look like this:

```
/Users/d/github/events_app/
├── Assets/                     # Unity looks here
│   ├── Scripts/
│   │   ├── Core/              # Pure C# logic
│   │   ├── Data/              # Data structures
│   │   ├── Managers/          # MonoBehaviour managers
│   │   ├── Systems/           # Game systems
│   │   ├── UI/                # UI controllers
│   │   └── Tests/             # Unit tests
│   ├── ScriptableObjects/     # Data files
│   ├── Scenes/                # Game scenes
│   └── Prefabs/               # Reusable objects
│
├── Packages/                   # Unity packages (auto-managed)
├── ProjectSettings/            # Unity settings (auto-managed)
│
├── documentation/              # Your guides
│   └── unity_development_guide.md
│
└── .kiro/specs/               # Specifications
    └── event-planner-simulator/
        ├── requirements.md
        ├── design.md
        ├── tasks.md
        ├── business_analysis.md
        └── session_notes.md
```

---

## Troubleshooting

### "Unity doesn't see my new script"

1. Make sure the file is in `Assets/Scripts/` folder
2. Make sure file ends with `.cs`
3. In Unity, right-click in Project window → Refresh
4. Check Console for compile errors

### "Script has errors"

1. Copy the exact error message from Unity Console
2. Paste it to Claude: "Fix this error: [paste error]"
3. Wait for Unity to recompile after Claude fixes it

### "Game won't play"

1. Check Unity Console (red errors block Play)
2. Fix all red errors first
3. Yellow warnings are okay

### "Can't build for iOS"

1. Make sure Xcode is installed (Mac App Store, free)
2. Make sure iOS Build Support module is installed in Unity Hub
3. For real device: need Apple Developer account ($99/year)

### "Can't build for Android"

1. In Unity Hub → Installs → Your Unity Version → Add Modules
2. Add: Android Build Support, Android SDK, Android NDK
3. Enable Developer Mode on your phone
4. Connect via USB and trust the computer

---

## Quick Reference

### Unity Shortcuts (Mac)

| Action | Shortcut |
|--------|----------|
| Play/Stop | Cmd + P |
| Pause | Cmd + Shift + P |
| Save Scene | Cmd + S |
| Build | Cmd + B |

### Common Unity Windows

| Window | What It Shows |
|--------|---------------|
| **Scene** | Visual editor for placing objects |
| **Game** | What player sees when playing |
| **Hierarchy** | List of objects in scene |
| **Project** | All your files |
| **Inspector** | Properties of selected object |
| **Console** | Errors and debug messages |

### Useful Commands to Ask Claude

```
"Start Task [X.X] from tasks.md"
"Unity shows this error: [paste error]"
"Explain what [concept] does in Unity"
"Run the tests"
"Commit my changes"
"What task should I work on next?"
```

---

## Next Steps

1. ✅ Read this guide
2. ⬜ Download and install Unity Hub
3. ⬜ Create Unity account (free)
4. ⬜ Install Unity 6 with iOS/Android support
5. ⬜ Open the events_app project in Unity
6. ⬜ Ask Claude to start Task 1.1

---

## Unity Tasks by Development Phase

This section lists exactly what YOU need to do in Unity at each phase. Claude handles the code; you handle these Unity tasks.

### Phase 1: Project Setup (Task 1)

**Claude Does:**
- Creates folder structure
- Writes .gitignore
- Creates Assembly Definitions

**You Do in Unity:**
| Step | How To Do It |
|------|--------------|
| Open project | Unity Hub → Open → select events_app folder |
| Verify folders appear | Project window should show Assets/Scripts/* |
| Create main scene | File → New Scene → Save as "Game.unity" in Assets/Scenes/ |

---

### Phase 2: Core Systems (Tasks 2-14)

**Claude Does:**
- Writes all C# scripts (data models, systems, calculators)
- Writes unit tests

**You Do in Unity:**
| Step | How To Do It |
|------|--------------|
| Run tests | Window → General → Test Runner → Run All |
| Check for errors | Console window (red = error, yellow = warning) |
| Report errors to Claude | Copy error text, paste to Claude |

*Most of Phase 2 is code-only - minimal Unity work!*

---

### Phase 3: Scene Setup (After Task 14 Checkpoint)

**Claude Does:**
- Writes UI controller scripts
- Writes GameManager

**You Do in Unity:**

#### Create the Main Scene Structure
| Step | How To Do It |
|------|--------------|
| Create Canvas | Hierarchy → Right-click → UI → Canvas |
| Set Canvas to Screen Space | Inspector → Render Mode: Screen Space - Overlay |
| Set Canvas Scaler | Inspector → UI Scale Mode: Scale With Screen Size |
| Set Reference Resolution | 1080 x 1920 (portrait mobile) |

#### Create GameManager Object
| Step | How To Do It |
|------|--------------|
| Create empty GameObject | Hierarchy → Right-click → Create Empty |
| Rename to "GameManager" | Select it, press Enter, type name |
| Attach script | Drag GameManager.cs from Project → onto GameManager object |

#### Create Phone UI (Main Interface)
| Step | How To Do It |
|------|--------------|
| Create Panel | Right-click Canvas → UI → Panel |
| Rename to "PhoneScreen" | |
| Set anchors to stretch | Inspector → Rect Transform → Anchor presets (Alt+click stretch) |
| Add child panels | Apps grid, notification bar, etc. |

---

### Phase 4: UI Layout (Tasks 15-21)

**Claude Does:**
- Writes PhoneSystem, MapSystem, TutorialSystem scripts
- Writes UI controller scripts for each screen

**You Do in Unity:**

#### Map Screen Setup
| Step | How To Do It |
|------|--------------|
| Create Map panel | Under Canvas, UI → Panel, name "MapScreen" |
| Add map background | UI → Raw Image, assign map texture |
| Create pin prefab | Create UI button, add icon, save as Prefab |
| Attach MapController script | Drag script onto MapScreen panel |

#### Phone App Icons
| Step | How To Do It |
|------|--------------|
| Create app grid | UI → Panel with Grid Layout Group |
| Create app button prefab | Button with icon + label, save as Prefab |
| Add 7 app buttons | Calendar, Messages, Bank, Contacts, Reviews, Tasks, Clients |
| Wire OnClick events | Inspector → Button → OnClick → Add, drag controller, select method |

#### Common UI Elements
| Step | How To Do It |
|------|--------------|
| Create button prefab | UI → Button, style it, save to Prefabs/UI/ |
| Create popup prefab | Panel with title, message, buttons |
| Create loading spinner | Animated UI element |

---

### Phase 5: ScriptableObject Data (Tasks 2.4, 2.11, 22)

**Claude Does:**
- Writes ScriptableObject class definitions
- Can write editor scripts to help create instances

**You Do in Unity:**

#### Create Venue Data
| Step | How To Do It |
|------|--------------|
| Create folder | Assets/ScriptableObjects/Venues/ |
| Create venue | Right-click → Create → EventPlanner → Venue |
| Fill in data | Inspector: name, capacity, price, zone, etc. |
| Repeat | Create 10-15 venues for each zone |

#### Create Vendor Data
| Step | How To Do It |
|------|--------------|
| Create folder | Assets/ScriptableObjects/Vendors/ |
| Create vendor | Right-click → Create → EventPlanner → Vendor |
| Fill in data | Inspector: name, category, tier, price, ratings |
| Repeat | Create 5-10 vendors per category |

#### Create Event Types
| Step | How To Do It |
|------|--------------|
| Create folder | Assets/ScriptableObjects/EventTypes/ |
| Create event type | Right-click → Create → EventPlanner → EventType |
| Fill in data | Inspector: name, complexity, budget range, subcategories |

---

### Phase 6: Audio & Visual Polish (Task 18)

**Claude Does:**
- Writes AudioManager script

**You Do in Unity:**

#### Import Audio Files
| Step | How To Do It |
|------|--------------|
| Get audio files | Purchase from Asset Store or use free sounds |
| Drag into project | Assets/Audio/Music/ and Assets/Audio/SFX/ |
| Configure import | Inspector → Load Type: Compressed in Memory (music) or Decompress on Load (SFX) |

#### Set Up Audio Sources
| Step | How To Do It |
|------|--------------|
| Add to GameManager | Add AudioSource component |
| Configure for music | Loop: true, Play On Awake: false |
| Add second AudioSource | For SFX (or use pooling) |

---

### Phase 7: Build Configuration (Task 28+)

**Claude Does:**
- Writes any platform-specific code

**You Do in Unity:**

#### Project Settings
| Setting | Location | Value |
|---------|----------|-------|
| Company Name | Edit → Project Settings → Player | Your name |
| Product Name | Player Settings | "Event Planning Simulator" |
| Bundle ID | Player Settings | com.yourname.eventplanner |
| Default Orientation | Player Settings → Resolution | Portrait |
| App Icon | Player Settings → Icon | Your icon images |

#### Build Settings
| Step | How To Do It |
|------|--------------|
| Open Build Settings | File → Build Settings |
| Add scenes | Drag scenes from Project to "Scenes in Build" |
| Order scenes | Loading scene = 0, Game scene = 1 |
| Switch platform | Select iOS or Android, click "Switch Platform" |

#### Android Specific
| Setting | Value |
|---------|-------|
| Minimum API Level | Android 7.0 (API 24) |
| Target API Level | Latest |
| Scripting Backend | IL2CPP |

#### iOS Specific
| Setting | Value |
|---------|-------|
| Target iOS Version | 13.0+ |
| Signing Team ID | Your Apple Developer ID |

---

### Phase 8: Monetization Setup (Tasks 22-23)

**Claude Does:**
- Writes IMonetizationSystem implementation
- Writes IUnityServicesManager implementation

**You Do in Unity:**

#### Unity Ads Setup
| Step | How To Do It |
|------|--------------|
| Open Services | Window → General → Services |
| Create/link project | Click "Create" or link to Unity Dashboard project |
| Enable Ads | Services → Ads → Enable |
| Get Ad Unit IDs | Unity Dashboard → Monetization → Ad Units |

#### Unity IAP Setup
| Step | How To Do It |
|------|--------------|
| Install IAP package | Window → Package Manager → Unity IAP → Install |
| Enable IAP | Services → In-App Purchasing → Enable |
| Configure products | Services → IAP Catalog → Add products |

#### Unity Analytics Setup
| Step | How To Do It |
|------|--------------|
| Enable Analytics | Services → Analytics → Enable |
| Configure events | Unity Dashboard → Analytics → Custom Events |

---

## Unity Work Summary by Task Group

| Tasks | Unity Work | Effort |
|-------|------------|--------|
| 1 (Setup) | Open project, create scene | 🟢 Low |
| 2-3 (Data Models) | Run tests only | 🟢 Low |
| 4-7 (Core Systems) | Run tests only | 🟢 Low |
| 8-10 (Event Planning) | Run tests only | 🟢 Low |
| 11-14 (Game Systems) | Run tests only | 🟢 Low |
| 15 (Map) | Create map UI, pins | 🟡 Medium |
| 16 (Phone) | Create phone UI, app grid | 🟡 Medium |
| 17 (Tutorial) | Create tutorial overlays | 🟡 Medium |
| 18 (Audio) | Import audio, configure | 🟡 Medium |
| 19-20 (Notifications/Achievements) | Minimal | 🟢 Low |
| 21 (UI Checkpoint) | Wire all UI elements | 🟠 Higher |
| 22-23 (Monetization) | Unity Services setup | 🟡 Medium |
| 24-27 (Features) | Run tests only | 🟢 Low |
| 28 (GameManager) | Attach to scene | 🟢 Low |
| 29-30 (Milestone/Extensions) | Minimal | 🟢 Low |
| 31-32 (Final) | Full testing, build | 🟠 Higher |

---

## Tips for Unity Tasks

### UI Layout Tips
- Use **anchors** so UI scales on different phones
- Test with Game view aspect ratios (Free Aspect → select device)
- Use **Layout Groups** for automatic spacing

### Performance Tips
- Mark static objects as "Static" in Inspector
- Use Sprite Atlases for UI images
- Set audio compression appropriately

### When You're Stuck
1. Google the error message
2. Check Unity Manual/Documentation
3. Ask Claude to explain the concept
4. Check YouTube tutorials for visual walkthroughs

---

*Guide created: January 6, 2026*
