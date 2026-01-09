using System;
using EventPlannerSim.Core;

namespace EventPlannerSim.Systems
{
    /// <summary>
    /// Stub implementation of IOfficeSystem for MVP.
    /// Returns 0 bonus - full implementation is Post-MVP.
    /// Requirements: R21.1-R21.11
    /// </summary>
    public class OfficeSystemImpl : IOfficeSystem
    {
        private OfficeType _currentOffice = OfficeType.HomeOffice;

        /// <inheritdoc />
        public event Action<OfficeType> OnOfficeUpgraded;

        /// <inheritdoc />
        public event Action<OfficeType, OfficeType> OnOfficeChanged;

        /// <inheritdoc />
        public OfficeType CurrentOffice => _currentOffice;

        /// <inheritdoc />
        public OfficeType GetOfficeForStage(BusinessStage stage, CareerPath careerPath)
        {
            // MVP implementation: Only Stage 1-2 offices are active
            return stage switch
            {
                BusinessStage.Solo => OfficeType.HomeOffice,
                BusinessStage.Employee => OfficeType.CompanyOffice,
                // Stage 3+ returns appropriate office but bonuses are not active in MVP
                BusinessStage.SmallCompany => careerPath == CareerPath.Corporate 
                    ? OfficeType.DirectorOffice 
                    : OfficeType.SmallOffice,
                BusinessStage.Established => careerPath == CareerPath.Corporate 
                    ? OfficeType.RegionalHeadquarters 
                    : OfficeType.AgencyOffice,
                BusinessStage.Premier => careerPath == CareerPath.Corporate 
                    ? OfficeType.ExecutiveSuite 
                    : OfficeType.FlagshipShowroom,
                _ => OfficeType.HomeOffice
            };
        }

        /// <inheritdoc />
        public float GetEfficiencyBonus()
        {
            // MVP stub: Returns 0 (no efficiency bonus)
            // Full implementation would return bonuses based on office type
            return 0f;
        }

        /// <inheritdoc />
        public float GetEfficiencyBonusForOffice(OfficeType office)
        {
            // MVP stub: Returns 0 for all offices
            // Full implementation would return:
            // - SmallOffice/DirectorOffice: 0.10 (+10%)
            // - AgencyOffice/RegionalHeadquarters: 0.20 (+20%)
            // - FlagshipShowroom/ExecutiveSuite: 0.30 (+30%)
            return 0f;
        }

        /// <inheritdoc />
        public bool CanUpgradeOffice(BusinessStage stage, CareerPath careerPath, float playerMoney)
        {
            // MVP stub: Office upgrades not available
            return false;
        }

        /// <inheritdoc />
        public float GetUpgradeCost(OfficeType targetOffice)
        {
            // Return defined costs for reference even in MVP
            return targetOffice switch
            {
                OfficeType.SmallOffice => 5000f,
                OfficeType.AgencyOffice => 15000f,
                OfficeType.FlagshipShowroom => 50000f,
                // Corporate offices are company-provided
                OfficeType.DirectorOffice => 0f,
                OfficeType.RegionalHeadquarters => 0f,
                OfficeType.ExecutiveSuite => 0f,
                _ => 0f
            };
        }

        /// <inheritdoc />
        public float GetMonthlyExpense(OfficeType office)
        {
            // Return defined expenses for reference even in MVP
            return office switch
            {
                OfficeType.HomeOffice => 0f,
                OfficeType.CompanyOffice => 0f, // Company-provided
                OfficeType.SmallOffice => 500f,
                OfficeType.AgencyOffice => 1500f,
                OfficeType.FlagshipShowroom => 3000f,
                // Corporate offices are company-provided
                OfficeType.DirectorOffice => 0f,
                OfficeType.RegionalHeadquarters => 0f,
                OfficeType.ExecutiveSuite => 0f,
                _ => 0f
            };
        }

        /// <inheritdoc />
        public float GetCurrentMonthlyExpense()
        {
            // MVP stub: No office expenses
            return 0f;
        }

        /// <inheritdoc />
        public bool UpgradeOffice(OfficeType targetOffice)
        {
            // MVP stub: Office upgrades not available
            return false;
        }

        /// <inheritdoc />
        public void UpdateOfficeForStage(BusinessStage stage, CareerPath careerPath)
        {
            var previousOffice = _currentOffice;
            _currentOffice = GetOfficeForStage(stage, careerPath);
            
            if (_currentOffice != previousOffice)
            {
                OnOfficeChanged?.Invoke(previousOffice, _currentOffice);
            }
        }

        /// <inheritdoc />
        public string GetOfficeName(OfficeType office)
        {
            return office switch
            {
                OfficeType.HomeOffice => "Home Office",
                OfficeType.CompanyOffice => "Premier Events Co. Office",
                OfficeType.SmallOffice => "Small Office",
                OfficeType.AgencyOffice => "Agency Office",
                OfficeType.FlagshipShowroom => "Flagship Showroom",
                OfficeType.DirectorOffice => "Director's Office",
                OfficeType.RegionalHeadquarters => "Regional Headquarters",
                OfficeType.ExecutiveSuite => "Executive Suite",
                _ => "Unknown Office"
            };
        }

        /// <inheritdoc />
        public string GetOfficeDescription(OfficeType office)
        {
            return office switch
            {
                OfficeType.HomeOffice => "Your humble beginnings - a dedicated workspace at home.",
                OfficeType.CompanyOffice => "Your desk at Premier Events Co. headquarters.",
                OfficeType.SmallOffice => "A small but professional office space for your growing business.",
                OfficeType.AgencyOffice => "A full agency office with meeting rooms and staff space.",
                OfficeType.FlagshipShowroom => "A prestigious showroom that impresses high-end clients.",
                OfficeType.DirectorOffice => "A corner office befitting your director status.",
                OfficeType.RegionalHeadquarters => "The regional headquarters under your leadership.",
                OfficeType.ExecutiveSuite => "The executive suite in the company's flagship building.",
                _ => "An office space."
            };
        }

        /// <inheritdoc />
        public bool IsCompanyProvided(CareerPath careerPath)
        {
            // Corporate path offices are always company-provided
            return careerPath == CareerPath.Corporate;
        }

        // Helper method to suppress unused event warning
        protected virtual void RaiseOfficeUpgraded(OfficeType office) => OnOfficeUpgraded?.Invoke(office);
    }
}
