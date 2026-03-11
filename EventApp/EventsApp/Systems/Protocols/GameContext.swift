import Foundation

protocol GameContext: AnyObject {
    var isInitialized: Bool { get }
    var playerData: PlayerData { get }
    var saveData: SaveData { get }
    var timeSystem: TimeSystem { get }
    var mapSystem: MapSystem { get }
}
