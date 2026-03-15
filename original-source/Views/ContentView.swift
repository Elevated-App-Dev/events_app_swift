import SwiftUI

struct ContentView: View {
    @EnvironmentObject var gameManager: GameManager

    var body: some View {
        Group {
            switch gameManager.gameState {
            case .mainMenu:
                MainMenuView()
            case .playing, .tutorial:
                GameplayView()
            case .paused:
                GameplayView()
                    .overlay(PauseOverlay())
            case .results:
                GameplayView()
            case .settings:
                SettingsView()
            case .loading:
                LoadingView()
            }
        }
        .animation(.easeInOut(duration: 0.3), value: gameManager.gameState)
    }
}

// MARK: - Main Menu

struct MainMenuView: View {
    @EnvironmentObject var gameManager: GameManager

    var body: some View {
        VStack(spacing: 24) {
            Spacer()

            Text("Event Planner")
                .font(.system(size: 36, weight: .bold))
                .foregroundStyle(.primary)

            Text("Build Your Event Planning Empire")
                .font(.subheadline)
                .foregroundStyle(.secondary)

            Spacer()

            VStack(spacing: 16) {
                Button(action: { gameManager.startNewGame() }) {
                    Text("New Game")
                        .font(.headline)
                        .frame(maxWidth: .infinity)
                        .padding()
                        .background(Color.accentColor)
                        .foregroundColor(.white)
                        .cornerRadius(12)
                }

                Button(action: { gameManager.continueGame() }) {
                    Text("Continue")
                        .font(.headline)
                        .frame(maxWidth: .infinity)
                        .padding()
                        .background(Color.secondary.opacity(0.2))
                        .foregroundColor(.primary)
                        .cornerRadius(12)
                }

                Button(action: { gameManager.gameState = .settings }) {
                    Text("Settings")
                        .font(.headline)
                        .frame(maxWidth: .infinity)
                        .padding()
                        .background(Color.secondary.opacity(0.2))
                        .foregroundColor(.primary)
                        .cornerRadius(12)
                }
            }
            .padding(.horizontal, 40)

            Spacer()
        }
        .padding()
    }
}

// MARK: - Gameplay (placeholder)

struct GameplayView: View {
    @EnvironmentObject var gameManager: GameManager

    var body: some View {
        VStack {
            // HUD
            HStack {
                VStack(alignment: .leading) {
                    Text(gameManager.playerData.playerName)
                        .font(.headline)
                    Text("Stage: \(gameManager.playerData.stage.rawValue.capitalized)")
                        .font(.caption)
                        .foregroundStyle(.secondary)
                }
                Spacer()
                VStack(alignment: .trailing) {
                    Text("$\(gameManager.playerData.money, specifier: "%.0f")")
                        .font(.headline)
                    Text("Rep: \(gameManager.playerData.reputation)")
                        .font(.caption)
                        .foregroundStyle(.secondary)
                }
            }
            .padding()
            .background(.ultraThinMaterial)

            Spacer()

            Text("Gameplay Area")
                .foregroundStyle(.secondary)
            Text("(Systems being ported)")
                .font(.caption)
                .foregroundStyle(.tertiary)

            Spacer()

            // Bottom bar
            HStack(spacing: 32) {
                Button(action: {}) {
                    VStack {
                        Image(systemName: "calendar")
                        Text("Events").font(.caption2)
                    }
                }
                Button(action: {}) {
                    VStack {
                        Image(systemName: "map")
                        Text("Map").font(.caption2)
                    }
                }
                Button(action: {}) {
                    VStack {
                        Image(systemName: "iphone")
                        Text("Phone").font(.caption2)
                    }
                }
                Button(action: { gameManager.pauseGame() }) {
                    VStack {
                        Image(systemName: "pause.circle")
                        Text("Pause").font(.caption2)
                    }
                }
            }
            .padding()
            .background(.ultraThinMaterial)
        }
    }
}

// MARK: - Pause Overlay

struct PauseOverlay: View {
    @EnvironmentObject var gameManager: GameManager

    var body: some View {
        ZStack {
            Color.black.opacity(0.5)
                .ignoresSafeArea()

            VStack(spacing: 20) {
                Text("Paused")
                    .font(.largeTitle.bold())
                    .foregroundColor(.white)

                Button("Resume") { gameManager.resumeGame() }
                    .buttonStyle(.borderedProminent)

                Button("Settings") { gameManager.gameState = .settings }
                    .buttonStyle(.bordered)

                Button("Main Menu") { gameManager.returnToMainMenu() }
                    .buttonStyle(.bordered)
                    .tint(.red)
            }
        }
    }
}

// MARK: - Settings (placeholder)

struct SettingsView: View {
    @EnvironmentObject var gameManager: GameManager

    var body: some View {
        NavigationStack {
            List {
                Section("Audio") {
                    Text("Music Volume")
                    Text("SFX Volume")
                }
                Section("Accessibility") {
                    Text("Text Size")
                    Text("Colorblind Mode")
                }
            }
            .navigationTitle("Settings")
            .toolbar {
                Button("Done") {
                    gameManager.gameState = .mainMenu
                }
            }
        }
    }
}

// MARK: - Loading

struct LoadingView: View {
    var body: some View {
        VStack {
            ProgressView()
                .scaleEffect(1.5)
            Text("Loading...")
                .padding(.top)
                .foregroundStyle(.secondary)
        }
    }
}

#Preview {
    ContentView()
        .environmentObject(GameManager())
}
