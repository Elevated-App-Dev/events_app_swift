import Foundation

protocol SaveSystemProtocol {
    var hasSaveFile: Bool { get }
    var hasBackupFile: Bool { get }
    func save(_ data: SaveData)
    func load() -> SaveData?
    func deleteSave()
    func createBackup()
    func restoreFromBackup() -> SaveData?
    func validateSaveFile() -> Bool
    func getSaveVersion() -> String?
    func migrateSaveData(_ data: SaveData, fromVersion: String) -> SaveData
}
