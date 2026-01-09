namespace EventPlannerSim.Core
{
    /// <summary>
    /// Represents the type of venue available for events.
    /// </summary>
    public enum VenueType
    {
        // Neighborhood (Stage 1)
        Backyard,
        CommunityCenter,
        ParkPavilion,
        
        // Downtown (Stage 2+)
        Hotel,
        Restaurant,
        SmallBallroom,
        
        // Large (Stage 3+)
        ConventionCenter,
        ConferenceHotel,
        
        // Uptown (Stage 4+)
        LuxuryHotel,
        Estate,
        Rooftop,
        
        // Waterfront (Stage 5)
        Beach,
        GardenEstate
    }
}
