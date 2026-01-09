using System;
using EventPlannerSim.Core;

namespace EventPlannerSim.Systems
{
    /// <summary>
    /// Manages office progression and workspace bonuses.
    /// This is a stub interface for MVP - full implementation is Post-MVP.
    /// Requirements: R21.1-R21.11
    /// </summary>
    public interface IOfficeSystem
    {
        /// <summary>
        /// Get the current office type for the player.
        /// Requirements: R21.1-R21.3
        /// </summary>
        OfficeType CurrentOffice { get; }

        /// <summary>
        /// Get the office type based on stage and career path.
        /// Requirements: R21.1-R21.11
        /// </summary>
        OfficeType GetOfficeForStage(BusinessStage stage, CareerPath careerPath);

        /// <summary>
        /// Get the efficiency bonus percentage for the current office.
        /// Returns 0 for MVP (no bonus).
        /// Requirements: R21.4-R21.10
        /// </summary>
        float GetEfficiencyBonus();

        /// <summary>
        /// Get the efficiency bonus for a specific office type.
        /// Requirements: R21.4-R21.10
        /// </summary>
        float GetEfficiencyBonusForOffice(OfficeType office);

        /// <summary>
        /// Check if office upgrade is available.
        /// Requirements: R21.4-R21.6
        /// </summary>
        bool CanUpgradeOffice(BusinessStage stage, CareerPath careerPath, float playerMoney);

        /// <summary>
        /// Get the cost to upgrade to the next office tier (Entrepreneur Path).
        /// Requirements: R21.7
        /// </summary>
        float GetUpgradeCost(OfficeType targetOffice);

        /// <summary>
        /// Get the monthly rent/mortgage for an office (Entrepreneur Path).
        /// Requirements: R21.7
        /// </summary>
        float GetMonthlyExpense(OfficeType office);

        /// <summary>
        /// Get the current monthly office expense.
        /// Requirements: R21.7, R21.11
        /// </summary>
        float GetCurrentMonthlyExpense();

        /// <summary>
        /// Upgrade to a new office (Entrepreneur Path only).
        /// Requirements: R21.4-R21.6
        /// </summary>
        bool UpgradeOffice(OfficeType targetOffice);

        /// <summary>
        /// Update office based on stage progression.
        /// Called when player advances to a new stage.
        /// Requirements: R21.2, R21.8-R21.10
        /// </summary>
        void UpdateOfficeForStage(BusinessStage stage, CareerPath careerPath);

        /// <summary>
        /// Get the display name for an office type.
        /// </summary>
        string GetOfficeName(OfficeType office);

        /// <summary>
        /// Get the description for an office type.
        /// </summary>
        string GetOfficeDescription(OfficeType office);

        /// <summary>
        /// Check if office expenses are paid by company (Corporate Path).
        /// Requirements: R21.11
        /// </summary>
        bool IsCompanyProvided(CareerPath careerPath);

        /// <summary>
        /// Event fired when office is upgraded.
        /// </summary>
        event Action<OfficeType> OnOfficeUpgraded;

        /// <summary>
        /// Event fired when office changes due to stage progression.
        /// </summary>
        event Action<OfficeType, OfficeType> OnOfficeChanged;
    }

    /// <summary>
    /// Office types available throughout the game.
    /// </summary>
    public enum OfficeType
    {
        // Stage 1
        HomeOffice,             // Default starting office

        // Stage 2
        CompanyOffice,          // Premier Events Co. office

        // Stage 3+ Entrepreneur Path
        SmallOffice,            // +10% efficiency
        AgencyOffice,           // +20% efficiency (Stage 4)
        FlagshipShowroom,       // +30% efficiency (Stage 5)

        // Stage 3+ Corporate Path
        DirectorOffice,         // +10% efficiency
        RegionalHeadquarters,   // +20% efficiency (Stage 4)
        ExecutiveSuite          // +30% efficiency (Stage 5)
    }
}
