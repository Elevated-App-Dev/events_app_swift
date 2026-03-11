import Foundation
import Combine

protocol TutorialSystemProtocol {
    var isTutorialComplete: Bool { get }
    var isTutorialActive: Bool { get }
    var currentStep: TutorialStep { get }
    var highlightedElements: [String] { get }
    var currentTip: String? { get }
    var currentStage: Int { get }
    func startTutorial()
    func advanceStep()
    func skipTutorial()
    func highlightElements(_ elementIds: [String])
    func clearHighlights()
    func showContextualTip(_ tipKey: String)
    func hideContextualTip()
    func getTipText(_ tipKey: String) -> String?
    func getCurrentStepInstruction() -> String
    func getCurrentStepTitle() -> String
    func hasTipBeenShown(_ tipKey: String) -> Bool
    func markTipAsShown(_ tipKey: String)
    var onTutorialStarted: AnyPublisher<Void, Never> { get }
    var onStepAdvanced: AnyPublisher<TutorialStep, Never> { get }
    var onTutorialCompleted: AnyPublisher<Void, Never> { get }
    var onTutorialSkipped: AnyPublisher<Void, Never> { get }
}
