import Foundation

struct IAPProductData: Codable, Equatable, Identifiable {
    var id: String { productId }
    var productId: String
    var displayName: String
    var description: String
    var priceUSD: Double
    var productType: IAPProductType
    var currencyReward: Int = 0
    var unlockIds: [String] = []
    var grantsAdFree: Bool = false
    var grantsVIP: Bool = false
    var grantsPremiumIdleMode: Bool = false
    var iconId: String = ""
    var isAvailable: Bool = true
    var sortOrder: Int = 0
    var category: IAPProductCategory
    var isLimitedTime: Bool = false
    var discountPercent: Double = 0

    var effectivePrice: Double {
        priceUSD * (1 - discountPercent / 100)
    }

    var isOnSale: Bool {
        discountPercent > 0
    }

    var hasUnlocks: Bool {
        !unlockIds.isEmpty
    }

    var isOneTimePurchase: Bool {
        productType == .nonConsumable
    }

    var isRepurchasable: Bool {
        productType == .consumable
    }

    static func createCurrencyPack(id: String, name: String, price: Double, currency: Int) -> IAPProductData {
        IAPProductData(
            productId: id,
            displayName: name,
            description: "\(currency) coins",
            priceUSD: price,
            productType: .consumable,
            currencyReward: currency,
            category: .currency
        )
    }

    static func createUnlock(id: String, name: String, description: String, price: Double, unlocks: [String]) -> IAPProductData {
        IAPProductData(
            productId: id,
            displayName: name,
            description: description,
            priceUSD: price,
            productType: .nonConsumable,
            unlockIds: unlocks,
            category: .unlock
        )
    }
}

struct MonetizationState: Codable, Equatable {
    private var _isAdFree: Bool = false
    private var _isVIP: Bool = false
    private var _hasPremiumIdleMode: Bool = false
    var isOnCorporatePath: Bool = false
    var purchasedProducts: [String] = []
    var dailyAdWatchCounts: [String: Int] = [:]
    var lastAdWatchTimes: [String: Date] = [:]
    var lastDailyReset: Date = Date()
    var totalAdsWatched: Int = 0
    var totalSpent: Double = 0
    var vipExpiry: Date?

    var isAdFree: Bool { _isAdFree }
    var isVIP: Bool { _isVIP }
    var hasPremiumIdleMode: Bool { _hasPremiumIdleMode }

    func canWatchAd(_ placement: AdPlacement, dailyLimit: Int) -> Bool {
        let count = dailyAdWatchCounts[placement.rawValue] ?? 0
        return count < dailyLimit
    }

    mutating func recordAdWatch(_ placement: AdPlacement) {
        let key = placement.rawValue
        dailyAdWatchCounts[key] = (dailyAdWatchCounts[key] ?? 0) + 1
        lastAdWatchTimes[key] = Date()
        totalAdsWatched += 1
    }

    mutating func resetDailyCounts() {
        dailyAdWatchCounts.removeAll()
        lastDailyReset = Date()
    }

    mutating func recordPurchase(_ productId: String) {
        if !purchasedProducts.contains(productId) {
            purchasedProducts.append(productId)
        }
    }

    func hasPurchased(_ productId: String) -> Bool {
        purchasedProducts.contains(productId)
    }

    mutating func setAdFree(_ value: Bool) { _isAdFree = value }
    mutating func setVIP(_ value: Bool, expiry: Date? = nil) { _isVIP = value; vipExpiry = expiry }
    mutating func setPremiumIdleMode(_ value: Bool) { _hasPremiumIdleMode = value }
    mutating func recordSpending(_ amount: Double) { totalSpent += amount }
}

struct AdPlacementData: Codable, Equatable {
    var placement: AdPlacement
    var displayName: String
    var description: String
    var cooldownSeconds: Double
    var dailyLimit: Int
    var reward: AdReward
    var isEnabled: Bool = true
    var minFundsRequired: Double = 0
    var maxFundsAllowed: Double = Double.greatestFiniteMagnitude
    var requiresSpecificState: Bool = false
    var iconId: String = ""
    var priority: Int = 0

    func shouldShowForFunds(_ playerFunds: Double) -> Bool {
        playerFunds >= minFundsRequired && playerFunds <= maxFundsAllowed
    }
}

struct AdReward: Codable, Equatable {
    var placement: AdPlacement
    var rewardType: AdRewardType
    var amount: Double
    var description: String

    init(placement: AdPlacement = .emergencyFunding, rewardType: AdRewardType = .currency, amount: Double = 0, description: String = "") {
        self.placement = placement
        self.rewardType = rewardType
        self.amount = amount
        self.description = description
    }
}
