using System;

namespace EventPlannerSim.Data
{
    /// <summary>
    /// Game settings including audio, notifications, privacy, and accessibility.
    /// Requirements: R35.6-R35.27, R36.6-R36.7
    /// </summary>
    [Serializable]
    public class GameSettings
    {
        /// <summary>
        /// Music volume (0-1).
        /// Requirements: R35.6
        /// </summary>
        public float musicVolume = 1f;

        /// <summary>
        /// Sound effects volume (0-1).
        /// Requirements: R35.7
        /// </summary>
        public float sfxVolume = 1f;

        /// <summary>
        /// Mute all audio toggle.
        /// Requirements: R35.8
        /// </summary>
        public bool muteAll = false;

        /// <summary>
        /// Show tutorial tips for returning players.
        /// Requirements: R35.11
        /// </summary>
        public bool showTutorialTips = true;

        /// <summary>
        /// Notification preferences.
        /// Requirements: R35.10, R36.6-R36.7
        /// </summary>
        public NotificationSettings notifications = new NotificationSettings();

        /// <summary>
        /// Privacy and data collection settings.
        /// Requirements: R35.22-R35.26
        /// </summary>
        public PrivacySettings privacy = new PrivacySettings();

        /// <summary>
        /// Accessibility settings.
        /// Requirements: R35.17-R35.20
        /// </summary>
        public AccessibilitySettings accessibility = new AccessibilitySettings();

        /// <summary>
        /// Creates default settings with all notifications OFF (opt-in required).
        /// </summary>
        public static GameSettings CreateDefault()
        {
            return new GameSettings
            {
                musicVolume = 1f,
                sfxVolume = 1f,
                muteAll = false,
                showTutorialTips = true,
                notifications = NotificationSettings.CreateDefault(),
                privacy = PrivacySettings.CreateDefault(),
                accessibility = AccessibilitySettings.CreateDefault()
            };
        }
    }


    /// <summary>
    /// Notification preferences for push notifications.
    /// Requirements: R36.6-R36.7
    /// All notifications default to OFF, requiring player opt-in.
    /// </summary>
    [Serializable]
    public class NotificationSettings
    {
        /// <summary>
        /// Event deadline notifications.
        /// "Your event '[Event Name]' is tomorrow! Make sure everything is ready."
        /// Requirements: R36.1
        /// </summary>
        public bool eventDeadlines = false;

        /// <summary>
        /// Task deadline notifications (critical tasks only).
        /// "[Task Name] is due in 2 hours"
        /// Requirements: R36.2
        /// </summary>
        public bool taskDeadlines = false;

        /// <summary>
        /// New inquiry notifications (max 1 per day).
        /// "A new client is interested in your services!"
        /// Requirements: R36.3
        /// </summary>
        public bool newInquiries = false;

        /// <summary>
        /// Referral notifications.
        /// "You received a referral from [Client Name]!"
        /// Requirements: R36.4
        /// </summary>
        public bool referrals = false;

        /// <summary>
        /// Financial warning notifications.
        /// "Your funds are running low. Check your Bank app."
        /// Requirements: R36.5
        /// </summary>
        public bool financialWarnings = false;

        /// <summary>
        /// Creates default notification settings with all OFF (opt-in required).
        /// Requirements: R36.7
        /// </summary>
        public static NotificationSettings CreateDefault()
        {
            return new NotificationSettings
            {
                eventDeadlines = false,
                taskDeadlines = false,
                newInquiries = false,
                referrals = false,
                financialWarnings = false
            };
        }

        /// <summary>
        /// Enables all notification types.
        /// </summary>
        public void EnableAll()
        {
            eventDeadlines = true;
            taskDeadlines = true;
            newInquiries = true;
            referrals = true;
            financialWarnings = true;
        }

        /// <summary>
        /// Disables all notification types.
        /// </summary>
        public void DisableAll()
        {
            eventDeadlines = false;
            taskDeadlines = false;
            newInquiries = false;
            referrals = false;
            financialWarnings = false;
        }

        /// <summary>
        /// Checks if any notification type is enabled.
        /// </summary>
        public bool AnyEnabled => eventDeadlines || taskDeadlines || newInquiries || referrals || financialWarnings;
    }


    /// <summary>
    /// Privacy and data collection settings.
    /// Requirements: R35.22-R35.26
    /// </summary>
    [Serializable]
    public class PrivacySettings
    {
        /// <summary>
        /// Whether analytics data collection is enabled.
        /// Defaults to OFF, requires explicit opt-in (GDPR/CCPA compliance).
        /// Requirements: R35.23, R35.24
        /// </summary>
        public bool analyticsEnabled = false;

        /// <summary>
        /// Whether crash reporting is enabled.
        /// Defaults to ON for stability monitoring.
        /// </summary>
        public bool crashReportingEnabled = true;

        /// <summary>
        /// Timestamp when consent was given (stored as ticks for serialization).
        /// </summary>
        public long consentTimestampTicks;

        /// <summary>
        /// Whether the user has explicitly given consent.
        /// Requirements: R35.23
        /// </summary>
        public bool hasGivenConsent = false;

        /// <summary>
        /// Gets or sets the consent timestamp as DateTime.
        /// </summary>
        public DateTime ConsentTimestamp
        {
            get => new DateTime(consentTimestampTicks);
            set => consentTimestampTicks = value.Ticks;
        }

        /// <summary>
        /// Creates default privacy settings with analytics OFF (opt-in required).
        /// Requirements: R35.23
        /// </summary>
        public static PrivacySettings CreateDefault()
        {
            return new PrivacySettings
            {
                analyticsEnabled = false,
                crashReportingEnabled = true,
                hasGivenConsent = false,
                consentTimestampTicks = 0
            };
        }

        /// <summary>
        /// Records user consent for data collection.
        /// </summary>
        public void RecordConsent(bool analyticsConsent)
        {
            hasGivenConsent = true;
            analyticsEnabled = analyticsConsent;
            ConsentTimestamp = DateTime.UtcNow;
        }

        /// <summary>
        /// Revokes all data collection consent.
        /// Requirements: R35.25
        /// </summary>
        public void RevokeConsent()
        {
            hasGivenConsent = false;
            analyticsEnabled = false;
            ConsentTimestamp = DateTime.UtcNow;
        }
    }


    /// <summary>
    /// Accessibility settings for improved usability.
    /// Requirements: R35.17-R35.20
    /// </summary>
    [Serializable]
    public class AccessibilitySettings
    {
        /// <summary>
        /// Text size preference.
        /// Requirements: R35.17, R35.18
        /// </summary>
        public TextSize textSize = TextSize.Medium;

        /// <summary>
        /// Reduced motion for players sensitive to animations.
        /// Requirements: R35.20
        /// </summary>
        public bool reducedMotion = false;

        /// <summary>
        /// Colorblind mode for improved color distinction.
        /// Requirements: R35.19
        /// </summary>
        public ColorblindMode colorblindMode = ColorblindMode.None;

        /// <summary>
        /// Creates default accessibility settings.
        /// </summary>
        public static AccessibilitySettings CreateDefault()
        {
            return new AccessibilitySettings
            {
                textSize = TextSize.Medium,
                reducedMotion = false,
                colorblindMode = ColorblindMode.None
            };
        }

        /// <summary>
        /// Gets the text scale multiplier based on text size setting.
        /// </summary>
        public float GetTextScaleMultiplier() => textSize switch
        {
            TextSize.Small => 0.85f,
            TextSize.Medium => 1.0f,
            TextSize.Large => 1.25f,
            _ => 1.0f
        };
    }

    /// <summary>
    /// Text size options for accessibility.
    /// Requirements: R35.17
    /// </summary>
    public enum TextSize
    {
        Small,
        Medium,
        Large
    }

    /// <summary>
    /// Colorblind mode options for accessibility.
    /// Requirements: R35.19
    /// </summary>
    public enum ColorblindMode
    {
        None,
        Deuteranopia,  // Red-green (most common)
        Protanopia,    // Red-green
        Tritanopia     // Blue-yellow
    }
}
