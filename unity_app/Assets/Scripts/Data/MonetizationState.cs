using System;
using System.Collections.Generic;
using EventPlannerSim.Core;

namespace EventPlannerSim.Data
{
    /// <summary>
    /// Tracks the player's monetization state including ad watches and purchases.
    /// Requirements: R28.6, R28.10, R30.4
    /// </summary>
    [Serializable]
    public class MonetizationState
    {
        /// <summary>
        /// Whether the player has ad-free status (from purchase or VIP).
        /// </summary>
        private bool _isAdFree;

        /// <summary>
        /// Whether the player has active VIP subscription.
        /// </summary>
        private bool _isVIP;

        /// <summary>
        /// Whether the player has Premium Idle Mode.
        /// </summary>
        private bool _hasPremiumIdleMode;

        /// <summary>
        /// Whether the player is on the corporate path (affects interstitial ads).
        /// Requirements: R30.9
        /// </summary>
        public bool IsOnCorporatePath;

        /// <summary>
        /// List of purchased non-consumable product IDs.
        /// Requirements: R28.6
        /// </summary>
        public List<string> purchasedProducts = new List<string>();

        /// <summary>
        /// Daily ad watch counts by placement.
        /// Requirements: R30.4
        /// </summary>
        public Dictionary<AdPlacement, int> dailyAdWatchCounts = new Dictionary<AdPlacement, int>();

        /// <summary>
        /// Last ad watch time by placement (for cooldown tracking).
        /// Requirements: R30.4
        /// </summary>
        public Dictionary<AdPlacement, long> lastAdWatchTicks = new Dictionary<AdPlacement, long>();

        /// <summary>
        /// The date of the last daily reset (for tracking when to reset counts).
        /// </summary>
        public long lastDailyResetTicks;

        /// <summary>
        /// Total number of ads watched (for analytics).
        /// Requirements: R28.10
        /// </summary>
        public int totalAdsWatched;

        /// <summary>
        /// Total amount spent on IAP (for analytics).
        /// Requirements: R28.10
        /// </summary>
        public float totalSpent;

        /// <summary>
        /// VIP subscription expiry time (if applicable).
        /// </summary>
        public long vipExpiryTicks;

        // Properties
        public bool IsAdFree => _isAdFree || _isVIP;
        public bool IsVIP => _isVIP && DateTime.UtcNow.Ticks < vipExpiryTicks;
        public bool HasPremiumIdleMode => _hasPremiumIdleMode || IsVIP;

        public MonetizationState()
        {
            lastDailyResetTicks = DateTime.UtcNow.Date.Ticks;
        }

        /// <summary>
        /// Check if the player can watch an ad for the specified placement.
        /// Requirements: R30.4
        /// </summary>
        /// <param name="placement">The ad placement to check</param>
        /// <param name="dailyLimit">The daily limit for this placement</param>
        /// <returns>True if the player can watch the ad</returns>
        public bool CanWatchAd(AdPlacement placement, int dailyLimit)
        {
            // Check if daily reset is needed
            CheckDailyReset();

            // Get current count for this placement
            if (!dailyAdWatchCounts.TryGetValue(placement, out int count))
                count = 0;

            return count < dailyLimit;
        }

        /// <summary>
        /// Record that an ad was watched for the specified placement.
        /// Requirements: R30.4
        /// </summary>
        public void RecordAdWatch(AdPlacement placement)
        {
            // Check if daily reset is needed
            CheckDailyReset();

            // Increment daily count
            if (!dailyAdWatchCounts.ContainsKey(placement))
                dailyAdWatchCounts[placement] = 0;
            dailyAdWatchCounts[placement]++;

            // Record last watch time
            lastAdWatchTicks[placement] = DateTime.UtcNow.Ticks;

            // Increment total count
            totalAdsWatched++;
        }

        /// <summary>
        /// Get the last time an ad was watched for the specified placement.
        /// </summary>
        public DateTime GetLastAdWatchTime(AdPlacement placement)
        {
            if (lastAdWatchTicks.TryGetValue(placement, out long ticks))
                return new DateTime(ticks);
            return DateTime.MinValue;
        }

        /// <summary>
        /// Get the number of ads watched today for the specified placement.
        /// </summary>
        public int GetDailyAdWatchCount(AdPlacement placement)
        {
            CheckDailyReset();
            return dailyAdWatchCounts.TryGetValue(placement, out int count) ? count : 0;
        }

        /// <summary>
        /// Reset daily ad watch counts.
        /// Requirements: R30.4
        /// </summary>
        public void ResetDailyCounts()
        {
            dailyAdWatchCounts.Clear();
            lastDailyResetTicks = DateTime.UtcNow.Date.Ticks;
        }

        /// <summary>
        /// Check if a daily reset is needed and perform it if so.
        /// </summary>
        private void CheckDailyReset()
        {
            var today = DateTime.UtcNow.Date.Ticks;
            if (today > lastDailyResetTicks)
            {
                ResetDailyCounts();
            }
        }

        /// <summary>
        /// Record a purchase.
        /// Requirements: R28.6
        /// </summary>
        public void RecordPurchase(string productId)
        {
            if (!purchasedProducts.Contains(productId))
                purchasedProducts.Add(productId);
        }

        /// <summary>
        /// Check if a product has been purchased.
        /// Requirements: R28.6
        /// </summary>
        public bool HasPurchased(string productId)
        {
            return purchasedProducts.Contains(productId);
        }

        /// <summary>
        /// Set ad-free status.
        /// </summary>
        public void SetAdFree(bool value)
        {
            _isAdFree = value;
        }

        /// <summary>
        /// Set VIP status with expiry.
        /// </summary>
        public void SetVIP(bool value, DateTime? expiry = null)
        {
            _isVIP = value;
            if (value && expiry.HasValue)
                vipExpiryTicks = expiry.Value.Ticks;
            else if (value)
                vipExpiryTicks = DateTime.MaxValue.Ticks; // Permanent for testing
        }

        /// <summary>
        /// Set Premium Idle Mode status.
        /// </summary>
        public void SetPremiumIdleMode(bool value)
        {
            _hasPremiumIdleMode = value;
        }

        /// <summary>
        /// Record spending amount (for analytics).
        /// Requirements: R28.10
        /// </summary>
        public void RecordSpending(float amount)
        {
            totalSpent += amount;
        }

        /// <summary>
        /// Get remaining ad watches for a placement today.
        /// </summary>
        public int GetRemainingAdWatches(AdPlacement placement, int dailyLimit)
        {
            CheckDailyReset();
            var count = GetDailyAdWatchCount(placement);
            return Math.Max(0, dailyLimit - count);
        }

        /// <summary>
        /// Create a copy of this state for save/load.
        /// </summary>
        public MonetizationState Clone()
        {
            return new MonetizationState
            {
                _isAdFree = _isAdFree,
                _isVIP = _isVIP,
                _hasPremiumIdleMode = _hasPremiumIdleMode,
                IsOnCorporatePath = IsOnCorporatePath,
                purchasedProducts = new List<string>(purchasedProducts),
                dailyAdWatchCounts = new Dictionary<AdPlacement, int>(dailyAdWatchCounts),
                lastAdWatchTicks = new Dictionary<AdPlacement, long>(lastAdWatchTicks),
                lastDailyResetTicks = lastDailyResetTicks,
                totalAdsWatched = totalAdsWatched,
                totalSpent = totalSpent,
                vipExpiryTicks = vipExpiryTicks
            };
        }
    }
}
