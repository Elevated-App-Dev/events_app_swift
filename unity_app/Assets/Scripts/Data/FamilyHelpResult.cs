using System;

namespace EventPlannerSim.Data
{
    /// <summary>
    /// Result of a family help request.
    /// Requirements: R34.1-R34.7
    /// </summary>
    [Serializable]
    public class FamilyHelpResult
    {
        /// <summary>
        /// Whether the request was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Amount of money received from family.
        /// $500 for first request, $400 for second, $300 for third.
        /// Requirements: R34.3, R34.4, R34.5
        /// </summary>
        public float AmountReceived { get; set; }

        /// <summary>
        /// Updated count of family help used after this request.
        /// </summary>
        public int NewFamilyHelpUsed { get; set; }

        /// <summary>
        /// Number of remaining family help requests after this one.
        /// Requirements: R34.7
        /// </summary>
        public int RemainingRequests { get; set; }

        /// <summary>
        /// Message from family member.
        /// Requirements: R34.3, R34.4, R34.5, R34.6
        /// </summary>
        public string FamilyMessage { get; set; }

        /// <summary>
        /// Reason for failure if Success is false.
        /// </summary>
        public string FailureReason { get; set; }

        /// <summary>
        /// The request number (1, 2, or 3) if successful.
        /// </summary>
        public int RequestNumber { get; set; }

        /// <summary>
        /// Creates a successful family help result.
        /// </summary>
        public static FamilyHelpResult CreateSuccess(float amount, int newHelpUsed, int remaining, string message, int requestNumber)
        {
            return new FamilyHelpResult
            {
                Success = true,
                AmountReceived = amount,
                NewFamilyHelpUsed = newHelpUsed,
                RemainingRequests = remaining,
                FamilyMessage = message,
                FailureReason = null,
                RequestNumber = requestNumber
            };
        }

        /// <summary>
        /// Creates a failed family help result.
        /// </summary>
        public static FamilyHelpResult CreateFailure(string reason, int currentHelpUsed)
        {
            return new FamilyHelpResult
            {
                Success = false,
                AmountReceived = 0f,
                NewFamilyHelpUsed = currentHelpUsed,
                RemainingRequests = System.Math.Max(0, 3 - currentHelpUsed),
                FamilyMessage = null,
                FailureReason = reason,
                RequestNumber = 0
            };
        }
    }
}
