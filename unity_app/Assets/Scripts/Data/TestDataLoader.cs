using System.Collections.Generic;
using UnityEngine;
using EventPlannerSim.Core;

namespace EventPlannerSim.Data
{
    /// <summary>
    /// Loads test/development data for prototyping without requiring ScriptableObjects.
    /// This allows testing the UI before creating actual asset files in Unity Editor.
    /// </summary>
    public static class TestDataLoader
    {
        /// <summary>
        /// Create test venues for development.
        /// </summary>
        public static List<VenueData> CreateTestVenues()
        {
            var venues = new List<VenueData>();

            // Neighborhood venues (Stage 1)
            venues.Add(CreateVenue("venue_backyard_smith", "Smith Family Backyard", VenueType.Backyard, VendorTier.Budget,
                MapZone.Neighborhood, 10, 30, 20, 150f, 8f, true, true, true, 2.5f, BusinessStage.Solo));

            venues.Add(CreateVenue("venue_community_oakdale", "Oakdale Community Center", VenueType.CommunityCenter, VendorTier.Budget,
                MapZone.Neighborhood, 30, 100, 60, 300f, 5f, true, false, false, 3.0f, BusinessStage.Solo));

            venues.Add(CreateVenue("venue_park_riverside", "Riverside Park Pavilion", VenueType.ParkPavilion, VendorTier.Budget,
                MapZone.Neighborhood, 20, 80, 50, 200f, 6f, false, true, true, 3.5f, BusinessStage.Solo));

            // Downtown venues (Stage 2+)
            venues.Add(CreateVenue("venue_hotel_grandview", "Grand View Hotel", VenueType.Hotel, VendorTier.Standard,
                MapZone.Downtown, 50, 200, 120, 800f, 15f, true, false, false, 4.0f, BusinessStage.Employee));

            venues.Add(CreateVenue("venue_restaurant_bella", "Bella's Italian", VenueType.Restaurant, VendorTier.Standard,
                MapZone.Downtown, 20, 60, 40, 500f, 20f, true, false, false, 4.2f, BusinessStage.Employee));

            venues.Add(CreateVenue("venue_ballroom_crystal", "Crystal Ballroom", VenueType.SmallBallroom, VendorTier.Standard,
                MapZone.Downtown, 80, 200, 150, 1200f, 12f, true, false, false, 4.3f, BusinessStage.Employee));

            // Large venues (Stage 3+)
            venues.Add(CreateVenue("venue_convention_metro", "Metro Convention Center", VenueType.ConventionCenter, VendorTier.Premium,
                MapZone.Downtown, 200, 1000, 500, 3000f, 8f, true, false, false, 4.0f, BusinessStage.SmallCompany));

            venues.Add(CreateVenue("venue_conference_plaza", "Plaza Conference Hotel", VenueType.ConferenceHotel, VendorTier.Premium,
                MapZone.Downtown, 100, 400, 250, 2000f, 18f, true, false, false, 4.4f, BusinessStage.SmallCompany));

            // Uptown venues (Stage 4+)
            venues.Add(CreateVenue("venue_luxury_monarch", "The Monarch Hotel", VenueType.LuxuryHotel, VendorTier.Luxury,
                MapZone.Uptown, 50, 300, 150, 5000f, 40f, true, true, false, 4.8f, BusinessStage.Established));

            venues.Add(CreateVenue("venue_estate_hartford", "Hartford Estate", VenueType.Estate, VendorTier.Luxury,
                MapZone.Uptown, 100, 400, 200, 8000f, 30f, true, true, true, 4.9f, BusinessStage.Established));

            venues.Add(CreateVenue("venue_rooftop_skyline", "Skyline Rooftop", VenueType.Rooftop, VendorTier.Premium,
                MapZone.Uptown, 30, 120, 80, 3500f, 35f, false, true, true, 4.6f, BusinessStage.Established));

            // Waterfront venues (Stage 5)
            venues.Add(CreateVenue("venue_beach_sunset", "Sunset Beach Club", VenueType.Beach, VendorTier.Luxury,
                MapZone.Waterfront, 50, 250, 150, 10000f, 50f, false, true, true, 4.7f, BusinessStage.Premier));

            venues.Add(CreateVenue("venue_garden_windsor", "Windsor Garden Estate", VenueType.GardenEstate, VendorTier.Luxury,
                MapZone.Waterfront, 100, 500, 300, 15000f, 45f, true, true, true, 5.0f, BusinessStage.Premier));

            return venues;
        }

        /// <summary>
        /// Create test vendors for development.
        /// </summary>
        public static List<VendorData> CreateTestVendors()
        {
            var vendors = new List<VendorData>();

            // Neighborhood caterers
            vendors.Add(CreateVendor("vendor_caterer_mamas", "Mama's Kitchen", VendorCategory.Caterer, VendorTier.Budget,
                MapZone.Neighborhood, 200f, 3.8f, "Home-style cooking", 0.85f, 0.9f));

            vendors.Add(CreateVendor("vendor_caterer_grill", "Backyard Grill Masters", VendorCategory.Caterer, VendorTier.Budget,
                MapZone.Neighborhood, 180f, 4.0f, "BBQ & grilling", 0.8f, 0.85f));

            // Neighborhood entertainers
            vendors.Add(CreateVendor("vendor_entertainer_magic", "Amazing Marcus Magic", VendorCategory.Entertainer, VendorTier.Budget,
                MapZone.Neighborhood, 150f, 4.2f, "Kids magic shows", 0.9f, 0.7f));

            vendors.Add(CreateVendor("vendor_entertainer_dj_local", "DJ Local Vibes", VendorCategory.Entertainer, VendorTier.Budget,
                MapZone.Neighborhood, 200f, 3.5f, "All genres", 0.75f, 0.8f));

            // Neighborhood decorators
            vendors.Add(CreateVendor("vendor_decorator_party", "Party Time Decor", VendorCategory.Decorator, VendorTier.Budget,
                MapZone.Neighborhood, 100f, 3.7f, "Balloons & basics", 0.85f, 0.9f));

            // Neighborhood photographers
            vendors.Add(CreateVendor("vendor_photo_snap", "Snap Happy Photos", VendorCategory.Photographer, VendorTier.Budget,
                MapZone.Neighborhood, 250f, 3.9f, "Event photography", 0.8f, 0.75f));

            // Downtown caterers
            vendors.Add(CreateVendor("vendor_caterer_elite", "Elite Catering Co", VendorCategory.Caterer, VendorTier.Standard,
                MapZone.Downtown, 500f, 4.3f, "Corporate events", 0.9f, 0.85f));

            vendors.Add(CreateVendor("vendor_caterer_fusion", "Fusion Flavors", VendorCategory.Caterer, VendorTier.Standard,
                MapZone.Downtown, 450f, 4.5f, "International cuisine", 0.85f, 0.7f));

            // Downtown entertainers
            vendors.Add(CreateVendor("vendor_entertainer_band_city", "City Lights Band", VendorCategory.Entertainer, VendorTier.Standard,
                MapZone.Downtown, 800f, 4.4f, "Live music", 0.88f, 0.6f));

            vendors.Add(CreateVendor("vendor_entertainer_dj_pro", "DJ Pro Beats", VendorCategory.Entertainer, VendorTier.Standard,
                MapZone.Downtown, 400f, 4.2f, "Club & corporate", 0.92f, 0.85f));

            // Downtown decorators
            vendors.Add(CreateVendor("vendor_decorator_elegant", "Elegant Designs", VendorCategory.Decorator, VendorTier.Standard,
                MapZone.Downtown, 350f, 4.3f, "Sophisticated themes", 0.9f, 0.8f));

            // Downtown photographers
            vendors.Add(CreateVendor("vendor_photo_studio", "Studio Moments", VendorCategory.Photographer, VendorTier.Standard,
                MapZone.Downtown, 600f, 4.5f, "Professional portraits", 0.95f, 0.7f));

            // Downtown florists
            vendors.Add(CreateVendor("vendor_florist_bloom", "Bloom & Petal", VendorCategory.Florist, VendorTier.Standard,
                MapZone.Downtown, 300f, 4.4f, "Fresh arrangements", 0.88f, 0.75f));

            // Downtown bakers
            vendors.Add(CreateVendor("vendor_baker_sweet", "Sweet Dreams Bakery", VendorCategory.Baker, VendorTier.Standard,
                MapZone.Downtown, 200f, 4.6f, "Custom cakes", 0.92f, 0.65f));

            // Uptown premium vendors
            vendors.Add(CreateVendor("vendor_caterer_artisan", "Artisan Table", VendorCategory.Caterer, VendorTier.Premium,
                MapZone.Uptown, 1200f, 4.7f, "Farm-to-table", 0.95f, 0.75f));

            vendors.Add(CreateVendor("vendor_entertainer_orchestra", "Chamber Orchestra", VendorCategory.Entertainer, VendorTier.Premium,
                MapZone.Uptown, 2000f, 4.8f, "Classical ensemble", 0.98f, 0.5f));

            vendors.Add(CreateVendor("vendor_decorator_luxe", "Luxe Events Design", VendorCategory.Decorator, VendorTier.Premium,
                MapZone.Uptown, 800f, 4.7f, "Luxury weddings", 0.93f, 0.7f));

            vendors.Add(CreateVendor("vendor_photo_vogue", "Vogue Photography", VendorCategory.Photographer, VendorTier.Premium,
                MapZone.Uptown, 1500f, 4.9f, "Editorial style", 0.97f, 0.6f));

            // Waterfront luxury vendors
            vendors.Add(CreateVendor("vendor_caterer_michelin", "Michelin Events", VendorCategory.Caterer, VendorTier.Luxury,
                MapZone.Waterfront, 3000f, 5.0f, "Fine dining", 0.99f, 0.5f));

            vendors.Add(CreateVendor("vendor_decorator_couture", "Couture Events", VendorCategory.Decorator, VendorTier.Luxury,
                MapZone.Waterfront, 2000f, 4.9f, "Bespoke design", 0.96f, 0.6f));

            return vendors;
        }

        /// <summary>
        /// Create test event types for development.
        /// </summary>
        public static List<EventTypeData> CreateTestEventTypes()
        {
            var eventTypes = new List<EventTypeData>();

            // Stage 1 events
            eventTypes.Add(CreateEventType("event_birthday_kids", "Kids Birthday Party", EventComplexity.Low, 1,
                300, 800,
                new List<string> { "Princess Theme", "Superhero Theme", "Dinosaur Theme", "Unicorn Theme" },
                new List<VendorCategory> { VendorCategory.Caterer, VendorCategory.Entertainer },
                new List<VendorCategory> { VendorCategory.Decorator, VendorCategory.Baker }));

            eventTypes.Add(CreateEventType("event_birthday_adult", "Adult Birthday Party", EventComplexity.Low, 1,
                500, 1500,
                new List<string> { "Milestone Birthday", "Surprise Party", "Themed Party", "Casual Celebration" },
                new List<VendorCategory> { VendorCategory.Caterer },
                new List<VendorCategory> { VendorCategory.Entertainer, VendorCategory.Decorator, VendorCategory.Photographer }));

            eventTypes.Add(CreateEventType("event_baby_shower", "Baby Shower", EventComplexity.Low, 1,
                400, 1200,
                new List<string> { "Gender Reveal", "Classic Shower", "Co-ed Shower", "Virtual Shower" },
                new List<VendorCategory> { VendorCategory.Caterer, VendorCategory.Decorator },
                new List<VendorCategory> { VendorCategory.Baker, VendorCategory.Photographer }));

            // Stage 2 events
            eventTypes.Add(CreateEventType("event_graduation", "Graduation Party", EventComplexity.Medium, 2,
                800, 2500,
                new List<string> { "High School Grad", "College Grad", "Masters/PhD Celebration" },
                new List<VendorCategory> { VendorCategory.Caterer, VendorCategory.Decorator },
                new List<VendorCategory> { VendorCategory.Entertainer, VendorCategory.Photographer }));

            eventTypes.Add(CreateEventType("event_corporate_small", "Small Corporate Event", EventComplexity.Medium, 2,
                1500, 5000,
                new List<string> { "Team Building", "Product Launch", "Quarterly Meeting", "Award Ceremony" },
                new List<VendorCategory> { VendorCategory.Caterer, VendorCategory.AVTechnician },
                new List<VendorCategory> { VendorCategory.Photographer, VendorCategory.Decorator }));

            eventTypes.Add(CreateEventType("event_engagement", "Engagement Party", EventComplexity.Medium, 2,
                1000, 3000,
                new List<string> { "Intimate Gathering", "Cocktail Party", "Garden Party" },
                new List<VendorCategory> { VendorCategory.Caterer, VendorCategory.Decorator },
                new List<VendorCategory> { VendorCategory.Photographer, VendorCategory.Florist }));

            // Stage 3 events
            eventTypes.Add(CreateEventType("event_wedding_small", "Intimate Wedding", EventComplexity.High, 3,
                5000, 15000,
                new List<string> { "Garden Wedding", "Courthouse + Reception", "Destination Elopement" },
                new List<VendorCategory> { VendorCategory.Caterer, VendorCategory.Photographer, VendorCategory.Florist },
                new List<VendorCategory> { VendorCategory.Entertainer, VendorCategory.Decorator, VendorCategory.Baker }));

            eventTypes.Add(CreateEventType("event_corporate_medium", "Corporate Conference", EventComplexity.High, 3,
                8000, 25000,
                new List<string> { "Annual Conference", "Training Seminar", "Industry Summit" },
                new List<VendorCategory> { VendorCategory.Caterer, VendorCategory.AVTechnician, VendorCategory.RentalCompany },
                new List<VendorCategory> { VendorCategory.Photographer, VendorCategory.Decorator }));

            // Stage 4 events
            eventTypes.Add(CreateEventType("event_wedding_large", "Traditional Wedding", EventComplexity.VeryHigh, 4,
                20000, 60000,
                new List<string> { "Classic Wedding", "Cultural Wedding", "Religious Ceremony" },
                new List<VendorCategory> { VendorCategory.Caterer, VendorCategory.Photographer, VendorCategory.Florist, VendorCategory.Entertainer },
                new List<VendorCategory> { VendorCategory.Decorator, VendorCategory.Baker, VendorCategory.RentalCompany }));

            eventTypes.Add(CreateEventType("event_gala", "Charity Gala", EventComplexity.VeryHigh, 4,
                30000, 100000,
                new List<string> { "Black Tie Gala", "Themed Fundraiser", "Award Ceremony" },
                new List<VendorCategory> { VendorCategory.Caterer, VendorCategory.Entertainer, VendorCategory.Decorator },
                new List<VendorCategory> { VendorCategory.Photographer, VendorCategory.Florist, VendorCategory.RentalCompany }));

            // Stage 5 events
            eventTypes.Add(CreateEventType("event_wedding_luxury", "Luxury Wedding", EventComplexity.VeryHigh, 5,
                75000, 200000,
                new List<string> { "Celebrity-Style Wedding", "Destination Wedding", "Multi-Day Celebration" },
                new List<VendorCategory> { VendorCategory.Caterer, VendorCategory.Photographer, VendorCategory.Florist, VendorCategory.Entertainer, VendorCategory.Decorator },
                new List<VendorCategory> { VendorCategory.Baker, VendorCategory.RentalCompany, VendorCategory.Transportation }));

            eventTypes.Add(CreateEventType("event_corporate_large", "Major Corporate Event", EventComplexity.VeryHigh, 5,
                50000, 150000,
                new List<string> { "Product Launch", "Company Anniversary", "International Conference" },
                new List<VendorCategory> { VendorCategory.Caterer, VendorCategory.AVTechnician, VendorCategory.RentalCompany, VendorCategory.Decorator },
                new List<VendorCategory> { VendorCategory.Photographer, VendorCategory.Entertainer, VendorCategory.Transportation }));

            return eventTypes;
        }

        #region Factory Methods

        private static VenueData CreateVenue(string id, string name, VenueType type, VendorTier tier,
            MapZone zone, int minCap, int maxCap, int comfortCap, float basePrice, float pricePerGuest,
            bool indoor, bool outdoor, bool weatherDep, float ambiance, BusinessStage reqStage)
        {
            var venue = ScriptableObject.CreateInstance<VenueData>();
            venue.id = id;
            venue.venueName = name;
            venue.venueType = type;
            venue.tier = tier;
            venue.zone = zone;
            venue.capacityMin = minCap;
            venue.capacityMax = maxCap;
            venue.capacityComfortable = comfortCap;
            venue.basePrice = basePrice;
            venue.pricePerGuest = pricePerGuest;
            venue.isIndoor = indoor;
            venue.hasOutdoorSpace = outdoor;
            venue.weatherDependent = weatherDep;
            venue.ambianceRating = ambiance;
            venue.requiredStage = reqStage;
            return venue;
        }

        private static VendorData CreateVendor(string id, string name, VendorCategory category, VendorTier tier,
            MapZone zone, float basePrice, float quality, string specialty, float reliability, float flexibility)
        {
            var vendor = ScriptableObject.CreateInstance<VendorData>();
            vendor.id = id;
            vendor.vendorName = name;
            vendor.category = category;
            vendor.tier = tier;
            vendor.zone = zone;
            vendor.basePrice = basePrice;
            vendor.qualityRating = quality;
            vendor.specialty = specialty;
            vendor.reliability = reliability;
            vendor.flexibility = flexibility;
            return vendor;
        }

        private static EventTypeData CreateEventType(string id, string displayName, EventComplexity complexity, int minStage,
            int minBudget, int maxBudget, List<string> subCategories,
            List<VendorCategory> requiredVendors, List<VendorCategory> optionalVendors)
        {
            var eventType = ScriptableObject.CreateInstance<EventTypeData>();
            eventType.eventTypeId = id;
            eventType.displayName = displayName;
            eventType.complexity = complexity;
            eventType.minStageRequired = minStage;
            eventType.minBudget = minBudget;
            eventType.maxBudget = maxBudget;
            eventType.subCategories = subCategories;
            eventType.requiredVendors = requiredVendors;
            eventType.optionalVendors = optionalVendors;
            return eventType;
        }

        #endregion

        /// <summary>
        /// Get all test data as a tuple for easy registration.
        /// </summary>
        public static (List<VenueData> venues, List<VendorData> vendors, List<EventTypeData> eventTypes) GetAllTestData()
        {
            return (CreateTestVenues(), CreateTestVendors(), CreateTestEventTypes());
        }
    }
}
