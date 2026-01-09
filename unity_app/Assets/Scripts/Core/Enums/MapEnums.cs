namespace EventPlannerSim.Core
{
    /// <summary>
    /// Represents a geographic area of the city map containing related venues and vendors.
    /// </summary>
    public enum MapZone
    {
        Neighborhood,
        Downtown,
        Uptown,
        Waterfront
    }

    /// <summary>
    /// Represents the type of location on the map.
    /// </summary>
    public enum LocationType
    {
        All,
        Venue,
        Vendor,
        Office,
        MeetingPoint
    }
}
