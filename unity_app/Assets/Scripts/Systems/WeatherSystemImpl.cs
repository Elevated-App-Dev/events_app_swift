using System;
using System.Collections.Generic;
using EventPlannerSim.Core;
using EventPlannerSim.Data;

namespace EventPlannerSim.Systems
{
    /// <summary>
    /// Implementation of the weather system.
    /// Manages weather forecasting and outdoor event risk assessment.
    /// Requirements: R32.1-R32.12
    /// </summary>
    public class WeatherSystemImpl : IWeatherSystem
    {
        private GameDate _currentDate;
        private List<WeatherForecast> _forecasts;
        private Random _random;
        private string _currentSeason;

        // Weather probabilities by season (R32.12 - learnable patterns)
        private static readonly Dictionary<string, Dictionary<WeatherType, float>> SeasonProbabilities = 
            new Dictionary<string, Dictionary<WeatherType, float>>
        {
            ["Spring"] = new Dictionary<WeatherType, float>
            {
                { WeatherType.Clear, 0.35f },
                { WeatherType.Cloudy, 0.25f },
                { WeatherType.LightRain, 0.25f },
                { WeatherType.HeavyRain, 0.10f },
                { WeatherType.ExtremeHeat, 0.03f },
                { WeatherType.ExtremeCold, 0.02f }
            },
            ["Summer"] = new Dictionary<WeatherType, float>
            {
                { WeatherType.Clear, 0.45f },
                { WeatherType.Cloudy, 0.20f },
                { WeatherType.LightRain, 0.10f },
                { WeatherType.HeavyRain, 0.05f },
                { WeatherType.ExtremeHeat, 0.18f },
                { WeatherType.ExtremeCold, 0.02f }
            },
            ["Fall"] = new Dictionary<WeatherType, float>
            {
                { WeatherType.Clear, 0.30f },
                { WeatherType.Cloudy, 0.30f },
                { WeatherType.LightRain, 0.20f },
                { WeatherType.HeavyRain, 0.10f },
                { WeatherType.ExtremeHeat, 0.02f },
                { WeatherType.ExtremeCold, 0.08f }
            },
            ["Winter"] = new Dictionary<WeatherType, float>
            {
                { WeatherType.Clear, 0.25f },
                { WeatherType.Cloudy, 0.30f },
                { WeatherType.LightRain, 0.15f },
                { WeatherType.HeavyRain, 0.08f },
                { WeatherType.ExtremeHeat, 0.02f },
                { WeatherType.ExtremeCold, 0.20f }
            }
        };

        /// <summary>
        /// Create a new weather system with default date.
        /// </summary>
        public WeatherSystemImpl() : this(new GameDate(1, 1, 1))
        {
        }

        /// <summary>
        /// Create a new weather system with specified starting date.
        /// </summary>
        /// <param name="startDate">The starting date.</param>
        public WeatherSystemImpl(GameDate startDate) : this(startDate, Environment.TickCount)
        {
        }

        /// <summary>
        /// Create a new weather system with specified date and random seed.
        /// </summary>
        /// <param name="startDate">The starting date.</param>
        /// <param name="seed">Random seed for reproducible weather.</param>
        public WeatherSystemImpl(GameDate startDate, int seed)
        {
            _currentDate = startDate;
            _random = new Random(seed);
            _forecasts = new List<WeatherForecast>();
            _currentSeason = GetSeasonForMonth(startDate.month);
            RegenerateForecasts();
        }

        /// <inheritdoc/>
        public GameDate CurrentDate => _currentDate;

        /// <inheritdoc/>
        public string CurrentSeason => _currentSeason;

        /// <inheritdoc/>
        List<WeatherForecast> IWeatherSystem.GetForecast()
        {
            return new List<WeatherForecast>(_forecasts);
        }

        /// <summary>
        /// Get current 7-day forecast.
        /// </summary>
        public List<WeatherForecast> GetForecast()
        {
            return new List<WeatherForecast>(_forecasts);
        }

        /// <inheritdoc/>
        WeatherForecast IWeatherSystem.GetForecastForDate(GameDate date)
        {
            return GetForecastForDate(date);
        }

        /// <summary>
        /// Get weather forecast for a specific date.
        /// </summary>
        public WeatherForecast GetForecastForDate(GameDate date)
        {
            foreach (var forecast in _forecasts)
            {
                if (forecast.date.Equals(date))
                {
                    return forecast;
                }
            }
            return null;
        }

        /// <inheritdoc/>
        public WeatherRisk GetSimplifiedRisk(GameDate date)
        {
            var forecast = GetForecastForDate(date);
            if (forecast == null)
            {
                // If date is not in forecast range, generate a temporary forecast
                return WeatherRisk.Good;
            }
            return forecast.GetSimplifiedRisk();
        }

        /// <inheritdoc/>
        public void AdvanceDay(GameDate newDate)
        {
            // Update current date
            _currentDate = newDate;
            _currentSeason = GetSeasonForMonth(newDate.month);

            // Reveal actual weather for today and past dates
            foreach (var forecast in _forecasts)
            {
                if (forecast.date <= newDate && !forecast.isActualRevealed)
                {
                    // Determine if prediction was accurate based on accuracy
                    float roll = (float)_random.NextDouble();
                    if (roll <= forecast.accuracy)
                    {
                        // Prediction was correct
                        forecast.actualWeather = forecast.predictedWeather;
                    }
                    else
                    {
                        // Prediction was wrong - generate different weather
                        forecast.actualWeather = GenerateRandomWeather(_currentSeason);
                        // Make sure it's different from predicted
                        while (forecast.actualWeather == forecast.predictedWeather)
                        {
                            forecast.actualWeather = GenerateRandomWeather(_currentSeason);
                        }
                    }
                    forecast.isActualRevealed = true;
                }
            }

            // Remove old forecasts (before current date)
            _forecasts.RemoveAll(f => f.date < newDate);

            // Add new forecasts to maintain 7-day window
            while (_forecasts.Count < 7)
            {
                var lastDate = _forecasts.Count > 0 
                    ? _forecasts[_forecasts.Count - 1].date 
                    : newDate.AddDays(-1);
                var nextDate = lastDate.AddDays(1);
                int daysOut = GameDate.DaysBetween(newDate, nextDate);
                float accuracy = WeatherForecast.CalculateAccuracy(daysOut);
                var weather = GenerateRandomWeather(GetSeasonForMonth(nextDate.month));
                
                _forecasts.Add(new WeatherForecast(nextDate, weather, accuracy));
            }

            // Update accuracy for existing forecasts based on new distance
            foreach (var forecast in _forecasts)
            {
                if (!forecast.isActualRevealed)
                {
                    int daysOut = GameDate.DaysBetween(newDate, forecast.date);
                    forecast.accuracy = WeatherForecast.CalculateAccuracy(daysOut);
                }
            }
        }

        /// <inheritdoc/>
        WeatherWarning IWeatherSystem.CheckOutdoorEventRisk(GameDate eventDate, int stage)
        {
            return CheckOutdoorEventRisk(eventDate, stage);
        }

        /// <summary>
        /// Check if outdoor event on date has weather risk.
        /// </summary>
        public WeatherWarning CheckOutdoorEventRisk(GameDate eventDate, int stage)
        {
            var forecast = GetForecastForDate(eventDate);
            
            // If no forecast available, generate one
            if (forecast == null)
            {
                int daysOut = GameDate.DaysBetween(_currentDate, eventDate);
                if (daysOut < 0)
                {
                    // Past date - assume good weather
                    return WeatherWarning.NoWarning(WeatherType.Clear, 1.0f);
                }
                
                float accuracy = WeatherForecast.CalculateAccuracy(daysOut);
                var weather = GenerateRandomWeather(GetSeasonForMonth(eventDate.month));
                forecast = new WeatherForecast(eventDate, weather, accuracy);
            }

            var weatherToCheck = forecast.GetDisplayWeather();
            float currentAccuracy = forecast.accuracy;

            // Stage 1: Simplified display with 100% accuracy (R32.7a)
            if (stage == 1)
            {
                currentAccuracy = 1.0f;
            }

            // Generate appropriate warning based on weather type
            return weatherToCheck switch
            {
                WeatherType.Clear => WeatherWarning.NoWarning(weatherToCheck, currentAccuracy),
                WeatherType.Cloudy => WeatherWarning.NoWarning(weatherToCheck, currentAccuracy),
                WeatherType.LightRain => WeatherWarning.RiskyWarning(weatherToCheck, currentAccuracy),
                WeatherType.HeavyRain => WeatherWarning.BadWeatherWarning(weatherToCheck, currentAccuracy),
                WeatherType.ExtremeHeat => WeatherWarning.BadWeatherWarning(weatherToCheck, currentAccuracy),
                WeatherType.ExtremeCold => WeatherWarning.BadWeatherWarning(weatherToCheck, currentAccuracy),
                _ => WeatherWarning.NoWarning(weatherToCheck, currentAccuracy)
            };
        }

        /// <inheritdoc/>
        public void SetCurrentDate(GameDate date)
        {
            _currentDate = date;
            _currentSeason = GetSeasonForMonth(date.month);
        }

        /// <inheritdoc/>
        public void RegenerateForecasts()
        {
            _forecasts.Clear();
            
            for (int i = 0; i < 7; i++)
            {
                var forecastDate = _currentDate.AddDays(i);
                int daysOut = i;
                float accuracy = WeatherForecast.CalculateAccuracy(daysOut);
                var season = GetSeasonForMonth(forecastDate.month);
                var weather = GenerateRandomWeather(season);
                
                var forecast = new WeatherForecast(forecastDate, weather, accuracy);
                
                // Day 0 (today) has actual weather revealed
                if (i == 0)
                {
                    forecast.isActualRevealed = true;
                    forecast.actualWeather = forecast.predictedWeather;
                }
                
                _forecasts.Add(forecast);
            }
        }

        /// <summary>
        /// Generate random weather based on seasonal probabilities.
        /// </summary>
        private WeatherType GenerateRandomWeather(string season)
        {
            if (!SeasonProbabilities.ContainsKey(season))
            {
                season = "Spring";
            }

            var probabilities = SeasonProbabilities[season];
            float roll = (float)_random.NextDouble();
            float cumulative = 0f;

            foreach (var kvp in probabilities)
            {
                cumulative += kvp.Value;
                if (roll <= cumulative)
                {
                    return kvp.Key;
                }
            }

            return WeatherType.Clear;
        }

        /// <summary>
        /// Get the season name for a given month.
        /// </summary>
        private static string GetSeasonForMonth(int month)
        {
            return month switch
            {
                3 or 4 or 5 => "Spring",
                6 or 7 or 8 => "Summer",
                9 or 10 or 11 => "Fall",
                12 or 1 or 2 => "Winter",
                _ => "Spring"
            };
        }

        /// <summary>
        /// Load weather system state from saved data.
        /// </summary>
        public void LoadFromData(WeatherSystemData data)
        {
            if (data == null) return;

            _currentSeason = data.currentSeason ?? "Spring";
            _random = new Random(data.randomSeed);
            _forecasts = data.forecasts ?? new List<WeatherForecast>();
        }

        /// <summary>
        /// Export weather system state for saving.
        /// </summary>
        public WeatherSystemData ExportData()
        {
            return new WeatherSystemData
            {
                forecasts = new List<WeatherForecast>(_forecasts),
                currentSeason = _currentSeason,
                randomSeed = _random.Next()
            };
        }
    }
}
