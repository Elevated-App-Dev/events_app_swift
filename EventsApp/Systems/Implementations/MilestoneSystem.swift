import Foundation

/// Manages Stage 3 milestone sequence and career path choice.
class MilestoneSystem: MilestoneSystemProtocol {

    func shouldTriggerMilestone(player: PlayerData, progress: MilestoneProgress) -> Bool {
        if progress.hasSeenStage3Milestone && progress.hasChosenPath { return false }
        return player.stage == .smallCompany
    }

    func generateCareerSummary(player: PlayerData, events: [EventData], startDate: Date) -> CareerSummaryData {
        var summary = CareerSummaryData()
        summary.totalEventsCompleted = events.count
        summary.currentReputation = player.reputation

        if !events.isEmpty {
            let sorted = events.sorted { ($0.acceptedDate ?? GameDate()) < ($1.acceptedDate ?? GameDate()) }
            summary.firstEventName = sorted.first?.eventTitle ?? "Unknown Event"

            if let best = events.compactMap({ e -> (String, Double)? in
                guard let r = e.results else { return nil }
                return (e.eventTitle, r.finalSatisfaction)
            }).max(by: { $0.1 < $1.1 }) {
                summary.highestSatisfactionEventName = best.0
                summary.highestSatisfactionScore = best.1
            } else {
                summary.highestSatisfactionEventName = "N/A"
                summary.highestSatisfactionScore = 0
            }

            summary.totalMoneyEarned = events.compactMap { $0.results?.profit }.reduce(0, +)
        } else {
            summary.firstEventName = "N/A"
            summary.highestSatisfactionEventName = "N/A"
        }

        return summary
    }

    func triggerMilestoneSequence(player: PlayerData, events: [EventData], progress: MilestoneProgress, startDate: Date) -> MilestoneSequenceResult {
        let summary = generateCareerSummary(player: player, events: events, startDate: startDate)
        return MilestoneSequenceResult(
            elements: [],
            careerSummary: summary
        )
    }

    func processPathChoice(_ path: CareerPath, progress: MilestoneProgress) -> PathChoiceResult {
        var updatedProgress = progress
        updatedProgress.hasChosenPath = true
        updatedProgress.chosenPath = path

        let narrative = path == .entrepreneur
            ? getEntrepreneurNarrative()
            : getCorporateNarrative()

        return PathChoiceResult(
            chosenPath: path,
            narrative: narrative,
            updatedProgress: updatedProgress
        )
    }

    func getEntrepreneurNarrative() -> String {
        "You sign the lease on your very first office space. It's small, but it's yours. The beginning of something great."
    }

    func getCorporateNarrative() -> String {
        "The entire company gathers in the conference room. Your promotion to Director is announced to applause."
    }

    func getCreditsSequence() -> [String] {
        [
            "Congratulations! You've completed the main story of Event Planning Simulator.",
            "Event Planning Simulator — Developed with passion for event planners everywhere.",
            "But this isn't the end. Stages 4 and 5 await with new challenges, bigger events, and greater rewards.",
            "Expansion Mode Unlocked — Continue building your empire in Stages 4 and 5!"
        ]
    }

    func completeMilestoneSequence(_ progress: inout MilestoneProgress) {
        progress.hasSeenStage3Milestone = true
        progress.canSkipMilestoneSequence = true
    }

    func canSkipSequence(_ progress: MilestoneProgress) -> Bool {
        progress.canSkipMilestoneSequence
    }
}
