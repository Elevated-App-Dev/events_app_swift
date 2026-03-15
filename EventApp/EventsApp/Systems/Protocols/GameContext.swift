import Foundation

protocol GameContext: AnyObject {
    var isInitialized: Bool { get }
    var playerData: PlayerData { get }
    var saveData: SaveData { get }
    var advanceSystem: AdvanceSystem { get }
    var mapSystem: MapSystem { get }
}
