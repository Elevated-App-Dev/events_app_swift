import Foundation

enum TextSize: String, CaseIterable, Codable, Hashable {
    case small
    case medium
    case large
}

enum ColorblindMode: String, CaseIterable, Codable, Hashable {
    case none
    case deuteranopia
    case protanopia
    case tritanopia
}
