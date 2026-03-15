import Foundation

enum AchievementType: String, CaseIterable, Codable, Hashable {
    case firstSteps
    case risingStar
    case goingPro
    case industryLeader
    case eventPlanningMogul
    case perfectPlanner
    case consistencyIsKey
    case excellenceStreak
    case budgetMaster
    case vendorWhisperer
    case weatherWatcher
    case perfectionistsPerfectionist
    case jugglingAct
    case crisisManager
    case selfMade
    case corporateClimber
    case celebrityHandler
    case aboveAndBeyond
    case familyFirst
    case comebackKid
}

enum AchievementCategory: String, CaseIterable, Codable, Hashable {
    case progression
    case mastery
    case challenge
    case secret
}
