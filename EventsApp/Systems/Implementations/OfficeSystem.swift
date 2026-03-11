import Foundation
import Combine

/// Stub implementation of OfficeSystem for MVP.
/// Returns 0 bonus — full implementation is Post-MVP.
class OfficeSystem: OfficeSystemProtocol {

    private(set) var currentOffice: OfficeType = .homeOffice

    private let _onOfficeUpgraded = PassthroughSubject<OfficeType, Never>()
    private let _onOfficeChanged = PassthroughSubject<OfficeType, Never>()

    var onOfficeUpgraded: AnyPublisher<OfficeType, Never> { _onOfficeUpgraded.eraseToAnyPublisher() }
    var onOfficeChanged: AnyPublisher<OfficeType, Never> { _onOfficeChanged.eraseToAnyPublisher() }

    func getOfficeForStage(_ stage: BusinessStage, path: CareerPath) -> OfficeType {
        switch stage {
        case .solo:         return .homeOffice
        case .employee:     return .companyOffice
        case .smallCompany: return path == .corporate ? .directorOffice : .smallOffice
        case .established:  return path == .corporate ? .regionalHeadquarters : .agencyOffice
        case .premier:      return path == .corporate ? .executiveSuite : .flagshipShowroom
        }
    }

    func getEfficiencyBonus() -> Double { 0 }
    func getEfficiencyBonusForOffice(_ office: OfficeType) -> Double { 0 }

    func canUpgradeOffice(stage: BusinessStage, path: CareerPath, funds: Double) -> Bool { false }

    func getUpgradeCost(_ office: OfficeType) -> Double {
        switch office {
        case .smallOffice:       return 5000
        case .agencyOffice:      return 15000
        case .flagshipShowroom:  return 50000
        default:                 return 0
        }
    }

    func getMonthlyExpense(_ office: OfficeType) -> Double {
        switch office {
        case .homeOffice, .companyOffice, .directorOffice, .regionalHeadquarters, .executiveSuite:
            return 0
        case .smallOffice:      return 500
        case .agencyOffice:     return 1500
        case .flagshipShowroom: return 3000
        }
    }

    func getCurrentMonthlyExpense() -> Double { 0 }

    func upgradeOffice(_ office: OfficeType) {
        // MVP stub
    }

    func updateOfficeForStage(_ stage: BusinessStage, path: CareerPath) {
        let previous = currentOffice
        currentOffice = getOfficeForStage(stage, path: path)
        if currentOffice != previous {
            _onOfficeChanged.send(currentOffice)
        }
    }

    func getOfficeName(_ office: OfficeType) -> String {
        switch office {
        case .homeOffice:             return "Home Office"
        case .companyOffice:          return "Premier Events Co. Office"
        case .smallOffice:            return "Small Office"
        case .agencyOffice:           return "Agency Office"
        case .flagshipShowroom:       return "Flagship Showroom"
        case .directorOffice:         return "Director's Office"
        case .regionalHeadquarters:   return "Regional Headquarters"
        case .executiveSuite:         return "Executive Suite"
        }
    }

    func getOfficeDescription(_ office: OfficeType) -> String {
        switch office {
        case .homeOffice:           return "Your humble beginnings - a dedicated workspace at home."
        case .companyOffice:        return "Your desk at Premier Events Co. headquarters."
        case .smallOffice:          return "A small but professional office space for your growing business."
        case .agencyOffice:         return "A full agency office with meeting rooms and staff space."
        case .flagshipShowroom:     return "A prestigious showroom that impresses high-end clients."
        case .directorOffice:       return "A corner office befitting your director status."
        case .regionalHeadquarters: return "The regional headquarters under your leadership."
        case .executiveSuite:       return "The executive suite in the company's flagship building."
        }
    }

    func isCompanyProvided(_ path: CareerPath) -> Bool {
        path == .corporate
    }
}
