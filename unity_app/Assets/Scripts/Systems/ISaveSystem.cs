using EventPlannerSim.Data;

namespace EventPlannerSim.Systems
{
    /// <summary>
    /// Interface for handling persistent game state storage and retrieval.
    /// Requirements: R2.2-R2.6, R27.1-R27.7
    /// </summary>
    public interface ISaveSystem
    {
        /// <summary>
        /// Save current game state to persistent storage.
        /// </summary>
        void Save(SaveData data);

        /// <summary>
        /// Load game state from persistent storage.
        /// Returns null if no save exists.
        /// </summary>
        SaveData Load();

        /// <summary>
        /// Check if a save file exists.
        /// </summary>
        bool HasSaveFile { get; }

        /// <summary>
        /// Delete the current save file.
        /// </summary>
        void DeleteSave();

        /// <summary>
        /// Create a backup of the current save before major operations.
        /// </summary>
        void CreateBackup();

        /// <summary>
        /// Restore from the most recent backup.
        /// Returns null if no backup exists.
        /// </summary>
        SaveData RestoreFromBackup();

        /// <summary>
        /// Check if save file is corrupted.
        /// </summary>
        bool ValidateSaveFile();

        /// <summary>
        /// Get the current save file version.
        /// </summary>
        string GetSaveVersion();

        /// <summary>
        /// Migrate save data from an older version to current.
        /// </summary>
        SaveData MigrateSaveData(SaveData oldData, string fromVersion);

        /// <summary>
        /// Check if a backup file exists.
        /// </summary>
        bool HasBackupFile { get; }
    }
}
