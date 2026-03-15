import Foundation

struct VendorData: Codable, Equatable, Identifiable {
    var id: String
    var vendorName: String
    var category: VendorCategory
    var tier: VendorTier
    var basePrice: Double
    var qualityRating: Double
    var specialty: String
    var zone: MapZone
    var reliability: Double
    var flexibility: Double
}
