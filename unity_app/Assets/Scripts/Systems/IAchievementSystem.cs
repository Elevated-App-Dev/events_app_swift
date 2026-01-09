using System;
using System.Collections.Generic;
using EventPlannerSim.Core;
using EventPlannerSim.Data;

namespace EventPlannerSim.Systems
{
    /// <summary>
    /// Manages player achievements and platform integration.
    /// Requirements: R37.1-R37.5
    /// </summary>
    public interface IAchievementSystem
    {
        /// <summary>
        /// Check if achievement criteria are met and award if so.
        /// Requirements: R37.1
        /// </summary>
        void CheckAndAward(AchievementType achievement);

        /// <summary>
        /// Get current progress for a trackable achievement.
        /// Requirements: R37.27
        /// </summary>
        AchievementProgress GetProgress(AchievementType achievement);

        /// <summary>
        /// Get all earned achievements.
        /// </summary>
        List<AchievementData> GetEarnedAchievements();

        /// <summary>
        /// Get all available achievements with their status.
        /// Requirements: R37.2
        /// </summary>
        List<AchievementData> GetAllAchievements();

        /// <summary>
        /// Sync achievements with platform services (Game Center, Google Play).
        /// Requirements: R37.5
        /// </summary>
        void SyncWithPlatform();

        /// <summary>
        /// Increment progress for cumulative achievements.
        /// Requirements: R37.27
        /// </summary>
        void IncrementProgress(AchievementType achievement, int amount = 1);

        /// <summary>
        /// Check if a specific achievement has been earned.
        /// </summary>
        bool IsAchievementEarned(AchievementType achievement);

        /// <summary>
        /// Get the category of an achievement.
        /// Requirements: R37.4
        /// </summary>
        AchievementCategory GetCategory(AchievementType achievement);

        /// <summary>
        /// Get achievement definition data.
        /// </summary>
        AchievementDefinition GetDefinition(AchievementType achievement);

        /// <summary>
        /// Initialize the achievement system with saved progress.
        /// </summary>
        void Initialize(AchievementSaveData saveData);

        /// <summary>
        /// Get save data for persistence.
        /// </summary>
        AchievementSaveData GetSaveData();

        /// <summary>
        /// Reset all achievement progress (for new game).
        /// </summary>
        void ResetAll();

        /// <summary>
        /// Event fired when an achievement is earned.
        /// Requirements: R37.3
        /// </summary>
        event Action<AchievementData> OnAchievementEarned;

        /// <summary>
        /// Event fired when achievement progress is updated.
        /// </summary>
        event Action<AchievementType, int, int> OnProgressUpdated; // type, current, target
    }
}
