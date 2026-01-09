using System.Collections.Generic;
using EventPlannerSim.Core;
using EventPlannerSim.Data;

namespace EventPlannerSim.Systems
{
    /// <summary>
    /// Manages the city map UI, zone navigation, and location discovery.
    /// Requirements: R3.1-R3.10
    /// </summary>
    public interface IMapSystem
    {
        /// <summary>
        /// Get zones currently visible/unlocked for the player based on their stage.
        /// Stage 1: Neighborhood only
        /// Stage 2: Neighborhood + Downtown (employer network)
        /// Stage 3: + Downtown fully unlocked
        /// Stage 4: + Uptown
        /// Stage 5: + Waterfront
        /// </summary>
        List<MapZone> GetVisibleZones(int stage);

        /// <summary>
        /// Get all locations (venues, vendors) in a specific zone.
        /// </summary>
        List<LocationData> GetLocationsInZone(MapZone zone);

        /// <summary>
        /// Navigate to a specific zone, triggering zoom animation.
        /// </summary>
        void NavigateToZone(MapZone zone);

        /// <summary>
        /// Navigate to a specific location within a zone.
        /// </summary>
        void NavigateToLocation(string locationId);

        /// <summary>
        /// Filter visible location pins by type.
        /// </summary>
        void SetLocationFilter(LocationType filter);

        /// <summary>
        /// Get the current location filter.
        /// </summary>
        LocationType CurrentFilter { get; }

        /// <summary>
        /// Get the preview card data for a location.
        /// </summary>
        LocationPreviewData GetLocationPreview(string locationId);

        /// <summary>
        /// Check if a zone is unlocked for the given stage.
        /// </summary>
        bool IsZoneUnlocked(MapZone zone, int stage);

        /// <summary>
        /// Get the player's current office location.
        /// Returns null if player doesn't have an office yet (Stage 1).
        /// </summary>
        LocationData GetPlayerOffice(int stage);

        /// <summary>
        /// Get the currently selected/focused zone.
        /// </summary>
        MapZone? CurrentZone { get; }

        /// <summary>
        /// Register a location in the map system.
        /// </summary>
        void RegisterLocation(LocationData location);

        /// <summary>
        /// Register a venue as a location.
        /// </summary>
        void RegisterVenue(VenueData venue);

        /// <summary>
        /// Register a vendor as a location.
        /// </summary>
        void RegisterVendor(VendorData vendor);
    }
}
