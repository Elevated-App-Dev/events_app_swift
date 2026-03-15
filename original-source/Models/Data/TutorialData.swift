import Foundation

struct TutorialStepData: Codable, Equatable {
    var step: TutorialStep
    var title: String
    var instruction: String
    var highlightElementIds: [String] = []
    var disabledElementIds: [String] = []
    var tipKey: String = ""
    var requiresPlayerAction: Bool = false
    var triggerAction: String?
    var minimumStage: Int = 1
}

struct TutorialSaveData: Codable, Equatable {
    var isComplete: Bool = false
    var wasSkipped: Bool = false
    var currentStep: TutorialStep = .welcome
    var shownTips: [String] = []
}

enum TutorialStepProgression {
    private static let allSteps: [TutorialStep] = [
        .welcome, .acceptClient, .selectVenue, .selectCaterer,
        .eventExecution, .viewResults, .complete
    ]

    static func getNextStep(_ current: TutorialStep) -> TutorialStep? {
        guard let idx = allSteps.firstIndex(of: current),
              idx + 1 < allSteps.count else { return nil }
        return allSteps[idx + 1]
    }

    static func getPreviousStep(_ current: TutorialStep) -> TutorialStep? {
        guard let idx = allSteps.firstIndex(of: current),
              idx > 0 else { return nil }
        return allSteps[idx - 1]
    }

    static func isFirstStep(_ step: TutorialStep) -> Bool {
        step == allSteps.first
    }

    static func isLastStep(_ step: TutorialStep) -> Bool {
        step == allSteps.last
    }

    static func getStepIndex(_ step: TutorialStep) -> Int {
        allSteps.firstIndex(of: step) ?? 0
    }

    static func getTotalSteps() -> Int {
        allSteps.count
    }

    static func getProgressPercent(_ step: TutorialStep) -> Double {
        let idx = getStepIndex(step)
        let total = getTotalSteps()
        guard total > 1 else { return 0 }
        return Double(idx) / Double(total - 1) * 100
    }

    static func getStepFromIndex(_ index: Int) -> TutorialStep {
        guard index >= 0 && index < allSteps.count else { return .welcome }
        return allSteps[index]
    }

    static func getAllSteps() -> [TutorialStep] {
        allSteps
    }

    static func getDefaultStepData() -> [TutorialStep: TutorialStepData] {
        [
            .welcome: TutorialStepData(step: .welcome, title: "Welcome!", instruction: "Welcome to your event planning career! Let's learn the basics."),
            .acceptClient: TutorialStepData(step: .acceptClient, title: "Your First Client", instruction: "Accept a client inquiry to start planning your first event."),
            .selectVenue: TutorialStepData(step: .selectVenue, title: "Choose a Venue", instruction: "Select a venue that fits your client's budget and guest count."),
            .selectCaterer: TutorialStepData(step: .selectCaterer, title: "Book a Caterer", instruction: "Choose a caterer for the event."),
            .eventExecution: TutorialStepData(step: .eventExecution, title: "Event Day", instruction: "Watch how your event unfolds and handle any surprises."),
            .viewResults: TutorialStepData(step: .viewResults, title: "Results", instruction: "See how satisfied your client was and how much you earned."),
            .complete: TutorialStepData(step: .complete, title: "Tutorial Complete", instruction: "You're ready to start your event planning career!")
        ]
    }
}
