# Event Planning Simulator

A Unity 6 mobile business simulation game where players build an event planning career.

## Project Structure

```
Assets/
├── Art/              # Sprites, textures, UI assets
├── Audio/            # Music and sound effects
├── Fonts/            # Custom fonts
├── Plugins/          # Third-party plugins (Unity IAP, Ads, etc.)
├── Prefabs/          # Reusable game objects
├── Resources/        # Runtime-loaded assets
├── Scenes/           # Game scenes
├── ScriptableObjects/# Data assets (vendors, venues, events)
└── Scripts/
    ├── Core/         # Pure C# logic (no Unity deps, fully testable)
    ├── Data/         # Data structures, enums, ScriptableObjects
    ├── Managers/     # MonoBehaviour managers (GameManager, etc.)
    ├── Systems/      # Game system implementations
    ├── UI/           # UI controllers and views
    └── Tests/
        ├── EditMode/ # Unit tests (run without Play mode)
        └── PlayMode/ # Integration tests (require Play mode)
```

## Assembly Definitions

The project uses Assembly Definitions for:
- Clean dependency management
- Faster compilation (only recompile changed assemblies)
- Testability (Core has no Unity dependencies)

Dependency graph:
```
Data (base) ← Core ← Systems ← UI ← Managers
                 ↑       ↑
                 └───────┴─── Tests
```

## Requirements

- Unity 6 (6000.0 or later)
- Unity IAP package
- Unity Ads package
- Unity Analytics package

## Getting Started

1. Open the project in Unity 6
2. Install required packages via Package Manager
3. Open the main scene in Assets/Scenes/
4. Press Play to test

## Testing

Run tests via Window > General > Test Runner:
- EditMode: Pure logic tests (fast, no Play mode needed)
- PlayMode: Integration tests (require Play mode)
