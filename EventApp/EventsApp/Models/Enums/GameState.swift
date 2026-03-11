import Foundation

enum GameState: String, CaseIterable, Codable, Hashable {
    case loading
    case mainMenu
    case playing
    case paused
    case tutorial
    case results
    case settings
}
