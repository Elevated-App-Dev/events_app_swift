using System;
using EventPlannerSim.Core;

namespace EventPlannerSim.Systems
{
    /// <summary>
    /// Manages the smartphone UI overlay and app navigation.
    /// Requirements: R4.1-R4.14
    /// </summary>
    public interface IPhoneSystem
    {
        /// <summary>
        /// Open the phone overlay, displaying available apps.
        /// Requirements: R4.1
        /// </summary>
        void OpenPhone();

        /// <summary>
        /// Close the phone overlay and return to previous screen.
        /// Requirements: R4.8
        /// </summary>
        void ClosePhone();

        /// <summary>
        /// Open a specific app within the phone.
        /// Requirements: R4.2-R4.5, R4.9-R4.11
        /// </summary>
        void OpenApp(PhoneApp app);

        /// <summary>
        /// Close the currently open app and return to phone home screen.
        /// </summary>
        void CloseApp();

        /// <summary>
        /// Get notification badge count for an app.
        /// Requirements: R4.1, R4.6, R4.7
        /// </summary>
        int GetBadgeCount(PhoneApp app);

        /// <summary>
        /// Update badge count for an app.
        /// Requirements: R4.6, R4.7
        /// </summary>
        void SetBadgeCount(PhoneApp app, int count);

        /// <summary>
        /// Increment badge count for an app by a specified amount.
        /// </summary>
        void IncrementBadgeCount(PhoneApp app, int amount = 1);

        /// <summary>
        /// Clear badge count for an app (set to 0).
        /// </summary>
        void ClearBadgeCount(PhoneApp app);

        /// <summary>
        /// Get total badge count across all apps.
        /// </summary>
        int GetTotalBadgeCount();

        /// <summary>
        /// Check if phone overlay is currently visible.
        /// </summary>
        bool IsPhoneOpen { get; }

        /// <summary>
        /// Get the currently open app, or null if on home screen.
        /// </summary>
        PhoneApp? CurrentApp { get; }

        /// <summary>
        /// Check if a specific app is currently open.
        /// </summary>
        bool IsAppOpen(PhoneApp app);

        /// <summary>
        /// Get the current business stage for Stage 2 specific features.
        /// Requirements: R4.12-R4.14
        /// </summary>
        int CurrentStage { get; set; }

        /// <summary>
        /// Check if Stage 2 company/personal separation is active.
        /// Requirements: R4.12-R4.14
        /// </summary>
        bool IsStage2SeparationActive { get; }

        /// <summary>
        /// Event fired when phone is opened.
        /// </summary>
        event Action OnPhoneOpened;

        /// <summary>
        /// Event fired when phone is closed.
        /// </summary>
        event Action OnPhoneClosed;

        /// <summary>
        /// Event fired when an app is opened.
        /// </summary>
        event Action<PhoneApp> OnAppOpened;

        /// <summary>
        /// Event fired when an app is closed.
        /// </summary>
        event Action<PhoneApp> OnAppClosed;

        /// <summary>
        /// Event fired when a badge count changes.
        /// </summary>
        event Action<PhoneApp, int> OnBadgeCountChanged;
    }
}
