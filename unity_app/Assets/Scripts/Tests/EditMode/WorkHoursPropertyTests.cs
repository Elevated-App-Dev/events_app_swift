using System;
using NUnit.Framework;
using EventPlannerSim.Data;

namespace EventPlannerSim.Tests.EditMode
{
    /// <summary>
    /// Property-based tests for WorkHoursData accumulation and reset.
    /// Feature: event-planner-simulator, Property 11: Work Hours Accumulation and Reset
    /// Validates: Requirements R10
    /// </summary>
    [TestFixture]
    public class WorkHoursPropertyTests
    {
        private Random _random;

        [SetUp]
        public void Setup()
        {
            _random = new Random(42); // Fixed seed for reproducibility
        }

        /// <summary>
        /// Property 11: Work Hours Accumulation and Reset - RemainingHours calculation
        /// RemainingHours == dailyCapacity + (overtimeUsedToday * 4) - hoursUsedToday
        /// **Validates: Requirements R10**
        /// </summary>
        [Test]
        public void WorkHours_RemainingHours_CalculatesCorrectly()
        {
            for (int i = 0; i < 100; i++)
            {
                int hoursUsed = _random.Next(0, 20);
                int overtimeUsed = _random.Next(0, 3); // 0, 1, or 2

                var workHours = new WorkHoursData
                {
                    hoursUsedToday = hoursUsed,
                    overtimeUsedToday = overtimeUsed
                };

                int expected = 8 + (overtimeUsed * 4) - hoursUsed;
                int actual = workHours.RemainingHours;

                Assert.AreEqual(expected, actual,
                    $"RemainingHours calculation failed. HoursUsed: {hoursUsed}, OvertimeUsed: {overtimeUsed}");
            }
        }

        /// <summary>
        /// Property 11: Work Hours Accumulation and Reset - ResetDaily clears all
        /// After ResetDaily, hoursUsedToday and overtimeUsedToday should be 0
        /// **Validates: Requirements R10**
        /// </summary>
        [Test]
        public void WorkHours_ResetDaily_ClearsAllUsage()
        {
            for (int i = 0; i < 100; i++)
            {
                var workHours = new WorkHoursData
                {
                    hoursUsedToday = _random.Next(0, 20),
                    overtimeUsedToday = _random.Next(0, 3)
                };

                workHours.ResetDaily();

                Assert.AreEqual(0, workHours.hoursUsedToday,
                    "hoursUsedToday should be 0 after ResetDaily");
                Assert.AreEqual(0, workHours.overtimeUsedToday,
                    "overtimeUsedToday should be 0 after ResetDaily");
                Assert.AreEqual(8, workHours.RemainingHours,
                    "RemainingHours should be 8 after ResetDaily");
            }
        }

        /// <summary>
        /// Property 11: Work Hours Accumulation and Reset - Overtime limited to 2
        /// CanUseOvertime should be false when overtimeUsedToday >= 2
        /// **Validates: Requirements R10**
        /// </summary>
        [Test]
        public void WorkHours_CanUseOvertime_LimitedToTwo()
        {
            for (int i = 0; i < 100; i++)
            {
                int overtimeUsed = _random.Next(0, 5);
                var workHours = new WorkHoursData
                {
                    overtimeUsedToday = overtimeUsed
                };

                bool expected = overtimeUsed < 2;
                bool actual = workHours.CanUseOvertime;

                Assert.AreEqual(expected, actual,
                    $"CanUseOvertime should be {expected} when overtimeUsedToday is {overtimeUsed}");
            }
        }

        /// <summary>
        /// Property 11: Work Hours Accumulation and Reset - UseHours deducts correctly
        /// After UseHours(n), hoursUsedToday increases by n
        /// **Validates: Requirements R10**
        /// </summary>
        [Test]
        public void WorkHours_UseHours_DeductsCorrectly()
        {
            for (int i = 0; i < 100; i++)
            {
                var workHours = new WorkHoursData();
                int hoursToUse = _random.Next(1, 9); // 1 to 8 hours

                int initialRemaining = workHours.RemainingHours;
                bool result = workHours.UseHours(hoursToUse);

                Assert.IsTrue(result, $"UseHours should succeed for {hoursToUse} hours");
                Assert.AreEqual(hoursToUse, workHours.hoursUsedToday,
                    $"hoursUsedToday should be {hoursToUse}");
                Assert.AreEqual(initialRemaining - hoursToUse, workHours.RemainingHours,
                    $"RemainingHours should decrease by {hoursToUse}");
            }
        }

        /// <summary>
        /// Property 11: Work Hours Accumulation and Reset - UseHours fails when insufficient
        /// UseHours should return false and not change state when hours > RemainingHours
        /// **Validates: Requirements R10**
        /// </summary>
        [Test]
        public void WorkHours_UseHours_FailsWhenInsufficient()
        {
            for (int i = 0; i < 100; i++)
            {
                var workHours = new WorkHoursData();
                int hoursToUse = workHours.RemainingHours + _random.Next(1, 10); // More than available

                int initialUsed = workHours.hoursUsedToday;
                bool result = workHours.UseHours(hoursToUse);

                Assert.IsFalse(result, $"UseHours should fail for {hoursToUse} hours when only {workHours.RemainingHours + hoursToUse - initialUsed} available");
                Assert.AreEqual(initialUsed, workHours.hoursUsedToday,
                    "hoursUsedToday should not change on failure");
            }
        }

        /// <summary>
        /// Property 11: Work Hours Accumulation and Reset - AddOvertime grants 4 hours
        /// After AddOvertime, RemainingHours increases by 4
        /// **Validates: Requirements R10**
        /// </summary>
        [Test]
        public void WorkHours_AddOvertime_Grants4Hours()
        {
            for (int i = 0; i < 100; i++)
            {
                var workHours = new WorkHoursData();
                int initialRemaining = workHours.RemainingHours;

                bool result = workHours.AddOvertime();

                Assert.IsTrue(result, "First AddOvertime should succeed");
                Assert.AreEqual(initialRemaining + 4, workHours.RemainingHours,
                    "RemainingHours should increase by 4 after AddOvertime");
            }
        }

        /// <summary>
        /// Property 11: Work Hours Accumulation and Reset - AddOvertime fails after 2
        /// AddOvertime should return false when overtimeUsedToday >= 2
        /// **Validates: Requirements R10**
        /// </summary>
        [Test]
        public void WorkHours_AddOvertime_FailsAfterTwo()
        {
            for (int i = 0; i < 100; i++)
            {
                var workHours = new WorkHoursData();

                // Use both overtime slots
                Assert.IsTrue(workHours.AddOvertime(), "First overtime should succeed");
                Assert.IsTrue(workHours.AddOvertime(), "Second overtime should succeed");

                int remainingBefore = workHours.RemainingHours;
                bool result = workHours.AddOvertime();

                Assert.IsFalse(result, "Third overtime should fail");
                Assert.AreEqual(remainingBefore, workHours.RemainingHours,
                    "RemainingHours should not change after failed overtime");
            }
        }

        /// <summary>
        /// Property 11: Work Hours Accumulation and Reset - TotalAvailableHours calculation
        /// TotalAvailableHours == dailyCapacity + (overtimeUsedToday * 4)
        /// **Validates: Requirements R10**
        /// </summary>
        [Test]
        public void WorkHours_TotalAvailableHours_CalculatesCorrectly()
        {
            for (int i = 0; i < 100; i++)
            {
                int overtimeUsed = _random.Next(0, 3);
                var workHours = new WorkHoursData
                {
                    overtimeUsedToday = overtimeUsed
                };

                int expected = 8 + (overtimeUsed * 4);
                int actual = workHours.TotalAvailableHours;

                Assert.AreEqual(expected, actual,
                    $"TotalAvailableHours calculation failed. OvertimeUsed: {overtimeUsed}");
            }
        }
    }
}
