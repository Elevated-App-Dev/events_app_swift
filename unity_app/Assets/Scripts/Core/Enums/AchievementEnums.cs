namespace EventPlannerSim.Core
{
    /// <summary>
    /// Represents the type of achievement that can be earned.
    /// </summary>
    public enum AchievementType
    {
        // Progression
        FirstSteps,
        RisingStar,
        GoingPro,
        IndustryLeader,
        EventPlanningMogul,
        
        // Mastery
        PerfectPlanner,
        ConsistencyIsKey,
        ExcellenceStreak,
        BudgetMaster,
        VendorWhisperer,
        WeatherWatcher,
        
        // Challenge
        PerfectionistsPerfectionist,
        JugglingAct,
        CrisisManager,
        SelfMade,
        CorporateClimber,
        CelebrityHandler,
        
        // Secret
        AboveAndBeyond,
        FamilyFirst,
        ComebackKid
    }

    /// <summary>
    /// Represents the category of an achievement.
    /// </summary>
    public enum AchievementCategory
    {
        Progression,
        Mastery,
        Challenge,
        Secret
    }
}
