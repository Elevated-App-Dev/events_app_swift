using System;
using EventPlannerSim.Core;

namespace EventPlannerSim.Systems
{
    /// <summary>
    /// Result of event phase calculation containing phase info and timing.
    /// </summary>
    [Serializable]
    public class EventPhaseInfo
    {
        /// <summary>
        /// The current phase of the event.
        /// </summary>
        public EventPhase Phase { get; set; }

        /// <summary>
        /// The stress weight multiplier for this phase.
        /// </summary>
        public float StressWeight { get; set; }

        /// <summary>
        /// Days remaining in the current phase.
        /// </summary>
        public int DaysRemainingInPhase { get; set; }

        /// <summary>
        /// Total days in the current phase.
        /// </summary>
        public int TotalDaysInPhase { get; set; }

        /// <summary>
        /// Days until the event execution day.
        /// </summary>
        public int DaysUntilEvent { get; set; }

        /// <summary>
        /// Whether this is a high-stress phase (Final Prep or Execution Day).
        /// </summary>
        public bool IsHighStressPhase => Phase == EventPhase.FinalPrep || Phase == EventPhase.ExecutionDay;
    }

    /// <summary>
    /// Contains the phase schedule for an event.
    /// </summary>
    [Serializable]
    public class EventPhaseSchedule
    {
        public GameDate AcceptedDate { get; set; }
        public GameDate EventDate { get; set; }
        public GameDate BookingEndDate { get; set; }
        public GameDate PrePlanningEndDate { get; set; }
        public GameDate ActivePlanningEndDate { get; set; }
        public GameDate FinalPrepEndDate { get; set; }

        public int BookingDays { get; set; }
        public int PrePlanningDays { get; set; }
        public int ActivePlanningDays { get; set; }
        public int FinalPrepDays { get; set; }
    }

    /// <summary>
    /// Calculator for event phases and stress weights.
    /// Pure C# logic - no Unity dependencies for testability.
    /// Implements R11.4-R11.7.
    /// </summary>
    public static class EventPhaseCalculator
    {
        // Stress weights from R11.5
        private static readonly float[] StressWeights = new float[]
        {
            0.5f,   // Booking
            0.75f,  // PrePlanning
            1.0f,   // ActivePlanning
            1.5f,   // FinalPrep
            2.0f,   // ExecutionDay
            0.0f    // Results (no stress)
        };

        // Phase time allocations from R11.4
        private const int BookingDays = 2;
        private const float PrePlanningPercent = 0.25f;
        private const float ActivePlanningPercent = 0.50f;
        private const float FinalPrepPercent = 0.20f;
        // Remaining 5% goes to buffer/rounding

        /// <summary>
        /// Gets the stress weight for a given event phase.
        /// Booking = 0.5x, Pre-Planning = 0.75x, Active Planning = 1.0x,
        /// Final Prep = 1.5x, Execution Day = 2.0x, Results = 0.0x.
        /// </summary>
        /// <param name="phase">The event phase.</param>
        /// <returns>The stress weight multiplier.</returns>
        public static float GetStressWeight(EventPhase phase)
        {
            int index = (int)phase;
            if (index < 0 || index >= StressWeights.Length)
                return 1.0f;
            return StressWeights[index];
        }

        /// <summary>
        /// Calculates the phase schedule for an event based on accepted and event dates.
        /// Phase allocation from R11.4:
        /// - Booking: days 1-2 (fixed)
        /// - Pre-Planning: 25% of remaining time
        /// - Active Planning: 50% of remaining time
        /// - Final Prep: 20% of remaining time
        /// - Execution Day: last day
        /// </summary>
        /// <param name="acceptedDate">The date the event was accepted.</param>
        /// <param name="eventDate">The date of the event.</param>
        /// <returns>The phase schedule with all phase boundaries.</returns>
        public static EventPhaseSchedule CalculatePhaseSchedule(GameDate acceptedDate, GameDate eventDate)
        {
            int totalDays = GameDate.DaysBetween(acceptedDate, eventDate);
            
            // Ensure minimum time for phases
            if (totalDays < 3)
            {
                // Very short event - compress phases
                return new EventPhaseSchedule
                {
                    AcceptedDate = acceptedDate,
                    EventDate = eventDate,
                    BookingEndDate = acceptedDate.AddDays(Math.Min(1, totalDays - 1)),
                    PrePlanningEndDate = acceptedDate.AddDays(Math.Min(1, totalDays - 1)),
                    ActivePlanningEndDate = acceptedDate.AddDays(Math.Max(1, totalDays - 1)),
                    FinalPrepEndDate = eventDate.AddDays(-1),
                    BookingDays = Math.Min(1, totalDays - 1),
                    PrePlanningDays = 0,
                    ActivePlanningDays = Math.Max(0, totalDays - 2),
                    FinalPrepDays = 1
                };
            }

            // Calculate booking phase (fixed 2 days or less if event is soon)
            int bookingDays = Math.Min(BookingDays, totalDays - 1);
            int remainingAfterBooking = totalDays - bookingDays - 1; // -1 for execution day

            // Calculate other phases based on remaining time
            int prePlanningDays = Math.Max(1, (int)(remainingAfterBooking * PrePlanningPercent));
            int activePlanningDays = Math.Max(1, (int)(remainingAfterBooking * ActivePlanningPercent));
            int finalPrepDays = Math.Max(1, (int)(remainingAfterBooking * FinalPrepPercent));

            // Adjust to ensure we don't exceed total days
            int allocatedDays = bookingDays + prePlanningDays + activePlanningDays + finalPrepDays + 1; // +1 for execution day
            
            if (allocatedDays > totalDays)
            {
                // Reduce active planning first, then pre-planning
                int excess = allocatedDays - totalDays;
                int reduceActive = Math.Min(excess, activePlanningDays - 1);
                activePlanningDays -= reduceActive;
                excess -= reduceActive;
                
                if (excess > 0)
                {
                    int reducePrePlanning = Math.Min(excess, prePlanningDays - 1);
                    prePlanningDays -= reducePrePlanning;
                    excess -= reducePrePlanning;
                }
                
                if (excess > 0)
                {
                    int reduceFinalPrep = Math.Min(excess, finalPrepDays - 1);
                    finalPrepDays -= reduceFinalPrep;
                }
            }
            else if (allocatedDays < totalDays)
            {
                // Add extra days to active planning
                activePlanningDays += totalDays - allocatedDays;
            }

            // Calculate end dates for each phase
            var bookingEndDate = acceptedDate.AddDays(bookingDays);
            var prePlanningEndDate = bookingEndDate.AddDays(prePlanningDays);
            var activePlanningEndDate = prePlanningEndDate.AddDays(activePlanningDays);
            var finalPrepEndDate = activePlanningEndDate.AddDays(finalPrepDays);

            return new EventPhaseSchedule
            {
                AcceptedDate = acceptedDate,
                EventDate = eventDate,
                BookingEndDate = bookingEndDate,
                PrePlanningEndDate = prePlanningEndDate,
                ActivePlanningEndDate = activePlanningEndDate,
                FinalPrepEndDate = finalPrepEndDate,
                BookingDays = bookingDays,
                PrePlanningDays = prePlanningDays,
                ActivePlanningDays = activePlanningDays,
                FinalPrepDays = finalPrepDays
            };
        }

        /// <summary>
        /// Determines the current phase of an event based on the current date.
        /// </summary>
        /// <param name="currentDate">The current in-game date.</param>
        /// <param name="schedule">The event's phase schedule.</param>
        /// <returns>Information about the current phase.</returns>
        public static EventPhaseInfo GetCurrentPhase(GameDate currentDate, EventPhaseSchedule schedule)
        {
            int daysUntilEvent = GameDate.DaysBetween(currentDate, schedule.EventDate);

            // Results phase (after event)
            if (currentDate > schedule.EventDate)
            {
                return new EventPhaseInfo
                {
                    Phase = EventPhase.Results,
                    StressWeight = GetStressWeight(EventPhase.Results),
                    DaysRemainingInPhase = 0,
                    TotalDaysInPhase = 0,
                    DaysUntilEvent = daysUntilEvent
                };
            }

            // Execution Day
            if (currentDate == schedule.EventDate)
            {
                return new EventPhaseInfo
                {
                    Phase = EventPhase.ExecutionDay,
                    StressWeight = GetStressWeight(EventPhase.ExecutionDay),
                    DaysRemainingInPhase = 0,
                    TotalDaysInPhase = 1,
                    DaysUntilEvent = 0
                };
            }

            // Final Prep
            if (currentDate > schedule.ActivePlanningEndDate)
            {
                int daysInPhase = GameDate.DaysBetween(schedule.ActivePlanningEndDate, schedule.EventDate);
                int daysRemaining = GameDate.DaysBetween(currentDate, schedule.EventDate);
                return new EventPhaseInfo
                {
                    Phase = EventPhase.FinalPrep,
                    StressWeight = GetStressWeight(EventPhase.FinalPrep),
                    DaysRemainingInPhase = daysRemaining,
                    TotalDaysInPhase = daysInPhase,
                    DaysUntilEvent = daysUntilEvent
                };
            }

            // Active Planning
            if (currentDate > schedule.PrePlanningEndDate)
            {
                int daysInPhase = GameDate.DaysBetween(schedule.PrePlanningEndDate, schedule.ActivePlanningEndDate);
                int daysRemaining = GameDate.DaysBetween(currentDate, schedule.ActivePlanningEndDate);
                return new EventPhaseInfo
                {
                    Phase = EventPhase.ActivePlanning,
                    StressWeight = GetStressWeight(EventPhase.ActivePlanning),
                    DaysRemainingInPhase = daysRemaining,
                    TotalDaysInPhase = daysInPhase,
                    DaysUntilEvent = daysUntilEvent
                };
            }

            // Pre-Planning
            if (currentDate > schedule.BookingEndDate)
            {
                int daysInPhase = GameDate.DaysBetween(schedule.BookingEndDate, schedule.PrePlanningEndDate);
                int daysRemaining = GameDate.DaysBetween(currentDate, schedule.PrePlanningEndDate);
                return new EventPhaseInfo
                {
                    Phase = EventPhase.PrePlanning,
                    StressWeight = GetStressWeight(EventPhase.PrePlanning),
                    DaysRemainingInPhase = daysRemaining,
                    TotalDaysInPhase = daysInPhase,
                    DaysUntilEvent = daysUntilEvent
                };
            }

            // Booking
            int bookingDaysInPhase = GameDate.DaysBetween(schedule.AcceptedDate, schedule.BookingEndDate);
            int bookingDaysRemaining = GameDate.DaysBetween(currentDate, schedule.BookingEndDate);
            return new EventPhaseInfo
            {
                Phase = EventPhase.Booking,
                StressWeight = GetStressWeight(EventPhase.Booking),
                DaysRemainingInPhase = bookingDaysRemaining,
                TotalDaysInPhase = bookingDaysInPhase,
                DaysUntilEvent = daysUntilEvent
            };
        }

        /// <summary>
        /// Calculates the total stress penalty for overlapping high-stress events.
        /// Per R11.7: 10% additional penalty per overlapping event in Final Prep or Execution Day.
        /// </summary>
        /// <param name="highStressEventCount">Number of events in Final Prep or Execution Day.</param>
        /// <returns>The additional stress penalty multiplier (0.0 for 0-1 events, 0.1 per additional event).</returns>
        public static float CalculateOverlappingStressPenalty(int highStressEventCount)
        {
            if (highStressEventCount <= 1)
                return 0f;
            
            // 10% per overlapping event beyond the first
            return (highStressEventCount - 1) * 0.10f;
        }

        /// <summary>
        /// Calculates the effective workload penalty considering phase stress weights.
        /// Per R11.6: Base penalty is multiplied by the stress weight of each event's current phase.
        /// </summary>
        /// <param name="basePenalty">The base workload penalty percentage.</param>
        /// <param name="phase">The current event phase.</param>
        /// <returns>The effective penalty after applying stress weight.</returns>
        public static float CalculateEffectiveWorkloadPenalty(float basePenalty, EventPhase phase)
        {
            return basePenalty * GetStressWeight(phase);
        }
    }
}
