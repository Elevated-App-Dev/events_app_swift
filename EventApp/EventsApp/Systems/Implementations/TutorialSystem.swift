import Foundation

/// Guided instruction system for new players.
/// Manages tutorial steps, contextual tips, and UI highlighting.
class TutorialSystem {

    private(set) var isTutorialComplete = false
    private(set) var isTutorialActive = false
    private(set) var currentStep: TutorialStep = .welcome
    private(set) var highlightedElements: [String] = []
    private(set) var currentTip: String?
    private var shownTips: Set<String> = []
    var currentStage: Int = 1

    // Step instructions aligned with TutorialStep enum cases
    private let stepInstructions: [TutorialStep: (title: String, instruction: String)] = [
        .welcome:        ("Welcome!", "Welcome to Event Planning Simulator! Let's learn the basics."),
        .acceptClient:   ("First Client", "You've received your first client inquiry! Tap to view and accept it."),
        .selectVenue:    ("Book a Venue", "Now let's find a venue for your event. Open the map to browse locations."),
        .selectCaterer:  ("Hire a Caterer", "Great! Now hire a caterer for your event."),
        .eventExecution: ("Event Day", "Your event is ready! Wait for the event day to see how it goes."),
        .viewResults:    ("Results", "Check your event results to see client satisfaction and earnings."),
        .complete:       ("You're Ready!", "You've completed the tutorial. Good luck with your event planning career!")
    ]

    // MARK: - Tutorial Flow

    func startTutorial() {
        guard !isTutorialComplete else { return }
        isTutorialActive = true
        currentStep = .welcome
    }

    func advanceStep() {
        let steps = TutorialStep.allCases
        guard let currentIndex = steps.firstIndex(of: currentStep),
              currentIndex + 1 < steps.count else {
            completeTutorial()
            return
        }

        currentStep = steps[currentIndex + 1]
        clearHighlights()

        if currentStep == .complete {
            completeTutorial()
        }
    }

    func completeTutorial() {
        isTutorialActive = false
        isTutorialComplete = true
        clearHighlights()
    }

    func skipTutorial() {
        isTutorialActive = false
        isTutorialComplete = true
        clearHighlights()
    }

    // MARK: - Step Info

    func getCurrentStepTitle() -> String {
        stepInstructions[currentStep]?.title ?? ""
    }

    func getCurrentStepInstruction() -> String {
        stepInstructions[currentStep]?.instruction ?? ""
    }

    // MARK: - Highlighting

    func highlightElements(_ elementIds: [String]) {
        highlightedElements = elementIds
    }

    func clearHighlights() {
        highlightedElements.removeAll()
    }

    // MARK: - Contextual Tips

    func showContextualTip(_ tipKey: String) {
        guard !shownTips.contains(tipKey) else { return }
        currentTip = tipKey
        shownTips.insert(tipKey)
    }

    func hideContextualTip() {
        currentTip = nil
    }

    func hasTipBeenShown(_ tipKey: String) -> Bool {
        shownTips.contains(tipKey)
    }

    func markTipAsShown(_ tipKey: String) {
        shownTips.insert(tipKey)
    }

    // MARK: - Save/Load

    func getSaveData() -> TutorialSaveData {
        TutorialSaveData(
            isComplete: isTutorialComplete,
            currentStep: currentStep,
            shownTips: Array(shownTips)
        )
    }

    func loadSaveData(_ data: TutorialSaveData) {
        isTutorialComplete = data.isComplete
        currentStep = data.currentStep
        shownTips = Set(data.shownTips)
        isTutorialActive = false
    }
}
