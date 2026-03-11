import Foundation

protocol MonetizationSystemProtocol {
    var isAdFree: Bool { get }
    var isVIP: Bool { get }
    var hasPremiumIdleMode: Bool { get }
    var isInitialized: Bool { get }
    func initialize()
    func isRewardedAdReady(_ placement: AdPlacement) -> Bool
    func showRewardedAd(_ placement: AdPlacement, completion: @escaping (Bool) -> Void)
    func purchaseProduct(_ productId: String, completion: @escaping (Bool) -> Void)
    func restorePurchases(completion: @escaping (Bool) -> Void)
    func hasPurchased(_ productId: String) -> Bool
    func getAdReward(_ placement: AdPlacement) -> AdReward?
    func getProduct(_ productId: String) -> IAPProductData?
    func getAllProducts() -> [IAPProductData]
    func getState() -> MonetizationState
    func setState(_ newState: MonetizationState)
    func resetDailyCounts()
}
