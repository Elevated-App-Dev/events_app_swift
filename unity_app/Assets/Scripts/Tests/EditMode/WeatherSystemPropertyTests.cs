using System;
using NUnit.Framework;
using EventPlannerSim.Core;
using EventPlannerSim.Data;
using EventPlannerSim.Systems;

namespace EventPlannerSim.Tests.EditMode
{
    /// <summary>
    /// Property-based tests for WeatherSystem.
    /// Feature: event-planner-simulator, Property 22: Weather Forecast Accuracy
    /// Validates: Requirements R32
    /// </summary>
    [TestFixture]
    public class WeatherSystemPropertyTests
    {
        private Random _random;

        [SetUp]
        public void Setup()
        {
            _random = new Random(42); // Fixed seed for reproducibility
        }

        /// <summary>
        /// Property 22: Weather Forecast Accuracy
        /// For any date D days in the future:
        /// - D >= 7: 70% accuracy
        /// - D >= 2 and D &lt; 7: 90% accuracy
        /// - D == 0 or D == 1: 100% accuracy (actual weather revealed)
        /// Stage 1 simplified: Always 100% accuracy, displayed as Good/Risky/Bad
        /// **Validates: Requirements R32.7**
        /// </summary>
        [Test]
        public void WeatherForecast_CalculateAccuracy_ReturnsCorrectAccuracyByDaysOut()
        {
            // Run 100 iterations as per testing strategy
            for (int i = 0; i < 100; i++)
            {
                // Generate random days out (0-14)
                int daysOut = _random.Next(0, 15);
                
                float accuracy = WeatherForecast.CalculateAccuracy(daysOut);
                
                if (daysOut <= 1)
                {
                    Assert.AreEqual(1.0f, accuracy, 0.001f,
                        $"Days out {daysOut} should have 100% accuracy, got {accuracy * 100}%");
                }
                else if (daysOut <= 6)
                {
                    Assert.AreEqual(0.9f, accuracy, 0.001f,
                        $"Days out {daysOut} should have 90% accuracy, got {accuracy * 100}%");
                }
                else
                {
                    Assert.AreEqual(0.7f, accuracy, 0.001f,
                        $"Days out {daysOut} should have 70% accuracy, got {accuracy * 100}%");
                }
            }
        }

        /// <summary>
        /// Property 22a: Weather System Generates 7-Day Forecast
        /// For any starting date, the weather system should generate exactly 7 forecasts.
        /// **Validates: Requirements R32.1**
        /// </summary>
        [Test]
        public void WeatherSystem_GetForecast_Returns7DayForecast()
        {
            for (int i = 0; i < 100; i++)
            {
                var startDate = new GameDate(
                    _random.Next(1, 31),
                    _random.Next(1, 13),
                    _random.Next(1, 10)
                );
                
                var weatherSystem = new WeatherSystemImpl(startDate, i);
                var forecasts = weatherSystem.GetForecast();
                
                Assert.AreEqual(7, forecasts.Count,
                    $"Weather system should generate exactly 7 forecasts, got {forecasts.Count}");
            }
        }

        /// <summary>
        /// Property 22b: Forecasts Are For Consecutive Days
        /// For any weather system, the 7-day forecast should cover consecutive days starting from current date.
        /// **Validates: Requirements R32.1**
        /// </summary>
        [Test]
        public void WeatherSystem_GetForecast_CoversConsecutiveDays()
        {
            for (int i = 0; i < 100; i++)
            {
                var startDate = new GameDate(
                    _random.Next(1, 31),
                    _random.Next(1, 13),
                    _random.Next(1, 10)
                );
                
                var weatherSystem = new WeatherSystemImpl(startDate, i);
                var forecasts = weatherSystem.GetForecast();
                
                for (int day = 0; day < 7; day++)
                {
                    var expectedDate = startDate.AddDays(day);
                    Assert.AreEqual(expectedDate, forecasts[day].date,
                        $"Forecast {day} should be for {expectedDate}, got {forecasts[day].date}");
                }
            }
        }

        /// <summary>
        /// Property 22c: Forecast Accuracy Matches Days Out
        /// For any forecast in the 7-day window, its accuracy should match the days out calculation.
        /// **Validates: Requirements R32.7**
        /// </summary>
        [Test]
        public void WeatherSystem_ForecastAccuracy_MatchesDaysOut()
        {
            for (int i = 0; i < 100; i++)
            {
                var startDate = new GameDate(
                    _random.Next(1, 31),
                    _random.Next(1, 13),
                    _random.Next(1, 10)
                );
                
                var weatherSystem = new WeatherSystemImpl(startDate, i);
                var forecasts = weatherSystem.GetForecast();
                
                for (int day = 0; day < 7; day++)
                {
                    float expectedAccuracy = WeatherForecast.CalculateAccuracy(day);
                    Assert.AreEqual(expectedAccuracy, forecasts[day].accuracy, 0.001f,
                        $"Forecast for day {day} should have accuracy {expectedAccuracy * 100}%, got {forecasts[day].accuracy * 100}%");
                }
            }
        }

        /// <summary>
        /// Property 22d: Day-Of Weather Is Revealed
        /// For any weather system, the forecast for the current day should have actual weather revealed.
        /// **Validates: Requirements R32.7**
        /// </summary>
        [Test]
        public void WeatherSystem_CurrentDayForecast_HasActualWeatherRevealed()
        {
            for (int i = 0; i < 100; i++)
            {
                var startDate = new GameDate(
                    _random.Next(1, 31),
                    _random.Next(1, 13),
                    _random.Next(1, 10)
                );
                
                var weatherSystem = new WeatherSystemImpl(startDate, i);
                var todayForecast = weatherSystem.GetForecastForDate(startDate);
                
                Assert.IsNotNull(todayForecast, "Should have forecast for current day");
                Assert.IsTrue(todayForecast.isActualRevealed,
                    "Current day forecast should have actual weather revealed");
                Assert.AreEqual(1.0f, todayForecast.accuracy, 0.001f,
                    "Current day forecast should have 100% accuracy");
            }
        }

        /// <summary>
        /// Property 22e: Simplified Risk Mapping
        /// For any weather type, GetSimplifiedRisk should return the correct risk level:
        /// Clear/Cloudy = Good, LightRain = Risky, HeavyRain/Extreme = Bad
        /// **Validates: Requirements R32.7a**
        /// </summary>
        [Test]
        public void WeatherForecast_GetSimplifiedRisk_ReturnsCorrectRiskLevel()
        {
            // Test all weather types
            var testCases = new (WeatherType weather, WeatherRisk expectedRisk)[]
            {
                (WeatherType.Clear, WeatherRisk.Good),
                (WeatherType.Cloudy, WeatherRisk.Good),
                (WeatherType.LightRain, WeatherRisk.Risky),
                (WeatherType.HeavyRain, WeatherRisk.Bad),
                (WeatherType.ExtremeHeat, WeatherRisk.Bad),
                (WeatherType.ExtremeCold, WeatherRisk.Bad)
            };

            foreach (var (weather, expectedRisk) in testCases)
            {
                var forecast = new WeatherForecast(new GameDate(1, 1, 1), weather, 1.0f);
                forecast.isActualRevealed = true;
                forecast.actualWeather = weather;
                
                var actualRisk = forecast.GetSimplifiedRisk();
                
                Assert.AreEqual(expectedRisk, actualRisk,
                    $"Weather type {weather} should map to risk {expectedRisk}, got {actualRisk}");
            }
        }

        /// <summary>
        /// Property 22f: Stage 1 Always Shows 100% Accuracy
        /// For any outdoor event risk check in Stage 1, the warning should indicate 100% accuracy.
        /// **Validates: Requirements R32.7a**
        /// </summary>
        [Test]
        public void WeatherSystem_Stage1_AlwaysShows100PercentAccuracy()
        {
            for (int i = 0; i < 100; i++)
            {
                var startDate = new GameDate(
                    _random.Next(1, 31),
                    _random.Next(1, 13),
                    _random.Next(1, 10)
                );
                
                var weatherSystem = new WeatherSystemImpl(startDate, i);
                
                // Check event dates at various distances
                int daysOut = _random.Next(0, 8);
                var eventDate = startDate.AddDays(daysOut);
                
                var warning = weatherSystem.CheckOutdoorEventRisk(eventDate, stage: 1);
                
                Assert.AreEqual(1.0f, warning.forecastAccuracy, 0.001f,
                    $"Stage 1 should always show 100% accuracy, got {warning.forecastAccuracy * 100}%");
            }
        }

        /// <summary>
        /// Property 22g: Stage 2+ Shows Variable Accuracy
        /// For any outdoor event risk check in Stage 2+, the warning accuracy should match days out.
        /// **Validates: Requirements R32.7**
        /// </summary>
        [Test]
        public void WeatherSystem_Stage2Plus_ShowsVariableAccuracy()
        {
            for (int i = 0; i < 100; i++)
            {
                var startDate = new GameDate(
                    _random.Next(1, 31),
                    _random.Next(1, 13),
                    _random.Next(1, 10)
                );
                
                var weatherSystem = new WeatherSystemImpl(startDate, i);
                int stage = _random.Next(2, 6); // Stage 2-5
                
                // Check event dates at various distances
                int daysOut = _random.Next(0, 7);
                var eventDate = startDate.AddDays(daysOut);
                
                var warning = weatherSystem.CheckOutdoorEventRisk(eventDate, stage);
                float expectedAccuracy = WeatherForecast.CalculateAccuracy(daysOut);
                
                Assert.AreEqual(expectedAccuracy, warning.forecastAccuracy, 0.001f,
                    $"Stage {stage} with {daysOut} days out should show {expectedAccuracy * 100}% accuracy, got {warning.forecastAccuracy * 100}%");
            }
        }

        /// <summary>
        /// Property 22h: Advance Day Updates Forecasts
        /// When advancing a day, the weather system should maintain 7 forecasts and update accuracies.
        /// **Validates: Requirements R32.7**
        /// </summary>
        [Test]
        public void WeatherSystem_AdvanceDay_MaintainsSevenForecasts()
        {
            for (int i = 0; i < 100; i++)
            {
                var startDate = new GameDate(
                    _random.Next(1, 31),
                    _random.Next(1, 13),
                    _random.Next(1, 10)
                );
                
                var weatherSystem = new WeatherSystemImpl(startDate, i);
                
                // Advance multiple days
                int daysToAdvance = _random.Next(1, 10);
                for (int d = 0; d < daysToAdvance; d++)
                {
                    var newDate = startDate.AddDays(d + 1);
                    weatherSystem.AdvanceDay(newDate);
                    
                    var forecasts = weatherSystem.GetForecast();
                    Assert.AreEqual(7, forecasts.Count,
                        $"After advancing {d + 1} days, should still have 7 forecasts, got {forecasts.Count}");
                    
                    // First forecast should be for the new current date
                    Assert.AreEqual(newDate, forecasts[0].date,
                        $"First forecast should be for current date {newDate}, got {forecasts[0].date}");
                }
            }
        }

        /// <summary>
        /// Property 22i: Weather Types Are Valid
        /// For any generated forecast, the weather type should be a valid enum value.
        /// **Validates: Requirements R32.4**
        /// </summary>
        [Test]
        public void WeatherSystem_GeneratedForecasts_HaveValidWeatherTypes()
        {
            var validWeatherTypes = new[]
            {
                WeatherType.Clear,
                WeatherType.Cloudy,
                WeatherType.LightRain,
                WeatherType.HeavyRain,
                WeatherType.ExtremeHeat,
                WeatherType.ExtremeCold
            };

            for (int i = 0; i < 100; i++)
            {
                var startDate = new GameDate(
                    _random.Next(1, 31),
                    _random.Next(1, 13),
                    _random.Next(1, 10)
                );
                
                var weatherSystem = new WeatherSystemImpl(startDate, i);
                var forecasts = weatherSystem.GetForecast();
                
                foreach (var forecast in forecasts)
                {
                    Assert.Contains(forecast.predictedWeather, validWeatherTypes,
                        $"Predicted weather {forecast.predictedWeather} should be a valid weather type");
                }
            }
        }

        /// <summary>
        /// Property 22j: Season Affects Weather Probabilities
        /// Weather patterns should be learnable by season (R32.12).
        /// Summer should have more extreme heat, winter more extreme cold.
        /// **Validates: Requirements R32.12**
        /// </summary>
        [Test]
        public void WeatherSystem_SeasonalPatterns_AreDistinct()
        {
            // Generate many forecasts for each season and count weather types
            int iterations = 1000;
            
            // Summer (month 7)
            int summerHeat = 0;
            int summerCold = 0;
            for (int i = 0; i < iterations; i++)
            {
                var summerDate = new GameDate(15, 7, 1);
                var weatherSystem = new WeatherSystemImpl(summerDate, i);
                var forecasts = weatherSystem.GetForecast();
                foreach (var f in forecasts)
                {
                    if (f.predictedWeather == WeatherType.ExtremeHeat) summerHeat++;
                    if (f.predictedWeather == WeatherType.ExtremeCold) summerCold++;
                }
            }
            
            // Winter (month 1)
            int winterHeat = 0;
            int winterCold = 0;
            for (int i = 0; i < iterations; i++)
            {
                var winterDate = new GameDate(15, 1, 1);
                var weatherSystem = new WeatherSystemImpl(winterDate, i);
                var forecasts = weatherSystem.GetForecast();
                foreach (var f in forecasts)
                {
                    if (f.predictedWeather == WeatherType.ExtremeHeat) winterHeat++;
                    if (f.predictedWeather == WeatherType.ExtremeCold) winterCold++;
                }
            }
            
            // Summer should have more extreme heat than winter
            Assert.Greater(summerHeat, winterHeat,
                $"Summer should have more extreme heat ({summerHeat}) than winter ({winterHeat})");
            
            // Winter should have more extreme cold than summer
            Assert.Greater(winterCold, summerCold,
                $"Winter should have more extreme cold ({winterCold}) than summer ({summerCold})");
        }
    }
}
