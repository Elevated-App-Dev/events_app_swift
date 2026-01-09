using EventPlannerSim.Core;
using EventPlannerSim.Data;

namespace EventPlannerSim.Systems
{
    /// <summary>
    /// Manages emergency funding options for players facing financial hardship.
    /// Provides family help in Stages 1-2 with diminishing returns.
    /// Requirements: R34.1-R34.7
    /// </summary>
    public interface IEmergencyFundingSystem
    {
        /// <summary>
        /// Check if family help is available for the player.
        /// Family help is only available in Stages 1-2 and limited to 3 requests total.
        /// Requirements: R34.1, R34.2
        /// </summary>
        /// <param name="stage">Current business stage</param>
        /// <param name="familyHelpUsed">Number of times family help has been used</param>
        /// <returns>True if family help can be requested</returns>
        bool IsFamilyHelpAvailable(BusinessStage stage, int familyHelpUsed);

        /// <summary>
        /// Check if player's funds are low enough to enable emergency funding options.
        /// Requirements: R34.1
        /// </summary>
        /// <param name="currentMoney">Player's current money</param>
        /// <returns>True if funds are below the threshold ($500)</returns>
        bool IsLowOnFunds(float currentMoney);

        /// <summary>
        /// Get the amount of money that will be provided for the next family help request.
        /// Implements diminishing returns: $500, $400, $300.
        /// Requirements: R34.3, R34.4, R34.5
        /// </summary>
        /// <param name="familyHelpUsed">Number of times family help has been used (0-2)</param>
        /// <returns>Amount of money to be provided, or 0 if no more help available</returns>
        float GetFamilyHelpAmount(int familyHelpUsed);

        /// <summary>
        /// Get the number of remaining family help requests.
        /// Requirements: R34.2, R34.7
        /// </summary>
        /// <param name="familyHelpUsed">Number of times family help has been used</param>
        /// <returns>Number of remaining requests (0-3)</returns>
        int GetRemainingFamilyHelpRequests(int familyHelpUsed);

        /// <summary>
        /// Request family help and get the result.
        /// Requirements: R34.1-R34.7
        /// </summary>
        /// <param name="stage">Current business stage</param>
        /// <param name="familyHelpUsed">Number of times family help has been used</param>
        /// <returns>Result containing amount received and updated help count</returns>
        FamilyHelpResult RequestFamilyHelp(BusinessStage stage, int familyHelpUsed);

        /// <summary>
        /// Get the family message based on the request number.
        /// Requirements: R34.3, R34.4, R34.5, R34.6
        /// </summary>
        /// <param name="familyHelpUsed">Number of times family help has been used (before this request)</param>
        /// <returns>Message from family member</returns>
        string GetFamilyMessage(int familyHelpUsed);
    }
}
