using System;
using NUnit.Framework;
using EventPlannerSim.Core;
using EventPlannerSim.Data;
using EventPlannerSim.Systems;

namespace EventPlannerSim.Tests.EditMode
{
    /// <summary>
    /// Unit tests for MonetizationSystemImpl.
    /// Validates: Requirements R28.1-R28.10, R29.1-R29.10, R30.1-R30.10
    /// </summary>
    [TestFixture]
    public class MonetizationSystemTests
    {
        private MonetizationSystemImpl _monetizationSystem;
        private MonetizationState _state;

        [SetUp]
        public void Setup()
        {
            _state = new MonetizationState();
            _monetizationSystem = new MonetizationSystemImpl(_state);
        }

        #region Initialization Tests (R28.1-R28.2)

        /// <summary>
        /// Test that MonetizationSystem initializes correctly.
        /// **Validates: Requirements R28.1-R28.2**
        /// </summary>
        [Test]
        public void Initialize_SetsIsInitializedTrue()
        {
            Assert.IsFalse(_monetizationSystem.IsInitialized);
            
            _monetizationSystem.Initialize();
            
            Assert.IsTrue(_monetizationSystem.IsInitialized);
        }

        /// <summary>
        /// Test that Initialize fires OnInitialized event.
        /// **Validates: Requirements R28.1**
        /// </summary>
        [Test]
        public void Initialize_FiresOnInitializedEvent()
        {
            bool? initResult = null;
            _monetizationSystem.OnInitialized += result => initResult = result;
            
            _monetizationSystem.Initialize();
            
            Assert.IsTrue(initResult);
        }

        /// <summary>
        /// Test that Initialize only runs once.
        /// **Validates: Requirements R28.1**
        /// </summary>
        [Test]
        public void Initialize_CalledTwice_OnlyFiresEventOnce()
        {
            int eventCount = 0;
            _monetizationSystem.OnInitialized += _ => eventCount++;
            
            _monetizationSystem.Initialize();
            _monetizationSystem.Initialize();
            
            Assert.AreEqual(1, eventCount);
        }

        #endregion

        #region Ad Availability Tests (R30.1-R30.4)

        /// <summary>
        /// Test that IsRewardedAdReady returns false when not initialized.
        /// **Validates: Requirements R30.1**
        /// </summary>
        [Test]
        public void IsRewardedAdReady_NotInitialized_ReturnsFalse()
        {
            var result = _monetizationSystem.IsRewardedAdReady(AdPlacement.EmergencyFunding);
            
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Test that IsRewardedAdReady returns true when initialized and available.
        /// **Validates: Requirements R30.1**
        /// </summary>
        [Test]
        public void IsRewardedAdReady_Initialized_ReturnsTrue()
        {
            _monetizationSystem.Initialize();
            
            var result = _monetizationSystem.IsRewardedAdReady(AdPlacement.EmergencyFunding);
            
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test that IsRewardedAdReady respects daily limit.
        /// **Validates: Requirements R30.2, R30.4**
        /// </summary>
        [Test]
        public void IsRewardedAdReady_DailyLimitReached_ReturnsFalse()
        {
            _monetizationSystem.Initialize();
            
            // Overtime hours has limit of 2 per day
            _state.RecordAdWatch(AdPlacement.OvertimeHours);
            _state.RecordAdWatch(AdPlacement.OvertimeHours);
            
            var result = _monetizationSystem.IsRewardedAdReady(AdPlacement.OvertimeHours);
            
            Assert.IsFalse(result);
        }

        #endregion

        #region Show Rewarded Ad Tests (R28.3, R30.1-R30.3)

        /// <summary>
        /// Test that ShowRewardedAd invokes callback with true on success.
        /// **Validates: Requirements R28.3**
        /// </summary>
        [Test]
        public void ShowRewardedAd_Success_InvokesCallbackWithTrue()
        {
            _monetizationSystem.Initialize();
            bool? callbackResult = null;
            
            _monetizationSystem.ShowRewardedAd(AdPlacement.EmergencyFunding, result => callbackResult = result);
            
            Assert.IsTrue(callbackResult);
        }

        /// <summary>
        /// Test that ShowRewardedAd fires OnAdWatched event.
        /// **Validates: Requirements R28.3**
        /// </summary>
        [Test]
        public void ShowRewardedAd_Success_FiresOnAdWatchedEvent()
        {
            _monetizationSystem.Initialize();
            AdPlacement? watchedPlacement = null;
            AdWatchResult? watchResult = null;
            _monetizationSystem.OnAdWatched += (placement, result) => 
            {
                watchedPlacement = placement;
                watchResult = result;
            };
            
            _monetizationSystem.ShowRewardedAd(AdPlacement.OvertimeHours, _ => { });
            
            Assert.AreEqual(AdPlacement.OvertimeHours, watchedPlacement);
            Assert.AreEqual(AdWatchResult.Completed, watchResult);
        }

        /// <summary>
        /// Test that ShowRewardedAd returns false when not available.
        /// **Validates: Requirements R30.7**
        /// </summary>
        [Test]
        public void ShowRewardedAd_NotAvailable_InvokesCallbackWithFalse()
        {
            // Not initialized, so ads not available
            bool? callbackResult = null;
            
            _monetizationSystem.ShowRewardedAd(AdPlacement.EmergencyFunding, result => callbackResult = result);
            
            Assert.IsFalse(callbackResult);
        }

        /// <summary>
        /// Test that ShowRewardedAd records the ad watch.
        /// **Validates: Requirements R30.4**
        /// </summary>
        [Test]
        public void ShowRewardedAd_RecordsAdWatch()
        {
            _monetizationSystem.Initialize();
            
            _monetizationSystem.ShowRewardedAd(AdPlacement.TimeSkip, _ => { });
            
            Assert.AreEqual(1, _state.GetDailyAdWatchCount(AdPlacement.TimeSkip));
        }

        #endregion

        #region Interstitial Ad Tests (R30.5, R30.9)

        /// <summary>
        /// Test that ShowInterstitialAd invokes callback.
        /// **Validates: Requirements R30.5**
        /// </summary>
        [Test]
        public void ShowInterstitialAd_InvokesCallback()
        {
            _monetizationSystem.Initialize();
            bool callbackInvoked = false;
            
            _monetizationSystem.ShowInterstitialAd(() => callbackInvoked = true);
            
            Assert.IsTrue(callbackInvoked);
        }

        /// <summary>
        /// Test that ShouldShowInterstitialAds returns false when ad-free.
        /// **Validates: Requirements R30.5**
        /// </summary>
        [Test]
        public void ShouldShowInterstitialAds_WhenAdFree_ReturnsFalse()
        {
            _state.SetAdFree(true);
            
            Assert.IsFalse(_monetizationSystem.ShouldShowInterstitialAds);
        }

        /// <summary>
        /// Test that ShouldShowInterstitialAds returns false when VIP.
        /// **Validates: Requirements R30.5**
        /// </summary>
        [Test]
        public void ShouldShowInterstitialAds_WhenVIP_ReturnsFalse()
        {
            _state.SetVIP(true);
            
            Assert.IsFalse(_monetizationSystem.ShouldShowInterstitialAds);
        }

        /// <summary>
        /// Test that ShouldShowInterstitialAds returns false on corporate path.
        /// **Validates: Requirements R30.9**
        /// </summary>
        [Test]
        public void ShouldShowInterstitialAds_OnCorporatePath_ReturnsFalse()
        {
            _state.IsOnCorporatePath = true;
            
            Assert.IsFalse(_monetizationSystem.ShouldShowInterstitialAds);
        }

        /// <summary>
        /// Test that ShouldShowInterstitialAds returns true for regular player.
        /// **Validates: Requirements R30.5**
        /// </summary>
        [Test]
        public void ShouldShowInterstitialAds_RegularPlayer_ReturnsTrue()
        {
            Assert.IsTrue(_monetizationSystem.ShouldShowInterstitialAds);
        }

        #endregion

        #region Purchase Tests (R28.4-R28.8)

        /// <summary>
        /// Test that PurchaseProduct fails when not initialized.
        /// **Validates: Requirements R28.4**
        /// </summary>
        [Test]
        public void PurchaseProduct_NotInitialized_ReturnsFalse()
        {
            bool? result = null;
            
            _monetizationSystem.PurchaseProduct("CurrencySmall", success => result = success);
            
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Test that PurchaseProduct succeeds for valid product.
        /// **Validates: Requirements R28.4**
        /// </summary>
        [Test]
        public void PurchaseProduct_ValidProduct_ReturnsTrue()
        {
            _monetizationSystem.Initialize();
            bool? result = null;
            
            _monetizationSystem.PurchaseProduct("CurrencySmall", success => result = success);
            
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test that PurchaseProduct fails for invalid product.
        /// **Validates: Requirements R28.5**
        /// </summary>
        [Test]
        public void PurchaseProduct_InvalidProduct_ReturnsFalse()
        {
            _monetizationSystem.Initialize();
            bool? result = null;
            
            _monetizationSystem.PurchaseProduct("InvalidProduct", success => result = success);
            
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Test that PurchaseProduct fires OnPurchaseComplete event.
        /// **Validates: Requirements R28.4**
        /// </summary>
        [Test]
        public void PurchaseProduct_FiresOnPurchaseCompleteEvent()
        {
            _monetizationSystem.Initialize();
            string purchasedId = null;
            PurchaseResult? purchaseResult = null;
            _monetizationSystem.OnPurchaseComplete += (id, result) =>
            {
                purchasedId = id;
                purchaseResult = result;
            };
            
            _monetizationSystem.PurchaseProduct("CurrencyMedium", _ => { });
            
            Assert.AreEqual("CurrencyMedium", purchasedId);
            Assert.AreEqual(PurchaseResult.Success, purchaseResult);
        }

        /// <summary>
        /// Test that purchasing NoAds sets IsAdFree.
        /// **Validates: Requirements R29.3**
        /// </summary>
        [Test]
        public void PurchaseProduct_NoAds_SetsIsAdFree()
        {
            _monetizationSystem.Initialize();
            
            _monetizationSystem.PurchaseProduct("NoAds", _ => { });
            
            Assert.IsTrue(_monetizationSystem.IsAdFree);
        }

        /// <summary>
        /// Test that purchasing VIPSubscription sets IsVIP.
        /// **Validates: Requirements R29.4**
        /// </summary>
        [Test]
        public void PurchaseProduct_VIPSubscription_SetsIsVIP()
        {
            _monetizationSystem.Initialize();
            
            _monetizationSystem.PurchaseProduct("VIPSubscription", _ => { });
            
            Assert.IsTrue(_monetizationSystem.IsVIP);
        }

        /// <summary>
        /// Test that purchasing PremiumIdleMode sets HasPremiumIdleMode.
        /// **Validates: Requirements R29.7-R29.8**
        /// </summary>
        [Test]
        public void PurchaseProduct_PremiumIdleMode_SetsHasPremiumIdleMode()
        {
            _monetizationSystem.Initialize();
            
            _monetizationSystem.PurchaseProduct("PremiumIdleMode", _ => { });
            
            Assert.IsTrue(_monetizationSystem.HasPremiumIdleMode);
        }

        /// <summary>
        /// Test that non-consumable cannot be purchased twice.
        /// **Validates: Requirements R28.6**
        /// </summary>
        [Test]
        public void PurchaseProduct_NonConsumableAlreadyOwned_ReturnsFalse()
        {
            _monetizationSystem.Initialize();
            _monetizationSystem.PurchaseProduct("NoAds", _ => { });
            
            PurchaseResult? result = null;
            _monetizationSystem.OnPurchaseComplete += (_, r) => result = r;
            bool? success = null;
            _monetizationSystem.PurchaseProduct("NoAds", s => success = s);
            
            Assert.IsFalse(success);
            Assert.AreEqual(PurchaseResult.AlreadyOwned, result);
        }

        #endregion

        #region Restore Purchases Tests (R28.7)

        /// <summary>
        /// Test that RestorePurchases fails when not initialized.
        /// **Validates: Requirements R28.7**
        /// </summary>
        [Test]
        public void RestorePurchases_NotInitialized_ReturnsFalse()
        {
            bool? result = null;
            
            _monetizationSystem.RestorePurchases(success => result = success);
            
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Test that RestorePurchases succeeds when initialized.
        /// **Validates: Requirements R28.7**
        /// </summary>
        [Test]
        public void RestorePurchases_Initialized_ReturnsTrue()
        {
            _monetizationSystem.Initialize();
            bool? result = null;
            
            _monetizationSystem.RestorePurchases(success => result = success);
            
            Assert.IsTrue(result);
        }

        #endregion

        #region HasPurchased Tests (R28.6)

        /// <summary>
        /// Test that HasPurchased returns false for unpurchased product.
        /// **Validates: Requirements R28.6**
        /// </summary>
        [Test]
        public void HasPurchased_NotPurchased_ReturnsFalse()
        {
            Assert.IsFalse(_monetizationSystem.HasPurchased("NoAds"));
        }

        /// <summary>
        /// Test that HasPurchased returns true after purchase.
        /// **Validates: Requirements R28.6**
        /// </summary>
        [Test]
        public void HasPurchased_AfterPurchase_ReturnsTrue()
        {
            _monetizationSystem.Initialize();
            _monetizationSystem.PurchaseProduct("NoAds", _ => { });
            
            Assert.IsTrue(_monetizationSystem.HasPurchased("NoAds"));
        }

        #endregion

        #region Ad Reward Tests (R28.3)

        /// <summary>
        /// Test that GetAdReward returns correct reward for placement.
        /// **Validates: Requirements R28.3**
        /// </summary>
        [Test]
        public void GetAdReward_ValidPlacement_ReturnsReward()
        {
            var reward = _monetizationSystem.GetAdReward(AdPlacement.OvertimeHours);
            
            Assert.IsNotNull(reward);
            Assert.AreEqual(AdPlacement.OvertimeHours, reward.placement);
            Assert.AreEqual(AdRewardType.OvertimeHours, reward.rewardType);
            Assert.AreEqual(4f, reward.amount);
        }

        /// <summary>
        /// Test that GetAdReward returns emergency funding reward.
        /// **Validates: Requirements R30.3**
        /// </summary>
        [Test]
        public void GetAdReward_EmergencyFunding_ReturnsCorrectReward()
        {
            var reward = _monetizationSystem.GetAdReward(AdPlacement.EmergencyFunding);
            
            Assert.IsNotNull(reward);
            Assert.AreEqual(AdRewardType.EmergencyFunding, reward.rewardType);
            Assert.AreEqual(500f, reward.amount);
        }

        #endregion

        #region Ad Cooldown Tests (R30.4)

        /// <summary>
        /// Test that GetAdCooldown returns 0 for fresh placement.
        /// **Validates: Requirements R30.4**
        /// </summary>
        [Test]
        public void GetAdCooldown_NeverWatched_ReturnsZero()
        {
            var cooldown = _monetizationSystem.GetAdCooldown(AdPlacement.EmergencyFunding);
            
            Assert.AreEqual(0f, cooldown);
        }

        #endregion

        #region Product Data Tests (R29.1-R29.6)

        /// <summary>
        /// Test that currency packs are available.
        /// **Validates: Requirements R29.1**
        /// </summary>
        [Test]
        public void GetProduct_CurrencyPacks_AreAvailable()
        {
            Assert.IsNotNull(_monetizationSystem.GetProduct("CurrencySmall"));
            Assert.IsNotNull(_monetizationSystem.GetProduct("CurrencyMedium"));
            Assert.IsNotNull(_monetizationSystem.GetProduct("CurrencyLarge"));
            Assert.IsNotNull(_monetizationSystem.GetProduct("CurrencyMega"));
        }

        /// <summary>
        /// Test that starter pack is available.
        /// **Validates: Requirements R29.2**
        /// </summary>
        [Test]
        public void GetProduct_StarterPack_IsAvailable()
        {
            var product = _monetizationSystem.GetProduct("StarterPack");
            
            Assert.IsNotNull(product);
            Assert.AreEqual(IAPProductType.NonConsumable, product.productType);
        }

        /// <summary>
        /// Test that permanent unlocks are available.
        /// **Validates: Requirements R29.3**
        /// </summary>
        [Test]
        public void GetProduct_PermanentUnlocks_AreAvailable()
        {
            Assert.IsNotNull(_monetizationSystem.GetProduct("PremiumVenuePack"));
            Assert.IsNotNull(_monetizationSystem.GetProduct("EliteVendorNetwork"));
            Assert.IsNotNull(_monetizationSystem.GetProduct("NoAds"));
        }

        /// <summary>
        /// Test that VIP subscription is available.
        /// **Validates: Requirements R29.4**
        /// </summary>
        [Test]
        public void GetProduct_VIPSubscription_IsAvailable()
        {
            var product = _monetizationSystem.GetProduct("VIPSubscription");
            
            Assert.IsNotNull(product);
            Assert.AreEqual(IAPProductType.Subscription, product.productType);
            Assert.IsTrue(product.grantsVIP);
            Assert.IsTrue(product.grantsAdFree);
            Assert.IsTrue(product.grantsPremiumIdleMode);
        }

        #endregion
    }


    /// <summary>
    /// Unit tests for MonetizationState.
    /// Validates: Requirements R28.6, R28.10, R30.4
    /// </summary>
    [TestFixture]
    public class MonetizationStateTests
    {
        private MonetizationState _state;

        [SetUp]
        public void Setup()
        {
            _state = new MonetizationState();
        }

        #region CanWatchAd Tests (R30.4)

        /// <summary>
        /// Test that CanWatchAd returns true when under limit.
        /// **Validates: Requirements R30.4**
        /// </summary>
        [Test]
        public void CanWatchAd_UnderLimit_ReturnsTrue()
        {
            Assert.IsTrue(_state.CanWatchAd(AdPlacement.OvertimeHours, 2));
        }

        /// <summary>
        /// Test that CanWatchAd returns false when at limit.
        /// **Validates: Requirements R30.4**
        /// </summary>
        [Test]
        public void CanWatchAd_AtLimit_ReturnsFalse()
        {
            _state.RecordAdWatch(AdPlacement.OvertimeHours);
            _state.RecordAdWatch(AdPlacement.OvertimeHours);
            
            Assert.IsFalse(_state.CanWatchAd(AdPlacement.OvertimeHours, 2));
        }

        #endregion

        #region RecordAdWatch Tests (R30.4)

        /// <summary>
        /// Test that RecordAdWatch increments daily count.
        /// **Validates: Requirements R30.4**
        /// </summary>
        [Test]
        public void RecordAdWatch_IncrementsDailyCount()
        {
            _state.RecordAdWatch(AdPlacement.TimeSkip);
            
            Assert.AreEqual(1, _state.GetDailyAdWatchCount(AdPlacement.TimeSkip));
        }

        /// <summary>
        /// Test that RecordAdWatch increments total count.
        /// **Validates: Requirements R28.10**
        /// </summary>
        [Test]
        public void RecordAdWatch_IncrementsTotalCount()
        {
            _state.RecordAdWatch(AdPlacement.TimeSkip);
            _state.RecordAdWatch(AdPlacement.EmergencyFunding);
            
            Assert.AreEqual(2, _state.totalAdsWatched);
        }

        /// <summary>
        /// Test that RecordAdWatch records last watch time.
        /// **Validates: Requirements R30.4**
        /// </summary>
        [Test]
        public void RecordAdWatch_RecordsLastWatchTime()
        {
            var before = DateTime.UtcNow;
            _state.RecordAdWatch(AdPlacement.TimeSkip);
            var after = DateTime.UtcNow;
            
            var lastWatch = _state.GetLastAdWatchTime(AdPlacement.TimeSkip);
            Assert.IsTrue(lastWatch >= before && lastWatch <= after);
        }

        #endregion

        #region ResetDailyCounts Tests (R30.4)

        /// <summary>
        /// Test that ResetDailyCounts clears all counts.
        /// **Validates: Requirements R30.4**
        /// </summary>
        [Test]
        public void ResetDailyCounts_ClearsAllCounts()
        {
            _state.RecordAdWatch(AdPlacement.TimeSkip);
            _state.RecordAdWatch(AdPlacement.OvertimeHours);
            
            _state.ResetDailyCounts();
            
            Assert.AreEqual(0, _state.GetDailyAdWatchCount(AdPlacement.TimeSkip));
            Assert.AreEqual(0, _state.GetDailyAdWatchCount(AdPlacement.OvertimeHours));
        }

        #endregion

        #region Purchase Tracking Tests (R28.6)

        /// <summary>
        /// Test that RecordPurchase adds to purchased list.
        /// **Validates: Requirements R28.6**
        /// </summary>
        [Test]
        public void RecordPurchase_AddsToList()
        {
            _state.RecordPurchase("NoAds");
            
            Assert.IsTrue(_state.HasPurchased("NoAds"));
        }

        /// <summary>
        /// Test that RecordPurchase does not duplicate.
        /// **Validates: Requirements R28.6**
        /// </summary>
        [Test]
        public void RecordPurchase_DoesNotDuplicate()
        {
            _state.RecordPurchase("NoAds");
            _state.RecordPurchase("NoAds");
            
            Assert.AreEqual(1, _state.purchasedProducts.Count);
        }

        #endregion

        #region Status Flag Tests

        /// <summary>
        /// Test that IsAdFree returns true when set.
        /// </summary>
        [Test]
        public void IsAdFree_WhenSet_ReturnsTrue()
        {
            _state.SetAdFree(true);
            
            Assert.IsTrue(_state.IsAdFree);
        }

        /// <summary>
        /// Test that IsAdFree returns true when VIP.
        /// </summary>
        [Test]
        public void IsAdFree_WhenVIP_ReturnsTrue()
        {
            _state.SetVIP(true);
            
            Assert.IsTrue(_state.IsAdFree);
        }

        /// <summary>
        /// Test that HasPremiumIdleMode returns true when VIP.
        /// </summary>
        [Test]
        public void HasPremiumIdleMode_WhenVIP_ReturnsTrue()
        {
            _state.SetVIP(true);
            
            Assert.IsTrue(_state.HasPremiumIdleMode);
        }

        /// <summary>
        /// Test that GetRemainingAdWatches calculates correctly.
        /// </summary>
        [Test]
        public void GetRemainingAdWatches_CalculatesCorrectly()
        {
            _state.RecordAdWatch(AdPlacement.OvertimeHours);
            
            Assert.AreEqual(1, _state.GetRemainingAdWatches(AdPlacement.OvertimeHours, 2));
        }

        #endregion

        #region Clone Tests

        /// <summary>
        /// Test that Clone creates independent copy.
        /// </summary>
        [Test]
        public void Clone_CreatesIndependentCopy()
        {
            _state.SetAdFree(true);
            _state.RecordPurchase("NoAds");
            _state.RecordAdWatch(AdPlacement.TimeSkip);
            
            var clone = _state.Clone();
            
            Assert.IsTrue(clone.IsAdFree);
            Assert.IsTrue(clone.HasPurchased("NoAds"));
            Assert.AreEqual(1, clone.GetDailyAdWatchCount(AdPlacement.TimeSkip));
            
            // Modify original, clone should be unaffected
            _state.RecordAdWatch(AdPlacement.TimeSkip);
            Assert.AreEqual(1, clone.GetDailyAdWatchCount(AdPlacement.TimeSkip));
        }

        #endregion
    }
}
