import Foundation

enum MapZone: String, CaseIterable, Codable, Hashable {
    case neighborhood
    case downtown
    case uptown
    case waterfront
}

enum LocationType: String, CaseIterable, Codable, Hashable {
    case all
    case venue
    case vendor
    case office
    case meetingPoint
}
