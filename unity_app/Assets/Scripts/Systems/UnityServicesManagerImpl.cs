using System;
using System.Collections.Generic;
using EventPlannerSim.Data;

namespace EventPlannerSim.Systems
{
    /// <summary>
    /// Implementation of Unity Gaming Services integration.
    /// Manages Analytics, Remote Config, and Cloud Diagnostics.
    /// Requirements: R31.1-R31.8
    /// 
    /// Note: This is a pure C# implementation that can be tested without Unity.
    /// In production, a MonoBehaviour wrapper would call the actual Unity Gaming Services APIs.
    /// </summary>
    public class UnityServicesManagerImpl : IUnityServicesManager
    {
        // State
        private bool _isInitialized;
        private bool _hasAnalyticsConsent;
        
        // Remote config cache (simulated - in production would come from Unity Remote Config)
        private readonly Dictionary<string, object> _remoteConfigCache;
        
        // Analytics event log (for testing and debugging)
        private readonly List<AnalyticsEventRecord> _eventLog;
        
        // Events
        public event Action<bool> OnInitialized;
        public event Action<string, Dictionary<string, object>> OnEventTracked;

        /// <summary>
        /// Check if Unity Gaming Services is initialized.
        /// </summary>
        public bool IsInitialized => _isInitialized;

        /// <summary>
        /// Check if user has consented to analytics.
        /// Requirements: R35.23, R35.24
        /// </summary>
        public bool HasAnalyticsConsent => _hasAnalyticsConsent;

        public UnityServicesManagerImpl()
        {
            _isInitialized = false;
            _hasAnalyticsConsent = false;
            _remoteConfigCache = new Dictionary<string, object>();
            _eventLog = new List<AnalyticsEventRecord>();
            
            // Initialize default remote config values
            InitializeDefaultRemoteConfig();
        }

        /// <summary>
        /// Initialize default remote config values.
        /// These would be overridden by actual Unity Remote Config in production.
        /// </summary>
        private void InitializeDefaultRemoteConfig()
        {
            // Game balance defaults (R31.2, R31.6)
            _remoteConfigCache["inquiry_interval_stage1_min"] = 8f;
            _remoteConfigCache["inquiry_interval_stage1_max"] = 12f;
            _remoteConfigCache["inquiry_interval_stage2_min"] = 6f;
            _remoteConfigCache["inquiry_interval_stage2_max"] = 10f;
            _remoteConfigCache["inquiry_interval_stage3_min"] = 5f;
            _remoteConfigCache["inquiry_interval_stage3_max"] = 8f;
            
            // Monetization A/B testing defaults
            _remoteConfigCache["ad_cooldown_emergency_funding"] = 300f; // 5 minutes
            _remoteConfigCache["ad_cooldown_overtime_hours"] = 600f; // 10 minutes
            _remoteConfigCache["interstitial_frequency"] = 3; // Every 3 events
            
            // Feature flags
            _remoteConfigCache["enable_weather_system"] = true;
            _remoteConfigCache["enable_referral_system"] = true;
            _remoteConfigCache["enable_celebrity_events"] = false; // Post-MVP
        }

        /// <summary>
        /// Initialize Unity Gaming Services with project credentials.
        /// Requirements: R31.4
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized)
                return;

            try
            {
                // In production, this would:
                // 1. Initialize Unity Services with UnityServices.InitializeAsync()
                // 2. Initialize Analytics with AnalyticsService.Instance
                // 3. Initialize Remote Config with RemoteConfigService.Instance
                // 4. Initialize Cloud Diagnostics (automatic with Unity)
                
                // For now, we simulate successful initialization
                _isInitialized = true;
                
                // Fire initialization event
                OnInitialized?.Invoke(true);
            }
            catch (Exception ex)
            {
                _isInitialized = false;
                LogException(ex);
                OnInitialized?.Invoke(false);
            }
        }

        /// <summary>
        /// Track an analytics event.
        /// Requirements: R31.1, R31.5
        /// </summary>
        public void TrackEvent(string eventName, Dictionary<string, object> parameters = null)
        {
            // Don't track if not initialized or no consent
            if (!_isInitialized)
                return;

            if (!_hasAnalyticsConsent)
                return;

            if (string.IsNullOrEmpty(eventName))
                return;

            // Validate event name (must be from Analytics Events List)
            if (!IsValidEventName(eventName))
            {
                // Log warning but don't throw - graceful degradation
                return;
            }

            // Create event record
            var record = new AnalyticsEventRecord
            {
                EventName = eventName,
                Parameters = parameters != null ? new Dictionary<string, object>(parameters) : new Dictionary<string, object>(),
                Timestamp = DateTime.UtcNow
            };

            // Add to event log
            _eventLog.Add(record);

            // In production, this would call:
            // AnalyticsService.Instance.CustomData(eventName, parameters);
            
            // Fire event for testing/debugging
            OnEventTracked?.Invoke(eventName, parameters);
        }

        /// <summary>
        /// Validate that the event name is from the approved Analytics Events List.
        /// Requirements: R31.5
        /// </summary>
        private bool IsValidEventName(string eventName)
        {
            return AnalyticsEvents.IsValidEvent(eventName);
        }

        /// <summary>
        /// Get a remote config value.
        /// Requirements: R31.2
        /// </summary>
        public T GetRemoteConfig<T>(string key, T defaultValue)
        {
            if (!_isInitialized)
                return defaultValue;

            if (string.IsNullOrEmpty(key))
                return defaultValue;

            if (_remoteConfigCache.TryGetValue(key, out var value))
            {
                try
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch
                {
                    return defaultValue;
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// Set a remote config value (for testing purposes).
        /// In production, values come from Unity Remote Config service.
        /// </summary>
        public void SetRemoteConfig(string key, object value)
        {
            if (string.IsNullOrEmpty(key))
                return;

            _remoteConfigCache[key] = value;
        }

        /// <summary>
        /// Set user analytics consent status.
        /// Requirements: R35.23, R35.24
        /// </summary>
        public void SetAnalyticsConsent(bool consented)
        {
            bool wasConsented = _hasAnalyticsConsent;
            _hasAnalyticsConsent = consented;

            // In production, this would also:
            // - Update Unity Analytics consent status
            // - Potentially flush or clear pending events if consent revoked
            
            // Note: We don't track consent events in the public event log
            // to avoid polluting analytics counts
        }

        /// <summary>
        /// Log a custom exception for crash reporting.
        /// Requirements: R31.3
        /// </summary>
        public void LogException(Exception exception)
        {
            if (exception == null)
                return;

            // In production, this would call:
            // CrashlyticsHandler.LogException(exception);
            // or Unity's built-in crash reporting
            
            // For now, we just log to the event log for testing
            var record = new AnalyticsEventRecord
            {
                EventName = "_exception",
                Parameters = new Dictionary<string, object>
                {
                    { "type", exception.GetType().Name },
                    { "message", exception.Message },
                    { "stack_trace", exception.StackTrace ?? "" }
                },
                Timestamp = DateTime.UtcNow
            };
            _eventLog.Add(record);
        }

        /// <summary>
        /// Get the event log (for testing purposes).
        /// </summary>
        public List<AnalyticsEventRecord> GetEventLog()
        {
            return new List<AnalyticsEventRecord>(_eventLog);
        }

        /// <summary>
        /// Clear the event log (for testing purposes).
        /// </summary>
        public void ClearEventLog()
        {
            _eventLog.Clear();
        }

        /// <summary>
        /// Get count of tracked events (for testing purposes).
        /// </summary>
        public int GetEventCount()
        {
            return _eventLog.Count;
        }

        /// <summary>
        /// Get events by name (for testing purposes).
        /// </summary>
        public List<AnalyticsEventRecord> GetEventsByName(string eventName)
        {
            return _eventLog.FindAll(e => e.EventName == eventName);
        }
    }

    /// <summary>
    /// Record of an analytics event for logging and testing.
    /// </summary>
    public class AnalyticsEventRecord
    {
        public string EventName { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
