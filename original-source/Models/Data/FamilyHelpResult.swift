import Foundation

struct FamilyHelpResult: Codable, Equatable {
    var success: Bool
    var amountReceived: Double
    var newFamilyHelpUsed: Int
    var remainingRequests: Int
    var familyMessage: String
    var failureReason: String?
    var requestNumber: Int

    static func createSuccess(amount: Double, newHelpUsed: Int, remaining: Int, message: String, requestNumber: Int) -> FamilyHelpResult {
        FamilyHelpResult(
            success: true,
            amountReceived: amount,
            newFamilyHelpUsed: newHelpUsed,
            remainingRequests: remaining,
            familyMessage: message,
            requestNumber: requestNumber
        )
    }

    static func createFailure(reason: String, currentHelpUsed: Int) -> FamilyHelpResult {
        FamilyHelpResult(
            success: false,
            amountReceived: 0,
            newFamilyHelpUsed: currentHelpUsed,
            remainingRequests: max(0, 3 - currentHelpUsed),
            familyMessage: "",
            failureReason: reason,
            requestNumber: currentHelpUsed
        )
    }
}
