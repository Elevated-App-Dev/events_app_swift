using System;
using NUnit.Framework;
using EventPlannerSim.Core;
using EventPlannerSim.Systems;

namespace EventPlannerSim.Tests.EditMode
{
    /// <summary>
    /// Property-based tests for TimeSystem.
    /// Feature: event-planner-simulator, Property 12: Time Passage by Stage
    /// Validates: Requirements R11
    /// </summary>
    [TestFixture]
    public class TimeSystemPropertyTests
    {
        private Random _random;

        [SetUp]
        public void Setup()
        {
            _random = new Random(42); // Fixed seed for reproducibility
        }

        /// <summary>
        /// Property 12: Time Passage by Stage
        /// For any stage (1-5), GetTimeRate should return the correct real minutes per game day.
        /// Stage 1 = 3 min/day, Stage 2 = 2.5 min/day, Stage 3 = 2 min/day,
        /// Stage 4 = 1.5 min/day, Stage 5 = 1 min/day.
        /// **Validates: Requirements R11.2**
        /// </summary>
        [Test]
        public void TimeSystem_GetTimeRate_ReturnsCorrectRateByStage()
        {
            var timeSystem = new TimeSystemImpl();
            
            // Expected rates from R11.2
            float[] expectedRates = { 3.0f, 2.5f, 2.0f, 1.5f, 1.0f };
            
            for (int stage = 1; stage <= 5; stage++)
            {
                float actualRate = timeSystem.GetTimeRate(stage);
                float expectedRate = expectedRates[stage - 1];
                
                Assert.AreEqual(expectedRate, actualRate, 0.001f,
                    $"Time rate for Stage {stage} should be {expectedRate} min/day, got {actualRate}");
            }
        }

        /// <summary>
        /// Property 12a: Time Passage Accumulation
        /// For any stage and real-time duration, advancing time should correctly
        /// accumulate days based on the stage's time rate.
        /// **Validates: Requirements R11.1, R11.2**
        /// </summary>
        [Test]
        public void TimeSystem_AdvanceTime_AccumulatesDaysCorrectly()
        {
            // Run 100 iterations as per testing strategy
            for (int i = 0; i < 100; i++)
            {
                // Generate random stage (1-5)
                int stage = _random.Next(1, 6);
                
                // Generate random starting date
                var startDate = new GameDate(
                    _random.Next(1, 31),
                    _random.Next(1, 13),
                    _random.Next(1, 10)
                );
                
                var timeSystem = new TimeSystemImpl(startDate);
                
                // Get time rate for this stage
                float minutesPerDay = timeSystem.GetTimeRate(stage);
                float secondsPerDay = minutesPerDay * 60f;
                
                // Generate random number of days to advance (1-10)
                int daysToAdvance = _random.Next(1, 11);
                float secondsToAdvance = daysToAdvance * secondsPerDay;
                
                // Advance time
                timeSystem.AdvanceTime(secondsToAdvance, stage);
                
                // Verify the date advanced correctly
                var expectedDate = startDate.AddDays(daysToAdvance);
                Assert.AreEqual(expectedDate, timeSystem.CurrentDate,
                    $"After advancing {daysToAdvance} days at Stage {stage}, " +
                    $"expected {expectedDate}, got {timeSystem.CurrentDate}");
            }
        }

        /// <summary>
        /// Property 12b: Higher Stages Have Faster Time
        /// For any two stages where stage1 < stage2, the time rate for stage2
        /// should be less than or equal to stage1 (faster time passage).
        /// **Validates: Requirements R11.2**
        /// </summary>
        [Test]
        public void TimeSystem_HigherStages_HaveFasterTimePassage()
        {
            var timeSystem = new TimeSystemImpl();
            
            for (int i = 0; i < 100; i++)
            {
                // Generate two different stages
                int stage1 = _random.Next(1, 5);
                int stage2 = _random.Next(stage1 + 1, 6);
                
                float rate1 = timeSystem.GetTimeRate(stage1);
                float rate2 = timeSystem.GetTimeRate(stage2);
                
                // Higher stage should have lower rate (faster time)
                Assert.Less(rate2, rate1,
                    $"Stage {stage2} rate ({rate2}) should be less than Stage {stage1} rate ({rate1})");
            }
        }

        /// <summary>
        /// Property 12c: Event Scheduling by Complexity
        /// For any complexity level, scheduled events should fall within the correct day range.
        /// Low = 3-7 days, Medium = 7-14 days, High = 14-21 days, Very High = 21-30 days.
        /// **Validates: Requirements R11.3**
        /// </summary>
        [Test]
        public void TimeSystem_ScheduleEvent_ReturnsDateWithinComplexityRange()
        {
            // Expected ranges from R11.3
            var expectedRanges = new (int min, int max)[]
            {
                (3, 7),     // Low
                (7, 14),    // Medium
                (14, 21),   // High
                (21, 30)    // VeryHigh
            };
            
            for (int i = 0; i < 100; i++)
            {
                // Use different seed for each iteration to get variety
                var timeSystem = new TimeSystemImpl(new GameDate(1, 1, 1), i);
                
                // Generate random starting date
                var currentDate = new GameDate(
                    _random.Next(1, 31),
                    _random.Next(1, 13),
                    _random.Next(1, 10)
                );
                
                // Test each complexity level
                foreach (EventComplexity complexity in Enum.GetValues(typeof(EventComplexity)))
                {
                    var scheduledDate = timeSystem.ScheduleEvent(complexity, currentDate);
                    int daysUntilEvent = GameDate.DaysBetween(currentDate, scheduledDate);
                    
                    var (minDays, maxDays) = expectedRanges[(int)complexity];
                    
                    Assert.GreaterOrEqual(daysUntilEvent, minDays,
                        $"Event with {complexity} complexity scheduled too soon: {daysUntilEvent} days (min: {minDays})");
                    Assert.LessOrEqual(daysUntilEvent, maxDays,
                        $"Event with {complexity} complexity scheduled too late: {daysUntilEvent} days (max: {maxDays})");
                }
            }
        }

        /// <summary>
        /// Property 12d: Paused Time Does Not Advance
        /// When the time system is paused, advancing time should not change the current date.
        /// **Validates: Requirements R11.8**
        /// </summary>
        [Test]
        public void TimeSystem_WhenPaused_DoesNotAdvanceTime()
        {
            for (int i = 0; i < 100; i++)
            {
                var startDate = new GameDate(
                    _random.Next(1, 31),
                    _random.Next(1, 13),
                    _random.Next(1, 10)
                );
                
                var timeSystem = new TimeSystemImpl(startDate);
                timeSystem.Pause();
                
                int stage = _random.Next(1, 6);
                float secondsToAdvance = _random.Next(1, 10000);
                
                timeSystem.AdvanceTime(secondsToAdvance, stage);
                
                Assert.AreEqual(startDate, timeSystem.CurrentDate,
                    $"Date should not change when paused. Expected {startDate}, got {timeSystem.CurrentDate}");
                Assert.IsTrue(timeSystem.IsPaused, "Time system should remain paused");
            }
        }

        /// <summary>
        /// Property 12e: Resume After Pause Allows Time Advancement
        /// After resuming from pause, time should advance normally.
        /// **Validates: Requirements R11.8**
        /// </summary>
        [Test]
        public void TimeSystem_AfterResume_AdvancesTimeNormally()
        {
            for (int i = 0; i < 100; i++)
            {
                var startDate = new GameDate(
                    _random.Next(1, 31),
                    _random.Next(1, 13),
                    _random.Next(1, 10)
                );
                
                var timeSystem = new TimeSystemImpl(startDate);
                int stage = _random.Next(1, 6);
                
                // Pause and try to advance (should not change)
                timeSystem.Pause();
                timeSystem.AdvanceTime(1000f, stage);
                Assert.AreEqual(startDate, timeSystem.CurrentDate);
                
                // Resume and advance
                timeSystem.Resume();
                Assert.IsFalse(timeSystem.IsPaused, "Time system should not be paused after resume");
                
                float minutesPerDay = timeSystem.GetTimeRate(stage);
                float secondsPerDay = minutesPerDay * 60f;
                int daysToAdvance = _random.Next(1, 5);
                
                timeSystem.AdvanceTime(daysToAdvance * secondsPerDay, stage);
                
                var expectedDate = startDate.AddDays(daysToAdvance);
                Assert.AreEqual(expectedDate, timeSystem.CurrentDate,
                    $"After resume, date should advance. Expected {expectedDate}, got {timeSystem.CurrentDate}");
            }
        }

        /// <summary>
        /// Property 12f: Skip To Date Advances Correctly
        /// Skipping to a future date should set the current date to the target date.
        /// **Validates: Requirements R11.10**
        /// </summary>
        [Test]
        public void TimeSystem_SkipToDate_SetsCorrectDate()
        {
            for (int i = 0; i < 100; i++)
            {
                var startDate = new GameDate(
                    _random.Next(1, 31),
                    _random.Next(1, 13),
                    _random.Next(1, 10)
                );
                
                var timeSystem = new TimeSystemImpl(startDate);
                
                // Generate a future date
                int daysToSkip = _random.Next(1, 100);
                var targetDate = startDate.AddDays(daysToSkip);
                
                timeSystem.SkipToDate(targetDate);
                
                Assert.AreEqual(targetDate, timeSystem.CurrentDate,
                    $"After skip, date should be {targetDate}, got {timeSystem.CurrentDate}");
            }
        }

        /// <summary>
        /// Property 12g: Skip To Past Date Does Not Change Date
        /// Attempting to skip to a past date should not change the current date.
        /// **Validates: Requirements R11.10**
        /// </summary>
        [Test]
        public void TimeSystem_SkipToPastDate_DoesNotChangeDate()
        {
            for (int i = 0; i < 100; i++)
            {
                var startDate = new GameDate(
                    _random.Next(1, 31),
                    _random.Next(1, 13),
                    _random.Next(2, 10) // Start from year 2+ to allow past dates
                );
                
                var timeSystem = new TimeSystemImpl(startDate);
                
                // Generate a past date
                int daysBack = _random.Next(1, 100);
                var pastDate = GameDate.FromTotalDays(Math.Max(1, startDate.TotalDays - daysBack));
                
                timeSystem.SkipToDate(pastDate);
                
                Assert.AreEqual(startDate, timeSystem.CurrentDate,
                    $"Skip to past date should not change current date. Expected {startDate}, got {timeSystem.CurrentDate}");
            }
        }

        /// <summary>
        /// Property 12h: Stage Clamping
        /// Invalid stage values should be clamped to valid range (1-5).
        /// **Validates: Requirements R11.2**
        /// </summary>
        [Test]
        public void TimeSystem_InvalidStage_ClampedToValidRange()
        {
            var timeSystem = new TimeSystemImpl();
            
            // Test stage 0 (should clamp to 1)
            Assert.AreEqual(3.0f, timeSystem.GetTimeRate(0), 0.001f,
                "Stage 0 should clamp to Stage 1 rate (3.0)");
            
            // Test negative stage (should clamp to 1)
            Assert.AreEqual(3.0f, timeSystem.GetTimeRate(-5), 0.001f,
                "Negative stage should clamp to Stage 1 rate (3.0)");
            
            // Test stage 6 (should clamp to 5)
            Assert.AreEqual(1.0f, timeSystem.GetTimeRate(6), 0.001f,
                "Stage 6 should clamp to Stage 5 rate (1.0)");
            
            // Test stage 100 (should clamp to 5)
            Assert.AreEqual(1.0f, timeSystem.GetTimeRate(100), 0.001f,
                "Stage 100 should clamp to Stage 5 rate (1.0)");
        }

        /// <summary>
        /// Property 12i: Partial Day Accumulation
        /// Time that doesn't complete a full day should accumulate for the next advance.
        /// **Validates: Requirements R11.1**
        /// </summary>
        [Test]
        public void TimeSystem_PartialDays_AccumulateCorrectly()
        {
            for (int i = 0; i < 100; i++)
            {
                var startDate = new GameDate(1, 1, 1);
                var timeSystem = new TimeSystemImpl(startDate);
                
                int stage = _random.Next(1, 6);
                float minutesPerDay = timeSystem.GetTimeRate(stage);
                float secondsPerDay = minutesPerDay * 60f;
                
                // Advance by half a day twice - should result in 1 full day
                float halfDaySeconds = secondsPerDay / 2f;
                
                timeSystem.AdvanceTime(halfDaySeconds, stage);
                Assert.AreEqual(startDate, timeSystem.CurrentDate,
                    "Half day should not advance the date yet");
                
                timeSystem.AdvanceTime(halfDaySeconds, stage);
                var expectedDate = startDate.AddDays(1);
                Assert.AreEqual(expectedDate, timeSystem.CurrentDate,
                    $"Two half-days should advance by 1 day. Expected {expectedDate}, got {timeSystem.CurrentDate}");
            }
        }
    }
}
