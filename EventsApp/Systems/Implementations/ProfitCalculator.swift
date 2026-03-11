import Foundation

class ProfitCalculator: ProfitCalculatorProtocol {
    // Profit margin constants
    private let successfulMinMargin: Double = 0.20
    private let successfulMaxMargin: Double = 0.30
    private let mediocreMinMargin: Double = 0.10
    private let mediocreMaxMargin: Double = 0.15

    // Satisfaction thresholds
    private let successfulThreshold: Double = 70
    private let mediocreThreshold: Double = 50

    func calculateProfit(revenue: Double, costs: Double) -> ProfitResult {
        let satisfaction = min(100, max(0, costs)) // costs used as satisfaction in this context
        let budget = max(0, revenue)

        let marginPercent: Double
        if satisfaction >= successfulThreshold {
            // R33.1: 70%+ = 20-30% margin
            marginPercent = calculateMarginInRange(
                satisfaction: satisfaction,
                minSat: successfulThreshold, maxSat: 100,
                minMargin: successfulMinMargin, maxMargin: successfulMaxMargin
            )
        } else if satisfaction >= mediocreThreshold {
            // R33.2: 50-69% = 10-15% margin
            marginPercent = calculateMarginInRange(
                satisfaction: satisfaction,
                minSat: mediocreThreshold, maxSat: successfulThreshold - 0.01,
                minMargin: mediocreMinMargin, maxMargin: mediocreMaxMargin
            )
        } else {
            // R33.3: <50% = break-even or loss
            marginPercent = calculateLossMargin(satisfaction)
        }

        let profitAmount = budget * marginPercent

        return ProfitResult(
            grossRevenue: budget,
            totalCosts: budget - profitAmount,
            netProfit: profitAmount,
            profitMargin: marginPercent
        )
    }

    func calculateCommission(level: Int, revenue: Double, satisfaction: Double) -> CommissionResult {
        let clampedLevel = min(5, max(1, level))
        let (basePay, commissionRate) = getCompensationByLevel(clampedLevel)

        let profitResult = calculateProfit(revenue: revenue, costs: satisfaction)
        let commissionableProfit = max(0, profitResult.netProfit)
        let commissionAmount = commissionableProfit * commissionRate

        return CommissionResult(
            basePay: basePay,
            commissionRate: commissionRate,
            commissionAmount: commissionAmount,
            totalCompensation: basePay + commissionAmount
        )
    }

    func getProfitMarginRange(satisfaction: Double) -> (min: Double, max: Double) {
        let clamped = min(100, max(0, satisfaction))
        if clamped >= successfulThreshold {
            return (successfulMinMargin, successfulMaxMargin)
        } else if clamped >= mediocreThreshold {
            return (mediocreMinMargin, mediocreMaxMargin)
        } else {
            return (-0.20, 0)
        }
    }

    func getCompensationByLevel(_ level: Int) -> (basePay: Double, commissionRate: Double) {
        switch level {
        case 1, 2: return (500, 0.05)    // Junior: $500 + 5%
        case 3, 4: return (750, 0.10)    // Planner: $750 + 10%
        case 5: return (1000, 0.15)      // Senior: $1000 + 15%
        default: return (500, 0.05)
        }
    }

    private func calculateMarginInRange(satisfaction: Double, minSat: Double, maxSat: Double, minMargin: Double, maxMargin: Double) -> Double {
        let normalized = min(1, max(0, (satisfaction - minSat) / (maxSat - minSat)))
        return minMargin + (normalized * (maxMargin - minMargin))
    }

    private func calculateLossMargin(_ satisfaction: Double) -> Double {
        let normalized = min(1, max(0, satisfaction / mediocreThreshold))
        let maxLoss = -0.20
        return maxLoss * (1 - normalized)
    }
}
