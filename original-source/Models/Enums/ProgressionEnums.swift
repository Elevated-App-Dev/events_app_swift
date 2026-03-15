import Foundation

enum PerformanceReviewOutcome: String, CaseIterable, Codable, Hashable {
    case positive
    case neutral
    case negative
}

enum PressCoverage: String, CaseIterable, Codable, Hashable {
    case positive
    case neutral
    case negative
}

enum NarrativeElementType: String, CaseIterable, Codable, Hashable {
    case careerSummary
    case pathChoice
    case entrepreneurNarrative
    case corporateNarrative
    case credits
    case storyContinues
}
