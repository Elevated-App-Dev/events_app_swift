import Foundation

protocol SaveSystemProtocol {
    var hasSaveFile: Bool { get }
    var hasBackupFile: Bool { get }
    func save(_ data: SaveData) throws
    func load() throws -> SaveData?
    func deleteSave() throws
    func createBackup() throws
    func restoreFromBackup() throws -> SaveData?
    func validateSaveFile() -> Bool
    func getSaveVersion() -> String
    func migrateSaveData(_ data: SaveData, fromVersion: String) -> SaveData
}
