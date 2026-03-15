import Foundation

/// Persistent game state storage using JSON and FileManager.
/// Replaces Unity's JsonUtility + Application.persistentDataPath.
class SaveSystem: SaveSystemProtocol {

    static let currentVersion = "1.0"
    private let saveFileName = "save.json"
    private let backupFileName = "save_backup.json"

    private var saveFilePath: URL {
        documentsDirectory.appendingPathComponent(saveFileName)
    }

    private var backupFilePath: URL {
        documentsDirectory.appendingPathComponent(backupFileName)
    }

    private var documentsDirectory: URL {
        FileManager.default.urls(for: .documentDirectory, in: .userDomainMask)[0]
    }

    var hasSaveFile: Bool {
        FileManager.default.fileExists(atPath: saveFilePath.path)
    }

    var hasBackupFile: Bool {
        FileManager.default.fileExists(atPath: backupFilePath.path)
    }

    func save(_ data: SaveData) {
        var mutableData = data
        mutableData.lastSavedTimestamp = Date().timeIntervalSince1970
        mutableData.saveVersion = Self.currentVersion

        do {
            let encoder = JSONEncoder()
            encoder.outputFormatting = .prettyPrinted
            let jsonData = try encoder.encode(mutableData)
            try jsonData.write(to: saveFilePath)
            print("[SaveSystem] Game saved successfully")
        } catch {
            print("[SaveSystem] Failed to save game: \(error.localizedDescription)")
        }
    }

    func load() -> SaveData? {
        guard hasSaveFile else {
            print("[SaveSystem] No save file found.")
            return nil
        }

        do {
            let jsonData = try Data(contentsOf: saveFilePath)
            let decoder = JSONDecoder()
            var data = try decoder.decode(SaveData.self, from: jsonData)

            if data.saveVersion != Self.currentVersion {
                print("[SaveSystem] Migrating save from \(data.saveVersion) to \(Self.currentVersion)")
                data = migrateSaveData(data, from: data.saveVersion)
            }

            return data
        } catch {
            print("[SaveSystem] Failed to load game: \(error.localizedDescription)")
            if hasBackupFile {
                print("[SaveSystem] Attempting to restore from backup...")
                return restoreFromBackup()
            }
            return nil
        }
    }

    func deleteSave() {
        let fm = FileManager.default
        try? fm.removeItem(at: saveFilePath)
        try? fm.removeItem(at: backupFilePath)
        print("[SaveSystem] Save files deleted.")
    }

    func createBackup() {
        guard hasSaveFile else { return }
        do {
            let fm = FileManager.default
            if fm.fileExists(atPath: backupFilePath.path) {
                try fm.removeItem(at: backupFilePath)
            }
            try fm.copyItem(at: saveFilePath, to: backupFilePath)
            print("[SaveSystem] Backup created successfully.")
        } catch {
            print("[SaveSystem] Failed to create backup: \(error.localizedDescription)")
        }
    }

    func restoreFromBackup() -> SaveData? {
        guard hasBackupFile else { return nil }
        do {
            let jsonData = try Data(contentsOf: backupFilePath)
            let data = try JSONDecoder().decode(SaveData.self, from: jsonData)
            // Restore backup as main save
            try FileManager.default.copyItem(at: backupFilePath, to: saveFilePath)
            return data
        } catch {
            print("[SaveSystem] Failed to restore from backup: \(error.localizedDescription)")
            return nil
        }
    }

    func validateSaveFile() -> Bool {
        guard hasSaveFile else { return false }
        do {
            let jsonData = try Data(contentsOf: saveFilePath)
            let data = try JSONDecoder().decode(SaveData.self, from: jsonData)
            return !data.saveVersion.isEmpty
        } catch {
            return false
        }
    }

    func getSaveVersion() -> String? {
        guard hasSaveFile else { return nil }
        do {
            let jsonData = try Data(contentsOf: saveFilePath)
            let data = try JSONDecoder().decode(SaveData.self, from: jsonData)
            return data.saveVersion
        } catch {
            return nil
        }
    }

    func migrateSaveData(_ data: SaveData, from version: String) -> SaveData {
        var migrated = data
        // Currently at version 1.0, no migrations needed
        migrated.saveVersion = Self.currentVersion
        return migrated
    }
}
