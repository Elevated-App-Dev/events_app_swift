import Foundation

/// Static catalog of seed data for Stage 1 (neighborhood zone).
enum SeedData {

    // MARK: - Venues

    static let venues: [VenueData] = [
        VenueData(
            id: "venue_backyard_sunny",
            venueName: "Sunny Side Backyard",
            venueType: .backyard,
            tier: .budget,
            capacityMin: 10,
            capacityMax: 40,
            capacityComfortable: 30,
            basePrice: 150,
            pricePerGuest: 5,
            isIndoor: false,
            hasOutdoorSpace: true,
            weatherDependent: true,
            ambianceRating: 55,
            zone: .neighborhood,
            requiredStage: .solo
        ),
        VenueData(
            id: "venue_comm_center_oak",
            venueName: "Oak Street Community Center",
            venueType: .communityCenter,
            tier: .budget,
            capacityMin: 20,
            capacityMax: 80,
            capacityComfortable: 60,
            basePrice: 300,
            pricePerGuest: 8,
            isIndoor: true,
            hasOutdoorSpace: false,
            weatherDependent: false,
            ambianceRating: 50,
            zone: .neighborhood,
            requiredStage: .solo
        ),
        VenueData(
            id: "venue_park_pavilion_riverside",
            venueName: "Riverside Park Pavilion",
            venueType: .parkPavilion,
            tier: .budget,
            capacityMin: 15,
            capacityMax: 60,
            capacityComfortable: 45,
            basePrice: 200,
            pricePerGuest: 6,
            isIndoor: false,
            hasOutdoorSpace: true,
            weatherDependent: true,
            ambianceRating: 65,
            zone: .neighborhood,
            requiredStage: .solo
        ),
        VenueData(
            id: "venue_restaurant_mamas",
            venueName: "Mama's Kitchen & Hall",
            venueType: .restaurant,
            tier: .standard,
            capacityMin: 10,
            capacityMax: 50,
            capacityComfortable: 35,
            basePrice: 400,
            pricePerGuest: 12,
            isIndoor: true,
            hasOutdoorSpace: false,
            weatherDependent: false,
            ambianceRating: 70,
            zone: .neighborhood,
            requiredStage: .solo
        ),
    ]

    // MARK: - Vendors

    static let vendors: [VendorData] = [
        // Caterers
        VendorData(
            id: "vendor_cat_comfort",
            vendorName: "Comfort Bites Catering",
            category: .caterer,
            tier: .budget,
            basePrice: 200,
            qualityRating: 55,
            specialty: "BBQ & Comfort Food",
            zone: .neighborhood,
            reliability: 0.80,
            flexibility: 0.70
        ),
        VendorData(
            id: "vendor_cat_garden",
            vendorName: "Garden Fresh Catering",
            category: .caterer,
            tier: .standard,
            basePrice: 400,
            qualityRating: 72,
            specialty: "Fresh & Healthy Menus",
            zone: .neighborhood,
            reliability: 0.90,
            flexibility: 0.60
        ),
        // Entertainers
        VendorData(
            id: "vendor_ent_sunny",
            vendorName: "Sunny Day Entertainment",
            category: .entertainer,
            tier: .budget,
            basePrice: 150,
            qualityRating: 60,
            specialty: "Kids Parties & Games",
            zone: .neighborhood,
            reliability: 0.85,
            flexibility: 0.75
        ),
        VendorData(
            id: "vendor_ent_beats",
            vendorName: "Local Beats DJ",
            category: .entertainer,
            tier: .standard,
            basePrice: 300,
            qualityRating: 68,
            specialty: "DJ & Music",
            zone: .neighborhood,
            reliability: 0.75,
            flexibility: 0.80
        ),
        // Decorator
        VendorData(
            id: "vendor_dec_bloom",
            vendorName: "Bloom & Drape Decor",
            category: .decorator,
            tier: .budget,
            basePrice: 175,
            qualityRating: 58,
            specialty: "Balloon & Fabric Decor",
            zone: .neighborhood,
            reliability: 0.85,
            flexibility: 0.65
        ),
        // Photographer
        VendorData(
            id: "vendor_photo_snap",
            vendorName: "SnapHappy Photos",
            category: .photographer,
            tier: .budget,
            basePrice: 250,
            qualityRating: 62,
            specialty: "Event Photography",
            zone: .neighborhood,
            reliability: 0.90,
            flexibility: 0.50
        ),
        // Baker
        VendorData(
            id: "vendor_baker_sweet",
            vendorName: "Sweet Tooth Bakery",
            category: .baker,
            tier: .budget,
            basePrice: 100,
            qualityRating: 70,
            specialty: "Custom Cakes & Cupcakes",
            zone: .neighborhood,
            reliability: 0.95,
            flexibility: 0.40
        ),
        // Florist
        VendorData(
            id: "vendor_florist_petal",
            vendorName: "Petal & Stem Florals",
            category: .florist,
            tier: .budget,
            basePrice: 150,
            qualityRating: 65,
            specialty: "Centerpieces & Bouquets",
            zone: .neighborhood,
            reliability: 0.90,
            flexibility: 0.55
        ),
        VendorData(
            id: "vendor_florist_wild",
            vendorName: "Wildflower Arrangements",
            category: .florist,
            tier: .standard,
            basePrice: 300,
            qualityRating: 78,
            specialty: "Organic & Garden Style",
            zone: .neighborhood,
            reliability: 0.85,
            flexibility: 0.65
        ),
        // Rental Company
        VendorData(
            id: "vendor_rental_party",
            vendorName: "Party Perfect Rentals",
            category: .rentalCompany,
            tier: .budget,
            basePrice: 200,
            qualityRating: 60,
            specialty: "Tables, Chairs & Linens",
            zone: .neighborhood,
            reliability: 0.85,
            flexibility: 0.70
        ),
    ]

    // MARK: - Lookup Helpers

    static func venue(byId id: String) -> VenueData? {
        venues.first { $0.id == id }
    }

    static func vendor(byId id: String) -> VendorData? {
        vendors.first { $0.id == id }
    }

    static func vendors(forCategory category: VendorCategory) -> [VendorData] {
        vendors.filter { $0.category == category }
    }

    static func venues(forZones zones: [MapZone]) -> [VenueData] {
        venues.filter { zones.contains($0.zone) }
    }
}
