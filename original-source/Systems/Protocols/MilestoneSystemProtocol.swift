import Foundation

struct NarrativeElement: Codable, Equatable {
    var type: NarrativeElementType
    var title: String
    var content: String
}

struct MilestoneSequenceResult: Codable, Equatable {
    var elements: [NarrativeElement]
    var careerSummary: CareerSummaryData
}

struct PathChoiceResult: Codable, Equatable {
    var chosenPath: CareerPath
    var narrative: String
    var updatedProgress: MilestoneProgress
}

protocol MilestoneSystemProtocol {
    func shouldTriggerMilestone(player: PlayerData, progress: MilestoneProgress) -> Bool
    func generateCareerSummary(player: PlayerData, events: [EventData], startDate: Date) -> CareerSummaryData
    func triggerMilestoneSequence(player: PlayerData, events: [EventData], progress: MilestoneProgress, startDate: Date) -> MilestoneSequenceResult
    func processPathChoice(_ path: CareerPath, progress: MilestoneProgress) -> PathChoiceResult
    func getEntrepreneurNarrative() -> String
    func getCorporateNarrative() -> String
    func getCreditsSequence() -> [String]
    func completeMilestoneSequence(_ progress: inout MilestoneProgress)
    func canSkipSequence(_ progress: MilestoneProgress) -> Bool
}
