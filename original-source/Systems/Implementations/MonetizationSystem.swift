import Foundation
import Combine

/// Handles IAP and ads integration.
/// Pure Swift — actual StoreKit integration to be added in Xcode.
class MonetizationSystem: MonetizationSystemProtocol {

    private(set) var isInitialized = false
    private var state: MonetizationState
    private var adPlacements: [AdPlacement: AdPlacementData] = [:]
    private var products: [String: IAPProductData] = [:]

    private let _onPurchaseComplete = PassthroughSubject<(String, Bool), Never>()
    var onPurchaseComplete: AnyPublisher<(String, Bool), Never> { _onPurchaseComplete.eraseToAnyPublisher() }

    var isAdFree: Bool { state.isAdFree }
    var isVIP: Bool { state.isVIP }
    var hasPremiumIdleMode: Bool { state.hasPremiumIdleMode }

    init(state: MonetizationState? = nil) {
        self.state = state ?? MonetizationState()
        initializeDefaultData()
    }

    private func initializeDefaultData() {
        // Ad placements
        adPlacements[.emergencyFunding] = AdPlacementData(
            placement: .emergencyFunding, cooldownSeconds: 300, dailyLimit: 3,
            reward: AdReward(placement: .emergencyFunding, rewardType: .emergencyFunding, amount: 500, description: "Emergency funding unlocked")
        )
        adPlacements[.overtimeHours] = AdPlacementData(
            placement: .overtimeHours, cooldownSeconds: 0, dailyLimit: 2,
            reward: AdReward(placement: .overtimeHours, rewardType: .overtimeHours, amount: 4, description: "+4 overtime hours")
        )
        adPlacements[.randomEventMitigation] = AdPlacementData(
            placement: .randomEventMitigation, cooldownSeconds: 600, dailyLimit: 5,
            reward: AdReward(placement: .randomEventMitigation, rewardType: .eventMitigation, amount: 1, description: "Event mitigation unlocked")
        )
        adPlacements[.timeSkip] = AdPlacementData(
            placement: .timeSkip, cooldownSeconds: 180, dailyLimit: 10,
            reward: AdReward(placement: .timeSkip, rewardType: .timeSkip, amount: 1, description: "Skip waiting time")
        )

        // IAP products
        initializeProducts()
    }

    private func initializeProducts() {
        let productList: [IAPProductData] = [
            IAPProductData(productId: "currencySmall", displayName: "Small Currency Pack", description: "A small boost to your funds", priceUSD: 0.99, productType: .consumable, currencyReward: 1000),
            IAPProductData(productId: "currencyMedium", displayName: "Medium Currency Pack", description: "A medium boost to your funds", priceUSD: 4.99, productType: .consumable, currencyReward: 6000),
            IAPProductData(productId: "currencyLarge", displayName: "Large Currency Pack", description: "A large boost to your funds", priceUSD: 9.99, productType: .consumable, currencyReward: 15000),
            IAPProductData(productId: "currencyMega", displayName: "Mega Currency Pack", description: "A massive boost to your funds", priceUSD: 19.99, productType: .consumable, currencyReward: 35000),
            IAPProductData(productId: "starterPack", displayName: "Starter Pack", description: "Currency, premium vendor unlock, and cosmetic item", priceUSD: 4.99, productType: .nonConsumable, currencyReward: 5000),
            IAPProductData(productId: "noAds", displayName: "Remove Ads", description: "Remove all interstitial advertisements", priceUSD: 4.99, productType: .nonConsumable, grantsAdFree: true),
            IAPProductData(productId: "premiumIdleMode", displayName: "Premium Idle Mode", description: "Enable background time progression", priceUSD: 7.99, productType: .nonConsumable, grantsPremiumIdleMode: true),
            IAPProductData(productId: "vipSubscription", displayName: "VIP Subscription", description: "Daily rewards, exclusive content, ad-free, and Premium Idle Mode", priceUSD: 9.99, productType: .subscription, grantsVIP: true, grantsAdFree: true, grantsPremiumIdleMode: true)
        ]

        for product in productList {
            products[product.productId] = product
        }
    }

    func initialize() {
        guard !isInitialized else { return }
        // In production: integrate with StoreKit
        isInitialized = true
    }

    func isRewardedAdReady(_ placement: AdPlacement) -> Bool {
        guard isInitialized, let data = adPlacements[placement] else { return false }
        return state.canWatchAd(placement: placement, dailyLimit: data.dailyLimit)
    }

    func showRewardedAd(_ placement: AdPlacement, completion: @escaping (Bool) -> Void) {
        guard isRewardedAdReady(placement) else {
            completion(false)
            return
        }
        state.recordAdWatch(placement: placement)
        completion(true)
    }

    func purchaseProduct(_ productId: String, completion: @escaping (Bool) -> Void) {
        guard isInitialized, let product = products[productId] else {
            completion(false)
            return
        }
        // In production: use StoreKit purchase flow
        state.recordPurchase(productId)
        if product.grantsAdFree { state.isAdFree = true }
        if product.grantsVIP { state.isVIP = true }
        if product.grantsPremiumIdleMode { state.hasPremiumIdleMode = true }
        _onPurchaseComplete.send((productId, true))
        completion(true)
    }

    func restorePurchases(completion: @escaping (Bool) -> Void) {
        // In production: use StoreKit restore
        completion(true)
    }

    func hasPurchased(_ productId: String) -> Bool {
        state.hasPurchased(productId)
    }

    func getAdReward(_ placement: AdPlacement) -> AdReward? {
        adPlacements[placement]?.reward
    }

    func getProduct(_ productId: String) -> IAPProductData? {
        products[productId]
    }

    func getAllProducts() -> [IAPProductData] {
        Array(products.values)
    }

    func getState() -> MonetizationState { state }
    func setState(_ newState: MonetizationState) { state = newState }
    func resetDailyCounts() { state.resetDailyCounts() }
}
