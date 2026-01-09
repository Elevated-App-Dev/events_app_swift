namespace EventPlannerSim.Core
{
    /// <summary>
    /// Represents the player's current career level determining available content and mechanics.
    /// </summary>
    public enum BusinessStage
    {
        Solo = 1,
        Employee = 2,
        SmallCompany = 3,
        Established = 4,
        Premier = 5
    }

    /// <summary>
    /// Represents the career path chosen at Stage 3 milestone.
    /// </summary>
    public enum CareerPath
    {
        None,
        Entrepreneur,
        Corporate
    }

    /// <summary>
    /// Represents the employee level in Stage 2.
    /// Level 1-2 = Junior, 3-4 = Planner, 5 = Senior
    /// </summary>
    public enum EmployeeLevel
    {
        Junior = 1,
        Planner = 3,
        Senior = 5
    }
}
