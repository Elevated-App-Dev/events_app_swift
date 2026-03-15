import Foundation
import Combine

/// Stub implementation of MarketingSystem for MVP.
/// Returns 1.0 modifier (no effect) — full implementation is Post-MVP.
class MarketingSystem: MarketingSystemProtocol {

    private(set) var currentTier: MarketingTier = .none
    private(set) var currentFocus: MarketingFocus = .general
    private(set) var isPaused: Bool = false

    private let _onTierChanged = PassthroughSubject<MarketingTier, Never>()
    var onTierChanged: AnyPublisher<MarketingTier, Never> { _onTierChanged.eraseToAnyPublisher() }

    func isMarketingAvailable(stage: BusinessStage, path: CareerPath) -> Bool { false }
    func getInquiryRateModifier() -> Double { 1.0 }
    func setMarketingTier(_ tier: MarketingTier) -> Bool { false }

    func getWeeklyCost(_ tier: MarketingTier) -> Double {
        switch tier {
        case .none:     return 0
        case .basic:    return 500
        case .standard: return 1500
        case .premium:  return 3000
        }
    }

    func getCurrentWeeklyCost() -> Double { 0 }

    func processWeeklyPayment(availableFunds: Double) -> MarketingPaymentResult {
        MarketingPaymentResult(
            paymentSuccessful: true,
            amountPaid: 0,
            wasDowngraded: false,
            previousTier: currentTier,
            newTier: currentTier
        )
    }

    func getROIStatistics() -> MarketingROI {
        MarketingROI()
    }

    func isTargetedMarketingAvailable(stage: BusinessStage) -> Bool { false }
    func setTargetedFocus(_ focus: MarketingFocus) -> Bool { false }
    func pauseMarketing() { isPaused = true }
    func resumeMarketing() { isPaused = false }
}
