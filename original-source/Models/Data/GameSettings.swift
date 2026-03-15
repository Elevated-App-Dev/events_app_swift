import Foundation

struct GameSettings: Codable, Equatable {
    var musicVolume: Double = 1.0
    var sfxVolume: Double = 1.0
    var muteAll: Bool = false
    var showTutorialTips: Bool = true
    var notifications: NotificationSettings = NotificationSettings()
    var privacy: PrivacySettings = PrivacySettings()
    var accessibility: AccessibilitySettings = AccessibilitySettings()

    static func createDefault() -> GameSettings {
        GameSettings()
    }
}

struct NotificationSettings: Codable, Equatable {
    var eventDeadlines: Bool = false
    var taskDeadlines: Bool = false
    var newInquiries: Bool = false
    var referrals: Bool = false
    var financialWarnings: Bool = false

    static func createDefault() -> NotificationSettings {
        NotificationSettings()
    }

    mutating func enableAll() {
        eventDeadlines = true
        taskDeadlines = true
        newInquiries = true
        referrals = true
        financialWarnings = true
    }

    mutating func disableAll() {
        eventDeadlines = false
        taskDeadlines = false
        newInquiries = false
        referrals = false
        financialWarnings = false
    }

    var anyEnabled: Bool {
        eventDeadlines || taskDeadlines || newInquiries || referrals || financialWarnings
    }
}

struct PrivacySettings: Codable, Equatable {
    var analyticsEnabled: Bool = false
    var crashReportingEnabled: Bool = true
    var consentTimestamp: Date?
    var hasGivenConsent: Bool = false

    static func createDefault() -> PrivacySettings {
        PrivacySettings()
    }

    mutating func recordConsent(analyticsConsent: Bool) {
        analyticsEnabled = analyticsConsent
        consentTimestamp = Date()
        hasGivenConsent = true
    }

    mutating func revokeConsent() {
        analyticsEnabled = false
        hasGivenConsent = false
        consentTimestamp = nil
    }
}

struct AccessibilitySettings: Codable, Equatable {
    var textSize: TextSize = .medium
    var reducedMotion: Bool = false
    var colorblindMode: ColorblindMode = .none

    static func createDefault() -> AccessibilitySettings {
        AccessibilitySettings()
    }

    func getTextScaleMultiplier() -> Double {
        switch textSize {
        case .small: return 0.85
        case .medium: return 1.0
        case .large: return 1.2
        }
    }
}
