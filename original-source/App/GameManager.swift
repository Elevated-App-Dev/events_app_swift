import Foundation
import Combine

/// Central game state manager - replaces Unity's GameManager MonoBehaviour singleton.
/// Uses ObservableObject so SwiftUI views react to state changes automatically.
class GameManager: ObservableObject {
    @Published var gameState: GameState = .mainMenu
    @Published var playerData: PlayerData = PlayerData()
    @Published var saveData: SaveData = SaveData()
    @Published var isInitialized: Bool = false

    // Systems (will be populated as implementations are ported)
    // var timeSystem: TimeSystemProtocol
    // var satisfactionCalculator: SatisfactionCalculatorProtocol
    // var eventPlanningSystem: EventPlanningSystemProtocol
    // var referralSystem: ReferralSystemProtocol
    // var consequenceSystem: ConsequenceSystemProtocol
    // var weatherSystem: WeatherSystemProtocol
    // var mapSystem: MapSystemProtocol
    // var phoneSystem: PhoneSystemProtocol
    // var progressionSystem: ProgressionSystemProtocol
    // var profitCalculator: ProfitCalculatorProtocol
    // var achievementSystem: AchievementSystemProtocol
    // var emergencyFundingSystem: EmergencyFundingSystemProtocol

    private var cancellables = Set<AnyCancellable>()

    init() {
        // Systems will be initialized here once implementations are ported
    }

    // MARK: - Game State Management

    func startNewGame() {
        playerData = PlayerData()
        saveData = SaveData()
        saveData.playerData = playerData
        saveData.currentDate = GameDate(month: 3, day: 1, year: 1)
        saveData.journeyStartTime = Date()
        gameState = .tutorial
        isInitialized = true
    }

    func continueGame() {
        // Load from save system
        gameState = .playing
    }

    func pauseGame() {
        gameState = .paused
    }

    func resumeGame() {
        gameState = .playing
    }

    func returnToMainMenu() {
        gameState = .mainMenu
    }
}
