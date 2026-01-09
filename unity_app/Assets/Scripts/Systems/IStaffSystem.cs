using System;
using System.Collections.Generic;
using EventPlannerSim.Core;
using EventPlannerSim.Data;

namespace EventPlannerSim.Systems
{
    /// <summary>
    /// Manages staff hiring, assignment, and performance for Stages 3-5.
    /// This is a stub interface for MVP - full implementation is Post-MVP.
    /// Requirements: R19.1-R19.10
    /// </summary>
    public interface IStaffSystem
    {
        /// <summary>
        /// Get all currently hired staff members.
        /// Requirements: R19.1, R19.2
        /// </summary>
        IReadOnlyList<StaffMember> GetAllStaff();

        /// <summary>
        /// Hire a new staff member (Entrepreneur Path only).
        /// Requirements: R19.1, R19.9
        /// </summary>
        bool HireStaff(StaffMember staff);

        /// <summary>
        /// Fire a staff member (Entrepreneur Path only).
        /// Requirements: R19.9
        /// </summary>
        bool FireStaff(string staffId);

        /// <summary>
        /// Assign staff to an event based on their specialties.
        /// Requirements: R19.3
        /// </summary>
        bool AssignStaffToEvent(string staffId, string eventId);

        /// <summary>
        /// Remove staff assignment from an event.
        /// </summary>
        bool UnassignStaffFromEvent(string staffId, string eventId);

        /// <summary>
        /// Get staff assigned to a specific event.
        /// </summary>
        IReadOnlyList<StaffMember> GetStaffForEvent(string eventId);

        /// <summary>
        /// Get the efficiency modifier for task completion based on assigned staff.
        /// Requirements: R19.4
        /// </summary>
        float GetTaskEfficiencyModifier(string eventId);

        /// <summary>
        /// Check if a task might fail due to staff reliability issues.
        /// Requirements: R19.5
        /// </summary>
        bool CheckTaskFailure(string eventId, float baseFailureChance);

        /// <summary>
        /// Calculate total weekly salary expenses (Entrepreneur Path).
        /// Requirements: R19.6
        /// </summary>
        float GetWeeklySalaryExpenses();

        /// <summary>
        /// Update staff skill levels after successful event completion.
        /// Requirements: R19.8
        /// </summary>
        void UpdateStaffExperience(string eventId, float eventSatisfaction);

        /// <summary>
        /// Check if staff changes require company approval (Corporate Path).
        /// Requirements: R19.10
        /// </summary>
        bool RequiresCompanyApproval(CareerPath careerPath);

        /// <summary>
        /// Event fired when staff is hired.
        /// </summary>
        event Action<StaffMember> OnStaffHired;

        /// <summary>
        /// Event fired when staff is fired.
        /// </summary>
        event Action<string> OnStaffFired;

        /// <summary>
        /// Event fired when staff is assigned to an event.
        /// </summary>
        event Action<string, string> OnStaffAssigned;
    }

    /// <summary>
    /// Represents a staff member that can be hired and assigned to events.
    /// </summary>
    [Serializable]
    public class StaffMember
    {
        public string staffId;
        public string name;
        public StaffRole role;
        public StaffSpecialty specialty;
        public int skillLevel; // 1-5
        public float reliability; // 0-1, affects task failure chance
        public float efficiency; // 0-1, affects task completion speed
        public float weeklySalary;
        public bool isCompanyProvided; // True for Corporate Path staff
        public List<string> assignedEventIds = new List<string>();
    }

    /// <summary>
    /// Staff roles available for hiring.
    /// </summary>
    public enum StaffRole
    {
        Assistant,
        Coordinator,
        Specialist,
        Manager
    }

    /// <summary>
    /// Staff specialties that match event types.
    /// </summary>
    public enum StaffSpecialty
    {
        General,
        Weddings,
        Corporate,
        Social,
        Luxury
    }
}
