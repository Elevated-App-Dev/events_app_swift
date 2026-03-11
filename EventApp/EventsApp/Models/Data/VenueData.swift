import Foundation

struct VenueData: Codable, Equatable, Identifiable {
    var id: String
    var venueName: String
    var venueType: VenueType
    var tier: VendorTier
    var capacityMin: Int
    var capacityMax: Int
    var capacityComfortable: Int
    var basePrice: Double
    var pricePerGuest: Double
    var isIndoor: Bool
    var hasOutdoorSpace: Bool
    var weatherDependent: Bool
    var ambianceRating: Double
    var zone: MapZone
    var requiredStage: BusinessStage
}
