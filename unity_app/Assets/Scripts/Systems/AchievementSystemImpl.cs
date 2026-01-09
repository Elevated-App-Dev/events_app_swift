using System;
using System.Collections.Generic;
using System.Linq;
using EventPlannerSim.Core;
using EventPlannerSim.Data;

namespace EventPlannerSim.Systems
{
    /// <summary>
    /// Implementation of the achievement system.
    /// Manages player achievements and platform integration.
    /// Requirements: R37.1-R37.27
    /// </summary>
    public class AchievementSystemImpl : IAchievementSystem
    {
        // Achievement definitions - all achievements from requirements
        private static readonly Dictionary<AchievementType, AchievementDefinition> Definitions = new Dictionary<AchievementType, AchievementDefinition>
        {
            // Progression Achievements (R37.6-R37.10)
            { AchievementType.FirstSteps, new AchievementDefinition(
                AchievementType.FirstSteps,
                "First Steps",
                "Complete your first event",
                AchievementCategory.Progression) },
            
            { AchievementType.RisingStar, new AchievementDefinition(
                AchievementType.RisingStar,
                "Rising Star",
                "Reach Stage 2",
                AchievementCategory.Progression) },
            
            { AchievementType.GoingPro, new AchievementDefinition(
                AchievementType.GoingPro,
                "Going Pro",
                "Reach Stage 3",
                AchievementCategory.Progression) },
            
            { AchievementType.IndustryLeader, new AchievementDefinition(
                AchievementType.IndustryLeader,
                "Industry Leader",
                "Reach Stage 4",
                AchievementCategory.Progression) },
            
            { AchievementType.EventPlanningMogul, new AchievementDefinition(
                AchievementType.EventPlanningMogul,
                "Event Planning Mogul",
                "Reach Stage 5",
                AchievementCategory.Progression) },

            // Mastery Achievements (R37.11-R37.16)
            { AchievementType.PerfectPlanner, new AchievementDefinition(
                AchievementType.PerfectPlanner,
                "Perfect Planner",
                "Achieve 100% satisfaction on any event",
                AchievementCategory.Mastery) },
            
            { AchievementType.ConsistencyIsKey, new AchievementDefinition(
                AchievementType.ConsistencyIsKey,
                "Consistency is Key",
                "Complete 10 events with 80%+ satisfaction",
                AchievementCategory.Mastery,
                targetProgress: 10) },
            
            { AchievementType.ExcellenceStreak, new AchievementDefinition(
                AchievementType.ExcellenceStreak,
                "Excellence Streak",
                "Achieve 5 consecutive 90%+ satisfaction events",
                AchievementCategory.Mastery,
                targetProgress: 5) },
            
            { AchievementType.BudgetMaster, new AchievementDefinition(
                AchievementType.BudgetMaster,
                "Budget Master",
                "Complete 10 events under budget",
                AchievementCategory.Mastery,
                targetProgress: 10) },
            
            { AchievementType.VendorWhisperer, new AchievementDefinition(
                AchievementType.VendorWhisperer,
                "Vendor Whisperer",
                "Reach relationship level 5 with 5 different vendors",
                AchievementCategory.Mastery,
                targetProgress: 5) },
            
            { AchievementType.WeatherWatcher, new AchievementDefinition(
                AchievementType.WeatherWatcher,
                "Weather Watcher",
                "Successfully handle 5 weather-related challenges",
                AchievementCategory.Mastery,
                targetProgress: 5) },

            // Challenge Achievements (R37.17-R37.22)
            { AchievementType.PerfectionistsPerfectionist, new AchievementDefinition(
                AchievementType.PerfectionistsPerfectionist,
                "Perfectionist's Perfectionist",
                "Satisfy a Perfectionist client with 95%+ satisfaction",
                AchievementCategory.Challenge) },
            
            { AchievementType.JugglingAct, new AchievementDefinition(
                AchievementType.JugglingAct,
                "Juggling Act",
                "Successfully complete 3 concurrent events",
                AchievementCategory.Challenge) },
            
            { AchievementType.CrisisManager, new AchievementDefinition(
                AchievementType.CrisisManager,
                "Crisis Manager",
                "Recover from 3 random events using contingency",
                AchievementCategory.Challenge,
                targetProgress: 3) },
            
            { AchievementType.SelfMade, new AchievementDefinition(
                AchievementType.SelfMade,
                "Self-Made",
                "Reach Stage 3 via Entrepreneur Path",
                AchievementCategory.Challenge) },
            
            { AchievementType.CorporateClimber, new AchievementDefinition(
                AchievementType.CorporateClimber,
                "Corporate Climber",
                "Reach Director on Corporate Path",
                AchievementCategory.Challenge) },
            
            { AchievementType.CelebrityHandler, new AchievementDefinition(
                AchievementType.CelebrityHandler,
                "Celebrity Handler",
                "Successfully complete a Celebrity event",
                AchievementCategory.Challenge) },

            // Secret Achievements (R37.23-R37.25)
            { AchievementType.AboveAndBeyond, new AchievementDefinition(
                AchievementType.AboveAndBeyond,
                "Above and Beyond",
                "Trigger hidden reputation bonuses 10 times",
                AchievementCategory.Secret,
                targetProgress: 10,
                isSecret: true,
                hiddenDescription: "Go the extra mile...") },
            
            { AchievementType.FamilyFirst, new AchievementDefinition(
                AchievementType.FamilyFirst,
                "Family First",
                "Never use family emergency funds",
                AchievementCategory.Secret,
                isSecret: true,
                hiddenDescription: "Stand on your own two feet") },
            
            { AchievementType.ComebackKid, new AchievementDefinition(
                AchievementType.ComebackKid,
                "Comeback Kid",
                "Recover from a Financial Crisis",
                AchievementCategory.Secret,
                isSecret: true,
                hiddenDescription: "What doesn't kill you...") }
        };

        // Runtime state
        private readonly Dictionary<AchievementType, AchievementData> _achievements;
        private bool _isSyncedWithPlatform;

        // Events
        public event Action<AchievementData> OnAchievementEarned;
        public event Action<AchievementType, int, int> OnProgressUpdated;

        public AchievementSystemImpl()
        {
            _achievements = new Dictionary<AchievementType, AchievementData>();
            InitializeAchievements();
        }

        /// <summary>
        /// Initialize all achievements from definitions.
        /// </summary>
        private void InitializeAchievements()
        {
            foreach (var kvp in Definitions)
            {
                _achievements[kvp.Key] = new AchievementData(kvp.Value);
            }
        }

        /// <summary>
        /// Initialize the achievement system with saved progress.
        /// </summary>
        public void Initialize(AchievementSaveData saveData)
        {
            if (saveData == null)
            {
                InitializeAchievements();
                return;
            }

            // Restore earned achievements
            foreach (var type in saveData.earnedAchievements)
            {
                if (_achievements.TryGetValue(type, out var achievement))
                {
                    achievement.isEarned = true;
                    var earnedTime = saveData.GetEarnedTime(type);
                    if (earnedTime.HasValue)
                    {
                        achievement.EarnedTime = earnedTime.Value;
                    }
                }
            }

            // Restore progress
            foreach (var entry in saveData.progressEntries)
            {
                if (_achievements.TryGetValue(entry.type, out var achievement))
                {
                    achievement.currentProgress = entry.progress;
                }
            }

            _isSyncedWithPlatform = saveData.isSyncedWithPlatform;
        }

        /// <summary>
        /// Get save data for persistence.
        /// </summary>
        public AchievementSaveData GetSaveData()
        {
            var saveData = new AchievementSaveData
            {
                isSyncedWithPlatform = _isSyncedWithPlatform
            };

            foreach (var kvp in _achievements)
            {
                var achievement = kvp.Value;
                
                if (achievement.isEarned)
                {
                    saveData.earnedAchievements.Add(kvp.Key);
                    saveData.earnedTimestamps.Add(achievement.earnedTimestamp);
                }

                if (achievement.currentProgress > 0)
                {
                    saveData.progressEntries.Add(new AchievementProgressEntry
                    {
                        type = kvp.Key,
                        progress = achievement.currentProgress
                    });
                }
            }

            return saveData;
        }

        /// <summary>
        /// Check if achievement criteria are met and award if so.
        /// Requirements: R37.1
        /// </summary>
        public void CheckAndAward(AchievementType achievement)
        {
            if (!_achievements.TryGetValue(achievement, out var data))
                return;

            if (data.isEarned)
                return;

            // For trackable achievements, check if progress meets target
            if (data.targetProgress > 1)
            {
                if (data.currentProgress >= data.targetProgress)
                {
                    AwardAchievement(data);
                }
            }
            else
            {
                // Non-trackable achievements are awarded directly
                AwardAchievement(data);
            }
        }

        /// <summary>
        /// Award an achievement to the player.
        /// </summary>
        private void AwardAchievement(AchievementData achievement)
        {
            if (achievement.isEarned)
                return;

            achievement.isEarned = true;
            achievement.EarnedTime = DateTime.Now;
            achievement.currentProgress = achievement.targetProgress; // Ensure progress is at max

            // Fire event for UI notification (R37.3)
            OnAchievementEarned?.Invoke(achievement.Clone());

            // Mark as needing sync
            _isSyncedWithPlatform = false;
        }

        /// <summary>
        /// Get current progress for a trackable achievement.
        /// Requirements: R37.27
        /// </summary>
        public AchievementProgress GetProgress(AchievementType achievement)
        {
            if (!_achievements.TryGetValue(achievement, out var data))
            {
                return new AchievementProgress(achievement, 0, 1, false);
            }

            return new AchievementProgress(
                achievement,
                data.currentProgress,
                data.targetProgress,
                data.isEarned);
        }

        /// <summary>
        /// Get all earned achievements.
        /// </summary>
        public List<AchievementData> GetEarnedAchievements()
        {
            return _achievements.Values
                .Where(a => a.isEarned)
                .Select(a => a.Clone())
                .ToList();
        }

        /// <summary>
        /// Get all available achievements with their status.
        /// Requirements: R37.2
        /// </summary>
        public List<AchievementData> GetAllAchievements()
        {
            return _achievements.Values
                .Select(a => {
                    var clone = a.Clone();
                    // Hide description for secret achievements that aren't earned
                    if (clone.isSecret && !clone.isEarned)
                    {
                        var def = Definitions[clone.type];
                        clone.description = def.hiddenDescription;
                        clone.name = "???";
                    }
                    return clone;
                })
                .ToList();
        }

        /// <summary>
        /// Sync achievements with platform services (Game Center, Google Play).
        /// Requirements: R37.5
        /// </summary>
        public void SyncWithPlatform()
        {
            // In a real implementation, this would call platform-specific APIs:
            // - iOS: GameKit/Game Center
            // - Android: Google Play Games Services
            
            // For now, we just mark as synced
            // The actual platform integration would be done in a MonoBehaviour wrapper
            // that has access to Unity's Social API
            
            _isSyncedWithPlatform = true;
        }

        /// <summary>
        /// Increment progress for cumulative achievements.
        /// Requirements: R37.27
        /// </summary>
        public void IncrementProgress(AchievementType achievement, int amount = 1)
        {
            if (!_achievements.TryGetValue(achievement, out var data))
                return;

            if (data.isEarned)
                return;

            int oldProgress = data.currentProgress;
            data.currentProgress = Math.Min(data.currentProgress + amount, data.targetProgress);

            // Fire progress update event
            if (data.currentProgress != oldProgress)
            {
                OnProgressUpdated?.Invoke(achievement, data.currentProgress, data.targetProgress);
            }

            // Check if achievement should be awarded
            if (data.currentProgress >= data.targetProgress)
            {
                AwardAchievement(data);
            }
        }

        /// <summary>
        /// Check if a specific achievement has been earned.
        /// </summary>
        public bool IsAchievementEarned(AchievementType achievement)
        {
            return _achievements.TryGetValue(achievement, out var data) && data.isEarned;
        }

        /// <summary>
        /// Get the category of an achievement.
        /// Requirements: R37.4
        /// </summary>
        public AchievementCategory GetCategory(AchievementType achievement)
        {
            if (Definitions.TryGetValue(achievement, out var def))
            {
                return def.category;
            }
            return AchievementCategory.Progression;
        }

        /// <summary>
        /// Get achievement definition data.
        /// </summary>
        public AchievementDefinition GetDefinition(AchievementType achievement)
        {
            return Definitions.TryGetValue(achievement, out var def) ? def : null;
        }

        /// <summary>
        /// Reset all achievement progress (for new game).
        /// </summary>
        public void ResetAll()
        {
            InitializeAchievements();
            _isSyncedWithPlatform = false;
        }

        /// <summary>
        /// Get achievements by category.
        /// </summary>
        public List<AchievementData> GetAchievementsByCategory(AchievementCategory category)
        {
            return _achievements.Values
                .Where(a => a.category == category)
                .Select(a => {
                    var clone = a.Clone();
                    if (clone.isSecret && !clone.isEarned)
                    {
                        var def = Definitions[clone.type];
                        clone.description = def.hiddenDescription;
                        clone.name = "???";
                    }
                    return clone;
                })
                .ToList();
        }

        /// <summary>
        /// Get count of earned achievements.
        /// </summary>
        public int GetEarnedCount()
        {
            return _achievements.Values.Count(a => a.isEarned);
        }

        /// <summary>
        /// Get total achievement count.
        /// </summary>
        public int GetTotalCount()
        {
            return _achievements.Count;
        }

        /// <summary>
        /// Set progress directly (for loading or special cases).
        /// </summary>
        public void SetProgress(AchievementType achievement, int progress)
        {
            if (!_achievements.TryGetValue(achievement, out var data))
                return;

            if (data.isEarned)
                return;

            data.currentProgress = Math.Min(Math.Max(0, progress), data.targetProgress);

            // Check if achievement should be awarded
            if (data.currentProgress >= data.targetProgress)
            {
                AwardAchievement(data);
            }
        }
    }
}
