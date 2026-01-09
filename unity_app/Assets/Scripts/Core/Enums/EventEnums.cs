namespace EventPlannerSim.Core
{
    /// <summary>
    /// Represents the current status of an event in its lifecycle.
    /// </summary>
    public enum EventStatus
    {
        Inquiry,
        Accepted,
        Planning,
        Executing,
        Completed,
        Cancelled
    }

    /// <summary>
    /// Represents the complexity level of an event type.
    /// </summary>
    public enum EventComplexity
    {
        Low,
        Medium,
        High,
        VeryHigh
    }

    /// <summary>
    /// Represents the current workload status based on active event count.
    /// </summary>
    public enum WorkloadStatus
    {
        Optimal,
        Comfortable,
        Strained,
        Critical
    }

    /// <summary>
    /// Represents the current phase of event planning/execution.
    /// </summary>
    public enum EventPhase
    {
        Booking,
        PrePlanning,
        ActivePlanning,
        FinalPrep,
        ExecutionDay,
        Results
    }

    /// <summary>
    /// Represents the budget categories for event allocation.
    /// </summary>
    public enum BudgetCategory
    {
        Venue,
        Catering,
        Entertainment,
        Decorations,
        Staffing,
        Contingency,
        Photography,
        Florals,
        Rentals,
        AudioVisual,
        Transportation,
        Security
    }
}
