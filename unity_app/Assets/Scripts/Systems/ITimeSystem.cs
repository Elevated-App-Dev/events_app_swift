using EventPlannerSim.Core;

namespace EventPlannerSim.Systems
{
    /// <summary>
    /// Manages in-game time passage and event scheduling.
    /// Pure logic interface - no Unity dependencies.
    /// </summary>
    public interface ITimeSystem
    {
        /// <summary>
        /// Current in-game date.
        /// </summary>
        GameDate CurrentDate { get; }

        /// <summary>
        /// Indicates whether time is currently paused (e.g., app backgrounded).
        /// </summary>
        bool IsPaused { get; }

        /// <summary>
        /// Advance time by specified real-time seconds.
        /// Time passage rate depends on the current business stage.
        /// </summary>
        /// <param name="realTimeSeconds">Real-world seconds elapsed.</param>
        /// <param name="stage">Current business stage (1-5).</param>
        void AdvanceTime(float realTimeSeconds, int stage);

        /// <summary>
        /// Get time passage rate for a given stage.
        /// Returns real minutes per game day.
        /// Stage 1 = 3 min/day, Stage 2 = 2.5 min/day, Stage 3 = 2 min/day,
        /// Stage 4 = 1.5 min/day, Stage 5 = 1 min/day.
        /// </summary>
        /// <param name="stage">Business stage (1-5).</param>
        /// <returns>Real minutes per in-game day.</returns>
        float GetTimeRate(int stage);

        /// <summary>
        /// Schedule an event for a future date based on complexity.
        /// Low = 3-7 days, Medium = 7-14 days, High = 14-21 days, Very High = 21-30 days.
        /// </summary>
        /// <param name="complexity">Event complexity level.</param>
        /// <param name="currentDate">The current in-game date.</param>
        /// <returns>The scheduled event date.</returns>
        GameDate ScheduleEvent(EventComplexity complexity, GameDate currentDate);

        /// <summary>
        /// Pause time progression (e.g., when app is backgrounded).
        /// </summary>
        void Pause();

        /// <summary>
        /// Resume time progression (e.g., when app is foregrounded).
        /// </summary>
        void Resume();

        /// <summary>
        /// Skip time to a specific date (for "Skip to Next Inquiry" feature).
        /// </summary>
        /// <param name="targetDate">The date to skip to.</param>
        void SkipToDate(GameDate targetDate);

        /// <summary>
        /// Set the current date directly (used for save/load).
        /// </summary>
        /// <param name="date">The date to set.</param>
        void SetCurrentDate(GameDate date);
    }
}
