import Foundation
import Combine

struct MarketingPaymentResult: Codable, Equatable {
    var paymentSuccessful: Bool
    var amountPaid: Double
    var wasDowngraded: Bool
    var previousTier: MarketingTier
    var newTier: MarketingTier
}

struct MarketingROI: Codable, Equatable {
    var totalSpent: Double = 0
    var estimatedAdditionalRevenue: Double = 0
    var inquiryRateModifier: Double = 1.0
}

protocol MarketingSystemProtocol {
    var currentTier: MarketingTier { get }
    var currentFocus: MarketingFocus { get }
    var isPaused: Bool { get }
    func isMarketingAvailable(stage: BusinessStage, path: CareerPath) -> Bool
    func getInquiryRateModifier() -> Double
    func setMarketingTier(_ tier: MarketingTier) -> Bool
    func getWeeklyCost(_ tier: MarketingTier) -> Double
    func getCurrentWeeklyCost() -> Double
    func processWeeklyPayment(availableFunds: Double) -> MarketingPaymentResult
    func getROIStatistics() -> MarketingROI
    func isTargetedMarketingAvailable(stage: BusinessStage) -> Bool
    func setTargetedFocus(_ focus: MarketingFocus) -> Bool
    func pauseMarketing()
    func resumeMarketing()
    var onTierChanged: AnyPublisher<MarketingTier, Never> { get }
}
