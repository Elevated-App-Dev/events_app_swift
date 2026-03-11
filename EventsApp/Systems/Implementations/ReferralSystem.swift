import Foundation

class ReferralSystem: ReferralSystemProtocol {
    // Thresholds (R23.2)
    private let excellentSatisfactionThreshold: Double = 95
    private let highSatisfactionThreshold: Double = 90
    private let streakResetThreshold: Double = 80

    // Base chances (R23.2)
    private let excellentReferralChance: Double = 0.80  // 95-100%
    private let highReferralChance: Double = 0.50       // 90-94%

    // Streak bonuses (R23.6-R23.8)
    private let streak3Bonus: Double = 0.10
    private let streak5Bonus: Double = 0.20

    func calculateReferralProbability(satisfaction: Double, excellenceStreak: Int) -> Double {
        let baseChance = getBaseReferralChance(satisfaction: satisfaction)
        guard baseChance > 0 else { return 0 }
        let streakBonus = getStreakBonus(excellenceStreak)
        return min(1.0, baseChance + streakBonus)
    }

    func evaluateReferral(satisfaction: Double, excellenceStreak: Int) -> ReferralResult {
        let baseChance = getBaseReferralChance(satisfaction: satisfaction)
        let streakBonus = baseChance > 0 ? getStreakBonus(excellenceStreak) : 0
        let probability = min(1.0, baseChance + streakBonus)
        let wasReferred = probability > 0 && Double.random(in: 0..<1) < probability

        return ReferralResult(
            wasReferred: wasReferred,
            referralChance: probability,
            streakBonus: streakBonus
        )
    }

    func updateExcellenceStreak(currentStreak: Int, satisfaction: Double) -> ExcellenceStreakResult {
        if satisfaction >= highSatisfactionThreshold {
            // 90%+: increment streak (R23.6)
            return ExcellenceStreakResult(
                newStreak: currentStreak + 1,
                streakBroken: false,
                previousStreak: currentStreak
            )
        } else if satisfaction < streakResetThreshold {
            // Below 80%: reset streak (R23.9)
            return ExcellenceStreakResult(
                newStreak: 0,
                streakBroken: true,
                previousStreak: currentStreak
            )
        } else {
            // 80-89%: unchanged
            return ExcellenceStreakResult(
                newStreak: currentStreak,
                streakBroken: false,
                previousStreak: currentStreak
            )
        }
    }

    func getStreakBonus(_ streak: Int) -> Double {
        if streak >= 5 { return streak5Bonus }
        if streak >= 3 { return streak3Bonus }
        return 0
    }

    func qualifiesForReferral(satisfaction: Double) -> Bool {
        satisfaction >= highSatisfactionThreshold
    }

    func getBaseReferralChance(satisfaction: Double) -> Double {
        if satisfaction >= excellentSatisfactionThreshold {
            return excellentReferralChance
        } else if satisfaction >= highSatisfactionThreshold {
            return highReferralChance
        }
        return 0
    }
}
