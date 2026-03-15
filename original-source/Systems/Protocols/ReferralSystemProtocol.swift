import Foundation

struct ReferralResult: Codable, Equatable {
    var wasReferred: Bool
    var referralChance: Double
    var streakBonus: Double
}

struct ExcellenceStreakResult: Codable, Equatable {
    var newStreak: Int
    var streakBroken: Bool
    var previousStreak: Int
}

protocol ReferralSystemProtocol {
    func calculateReferralProbability(satisfaction: Double, excellenceStreak: Int) -> Double
    func evaluateReferral(satisfaction: Double, excellenceStreak: Int) -> ReferralResult
    func updateExcellenceStreak(currentStreak: Int, satisfaction: Double) -> ExcellenceStreakResult
    func getStreakBonus(_ streak: Int) -> Double
    func qualifiesForReferral(satisfaction: Double) -> Bool
    func getBaseReferralChance(satisfaction: Double) -> Double
}
