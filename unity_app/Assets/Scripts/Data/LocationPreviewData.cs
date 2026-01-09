using System;
using EventPlannerSim.Core;

namespace EventPlannerSim.Data
{
    /// <summary>
    /// Preview card data for a location, shown when player taps a map pin.
    /// Requirements: R3.3, R3.4
    /// </summary>
    [Serializable]
    public class LocationPreviewData
    {
        /// <summary>
        /// Unique identifier matching the LocationData.
        /// </summary>
        public string locationId;

        /// <summary>
        /// Display name for the preview card.
        /// </summary>
        public string displayName;

        /// <summary>
        /// Description text for the preview card.
        /// </summary>
        public string description;

        /// <summary>
        /// Path to the thumbnail image for the preview card.
        /// </summary>
        public string thumbnailPath;

        /// <summary>
        /// Type of location for visual styling.
        /// </summary>
        public LocationType locationType;

        /// <summary>
        /// The zone this location belongs to.
        /// </summary>
        public MapZone zone;

        // Venue-specific fields
        /// <summary>
        /// Venue capacity (0 if not a venue).
        /// </summary>
        public int capacity;

        /// <summary>
        /// Base price per event (0 if not a venue).
        /// </summary>
        public float pricePerEvent;

        /// <summary>
        /// Venue type (only valid for venue locations).
        /// </summary>
        public VenueType venueType;

        /// <summary>
        /// Whether the venue is indoor.
        /// </summary>
        public bool isIndoor;

        /// <summary>
        /// Ambiance rating (1-5).
        /// </summary>
        public float ambianceRating;

        // Vendor-specific fields
        /// <summary>
        /// Vendor tier (Budget, Standard, Premium, Luxury).
        /// </summary>
        public VendorTier tier;

        /// <summary>
        /// Quality rating (1-5).
        /// </summary>
        public float rating;

        /// <summary>
        /// Vendor category (only valid for vendor locations).
        /// </summary>
        public VendorCategory vendorCategory;

        /// <summary>
        /// Vendor specialty description.
        /// </summary>
        public string specialty;

        /// <summary>
        /// Base price for vendor services.
        /// </summary>
        public float vendorBasePrice;

        /// <summary>
        /// Default constructor for serialization.
        /// </summary>
        public LocationPreviewData() { }

        /// <summary>
        /// Create preview data from a venue.
        /// </summary>
        public static LocationPreviewData FromVenue(VenueData venue)
        {
            return new LocationPreviewData
            {
                locationId = $"venue_{venue.id}",
                displayName = venue.venueName,
                description = GetVenueDescription(venue),
                thumbnailPath = $"Thumbnails/Venues/{venue.id}",
                locationType = LocationType.Venue,
                zone = venue.zone,
                capacity = venue.capacityComfortable,
                pricePerEvent = venue.basePrice,
                venueType = venue.venueType,
                isIndoor = venue.isIndoor,
                ambianceRating = venue.ambianceRating
            };
        }

        /// <summary>
        /// Create preview data from a vendor.
        /// </summary>
        public static LocationPreviewData FromVendor(VendorData vendor)
        {
            return new LocationPreviewData
            {
                locationId = $"vendor_{vendor.id}",
                displayName = vendor.vendorName,
                description = GetVendorDescription(vendor),
                thumbnailPath = $"Thumbnails/Vendors/{vendor.id}",
                locationType = LocationType.Vendor,
                zone = vendor.zone,
                tier = vendor.tier,
                rating = vendor.qualityRating,
                vendorCategory = vendor.category,
                specialty = vendor.specialty,
                vendorBasePrice = vendor.basePrice
            };
        }

        /// <summary>
        /// Create preview data from a generic location.
        /// </summary>
        public static LocationPreviewData FromLocation(LocationData location)
        {
            return new LocationPreviewData
            {
                locationId = location.locationId,
                displayName = location.displayName,
                description = location.description,
                thumbnailPath = $"Thumbnails/Locations/{location.locationId}",
                locationType = location.locationType,
                zone = location.zone
            };
        }

        private static string GetVenueDescription(VenueData venue)
        {
            string indoorOutdoor = venue.isIndoor ? "Indoor" : "Outdoor";
            if (venue.hasOutdoorSpace && venue.isIndoor)
                indoorOutdoor = "Indoor with outdoor space";
            
            return $"{venue.venueType} venue. {indoorOutdoor}. " +
                   $"Capacity: {venue.capacityMin}-{venue.capacityMax} guests. " +
                   $"Ambiance: {venue.ambianceRating:F1}/5";
        }

        private static string GetVendorDescription(VendorData vendor)
        {
            return $"{vendor.tier} {vendor.category}. " +
                   $"Specialty: {vendor.specialty}. " +
                   $"Quality: {vendor.qualityRating:F1}/5";
        }
    }
}
