using System;
using EventPlannerSim.Core;

namespace EventPlannerSim.Systems
{
    /// <summary>
    /// Implementation of the time system managing in-game time passage and event scheduling.
    /// Pure C# logic - no Unity dependencies for testability.
    /// </summary>
    public class TimeSystemImpl : ITimeSystem
    {
        private GameDate _currentDate;
        private bool _isPaused;
        private float _accumulatedSeconds;
        private readonly Random _random;

        // Time rates in real minutes per game day (from R11.2)
        private static readonly float[] TimeRatesByStage = new float[]
        {
            3.0f,   // Stage 1: 1 day per 3 real minutes
            2.5f,   // Stage 2: 1 day per 2.5 real minutes
            2.0f,   // Stage 3: 1 day per 2 real minutes
            1.5f,   // Stage 4: 1 day per 1.5 real minutes
            1.0f    // Stage 5: 1 day per 1 real minute
        };

        // Event scheduling ranges in days (from R11.3)
        private static readonly (int min, int max)[] SchedulingRangesByComplexity = new (int, int)[]
        {
            (3, 7),     // Low complexity: 3-7 days
            (7, 14),    // Medium complexity: 7-14 days
            (14, 21),   // High complexity: 14-21 days
            (21, 30)    // Very High complexity: 21-30 days
        };

        /// <summary>
        /// Creates a new TimeSystemImpl with default starting date (Day 1, Month 1, Year 1).
        /// </summary>
        public TimeSystemImpl() : this(new GameDate(1, 1, 1))
        {
        }

        /// <summary>
        /// Creates a new TimeSystemImpl with a specific starting date.
        /// </summary>
        /// <param name="startDate">The initial in-game date.</param>
        public TimeSystemImpl(GameDate startDate) : this(startDate, null)
        {
        }

        /// <summary>
        /// Creates a new TimeSystemImpl with a specific starting date and random seed.
        /// </summary>
        /// <param name="startDate">The initial in-game date.</param>
        /// <param name="seed">Optional random seed for deterministic scheduling.</param>
        public TimeSystemImpl(GameDate startDate, int? seed)
        {
            _currentDate = startDate;
            _isPaused = false;
            _accumulatedSeconds = 0f;
            _random = seed.HasValue ? new Random(seed.Value) : new Random();
        }

        /// <inheritdoc/>
        public GameDate CurrentDate => _currentDate;

        /// <inheritdoc/>
        public bool IsPaused => _isPaused;

        /// <inheritdoc/>
        public void AdvanceTime(float realTimeSeconds, int stage)
        {
            if (_isPaused || realTimeSeconds <= 0)
                return;

            // Clamp stage to valid range (1-5)
            int clampedStage = Math.Max(1, Math.Min(5, stage));
            
            // Get time rate (real minutes per game day)
            float minutesPerDay = GetTimeRate(clampedStage);
            float secondsPerDay = minutesPerDay * 60f;

            // Accumulate time
            _accumulatedSeconds += realTimeSeconds;

            // Calculate how many full days have passed
            int daysToAdvance = (int)(_accumulatedSeconds / secondsPerDay);
            
            if (daysToAdvance > 0)
            {
                _currentDate = _currentDate.AddDays(daysToAdvance);
                _accumulatedSeconds -= daysToAdvance * secondsPerDay;
            }
        }

        /// <inheritdoc/>
        public float GetTimeRate(int stage)
        {
            // Clamp stage to valid range (1-5) and convert to 0-based index
            int index = Math.Max(0, Math.Min(4, stage - 1));
            return TimeRatesByStage[index];
        }

        /// <inheritdoc/>
        public GameDate ScheduleEvent(EventComplexity complexity, GameDate currentDate)
        {
            // Get scheduling range for complexity
            int complexityIndex = (int)complexity;
            complexityIndex = Math.Max(0, Math.Min(3, complexityIndex));
            
            var (minDays, maxDays) = SchedulingRangesByComplexity[complexityIndex];
            
            // Generate random days within range (inclusive)
            int daysUntilEvent = _random.Next(minDays, maxDays + 1);
            
            return currentDate.AddDays(daysUntilEvent);
        }

        /// <inheritdoc/>
        public void Pause()
        {
            _isPaused = true;
        }

        /// <inheritdoc/>
        public void Resume()
        {
            _isPaused = false;
        }

        /// <inheritdoc/>
        public void SkipToDate(GameDate targetDate)
        {
            if (targetDate.TotalDays > _currentDate.TotalDays)
            {
                _currentDate = targetDate;
                _accumulatedSeconds = 0f; // Reset accumulated time on skip
            }
        }

        /// <inheritdoc/>
        public void SetCurrentDate(GameDate date)
        {
            _currentDate = date;
            _accumulatedSeconds = 0f;
        }

        /// <summary>
        /// Gets the accumulated seconds that haven't yet converted to a full day.
        /// Useful for testing and save/load.
        /// </summary>
        public float AccumulatedSeconds => _accumulatedSeconds;

        /// <summary>
        /// Sets the accumulated seconds directly (used for save/load).
        /// </summary>
        /// <param name="seconds">The accumulated seconds to set.</param>
        public void SetAccumulatedSeconds(float seconds)
        {
            _accumulatedSeconds = Math.Max(0, seconds);
        }
    }
}
