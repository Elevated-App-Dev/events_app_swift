import Foundation

protocol AchievementSystemProtocol {
    func checkAndAward(_ type: AchievementType)
    func incrementProgress(_ type: AchievementType, by amount: Int)
    func getProgress(_ type: AchievementType) -> AchievementProgress
    func isAchievementEarned(_ type: AchievementType) -> Bool
    func getEarnedAchievements() -> [AchievementData]
    func getAllAchievements() -> [AchievementData]
    func getAchievementsByCategory(_ category: AchievementCategory) -> [AchievementData]
    func getEarnedCount() -> Int
    func getTotalCount() -> Int
    func syncWithPlatform()
    func resetAll()
    func getSaveData() -> AchievementSaveData
    func initialize(saveData: AchievementSaveData?)
}
