import SwiftUI

struct MainMenuView: View {
    @Environment(GameManager.self) private var gameManager

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

                Button(action: { gameManager.showSettings() }) {
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
