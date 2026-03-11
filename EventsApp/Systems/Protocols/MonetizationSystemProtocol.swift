import Foundation

protocol MonetizationSystemProtocol {
    var isAdFree: Bool { get }
    var isVIP: Bool { get }
    var hasPremiumIdleMode: Bool { get }
    var shouldShowInterstitialAds: Bool { get }
    var isInitialized: Bool { get }
    func initialize()
    func isRewardedAdReady(_ placement: AdPlacement) -> Bool
    func showRewardedAd(_ placement: AdPlacement, completion: @escaping (Bool) -> Void)
    func showInterstitialAd(completion: @escaping () -> Void)
    func purchaseProduct(_ productId: String, completion: @escaping (Bool) -> Void)
    func restorePurchases(completion: @escaping (Bool) -> Void)
    func hasPurchased(_ productId: String) -> Bool
    func getAdCooldown(_ placement: AdPlacement) -> TimeInterval
    func getAdReward(_ placement: AdPlacement) -> AdReward?
}
