using System;
using System.Collections.Generic;
using EventPlannerSim.Core;

namespace EventPlannerSim.Systems
{
    /// <summary>
    /// Implementation of the Phone System for smartphone UI overlay and app navigation.
    /// Requirements: R4.1-R4.14
    /// </summary>
    public class PhoneSystemImpl : IPhoneSystem
    {
        private readonly Dictionary<PhoneApp, int> _badgeCounts;
        private bool _isPhoneOpen;
        private PhoneApp? _currentApp;
        private int _currentStage;

        // Events for UI integration
        public event Action OnPhoneOpened;
        public event Action OnPhoneClosed;
        public event Action<PhoneApp> OnAppOpened;
        public event Action<PhoneApp> OnAppClosed;
        public event Action<PhoneApp, int> OnBadgeCountChanged;

        public bool IsPhoneOpen => _isPhoneOpen;
        public PhoneApp? CurrentApp => _currentApp;

        public int CurrentStage
        {
            get => _currentStage;
            set => _currentStage = Math.Clamp(value, 1, 5);
        }

        /// <summary>
        /// Check if Stage 2 company/personal separation is active.
        /// Requirements: R4.12-R4.14
        /// </summary>
        public bool IsStage2SeparationActive => _currentStage == 2;

        public PhoneSystemImpl()
        {
            _badgeCounts = new Dictionary<PhoneApp, int>();
            _isPhoneOpen = false;
            _currentApp = null;
            _currentStage = 1;

            // Initialize badge counts for all apps to 0
            foreach (PhoneApp app in Enum.GetValues(typeof(PhoneApp)))
            {
                _badgeCounts[app] = 0;
            }
        }

        /// <summary>
        /// Open the phone overlay, displaying available apps.
        /// Requirements: R4.1
        /// </summary>
        public void OpenPhone()
        {
            if (_isPhoneOpen)
                return;

            _isPhoneOpen = true;
            _currentApp = null; // Start at home screen
            OnPhoneOpened?.Invoke();
        }

        /// <summary>
        /// Close the phone overlay and return to previous screen.
        /// Requirements: R4.8
        /// </summary>
        public void ClosePhone()
        {
            if (!_isPhoneOpen)
                return;

            // If an app is open, close it first
            if (_currentApp.HasValue)
            {
                var closedApp = _currentApp.Value;
                _currentApp = null;
                OnAppClosed?.Invoke(closedApp);
            }

            _isPhoneOpen = false;
            OnPhoneClosed?.Invoke();
        }

        /// <summary>
        /// Open a specific app within the phone.
        /// Requirements: R4.2-R4.5, R4.9-R4.11
        /// </summary>
        public void OpenApp(PhoneApp app)
        {
            // Phone must be open to open an app
            if (!_isPhoneOpen)
            {
                OpenPhone();
            }

            // If another app is open, close it first
            if (_currentApp.HasValue && _currentApp.Value != app)
            {
                var previousApp = _currentApp.Value;
                _currentApp = null;
                OnAppClosed?.Invoke(previousApp);
            }

            // Open the new app
            if (_currentApp != app)
            {
                _currentApp = app;
                OnAppOpened?.Invoke(app);
            }
        }

        /// <summary>
        /// Close the currently open app and return to phone home screen.
        /// </summary>
        public void CloseApp()
        {
            if (!_currentApp.HasValue)
                return;

            var closedApp = _currentApp.Value;
            _currentApp = null;
            OnAppClosed?.Invoke(closedApp);
        }

        /// <summary>
        /// Get notification badge count for an app.
        /// Requirements: R4.1, R4.6, R4.7
        /// </summary>
        public int GetBadgeCount(PhoneApp app)
        {
            return _badgeCounts.TryGetValue(app, out int count) ? count : 0;
        }

        /// <summary>
        /// Update badge count for an app.
        /// Requirements: R4.6, R4.7
        /// </summary>
        public void SetBadgeCount(PhoneApp app, int count)
        {
            // Ensure count is non-negative
            count = Math.Max(0, count);

            int previousCount = GetBadgeCount(app);
            _badgeCounts[app] = count;

            // Only fire event if count actually changed
            if (previousCount != count)
            {
                OnBadgeCountChanged?.Invoke(app, count);
            }
        }

        /// <summary>
        /// Increment badge count for an app by a specified amount.
        /// </summary>
        public void IncrementBadgeCount(PhoneApp app, int amount = 1)
        {
            int currentCount = GetBadgeCount(app);
            SetBadgeCount(app, currentCount + amount);
        }

        /// <summary>
        /// Clear badge count for an app (set to 0).
        /// </summary>
        public void ClearBadgeCount(PhoneApp app)
        {
            SetBadgeCount(app, 0);
        }

        /// <summary>
        /// Get total badge count across all apps.
        /// </summary>
        public int GetTotalBadgeCount()
        {
            int total = 0;
            foreach (var count in _badgeCounts.Values)
            {
                total += count;
            }
            return total;
        }

        /// <summary>
        /// Check if a specific app is currently open.
        /// </summary>
        public bool IsAppOpen(PhoneApp app)
        {
            return _currentApp.HasValue && _currentApp.Value == app;
        }
    }
}
