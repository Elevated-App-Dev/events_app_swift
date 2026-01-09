using System;
using System.Collections.Generic;
using NUnit.Framework;
using EventPlannerSim.Core;
using EventPlannerSim.Data;
using EventPlannerSim.Systems;

namespace EventPlannerSim.Tests.EditMode
{
    /// <summary>
    /// Defensive edge case tests for SatisfactionCalculator, TimeSystem, and SaveSystem.
    /// Tests invalid inputs and boundary conditions.
    /// Feature: event-planner-simulator
    /// Task: 33.4 Review other systems for similar defensive tests
    /// </summary>
    [TestFixture]
    public class SatisfactionCalculatorDefensiveTests
    {
        private SatisfactionCalculatorImpl _calculator;

        [SetUp]
        public void Setup()
        {
            _calculator = new SatisfactionCalculatorImpl();
        }

        #region Null Input Handling

        /// <summary>
        /// Test that null EventData throws ArgumentNullException.
        /// </summary>
        [Test]
        public void Calculate_NullEventData_ThrowsArgumentNullException()
        {
            var client = new ClientData { clientName = "Test", personality = ClientPersonality.EasyGoing };

            Assert.Throws<ArgumentNullException>(() => _calculator.Calculate(null, client),
                "Calculate should throw ArgumentNullException for null EventData");
        }

        /// <summary>
        /// Test that null ClientData throws ArgumentNullException.
        /// </summary>
        [Test]
        public void Calculate_NullClientData_ThrowsArgumentNullException()
        {
            var eventData = new EventData { id = "test", results = new EventResults() };

            Assert.Throws<ArgumentNullException>(() => _calculator.Calculate(eventData, null),
                "Calculate should throw ArgumentNullException for null ClientData");
        }

        /// <summary>
        /// Test that null EventResults is handled gracefully.
        /// </summary>
        [Test]
        public void Calculate_NullEventResults_HandledGracefully()
        {
            var eventData = new EventData { id = "test", results = null };
            var client = new ClientData { clientName = "Test", personality = ClientPersonality.EasyGoing };

            var result = _calculator.Calculate(eventData, client);

            Assert.IsNotNull(result, "Calculate should return result even with null EventResults");
            Assert.AreEqual(0f, result.FinalSatisfaction,
                "Null EventResults should result in 0 satisfaction");
        }

        #endregion

        #region Category Score Boundary Tests

        /// <summary>
        /// Test that category scores > 100 are handled.
        /// </summary>
        [Test]
        public void Calculate_CategoryScoresOver100_HandledGracefully()
        {
            var eventData = new EventData
            {
                id = "test",
                results = new EventResults
                {
                    venueScore = 150f,
                    foodScore = 200f,
                    entertainmentScore = 100f,
                    decorationScore = 100f,
                    serviceScore = 100f,
                    expectationScore = 100f,
                    randomEventModifier = 1f
                }
            };
            var client = new ClientData { clientName = "Test", personality = ClientPersonality.EasyGoing };

            var result = _calculator.Calculate(eventData, client);

            // Final satisfaction should be clamped to 100
            Assert.LessOrEqual(result.FinalSatisfaction, 100f,
                "Final satisfaction should be clamped to 100 max");
        }

        /// <summary>
        /// Test that negative category scores are handled.
        /// </summary>
        [Test]
        public void Calculate_NegativeCategoryScores_HandledGracefully()
        {
            var eventData = new EventData
            {
                id = "test",
                results = new EventResults
                {
                    venueScore = -50f,
                    foodScore = -100f,
                    entertainmentScore = 0f,
                    decorationScore = 0f,
                    serviceScore = 0f,
                    expectationScore = 0f,
                    randomEventModifier = 1f
                }
            };
            var client = new ClientData { clientName = "Test", personality = ClientPersonality.EasyGoing };

            var result = _calculator.Calculate(eventData, client);

            // Final satisfaction should be clamped to 0
            Assert.GreaterOrEqual(result.FinalSatisfaction, 0f,
                "Final satisfaction should be clamped to 0 min");
        }

        /// <summary>
        /// Test that NaN category scores are handled.
        /// </summary>
        [Test]
        public void Calculate_NaNCategoryScores_HandledGracefully()
        {
            var eventData = new EventData
            {
                id = "test",
                results = new EventResults
                {
                    venueScore = float.NaN,
                    foodScore = 80f,
                    entertainmentScore = 80f,
                    decorationScore = 80f,
                    serviceScore = 80f,
                    expectationScore = 80f,
                    randomEventModifier = 1f
                }
            };
            var client = new ClientData { clientName = "Test", personality = ClientPersonality.EasyGoing };

            var result = _calculator.Calculate(eventData, client);

            // Should not throw, result may be NaN but should be handled
            Assert.IsNotNull(result, "Calculate should return result even with NaN scores");
        }

        #endregion

        #region Random Event Modifier Tests

        /// <summary>
        /// Test that zero random event modifier results in zero satisfaction.
        /// </summary>
        [Test]
        public void Calculate_ZeroRandomEventModifier_ResultsInZeroSatisfaction()
        {
            var eventData = new EventData
            {
                id = "test",
                results = new EventResults
                {
                    venueScore = 100f,
                    foodScore = 100f,
                    entertainmentScore = 100f,
                    decorationScore = 100f,
                    serviceScore = 100f,
                    expectationScore = 100f,
                    randomEventModifier = 0f
                }
            };
            var client = new ClientData { clientName = "Test", personality = ClientPersonality.EasyGoing };

            var result = _calculator.Calculate(eventData, client);

            Assert.AreEqual(0f, result.FinalSatisfaction,
                "Zero random event modifier should result in 0 satisfaction");
        }

        /// <summary>
        /// Test that negative random event modifier is handled.
        /// </summary>
        [Test]
        public void Calculate_NegativeRandomEventModifier_HandledGracefully()
        {
            var eventData = new EventData
            {
                id = "test",
                results = new EventResults
                {
                    venueScore = 100f,
                    foodScore = 100f,
                    entertainmentScore = 100f,
                    decorationScore = 100f,
                    serviceScore = 100f,
                    expectationScore = 100f,
                    randomEventModifier = -1f
                }
            };
            var client = new ClientData { clientName = "Test", personality = ClientPersonality.EasyGoing };

            var result = _calculator.Calculate(eventData, client);

            // Negative modifier would result in negative satisfaction, clamped to 0
            Assert.GreaterOrEqual(result.FinalSatisfaction, 0f,
                "Negative random event modifier should result in satisfaction clamped to 0");
        }

        /// <summary>
        /// Test that very large random event modifier is handled.
        /// </summary>
        [Test]
        public void Calculate_LargeRandomEventModifier_ClampsTo100()
        {
            var eventData = new EventData
            {
                id = "test",
                results = new EventResults
                {
                    venueScore = 100f,
                    foodScore = 100f,
                    entertainmentScore = 100f,
                    decorationScore = 100f,
                    serviceScore = 100f,
                    expectationScore = 100f,
                    randomEventModifier = 10f // 10x multiplier
                }
            };
            var client = new ClientData { clientName = "Test", personality = ClientPersonality.EasyGoing };

            var result = _calculator.Calculate(eventData, client);

            Assert.LessOrEqual(result.FinalSatisfaction, 100f,
                "Large random event modifier should still clamp satisfaction to 100");
        }

        #endregion

        #region Personality Threshold Tests

        /// <summary>
        /// Test that all personality types return valid thresholds.
        /// </summary>
        [Test]
        public void GetPersonalityThreshold_AllPersonalities_ReturnValidThresholds()
        {
            foreach (ClientPersonality personality in Enum.GetValues(typeof(ClientPersonality)))
            {
                float threshold = _calculator.GetPersonalityThreshold(personality);

                Assert.GreaterOrEqual(threshold, 0f,
                    $"Threshold for {personality} should be >= 0");
                Assert.LessOrEqual(threshold, 100f,
                    $"Threshold for {personality} should be <= 100");
            }
        }

        /// <summary>
        /// Test that invalid personality enum value returns default threshold.
        /// </summary>
        [Test]
        public void GetPersonalityThreshold_InvalidPersonality_ReturnsDefaultThreshold()
        {
            // Cast an invalid int to ClientPersonality
            var invalidPersonality = (ClientPersonality)999;

            float threshold = _calculator.GetPersonalityThreshold(invalidPersonality);

            Assert.AreEqual(50f, threshold,
                "Invalid personality should return default threshold (50)");
        }

        #endregion

        #region Overage Tolerance Tests

        /// <summary>
        /// Test that all personality types return valid overage tolerances.
        /// </summary>
        [Test]
        public void GetOverageTolerance_AllPersonalities_ReturnValidTolerances()
        {
            foreach (ClientPersonality personality in Enum.GetValues(typeof(ClientPersonality)))
            {
                float tolerance = _calculator.GetOverageTolerance(personality);

                Assert.GreaterOrEqual(tolerance, 0f,
                    $"Overage tolerance for {personality} should be >= 0");
                Assert.LessOrEqual(tolerance, 100f,
                    $"Overage tolerance for {personality} should be <= 100");
            }
        }

        /// <summary>
        /// Test that invalid personality enum value returns default tolerance.
        /// </summary>
        [Test]
        public void GetOverageTolerance_InvalidPersonality_ReturnsDefaultTolerance()
        {
            var invalidPersonality = (ClientPersonality)999;

            float tolerance = _calculator.GetOverageTolerance(invalidPersonality);

            Assert.AreEqual(0f, tolerance,
                "Invalid personality should return default tolerance (0)");
        }

        /// <summary>
        /// Test IsOverageWithinTolerance with negative overage.
        /// </summary>
        [Test]
        public void IsOverageWithinTolerance_NegativeOverage_ReturnsTrue()
        {
            // Negative overage means under budget, should always be within tolerance
            bool result = _calculator.IsOverageWithinTolerance(-10f, ClientPersonality.BudgetConscious);

            Assert.IsTrue(result,
                "Negative overage (under budget) should always be within tolerance");
        }

        #endregion

        #region CalculateCategoryScore Tests

        /// <summary>
        /// Test that null EventData returns 0 for category score.
        /// </summary>
        [Test]
        public void CalculateCategoryScore_NullEventData_ReturnsZero()
        {
            float score = _calculator.CalculateCategoryScore(null, BudgetCategory.Venue);

            Assert.AreEqual(0f, score,
                "Null EventData should return 0 for category score");
        }

        /// <summary>
        /// Test that null EventResults returns 0 for category score.
        /// </summary>
        [Test]
        public void CalculateCategoryScore_NullEventResults_ReturnsZero()
        {
            var eventData = new EventData { id = "test", results = null };

            float score = _calculator.CalculateCategoryScore(eventData, BudgetCategory.Venue);

            Assert.AreEqual(0f, score,
                "Null EventResults should return 0 for category score");
        }

        /// <summary>
        /// Test that invalid BudgetCategory returns default score.
        /// </summary>
        [Test]
        public void CalculateCategoryScore_InvalidCategory_ReturnsDefaultScore()
        {
            var eventData = new EventData
            {
                id = "test",
                results = new EventResults { venueScore = 80f }
            };
            var invalidCategory = (BudgetCategory)999;

            float score = _calculator.CalculateCategoryScore(eventData, invalidCategory);

            Assert.AreEqual(50f, score,
                "Invalid BudgetCategory should return default score (50)");
        }

        #endregion

        #region ClampSatisfaction Static Method Tests

        /// <summary>
        /// Test ClampSatisfaction with various edge cases.
        /// </summary>
        [Test]
        public void ClampSatisfaction_EdgeCases_ClampsCorrectly()
        {
            Assert.AreEqual(0f, SatisfactionCalculatorImpl.ClampSatisfaction(-100f),
                "Negative value should clamp to 0");
            Assert.AreEqual(0f, SatisfactionCalculatorImpl.ClampSatisfaction(float.MinValue),
                "MinValue should clamp to 0");
            Assert.AreEqual(100f, SatisfactionCalculatorImpl.ClampSatisfaction(200f),
                "Value > 100 should clamp to 100");
            Assert.AreEqual(100f, SatisfactionCalculatorImpl.ClampSatisfaction(float.MaxValue),
                "MaxValue should clamp to 100");
            Assert.AreEqual(50f, SatisfactionCalculatorImpl.ClampSatisfaction(50f),
                "Value in range should remain unchanged");
            Assert.AreEqual(0f, SatisfactionCalculatorImpl.ClampSatisfaction(0f),
                "Zero should remain zero");
            Assert.AreEqual(100f, SatisfactionCalculatorImpl.ClampSatisfaction(100f),
                "100 should remain 100");
        }

        #endregion
    }

    /// <summary>
    /// Defensive edge case tests for TimeSystem.
    /// </summary>
    [TestFixture]
    public class TimeSystemDefensiveTests
    {
        private TimeSystemImpl _timeSystem;

        [SetUp]
        public void Setup()
        {
            _timeSystem = new TimeSystemImpl(new GameDate(1, 1, 1), 42);
        }

        #region AdvanceTime Edge Cases

        /// <summary>
        /// Test that negative time does not advance.
        /// </summary>
        [Test]
        public void AdvanceTime_NegativeTime_DoesNotAdvance()
        {
            var initialDate = _timeSystem.CurrentDate;

            _timeSystem.AdvanceTime(-100f, 1);

            Assert.AreEqual(initialDate, _timeSystem.CurrentDate,
                "Negative time should not advance the date");
        }

        /// <summary>
        /// Test that zero time does not advance.
        /// </summary>
        [Test]
        public void AdvanceTime_ZeroTime_DoesNotAdvance()
        {
            var initialDate = _timeSystem.CurrentDate;

            _timeSystem.AdvanceTime(0f, 1);

            Assert.AreEqual(initialDate, _timeSystem.CurrentDate,
                "Zero time should not advance the date");
        }

        /// <summary>
        /// Test that time does not advance when paused.
        /// </summary>
        [Test]
        public void AdvanceTime_WhenPaused_DoesNotAdvance()
        {
            var initialDate = _timeSystem.CurrentDate;
            _timeSystem.Pause();

            _timeSystem.AdvanceTime(1000f, 1);

            Assert.AreEqual(initialDate, _timeSystem.CurrentDate,
                "Time should not advance when paused");
        }

        /// <summary>
        /// Test that invalid stage (0) is clamped to stage 1.
        /// </summary>
        [Test]
        public void AdvanceTime_Stage0_ClampedToStage1()
        {
            float stage1Rate = _timeSystem.GetTimeRate(1);
            float stage0Rate = _timeSystem.GetTimeRate(0);

            Assert.AreEqual(stage1Rate, stage0Rate,
                "Stage 0 should be clamped to stage 1 rate");
        }

        /// <summary>
        /// Test that invalid stage (negative) is clamped to stage 1.
        /// </summary>
        [Test]
        public void AdvanceTime_NegativeStage_ClampedToStage1()
        {
            float stage1Rate = _timeSystem.GetTimeRate(1);
            float negativeStageRate = _timeSystem.GetTimeRate(-5);

            Assert.AreEqual(stage1Rate, negativeStageRate,
                "Negative stage should be clamped to stage 1 rate");
        }

        /// <summary>
        /// Test that invalid stage (> 5) is clamped to stage 5.
        /// </summary>
        [Test]
        public void AdvanceTime_StageOver5_ClampedToStage5()
        {
            float stage5Rate = _timeSystem.GetTimeRate(5);
            float stage10Rate = _timeSystem.GetTimeRate(10);

            Assert.AreEqual(stage5Rate, stage10Rate,
                "Stage > 5 should be clamped to stage 5 rate");
        }

        /// <summary>
        /// Test that very large time values are handled.
        /// </summary>
        [Test]
        public void AdvanceTime_VeryLargeTime_HandledGracefully()
        {
            // Advance by a very large amount (1 year of real seconds)
            _timeSystem.AdvanceTime(365 * 24 * 60 * 60f, 1);

            // Should not throw and date should have advanced significantly
            Assert.Greater(_timeSystem.CurrentDate.TotalDays, 1,
                "Very large time should advance the date");
        }

        #endregion

        #region GetTimeRate Edge Cases

        /// <summary>
        /// Test that all valid stages return positive time rates.
        /// </summary>
        [Test]
        public void GetTimeRate_AllValidStages_ReturnPositiveRates()
        {
            for (int stage = 1; stage <= 5; stage++)
            {
                float rate = _timeSystem.GetTimeRate(stage);

                Assert.Greater(rate, 0f,
                    $"Stage {stage} should have positive time rate");
            }
        }

        /// <summary>
        /// Test that higher stages have faster time rates.
        /// </summary>
        [Test]
        public void GetTimeRate_HigherStages_HaveFasterRates()
        {
            for (int stage = 1; stage < 5; stage++)
            {
                float currentRate = _timeSystem.GetTimeRate(stage);
                float nextRate = _timeSystem.GetTimeRate(stage + 1);

                Assert.Less(nextRate, currentRate,
                    $"Stage {stage + 1} should have faster rate (lower minutes per day) than stage {stage}");
            }
        }

        #endregion

        #region ScheduleEvent Edge Cases

        /// <summary>
        /// Test that invalid complexity is clamped.
        /// </summary>
        [Test]
        public void ScheduleEvent_InvalidComplexity_ClampedToValid()
        {
            var currentDate = new GameDate(1, 1, 1);
            var invalidComplexity = (EventComplexity)999;

            var scheduledDate = _timeSystem.ScheduleEvent(invalidComplexity, currentDate);

            // Should not throw and should return a valid future date
            Assert.Greater(scheduledDate.TotalDays, currentDate.TotalDays,
                "Invalid complexity should still schedule a future date");
        }

        /// <summary>
        /// Test that all valid complexities schedule future dates.
        /// </summary>
        [Test]
        public void ScheduleEvent_AllComplexities_ScheduleFutureDates()
        {
            var currentDate = new GameDate(1, 1, 1);

            foreach (EventComplexity complexity in Enum.GetValues(typeof(EventComplexity)))
            {
                var scheduledDate = _timeSystem.ScheduleEvent(complexity, currentDate);

                Assert.Greater(scheduledDate.TotalDays, currentDate.TotalDays,
                    $"Complexity {complexity} should schedule a future date");
            }
        }

        #endregion

        #region SkipToDate Edge Cases

        /// <summary>
        /// Test that skipping to past date does not change current date.
        /// </summary>
        [Test]
        public void SkipToDate_PastDate_DoesNotChange()
        {
            _timeSystem.SetCurrentDate(new GameDate(15, 6, 1));
            var currentDate = _timeSystem.CurrentDate;
            var pastDate = new GameDate(1, 1, 1);

            _timeSystem.SkipToDate(pastDate);

            Assert.AreEqual(currentDate, _timeSystem.CurrentDate,
                "Skipping to past date should not change current date");
        }

        /// <summary>
        /// Test that skipping to same date does not change current date.
        /// </summary>
        [Test]
        public void SkipToDate_SameDate_DoesNotChange()
        {
            var currentDate = _timeSystem.CurrentDate;

            _timeSystem.SkipToDate(currentDate);

            Assert.AreEqual(currentDate, _timeSystem.CurrentDate,
                "Skipping to same date should not change current date");
        }

        /// <summary>
        /// Test that skipping to future date works correctly.
        /// </summary>
        [Test]
        public void SkipToDate_FutureDate_UpdatesCorrectly()
        {
            var futureDate = new GameDate(15, 6, 2);

            _timeSystem.SkipToDate(futureDate);

            Assert.AreEqual(futureDate, _timeSystem.CurrentDate,
                "Skipping to future date should update current date");
            Assert.AreEqual(0f, _timeSystem.AccumulatedSeconds,
                "Skipping should reset accumulated seconds");
        }

        #endregion

        #region SetAccumulatedSeconds Edge Cases

        /// <summary>
        /// Test that negative accumulated seconds is clamped to 0.
        /// </summary>
        [Test]
        public void SetAccumulatedSeconds_Negative_ClampedToZero()
        {
            _timeSystem.SetAccumulatedSeconds(-100f);

            Assert.AreEqual(0f, _timeSystem.AccumulatedSeconds,
                "Negative accumulated seconds should be clamped to 0");
        }

        #endregion

        #region Pause/Resume Tests

        /// <summary>
        /// Test pause and resume functionality.
        /// </summary>
        [Test]
        public void PauseResume_TogglesCorrectly()
        {
            Assert.IsFalse(_timeSystem.IsPaused, "Should start unpaused");

            _timeSystem.Pause();
            Assert.IsTrue(_timeSystem.IsPaused, "Should be paused after Pause()");

            _timeSystem.Resume();
            Assert.IsFalse(_timeSystem.IsPaused, "Should be unpaused after Resume()");
        }

        /// <summary>
        /// Test that multiple pauses don't cause issues.
        /// </summary>
        [Test]
        public void Pause_MultipleTimes_HandledGracefully()
        {
            _timeSystem.Pause();
            _timeSystem.Pause();
            _timeSystem.Pause();

            Assert.IsTrue(_timeSystem.IsPaused, "Should still be paused after multiple Pause() calls");

            _timeSystem.Resume();
            Assert.IsFalse(_timeSystem.IsPaused, "Single Resume() should unpause");
        }

        #endregion
    }

    /// <summary>
    /// Defensive edge case tests for GameDate struct.
    /// </summary>
    [TestFixture]
    public class GameDateDefensiveTests
    {
        #region AddDays Edge Cases

        /// <summary>
        /// Test that adding negative days works correctly.
        /// </summary>
        [Test]
        public void AddDays_NegativeDays_SubtractsCorrectly()
        {
            var date = new GameDate(15, 6, 1);
            var result = date.AddDays(-10);

            Assert.AreEqual(5, result.day, "Day should be 5 after subtracting 10 days from day 15");
            Assert.AreEqual(6, result.month, "Month should remain 6");
        }

        /// <summary>
        /// Test that adding zero days returns same date.
        /// </summary>
        [Test]
        public void AddDays_ZeroDays_ReturnsSameDate()
        {
            var date = new GameDate(15, 6, 1);
            var result = date.AddDays(0);

            Assert.AreEqual(date, result, "Adding 0 days should return same date");
        }

        /// <summary>
        /// Test that adding large number of days handles year overflow.
        /// </summary>
        [Test]
        public void AddDays_LargeNumber_HandlesYearOverflow()
        {
            var date = new GameDate(1, 1, 1);
            var result = date.AddDays(360); // One full year

            Assert.AreEqual(1, result.day, "Day should be 1");
            Assert.AreEqual(1, result.month, "Month should be 1");
            Assert.AreEqual(2, result.year, "Year should be 2 after adding 360 days");
        }

        #endregion

        #region DaysBetween Edge Cases

        /// <summary>
        /// Test DaysBetween with same date returns 0.
        /// </summary>
        [Test]
        public void DaysBetween_SameDate_ReturnsZero()
        {
            var date = new GameDate(15, 6, 1);

            int days = GameDate.DaysBetween(date, date);

            Assert.AreEqual(0, days, "Days between same date should be 0");
        }

        /// <summary>
        /// Test DaysBetween with reversed dates returns negative.
        /// </summary>
        [Test]
        public void DaysBetween_ReversedDates_ReturnsNegative()
        {
            var earlier = new GameDate(1, 1, 1);
            var later = new GameDate(15, 1, 1);

            int days = GameDate.DaysBetween(later, earlier);

            Assert.Less(days, 0, "Days between reversed dates should be negative");
        }

        #endregion

        #region Comparison Edge Cases

        /// <summary>
        /// Test equality operators.
        /// </summary>
        [Test]
        public void Equality_SameDates_AreEqual()
        {
            var date1 = new GameDate(15, 6, 1);
            var date2 = new GameDate(15, 6, 1);

            Assert.IsTrue(date1 == date2, "Same dates should be equal with ==");
            Assert.IsFalse(date1 != date2, "Same dates should not be unequal with !=");
            Assert.IsTrue(date1.Equals(date2), "Same dates should be equal with Equals()");
        }

        /// <summary>
        /// Test comparison operators.
        /// </summary>
        [Test]
        public void Comparison_DifferentDates_CompareCorrectly()
        {
            var earlier = new GameDate(1, 1, 1);
            var later = new GameDate(2, 1, 1);

            Assert.IsTrue(earlier < later, "Earlier date should be less than later");
            Assert.IsTrue(later > earlier, "Later date should be greater than earlier");
            Assert.IsTrue(earlier <= later, "Earlier date should be less than or equal to later");
            Assert.IsTrue(later >= earlier, "Later date should be greater than or equal to earlier");
            Assert.IsTrue(earlier <= earlier, "Date should be less than or equal to itself");
            Assert.IsTrue(earlier >= earlier, "Date should be greater than or equal to itself");
        }

        #endregion

        #region TotalDays Edge Cases

        /// <summary>
        /// Test TotalDays calculation for day 1, month 1, year 1.
        /// </summary>
        [Test]
        public void TotalDays_FirstDay_ReturnsOne()
        {
            var date = new GameDate(1, 1, 1);

            Assert.AreEqual(1, date.TotalDays, "First day should have TotalDays = 1");
        }

        /// <summary>
        /// Test TotalDays calculation for end of first year.
        /// </summary>
        [Test]
        public void TotalDays_EndOfFirstYear_Returns360()
        {
            var date = new GameDate(30, 12, 1);

            Assert.AreEqual(360, date.TotalDays, "Last day of year 1 should have TotalDays = 360");
        }

        #endregion
    }
}
