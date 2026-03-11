import Foundation
import Combine

struct MarketingPaymentResult: Codable, Equatable {
    var success: Bool
    var amountPaid: Double
    var remainingFunds: Double
    var wasDowngraded: Bool
}

struct MarketingROI: Codable, Equatable {
    var totalSpent: Double
    var estimatedAdditionalRevenue: Double
    var inquiryRateModifier: Double
}

protocol MarketingSystemProtocol {
    var currentTier: MarketingTier { get }
    var currentFocus: MarketingFocus { get }
    var isPaused: Bool { get }
    func isMarketingAvailable(stage: BusinessStage, path: CareerPath) -> Bool
    func getInquiryRateModifier() -> Double
    func setMarketingTier(_ tier: MarketingTier)
    func getWeeklyCost(_ tier: MarketingTier) -> Double
    func getCurrentWeeklyCost() -> Double
    func processWeeklyPayment(availableFunds: Double) -> MarketingPaymentResult
    func getROIStatistics() -> MarketingROI
    func isTargetedMarketingAvailable(stage: BusinessStage) -> Bool
    func setTargetedFocus(_ focus: MarketingFocus)
    func pauseMarketing()
    func resumeMarketing()
    var onTierChanged: AnyPublisher<MarketingTier, Never> { get }
    var onAutoDowngraded: AnyPublisher<MarketingTier, Never> { get }
}
