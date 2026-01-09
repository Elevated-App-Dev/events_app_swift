namespace EventPlannerSim.Core
{
    /// <summary>
    /// Types of random events that can occur during event execution.
    /// Requirements: R12.2-R12.10
    /// </summary>
    public enum RandomEventType
    {
        // Vendor Issues
        VendorNoShow,
        VendorLate,
        VendorUnderperformance,
        
        // Equipment Issues
        EquipmentFailure,
        PowerOutage,
        AVMalfunction,
        
        // Guest Issues
        GuestConflict,
        UnexpectedGuests,
        GuestInjury,
        
        // Weather (outdoor events)
        WeatherChange,
        ExtremeWeather,
        
        // Client Issues
        LastMinuteChanges,
        ClientComplaint,
        BudgetDispute,
        
        // Positive Events (rare)
        UnexpectedCompliment,
        MediaCoverage,
        CelebrityAppearance
    }
}
