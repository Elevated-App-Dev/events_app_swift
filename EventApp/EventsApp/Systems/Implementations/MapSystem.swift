import Foundation

/// City navigation and location discovery system.
/// Zone unlocking: Stage 1 Neighborhood, Stage 2+ Downtown, Stage 4+ Uptown, Stage 5 Waterfront.
class MapSystem: MapSystemProtocol {

    private var locations: [String: LocationData] = [:]
    private var venues: [String: VenueData] = [:]
    private var vendors: [String: VendorData] = [:]
    private(set) var currentFilter: LocationType = .all
    private(set) var currentZone: MapZone?

    // MARK: - Zone Access

    func getVisibleZones(stage: Int) -> [MapZone] {
        let s = max(1, min(5, stage))
        var zones: [MapZone] = [.neighborhood]
        if s >= 2 { zones.append(.downtown) }
        if s >= 4 { zones.append(.uptown) }
        if s >= 5 { zones.append(.waterfront) }
        return zones
    }

    func isZoneUnlocked(_ zone: MapZone, stage: Int) -> Bool {
        let s = max(1, min(5, stage))
        switch zone {
        case .neighborhood: return true
        case .downtown:     return s >= 2
        case .uptown:       return s >= 4
        case .waterfront:   return s >= 5
        }
    }

    // MARK: - Location Queries

    func getLocationsInZone(_ zone: MapZone) -> [LocationData] {
        var results = locations.values.filter { $0.zone == zone }
        if currentFilter != .all {
            results = results.filter { $0.locationType == currentFilter }
        }
        return results
    }

    func getAllLocations() -> [LocationData] {
        Array(locations.values)
    }

    func getLocation(_ locationId: String) -> LocationData? {
        locations[locationId]
    }

    func getLocationsByType(_ type: LocationType) -> [LocationData] {
        if type == .all { return getAllLocations() }
        return locations.values.filter { $0.locationType == type }
    }

    func getLocationCount(zone: MapZone) -> Int {
        locations.values.filter { $0.zone == zone }.count
    }

    // MARK: - Navigation

    func navigateToZone(_ zone: MapZone) {
        currentZone = zone
    }

    func navigateToLocation(_ locationId: String) {
        guard let location = locations[locationId] else { return }
        if currentZone != location.zone {
            navigateToZone(location.zone)
        }
    }

    func clearZoneSelection() {
        currentZone = nil
    }

    // MARK: - Filtering

    func setLocationFilter(_ filter: LocationType) {
        currentFilter = filter
    }

    // MARK: - Preview

    func getLocationPreview(_ locationId: String) -> LocationPreviewData? {
        guard let location = locations[locationId] else { return nil }

        if let venueId = location.venueId, let venue = venues[venueId] {
            return LocationPreviewData.fromVenue(venue)
        }
        if let vendorId = location.vendorId, let vendor = vendors[vendorId] {
            return LocationPreviewData.fromVendor(vendor)
        }
        return LocationPreviewData.fromLocation(location)
    }

    // MARK: - Registration

    func registerLocation(_ location: LocationData) {
        guard !location.locationId.isEmpty else { return }
        locations[location.locationId] = location
    }

    func registerVenue(_ venue: VenueData) {
        guard !venue.id.isEmpty else { return }
        venues[venue.id] = venue
    }

    func registerVendor(_ vendor: VendorData) {
        guard !vendor.id.isEmpty else { return }
        vendors[vendor.id] = vendor
    }

    // MARK: - Office

    func getPlayerOffice(stage: Int) -> LocationData? {
        if stage < 2 { return nil }
        if stage == 2 {
            return locations.values.first { $0.locationType == .office && $0.zone == .downtown }
        }
        return locations.values.first { $0.locationType == .office }
    }
}
