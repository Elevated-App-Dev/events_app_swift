using System;
using EventPlannerSim.Core;
using EventPlannerSim.Data;
using EventPlannerSim.Systems;

namespace EventPlannerSim.Interfaces
{
    /// <summary>
    /// Interface providing game context to UI layer without circular dependencies.
    /// Implemented by GameManager.
    /// </summary>
    public interface IGameContext
    {
        /// <summary>
        /// Whether the game systems have been initialized.
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// Current player data.
        /// </summary>
        PlayerData CurrentPlayer { get; }

        /// <summary>
        /// Current save data.
        /// </summary>
        SaveData CurrentSaveData { get; }

        /// <summary>
        /// Time system reference.
        /// </summary>
        ITimeSystem TimeSystem { get; }

        /// <summary>
        /// Map system reference.
        /// </summary>
        IMapSystem MapSystem { get; }

        /// <summary>
        /// Event fired when systems are initialized.
        /// </summary>
        event Action OnSystemsInitialized;
    }

    /// <summary>
    /// Simple service locator for accessing game context from UI layer.
    /// </summary>
    public static class ServiceLocator
    {
        private static IGameContext _gameContext;

        /// <summary>
        /// Register the game context (called by GameManager on Awake).
        /// </summary>
        public static void RegisterGameContext(IGameContext context)
        {
            _gameContext = context;
        }

        /// <summary>
        /// Unregister the game context (called by GameManager on Destroy).
        /// </summary>
        public static void UnregisterGameContext()
        {
            _gameContext = null;
        }

        /// <summary>
        /// Get the registered game context.
        /// </summary>
        public static IGameContext GameContext => _gameContext;

        /// <summary>
        /// Check if a game context is registered.
        /// </summary>
        public static bool HasGameContext => _gameContext != null;
    }
}
