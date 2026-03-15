import Foundation

enum EventStatus: String, CaseIterable, Codable, Hashable {
    case inquiry
    case accepted
    case planning
    case executing
    case completed
    case cancelled
}

enum EventComplexity: String, CaseIterable, Codable, Hashable {
    case low
    case medium
    case high
    case veryHigh
}

enum WorkloadStatus: String, CaseIterable, Codable, Hashable {
    case optimal
    case comfortable
    case strained
    case critical
}

enum EventPhase: String, CaseIterable, Codable, Hashable {
    case booking
    case prePlanning
    case activePlanning
    case finalPrep
    case executionDay
    case results
}

enum BudgetCategory: String, CaseIterable, Codable, Hashable {
    case venue
    case catering
    case entertainment
    case decorations
    case staffing
    case contingency
    case photography
    case florals
    case rentals
    case audioVisual
    case transportation
    case security
}

enum TaskStatus: String, CaseIterable, Codable, Hashable {
    case locked
    case pending
    case inProgress
    case completed
    case failed
}
