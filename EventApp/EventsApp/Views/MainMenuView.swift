import SwiftUI

struct MainMenuView: View {
    @Environment(GameManager.self) private var gameManager

    var body: some View {
        ZStack {
            GameTheme.Colors.background
                .ignoresSafeArea()

            VStack(spacing: GameTheme.Spacing.md) {
                Spacer()

                Text("Event Planner")
                    .font(GameTheme.Typography.display)
                    .foregroundStyle(GameTheme.Colors.textPrimary)

                Text("Build Your Event Planning Empire")
                    .font(GameTheme.Typography.body)
                    .foregroundStyle(GameTheme.Colors.textSecondary)

                Spacer()

                VStack(spacing: GameTheme.Spacing.sm) {
                    Button(action: { gameManager.startNewGame() }) {
                        Text("New Game")
                            .primaryButton()
                    }

                    Button(action: { gameManager.continueGame() }) {
                        Text("Continue")
                            .secondaryButton()
                    }

                    Button(action: { gameManager.showSettings() }) {
                        Text("Settings")
                            .font(GameTheme.Typography.body)
                            .foregroundStyle(GameTheme.Colors.accent)
                    }
                    .padding(.top, GameTheme.Spacing.xs)
                }
                .padding(.horizontal, GameTheme.Spacing.xl)

                Spacer()
            }
        }
    }
}
