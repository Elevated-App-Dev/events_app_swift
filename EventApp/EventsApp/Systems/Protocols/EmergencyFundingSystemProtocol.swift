import Foundation

protocol EmergencyFundingSystemProtocol {
    func isFamilyHelpAvailable(stage: BusinessStage, familyHelpUsed: Int) -> Bool
    func isLowOnFunds(currentMoney: Double) -> Bool
    func getFamilyHelpAmount(familyHelpUsed: Int) -> Double
    func getRemainingFamilyHelpRequests(familyHelpUsed: Int) -> Int
    func requestFamilyHelp(stage: BusinessStage, familyHelpUsed: Int) -> FamilyHelpResult
    func getFamilyMessage(familyHelpUsed: Int) -> String
}
