using System;
using System.Collections.Generic;

namespace EventPlannerSim.Systems
{
    /// <summary>
    /// Manages Unity Gaming Services integration (Analytics, Remote Config, Cloud Diagnostics).
    /// Requirements: R31.1-R31.8
    /// </summary>
    public interface IUnityServicesManager
    {
        /// <summary>
        /// Initialize Unity Gaming Services with project credentials.
        /// Requirements: R31.4
        /// </summary>
        void Initialize();

        /// <summary>
        /// Track an analytics event.
        /// Requirements: R31.1, R31.5
        /// </summary>
        /// <param name="eventName">The name of the event to track</param>
        /// <param name="parameters">Optional parameters for the event</param>
        void TrackEvent(string eventName, Dictionary<string, object> parameters = null);

        /// <summary>
        /// Get a remote config value.
        /// Requirements: R31.2
        /// </summary>
        /// <typeparam name="T">The type of the config value</typeparam>
        /// <param name="key">The config key</param>
        /// <param name="defaultValue">Default value if key not found</param>
        /// <returns>The config value or default</returns>
        T GetRemoteConfig<T>(string key, T defaultValue);

        /// <summary>
        /// Check if user has consented to analytics.
        /// Requirements: R35.23, R35.24
        /// </summary>
        bool HasAnalyticsConsent { get; }

        /// <summary>
        /// Set user analytics consent status.
        /// Requirements: R35.23, R35.24
        /// </summary>
        /// <param name="consented">Whether the user has consented</param>
        void SetAnalyticsConsent(bool consented);

        /// <summary>
        /// Log a custom exception for crash reporting.
        /// Requirements: R31.3
        /// </summary>
        /// <param name="exception">The exception to log</param>
        void LogException(Exception exception);

        /// <summary>
        /// Check if Unity Gaming Services is initialized.
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// Event fired when initialization completes.
        /// </summary>
        event Action<bool> OnInitialized;

        /// <summary>
        /// Event fired when an analytics event is tracked (for testing/debugging).
        /// </summary>
        event Action<string, Dictionary<string, object>> OnEventTracked;
    }
}
