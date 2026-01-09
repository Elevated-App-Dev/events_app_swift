using System;
using UnityEngine;
using EventPlannerSim.Core;

namespace EventPlannerSim.Data
{
    /// <summary>
    /// Represents a location on the city map (venue, vendor, office, or meeting point).
    /// Requirements: R3.3, R3.7
    /// </summary>
    [Serializable]
    public class LocationData
    {
        /// <summary>
        /// Unique identifier for this location.
        /// </summary>
        public string locationId;

        /// <summary>
        /// Display name shown on the map and in UI.
        /// </summary>
        public string displayName;

        /// <summary>
        /// Type of location (Venue, Vendor, Office, MeetingPoint).
        /// Used for filtering and visual distinction on the map.
        /// </summary>
        public LocationType locationType;

        /// <summary>
        /// The zone this location belongs to.
        /// </summary>
        public MapZone zone;

        /// <summary>
        /// Position on the map UI (normalized 0-1 coordinates).
        /// </summary>
        public Vector2 mapPosition;

        /// <summary>
        /// Icon identifier for the map pin.
        /// </summary>
        public string iconId;

        /// <summary>
        /// Optional description for the location.
        /// </summary>
        public string description;

        /// <summary>
        /// Reference to the underlying venue data (if this is a venue location).
        /// </summary>
        public string venueId;

        /// <summary>
        /// Reference to the underlying vendor data (if this is a vendor location).
        /// </summary>
        public string vendorId;

        /// <summary>
        /// Default constructor for serialization.
        /// </summary>
        public LocationData() { }

        /// <summary>
        /// Create a location from a venue.
        /// </summary>
        public static LocationData FromVenue(VenueData venue, Vector2 mapPosition)
        {
            return new LocationData
            {
                locationId = $"venue_{venue.id}",
                displayName = venue.venueName,
                locationType = LocationType.Venue,
                zone = venue.zone,
                mapPosition = mapPosition,
                iconId = GetVenueIcon(venue.venueType),
                description = $"{venue.venueType} - Capacity: {venue.capacityComfortable}",
                venueId = venue.id
            };
        }

        /// <summary>
        /// Create a location from a vendor.
        /// </summary>
        public static LocationData FromVendor(VendorData vendor, Vector2 mapPosition)
        {
            return new LocationData
            {
                locationId = $"vendor_{vendor.id}",
                displayName = vendor.vendorName,
                locationType = LocationType.Vendor,
                zone = vendor.zone,
                mapPosition = mapPosition,
                iconId = GetVendorIcon(vendor.category),
                description = $"{vendor.category} - {vendor.tier}",
                vendorId = vendor.id
            };
        }

        /// <summary>
        /// Create an office location.
        /// </summary>
        public static LocationData CreateOffice(string id, string name, MapZone zone, Vector2 mapPosition)
        {
            return new LocationData
            {
                locationId = $"office_{id}",
                displayName = name,
                locationType = LocationType.Office,
                zone = zone,
                mapPosition = mapPosition,
                iconId = "icon_office",
                description = "Your office location"
            };
        }

        /// <summary>
        /// Create a meeting point location.
        /// </summary>
        public static LocationData CreateMeetingPoint(string id, string name, MapZone zone, Vector2 mapPosition)
        {
            return new LocationData
            {
                locationId = $"meeting_{id}",
                displayName = name,
                locationType = LocationType.MeetingPoint,
                zone = zone,
                mapPosition = mapPosition,
                iconId = "icon_meeting",
                description = "Client meeting location"
            };
        }

        private static string GetVenueIcon(VenueType venueType)
        {
            return venueType switch
            {
                VenueType.Backyard => "icon_venue_backyard",
                VenueType.CommunityCenter => "icon_venue_community",
                VenueType.ParkPavilion => "icon_venue_park",
                VenueType.Hotel => "icon_venue_hotel",
                VenueType.Restaurant => "icon_venue_restaurant",
                VenueType.SmallBallroom => "icon_venue_ballroom",
                VenueType.ConventionCenter => "icon_venue_convention",
                VenueType.ConferenceHotel => "icon_venue_conference",
                VenueType.LuxuryHotel => "icon_venue_luxury",
                VenueType.Estate => "icon_venue_estate",
                VenueType.Rooftop => "icon_venue_rooftop",
                VenueType.Beach => "icon_venue_beach",
                VenueType.GardenEstate => "icon_venue_garden",
                _ => "icon_venue_default"
            };
        }

        private static string GetVendorIcon(VendorCategory category)
        {
            return category switch
            {
                VendorCategory.Caterer => "icon_vendor_caterer",
                VendorCategory.Entertainer => "icon_vendor_entertainer",
                VendorCategory.Decorator => "icon_vendor_decorator",
                VendorCategory.Photographer => "icon_vendor_photographer",
                VendorCategory.Florist => "icon_vendor_florist",
                VendorCategory.Baker => "icon_vendor_baker",
                VendorCategory.RentalCompany => "icon_vendor_rental",
                VendorCategory.AVTechnician => "icon_vendor_av",
                VendorCategory.Transportation => "icon_vendor_transport",
                VendorCategory.Security => "icon_vendor_security",
                _ => "icon_vendor_default"
            };
        }
    }
}
