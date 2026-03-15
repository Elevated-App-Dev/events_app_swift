import Foundation

protocol GameContext {
    var isInitialized: Bool { get }
    var currentPlayer: PlayerData { get }
    var currentSaveData: SaveData { get }
    var timeSystem: TimeSystemProtocol { get }
    var mapSystem: MapSystemProtocol { get }
}
