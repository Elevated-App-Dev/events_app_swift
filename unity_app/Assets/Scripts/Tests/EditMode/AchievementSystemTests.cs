using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using EventPlannerSim.Core;
using EventPlannerSim.Data;
using EventPlannerSim.Systems;

namespace EventPlannerSim.Tests.EditMode
{
    /// <summary>
    /// Unit tests for AchievementSystem.
    /// Validates: Requirements R37.1-R37.27
    /// </summary>
    [TestFixture]
    public class AchievementSystemTests
    {
        private AchievementSystemImpl _achievementSystem;

        [SetUp]
        public void Setup()
        {
            _achievementSystem = new AchievementSystemImpl();
        }

        #region Initialization Tests (R37.1)

        /// <summary>
        /// Test that constructor initializes all achievements.
        /// **Validates: Requirements R37.1**
        /// </summary>
        [Test]
        public void Constructor_InitializesAllAchievements()
        {
            var allAchievements = _achievementSystem.GetAllAchievements();
            
            // Should have all 20 achievement types (5 Progression + 6 Mastery + 6 Challenge + 3 Secret)
            Assert.AreEqual(20, allAchievements.Count, "Should have 20 achievements");
        }

        /// <summary>
        /// Test that all achievements start as not earned.
        /// **Validates: Requirements R37.1**
        /// </summary>
        [Test]
        public void Constructor_AllAchievementsStartNotEarned()
        {
            var earnedAchievements = _achievementSystem.GetEarnedAchievements();
            
            Assert.AreEqual(0, earnedAchievements.Count, "No achievements should be earned initially");
        }

        #endregion


        #region CheckAndAward Tests (R37.1, R37.3)

        /// <summary>
        /// Test that CheckAndAward awards non-trackable achievement.
        /// **Validates: Requirements R37.1**
        /// </summary>
        [Test]
        public void CheckAndAward_AwardsNonTrackableAchievement()
        {
            _achievementSystem.CheckAndAward(AchievementType.FirstSteps);
            
            Assert.IsTrue(_achievementSystem.IsAchievementEarned(AchievementType.FirstSteps));
        }

        /// <summary>
        /// Test that CheckAndAward fires OnAchievementEarned event.
        /// **Validates: Requirements R37.3**
        /// </summary>
        [Test]
        public void CheckAndAward_FiresOnAchievementEarnedEvent()
        {
            AchievementData earnedAchievement = null;
            _achievementSystem.OnAchievementEarned += a => earnedAchievement = a;
            
            _achievementSystem.CheckAndAward(AchievementType.FirstSteps);
            
            Assert.IsNotNull(earnedAchievement, "OnAchievementEarned should fire");
            Assert.AreEqual(AchievementType.FirstSteps, earnedAchievement.type);
            Assert.IsTrue(earnedAchievement.isEarned);
        }

        /// <summary>
        /// Test that CheckAndAward does not award already earned achievement.
        /// **Validates: Requirements R37.1**
        /// </summary>
        [Test]
        public void CheckAndAward_DoesNotAwardAlreadyEarned()
        {
            _achievementSystem.CheckAndAward(AchievementType.FirstSteps);
            
            int eventCount = 0;
            _achievementSystem.OnAchievementEarned += _ => eventCount++;
            
            _achievementSystem.CheckAndAward(AchievementType.FirstSteps);
            
            Assert.AreEqual(0, eventCount, "Should not fire event for already earned achievement");
        }

        /// <summary>
        /// Test that CheckAndAward does not award trackable achievement without progress.
        /// **Validates: Requirements R37.1, R37.27**
        /// </summary>
        [Test]
        public void CheckAndAward_DoesNotAwardTrackableWithoutProgress()
        {
            // ConsistencyIsKey requires 10 events
            _achievementSystem.CheckAndAward(AchievementType.ConsistencyIsKey);
            
            Assert.IsFalse(_achievementSystem.IsAchievementEarned(AchievementType.ConsistencyIsKey));
        }

        #endregion

        #region IncrementProgress Tests (R37.27)

        /// <summary>
        /// Test that IncrementProgress increases progress correctly.
        /// **Validates: Requirements R37.27**
        /// </summary>
        [Test]
        public void IncrementProgress_IncreasesProgressCorrectly()
        {
            _achievementSystem.IncrementProgress(AchievementType.ConsistencyIsKey, 3);
            
            var progress = _achievementSystem.GetProgress(AchievementType.ConsistencyIsKey);
            
            Assert.AreEqual(3, progress.current);
            Assert.AreEqual(10, progress.target);
            Assert.IsFalse(progress.isComplete);
        }

        /// <summary>
        /// Test that IncrementProgress fires OnProgressUpdated event.
        /// **Validates: Requirements R37.27**
        /// </summary>
        [Test]
        public void IncrementProgress_FiresOnProgressUpdatedEvent()
        {
            AchievementType? updatedType = null;
            int? currentProgress = null;
            int? targetProgress = null;
            _achievementSystem.OnProgressUpdated += (type, current, target) =>
            {
                updatedType = type;
                currentProgress = current;
                targetProgress = target;
            };
            
            _achievementSystem.IncrementProgress(AchievementType.ConsistencyIsKey, 5);
            
            Assert.AreEqual(AchievementType.ConsistencyIsKey, updatedType);
            Assert.AreEqual(5, currentProgress);
            Assert.AreEqual(10, targetProgress);
        }

        /// <summary>
        /// Test that IncrementProgress awards achievement when target reached.
        /// **Validates: Requirements R37.27**
        /// </summary>
        [Test]
        public void IncrementProgress_AwardsWhenTargetReached()
        {
            AchievementData earnedAchievement = null;
            _achievementSystem.OnAchievementEarned += a => earnedAchievement = a;
            
            _achievementSystem.IncrementProgress(AchievementType.ConsistencyIsKey, 10);
            
            Assert.IsNotNull(earnedAchievement);
            Assert.AreEqual(AchievementType.ConsistencyIsKey, earnedAchievement.type);
            Assert.IsTrue(_achievementSystem.IsAchievementEarned(AchievementType.ConsistencyIsKey));
        }

        /// <summary>
        /// Test that IncrementProgress caps at target.
        /// **Validates: Requirements R37.27**
        /// </summary>
        [Test]
        public void IncrementProgress_CapsAtTarget()
        {
            _achievementSystem.IncrementProgress(AchievementType.ConsistencyIsKey, 15);
            
            var progress = _achievementSystem.GetProgress(AchievementType.ConsistencyIsKey);
            
            Assert.AreEqual(10, progress.current, "Progress should cap at target");
        }

        /// <summary>
        /// Test that IncrementProgress does nothing for earned achievement.
        /// **Validates: Requirements R37.27**
        /// </summary>
        [Test]
        public void IncrementProgress_DoesNothingForEarnedAchievement()
        {
            _achievementSystem.IncrementProgress(AchievementType.ConsistencyIsKey, 10);
            
            int eventCount = 0;
            _achievementSystem.OnProgressUpdated += (_, __, ___) => eventCount++;
            
            _achievementSystem.IncrementProgress(AchievementType.ConsistencyIsKey, 5);
            
            Assert.AreEqual(0, eventCount, "Should not fire event for earned achievement");
        }

        #endregion


        #region GetProgress Tests (R37.27)

        /// <summary>
        /// Test that GetProgress returns correct progress display string.
        /// **Validates: Requirements R37.27**
        /// </summary>
        [Test]
        public void GetProgress_ReturnsCorrectDisplayString()
        {
            _achievementSystem.IncrementProgress(AchievementType.BudgetMaster, 3);
            
            var progress = _achievementSystem.GetProgress(AchievementType.BudgetMaster);
            
            Assert.AreEqual("3/10", progress.DisplayString);
        }

        /// <summary>
        /// Test that GetProgress returns correct percentage.
        /// **Validates: Requirements R37.27**
        /// </summary>
        [Test]
        public void GetProgress_ReturnsCorrectPercentage()
        {
            _achievementSystem.IncrementProgress(AchievementType.BudgetMaster, 5);
            
            var progress = _achievementSystem.GetProgress(AchievementType.BudgetMaster);
            
            Assert.AreEqual(50f, progress.Percent, 0.01f);
        }

        /// <summary>
        /// Test that GetProgress returns 100% for completed achievement.
        /// **Validates: Requirements R37.27**
        /// </summary>
        [Test]
        public void GetProgress_Returns100PercentForCompleted()
        {
            _achievementSystem.IncrementProgress(AchievementType.BudgetMaster, 10);
            
            var progress = _achievementSystem.GetProgress(AchievementType.BudgetMaster);
            
            Assert.AreEqual(100f, progress.Percent, 0.01f);
            Assert.IsTrue(progress.isComplete);
        }

        #endregion

        #region Category Tests (R37.4)

        /// <summary>
        /// Test that GetCategory returns correct category for progression achievements.
        /// **Validates: Requirements R37.4, R37.6-R37.10**
        /// </summary>
        [Test]
        public void GetCategory_ReturnsProgressionForProgressionAchievements()
        {
            Assert.AreEqual(AchievementCategory.Progression, _achievementSystem.GetCategory(AchievementType.FirstSteps));
            Assert.AreEqual(AchievementCategory.Progression, _achievementSystem.GetCategory(AchievementType.RisingStar));
            Assert.AreEqual(AchievementCategory.Progression, _achievementSystem.GetCategory(AchievementType.GoingPro));
            Assert.AreEqual(AchievementCategory.Progression, _achievementSystem.GetCategory(AchievementType.IndustryLeader));
            Assert.AreEqual(AchievementCategory.Progression, _achievementSystem.GetCategory(AchievementType.EventPlanningMogul));
        }

        /// <summary>
        /// Test that GetCategory returns correct category for mastery achievements.
        /// **Validates: Requirements R37.4, R37.11-R37.16**
        /// </summary>
        [Test]
        public void GetCategory_ReturnsMasteryForMasteryAchievements()
        {
            Assert.AreEqual(AchievementCategory.Mastery, _achievementSystem.GetCategory(AchievementType.PerfectPlanner));
            Assert.AreEqual(AchievementCategory.Mastery, _achievementSystem.GetCategory(AchievementType.ConsistencyIsKey));
            Assert.AreEqual(AchievementCategory.Mastery, _achievementSystem.GetCategory(AchievementType.ExcellenceStreak));
            Assert.AreEqual(AchievementCategory.Mastery, _achievementSystem.GetCategory(AchievementType.BudgetMaster));
            Assert.AreEqual(AchievementCategory.Mastery, _achievementSystem.GetCategory(AchievementType.VendorWhisperer));
            Assert.AreEqual(AchievementCategory.Mastery, _achievementSystem.GetCategory(AchievementType.WeatherWatcher));
        }

        /// <summary>
        /// Test that GetCategory returns correct category for challenge achievements.
        /// **Validates: Requirements R37.4, R37.17-R37.22**
        /// </summary>
        [Test]
        public void GetCategory_ReturnsChallengeForChallengeAchievements()
        {
            Assert.AreEqual(AchievementCategory.Challenge, _achievementSystem.GetCategory(AchievementType.PerfectionistsPerfectionist));
            Assert.AreEqual(AchievementCategory.Challenge, _achievementSystem.GetCategory(AchievementType.JugglingAct));
            Assert.AreEqual(AchievementCategory.Challenge, _achievementSystem.GetCategory(AchievementType.CrisisManager));
            Assert.AreEqual(AchievementCategory.Challenge, _achievementSystem.GetCategory(AchievementType.SelfMade));
            Assert.AreEqual(AchievementCategory.Challenge, _achievementSystem.GetCategory(AchievementType.CorporateClimber));
            Assert.AreEqual(AchievementCategory.Challenge, _achievementSystem.GetCategory(AchievementType.CelebrityHandler));
        }

        /// <summary>
        /// Test that GetCategory returns correct category for secret achievements.
        /// **Validates: Requirements R37.4, R37.23-R37.25**
        /// </summary>
        [Test]
        public void GetCategory_ReturnsSecretForSecretAchievements()
        {
            Assert.AreEqual(AchievementCategory.Secret, _achievementSystem.GetCategory(AchievementType.AboveAndBeyond));
            Assert.AreEqual(AchievementCategory.Secret, _achievementSystem.GetCategory(AchievementType.FamilyFirst));
            Assert.AreEqual(AchievementCategory.Secret, _achievementSystem.GetCategory(AchievementType.ComebackKid));
        }

        #endregion


        #region Secret Achievement Tests (R37.23-R37.25)

        /// <summary>
        /// Test that secret achievements have hidden description before earned.
        /// **Validates: Requirements R37.23-R37.25**
        /// </summary>
        [Test]
        public void GetAllAchievements_HidesSecretDescriptionBeforeEarned()
        {
            var achievements = _achievementSystem.GetAllAchievements();
            
            var aboveAndBeyond = achievements.First(a => a.type == AchievementType.AboveAndBeyond);
            
            Assert.AreEqual("???", aboveAndBeyond.name, "Secret achievement name should be hidden");
            Assert.AreNotEqual("Trigger hidden reputation bonuses 10 times", aboveAndBeyond.description);
        }

        /// <summary>
        /// Test that secret achievements show description after earned.
        /// **Validates: Requirements R37.23-R37.25**
        /// </summary>
        [Test]
        public void GetAllAchievements_ShowsSecretDescriptionAfterEarned()
        {
            _achievementSystem.IncrementProgress(AchievementType.AboveAndBeyond, 10);
            
            var achievements = _achievementSystem.GetAllAchievements();
            var aboveAndBeyond = achievements.First(a => a.type == AchievementType.AboveAndBeyond);
            
            Assert.AreEqual("Above and Beyond", aboveAndBeyond.name);
            Assert.AreEqual("Trigger hidden reputation bonuses 10 times", aboveAndBeyond.description);
        }

        #endregion

        #region Save/Load Tests

        /// <summary>
        /// Test that GetSaveData returns correct data.
        /// **Validates: Requirements R37.1**
        /// </summary>
        [Test]
        public void GetSaveData_ReturnsCorrectData()
        {
            _achievementSystem.CheckAndAward(AchievementType.FirstSteps);
            _achievementSystem.IncrementProgress(AchievementType.ConsistencyIsKey, 5);
            
            var saveData = _achievementSystem.GetSaveData();
            
            Assert.Contains(AchievementType.FirstSteps, saveData.earnedAchievements);
            Assert.AreEqual(5, saveData.GetProgress(AchievementType.ConsistencyIsKey));
        }

        /// <summary>
        /// Test that Initialize restores state from save data.
        /// **Validates: Requirements R37.1**
        /// </summary>
        [Test]
        public void Initialize_RestoresStateFromSaveData()
        {
            var saveData = new AchievementSaveData();
            saveData.MarkEarned(AchievementType.FirstSteps, DateTime.Now);
            saveData.SetProgress(AchievementType.ConsistencyIsKey, 7);
            
            _achievementSystem.Initialize(saveData);
            
            Assert.IsTrue(_achievementSystem.IsAchievementEarned(AchievementType.FirstSteps));
            Assert.AreEqual(7, _achievementSystem.GetProgress(AchievementType.ConsistencyIsKey).current);
        }

        /// <summary>
        /// Test that ResetAll clears all progress.
        /// **Validates: Requirements R37.1**
        /// </summary>
        [Test]
        public void ResetAll_ClearsAllProgress()
        {
            _achievementSystem.CheckAndAward(AchievementType.FirstSteps);
            _achievementSystem.IncrementProgress(AchievementType.ConsistencyIsKey, 5);
            
            _achievementSystem.ResetAll();
            
            Assert.IsFalse(_achievementSystem.IsAchievementEarned(AchievementType.FirstSteps));
            Assert.AreEqual(0, _achievementSystem.GetProgress(AchievementType.ConsistencyIsKey).current);
        }

        #endregion

        #region GetEarnedAchievements Tests (R37.2)

        /// <summary>
        /// Test that GetEarnedAchievements returns only earned achievements.
        /// **Validates: Requirements R37.2**
        /// </summary>
        [Test]
        public void GetEarnedAchievements_ReturnsOnlyEarned()
        {
            _achievementSystem.CheckAndAward(AchievementType.FirstSteps);
            _achievementSystem.CheckAndAward(AchievementType.RisingStar);
            
            var earned = _achievementSystem.GetEarnedAchievements();
            
            Assert.AreEqual(2, earned.Count);
            Assert.IsTrue(earned.All(a => a.isEarned));
            Assert.IsTrue(earned.Any(a => a.type == AchievementType.FirstSteps));
            Assert.IsTrue(earned.Any(a => a.type == AchievementType.RisingStar));
        }

        #endregion

        #region SyncWithPlatform Tests (R37.5)

        /// <summary>
        /// Test that SyncWithPlatform can be called without error.
        /// **Validates: Requirements R37.5**
        /// </summary>
        [Test]
        public void SyncWithPlatform_CanBeCalledWithoutError()
        {
            _achievementSystem.CheckAndAward(AchievementType.FirstSteps);
            
            Assert.DoesNotThrow(() => _achievementSystem.SyncWithPlatform());
        }

        #endregion

        #region GetDefinition Tests

        /// <summary>
        /// Test that GetDefinition returns correct definition.
        /// **Validates: Requirements R37.6-R37.25**
        /// </summary>
        [Test]
        public void GetDefinition_ReturnsCorrectDefinition()
        {
            var definition = _achievementSystem.GetDefinition(AchievementType.FirstSteps);
            
            Assert.IsNotNull(definition);
            Assert.AreEqual(AchievementType.FirstSteps, definition.type);
            Assert.AreEqual("First Steps", definition.name);
            Assert.AreEqual("Complete your first event", definition.description);
            Assert.AreEqual(AchievementCategory.Progression, definition.category);
        }

        /// <summary>
        /// Test that trackable achievements have correct target progress.
        /// **Validates: Requirements R37.12-R37.16**
        /// </summary>
        [Test]
        public void GetDefinition_TrackableAchievementsHaveCorrectTarget()
        {
            Assert.AreEqual(10, _achievementSystem.GetDefinition(AchievementType.ConsistencyIsKey).targetProgress);
            Assert.AreEqual(5, _achievementSystem.GetDefinition(AchievementType.ExcellenceStreak).targetProgress);
            Assert.AreEqual(10, _achievementSystem.GetDefinition(AchievementType.BudgetMaster).targetProgress);
            Assert.AreEqual(5, _achievementSystem.GetDefinition(AchievementType.VendorWhisperer).targetProgress);
            Assert.AreEqual(5, _achievementSystem.GetDefinition(AchievementType.WeatherWatcher).targetProgress);
            Assert.AreEqual(3, _achievementSystem.GetDefinition(AchievementType.CrisisManager).targetProgress);
            Assert.AreEqual(10, _achievementSystem.GetDefinition(AchievementType.AboveAndBeyond).targetProgress);
        }

        #endregion
    }
}
