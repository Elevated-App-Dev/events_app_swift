using System;
using System.IO;
using UnityEngine;
using EventPlannerSim.Data;

namespace EventPlannerSim.Systems
{
    /// <summary>
    /// Implementation of ISaveSystem for persistent game state storage.
    /// Uses Unity's JsonUtility for serialization and Application.persistentDataPath for storage.
    /// Requirements: R2.2-R2.6, R27.1-R27.7
    /// </summary>
    public class SaveSystemImpl : ISaveSystem
    {
        /// <summary>
        /// Current save file version for migration support.
        /// </summary>
        public const string CurrentVersion = "1.0";

        /// <summary>
        /// Name of the main save file.
        /// </summary>
        private const string SaveFileName = "save.json";

        /// <summary>
        /// Name of the backup save file.
        /// </summary>
        private const string BackupFileName = "save_backup.json";

        /// <summary>
        /// Full path to the save file.
        /// </summary>
        private string SaveFilePath => Path.Combine(Application.persistentDataPath, SaveFileName);

        /// <summary>
        /// Full path to the backup file.
        /// </summary>
        private string BackupFilePath => Path.Combine(Application.persistentDataPath, BackupFileName);

        /// <summary>
        /// Check if a save file exists.
        /// </summary>
        public bool HasSaveFile => File.Exists(SaveFilePath);

        /// <summary>
        /// Check if a backup file exists.
        /// </summary>
        public bool HasBackupFile => File.Exists(BackupFilePath);

        /// <summary>
        /// Save current game state to persistent storage.
        /// Requirements: R27.1, R2.4
        /// </summary>
        public void Save(SaveData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            try
            {
                // Update timestamp
                data.LastSavedTime = DateTime.UtcNow;
                data.saveVersion = CurrentVersion;

                // Serialize to JSON
                string json = JsonUtility.ToJson(data, prettyPrint: true);

                // Write to file
                File.WriteAllText(SaveFilePath, json);

                Debug.Log($"[SaveSystem] Game saved successfully at {data.LastSavedTime}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveSystem] Failed to save game: {ex.Message}");
                throw;
            }
        }


        /// <summary>
        /// Load game state from persistent storage.
        /// Returns null if no save exists.
        /// Requirements: R27.2, R2.2
        /// </summary>
        public SaveData Load()
        {
            if (!HasSaveFile)
            {
                Debug.Log("[SaveSystem] No save file found.");
                return null;
            }

            try
            {
                string json = File.ReadAllText(SaveFilePath);
                SaveData data = JsonUtility.FromJson<SaveData>(json);

                if (data == null)
                {
                    Debug.LogWarning("[SaveSystem] Failed to deserialize save file.");
                    return null;
                }

                // Check for version migration
                if (data.saveVersion != CurrentVersion)
                {
                    Debug.Log($"[SaveSystem] Migrating save from version {data.saveVersion} to {CurrentVersion}");
                    data = MigrateSaveData(data, data.saveVersion);
                }

                Debug.Log($"[SaveSystem] Game loaded successfully. Last saved: {data.LastSavedTime}");
                return data;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveSystem] Failed to load game: {ex.Message}");
                
                // Attempt recovery from backup (R27.6)
                if (HasBackupFile)
                {
                    Debug.Log("[SaveSystem] Attempting to restore from backup...");
                    return RestoreFromBackup();
                }

                return null;
            }
        }

        /// <summary>
        /// Delete the current save file.
        /// Requirements: R35.14, R35.25
        /// </summary>
        public void DeleteSave()
        {
            try
            {
                if (File.Exists(SaveFilePath))
                {
                    File.Delete(SaveFilePath);
                    Debug.Log("[SaveSystem] Save file deleted.");
                }

                if (File.Exists(BackupFilePath))
                {
                    File.Delete(BackupFilePath);
                    Debug.Log("[SaveSystem] Backup file deleted.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveSystem] Failed to delete save files: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Create a backup of the current save before major operations.
        /// Requirements: R27.7
        /// </summary>
        public void CreateBackup()
        {
            if (!HasSaveFile)
            {
                Debug.Log("[SaveSystem] No save file to backup.");
                return;
            }

            try
            {
                File.Copy(SaveFilePath, BackupFilePath, overwrite: true);
                Debug.Log("[SaveSystem] Backup created successfully.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveSystem] Failed to create backup: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Restore from the most recent backup.
        /// Returns null if no backup exists.
        /// Requirements: R27.6
        /// </summary>
        public SaveData RestoreFromBackup()
        {
            if (!HasBackupFile)
            {
                Debug.Log("[SaveSystem] No backup file found.");
                return null;
            }

            try
            {
                string json = File.ReadAllText(BackupFilePath);
                SaveData data = JsonUtility.FromJson<SaveData>(json);

                if (data != null)
                {
                    // Restore backup as main save
                    File.Copy(BackupFilePath, SaveFilePath, overwrite: true);
                    Debug.Log("[SaveSystem] Restored from backup successfully.");
                }

                return data;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveSystem] Failed to restore from backup: {ex.Message}");
                return null;
            }
        }


        /// <summary>
        /// Check if save file is corrupted.
        /// Requirements: R2.5
        /// </summary>
        public bool ValidateSaveFile()
        {
            if (!HasSaveFile)
                return false;

            try
            {
                string json = File.ReadAllText(SaveFilePath);
                
                // Check for empty file
                if (string.IsNullOrWhiteSpace(json))
                    return false;

                // Attempt to deserialize
                SaveData data = JsonUtility.FromJson<SaveData>(json);
                
                // Validate essential fields
                if (data == null)
                    return false;

                if (data.playerData == null)
                    return false;

                if (string.IsNullOrEmpty(data.saveVersion))
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[SaveSystem] Save file validation failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Get the current save file version.
        /// Requirements: R27.3
        /// </summary>
        public string GetSaveVersion()
        {
            if (!HasSaveFile)
                return null;

            try
            {
                string json = File.ReadAllText(SaveFilePath);
                SaveData data = JsonUtility.FromJson<SaveData>(json);
                return data?.saveVersion;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Migrate save data from an older version to current.
        /// Requirements: R27.4
        /// </summary>
        public SaveData MigrateSaveData(SaveData oldData, string fromVersion)
        {
            if (oldData == null)
                return null;

            // Currently at version 1.0, no migrations needed yet
            // Future migrations would be handled here:
            // if (fromVersion == "0.9") { /* migrate from 0.9 to 1.0 */ }

            switch (fromVersion)
            {
                case "1.0":
                    // Current version, no migration needed
                    break;

                default:
                    Debug.LogWarning($"[SaveSystem] Unknown save version: {fromVersion}. Attempting to use as-is.");
                    break;
            }

            // Update version to current
            oldData.saveVersion = CurrentVersion;
            return oldData;
        }
    }
}
