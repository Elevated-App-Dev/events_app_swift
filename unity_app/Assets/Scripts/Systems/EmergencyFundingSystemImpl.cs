using EventPlannerSim.Core;
using EventPlannerSim.Data;

namespace EventPlannerSim.Systems
{
    /// <summary>
    /// Implementation of the Emergency Funding System.
    /// Manages family help in Stages 1-2 with diminishing returns.
    /// Requirements: R34.1-R34.7
    /// </summary>
    public class EmergencyFundingSystemImpl : IEmergencyFundingSystem
    {
        // Constants for family help system
        private const int MaxFamilyHelpRequests = 3;
        private const float LowFundsThreshold = 500f;
        
        // Diminishing returns amounts: $500, $400, $300
        private static readonly float[] FamilyHelpAmounts = { 500f, 400f, 300f };
        
        // Family messages for each request
        private static readonly string[] FamilyMessages = 
        {
            "We're happy to help you get started! Here's $500 to help with your business. We believe in you!",
            "We're a bit worried about how things are going, but we want to support you. Here's $400.",
            "This is the last time we can help financially. Here's $300. We really hope things turn around for you."
        };

        /// <summary>
        /// Check if family help is available for the player.
        /// Family help is only available in Stages 1-2 and limited to 3 requests total.
        /// Requirements: R34.1, R34.2
        /// </summary>
        public bool IsFamilyHelpAvailable(BusinessStage stage, int familyHelpUsed)
        {
            // Family help only available in Stages 1-2 (R34.1)
            if (stage != BusinessStage.Solo && stage != BusinessStage.Employee)
            {
                return false;
            }

            // Limited to 3 requests total (R34.2)
            return familyHelpUsed < MaxFamilyHelpRequests;
        }

        /// <summary>
        /// Check if player's funds are low enough to enable emergency funding options.
        /// Requirements: R34.1
        /// </summary>
        public bool IsLowOnFunds(float currentMoney)
        {
            // Funds must be below $500 to enable family help (R34.1)
            return currentMoney < LowFundsThreshold;
        }

        /// <summary>
        /// Get the amount of money that will be provided for the next family help request.
        /// Implements diminishing returns: $500, $400, $300.
        /// Requirements: R34.3, R34.4, R34.5
        /// </summary>
        public float GetFamilyHelpAmount(int familyHelpUsed)
        {
            // No more help available after 3 requests (R34.6)
            if (familyHelpUsed >= MaxFamilyHelpRequests)
            {
                return 0f;
            }

            // Return amount based on request number (R34.3, R34.4, R34.5)
            // familyHelpUsed = 0 -> $500 (first request)
            // familyHelpUsed = 1 -> $400 (second request)
            // familyHelpUsed = 2 -> $300 (third request)
            return FamilyHelpAmounts[familyHelpUsed];
        }

        /// <summary>
        /// Get the number of remaining family help requests.
        /// Requirements: R34.2, R34.7
        /// </summary>
        public int GetRemainingFamilyHelpRequests(int familyHelpUsed)
        {
            int remaining = MaxFamilyHelpRequests - familyHelpUsed;
            return remaining > 0 ? remaining : 0;
        }

        /// <summary>
        /// Request family help and get the result.
        /// Requirements: R34.1-R34.7
        /// </summary>
        public FamilyHelpResult RequestFamilyHelp(BusinessStage stage, int familyHelpUsed)
        {
            // Check if family help is available
            if (!IsFamilyHelpAvailable(stage, familyHelpUsed))
            {
                // Determine failure reason
                if (stage != BusinessStage.Solo && stage != BusinessStage.Employee)
                {
                    return FamilyHelpResult.CreateFailure(
                        "Family help is only available in Stages 1-2.",
                        familyHelpUsed);
                }
                
                // R34.6: Family cannot help further
                return FamilyHelpResult.CreateFailure(
                    "Family cannot help further.",
                    familyHelpUsed);
            }

            // Get the amount for this request
            float amount = GetFamilyHelpAmount(familyHelpUsed);
            
            // Get the message for this request
            string message = GetFamilyMessage(familyHelpUsed);
            
            // Calculate new values
            int newHelpUsed = familyHelpUsed + 1;
            int remaining = GetRemainingFamilyHelpRequests(newHelpUsed);
            int requestNumber = newHelpUsed; // 1, 2, or 3

            return FamilyHelpResult.CreateSuccess(amount, newHelpUsed, remaining, message, requestNumber);
        }

        /// <summary>
        /// Get the family message based on the request number.
        /// Requirements: R34.3, R34.4, R34.5, R34.6
        /// </summary>
        public string GetFamilyMessage(int familyHelpUsed)
        {
            // No message if help is exhausted (R34.6)
            if (familyHelpUsed >= MaxFamilyHelpRequests)
            {
                return "Family cannot help further.";
            }

            return FamilyMessages[familyHelpUsed];
        }
    }
}
