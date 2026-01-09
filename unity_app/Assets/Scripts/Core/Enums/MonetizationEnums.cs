namespace EventPlannerSim.Core
{
    /// <summary>
    /// Represents the placement/context for showing rewarded ads.
    /// Requirements: R30.1-R30.8
    /// </summary>
    public enum AdPlacement
    {
        EmergencyFunding,
        OvertimeHours,
        RandomEventMitigation,
        TimeSkip
    }

    /// <summary>
    /// Types of in-app purchase products.
    /// Requirements: R29.1-R29.10
    /// </summary>
    public enum IAPProductType
    {
        Consumable,      // Currency packs - can be purchased multiple times
        NonConsumable,   // Permanent unlocks - one-time purchase
        Subscription     // VIP subscription - recurring
    }

    /// <summary>
    /// Specific IAP product identifiers.
    /// Requirements: R29.1-R29.6
    /// </summary>
    public enum IAPProductId
    {
        // Currency Packs (Consumable) - R29.1
        CurrencySmall,    // $0.99
        CurrencyMedium,   // $4.99
        CurrencyLarge,    // $9.99
        CurrencyMega,     // $19.99

        // Starter Pack (Non-Consumable) - R29.2
        StarterPack,

        // Permanent Unlocks (Non-Consumable) - R29.3
        PremiumVenuePack,
        EliteVendorNetwork,
        NoAds,
        PremiumIdleMode,  // R29.7-R29.8

        // Subscription - R29.4
        VIPSubscription,

        // Business Upgrade Bundles (Post-MVP) - R29.9
        OfficeStarterKit,
        ProfessionalBundle,
        ExecutivePackage,

        // Training Fast-Track (Post-MVP) - R29.10
        TrainingFastTrack
    }

    /// <summary>
    /// Result of a purchase attempt.
    /// Requirements: R28.4-R28.5
    /// </summary>
    public enum PurchaseResult
    {
        Success,
        Failed,
        Cancelled,
        AlreadyOwned,
        NotAvailable,
        NetworkError
    }

    /// <summary>
    /// Result of watching an ad.
    /// Requirements: R28.3, R30.7
    /// </summary>
    public enum AdWatchResult
    {
        Completed,
        Skipped,
        Failed,
        NotAvailable
    }
}
