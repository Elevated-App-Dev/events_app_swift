using System;
using System.Collections.Generic;
using EventPlannerSim.Core;

namespace EventPlannerSim.Data
{
    /// <summary>
    /// Represents an achievement's current state and data.
    /// Requirements: R37.1-R37.27
    /// </summary>
    [Serializable]
    public class AchievementData
    {
        /// <summary>
        /// The type of achievement.
        /// </summary>
        public AchievementType type;

        /// <summary>
        /// Display name of the achievement.
        /// </summary>
        public string name;

        /// <summary>
        /// Description of how to earn the achievement.
        /// For secret achievements, this is hidden until earned.
        /// </summary>
        public string description;

        /// <summary>
        /// Category of the achievement.
        /// Requirements: R37.4
        /// </summary>
        public AchievementCategory category;

        /// <summary>
        /// Whether this achievement has been earned.
        /// </summary>
        public bool isEarned;

        /// <summary>
        /// When the achievement was earned (ticks for serialization).
        /// </summary>
        public long earnedTimestamp;

        /// <summary>
        /// Current progress towards the achievement (for trackable achievements).
        /// </summary>
        public int currentProgress;

        /// <summary>
        /// Target progress required to earn the achievement.
        /// </summary>
        public int targetProgress;

        /// <summary>
        /// Whether this is a secret achievement (hidden until earned).
        /// Requirements: R37.23-R37.25
        /// </summary>
        public bool isSecret;

        /// <summary>
        /// Icon identifier for the achievement.
        /// </summary>
        public string iconId;

        /// <summary>
        /// DateTime when the achievement was earned.
        /// </summary>
        public DateTime EarnedTime
        {
            get => new DateTime(earnedTimestamp);
            set => earnedTimestamp = value.Ticks;
        }

        /// <summary>
        /// Progress percentage (0-100).
        /// </summary>
        public float ProgressPercent => targetProgress > 0 ? (currentProgress * 100f / targetProgress) : (isEarned ? 100f : 0f);

        /// <summary>
        /// Whether this achievement has trackable progress.
        /// </summary>
        public bool IsTrackable => targetProgress > 1;

        public AchievementData() { }

        public AchievementData(AchievementDefinition definition)
        {
            type = definition.type;
            name = definition.name;
            description = definition.description;
            category = definition.category;
            targetProgress = definition.targetProgress;
            isSecret = definition.isSecret;
            iconId = definition.iconId;
            isEarned = false;
            currentProgress = 0;
        }

        /// <summary>
        /// Creates a copy of this achievement data.
        /// </summary>
        public AchievementData Clone()
        {
            return new AchievementData
            {
                type = this.type,
                name = this.name,
                description = this.description,
                category = this.category,
                isEarned = this.isEarned,
                earnedTimestamp = this.earnedTimestamp,
                currentProgress = this.currentProgress,
                targetProgress = this.targetProgress,
                isSecret = this.isSecret,
                iconId = this.iconId
            };
        }
    }

    /// <summary>
    /// Represents progress towards an achievement.
    /// Requirements: R37.27
    /// </summary>
    [Serializable]
    public class AchievementProgress
    {
        /// <summary>
        /// The achievement type.
        /// </summary>
        public AchievementType type;

        /// <summary>
        /// Current progress value.
        /// </summary>
        public int current;

        /// <summary>
        /// Target progress value required to earn.
        /// </summary>
        public int target;

        /// <summary>
        /// Whether the achievement has been earned.
        /// </summary>
        public bool isComplete;

        /// <summary>
        /// Progress as a percentage (0-100).
        /// </summary>
        public float Percent => target > 0 ? (current * 100f / target) : (isComplete ? 100f : 0f);

        /// <summary>
        /// Display string for progress (e.g., "3/10").
        /// </summary>
        public string DisplayString => $"{current}/{target}";

        public AchievementProgress() { }

        public AchievementProgress(AchievementType type, int current, int target, bool isComplete)
        {
            this.type = type;
            this.current = current;
            this.target = target;
            this.isComplete = isComplete;
        }
    }

    /// <summary>
    /// Static definition of an achievement (immutable).
    /// </summary>
    [Serializable]
    public class AchievementDefinition
    {
        public AchievementType type;
        public string name;
        public string description;
        public string hiddenDescription; // Shown for secret achievements before earned
        public AchievementCategory category;
        public int targetProgress;
        public bool isSecret;
        public string iconId;

        public AchievementDefinition() { }

        public AchievementDefinition(
            AchievementType type,
            string name,
            string description,
            AchievementCategory category,
            int targetProgress = 1,
            bool isSecret = false,
            string hiddenDescription = "???")
        {
            this.type = type;
            this.name = name;
            this.description = description;
            this.hiddenDescription = hiddenDescription;
            this.category = category;
            this.targetProgress = targetProgress;
            this.isSecret = isSecret;
            this.iconId = type.ToString().ToLower();
        }
    }

    /// <summary>
    /// Save data for achievement system persistence.
    /// </summary>
    [Serializable]
    public class AchievementSaveData
    {
        /// <summary>
        /// List of earned achievement types.
        /// </summary>
        public List<AchievementType> earnedAchievements = new List<AchievementType>();

        /// <summary>
        /// Progress entries for trackable achievements.
        /// </summary>
        public List<AchievementProgressEntry> progressEntries = new List<AchievementProgressEntry>();

        /// <summary>
        /// Timestamps when achievements were earned (parallel to earnedAchievements).
        /// </summary>
        public List<long> earnedTimestamps = new List<long>();

        /// <summary>
        /// Whether achievements have been synced with platform.
        /// </summary>
        public bool isSyncedWithPlatform;

        public AchievementSaveData() { }

        /// <summary>
        /// Get progress for a specific achievement type.
        /// </summary>
        public int GetProgress(AchievementType type)
        {
            var entry = progressEntries.Find(e => e.type == type);
            return entry?.progress ?? 0;
        }

        /// <summary>
        /// Set progress for a specific achievement type.
        /// </summary>
        public void SetProgress(AchievementType type, int progress)
        {
            var entry = progressEntries.Find(e => e.type == type);
            if (entry != null)
            {
                entry.progress = progress;
            }
            else
            {
                progressEntries.Add(new AchievementProgressEntry { type = type, progress = progress });
            }
        }

        /// <summary>
        /// Mark an achievement as earned.
        /// </summary>
        public void MarkEarned(AchievementType type, DateTime earnedTime)
        {
            if (!earnedAchievements.Contains(type))
            {
                earnedAchievements.Add(type);
                earnedTimestamps.Add(earnedTime.Ticks);
            }
        }

        /// <summary>
        /// Check if an achievement is earned.
        /// </summary>
        public bool IsEarned(AchievementType type)
        {
            return earnedAchievements.Contains(type);
        }

        /// <summary>
        /// Get the timestamp when an achievement was earned.
        /// </summary>
        public DateTime? GetEarnedTime(AchievementType type)
        {
            int index = earnedAchievements.IndexOf(type);
            if (index >= 0 && index < earnedTimestamps.Count)
            {
                return new DateTime(earnedTimestamps[index]);
            }
            return null;
        }
    }

    /// <summary>
    /// Progress entry for serialization (Unity JsonUtility doesn't support Dictionary).
    /// </summary>
    [Serializable]
    public class AchievementProgressEntry
    {
        public AchievementType type;
        public int progress;
    }
}
