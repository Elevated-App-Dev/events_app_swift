using System;
using UnityEngine;
using EventPlannerSim.Core;
using EventPlannerSim.Data;
using EventPlannerSim.Systems;

namespace EventPlannerSim.Managers
{
    /// <summary>
    /// Central game coordinator. Singleton MonoBehaviour.
    /// Owns references to all systems and manages game state.
    /// Requirements: R1.1, R2.1-R2.9
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        #region Singleton
        
        private static GameManager _instance;
        
        /// <summary>
        /// Singleton instance of the GameManager.
        /// </summary>
        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.LogWarning("[GameManager] Instance is null. Make sure GameManager exists in the scene.");
                }
                return _instance;
            }
        }
        
        /// <summary>
        /// Check if the GameManager instance exists.
        /// </summary>
        public static bool HasInstance => _instance != null;
        
        #endregion

        #region System References - Core Systems
        
        /// <summary>
        /// Handles persistent game state storage and retrieval.
        /// </summary>
        public ISaveSystem SaveSystem { get; private set; }
        
        /// <summary>
        /// Manages in-game time passage and event scheduling.
        /// </summary>
        public ITimeSystem TimeSystem { get; private set; }
        
        /// <summary>
        /// Manages weather forecasting and outdoor event impacts.
        /// </summary>
        public IWeatherSystem WeatherSystem { get; private set; }
        
        #endregion

        #region System References - Game Systems
        
        /// <summary>
        /// Pure logic calculator for client satisfaction scores.
        /// </summary>
        public ISatisfactionCalculator SatisfactionCalculator { get; private set; }
        
        /// <summary>
        /// Manages player progression through stages and reputation.
        /// </summary>
        public IProgressionSystem ProgressionSystem { get; private set; }
        
        /// <summary>
        /// Handles random events during event execution.
        /// </summary>
        public IConsequenceSystem ConsequenceSystem { get; private set; }
        
        /// <summary>
        /// Manages event lifecycle from inquiry to results.
        /// </summary>
        public IEventPlanningSystem EventPlanningSystem { get; private set; }
        
        /// <summary>
        /// Manages the city map UI, zone navigation, and location discovery.
        /// </summary>
        public IMapSystem MapSystem { get; private set; }
        
        /// <summary>
        /// Manages referral opportunities and excellence streak tracking.
        /// </summary>
        public IReferralSystem ReferralSystem { get; private set; }
        
        /// <summary>
        /// Calculates profit margins and employee commissions.
        /// </summary>
        public IProfitCalculator ProfitCalculator { get; private set; }
        
        /// <summary>
        /// Manages emergency funding options for players.
        /// </summary>
        public IEmergencyFundingSystem EmergencyFundingSystem { get; private set; }
        
        /// <summary>
        /// Manages Stage 3 milestone sequence and career path choice.
        /// </summary>
        public IMilestoneSystem MilestoneSystem { get; private set; }
        
        #endregion

        #region System References - Extension Systems (Stages 4-5 Stubs)
        
        /// <summary>
        /// Manages staff hiring, assignment, and performance (Post-MVP stub).
        /// Requirements: R19
        /// </summary>
        public IStaffSystem StaffSystem { get; private set; }
        
        /// <summary>
        /// Manages marketing investments and inquiry rate modifiers (Post-MVP stub).
        /// Requirements: R22
        /// </summary>
        public IMarketingSystem MarketingSystem { get; private set; }
        
        /// <summary>
        /// Manages office progression and workspace bonuses (Post-MVP stub).
        /// Requirements: R21
        /// </summary>
        public IOfficeSystem OfficeSystem { get; private set; }
        
        #endregion

        #region System References - UI Systems
        
        /// <summary>
        /// Manages the smartphone UI overlay and app navigation.
        /// </summary>
        public IPhoneSystem PhoneSystem { get; private set; }
        
        /// <summary>
        /// Manages guided instruction for new players.
        /// </summary>
        public ITutorialSystem TutorialSystem { get; private set; }
        
        /// <summary>
        /// Manages background music and sound effects.
        /// </summary>
        public IAudioManager AudioManager { get; private set; }
        
        #endregion

        #region System References - Platform Systems
        
        /// <summary>
        /// Handles in-app purchases and advertisements.
        /// </summary>
        public IMonetizationSystem MonetizationSystem { get; private set; }
        
        /// <summary>
        /// Manages Unity Gaming Services integration.
        /// </summary>
        public IUnityServicesManager UnityServices { get; private set; }
        
        /// <summary>
        /// Manages push notifications and local reminders.
        /// </summary>
        public INotificationSystem NotificationSystem { get; private set; }
        
        /// <summary>
        /// Manages player achievements and platform integration.
        /// </summary>
        public IAchievementSystem AchievementSystem { get; private set; }
        
        #endregion

        #region Game State
        
        /// <summary>
        /// Current player data and progression.
        /// </summary>
        public PlayerData CurrentPlayer { get; private set; }
        
        /// <summary>
        /// Current save data containing all game state.
        /// </summary>
        public SaveData CurrentSaveData { get; private set; }
        
        /// <summary>
        /// Game state manager for robust state transitions.
        /// </summary>
        public GameStateManager StateManager { get; private set; }
        
        /// <summary>
        /// Current game state (convenience property).
        /// </summary>
        public GameState State => StateManager?.CurrentState ?? GameState.Loading;
        
        /// <summary>
        /// Whether the device is currently online.
        /// </summary>
        public bool IsOnline => StateManager?.IsOnline ?? true;
        
        /// <summary>
        /// Whether all systems have been initialized.
        /// </summary>
        public bool IsInitialized { get; private set; }
        
        #endregion

        #region Events
        
        /// <summary>
        /// Event fired when game state changes.
        /// </summary>
        public event Action<GameState, GameState> OnGameStateChanged;
        
        /// <summary>
        /// Event fired when online status changes.
        /// </summary>
        public event Action<bool> OnOnlineStatusChanged;
        
        /// <summary>
        /// Event fired when player data changes.
        /// </summary>
        public event Action<PlayerData> OnPlayerDataChanged;
        
        /// <summary>
        /// Event fired when systems are initialized.
        /// </summary>
        public event Action OnSystemsInitialized;
        
        #endregion

        #region Unity Lifecycle
        
        private void Awake()
        {
            // Singleton pattern
            if (_instance != null && _instance != this)
            {
                Debug.LogWarning("[GameManager] Duplicate instance detected. Destroying this one.");
                Destroy(gameObject);
                return;
            }
            
            _instance = this;
            DontDestroyOnLoad(gameObject);
            
            InitializeSystems();
        }
        
        private void OnApplicationFocus(bool hasFocus)
        {
            if (!IsInitialized) return;
            
            if (hasFocus)
            {
                // App regained focus - resume systems
                OnApplicationResumed();
            }
            else
            {
                // App lost focus - pause and save
                OnApplicationPaused();
            }
        }
        
        private void OnApplicationPause(bool pauseStatus)
        {
            if (!IsInitialized) return;
            
            if (pauseStatus)
            {
                OnApplicationPaused();
            }
            else
            {
                OnApplicationResumed();
            }
        }
        
        private void OnApplicationQuit()
        {
            if (!IsInitialized) return;
            
            // Auto-save on quit (R2.6)
            SaveGame();
        }
        
        private void Update()
        {
            if (!IsInitialized || State != GameState.Playing) return;
            
            // Advance time based on real-time delta
            if (CurrentPlayer != null && TimeSystem != null)
            {
                TimeSystem.AdvanceTime(Time.deltaTime, (int)CurrentPlayer.stage);
            }
            
            // Check online status periodically
            CheckOnlineStatus();
        }
        
        #endregion

        #region System Initialization
        
        /// <summary>
        /// Initialize all systems in the correct dependency order.
        /// Requirements: R2.1
        /// </summary>
        public void InitializeSystems()
        {
            Debug.Log("[GameManager] Initializing systems...");
            
            try
            {
                // Initialize state manager first
                StateManager = new GameStateManager();
                StateManager.OnStateChanged += HandleStateChanged;
                StateManager.OnOnlineStatusChanged += HandleOnlineStatusChanged;
                
                // 1. Core systems (no dependencies)
                SaveSystem = new SaveSystemImpl();
                TimeSystem = new TimeSystemImpl();
                WeatherSystem = new WeatherSystemImpl();
                
                Debug.Log("[GameManager] Core systems initialized.");
                
                // 2. Game systems (depend on core)
                SatisfactionCalculator = new SatisfactionCalculatorImpl();
                ProgressionSystem = new ProgressionSystemImpl();
                ConsequenceSystem = new ConsequenceSystemImpl();
                EventPlanningSystem = new EventPlanningSystemImpl();
                MapSystem = new MapSystemImpl();
                ReferralSystem = new ReferralSystemImpl();
                ProfitCalculator = new ProfitCalculatorImpl();
                EmergencyFundingSystem = new EmergencyFundingSystemImpl();
                MilestoneSystem = new MilestoneSystemImpl();
                
                Debug.Log("[GameManager] Game systems initialized.");
                
                // 3. Extension systems - Stages 4-5 stubs (Post-MVP)
                StaffSystem = new StaffSystemImpl();
                MarketingSystem = new MarketingSystemImpl();
                OfficeSystem = new OfficeSystemImpl();
                
                Debug.Log("[GameManager] Extension systems (stubs) initialized.");
                
                // 3. UI systems (depend on game systems)
                PhoneSystem = new PhoneSystemImpl();
                TutorialSystem = new TutorialSystemImpl();
                AudioManager = new AudioManagerImpl();
                
                Debug.Log("[GameManager] UI systems initialized.");
                
                // 4. Platform systems (can initialize async)
                MonetizationSystem = new MonetizationSystemImpl();
                UnityServices = new UnityServicesManagerImpl();
                NotificationSystem = new NotificationSystemImpl();
                AchievementSystem = new AchievementSystemImpl();
                
                // Initialize platform systems
                MonetizationSystem.Initialize();
                UnityServices.Initialize();
                
                Debug.Log("[GameManager] Platform systems initialized.");
                
                IsInitialized = true;
                OnSystemsInitialized?.Invoke();
                
                Debug.Log("[GameManager] All systems initialized successfully.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GameManager] Failed to initialize systems: {ex.Message}");
                throw;
            }
        }
        
        /// <summary>
        /// Handle state changes from the state manager.
        /// </summary>
        private void HandleStateChanged(GameState oldState, GameState newState)
        {
            // Handle state-specific logic
            switch (newState)
            {
                case GameState.Playing:
                    TimeSystem?.Resume();
                    AudioManager?.ResumeAudio();
                    break;
                    
                case GameState.Paused:
                case GameState.Settings:
                    TimeSystem?.Pause();
                    break;
                    
                case GameState.Tutorial:
                    TimeSystem?.Pause();
                    break;
                    
                case GameState.Results:
                    TimeSystem?.Pause();
                    AudioManager?.PlayMusic(MusicTrack.Results);
                    break;
            }
            
            OnGameStateChanged?.Invoke(oldState, newState);
        }
        
        /// <summary>
        /// Handle online status changes from the state manager.
        /// </summary>
        private void HandleOnlineStatusChanged(bool isOnline)
        {
            if (isOnline)
            {
                OnDeviceCameOnline();
            }
            else
            {
                OnDeviceWentOffline();
            }
            
            OnOnlineStatusChanged?.Invoke(isOnline);
        }
        
        #endregion

        #region Game State Management
        
        /// <summary>
        /// Load existing save or start new game.
        /// Requirements: R2.2, R2.3
        /// </summary>
        public void StartGame()
        {
            Debug.Log("[GameManager] Starting game...");
            
            if (SaveSystem.HasSaveFile && SaveSystem.ValidateSaveFile())
            {
                LoadGame();
            }
            else
            {
                StartNewGame();
            }
        }
        
        /// <summary>
        /// Initialize a new game with Stage 1 defaults.
        /// Requirements: R2.3
        /// </summary>
        public void StartNewGame()
        {
            Debug.Log("[GameManager] Starting new game...");
            
            // Create new player data with Stage 1 defaults
            CurrentPlayer = new PlayerData
            {
                playerName = "Alex",
                stage = BusinessStage.Solo,
                money = 500f,
                reputation = 0,
                unlockedZones = new System.Collections.Generic.List<MapZone> { MapZone.Neighborhood }
            };
            
            // Create new save data
            CurrentSaveData = new SaveData
            {
                playerData = CurrentPlayer,
                currentDate = new GameDate(1, 1, 1)
            };
            
            // Initialize time system with starting date
            TimeSystem.SetCurrentDate(CurrentSaveData.currentDate);
            
            // Initialize weather system
            WeatherSystem.SetCurrentDate(CurrentSaveData.currentDate);
            WeatherSystem.RegenerateForecasts();
            
            // Initialize notification system with default settings
            NotificationSystem.Initialize(CurrentSaveData.settings.notifications);
            
            // Set game state to tutorial for new players
            SetGameState(GameState.Tutorial);
            
            // Start tutorial
            TutorialSystem.StartTutorial();
            
            // Play main menu music
            AudioManager.PlayMusic(MusicTrack.MainMenu);
            
            // Save initial state
            SaveGame();
            
            OnPlayerDataChanged?.Invoke(CurrentPlayer);
            
            Debug.Log("[GameManager] New game started successfully.");
        }
        
        /// <summary>
        /// Load game from save file.
        /// Requirements: R2.2
        /// </summary>
        public void LoadGame()
        {
            Debug.Log("[GameManager] Loading game...");
            
            try
            {
                CurrentSaveData = SaveSystem.Load();
                
                if (CurrentSaveData == null)
                {
                    Debug.LogWarning("[GameManager] Failed to load save data. Starting new game.");
                    StartNewGame();
                    return;
                }
                
                CurrentPlayer = CurrentSaveData.playerData;
                
                // Restore system states from save data
                TimeSystem.SetCurrentDate(CurrentSaveData.currentDate);
                WeatherSystem.SetCurrentDate(CurrentSaveData.currentDate);
                WeatherSystem.RegenerateForecasts();
                
                // Initialize notification system with saved settings
                NotificationSystem.Initialize(CurrentSaveData.settings.notifications);
                
                // Set phone system stage for Stage 2 features
                PhoneSystem.CurrentStage = (int)CurrentPlayer.stage;
                
                // Set tutorial system stage
                TutorialSystem.CurrentStage = (int)CurrentPlayer.stage;
                
                // Set game state based on tutorial completion
                if (TutorialSystem.IsTutorialComplete)
                {
                    SetGameState(GameState.Playing);
                }
                else
                {
                    SetGameState(GameState.Tutorial);
                }
                
                // Play appropriate music
                AudioManager.PlayMusic(MusicTrack.MainMenu);
                
                OnPlayerDataChanged?.Invoke(CurrentPlayer);
                
                Debug.Log($"[GameManager] Game loaded successfully. Stage: {CurrentPlayer.stage}, Reputation: {CurrentPlayer.reputation}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GameManager] Failed to load game: {ex.Message}");
                
                // Attempt to restore from backup (R2.5)
                if (SaveSystem.HasBackupFile)
                {
                    Debug.Log("[GameManager] Attempting to restore from backup...");
                    CurrentSaveData = SaveSystem.RestoreFromBackup();
                    
                    if (CurrentSaveData != null)
                    {
                        CurrentPlayer = CurrentSaveData.playerData;
                        SetGameState(GameState.Playing);
                        return;
                    }
                }
                
                // If all else fails, offer to start new game
                Debug.LogWarning("[GameManager] Could not recover save. Starting new game.");
                StartNewGame();
            }
        }
        
        /// <summary>
        /// Save current game state.
        /// Requirements: R2.4, R2.6
        /// </summary>
        public void SaveGame()
        {
            if (CurrentSaveData == null || CurrentPlayer == null)
            {
                Debug.LogWarning("[GameManager] Cannot save - no active game data.");
                return;
            }
            
            try
            {
                // Update save data with current state
                CurrentSaveData.playerData = CurrentPlayer;
                CurrentSaveData.currentDate = TimeSystem.CurrentDate;
                
                // Save to persistent storage
                SaveSystem.Save(CurrentSaveData);
                
                Debug.Log("[GameManager] Game saved successfully.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GameManager] Failed to save game: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Set the current game state.
        /// </summary>
        public void SetGameState(GameState newState)
        {
            if (StateManager == null)
            {
                Debug.LogWarning("[GameManager] StateManager not initialized.");
                return;
            }
            
            StateManager.TrySetState(newState);
        }
        
        #endregion

        #region Online/Offline Handling
        
        private float _lastOnlineCheck;
        private const float OnlineCheckInterval = 5f;
        
        /// <summary>
        /// Check and update online status.
        /// Requirements: R2.7-R2.9
        /// </summary>
        private void CheckOnlineStatus()
        {
            if (Time.time - _lastOnlineCheck < OnlineCheckInterval) return;
            _lastOnlineCheck = Time.time;
            
            StateManager?.UpdateOnlineStatus(Application.internetReachability);
        }
        
        /// <summary>
        /// Handle device going offline.
        /// Requirements: R2.8
        /// </summary>
        private void OnDeviceWentOffline()
        {
            Debug.Log("[GameManager] Device went offline. Disabling network features.");
            
            // Network-dependent features are disabled gracefully
            // Core gameplay continues unaffected
        }
        
        /// <summary>
        /// Handle device coming online.
        /// Requirements: R2.9
        /// </summary>
        private void OnDeviceCameOnline()
        {
            Debug.Log("[GameManager] Device came online. Re-enabling network features.");
            
            // Re-initialize platform systems without restart
            if (MonetizationSystem != null && !MonetizationSystem.IsInitialized)
            {
                MonetizationSystem.Initialize();
            }
            
            if (UnityServices != null && !UnityServices.IsInitialized)
            {
                UnityServices.Initialize();
            }
            
            // Sync achievements with platform
            AchievementSystem?.SyncWithPlatform();
        }
        
        #endregion

        #region Application Lifecycle
        
        /// <summary>
        /// Handle application being paused/backgrounded.
        /// Requirements: R2.6
        /// </summary>
        private void OnApplicationPaused()
        {
            Debug.Log("[GameManager] Application paused.");
            
            // Auto-save
            SaveGame();
            
            // Pause audio
            AudioManager?.PauseAudio();
            
            // Pause time
            TimeSystem?.Pause();
        }
        
        /// <summary>
        /// Handle application being resumed/foregrounded.
        /// </summary>
        private void OnApplicationResumed()
        {
            Debug.Log("[GameManager] Application resumed.");
            
            // Resume audio
            AudioManager?.ResumeAudio();
            
            // Resume time if playing
            if (State == GameState.Playing)
            {
                TimeSystem?.Resume();
            }
            
            // Check online status
            CheckOnlineStatus();
        }
        
        #endregion

        #region Player Data Updates
        
        /// <summary>
        /// Update player money and trigger save.
        /// </summary>
        public void UpdatePlayerMoney(float amount)
        {
            if (CurrentPlayer == null) return;
            
            CurrentPlayer.money += amount;
            OnPlayerDataChanged?.Invoke(CurrentPlayer);
            SaveGame();
        }
        
        /// <summary>
        /// Update player reputation and trigger save.
        /// </summary>
        public void UpdatePlayerReputation(int amount)
        {
            if (CurrentPlayer == null) return;
            
            CurrentPlayer.reputation += amount;
            OnPlayerDataChanged?.Invoke(CurrentPlayer);
            SaveGame();
        }
        
        /// <summary>
        /// Advance player to next stage if requirements are met.
        /// </summary>
        public bool TryAdvanceStage()
        {
            if (CurrentPlayer == null || ProgressionSystem == null) return false;
            
            if (ProgressionSystem.CanAdvanceStage(CurrentPlayer))
            {
                int nextStage = (int)CurrentPlayer.stage + 1;
                if (nextStage <= 5)
                {
                    CurrentPlayer.stage = (BusinessStage)nextStage;
                    
                    // Update systems with new stage
                    PhoneSystem.CurrentStage = nextStage;
                    TutorialSystem.CurrentStage = nextStage;
                    
                    // Update office for new stage (Post-MVP extension)
                    OfficeSystem?.UpdateOfficeForStage(CurrentPlayer.stage, CurrentPlayer.careerPath);
                    
                    OnPlayerDataChanged?.Invoke(CurrentPlayer);
                    SaveGame();
                    
                    Debug.Log($"[GameManager] Player advanced to Stage {nextStage}!");
                    return true;
                }
            }
            
            return false;
        }
        
        #endregion

        #region Utility Methods
        
        /// <summary>
        /// Delete save and reset to new game.
        /// </summary>
        public void ResetGame()
        {
            Debug.Log("[GameManager] Resetting game...");
            
            SaveSystem.DeleteSave();
            StartNewGame();
        }
        
        /// <summary>
        /// Create a backup of the current save.
        /// </summary>
        public void CreateBackup()
        {
            SaveSystem.CreateBackup();
        }
        
        #endregion
    }
}
