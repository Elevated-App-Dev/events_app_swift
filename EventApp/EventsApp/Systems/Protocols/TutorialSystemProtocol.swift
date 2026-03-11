import Foundation

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
    func getCurrentStepInstruction() -> String
    func getCurrentStepTitle() -> String
    func hasTipBeenShown(_ tipKey: String) -> Bool
    func markTipAsShown(_ tipKey: String)
    func getSaveData() -> TutorialSaveData
    func loadSaveData(_ data: TutorialSaveData)
}
