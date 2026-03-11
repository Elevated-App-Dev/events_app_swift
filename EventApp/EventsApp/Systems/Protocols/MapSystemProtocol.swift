import Foundation

protocol MapSystemProtocol {
    var currentFilter: LocationType { get }
    var currentZone: MapZone? { get }
    func getVisibleZones(stage: Int) -> [MapZone]
    func getLocationsInZone(_ zone: MapZone) -> [LocationData]
    func navigateToZone(_ zone: MapZone)
    func navigateToLocation(_ locationId: String)
    func setLocationFilter(_ filter: LocationType)
    func getLocationPreview(_ locationId: String) -> LocationPreviewData?
    func isZoneUnlocked(_ zone: MapZone, stage: Int) -> Bool
    func getPlayerOffice(stage: Int) -> LocationData?
    func registerLocation(_ location: LocationData)
    func registerVenue(_ venue: VenueData)
    func registerVendor(_ vendor: VendorData)
}
