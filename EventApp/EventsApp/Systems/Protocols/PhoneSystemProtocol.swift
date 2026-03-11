import Foundation
import Combine

protocol PhoneSystemProtocol {
    var isPhoneOpen: Bool { get }
    var currentApp: PhoneApp? { get }
    var currentStage: Int { get }
    var isStage2SeparationActive: Bool { get }
    func openPhone()
    func closePhone()
    func openApp(_ app: PhoneApp)
    func closeCurrentApp()
    func getAvailableApps(stage: Int) -> [PhoneApp]
    func getBadgeCount(for app: PhoneApp) -> Int
    func setBadgeCount(_ count: Int, for app: PhoneApp)
    func incrementBadge(for app: PhoneApp)
    func clearBadge(for app: PhoneApp)
    func clearAllBadges()
    func getTotalBadgeCount() -> Int
    var onPhoneOpened: AnyPublisher<Void, Never> { get }
    var onPhoneClosed: AnyPublisher<Void, Never> { get }
    var onAppOpened: AnyPublisher<PhoneApp, Never> { get }
    var onBadgeChanged: AnyPublisher<(PhoneApp, Int), Never> { get }
}
