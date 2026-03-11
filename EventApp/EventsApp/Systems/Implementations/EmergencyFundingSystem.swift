import Foundation

/// Manages family help in Stages 1-2 with diminishing returns.
/// 3 requests max: $500, $400, $300.
class EmergencyFundingSystem: EmergencyFundingSystemProtocol {

    private static let maxFamilyHelpRequests = 3
    private static let lowFundsThreshold: Double = 500
    private static let familyHelpAmounts: [Double] = [500, 400, 300]

    private static let familyMessages = [
        "We're happy to help you get started! Here's $500 to help with your business. We believe in you!",
        "We're a bit worried about how things are going, but we want to support you. Here's $400.",
        "This is the last time we can help financially. Here's $300. We really hope things turn around for you."
    ]

    func isFamilyHelpAvailable(stage: BusinessStage, familyHelpUsed: Int) -> Bool {
        guard stage == .solo || stage == .employee else { return false }
        return familyHelpUsed < Self.maxFamilyHelpRequests
    }

    func isLowOnFunds(currentMoney: Double) -> Bool {
        currentMoney < Self.lowFundsThreshold
    }

    func getFamilyHelpAmount(familyHelpUsed: Int) -> Double {
        guard familyHelpUsed < Self.maxFamilyHelpRequests else { return 0 }
        return Self.familyHelpAmounts[familyHelpUsed]
    }

    func getRemainingFamilyHelpRequests(familyHelpUsed: Int) -> Int {
        max(0, Self.maxFamilyHelpRequests - familyHelpUsed)
    }

    func requestFamilyHelp(stage: BusinessStage, familyHelpUsed: Int) -> FamilyHelpResult {
        guard isFamilyHelpAvailable(stage: stage, familyHelpUsed: familyHelpUsed) else {
            if stage != .solo && stage != .employee {
                return FamilyHelpResult.createFailure(
                    reason: "Family help is only available in Stages 1-2.",
                    currentHelpUsed: familyHelpUsed
                )
            }
            return FamilyHelpResult.createFailure(
                reason: "Family cannot help further.",
                currentHelpUsed: familyHelpUsed
            )
        }

        let amount = getFamilyHelpAmount(familyHelpUsed: familyHelpUsed)
        let message = getFamilyMessage(familyHelpUsed: familyHelpUsed)
        let newHelpUsed = familyHelpUsed + 1
        let remaining = getRemainingFamilyHelpRequests(familyHelpUsed: newHelpUsed)

        return FamilyHelpResult.createSuccess(
            amount: amount,
            newHelpUsed: newHelpUsed,
            remaining: remaining,
            message: message,
            requestNumber: newHelpUsed
        )
    }

    func getFamilyMessage(familyHelpUsed: Int) -> String {
        guard familyHelpUsed < Self.maxFamilyHelpRequests else {
            return "Family cannot help further."
        }
        return Self.familyMessages[familyHelpUsed]
    }
}
