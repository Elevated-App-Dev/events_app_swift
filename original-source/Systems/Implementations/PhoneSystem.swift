import Foundation
import Combine

/// Smartphone UI overlay and app navigation system.
/// Manages badge counts, app opening/closing, and stage-based app access.
class PhoneSystem: PhoneSystemProtocol {

    private var badgeCounts: [PhoneApp: Int] = [:]
    private(set) var isPhoneOpen = false
    private(set) var currentApp: PhoneApp?
    var currentStage: Int = 1

    private let _onPhoneOpened = PassthroughSubject<Void, Never>()
    private let _onPhoneClosed = PassthroughSubject<Void, Never>()
    private let _onAppOpened = PassthroughSubject<PhoneApp, Never>()
    private let _onBadgeChanged = PassthroughSubject<(PhoneApp, Int), Never>()

    var onPhoneOpened: AnyPublisher<Void, Never> { _onPhoneOpened.eraseToAnyPublisher() }
    var onPhoneClosed: AnyPublisher<Void, Never> { _onPhoneClosed.eraseToAnyPublisher() }
    var onAppOpened: AnyPublisher<PhoneApp, Never> { _onAppOpened.eraseToAnyPublisher() }
    var onBadgeChanged: AnyPublisher<(PhoneApp, Int), Never> { _onBadgeChanged.eraseToAnyPublisher() }

    /// Stage 2 company/personal separation active
    var isStage2SeparationActive: Bool { currentStage == 2 }

    init() {
        for app in PhoneApp.allCases {
            badgeCounts[app] = 0
        }
    }

    // MARK: - Available Apps

    func getAvailableApps(stage: Int) -> [PhoneApp] {
        let s = max(1, min(5, stage))
        var apps: [PhoneApp] = [.calendar, .messages, .bank, .contacts, .reviews, .tasks]

        if s >= 2 {
            apps.append(.clients)
        }
        if s >= 3 {
            apps.append(.marketing)
        }

        return apps
    }

    // MARK: - Phone State

    func openPhone() {
        guard !isPhoneOpen else { return }
        isPhoneOpen = true
        _onPhoneOpened.send()
    }

    func closePhone() {
        guard isPhoneOpen else { return }
        if currentApp != nil {
            closeCurrentApp()
        }
        isPhoneOpen = false
        _onPhoneClosed.send()
    }

    func openApp(_ app: PhoneApp) {
        if !isPhoneOpen { openPhone() }
        if currentApp != nil { closeCurrentApp() }
        currentApp = app
        _onAppOpened.send(app)
    }

    func closeCurrentApp() {
        currentApp = nil
    }

    // MARK: - Badges

    func getBadgeCount(for app: PhoneApp) -> Int {
        badgeCounts[app] ?? 0
    }

    func getTotalBadgeCount() -> Int {
        badgeCounts.values.reduce(0, +)
    }

    func setBadgeCount(_ count: Int, for app: PhoneApp) {
        let newCount = max(0, count)
        badgeCounts[app] = newCount
        _onBadgeChanged.send((app, newCount))
    }

    func incrementBadge(for app: PhoneApp) {
        let current = getBadgeCount(for: app)
        setBadgeCount(current + 1, for: app)
    }

    func clearBadge(for app: PhoneApp) {
        setBadgeCount(0, for: app)
    }

    func clearAllBadges() {
        for app in PhoneApp.allCases {
            clearBadge(for: app)
        }
    }
}
