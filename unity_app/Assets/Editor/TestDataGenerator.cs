using UnityEngine;
using UnityEditor;
using EventPlannerSim.Data;
using EventPlannerSim.Core;
using System.Collections.Generic;

namespace EventPlannerSim.Editor
{
    /// <summary>
    /// Generates ScriptableObject test data for venues, vendors, and event types.
    /// Run via: Tools > Generate Test Data
    /// </summary>
    public static class TestDataGenerator
    {
        private const string VenuePath = "Assets/ScriptableObjects/Venues";
        private const string VendorPath = "Assets/ScriptableObjects/Vendors";
        private const string EventTypePath = "Assets/ScriptableObjects/EventTypes";

        [MenuItem("Tools/Generate Test Data")]
        public static void GenerateAll()
        {
            EnsureDirectories();
            GenerateVenues();
            GenerateVendors();
            GenerateEventTypes();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            UnityEngine.Debug.Log("[TestDataGenerator] Test data generation complete!");
        }

        private static void EnsureDirectories()
        {
            if (!AssetDatabase.IsValidFolder("Assets/ScriptableObjects"))
                AssetDatabase.CreateFolder("Assets", "ScriptableObjects");
            if (!AssetDatabase.IsValidFolder(VenuePath))
                AssetDatabase.CreateFolder("Assets/ScriptableObjects", "Venues");
            if (!AssetDatabase.IsValidFolder(VendorPath))
                AssetDatabase.CreateFolder("Assets/ScriptableObjects", "Vendors");
            if (!AssetDatabase.IsValidFolder(EventTypePath))
                AssetDatabase.CreateFolder("Assets/ScriptableObjects", "EventTypes");
        }

        private static void GenerateVenues()
        {
            // Stage 1 - Neighborhood venues
            CreateVenue("BackyardBasic", "venue_backyard_basic", "Sunny Backyard",
                VenueType.Backyard, VendorTier.Budget,
                10, 50, 30, 200, 5,
                false, true, true, 2.5f,
                MapZone.Neighborhood, BusinessStage.Solo);

            CreateVenue("CommunityHall", "venue_community_hall", "Riverside Community Center",
                VenueType.CommunityCenter, VendorTier.Budget,
                20, 100, 75, 500, 8,
                true, false, false, 3.0f,
                MapZone.Neighborhood, BusinessStage.Solo);

            CreateVenue("ParkPavilion", "venue_park_pavilion", "Oak Grove Pavilion",
                VenueType.ParkPavilion, VendorTier.Standard,
                30, 150, 100, 750, 6,
                false, true, true, 3.5f,
                MapZone.Neighborhood, BusinessStage.Solo);

            // Stage 2 - Downtown venues
            CreateVenue("DowntownHotel", "venue_downtown_hotel", "Grand Plaza Hotel",
                VenueType.Hotel, VendorTier.Premium,
                50, 300, 200, 2500, 15,
                true, true, false, 4.0f,
                MapZone.Downtown, BusinessStage.Employee);

            CreateVenue("RestaurantPrivate", "venue_restaurant_private", "La Maison Private Dining",
                VenueType.Restaurant, VendorTier.Standard,
                20, 80, 50, 1500, 20,
                true, false, false, 4.0f,
                MapZone.Downtown, BusinessStage.Employee);

            // Stage 3+ - Uptown/Waterfront venues
            CreateVenue("UptownBallroom", "venue_uptown_ballroom", "Crystal Ballroom",
                VenueType.SmallBallroom, VendorTier.Premium,
                100, 400, 300, 5000, 20,
                true, false, false, 4.5f,
                MapZone.Uptown, BusinessStage.SmallCompany);

            CreateVenue("WaterfrontEstate", "venue_waterfront_estate", "Harborview Estate",
                VenueType.Estate, VendorTier.Luxury,
                100, 500, 350, 8000, 25,
                true, true, false, 5.0f,
                MapZone.Waterfront, BusinessStage.Established);

            UnityEngine.Debug.Log("[TestDataGenerator] Created 7 venue assets");
        }

        private static void CreateVenue(string assetName, string id, string venueName,
            VenueType venueType, VendorTier tier,
            int capMin, int capMax, int capComfortable,
            float basePrice, float pricePerGuest,
            bool isIndoor, bool hasOutdoorSpace, bool weatherDependent, float ambiance,
            MapZone zone, BusinessStage requiredStage)
        {
            string path = $"{VenuePath}/{assetName}.asset";
            var existing = AssetDatabase.LoadAssetAtPath<VenueData>(path);
            if (existing != null)
            {
                UnityEngine.Debug.Log($"[TestDataGenerator] Venue already exists: {assetName}");
                return;
            }

            var venue = ScriptableObject.CreateInstance<VenueData>();
            venue.id = id;
            venue.venueName = venueName;
            venue.venueType = venueType;
            venue.tier = tier;
            venue.capacityMin = capMin;
            venue.capacityMax = capMax;
            venue.capacityComfortable = capComfortable;
            venue.basePrice = basePrice;
            venue.pricePerGuest = pricePerGuest;
            venue.isIndoor = isIndoor;
            venue.hasOutdoorSpace = hasOutdoorSpace;
            venue.weatherDependent = weatherDependent;
            venue.ambianceRating = ambiance;
            venue.zone = zone;
            venue.requiredStage = requiredStage;

            AssetDatabase.CreateAsset(venue, path);
        }

        private static void GenerateVendors()
        {
            // Caterers
            CreateVendor("BudgetCaterer", "vendor_caterer_budget", "Quick Bites Catering",
                VendorCategory.Caterer, VendorTier.Budget, 500,
                2.5f, "Finger foods and appetizers", MapZone.Neighborhood, 0.7f, 0.6f);

            CreateVendor("StandardCaterer", "vendor_caterer_standard", "Hometown Kitchen Catering",
                VendorCategory.Caterer, VendorTier.Standard, 1200,
                3.5f, "Comfort food and BBQ", MapZone.Neighborhood, 0.85f, 0.75f);

            CreateVendor("PremiumCaterer", "vendor_caterer_premium", "Gourmet Gatherings",
                VendorCategory.Caterer, VendorTier.Premium, 3500,
                4.5f, "International cuisine", MapZone.Downtown, 0.95f, 0.85f);

            CreateVendor("LuxuryCaterer", "vendor_caterer_luxury", "Elite Culinary Events",
                VendorCategory.Caterer, VendorTier.Luxury, 8000,
                5.0f, "Michelin-star experience", MapZone.Waterfront, 0.98f, 0.9f);

            // Entertainers
            CreateVendor("BudgetEntertainer", "vendor_entertainer_budget", "DJ Mike",
                VendorCategory.Entertainer, VendorTier.Budget, 300,
                2.5f, "Party music and karaoke", MapZone.Neighborhood, 0.65f, 0.8f);

            CreateVendor("StandardEntertainer", "vendor_entertainer_standard", "The Groove Band",
                VendorCategory.Entertainer, VendorTier.Standard, 1000,
                3.5f, "Cover band for all occasions", MapZone.Neighborhood, 0.8f, 0.7f);

            CreateVendor("PremiumEntertainer", "vendor_entertainer_premium", "String Quartet Elegance",
                VendorCategory.Entertainer, VendorTier.Premium, 2500,
                4.5f, "Classical and contemporary strings", MapZone.Uptown, 0.95f, 0.6f);

            // Decorators
            CreateVendor("BudgetDecorator", "vendor_decorator_budget", "Party Supplies Plus",
                VendorCategory.Decorator, VendorTier.Budget, 200,
                2.0f, "Balloon arrangements and banners", MapZone.Neighborhood, 0.75f, 0.9f);

            CreateVendor("StandardDecorator", "vendor_decorator_standard", "Elegant Events Decor",
                VendorCategory.Decorator, VendorTier.Standard, 800,
                3.5f, "Themed decorations and lighting", MapZone.Neighborhood, 0.85f, 0.75f);

            CreateVendor("PremiumDecorator", "vendor_decorator_premium", "Luxe Design Studio",
                VendorCategory.Decorator, VendorTier.Premium, 3000,
                4.5f, "Custom floral and scenic design", MapZone.Downtown, 0.92f, 0.7f);

            // Photographers
            CreateVendor("BudgetPhotographer", "vendor_photographer_budget", "Snap Happy Photos",
                VendorCategory.Photographer, VendorTier.Budget, 300,
                2.5f, "Basic event coverage", MapZone.Neighborhood, 0.7f, 0.8f);

            CreateVendor("StandardPhotographer", "vendor_photographer_standard", "Capture Moments",
                VendorCategory.Photographer, VendorTier.Standard, 600,
                3.5f, "Event and portrait photography", MapZone.Neighborhood, 0.9f, 0.7f);

            CreateVendor("PremiumPhotographer", "vendor_photographer_premium", "Artistry Images",
                VendorCategory.Photographer, VendorTier.Premium, 2000,
                4.5f, "Editorial and fine art style", MapZone.Downtown, 0.95f, 0.65f);

            // Florists
            CreateVendor("StandardFlorist", "vendor_florist_standard", "Bloom & Petal",
                VendorCategory.Florist, VendorTier.Standard, 500,
                3.5f, "Fresh bouquets and centerpieces", MapZone.Neighborhood, 0.85f, 0.7f);

            CreateVendor("PremiumFlorist", "vendor_florist_premium", "Enchanted Gardens",
                VendorCategory.Florist, VendorTier.Premium, 2000,
                4.5f, "Luxury floral installations", MapZone.Uptown, 0.9f, 0.6f);

            // Bakers
            CreateVendor("BudgetBaker", "vendor_baker_budget", "Sweet Tooth Bakery",
                VendorCategory.Baker, VendorTier.Budget, 150,
                2.5f, "Cupcakes and sheet cakes", MapZone.Neighborhood, 0.8f, 0.85f);

            CreateVendor("StandardBaker", "vendor_baker_standard", "Artisan Cakes Co",
                VendorCategory.Baker, VendorTier.Standard, 500,
                3.5f, "Custom tiered cakes", MapZone.Neighborhood, 0.85f, 0.7f);

            UnityEngine.Debug.Log("[TestDataGenerator] Created 17 vendor assets");
        }

        private static void CreateVendor(string assetName, string id, string vendorName,
            VendorCategory category, VendorTier tier, float basePrice,
            float quality, string specialty, MapZone zone,
            float reliability, float flexibility)
        {
            string path = $"{VendorPath}/{assetName}.asset";
            var existing = AssetDatabase.LoadAssetAtPath<VendorData>(path);
            if (existing != null)
            {
                UnityEngine.Debug.Log($"[TestDataGenerator] Vendor already exists: {assetName}");
                return;
            }

            var vendor = ScriptableObject.CreateInstance<VendorData>();
            vendor.id = id;
            vendor.vendorName = vendorName;
            vendor.category = category;
            vendor.tier = tier;
            vendor.basePrice = basePrice;
            vendor.qualityRating = quality;
            vendor.specialty = specialty;
            vendor.zone = zone;
            vendor.reliability = reliability;
            vendor.flexibility = flexibility;

            AssetDatabase.CreateAsset(vendor, path);
        }

        private static void GenerateEventTypes()
        {
            // Stage 1 - Basic events
            CreateEventType("BirthdayParty", "event_birthday", "Birthday Party",
                EventComplexity.Low, 1, 500, 3000,
                new[] { "Kids Birthday", "Adult Birthday", "Milestone Birthday", "Surprise Party" },
                new[] { VendorCategory.Caterer },
                new[] { VendorCategory.Entertainer, VendorCategory.Decorator, VendorCategory.Baker },
                new[] { 0.25f, 0.35f, 0.15f, 0.15f, 0.05f, 0.05f });

            CreateEventType("BabyShower", "event_babyshower", "Baby Shower",
                EventComplexity.Low, 1, 400, 2500,
                new[] { "Traditional Baby Shower", "Gender Reveal", "Couples Baby Shower" },
                new[] { VendorCategory.Caterer },
                new[] { VendorCategory.Decorator, VendorCategory.Baker },
                new[] { 0.20f, 0.35f, 0.10f, 0.25f, 0.05f, 0.05f });

            CreateEventType("Graduation", "event_graduation", "Graduation Party",
                EventComplexity.Medium, 1, 800, 5000,
                new[] { "High School Graduation", "College Graduation", "PhD Celebration" },
                new[] { VendorCategory.Caterer },
                new[] { VendorCategory.Entertainer, VendorCategory.Decorator, VendorCategory.Photographer },
                new[] { 0.25f, 0.35f, 0.15f, 0.15f, 0.05f, 0.05f });

            // Stage 2 - More complex events
            CreateEventType("CorporateMeeting", "event_corporate", "Corporate Meeting",
                EventComplexity.Medium, 2, 2000, 15000,
                new[] { "Board Meeting", "Training Session", "Team Building", "Product Launch" },
                new[] { VendorCategory.Caterer, VendorCategory.AVTechnician },
                new[] { VendorCategory.Decorator },
                new[] { 0.40f, 0.25f, 0.05f, 0.10f, 0.15f, 0.05f });

            CreateEventType("Anniversary", "event_anniversary", "Anniversary Celebration",
                EventComplexity.Medium, 2, 2000, 20000,
                new[] { "Silver Anniversary", "Golden Anniversary", "Vow Renewal" },
                new[] { VendorCategory.Caterer, VendorCategory.Photographer },
                new[] { VendorCategory.Entertainer, VendorCategory.Decorator, VendorCategory.Florist },
                new[] { 0.30f, 0.30f, 0.15f, 0.15f, 0.05f, 0.05f });

            // Stage 3+ - High-end events
            CreateEventType("Wedding", "event_wedding", "Wedding",
                EventComplexity.VeryHigh, 2, 10000, 100000,
                new[] { "Traditional Wedding", "Beach Wedding", "Garden Wedding", "Destination Wedding", "Intimate Wedding" },
                new[] { VendorCategory.Caterer, VendorCategory.Photographer, VendorCategory.Florist },
                new[] { VendorCategory.Entertainer, VendorCategory.Decorator, VendorCategory.Baker, VendorCategory.AVTechnician },
                new[] { 0.35f, 0.25f, 0.10f, 0.15f, 0.10f, 0.05f });

            CreateEventType("Gala", "event_gala", "Gala Evening",
                EventComplexity.VeryHigh, 3, 20000, 150000,
                new[] { "Charity Gala", "Awards Gala", "Black Tie Gala", "Masquerade Ball" },
                new[] { VendorCategory.Caterer, VendorCategory.Entertainer, VendorCategory.Decorator, VendorCategory.Photographer },
                new[] { VendorCategory.Florist, VendorCategory.AVTechnician, VendorCategory.Transportation },
                new[] { 0.30f, 0.30f, 0.15f, 0.15f, 0.05f, 0.05f });

            CreateEventType("Conference", "event_conference", "Conference",
                EventComplexity.High, 3, 15000, 80000,
                new[] { "Industry Conference", "Tech Summit", "Academic Symposium", "Trade Show" },
                new[] { VendorCategory.Caterer, VendorCategory.AVTechnician },
                new[] { VendorCategory.Decorator, VendorCategory.Photographer },
                new[] { 0.35f, 0.20f, 0.05f, 0.10f, 0.25f, 0.05f });

            UnityEngine.Debug.Log("[TestDataGenerator] Created 8 event type assets");
        }

        private static void CreateEventType(string assetName, string id, string displayName,
            EventComplexity complexity, int minStage, int minBudget, int maxBudget,
            string[] subCategories, VendorCategory[] required, VendorCategory[] optional,
            float[] budgetSplit)
        {
            string path = $"{EventTypePath}/{assetName}.asset";
            var existing = AssetDatabase.LoadAssetAtPath<EventTypeData>(path);
            if (existing != null)
            {
                UnityEngine.Debug.Log($"[TestDataGenerator] EventType already exists: {assetName}");
                return;
            }

            var eventType = ScriptableObject.CreateInstance<EventTypeData>();
            eventType.eventTypeId = id;
            eventType.displayName = displayName;
            eventType.complexity = complexity;
            eventType.minStageRequired = minStage;
            eventType.minBudget = minBudget;
            eventType.maxBudget = maxBudget;
            eventType.subCategories = new List<string>(subCategories);
            eventType.requiredVendors = new List<VendorCategory>(required);
            eventType.optionalVendors = new List<VendorCategory>(optional);
            eventType.recommendedBudgetSplit = budgetSplit;

            AssetDatabase.CreateAsset(eventType, path);
        }
    }
}
