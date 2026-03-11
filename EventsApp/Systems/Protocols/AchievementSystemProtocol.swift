import Foundation
import Combine

protocol AchievementSystemProtocol {
    func checkAndAward(_ type: AchievementType)
    func getProgress(_ type: AchievementType) -> AchievementProgress
    func getEarnedAchievements() -> [AchievementData]
    func getAllAchievements() -> [AchievementData]
    func syncWithPlatform()
    func incrementProgress(_ type: AchievementType, by amount: Int)
    func isAchievementEarned(_ type: AchievementType) -> Bool
    func getCategory(_ type: AchievementType) -> AchievementCategory
    func getDefinition(_ type: AchievementType) -> AchievementDefinition
    func initialize(from saveData: AchievementSaveData)
    func getSaveData() -> AchievementSaveData
    func resetAll()
    var onAchievementEarned: AnyPublisher<AchievementData, Never> { get }
    var onProgressUpdated: AnyPublisher<AchievementProgress, Never> { get }
}
