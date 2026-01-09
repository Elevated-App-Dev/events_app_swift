using System;

namespace EventPlannerSim.Data
{
    /// <summary>
    /// Tracks the player's daily work capacity and overtime usage.
    /// Requirements: R10.7-R10.11
    /// </summary>
    [Serializable]
    public class WorkHoursData
    {
        /// <summary>
        /// Base daily work capacity in hours.
        /// </summary>
        public int dailyCapacity = 8;

        /// <summary>
        /// Hours used today from base capacity.
        /// </summary>
        public int hoursUsedToday = 0;

        /// <summary>
        /// Number of overtime ads watched today (max 2).
        /// Each ad grants 4 bonus hours.
        /// </summary>
        public int overtimeUsedToday = 0;

        /// <summary>
        /// Maximum overtime ads allowed per day.
        /// </summary>
        public const int MaxOvertimeAdsPerDay = 2;

        /// <summary>
        /// Hours granted per overtime ad.
        /// </summary>
        public const int HoursPerOvertimeAd = 4;

        /// <summary>
        /// Calculates remaining work hours for today.
        /// Includes base capacity plus any overtime hours, minus hours used.
        /// </summary>
        public int RemainingHours => dailyCapacity + (overtimeUsedToday * HoursPerOvertimeAd) - hoursUsedToday;

        /// <summary>
        /// Checks if the player can use overtime (watch an ad for bonus hours).
        /// Limited to 2 overtime ads per day.
        /// </summary>
        public bool CanUseOvertime => overtimeUsedToday < MaxOvertimeAdsPerDay;

        /// <summary>
        /// Resets daily work hours at the start of a new in-game day.
        /// </summary>
        public void ResetDaily()
        {
            hoursUsedToday = 0;
            overtimeUsedToday = 0;
        }

        /// <summary>
        /// Uses work hours for a task.
        /// Returns true if successful, false if not enough hours.
        /// </summary>
        public bool UseHours(int hours)
        {
            if (hours > RemainingHours)
                return false;

            hoursUsedToday += hours;
            return true;
        }

        /// <summary>
        /// Adds overtime hours by watching an ad.
        /// Returns true if successful, false if max overtime reached.
        /// </summary>
        public bool AddOvertime()
        {
            if (!CanUseOvertime)
                return false;

            overtimeUsedToday++;
            return true;
        }

        /// <summary>
        /// Gets the total available hours for today (base + overtime).
        /// </summary>
        public int TotalAvailableHours => dailyCapacity + (overtimeUsedToday * HoursPerOvertimeAd);
    }
}
