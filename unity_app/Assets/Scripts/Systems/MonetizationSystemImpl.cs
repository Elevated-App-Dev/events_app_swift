using System;
using System.Collections.Generic;
using EventPlannerSim.Core;
using EventPlannerSim.Data;

namespace EventPlannerSim.Systems
{
    /// <summary>
    /// Implementation of the monetization system handling IAP and ads.
    /// Requirements: R28.1-R28.10, R29.1-R29.10, R30.1-R30.10
    /// 
    /// Note: This is a pure C# implementation that abstracts Unity IAP and Unity Ads.
    /// In a real Unity project, this would integrate with Unity's actual IAP and Ads SDKs.
    /// For testability, this implementation uses interfaces and can be mocked.
    /// </summary>
    public class MonetizationSystemImpl : IMonetizationSystem
    {
        // State
        private bool _isInitialized;
        private MonetizationState _state;
        private readonly Dictionary<AdPlacement, AdPlacementData> _adPlacements;
        private readonly Dictionary<string, IAPProductData> _products;

        // Events
        public event Action<bool> OnInitialized;
        public event Action<string, PurchaseResult> OnPurchaseComplete;
        public event Action<AdPlacement, AdWatchResult> OnAdWatched;

        // Properties
        public bool IsInitialized => _isInitialized;
        public bool IsAdFree => _state?.IsAdFree ?? false;
        public bool IsVIP => _state?.IsVIP ?? false;
        public bool HasPremiumIdleMode => _state?.HasPremiumIdleMode ?? false;
        public bool ShouldShowInterstitialAds => !IsAdFree && !IsVIP && !_state.IsOnCorporatePath;

        public MonetizationSystemImpl()
        {
            _state = new MonetizationState();
            _adPlacements = new Dictionary<AdPlacement, AdPlacementData>();
            _products = new Dictionary<string, IAPProductData>();
            InitializeDefaultData();
        }

        /// <summary>
        /// Constructor for testing with injected state.
        /// </summary>
        public MonetizationSystemImpl(MonetizationState state)
        {
            _state = state ?? new MonetizationState();
            _adPlacements = new Dictionary<AdPlacement, AdPlacementData>();
            _products = new Dictionary<string, IAPProductData>();
            InitializeDefaultData();
        }

        private void InitializeDefaultData()
        {
            // Initialize ad placements with default data
            // Requirements: R30.1-R30.4
            _adPlacements[AdPlacement.EmergencyFunding] = new AdPlacementData
            {
                placement = AdPlacement.EmergencyFunding,
                cooldownSeconds = 300f, // 5 minutes
                dailyLimit = 3,
                reward = new AdReward(AdPlacement.EmergencyFunding, AdRewardType.EmergencyFunding, 500f, "Emergency funding unlocked")
            };

            _adPlacements[AdPlacement.OvertimeHours] = new AdPlacementData
            {
                placement = AdPlacement.OvertimeHours,
                cooldownSeconds = 0f, // No cooldown, but limited to 2 per day
                dailyLimit = 2, // R30.2
                reward = new AdReward(AdPlacement.OvertimeHours, AdRewardType.OvertimeHours, 4f, "+4 overtime hours")
            };

            _adPlacements[AdPlacement.RandomEventMitigation] = new AdPlacementData
            {
                placement = AdPlacement.RandomEventMitigation,
                cooldownSeconds = 600f, // 10 minutes
                dailyLimit = 5,
                reward = new AdReward(AdPlacement.RandomEventMitigation, AdRewardType.EventMitigation, 1f, "Event mitigation unlocked")
            };

            _adPlacements[AdPlacement.TimeSkip] = new AdPlacementData
            {
                placement = AdPlacement.TimeSkip,
                cooldownSeconds = 180f, // 3 minutes
                dailyLimit = 10,
                reward = new AdReward(AdPlacement.TimeSkip, AdRewardType.TimeSkip, 1f, "Skip waiting time")
            };

            // Initialize IAP products
            // Requirements: R29.1-R29.6
            InitializeProducts();
        }

        private void InitializeProducts()
        {
            // Currency Packs (Consumable) - R29.1
            AddProduct(new IAPProductData
            {
                productId = IAPProductId.CurrencySmall.ToString(),
                displayName = "Small Currency Pack",
                description = "A small boost to your funds",
                priceUSD = 0.99f,
                productType = IAPProductType.Consumable,
                currencyReward = 1000
            });

            AddProduct(new IAPProductData
            {
                productId = IAPProductId.CurrencyMedium.ToString(),
                displayName = "Medium Currency Pack",
                description = "A medium boost to your funds",
                priceUSD = 4.99f,
                productType = IAPProductType.Consumable,
                currencyReward = 6000
            });

            AddProduct(new IAPProductData
            {
                productId = IAPProductId.CurrencyLarge.ToString(),
                displayName = "Large Currency Pack",
                description = "A large boost to your funds",
                priceUSD = 9.99f,
                productType = IAPProductType.Consumable,
                currencyReward = 15000
            });

            AddProduct(new IAPProductData
            {
                productId = IAPProductId.CurrencyMega.ToString(),
                displayName = "Mega Currency Pack",
                description = "A massive boost to your funds",
                priceUSD = 19.99f,
                productType = IAPProductType.Consumable,
                currencyReward = 35000
            });

            // Starter Pack (Non-Consumable) - R29.2
            AddProduct(new IAPProductData
            {
                productId = IAPProductId.StarterPack.ToString(),
                displayName = "Starter Pack",
                description = "Currency, premium vendor unlock, and cosmetic item",
                priceUSD = 4.99f,
                productType = IAPProductType.NonConsumable,
                currencyReward = 5000,
                unlockIds = new List<string> { "premium_vendor_1", "cosmetic_starter" }
            });

            // Permanent Unlocks (Non-Consumable) - R29.3
            AddProduct(new IAPProductData
            {
                productId = IAPProductId.PremiumVenuePack.ToString(),
                displayName = "Premium Venue Pack",
                description = "Unlock exclusive premium venues",
                priceUSD = 9.99f,
                productType = IAPProductType.NonConsumable,
                unlockIds = new List<string> { "premium_venues" }
            });

            AddProduct(new IAPProductData
            {
                productId = IAPProductId.EliteVendorNetwork.ToString(),
                displayName = "Elite Vendor Network",
                description = "Access to elite vendors with better rates",
                priceUSD = 9.99f,
                productType = IAPProductType.NonConsumable,
                unlockIds = new List<string> { "elite_vendors" }
            });

            AddProduct(new IAPProductData
            {
                productId = IAPProductId.NoAds.ToString(),
                displayName = "Remove Ads",
                description = "Remove all interstitial advertisements",
                priceUSD = 4.99f,
                productType = IAPProductType.NonConsumable,
                grantsAdFree = true
            });

            // Premium Idle Mode - R29.7-R29.8
            AddProduct(new IAPProductData
            {
                productId = IAPProductId.PremiumIdleMode.ToString(),
                displayName = "Premium Idle Mode",
                description = "Enable background time progression",
                priceUSD = 7.99f,
                productType = IAPProductType.NonConsumable,
                grantsPremiumIdleMode = true
            });

            // VIP Subscription - R29.4
            AddProduct(new IAPProductData
            {
                productId = IAPProductId.VIPSubscription.ToString(),
                displayName = "VIP Subscription",
                description = "Daily rewards, exclusive content, ad-free, and Premium Idle Mode",
                priceUSD = 9.99f,
                productType = IAPProductType.Subscription,
                grantsVIP = true,
                grantsAdFree = true,
                grantsPremiumIdleMode = true
            });
        }

        private void AddProduct(IAPProductData product)
        {
            _products[product.productId] = product;
        }

        /// <summary>
        /// Initialize Unity IAP and Unity Ads.
        /// Requirements: R28.1-R28.2
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized) return;

            // In a real implementation, this would:
            // 1. Initialize Unity IAP with product catalog
            // 2. Initialize Unity Ads with game ID
            // 3. Load any cached purchase state
            // 4. Verify subscriptions

            _isInitialized = true;
            OnInitialized?.Invoke(true);
        }

        /// <summary>
        /// Check if a rewarded ad is available for the specified placement.
        /// Requirements: R30.1-R30.4, R30.7
        /// </summary>
        public bool IsRewardedAdReady(AdPlacement placement)
        {
            if (!_isInitialized) return false;

            // Check if placement exists
            if (!_adPlacements.TryGetValue(placement, out var placementData))
                return false;

            // Check daily limit
            if (!_state.CanWatchAd(placement, placementData.dailyLimit))
                return false;

            // Check cooldown
            if (GetAdCooldown(placement) > 0)
                return false;

            // In a real implementation, would also check if Unity Ads has an ad loaded
            return true;
        }

        /// <summary>
        /// Show a rewarded ad and invoke callback on completion.
        /// Requirements: R28.3, R30.1-R30.3
        /// </summary>
        public void ShowRewardedAd(AdPlacement placement, Action<bool> onComplete)
        {
            if (!IsRewardedAdReady(placement))
            {
                OnAdWatched?.Invoke(placement, AdWatchResult.NotAvailable);
                onComplete?.Invoke(false);
                return;
            }

            // Record the ad watch
            _state.RecordAdWatch(placement);

            // In a real implementation, this would show the Unity Ads rewarded ad
            // and wait for the callback. For now, we simulate success.
            OnAdWatched?.Invoke(placement, AdWatchResult.Completed);
            onComplete?.Invoke(true);
        }

        /// <summary>
        /// Show an interstitial ad at a natural break point.
        /// Requirements: R30.5
        /// </summary>
        public void ShowInterstitialAd(Action onComplete)
        {
            if (!ShouldShowInterstitialAds)
            {
                onComplete?.Invoke();
                return;
            }

            // In a real implementation, this would show the Unity Ads interstitial
            // For now, we just invoke the callback
            onComplete?.Invoke();
        }

        /// <summary>
        /// Initiate a purchase for a product.
        /// Requirements: R28.4-R28.6
        /// </summary>
        public void PurchaseProduct(string productId, Action<bool> onComplete)
        {
            if (!_isInitialized)
            {
                OnPurchaseComplete?.Invoke(productId, PurchaseResult.Failed);
                onComplete?.Invoke(false);
                return;
            }

            if (!_products.TryGetValue(productId, out var product))
            {
                OnPurchaseComplete?.Invoke(productId, PurchaseResult.NotAvailable);
                onComplete?.Invoke(false);
                return;
            }

            // Check if non-consumable is already owned
            if (product.productType == IAPProductType.NonConsumable && HasPurchased(productId))
            {
                OnPurchaseComplete?.Invoke(productId, PurchaseResult.AlreadyOwned);
                onComplete?.Invoke(false);
                return;
            }

            // In a real implementation, this would initiate Unity IAP purchase flow
            // For now, we simulate the purchase process
            ProcessPurchase(product);
            OnPurchaseComplete?.Invoke(productId, PurchaseResult.Success);
            onComplete?.Invoke(true);
        }

        private void ProcessPurchase(IAPProductData product)
        {
            // Record the purchase
            _state.RecordPurchase(product.productId);

            // Apply product effects
            if (product.grantsAdFree)
                _state.SetAdFree(true);

            if (product.grantsVIP)
                _state.SetVIP(true);

            if (product.grantsPremiumIdleMode)
                _state.SetPremiumIdleMode(true);

            // Note: Currency rewards and unlocks would be handled by the game manager
            // This system just tracks the purchase state
        }

        /// <summary>
        /// Restore previously purchased non-consumable items.
        /// Requirements: R28.7
        /// </summary>
        public void RestorePurchases(Action<bool> onComplete)
        {
            if (!_isInitialized)
            {
                onComplete?.Invoke(false);
                return;
            }

            // In a real implementation, this would call Unity IAP's RestorePurchases
            // and iterate through all previously purchased non-consumables
            // For now, we just invoke success
            onComplete?.Invoke(true);
        }

        /// <summary>
        /// Get remaining cooldown in seconds for a rewarded ad placement.
        /// Requirements: R30.4
        /// </summary>
        public float GetAdCooldown(AdPlacement placement)
        {
            if (!_adPlacements.TryGetValue(placement, out var placementData))
                return 0f;

            var lastWatchTime = _state.GetLastAdWatchTime(placement);
            if (lastWatchTime == DateTime.MinValue)
                return 0f;

            var elapsed = (float)(DateTime.UtcNow - lastWatchTime).TotalSeconds;
            var remaining = placementData.cooldownSeconds - elapsed;
            return Math.Max(0f, remaining);
        }

        /// <summary>
        /// Check if a specific product has been purchased (for non-consumables).
        /// Requirements: R28.6
        /// </summary>
        public bool HasPurchased(string productId)
        {
            return _state.HasPurchased(productId);
        }

        /// <summary>
        /// Get the reward amount for a specific ad placement.
        /// Requirements: R28.3
        /// </summary>
        public AdReward GetAdReward(AdPlacement placement)
        {
            if (_adPlacements.TryGetValue(placement, out var placementData))
                return placementData.reward;

            return null;
        }

        /// <summary>
        /// Get the current monetization state (for save/load).
        /// </summary>
        public MonetizationState GetState() => _state;

        /// <summary>
        /// Set the monetization state (for save/load).
        /// </summary>
        public void SetState(MonetizationState state)
        {
            _state = state ?? new MonetizationState();
        }

        /// <summary>
        /// Reset daily ad counts (called at start of new game day).
        /// </summary>
        public void ResetDailyCounts()
        {
            _state.ResetDailyCounts();
        }

        /// <summary>
        /// Set whether player is on corporate path (affects interstitial ads).
        /// Requirements: R30.9
        /// </summary>
        public void SetCorporatePath(bool isOnCorporatePath)
        {
            _state.IsOnCorporatePath = isOnCorporatePath;
        }

        /// <summary>
        /// Get product data by ID.
        /// </summary>
        public IAPProductData GetProduct(string productId)
        {
            return _products.TryGetValue(productId, out var product) ? product : null;
        }

        /// <summary>
        /// Get all available products.
        /// </summary>
        public IEnumerable<IAPProductData> GetAllProducts() => _products.Values;
    }
}
