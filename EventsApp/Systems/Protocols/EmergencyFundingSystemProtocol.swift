import Foundation

protocol EmergencyFundingSystemProtocol {
    func isFamilyHelpAvailable(stage: BusinessStage, familyHelpUsed: Int) -> Bool
    func isLowOnFunds(_ funds: Double) -> Bool
    func getFamilyHelpAmount(requestNumber: Int) -> Double
    func getRemainingFamilyHelpRequests(familyHelpUsed: Int) -> Int
    func requestFamilyHelp(stage: BusinessStage, familyHelpUsed: Int) -> FamilyHelpResult
    func getFamilyMessage(requestNumber: Int) -> String
}
