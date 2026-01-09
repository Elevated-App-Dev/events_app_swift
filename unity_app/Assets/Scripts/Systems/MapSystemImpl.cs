using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using EventPlannerSim.Core;
using EventPlannerSim.Data;

namespace EventPlannerSim.Systems
{
    /// <summary>
    /// Implementation of the Map System for city navigation and location discovery.
    /// Requirements: R3.1-R3.10
    /// </summary>
    public class MapSystemImpl : IMapSystem
    {
        private readonly Dictionary<string, LocationData> _locations;
        private readonly Dictionary<string, VenueData> _venues;
        private readonly Dictionary<string, VendorData> _vendors;
        private LocationType _currentFilter;
        private MapZone? _currentZone;

        // Events for UI integration
        public event Action<MapZone> OnZoneNavigated;
        public event Action<string> OnLocationNavigated;
        public event Action<LocationType> OnFilterChanged;

        public LocationType CurrentFilter => _currentFilter;
        public MapZone? CurrentZone => _currentZone;

        public MapSystemImpl()
        {
            _locations = new Dictionary<string, LocationData>();
            _venues = new Dictionary<string, VenueData>();
            _vendors = new Dictionary<string, VendorData>();
            _currentFilter = LocationType.All;
            _currentZone = null;
        }

        /// <summary>
        /// Get zones currently visible/unlocked for the player based on their stage.
        /// Requirements: R3.5-R3.10
        /// Stage 1: Neighborhood only
        /// Stage 2: Neighborhood + Downtown (employer network access)
        /// Stage 3: Downtown fully unlocked
        /// Stage 4: + Uptown
        /// Stage 5: + Waterfront
        /// </summary>
        public List<MapZone> GetVisibleZones(int stage)
        {
            // Clamp stage to valid range
            stage = Math.Clamp(stage, 1, 5);

            var zones = new List<MapZone>();

            // Stage 1+: Neighborhood always available
            zones.Add(MapZone.Neighborhood);

            // Stage 2+: Downtown (employer network in Stage 2, full access Stage 3+)
            if (stage >= 2)
            {
                zones.Add(MapZone.Downtown);
            }

            // Stage 4+: Uptown
            if (stage >= 4)
            {
                zones.Add(MapZone.Uptown);
            }

            // Stage 5: Waterfront
            if (stage >= 5)
            {
                zones.Add(MapZone.Waterfront);
            }

            return zones;
        }

        /// <summary>
        /// Get all locations in a specific zone, filtered by current filter.
        /// Requirements: R3.1, R3.2
        /// </summary>
        public List<LocationData> GetLocationsInZone(MapZone zone)
        {
            var locationsInZone = _locations.Values
                .Where(loc => loc.zone == zone)
                .ToList();

            // Apply current filter
            if (_currentFilter != LocationType.All)
            {
                locationsInZone = locationsInZone
                    .Where(loc => loc.locationType == _currentFilter)
                    .ToList();
            }

            return locationsInZone;
        }

        /// <summary>
        /// Navigate to a specific zone, triggering zoom animation.
        /// Requirements: R3.2
        /// </summary>
        public void NavigateToZone(MapZone zone)
        {
            _currentZone = zone;
            OnZoneNavigated?.Invoke(zone);
        }

        /// <summary>
        /// Navigate to a specific location within a zone.
        /// Requirements: R3.4
        /// </summary>
        public void NavigateToLocation(string locationId)
        {
            if (string.IsNullOrEmpty(locationId))
                return;

            if (_locations.TryGetValue(locationId, out var location))
            {
                // First navigate to the zone if not already there
                if (_currentZone != location.zone)
                {
                    NavigateToZone(location.zone);
                }
                
                OnLocationNavigated?.Invoke(locationId);
            }
        }

        /// <summary>
        /// Filter visible location pins by type.
        /// Requirements: R3.2, R3.7
        /// </summary>
        public void SetLocationFilter(LocationType filter)
        {
            _currentFilter = filter;
            OnFilterChanged?.Invoke(filter);
        }

        /// <summary>
        /// Get the preview card data for a location.
        /// Requirements: R3.3
        /// </summary>
        public LocationPreviewData GetLocationPreview(string locationId)
        {
            if (string.IsNullOrEmpty(locationId))
                return null;

            if (!_locations.TryGetValue(locationId, out var location))
                return null;

            // Check if this is a venue location
            if (!string.IsNullOrEmpty(location.venueId) && _venues.TryGetValue(location.venueId, out var venue))
            {
                return LocationPreviewData.FromVenue(venue);
            }

            // Check if this is a vendor location
            if (!string.IsNullOrEmpty(location.vendorId) && _vendors.TryGetValue(location.vendorId, out var vendor))
            {
                return LocationPreviewData.FromVendor(vendor);
            }

            // Generic location preview
            return LocationPreviewData.FromLocation(location);
        }

        /// <summary>
        /// Check if a zone is unlocked for the given stage.
        /// Requirements: R3.5-R3.10
        /// </summary>
        public bool IsZoneUnlocked(MapZone zone, int stage)
        {
            // Clamp stage to valid range
            stage = Math.Clamp(stage, 1, 5);

            return zone switch
            {
                MapZone.Neighborhood => true, // Always unlocked
                MapZone.Downtown => stage >= 2, // Stage 2+ (employer network), Stage 3+ (full)
                MapZone.Uptown => stage >= 4, // Stage 4+
                MapZone.Waterfront => stage >= 5, // Stage 5 only
                _ => false
            };
        }

        /// <summary>
        /// Get the player's current office location.
        /// Returns null if player doesn't have an office yet (Stage 1).
        /// </summary>
        public LocationData GetPlayerOffice(int stage)
        {
            // Stage 1: No office (working from home)
            if (stage < 2)
                return null;

            // Stage 2: Employer's office in Downtown
            if (stage == 2)
            {
                return _locations.Values
                    .FirstOrDefault(loc => loc.locationType == LocationType.Office && loc.zone == MapZone.Downtown);
            }

            // Stage 3+: Player's own office
            return _locations.Values
                .FirstOrDefault(loc => loc.locationType == LocationType.Office);
        }

        /// <summary>
        /// Register a location in the map system.
        /// </summary>
        public void RegisterLocation(LocationData location)
        {
            if (location == null || string.IsNullOrEmpty(location.locationId))
                return;

            _locations[location.locationId] = location;
        }

        /// <summary>
        /// Register a venue as a location.
        /// </summary>
        public void RegisterVenue(VenueData venue)
        {
            if (venue == null || string.IsNullOrEmpty(venue.id))
                return;

            _venues[venue.id] = venue;

            // Create and register the location
            var mapPosition = GenerateMapPosition(venue.zone, venue.id);
            var location = LocationData.FromVenue(venue, mapPosition);
            RegisterLocation(location);
        }

        /// <summary>
        /// Register a vendor as a location.
        /// </summary>
        public void RegisterVendor(VendorData vendor)
        {
            if (vendor == null || string.IsNullOrEmpty(vendor.id))
                return;

            _vendors[vendor.id] = vendor;

            // Create and register the location
            var mapPosition = GenerateMapPosition(vendor.zone, vendor.id);
            var location = LocationData.FromVendor(vendor, mapPosition);
            RegisterLocation(location);
        }

        /// <summary>
        /// Get all registered locations (unfiltered).
        /// </summary>
        public List<LocationData> GetAllLocations()
        {
            return _locations.Values.ToList();
        }

        /// <summary>
        /// Get location by ID.
        /// </summary>
        public LocationData GetLocation(string locationId)
        {
            if (string.IsNullOrEmpty(locationId))
                return null;

            _locations.TryGetValue(locationId, out var location);
            return location;
        }

        /// <summary>
        /// Get locations by type across all zones.
        /// </summary>
        public List<LocationData> GetLocationsByType(LocationType type)
        {
            if (type == LocationType.All)
                return GetAllLocations();

            return _locations.Values
                .Where(loc => loc.locationType == type)
                .ToList();
        }

        /// <summary>
        /// Get the count of locations in a zone.
        /// </summary>
        public int GetLocationCount(MapZone zone)
        {
            return _locations.Values.Count(loc => loc.zone == zone);
        }

        /// <summary>
        /// Get the count of locations by type in a zone.
        /// </summary>
        public int GetLocationCount(MapZone zone, LocationType type)
        {
            if (type == LocationType.All)
                return GetLocationCount(zone);

            return _locations.Values.Count(loc => loc.zone == zone && loc.locationType == type);
        }

        /// <summary>
        /// Clear the current zone selection (return to overview).
        /// </summary>
        public void ClearZoneSelection()
        {
            _currentZone = null;
        }

        /// <summary>
        /// Generate a deterministic map position based on zone and ID.
        /// This ensures consistent positioning across sessions.
        /// </summary>
        private Vector2 GenerateMapPosition(MapZone zone, string id)
        {
            // Use hash of ID for deterministic but varied positioning
            int hash = id.GetHashCode();
            float x = (hash & 0xFFFF) / 65535f;
            float y = ((hash >> 16) & 0xFFFF) / 65535f;

            // Adjust position based on zone (each zone has a different area)
            return zone switch
            {
                MapZone.Neighborhood => new Vector2(0.1f + x * 0.3f, 0.1f + y * 0.3f),
                MapZone.Downtown => new Vector2(0.4f + x * 0.3f, 0.1f + y * 0.3f),
                MapZone.Uptown => new Vector2(0.1f + x * 0.3f, 0.5f + y * 0.3f),
                MapZone.Waterfront => new Vector2(0.4f + x * 0.3f, 0.5f + y * 0.3f),
                _ => new Vector2(x, y)
            };
        }
    }
}
