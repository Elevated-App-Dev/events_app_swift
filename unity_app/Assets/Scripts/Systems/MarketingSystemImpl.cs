using System;
using EventPlannerSim.Core;

namespace EventPlannerSim.Systems
{
    /// <summary>
    /// Stub implementation of IMarketingSystem for MVP.
    /// Returns 1.0 modifier (no effect) - full implementation is Post-MVP.
    /// Requirements: R22.1-R22.11
    /// </summary>
    public class MarketingSystemImpl : IMarketingSystem
    {
        private MarketingTier _currentTier = MarketingTier.None;
        private MarketingFocus _currentFocus = MarketingFocus.General;
        private bool _isPaused = false;
        private readonly MarketingROI _emptyROI = new MarketingROI();

        /// <inheritdoc />
        public event Action<MarketingTier> OnTierChanged;

        /// <inheritdoc />
        public event Action<MarketingTier, MarketingTier> OnAutoDowngraded;

        /// <inheritdoc />
        public MarketingTier CurrentTier => _currentTier;

        /// <inheritdoc />
        public MarketingFocus CurrentFocus => _currentFocus;

        /// <inheritdoc />
        public bool IsPaused => _isPaused;

        /// <inheritdoc />
        public bool IsMarketingAvailable(BusinessStage stage, CareerPath careerPath)
        {
            // MVP stub: Marketing not available
            // Full implementation: Stage 3+ on Entrepreneur Path only
            return false;
        }

        /// <inheritdoc />
        public float GetInquiryRateModifier()
        {
            // MVP stub: Returns 1.0 (no modification to inquiry rate)
            // Full implementation would return:
            // - 0.7 for None tier in Stage 3+ (30% reduction)
            // - 1.0 for Basic tier (restores base rate)
            // - 1.25 for Standard tier (+25% above base)
            // - 1.5 for Premium tier (+50% above base)
            return 1.0f;
        }

        /// <inheritdoc />
        public bool SetMarketingTier(MarketingTier tier)
        {
            // MVP stub: Marketing tier changes not available
            return false;
        }

        /// <inheritdoc />
        public float GetWeeklyCost(MarketingTier tier)
        {
            // Return the defined costs even in MVP for reference
            return tier switch
            {
                MarketingTier.None => 0f,
                MarketingTier.Basic => 500f,
                MarketingTier.Standard => 1500f,
                MarketingTier.Premium => 3000f,
                _ => 0f
            };
        }

        /// <inheritdoc />
        public float GetCurrentWeeklyCost()
        {
            // MVP stub: No marketing costs
            return 0f;
        }

        /// <inheritdoc />
        public MarketingPaymentResult ProcessWeeklyPayment(float availableFunds)
        {
            // MVP stub: No payment processing needed
            return new MarketingPaymentResult
            {
                PaymentSuccessful = true,
                AmountPaid = 0f,
                WasDowngraded = false,
                PreviousTier = _currentTier,
                NewTier = _currentTier
            };
        }

        /// <inheritdoc />
        public MarketingROI GetROIStatistics()
        {
            // MVP stub: Empty ROI statistics
            return _emptyROI;
        }

        /// <inheritdoc />
        public bool IsTargetedMarketingAvailable(BusinessStage stage)
        {
            // MVP stub: Targeted marketing not available
            // Full implementation: Stage 4+ only
            return false;
        }

        /// <inheritdoc />
        public bool SetTargetedFocus(MarketingFocus focus)
        {
            // MVP stub: Targeted focus changes not available
            return false;
        }

        /// <inheritdoc />
        public void PauseMarketing()
        {
            // MVP stub: No-op since marketing is not active
            _isPaused = true;
        }

        /// <inheritdoc />
        public void ResumeMarketing()
        {
            // MVP stub: No-op since marketing is not active
            _isPaused = false;
        }

        // Helper methods to suppress unused event warnings
        protected virtual void RaiseTierChanged(MarketingTier tier) => OnTierChanged?.Invoke(tier);
        protected virtual void RaiseAutoDowngraded(MarketingTier previous, MarketingTier current) => OnAutoDowngraded?.Invoke(previous, current);
    }
}
