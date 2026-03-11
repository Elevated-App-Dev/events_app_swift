import SwiftUI

struct ContentView: View {
    @Environment(GameManager.self) private var gameManager

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

#Preview {
    ContentView()
        .environment(GameManager())
}
