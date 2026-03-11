import SwiftUI

struct TutorialOverlayView: View {
    @Environment(GameManager.self) private var gameManager

    private var title: String {
        gameManager.tutorialSystem.getCurrentStepTitle()
    }

    private var instruction: String {
        gameManager.tutorialSystem.getCurrentStepInstruction()
    }

    private var showContinueButton: Bool {
        let step = gameManager.tutorialSystem.currentStep
        // Only show Continue for welcome and complete steps.
        // Other steps advance when the player performs the action.
        return step == .welcome || step == .complete
    }

    var body: some View {
        VStack {
            Spacer()

            VStack(spacing: 12) {
                Text(title)
                    .font(.headline)
                    .foregroundStyle(.white)

                Text(instruction)
                    .font(.subheadline)
                    .foregroundStyle(.white.opacity(0.9))
                    .multilineTextAlignment(.center)

                HStack(spacing: 16) {
                    if showContinueButton {
                        Button("Continue") {
                            gameManager.advanceTutorial()
                        }
                        .buttonStyle(.borderedProminent)
                    }

                    Button("Skip Tutorial") {
                        gameManager.skipTutorial()
                    }
                    .buttonStyle(.bordered)
                    .tint(.white)
                }
                .padding(.top, 4)
            }
            .padding(20)
            .frame(maxWidth: .infinity)
            .background(.black.opacity(0.8), in: RoundedRectangle(cornerRadius: 16))
            .padding()
        }
        .allowsHitTesting(true)
    }
}
