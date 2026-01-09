using EventPlannerSim.Core;
using EventPlannerSim.Data;

namespace EventPlannerSim.Systems
{
    /// <summary>
    /// Result of a profit calculation.
    /// </summary>
    public class ProfitResult
    {
        /// <summary>
        /// The calculated profit amount (can be negative for losses).
        /// </summary>
        public float ProfitAmount { get; set; }

        /// <summary>
        /// The profit margin percentage (0-30% typically).
        /// </summary>
        public float ProfitMarginPercent { get; set; }

        /// <summary>
        /// Whether the event was profitable.
        /// </summary>
        public bool IsProfitable => ProfitAmount > 0;

        /// <summary>
        /// Whether the event broke even (no profit, no loss).
        /// </summary>
        public bool IsBreakEven => ProfitAmount == 0;

        /// <summary>
        /// Whether the event resulted in a loss.
        /// </summary>
        public bool IsLoss => ProfitAmount < 0;

        /// <summary>
        /// The original event budget.
        /// </summary>
        public float EventBudget { get; set; }

        /// <summary>
        /// The satisfaction score that determined the profit tier.
        /// </summary>
        public float SatisfactionScore { get; set; }

        /// <summary>
        /// Description of the profit tier (Successful, Mediocre, Failed).
        /// </summary>
        public string ProfitTier { get; set; }
    }

    /// <summary>
    /// Result of a commission calculation for Stage 2 employees.
    /// </summary>
    public class CommissionResult
    {
        /// <summary>
        /// Base pay for the employee level.
        /// </summary>
        public float BasePay { get; set; }

        /// <summary>
        /// Commission rate as a decimal (0.05 = 5%).
        /// </summary>
        public float CommissionRate { get; set; }

        /// <summary>
        /// The event profit used to calculate commission.
        /// </summary>
        public float EventProfit { get; set; }

        /// <summary>
        /// The calculated commission amount.
        /// </summary>
        public float CommissionAmount { get; set; }

        /// <summary>
        /// Total compensation (base pay + commission).
        /// </summary>
        public float TotalCompensation { get; set; }

        /// <summary>
        /// The employee level used for calculation.
        /// </summary>
        public int EmployeeLevel { get; set; }
    }

    /// <summary>
    /// Calculates profit margins and employee commissions.
    /// Requirements: R33.1-R33.3, R16.3
    /// </summary>
    public interface IProfitCalculator
    {
        /// <summary>
        /// Calculate profit for an event based on budget and satisfaction.
        /// Requirements: R33.1-R33.3
        /// - 70%+ satisfaction: 20-30% profit margin
        /// - 50-69% satisfaction: 10-15% profit margin
        /// - Below 50%: Break-even or loss
        /// </summary>
        /// <param name="eventBudget">The total event budget</param>
        /// <param name="satisfaction">Client satisfaction score (0-100)</param>
        /// <returns>Profit calculation result</returns>
        ProfitResult CalculateProfit(float eventBudget, float satisfaction);

        /// <summary>
        /// Calculate employee commission for a Stage 2 company event.
        /// Requirements: R16.3
        /// - Junior (Level 1-2): $500 base + 5% commission
        /// - Planner (Level 3-4): $750 base + 10% commission
        /// - Senior (Level 5): $1000 base + 15% commission
        /// </summary>
        /// <param name="employeeLevel">Employee level (1-5)</param>
        /// <param name="eventBudget">The total event budget</param>
        /// <param name="satisfaction">Client satisfaction score (0-100)</param>
        /// <returns>Commission calculation result</returns>
        CommissionResult CalculateCommission(int employeeLevel, float eventBudget, float satisfaction);

        /// <summary>
        /// Get the profit margin range for a given satisfaction level.
        /// </summary>
        /// <param name="satisfaction">Client satisfaction score (0-100)</param>
        /// <returns>Tuple of (minMargin, maxMargin) as percentages (0-1)</returns>
        (float min, float max) GetProfitMarginRange(float satisfaction);

        /// <summary>
        /// Get the base pay and commission rate for an employee level.
        /// </summary>
        /// <param name="employeeLevel">Employee level (1-5)</param>
        /// <returns>Tuple of (basePay, commissionRate)</returns>
        (float basePay, float commissionRate) GetCompensationByLevel(int employeeLevel);
    }
}
