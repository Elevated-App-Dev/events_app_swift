import Foundation
import CoreGraphics

struct LocationData: Codable, Equatable, Identifiable {
    var id: String { locationId }
    var locationId: String
    var displayName: String
    var locationType: LocationType
    var zone: MapZone
    var mapPositionX: Double
    var mapPositionY: Double
    var iconId: String
    var description: String
    var venueId: String?
    var vendorId: String?

    var mapPosition: CGPoint {
        CGPoint(x: mapPositionX, y: mapPositionY)
    }

    static func fromVenue(_ venue: VenueData, mapPosition: CGPoint) -> LocationData {
        LocationData(
            locationId: "loc_venue_\(venue.id)",
            displayName: venue.venueName,
            locationType: .venue,
            zone: venue.zone,
            mapPositionX: mapPosition.x,
            mapPositionY: mapPosition.y,
            iconId: getVenueIcon(venue.venueType),
            description: "\(venue.venueType.rawValue) venue",
            venueId: venue.id
        )
    }

    static func fromVendor(_ vendor: VendorData, mapPosition: CGPoint) -> LocationData {
        LocationData(
            locationId: "loc_vendor_\(vendor.id)",
            displayName: vendor.vendorName,
            locationType: .vendor,
            zone: vendor.zone,
            mapPositionX: mapPosition.x,
            mapPositionY: mapPosition.y,
            iconId: getVendorIcon(vendor.category),
            description: "\(vendor.tier.rawValue) \(vendor.category.rawValue)",
            vendorId: vendor.id
        )
    }

    static func createOffice(id: String, name: String, zone: MapZone, mapPosition: CGPoint) -> LocationData {
        LocationData(
            locationId: id,
            displayName: name,
            locationType: .office,
            zone: zone,
            mapPositionX: mapPosition.x,
            mapPositionY: mapPosition.y,
            iconId: "icon_office",
            description: "Your office"
        )
    }

    static func createMeetingPoint(id: String, name: String, zone: MapZone, mapPosition: CGPoint) -> LocationData {
        LocationData(
            locationId: id,
            displayName: name,
            locationType: .meetingPoint,
            zone: zone,
            mapPositionX: mapPosition.x,
            mapPositionY: mapPosition.y,
            iconId: "icon_meeting",
            description: "Meeting point"
        )
    }

    private static func getVenueIcon(_ venueType: VenueType) -> String {
        "icon_venue_\(venueType.rawValue)"
    }

    private static func getVendorIcon(_ category: VendorCategory) -> String {
        "icon_vendor_\(category.rawValue)"
    }
}

struct LocationPreviewData: Codable, Equatable {
    var locationId: String
    var displayName: String
    var description: String
    var thumbnailPath: String?
    var locationType: LocationType
    var zone: MapZone
    // Venue properties
    var capacity: Int?
    var pricePerEvent: Double?
    var venueType: VenueType?
    var isIndoor: Bool?
    var ambianceRating: Double?
    // Vendor properties
    var tier: VendorTier?
    var rating: Double?
    var vendorCategory: VendorCategory?
    var specialty: String?
    var vendorBasePrice: Double?

    static func fromVenue(_ venue: VenueData) -> LocationPreviewData {
        LocationPreviewData(
            locationId: "loc_venue_\(venue.id)",
            displayName: venue.venueName,
            description: "\(venue.venueType.rawValue) in \(venue.zone.rawValue)",
            locationType: .venue,
            zone: venue.zone,
            capacity: venue.capacityComfortable,
            pricePerEvent: venue.basePrice,
            venueType: venue.venueType,
            isIndoor: venue.isIndoor,
            ambianceRating: venue.ambianceRating
        )
    }

    static func fromVendor(_ vendor: VendorData) -> LocationPreviewData {
        LocationPreviewData(
            locationId: "loc_vendor_\(vendor.id)",
            displayName: vendor.vendorName,
            description: "\(vendor.tier.rawValue) \(vendor.category.rawValue)",
            locationType: .vendor,
            zone: vendor.zone,
            tier: vendor.tier,
            rating: vendor.qualityRating,
            vendorCategory: vendor.category,
            specialty: vendor.specialty,
            vendorBasePrice: vendor.basePrice
        )
    }
}
