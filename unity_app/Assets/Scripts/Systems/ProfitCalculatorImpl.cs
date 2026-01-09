using System;
using EventPlannerSim.Core;
using EventPlannerSim.Data;

namespace EventPlannerSim.Systems
{
    /// <summary>
    /// Implementation of profit margin and commission calculations.
    /// Requirements: R33.1-R33.3, R16.3
    /// </summary>
    public class ProfitCalculatorImpl : IProfitCalculator
    {
        // Profit margin constants
        private const float SuccessfulMinMargin = 0.20f;  // 20%
        private const float SuccessfulMaxMargin = 0.30f;  // 30%
        private const float MediocreMinMargin = 0.10f;    // 10%
        private const float MediocreMaxMargin = 0.15f;    // 15%
        
        // Satisfaction thresholds
        private const float SuccessfulThreshold = 70f;
        private const float MediocreThreshold = 50f;

        // Random for margin variation within ranges
        private readonly Random _random;

        public ProfitCalculatorImpl()
        {
            _random = new Random();
        }

        public ProfitCalculatorImpl(int seed)
        {
            _random = new Random(seed);
        }

        /// <summary>
        /// Calculate profit for an event based on budget and satisfaction.
        /// Requirements: R33.1-R33.3
        /// </summary>
        public ProfitResult CalculateProfit(float eventBudget, float satisfaction)
        {
            // Clamp satisfaction to valid range
            satisfaction = Math.Clamp(satisfaction, 0f, 100f);
            
            // Ensure budget is non-negative
            eventBudget = Math.Max(0f, eventBudget);

            var result = new ProfitResult
            {
                EventBudget = eventBudget,
                SatisfactionScore = satisfaction
            };

            if (satisfaction >= SuccessfulThreshold)
            {
                // R33.1: 70%+ satisfaction = 20-30% profit margin
                result.ProfitTier = "Successful";
                result.ProfitMarginPercent = CalculateMarginInRange(
                    satisfaction, 
                    SuccessfulThreshold, 
                    100f, 
                    SuccessfulMinMargin, 
                    SuccessfulMaxMargin);
            }
            else if (satisfaction >= MediocreThreshold)
            {
                // R33.2: 50-69% satisfaction = 10-15% profit margin
                result.ProfitTier = "Mediocre";
                result.ProfitMarginPercent = CalculateMarginInRange(
                    satisfaction, 
                    MediocreThreshold, 
                    SuccessfulThreshold - 0.01f, 
                    MediocreMinMargin, 
                    MediocreMaxMargin);
            }
            else
            {
                // R33.3: Below 50% = Break-even or loss
                result.ProfitTier = "Failed";
                // Scale from 0% at 50% satisfaction to -20% at 0% satisfaction
                result.ProfitMarginPercent = CalculateLossMargin(satisfaction);
            }

            result.ProfitAmount = eventBudget * result.ProfitMarginPercent;

            return result;
        }

        /// <summary>
        /// Calculate employee commission for a Stage 2 company event.
        /// Requirements: R16.3
        /// </summary>
        public CommissionResult CalculateCommission(int employeeLevel, float eventBudget, float satisfaction)
        {
            // Clamp employee level to valid range
            employeeLevel = Math.Clamp(employeeLevel, 1, 5);

            // Get compensation structure for level
            var (basePay, commissionRate) = GetCompensationByLevel(employeeLevel);

            // Calculate event profit first
            var profitResult = CalculateProfit(eventBudget, satisfaction);

            // Commission is based on event profit (not budget)
            // Only positive profits generate commission
            float commissionableProfit = Math.Max(0f, profitResult.ProfitAmount);
            float commissionAmount = commissionableProfit * commissionRate;

            return new CommissionResult
            {
                BasePay = basePay,
                CommissionRate = commissionRate,
                EventProfit = profitResult.ProfitAmount,
                CommissionAmount = commissionAmount,
                TotalCompensation = basePay + commissionAmount,
                EmployeeLevel = employeeLevel
            };
        }

        /// <summary>
        /// Get the profit margin range for a given satisfaction level.
        /// </summary>
        public (float min, float max) GetProfitMarginRange(float satisfaction)
        {
            satisfaction = Math.Clamp(satisfaction, 0f, 100f);

            if (satisfaction >= SuccessfulThreshold)
            {
                return (SuccessfulMinMargin, SuccessfulMaxMargin);
            }
            else if (satisfaction >= MediocreThreshold)
            {
                return (MediocreMinMargin, MediocreMaxMargin);
            }
            else
            {
                // For failed events, return the loss range
                // At 0% satisfaction: -20% margin
                // At 49% satisfaction: ~0% margin
                float maxLoss = -0.20f;
                float minLoss = 0f;
                return (maxLoss, minLoss);
            }
        }

        /// <summary>
        /// Get the base pay and commission rate for an employee level.
        /// Requirements: R16.3
        /// </summary>
        public (float basePay, float commissionRate) GetCompensationByLevel(int employeeLevel)
        {
            return employeeLevel switch
            {
                1 or 2 => (500f, 0.05f),   // Junior Planner: $500 base + 5% commission
                3 or 4 => (750f, 0.10f),   // Planner: $750 base + 10% commission
                5 => (1000f, 0.15f),       // Senior Planner: $1000 base + 15% commission
                _ => (500f, 0.05f)         // Default to Junior
            };
        }

        /// <summary>
        /// Calculate margin within a range based on satisfaction score.
        /// Higher satisfaction within the tier = higher margin.
        /// </summary>
        private float CalculateMarginInRange(float satisfaction, float minSat, float maxSat, float minMargin, float maxMargin)
        {
            // Normalize satisfaction within the tier range
            float normalizedSat = (satisfaction - minSat) / (maxSat - minSat);
            normalizedSat = Math.Clamp(normalizedSat, 0f, 1f);

            // Linear interpolation between min and max margin
            return minMargin + (normalizedSat * (maxMargin - minMargin));
        }

        /// <summary>
        /// Calculate loss margin for failed events (satisfaction < 50%).
        /// Scales from 0% at 50% satisfaction to -20% at 0% satisfaction.
        /// </summary>
        private float CalculateLossMargin(float satisfaction)
        {
            // At 50% satisfaction: 0% margin (break-even)
            // At 0% satisfaction: -20% margin (loss)
            float normalizedSat = satisfaction / MediocreThreshold;
            normalizedSat = Math.Clamp(normalizedSat, 0f, 1f);

            // Interpolate from -20% to 0%
            const float maxLoss = -0.20f;
            return maxLoss * (1f - normalizedSat);
        }
    }
}
