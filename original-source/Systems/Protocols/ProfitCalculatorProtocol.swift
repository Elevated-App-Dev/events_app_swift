import Foundation

struct ProfitResult: Codable, Equatable {
    var grossRevenue: Double
    var totalCosts: Double
    var netProfit: Double
    var profitMargin: Double
}

struct CommissionResult: Codable, Equatable {
    var basePay: Double
    var commissionRate: Double
    var commissionAmount: Double
    var totalCompensation: Double
}

protocol ProfitCalculatorProtocol {
    func calculateProfit(revenue: Double, costs: Double) -> ProfitResult
    func calculateCommission(level: Int, revenue: Double, satisfaction: Double) -> CommissionResult
    func getProfitMarginRange(satisfaction: Double) -> (min: Double, max: Double)
    func getCompensationByLevel(_ level: Int) -> (basePay: Double, commissionRate: Double)
}
