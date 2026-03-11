import SwiftUI

struct PauseOverlay: View {
    @Environment(GameManager.self) private var gameManager

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

                Button("Settings") { gameManager.showSettings() }
                    .buttonStyle(.bordered)

                Button("Main Menu") { gameManager.returnToMainMenu() }
                    .buttonStyle(.bordered)
                    .tint(.red)
            }
        }
    }
}
