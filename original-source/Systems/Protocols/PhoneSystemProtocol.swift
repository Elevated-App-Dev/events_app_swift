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
    func closeApp()
    func getBadgeCount(for app: PhoneApp) -> Int
    func setBadgeCount(for app: PhoneApp, count: Int)
    func incrementBadgeCount(for app: PhoneApp, by amount: Int)
    func clearBadgeCount(for app: PhoneApp)
    func getTotalBadgeCount() -> Int
    func isAppOpen(_ app: PhoneApp) -> Bool
    var onPhoneOpened: AnyPublisher<Void, Never> { get }
    var onPhoneClosed: AnyPublisher<Void, Never> { get }
    var onAppOpened: AnyPublisher<PhoneApp, Never> { get }
    var onAppClosed: AnyPublisher<Void, Never> { get }
    var onBadgeCountChanged: AnyPublisher<(PhoneApp, Int), Never> { get }
}
