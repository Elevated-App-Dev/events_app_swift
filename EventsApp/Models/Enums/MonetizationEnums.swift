import Foundation

enum AdPlacement: String, CaseIterable, Codable, Hashable {
    case emergencyFunding
    case overtimeHours
    case randomEventMitigation
    case timeSkip
}

enum IAPProductType: String, CaseIterable, Codable, Hashable {
    case consumable
    case nonConsumable
    case subscription
}

enum IAPProductId: String, CaseIterable, Codable, Hashable {
    case currencySmall
    case currencyMedium
    case currencyLarge
    case currencyMega
    case starterPack
    case premiumVenuePack
    case eliteVendorNetwork
    case noAds
    case premiumIdleMode
    case vipSubscription
    case officeStarterKit
    case professionalBundle
    case executivePackage
    case trainingFastTrack
}

enum PurchaseResult: String, CaseIterable, Codable, Hashable {
    case success
    case failed
    case cancelled
    case alreadyOwned
    case notAvailable
    case networkError
}

enum AdWatchResult: String, CaseIterable, Codable, Hashable {
    case completed
    case skipped
    case failed
    case notAvailable
}

enum AdRewardType: String, CaseIterable, Codable, Hashable {
    case currency
    case overtimeHours
    case emergencyFunding
    case eventMitigation
    case timeSkip
}

enum IAPProductCategory: String, CaseIterable, Codable, Hashable {
    case currency
    case starterPack
    case unlock
    case subscription
    case bundle
    case special
}
