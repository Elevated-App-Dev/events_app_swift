using System;
using EventPlannerSim.Core;

namespace EventPlannerSim.Systems
{
    /// <summary>
    /// Manages marketing investments and their effects on client inquiry rates.
    /// This is a stub interface for MVP - full implementation is Post-MVP.
    /// Requirements: R22.1-R22.11
    /// </summary>
    public interface IMarketingSystem
    {
        /// <summary>
        /// Get the current marketing tier.
        /// Requirements: R22.3
        /// </summary>
        MarketingTier CurrentTier { get; }

        /// <summary>
        /// Check if marketing system is available for the player.
        /// Only available in Stage 3+ on Entrepreneur Path.
        /// Requirements: R22.1
        /// </summary>
        bool IsMarketingAvailable(BusinessStage stage, CareerPath careerPath);

        /// <summary>
        /// Get the inquiry rate modifier based on current marketing tier.
        /// Returns 1.0 for no effect, >1.0 for increased inquiries.
        /// Requirements: R22.2-R22.5
        /// </summary>
        float GetInquiryRateModifier();

        /// <summary>
        /// Set the marketing tier (None, Basic, Standard, Premium).
        /// Requirements: R22.3
        /// </summary>
        bool SetMarketingTier(MarketingTier tier);

        /// <summary>
        /// Get the weekly cost for a marketing tier.
        /// Requirements: R22.3
        /// </summary>
        float GetWeeklyCost(MarketingTier tier);

        /// <summary>
        /// Get the current weekly marketing cost.
        /// Requirements: R22.6
        /// </summary>
        float GetCurrentWeeklyCost();

        /// <summary>
        /// Process weekly marketing costs and handle insufficient funds.
        /// Requirements: R22.6, R22.7
        /// </summary>
        MarketingPaymentResult ProcessWeeklyPayment(float availableFunds);

        /// <summary>
        /// Get marketing ROI statistics.
        /// Requirements: R22.8
        /// </summary>
        MarketingROI GetROIStatistics();

        /// <summary>
        /// Check if targeted marketing is available (Stage 4+).
        /// Requirements: R22.9
        /// </summary>
        bool IsTargetedMarketingAvailable(BusinessStage stage);

        /// <summary>
        /// Set targeted marketing focus (Corporate, Wedding, etc.).
        /// Requirements: R22.9
        /// </summary>
        bool SetTargetedFocus(MarketingFocus focus);

        /// <summary>
        /// Get the current targeted marketing focus.
        /// Requirements: R22.9
        /// </summary>
        MarketingFocus CurrentFocus { get; }

        /// <summary>
        /// Pause marketing without canceling.
        /// Requirements: R22.10
        /// </summary>
        void PauseMarketing();

        /// <summary>
        /// Resume paused marketing.
        /// Requirements: R22.10
        /// </summary>
        void ResumeMarketing();

        /// <summary>
        /// Check if marketing is currently paused.
        /// </summary>
        bool IsPaused { get; }

        /// <summary>
        /// Event fired when marketing tier changes.
        /// </summary>
        event Action<MarketingTier> OnTierChanged;

        /// <summary>
        /// Event fired when marketing is auto-downgraded due to insufficient funds.
        /// </summary>
        event Action<MarketingTier, MarketingTier> OnAutoDowngraded;
    }

    /// <summary>
    /// Marketing investment tiers.
    /// </summary>
    public enum MarketingTier
    {
        None,       // No marketing - 70% base inquiry rate in Stage 3+
        Basic,      // $500/week - restores base inquiry rate
        Standard,   // $1,500/week - +25% above base
        Premium     // $3,000/week - +50% above base
    }

    /// <summary>
    /// Targeted marketing focus options (Stage 4+).
    /// </summary>
    public enum MarketingFocus
    {
        General,
        Corporate,
        Wedding,
        Social,
        Luxury
    }

    /// <summary>
    /// Result of weekly marketing payment processing.
    /// </summary>
    [Serializable]
    public class MarketingPaymentResult
    {
        public bool PaymentSuccessful;
        public float AmountPaid;
        public bool WasDowngraded;
        public MarketingTier PreviousTier;
        public MarketingTier NewTier;
    }

    /// <summary>
    /// Marketing ROI statistics for display in Bank app.
    /// </summary>
    [Serializable]
    public class MarketingROI
    {
        public float TotalSpent;
        public int InquiriesGenerated;
        public int EventsBooked;
        public float RevenueGenerated;
        public float ROIPercentage;
    }
}
