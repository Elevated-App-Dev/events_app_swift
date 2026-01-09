using System;
using EventPlannerSim.Core;
using EventPlannerSim.Data;

namespace EventPlannerSim.Systems
{
    /// <summary>
    /// Handles in-app purchases and advertisements.
    /// Requirements: R28.1-R28.10, R29.1-R29.10, R30.1-R30.10
    /// </summary>
    public interface IMonetizationSystem
    {
        /// <summary>
        /// Initialize Unity IAP and Unity Ads.
        /// Requirements: R28.1-R28.2
        /// </summary>
        void Initialize();

        /// <summary>
        /// Check if a rewarded ad is available for the specified placement.
        /// Requirements: R30.1-R30.4
        /// </summary>
        bool IsRewardedAdReady(AdPlacement placement);

        /// <summary>
        /// Show a rewarded ad and invoke callback on completion.
        /// Requirements: R28.3, R30.1-R30.3
        /// </summary>
        /// <param name="placement">The ad placement context</param>
        /// <param name="onComplete">Callback with true if ad was watched completely, false otherwise</param>
        void ShowRewardedAd(AdPlacement placement, Action<bool> onComplete);

        /// <summary>
        /// Show an interstitial ad at a natural break point.
        /// Requirements: R30.5
        /// </summary>
        /// <param name="onComplete">Callback when ad is dismissed</param>
        void ShowInterstitialAd(Action onComplete);

        /// <summary>
        /// Initiate a purchase for a product.
        /// Requirements: R28.4-R28.6
        /// </summary>
        /// <param name="productId">The product identifier</param>
        /// <param name="onComplete">Callback with true if purchase succeeded, false otherwise</param>
        void PurchaseProduct(string productId, Action<bool> onComplete);

        /// <summary>
        /// Restore previously purchased non-consumable items.
        /// Requirements: R28.7
        /// </summary>
        /// <param name="onComplete">Callback with true if restore succeeded, false otherwise</param>
        void RestorePurchases(Action<bool> onComplete);

        /// <summary>
        /// Check if player has purchased "No Ads" or is VIP.
        /// Requirements: R29.3, R30.10
        /// </summary>
        bool IsAdFree { get; }

        /// <summary>
        /// Check if player has active VIP subscription.
        /// Requirements: R29.4
        /// </summary>
        bool IsVIP { get; }

        /// <summary>
        /// Check if Premium Idle Mode is unlocked.
        /// Requirements: R29.7-R29.8
        /// </summary>
        bool HasPremiumIdleMode { get; }

        /// <summary>
        /// Get remaining cooldown in seconds for a rewarded ad placement.
        /// Requirements: R30.4
        /// </summary>
        float GetAdCooldown(AdPlacement placement);

        /// <summary>
        /// Check if a specific product has been purchased (for non-consumables).
        /// Requirements: R28.6
        /// </summary>
        bool HasPurchased(string productId);

        /// <summary>
        /// Check if interstitial ads should be shown based on player status.
        /// Requirements: R30.5, R30.9
        /// </summary>
        bool ShouldShowInterstitialAds { get; }

        /// <summary>
        /// Get the reward amount for a specific ad placement.
        /// Requirements: R28.3
        /// </summary>
        AdReward GetAdReward(AdPlacement placement);

        /// <summary>
        /// Check if the monetization system has been initialized.
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// Event fired when initialization completes.
        /// </summary>
        event Action<bool> OnInitialized;

        /// <summary>
        /// Event fired when a purchase completes.
        /// </summary>
        event Action<string, PurchaseResult> OnPurchaseComplete;

        /// <summary>
        /// Event fired when an ad is watched.
        /// </summary>
        event Action<AdPlacement, AdWatchResult> OnAdWatched;
    }
}
