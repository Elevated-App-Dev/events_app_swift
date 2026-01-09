using System;
using NUnit.Framework;
using EventPlannerSim.Core;

namespace EventPlannerSim.Tests.EditMode
{
    /// <summary>
    /// Property-based tests for GameDate struct.
    /// Feature: event-planner-simulator, Property: GameDate serialization round-trip
    /// Validates: Requirements R27.1
    /// </summary>
    [TestFixture]
    public class GameDatePropertyTests
    {
        private Random _random;

        [SetUp]
        public void Setup()
        {
            _random = new Random(42); // Fixed seed for reproducibility
        }

        /// <summary>
        /// Property 1: GameDate serialization round-trip
        /// For any valid GameDate, converting to TotalDays and back should produce an equivalent date.
        /// **Validates: Requirements R27.1**
        /// </summary>
        [Test]
        public void GameDate_TotalDays_RoundTrip_PreservesDate()
        {
            // Run 100 iterations as per testing strategy
            for (int i = 0; i < 100; i++)
            {
                // Generate random valid GameDate
                int day = _random.Next(1, 31);   // 1-30
                int month = _random.Next(1, 13); // 1-12
                int year = _random.Next(1, 101); // 1-100

                var original = new GameDate(day, month, year);
                
                // Round-trip through TotalDays
                int totalDays = original.TotalDays;
                var reconstructed = GameDate.FromTotalDays(totalDays);

                // Assert equality
                Assert.AreEqual(original.day, reconstructed.day, 
                    $"Day mismatch for input ({day}, {month}, {year}). TotalDays={totalDays}");
                Assert.AreEqual(original.month, reconstructed.month, 
                    $"Month mismatch for input ({day}, {month}, {year}). TotalDays={totalDays}");
                Assert.AreEqual(original.year, reconstructed.year, 
                    $"Year mismatch for input ({day}, {month}, {year}). TotalDays={totalDays}");
                Assert.AreEqual(original, reconstructed,
                    $"GameDate equality failed for input ({day}, {month}, {year})");
            }
        }

        /// <summary>
        /// Property 2: AddDays then DaysBetween round-trip
        /// For any GameDate and positive days to add, adding days then calculating difference should return original days.
        /// **Validates: Requirements R27.1**
        /// </summary>
        [Test]
        public void GameDate_AddDays_DaysBetween_RoundTrip()
        {
            for (int i = 0; i < 100; i++)
            {
                // Generate random valid GameDate
                int day = _random.Next(1, 31);
                int month = _random.Next(1, 13);
                int year = _random.Next(1, 50); // Keep year smaller to avoid overflow

                var original = new GameDate(day, month, year);
                int daysToAdd = _random.Next(0, 1000); // 0-999 days

                // Add days and calculate difference
                var future = original.AddDays(daysToAdd);
                int calculatedDays = GameDate.DaysBetween(original, future);

                Assert.AreEqual(daysToAdd, calculatedDays,
                    $"DaysBetween mismatch. Original: ({day}, {month}, {year}), DaysToAdd: {daysToAdd}, Future: ({future.day}, {future.month}, {future.year})");
            }
        }

        /// <summary>
        /// Property 3: Comparison operators consistency
        /// For any two GameDates, comparison operators should be consistent with TotalDays comparison.
        /// **Validates: Requirements R27.1**
        /// </summary>
        [Test]
        public void GameDate_ComparisonOperators_ConsistentWithTotalDays()
        {
            for (int i = 0; i < 100; i++)
            {
                // Generate two random GameDates
                var date1 = new GameDate(
                    _random.Next(1, 31),
                    _random.Next(1, 13),
                    _random.Next(1, 50)
                );
                var date2 = new GameDate(
                    _random.Next(1, 31),
                    _random.Next(1, 13),
                    _random.Next(1, 50)
                );

                // Verify consistency
                Assert.AreEqual(date1.TotalDays < date2.TotalDays, date1 < date2,
                    $"< operator inconsistent for {date1} and {date2}");
                Assert.AreEqual(date1.TotalDays > date2.TotalDays, date1 > date2,
                    $"> operator inconsistent for {date1} and {date2}");
                Assert.AreEqual(date1.TotalDays <= date2.TotalDays, date1 <= date2,
                    $"<= operator inconsistent for {date1} and {date2}");
                Assert.AreEqual(date1.TotalDays >= date2.TotalDays, date1 >= date2,
                    $">= operator inconsistent for {date1} and {date2}");
                Assert.AreEqual(date1.TotalDays == date2.TotalDays, date1 == date2,
                    $"== operator inconsistent for {date1} and {date2}");
                Assert.AreEqual(date1.TotalDays != date2.TotalDays, date1 != date2,
                    $"!= operator inconsistent for {date1} and {date2}");
            }
        }

        /// <summary>
        /// Property 4: IsBefore/IsAfter consistency
        /// For any two GameDates, IsBefore and IsAfter should be consistent with comparison operators.
        /// **Validates: Requirements R27.1**
        /// </summary>
        [Test]
        public void GameDate_IsBeforeIsAfter_ConsistentWithOperators()
        {
            for (int i = 0; i < 100; i++)
            {
                var date1 = new GameDate(
                    _random.Next(1, 31),
                    _random.Next(1, 13),
                    _random.Next(1, 50)
                );
                var date2 = new GameDate(
                    _random.Next(1, 31),
                    _random.Next(1, 13),
                    _random.Next(1, 50)
                );

                Assert.AreEqual(date1 < date2, date1.IsBefore(date2),
                    $"IsBefore inconsistent with < for {date1} and {date2}");
                Assert.AreEqual(date1 > date2, date1.IsAfter(date2),
                    $"IsAfter inconsistent with > for {date1} and {date2}");
            }
        }

        /// <summary>
        /// Property 5: AddDays with zero is identity
        /// For any GameDate, adding zero days should return the same date.
        /// **Validates: Requirements R27.1**
        /// </summary>
        [Test]
        public void GameDate_AddZeroDays_IsIdentity()
        {
            for (int i = 0; i < 100; i++)
            {
                var original = new GameDate(
                    _random.Next(1, 31),
                    _random.Next(1, 13),
                    _random.Next(1, 100)
                );

                var result = original.AddDays(0);

                Assert.AreEqual(original, result,
                    $"AddDays(0) should be identity for {original}");
            }
        }
    }
}
