import Foundation
import Combine

protocol OfficeSystemProtocol {
    var currentOffice: OfficeType { get }
    func getOfficeForStage(_ stage: BusinessStage, path: CareerPath) -> OfficeType
    func getEfficiencyBonus() -> Double
    func getEfficiencyBonusForOffice(_ office: OfficeType) -> Double
    func canUpgradeOffice(stage: BusinessStage, path: CareerPath, funds: Double) -> Bool
    func getUpgradeCost(_ office: OfficeType) -> Double
    func getMonthlyExpense(_ office: OfficeType) -> Double
    func getCurrentMonthlyExpense() -> Double
    func upgradeOffice(_ office: OfficeType)
    func updateOfficeForStage(_ stage: BusinessStage, path: CareerPath)
    func getOfficeName(_ office: OfficeType) -> String
    func getOfficeDescription(_ office: OfficeType) -> String
    func isCompanyProvided(_ path: CareerPath) -> Bool
    var onOfficeUpgraded: AnyPublisher<OfficeType, Never> { get }
    var onOfficeChanged: AnyPublisher<OfficeType, Never> { get }
}
