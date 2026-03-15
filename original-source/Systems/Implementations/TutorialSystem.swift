import Foundation
import Combine

/// Guided instruction system for new players.
/// Manages tutorial steps, contextual tips, and UI highlighting.
class TutorialSystem: TutorialSystemProtocol {

    private(set) var isTutorialComplete = false
    private(set) var isTutorialActive = false
    private(set) var currentStep: TutorialStep = .welcome
    private(set) var highlightedElements: [String] = []
    private(set) var currentTip: String?
    private var shownTips: Set<String> = []
    var currentStage: Int = 1

    private let _onTutorialStarted = PassthroughSubject<Void, Never>()
    private let _onStepAdvanced = PassthroughSubject<TutorialStep, Never>()
    private let _onTutorialCompleted = PassthroughSubject<Void, Never>()
    private let _onTipShown = PassthroughSubject<(String, String), Never>()

    var onTutorialStarted: AnyPublisher<Void, Never> { _onTutorialStarted.eraseToAnyPublisher() }
    var onStepAdvanced: AnyPublisher<TutorialStep, Never> { _onStepAdvanced.eraseToAnyPublisher() }
    var onTutorialCompleted: AnyPublisher<Void, Never> { _onTutorialCompleted.eraseToAnyPublisher() }
    var onTipShown: AnyPublisher<(String, String), Never> { _onTipShown.eraseToAnyPublisher() }

    // Step instructions
    private let stepInstructions: [TutorialStep: (title: String, instruction: String)] = [
        .welcome:           ("Welcome!", "Welcome to Event Planning Simulator! Let's learn the basics."),
        .openPhone:         ("Your Phone", "Tap the phone icon at the bottom of the screen to open your phone."),
        .viewCalendar:      ("Calendar", "Open the Calendar app to see your upcoming events."),
        .acceptInquiry:     ("First Client", "You've received your first client inquiry! Tap to view and accept it."),
        .bookVenue:         ("Book a Venue", "Now let's find a venue for your event. Open the map to browse locations."),
        .bookVendor:        ("Hire Vendors", "Great! Now hire vendors for catering, entertainment, and decorations."),
        .completeEvent:     ("Event Day", "Your event is ready! Wait for the event day to see how it goes."),
        .viewResults:       ("Results", "Check your event results to see client satisfaction and earnings."),
        .tutorialComplete:  ("You're Ready!", "You've completed the tutorial. Good luck with your event planning career!")
    ]

    // MARK: - Tutorial Flow

    func startTutorial() {
        guard !isTutorialComplete else { return }
        isTutorialActive = true
        currentStep = .welcome
        _onTutorialStarted.send()
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
        _onStepAdvanced.send(currentStep)

        if currentStep == .tutorialComplete {
            completeTutorial()
        }
    }

    func completeTutorial() {
        isTutorialActive = false
        isTutorialComplete = true
        clearHighlights()
        _onTutorialCompleted.send()
    }

    func skipTutorial() {
        isTutorialActive = false
        isTutorialComplete = true
        clearHighlights()
        _onTutorialCompleted.send()
    }

    // MARK: - Step Info

    func getStepTitle() -> String {
        stepInstructions[currentStep]?.title ?? ""
    }

    func getStepInstruction() -> String {
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

    func showTip(id: String, title: String, message: String) {
        guard !shownTips.contains(id) else { return }
        currentTip = message
        shownTips.insert(id)
        _onTipShown.send((title, message))
    }

    func hideTip() {
        currentTip = nil
    }

    func hasTipBeenShown(_ tipId: String) -> Bool {
        shownTips.contains(tipId)
    }

    // MARK: - Save/Load

    func getSaveData() -> TutorialSaveData {
        TutorialSaveData(
            isTutorialComplete: isTutorialComplete,
            currentStep: currentStep,
            shownTips: Array(shownTips)
        )
    }

    func loadSaveData(_ data: TutorialSaveData) {
        isTutorialComplete = data.isTutorialComplete
        currentStep = data.currentStep
        shownTips = Set(data.shownTips)
        isTutorialActive = false
    }
}
