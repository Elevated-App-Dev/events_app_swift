using System;
using System.Collections.Generic;
using EventPlannerSim.Core;
using EventPlannerSim.Data;

namespace EventPlannerSim.Systems
{
    /// <summary>
    /// Stub implementation of IStaffSystem for MVP.
    /// Returns empty/default values - full implementation is Post-MVP.
    /// Requirements: R19.1-R19.10
    /// </summary>
    public class StaffSystemImpl : IStaffSystem
    {
        private readonly List<StaffMember> _emptyStaffList = new List<StaffMember>();

        /// <inheritdoc />
        public event Action<StaffMember> OnStaffHired;

        /// <inheritdoc />
        public event Action<string> OnStaffFired;

        /// <inheritdoc />
        public event Action<string, string> OnStaffAssigned;

        /// <inheritdoc />
        public IReadOnlyList<StaffMember> GetAllStaff()
        {
            // MVP stub: No staff available
            return _emptyStaffList;
        }

        /// <inheritdoc />
        public bool HireStaff(StaffMember staff)
        {
            // MVP stub: Staff hiring not available
            return false;
        }

        /// <inheritdoc />
        public bool FireStaff(string staffId)
        {
            // MVP stub: Staff firing not available
            return false;
        }

        /// <inheritdoc />
        public bool AssignStaffToEvent(string staffId, string eventId)
        {
            // MVP stub: Staff assignment not available
            return false;
        }

        /// <inheritdoc />
        public bool UnassignStaffFromEvent(string staffId, string eventId)
        {
            // MVP stub: Staff unassignment not available
            return false;
        }

        /// <inheritdoc />
        public IReadOnlyList<StaffMember> GetStaffForEvent(string eventId)
        {
            // MVP stub: No staff assigned to any events
            return _emptyStaffList;
        }

        /// <inheritdoc />
        public float GetTaskEfficiencyModifier(string eventId)
        {
            // MVP stub: No efficiency modifier (1.0 = no change)
            return 1.0f;
        }

        /// <inheritdoc />
        public bool CheckTaskFailure(string eventId, float baseFailureChance)
        {
            // MVP stub: No additional failure chance from staff
            return false;
        }

        /// <inheritdoc />
        public float GetWeeklySalaryExpenses()
        {
            // MVP stub: No salary expenses
            return 0f;
        }

        /// <inheritdoc />
        public void UpdateStaffExperience(string eventId, float eventSatisfaction)
        {
            // MVP stub: No staff experience tracking
        }

        /// <inheritdoc />
        public bool RequiresCompanyApproval(CareerPath careerPath)
        {
            // MVP stub: Corporate path requires approval, but system is not active
            return careerPath == CareerPath.Corporate;
        }

        // Helper method to suppress unused event warnings
        protected virtual void RaiseStaffHired(StaffMember staff) => OnStaffHired?.Invoke(staff);
        protected virtual void RaiseStaffFired(string staffId) => OnStaffFired?.Invoke(staffId);
        protected virtual void RaiseStaffAssigned(string staffId, string eventId) => OnStaffAssigned?.Invoke(staffId, eventId);
    }
}
