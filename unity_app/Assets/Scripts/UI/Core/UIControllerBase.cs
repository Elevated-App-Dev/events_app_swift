using UnityEngine;
using EventPlannerSim.Interfaces;

namespace EventPlannerSim.UI.Core
{
    /// <summary>
    /// Base class for all UI controllers.
    /// Provides common functionality for event subscription and game context access.
    /// </summary>
    public abstract class UIControllerBase : MonoBehaviour
    {
        /// <summary>
        /// Reference to the game context via ServiceLocator.
        /// </summary>
        protected IGameContext GameManager => ServiceLocator.GameContext;

        /// <summary>
        /// Whether the controller has been initialized.
        /// </summary>
        protected bool IsInitialized { get; private set; }

        #region Unity Lifecycle

        protected virtual void Awake()
        {
            // Override in derived classes if needed
        }

        protected virtual void Start()
        {
            // Try to initialize on Start
            TryInitialize();
        }

        protected virtual void OnEnable()
        {
            if (GameManager != null && GameManager.IsInitialized)
            {
                Initialize();
            }
            else if (GameManager != null)
            {
                // Wait for GameManager to initialize
                GameManager.OnSystemsInitialized += OnGameManagerInitialized;
            }
        }

        protected virtual void OnDisable()
        {
            if (GameManager != null)
            {
                GameManager.OnSystemsInitialized -= OnGameManagerInitialized;
            }

            UnsubscribeFromEvents();
            IsInitialized = false;
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Try to initialize if game context is available.
        /// </summary>
        private void TryInitialize()
        {
            if (GameManager != null && GameManager.IsInitialized && !IsInitialized)
            {
                Initialize();
            }
        }

        /// <summary>
        /// Called when GameManager is ready.
        /// </summary>
        private void OnGameManagerInitialized()
        {
            Initialize();
        }

        /// <summary>
        /// Initialize the controller and subscribe to events.
        /// </summary>
        private void Initialize()
        {
            if (IsInitialized) return;

            SubscribeToEvents();
            RefreshDisplay();
            IsInitialized = true;
        }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Subscribe to system events. Override in derived classes.
        /// </summary>
        protected abstract void SubscribeToEvents();

        /// <summary>
        /// Unsubscribe from system events. Override in derived classes.
        /// </summary>
        protected abstract void UnsubscribeFromEvents();

        /// <summary>
        /// Refresh the UI display. Override in derived classes.
        /// </summary>
        protected abstract void RefreshDisplay();

        #endregion

        #region Utility Methods

        /// <summary>
        /// Safe check for null GameManager and player data.
        /// </summary>
        protected bool HasValidGameState()
        {
            return GameManager != null &&
                   GameManager.IsInitialized &&
                   GameManager.CurrentPlayer != null;
        }

        /// <summary>
        /// Log a warning with controller context.
        /// </summary>
        protected void LogWarning(string message)
        {
            Debug.LogWarning($"[{GetType().Name}] {message}");
        }

        /// <summary>
        /// Log info with controller context.
        /// </summary>
        protected void Log(string message)
        {
            Debug.Log($"[{GetType().Name}] {message}");
        }

        #endregion
    }
}
