using System.Linq;
using NUnit.Framework;
using EventPlannerSim.Core;
using EventPlannerSim.Data;
using EventPlannerSim.Managers;
using EventPlannerSim.Systems;

namespace EventPlannerSim.Tests.EditMode
{
    /// <summary>
    /// Tests for system initialization order and interactions.
    /// Requirements: R1, R2
    /// </summary>
    [TestFixture]
    public class SystemInitializationTests
    {
        #region Core Systems Initialization Tests

        [Test]
        public void SaveSystemImpl_ShouldInstantiateWithoutError()
        {
            var saveSystem = new SaveSystemImpl();
            Assert.IsNotNull(saveSystem);
        }

        [Test]
        public void TimeSystemImpl_ShouldInstantiateWithoutError()
        {
            var timeSystem = new TimeSystemImpl();
            Assert.IsNotNull(timeSystem);
        }

        [Test]
        public void WeatherSystemImpl_ShouldInstantiateWithoutError()
        {
            var weatherSystem = new WeatherSystemImpl();
            Assert.IsNotNull(weatherSystem);
        }

        #endregion

        #region Game Systems Initialization Tests

        [Test]
        public void SatisfactionCalculatorImpl_ShouldInstantiateWithoutError()
        {
            var calculator = new SatisfactionCalculatorImpl();
            Assert.IsNotNull(calculator);
        }

        [Test]
        public void ProgressionSystemImpl_ShouldInstantiateWithoutError()
        {
            var progressionSystem = new ProgressionSystemImpl();
            Assert.IsNotNull(progressionSystem);
        }

        [Test]
        public void ConsequenceSystemImpl_ShouldInstantiateWithoutError()
        {
            var consequenceSystem = new ConsequenceSystemImpl();
            Assert.IsNotNull(consequenceSystem);
        }

        [Test]
        public void EventPlanningSystemImpl_ShouldInstantiateWithoutError()
        {
            var eventPlanningSystem = new EventPlanningSystemImpl();
            Assert.IsNotNull(eventPlanningSystem);
        }

        [Test]
        public void MapSystemImpl_ShouldInstantiateWithoutError()
        {
            var mapSystem = new MapSystemImpl();
            Assert.IsNotNull(mapSystem);
        }

        [Test]
        public void ReferralSystemImpl_ShouldInstantiateWithoutError()
        {
            var referralSystem = new ReferralSystemImpl();
            Assert.IsNotNull(referralSystem);
        }

        [Test]
        public void ProfitCalculatorImpl_ShouldInstantiateWithoutError()
        {
            var profitCalculator = new ProfitCalculatorImpl();
            Assert.IsNotNull(profitCalculator);
        }

        [Test]
        public void EmergencyFundingSystemImpl_ShouldInstantiateWithoutError()
        {
            var emergencyFundingSystem = new EmergencyFundingSystemImpl();
            Assert.IsNotNull(emergencyFundingSystem);
        }

        [Test]
        public void MilestoneSystemImpl_ShouldInstantiateWithoutError()
        {
            var milestoneSystem = new MilestoneSystemImpl();
            Assert.IsNotNull(milestoneSystem);
        }

        #endregion

        #region Extension Systems (Stubs) Initialization Tests

        [Test]
        public void StaffSystemImpl_ShouldInstantiateWithoutError()
        {
            var staffSystem = new StaffSystemImpl();
            Assert.IsNotNull(staffSystem);
        }

        [Test]
        public void MarketingSystemImpl_ShouldInstantiateWithoutError()
        {
            var marketingSystem = new MarketingSystemImpl();
            Assert.IsNotNull(marketingSystem);
        }

        [Test]
        public void OfficeSystemImpl_ShouldInstantiateWithoutError()
        {
            var officeSystem = new OfficeSystemImpl();
            Assert.IsNotNull(officeSystem);
        }

        [Test]
        public void StaffSystemImpl_GetAllStaff_ShouldReturnEmptyList()
        {
            var staffSystem = new StaffSystemImpl();
            var staff = staffSystem.GetAllStaff();
            
            Assert.IsNotNull(staff);
            Assert.AreEqual(0, staff.Count);
        }

        [Test]
        public void MarketingSystemImpl_GetInquiryRateModifier_ShouldReturn1()
        {
            var marketingSystem = new MarketingSystemImpl();
            var modifier = marketingSystem.GetInquiryRateModifier();
            
            Assert.AreEqual(1.0f, modifier);
        }

        [Test]
        public void OfficeSystemImpl_GetEfficiencyBonus_ShouldReturn0()
        {
            var officeSystem = new OfficeSystemImpl();
            var bonus = officeSystem.GetEfficiencyBonus();
            
            Assert.AreEqual(0f, bonus);
        }

        #endregion

        #region UI Systems Initialization Tests

        [Test]
        public void PhoneSystemImpl_ShouldInstantiateWithoutError()
        {
            var phoneSystem = new PhoneSystemImpl();
            Assert.IsNotNull(phoneSystem);
        }

        [Test]
        public void TutorialSystemImpl_ShouldInstantiateWithoutError()
        {
            var tutorialSystem = new TutorialSystemImpl();
            Assert.IsNotNull(tutorialSystem);
        }

        [Test]
        public void AudioManagerImpl_ShouldInstantiateWithoutError()
        {
            var audioManager = new AudioManagerImpl();
            Assert.IsNotNull(audioManager);
        }

        #endregion

        #region Platform Systems Initialization Tests

        [Test]
        public void MonetizationSystemImpl_ShouldInstantiateWithoutError()
        {
            var monetizationSystem = new MonetizationSystemImpl();
            Assert.IsNotNull(monetizationSystem);
        }

        [Test]
        public void UnityServicesManagerImpl_ShouldInstantiateWithoutError()
        {
            var unityServices = new UnityServicesManagerImpl();
            Assert.IsNotNull(unityServices);
        }

        [Test]
        public void NotificationSystemImpl_ShouldInstantiateWithoutError()
        {
            var notificationSystem = new NotificationSystemImpl();
            Assert.IsNotNull(notificationSystem);
        }

        [Test]
        public void AchievementSystemImpl_ShouldInstantiateWithoutError()
        {
            var achievementSystem = new AchievementSystemImpl();
            Assert.IsNotNull(achievementSystem);
        }

        #endregion

        #region System Interaction Tests

        [Test]
        public void TimeSystem_SetCurrentDate_ShouldUpdateCurrentDate()
        {
            var timeSystem = new TimeSystemImpl();
            var testDate = new GameDate(15, 6, 2);
            
            timeSystem.SetCurrentDate(testDate);
            
            Assert.AreEqual(testDate, timeSystem.CurrentDate);
        }

        [Test]
        public void WeatherSystem_SetCurrentDate_ShouldUpdateCurrentDate()
        {
            var weatherSystem = new WeatherSystemImpl();
            var testDate = new GameDate(10, 3, 1);
            
            weatherSystem.SetCurrentDate(testDate);
            
            Assert.AreEqual(testDate, weatherSystem.CurrentDate);
        }

        [Test]
        public void ProgressionSystem_CanAdvanceStage_WithNewPlayer_ShouldReturnFalse()
        {
            var progressionSystem = new ProgressionSystemImpl();
            var player = new PlayerData
            {
                stage = BusinessStage.Solo,
                reputation = 0,
                money = 500f
            };
            
            var canAdvance = progressionSystem.CanAdvanceStage(player);
            
            Assert.IsFalse(canAdvance);
        }

        [Test]
        public void SatisfactionCalculator_CalculateCategoryScore_ShouldReturnValidScore()
        {
            var calculator = new SatisfactionCalculatorImpl();
            var eventData = new EventData
            {
                id = "test-event",
                budget = new EventBudget { total = 5000f }
            };
            
            var score = calculator.CalculateCategoryScore(eventData, BudgetCategory.Venue);
            
            Assert.GreaterOrEqual(score, 0f);
            Assert.LessOrEqual(score, 100f);
        }

        [Test]
        public void MapSystem_GetVisibleZones_Stage1_ShouldReturnNeighborhood()
        {
            var mapSystem = new MapSystemImpl();
            
            var zones = mapSystem.GetVisibleZones(1);
            
            Assert.IsNotNull(zones);
            Assert.IsTrue(zones.Contains(MapZone.Neighborhood));
        }

        [Test]
        public void ReferralSystem_CalculateReferralProbability_HighSatisfaction_ShouldReturnHighProbability()
        {
            var referralSystem = new ReferralSystemImpl();
            
            var probability = referralSystem.CalculateReferralProbability(95f, 0);
            
            Assert.Greater(probability, 0.5f);
        }

        [Test]
        public void ProfitCalculator_CalculateProfitMargin_HighSatisfaction_ShouldReturnPositiveMargin()
        {
            var profitCalculator = new ProfitCalculatorImpl();
            
            var result = profitCalculator.CalculateProfit(5000f, 85f);
            
            Assert.Greater(result.ProfitMarginPercent, 0f);
        }

        [Test]
        public void EmergencyFundingSystem_RequestFamilyHelp_FirstRequest_ShouldReturn500()
        {
            var emergencyFundingSystem = new EmergencyFundingSystemImpl();
            
            var result = emergencyFundingSystem.RequestFamilyHelp(BusinessStage.Solo, 0);
            
            Assert.IsTrue(result.Success);
            Assert.AreEqual(500f, result.AmountReceived);
        }

        #endregion
    }


    /// <summary>
    /// Tests for GameStateManager state transitions and online/offline handling.
    /// Requirements: R2.7-R2.9
    /// </summary>
    [TestFixture]
    public class GameStateManagerTests
    {
        private GameStateManager _stateManager;

        [SetUp]
        public void SetUp()
        {
            _stateManager = new GameStateManager();
        }

        #region Initial State Tests

        [Test]
        public void InitialState_ShouldBeLoading()
        {
            Assert.AreEqual(GameState.Loading, _stateManager.CurrentState);
        }

        [Test]
        public void InitialOnlineStatus_ShouldBeTrue()
        {
            Assert.IsTrue(_stateManager.IsOnline);
        }

        #endregion

        #region Valid State Transition Tests

        [Test]
        public void TrySetState_FromLoadingToMainMenu_ShouldSucceed()
        {
            bool result = _stateManager.TrySetState(GameState.MainMenu);
            
            Assert.IsTrue(result);
            Assert.AreEqual(GameState.MainMenu, _stateManager.CurrentState);
        }

        [Test]
        public void TrySetState_FromLoadingToPlaying_ShouldSucceed()
        {
            bool result = _stateManager.TrySetState(GameState.Playing);
            
            Assert.IsTrue(result);
            Assert.AreEqual(GameState.Playing, _stateManager.CurrentState);
        }

        [Test]
        public void TrySetState_FromLoadingToTutorial_ShouldSucceed()
        {
            bool result = _stateManager.TrySetState(GameState.Tutorial);
            
            Assert.IsTrue(result);
            Assert.AreEqual(GameState.Tutorial, _stateManager.CurrentState);
        }

        [Test]
        public void TrySetState_FromPlayingToPaused_ShouldSucceed()
        {
            _stateManager.TrySetState(GameState.Playing);
            
            bool result = _stateManager.TrySetState(GameState.Paused);
            
            Assert.IsTrue(result);
            Assert.AreEqual(GameState.Paused, _stateManager.CurrentState);
        }

        [Test]
        public void TrySetState_FromPausedToPlaying_ShouldSucceed()
        {
            _stateManager.TrySetState(GameState.Playing);
            _stateManager.TrySetState(GameState.Paused);
            
            bool result = _stateManager.TrySetState(GameState.Playing);
            
            Assert.IsTrue(result);
            Assert.AreEqual(GameState.Playing, _stateManager.CurrentState);
        }

        [Test]
        public void TrySetState_FromPlayingToSettings_ShouldSucceed()
        {
            _stateManager.TrySetState(GameState.Playing);
            
            bool result = _stateManager.TrySetState(GameState.Settings);
            
            Assert.IsTrue(result);
            Assert.AreEqual(GameState.Settings, _stateManager.CurrentState);
        }

        [Test]
        public void TrySetState_FromPlayingToResults_ShouldSucceed()
        {
            _stateManager.TrySetState(GameState.Playing);
            
            bool result = _stateManager.TrySetState(GameState.Results);
            
            Assert.IsTrue(result);
            Assert.AreEqual(GameState.Results, _stateManager.CurrentState);
        }

        [Test]
        public void TrySetState_FromResultsToPlaying_ShouldSucceed()
        {
            _stateManager.TrySetState(GameState.Playing);
            _stateManager.TrySetState(GameState.Results);
            
            bool result = _stateManager.TrySetState(GameState.Playing);
            
            Assert.IsTrue(result);
            Assert.AreEqual(GameState.Playing, _stateManager.CurrentState);
        }

        #endregion

        #region Invalid State Transition Tests

        [Test]
        public void TrySetState_FromLoadingToResults_ShouldFail()
        {
            bool result = _stateManager.TrySetState(GameState.Results);
            
            Assert.IsFalse(result);
            Assert.AreEqual(GameState.Loading, _stateManager.CurrentState);
        }

        [Test]
        public void TrySetState_FromLoadingToPaused_ShouldFail()
        {
            bool result = _stateManager.TrySetState(GameState.Paused);
            
            Assert.IsFalse(result);
            Assert.AreEqual(GameState.Loading, _stateManager.CurrentState);
        }

        [Test]
        public void TrySetState_FromResultsToPaused_ShouldFail()
        {
            _stateManager.TrySetState(GameState.Playing);
            _stateManager.TrySetState(GameState.Results);
            
            bool result = _stateManager.TrySetState(GameState.Paused);
            
            Assert.IsFalse(result);
            Assert.AreEqual(GameState.Results, _stateManager.CurrentState);
        }

        #endregion

        #region Same State Transition Tests

        [Test]
        public void TrySetState_ToSameState_ShouldReturnTrueWithoutChange()
        {
            _stateManager.TrySetState(GameState.Playing);
            
            bool result = _stateManager.TrySetState(GameState.Playing);
            
            Assert.IsTrue(result);
            Assert.AreEqual(GameState.Playing, _stateManager.CurrentState);
        }

        #endregion

        #region Previous State Tests

        [Test]
        public void PreviousState_AfterTransition_ShouldBeOldState()
        {
            _stateManager.TrySetState(GameState.Playing);
            _stateManager.TrySetState(GameState.Paused);
            
            Assert.AreEqual(GameState.Playing, _stateManager.PreviousState);
        }

        [Test]
        public void ReturnToPreviousState_ShouldRestorePreviousState()
        {
            _stateManager.TrySetState(GameState.Playing);
            _stateManager.TrySetState(GameState.Paused);
            
            bool result = _stateManager.ReturnToPreviousState();
            
            Assert.IsTrue(result);
            Assert.AreEqual(GameState.Playing, _stateManager.CurrentState);
        }

        #endregion

        #region Convenience Method Tests

        [Test]
        public void Pause_WhenPlaying_ShouldTransitionToPaused()
        {
            _stateManager.TrySetState(GameState.Playing);
            
            bool result = _stateManager.Pause();
            
            Assert.IsTrue(result);
            Assert.AreEqual(GameState.Paused, _stateManager.CurrentState);
        }

        [Test]
        public void Pause_WhenNotPlaying_ShouldReturnFalse()
        {
            _stateManager.TrySetState(GameState.MainMenu);
            
            bool result = _stateManager.Pause();
            
            Assert.IsFalse(result);
            Assert.AreEqual(GameState.MainMenu, _stateManager.CurrentState);
        }

        [Test]
        public void Resume_WhenPaused_ShouldTransitionToPlaying()
        {
            _stateManager.TrySetState(GameState.Playing);
            _stateManager.TrySetState(GameState.Paused);
            
            bool result = _stateManager.Resume();
            
            Assert.IsTrue(result);
            Assert.AreEqual(GameState.Playing, _stateManager.CurrentState);
        }

        [Test]
        public void Resume_WhenNotPaused_ShouldReturnFalse()
        {
            _stateManager.TrySetState(GameState.Playing);
            
            bool result = _stateManager.Resume();
            
            Assert.IsFalse(result);
            Assert.AreEqual(GameState.Playing, _stateManager.CurrentState);
        }

        [Test]
        public void OpenSettings_FromPlaying_ShouldSucceed()
        {
            _stateManager.TrySetState(GameState.Playing);
            
            bool result = _stateManager.OpenSettings();
            
            Assert.IsTrue(result);
            Assert.AreEqual(GameState.Settings, _stateManager.CurrentState);
        }

        [Test]
        public void CloseSettings_ShouldReturnToPreviousState()
        {
            _stateManager.TrySetState(GameState.Playing);
            _stateManager.OpenSettings();
            
            bool result = _stateManager.CloseSettings();
            
            Assert.IsTrue(result);
            Assert.AreEqual(GameState.Playing, _stateManager.CurrentState);
        }

        #endregion

        #region Online/Offline Tests

        [Test]
        public void SetOnlineStatus_ToFalse_ShouldUpdateIsOnline()
        {
            _stateManager.SetOnlineStatus(false);
            
            Assert.IsFalse(_stateManager.IsOnline);
        }

        [Test]
        public void SetOnlineStatus_ToTrue_ShouldUpdateIsOnline()
        {
            _stateManager.SetOnlineStatus(false);
            _stateManager.SetOnlineStatus(true);
            
            Assert.IsTrue(_stateManager.IsOnline);
        }

        [Test]
        public void AreNetworkFeaturesAvailable_WhenOnline_ShouldBeTrue()
        {
            _stateManager.SetOnlineStatus(true);
            
            Assert.IsTrue(_stateManager.AreNetworkFeaturesAvailable);
        }

        [Test]
        public void AreNetworkFeaturesAvailable_WhenOffline_ShouldBeFalse()
        {
            _stateManager.SetOnlineStatus(false);
            
            Assert.IsFalse(_stateManager.AreNetworkFeaturesAvailable);
        }

        #endregion

        #region Property Tests

        [Test]
        public void IsPlayable_WhenPlaying_ShouldBeTrue()
        {
            _stateManager.TrySetState(GameState.Playing);
            
            Assert.IsTrue(_stateManager.IsPlayable);
        }

        [Test]
        public void IsPlayable_WhenTutorial_ShouldBeTrue()
        {
            _stateManager.TrySetState(GameState.Tutorial);
            
            Assert.IsTrue(_stateManager.IsPlayable);
        }

        [Test]
        public void IsPlayable_WhenPaused_ShouldBeFalse()
        {
            _stateManager.TrySetState(GameState.Playing);
            _stateManager.TrySetState(GameState.Paused);
            
            Assert.IsFalse(_stateManager.IsPlayable);
        }

        [Test]
        public void IsPaused_WhenPaused_ShouldBeTrue()
        {
            _stateManager.TrySetState(GameState.Playing);
            _stateManager.TrySetState(GameState.Paused);
            
            Assert.IsTrue(_stateManager.IsPaused);
        }

        [Test]
        public void IsPaused_WhenSettings_ShouldBeTrue()
        {
            _stateManager.TrySetState(GameState.Playing);
            _stateManager.TrySetState(GameState.Settings);
            
            Assert.IsTrue(_stateManager.IsPaused);
        }

        [Test]
        public void IsInMenu_WhenMainMenu_ShouldBeTrue()
        {
            _stateManager.TrySetState(GameState.MainMenu);
            
            Assert.IsTrue(_stateManager.IsInMenu);
        }

        [Test]
        public void IsInMenu_WhenSettings_ShouldBeTrue()
        {
            _stateManager.TrySetState(GameState.Playing);
            _stateManager.TrySetState(GameState.Settings);
            
            Assert.IsTrue(_stateManager.IsInMenu);
        }

        #endregion

        #region Event Tests

        [Test]
        public void OnStateChanged_ShouldFireOnValidTransition()
        {
            GameState? oldState = null;
            GameState? newState = null;
            _stateManager.OnStateChanged += (old, @new) =>
            {
                oldState = old;
                newState = @new;
            };
            
            _stateManager.TrySetState(GameState.Playing);
            
            Assert.AreEqual(GameState.Loading, oldState);
            Assert.AreEqual(GameState.Playing, newState);
        }

        [Test]
        public void OnStateChanged_ShouldNotFireOnInvalidTransition()
        {
            bool eventFired = false;
            _stateManager.OnStateChanged += (old, @new) => eventFired = true;
            
            _stateManager.TrySetState(GameState.Results); // Invalid from Loading
            
            Assert.IsFalse(eventFired);
        }

        [Test]
        public void OnInvalidTransitionAttempted_ShouldFireOnInvalidTransition()
        {
            GameState? attemptedFrom = null;
            GameState? attemptedTo = null;
            _stateManager.OnInvalidTransitionAttempted += (from, to) =>
            {
                attemptedFrom = from;
                attemptedTo = to;
            };
            
            _stateManager.TrySetState(GameState.Results); // Invalid from Loading
            
            Assert.AreEqual(GameState.Loading, attemptedFrom);
            Assert.AreEqual(GameState.Results, attemptedTo);
        }

        [Test]
        public void OnOnlineStatusChanged_ShouldFireOnStatusChange()
        {
            bool? newStatus = null;
            _stateManager.OnOnlineStatusChanged += status => newStatus = status;
            
            _stateManager.SetOnlineStatus(false);
            
            Assert.AreEqual(false, newStatus);
        }

        [Test]
        public void OnOnlineStatusChanged_ShouldNotFireWhenStatusUnchanged()
        {
            bool eventFired = false;
            _stateManager.OnOnlineStatusChanged += status => eventFired = true;
            
            _stateManager.SetOnlineStatus(true); // Already true
            
            Assert.IsFalse(eventFired);
        }

        #endregion

        #region ForceSetState Tests

        [Test]
        public void ForceSetState_ShouldBypassValidation()
        {
            _stateManager.ForceSetState(GameState.Results); // Would be invalid normally
            
            Assert.AreEqual(GameState.Results, _stateManager.CurrentState);
        }

        #endregion

        #region GetValidNextStates Tests

        [Test]
        public void GetValidNextStates_FromLoading_ShouldReturnCorrectStates()
        {
            var validStates = _stateManager.GetValidNextStates();
            
            Assert.IsTrue(validStates.Contains(GameState.MainMenu));
            Assert.IsTrue(validStates.Contains(GameState.Tutorial));
            Assert.IsTrue(validStates.Contains(GameState.Playing));
        }

        [Test]
        public void GetValidNextStates_FromPlaying_ShouldReturnCorrectStates()
        {
            _stateManager.TrySetState(GameState.Playing);
            
            var validStates = _stateManager.GetValidNextStates();
            
            Assert.IsTrue(validStates.Contains(GameState.Paused));
            Assert.IsTrue(validStates.Contains(GameState.Settings));
            Assert.IsTrue(validStates.Contains(GameState.Results));
            Assert.IsTrue(validStates.Contains(GameState.MainMenu));
        }

        #endregion
    }
}
