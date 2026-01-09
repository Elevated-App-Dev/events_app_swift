using System;
using System.Collections.Generic;
using UnityEngine;
using EventPlannerSim.Core;

namespace EventPlannerSim.Managers
{
    /// <summary>
    /// Manages game state transitions with validation and history tracking.
    /// Requirements: R2.7-R2.9
    /// </summary>
    public class GameStateManager
    {
        #region State Transition Rules
        
        /// <summary>
        /// Valid state transitions map.
        /// Key = current state, Value = list of valid next states.
        /// </summary>
        private static readonly Dictionary<GameState, HashSet<GameState>> ValidTransitions = new()
        {
            { GameState.Loading, new HashSet<GameState> { GameState.MainMenu, GameState.Tutorial, GameState.Playing } },
            { GameState.MainMenu, new HashSet<GameState> { GameState.Playing, GameState.Tutorial, GameState.Settings, GameState.Loading } },
            { GameState.Playing, new HashSet<GameState> { GameState.Paused, GameState.Settings, GameState.Results, GameState.MainMenu } },
            { GameState.Paused, new HashSet<GameState> { GameState.Playing, GameState.Settings, GameState.MainMenu } },
            { GameState.Tutorial, new HashSet<GameState> { GameState.Playing, GameState.MainMenu, GameState.Paused } },
            { GameState.Results, new HashSet<GameState> { GameState.Playing, GameState.MainMenu } },
            { GameState.Settings, new HashSet<GameState> { GameState.Playing, GameState.Paused, GameState.MainMenu } }
        };
        
        #endregion

        #region Properties
        
        /// <summary>
        /// Current game state.
        /// </summary>
        public GameState CurrentState { get; private set; } = GameState.Loading;
        
        /// <summary>
        /// Previous game state (for returning from menus/settings).
        /// </summary>
        public GameState PreviousState { get; private set; } = GameState.Loading;
        
        /// <summary>
        /// Whether the game is currently in a playable state.
        /// </summary>
        public bool IsPlayable => CurrentState == GameState.Playing || CurrentState == GameState.Tutorial;
        
        /// <summary>
        /// Whether the game is currently paused.
        /// </summary>
        public bool IsPaused => CurrentState == GameState.Paused || CurrentState == GameState.Settings;
        
        /// <summary>
        /// Whether the game is in a menu state.
        /// </summary>
        public bool IsInMenu => CurrentState == GameState.MainMenu || CurrentState == GameState.Settings;
        
        /// <summary>
        /// Whether the device is currently online.
        /// </summary>
        public bool IsOnline { get; private set; } = true;
        
        /// <summary>
        /// Whether network features are available.
        /// </summary>
        public bool AreNetworkFeaturesAvailable => IsOnline;
        
        #endregion

        #region Events
        
        /// <summary>
        /// Event fired before state changes (can be used for validation).
        /// </summary>
        public event Func<GameState, GameState, bool> OnStateChangeRequested;
        
        /// <summary>
        /// Event fired when state changes.
        /// </summary>
        public event Action<GameState, GameState> OnStateChanged;
        
        /// <summary>
        /// Event fired when online status changes.
        /// </summary>
        public event Action<bool> OnOnlineStatusChanged;
        
        /// <summary>
        /// Event fired when an invalid state transition is attempted.
        /// </summary>
        public event Action<GameState, GameState> OnInvalidTransitionAttempted;
        
        #endregion

        #region State Transitions
        
        /// <summary>
        /// Attempt to transition to a new state.
        /// Returns true if transition was successful.
        /// </summary>
        public bool TrySetState(GameState newState)
        {
            if (CurrentState == newState)
            {
                Debug.Log($"[GameStateManager] Already in state {newState}");
                return true;
            }
            
            // Validate transition
            if (!IsValidTransition(CurrentState, newState))
            {
                Debug.LogWarning($"[GameStateManager] Invalid transition: {CurrentState} -> {newState}");
                OnInvalidTransitionAttempted?.Invoke(CurrentState, newState);
                return false;
            }
            
            // Allow external validation
            if (OnStateChangeRequested != null)
            {
                bool allowed = OnStateChangeRequested.Invoke(CurrentState, newState);
                if (!allowed)
                {
                    Debug.Log($"[GameStateManager] State change blocked by external handler: {CurrentState} -> {newState}");
                    return false;
                }
            }
            
            // Perform transition
            PreviousState = CurrentState;
            CurrentState = newState;
            
            Debug.Log($"[GameStateManager] State changed: {PreviousState} -> {CurrentState}");
            OnStateChanged?.Invoke(PreviousState, CurrentState);
            
            return true;
        }
        
        /// <summary>
        /// Force set state without validation (use sparingly).
        /// </summary>
        public void ForceSetState(GameState newState)
        {
            PreviousState = CurrentState;
            CurrentState = newState;
            
            Debug.Log($"[GameStateManager] State forced: {PreviousState} -> {CurrentState}");
            OnStateChanged?.Invoke(PreviousState, CurrentState);
        }
        
        /// <summary>
        /// Return to the previous state.
        /// </summary>
        public bool ReturnToPreviousState()
        {
            return TrySetState(PreviousState);
        }
        
        /// <summary>
        /// Check if a transition from one state to another is valid.
        /// </summary>
        public bool IsValidTransition(GameState from, GameState to)
        {
            if (ValidTransitions.TryGetValue(from, out var validNextStates))
            {
                return validNextStates.Contains(to);
            }
            return false;
        }
        
        /// <summary>
        /// Get all valid next states from the current state.
        /// </summary>
        public IReadOnlyCollection<GameState> GetValidNextStates()
        {
            if (ValidTransitions.TryGetValue(CurrentState, out var validNextStates))
            {
                return validNextStates;
            }
            return Array.Empty<GameState>();
        }
        
        #endregion

        #region Online/Offline Management
        
        /// <summary>
        /// Update online status based on network reachability.
        /// Requirements: R2.7-R2.9
        /// </summary>
        public void UpdateOnlineStatus(NetworkReachability reachability)
        {
            bool wasOnline = IsOnline;
            IsOnline = reachability != NetworkReachability.NotReachable;
            
            if (wasOnline != IsOnline)
            {
                Debug.Log($"[GameStateManager] Online status changed: {wasOnline} -> {IsOnline}");
                OnOnlineStatusChanged?.Invoke(IsOnline);
            }
        }
        
        /// <summary>
        /// Manually set online status (for testing).
        /// </summary>
        public void SetOnlineStatus(bool online)
        {
            if (IsOnline != online)
            {
                IsOnline = online;
                Debug.Log($"[GameStateManager] Online status set to: {IsOnline}");
                OnOnlineStatusChanged?.Invoke(IsOnline);
            }
        }
        
        #endregion

        #region Convenience Methods
        
        /// <summary>
        /// Pause the game.
        /// </summary>
        public bool Pause()
        {
            if (CurrentState == GameState.Playing)
            {
                return TrySetState(GameState.Paused);
            }
            return false;
        }
        
        /// <summary>
        /// Resume the game from pause.
        /// </summary>
        public bool Resume()
        {
            if (CurrentState == GameState.Paused)
            {
                return TrySetState(GameState.Playing);
            }
            return false;
        }
        
        /// <summary>
        /// Open settings menu.
        /// </summary>
        public bool OpenSettings()
        {
            return TrySetState(GameState.Settings);
        }
        
        /// <summary>
        /// Close settings and return to previous state.
        /// </summary>
        public bool CloseSettings()
        {
            if (CurrentState == GameState.Settings)
            {
                return ReturnToPreviousState();
            }
            return false;
        }
        
        /// <summary>
        /// Return to main menu.
        /// </summary>
        public bool ReturnToMainMenu()
        {
            return TrySetState(GameState.MainMenu);
        }
        
        /// <summary>
        /// Start playing (from menu or tutorial).
        /// </summary>
        public bool StartPlaying()
        {
            return TrySetState(GameState.Playing);
        }
        
        /// <summary>
        /// Show results screen.
        /// </summary>
        public bool ShowResults()
        {
            return TrySetState(GameState.Results);
        }
        
        /// <summary>
        /// Complete results and return to playing.
        /// </summary>
        public bool CompleteResults()
        {
            if (CurrentState == GameState.Results)
            {
                return TrySetState(GameState.Playing);
            }
            return false;
        }
        
        #endregion
    }
}
