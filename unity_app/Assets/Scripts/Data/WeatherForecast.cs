using System;
using EventPlannerSim.Core;

namespace EventPlannerSim.Data
{
    /// <summary>
    /// Represents a weather forecast for a specific date.
    /// Accuracy varies based on how far out the forecast is:
    /// - 7+ days: 70% accuracy
    /// - 2-6 days: 90% accuracy  
    /// - 0-1 days: 100% accuracy (actual weather revealed)
    /// </summary>
    [Serializable]
    public class WeatherForecast
    {
        /// <summary>
        /// The date this forecast is for.
        /// </summary>
        public GameDate date;

        /// <summary>
        /// The predicted weather type.
        /// </summary>
        public WeatherType predictedWeather;

        /// <summary>
        /// Forecast accuracy (0.7 at 7 days, 0.9 at 2 days, 1.0 day-of).
        /// </summary>
        public float accuracy;

        /// <summary>
        /// The actual weather that occurs (revealed on day-of).
        /// </summary>
        public WeatherType actualWeather;

        /// <summary>
        /// Whether the actual weather has been revealed (day-of or past).
        /// </summary>
        public bool isActualRevealed;

        /// <summary>
        /// Default constructor for serialization.
        /// </summary>
        public WeatherForecast()
        {
            date = new GameDate(1, 1, 1);
            predictedWeather = WeatherType.Clear;
            accuracy = 1.0f;
            actualWeather = WeatherType.Clear;
            isActualRevealed = false;
        }

        /// <summary>
        /// Create a new weather forecast.
        /// </summary>
        public WeatherForecast(GameDate date, WeatherType predictedWeather, float accuracy)
        {
            this.date = date;
            this.predictedWeather = predictedWeather;
            this.accuracy = accuracy;
            this.actualWeather = predictedWeather;
            this.isActualRevealed = false;
        }

        /// <summary>
        /// Stage 1 simplified display.
        /// Clear/Cloudy = Good, LightRain = Risky, HeavyRain/Extreme = Bad.
        /// </summary>
        public WeatherRisk GetSimplifiedRisk()
        {
            var weatherToCheck = isActualRevealed ? actualWeather : predictedWeather;
            return weatherToCheck switch
            {
                WeatherType.Clear => WeatherRisk.Good,
                WeatherType.Cloudy => WeatherRisk.Good,
                WeatherType.LightRain => WeatherRisk.Risky,
                WeatherType.HeavyRain => WeatherRisk.Bad,
                WeatherType.ExtremeHeat => WeatherRisk.Bad,
                WeatherType.ExtremeCold => WeatherRisk.Bad,
                _ => WeatherRisk.Good
            };
        }

        /// <summary>
        /// Get the weather type to display (actual if revealed, predicted otherwise).
        /// </summary>
        public WeatherType GetDisplayWeather()
        {
            return isActualRevealed ? actualWeather : predictedWeather;
        }

        /// <summary>
        /// Calculate accuracy based on days until the forecast date.
        /// </summary>
        public static float CalculateAccuracy(int daysOut)
        {
            if (daysOut <= 1)
                return 1.0f;  // Day-of or tomorrow: 100% accuracy
            if (daysOut <= 6)
                return 0.9f;  // 2-6 days out: 90% accuracy
            return 0.7f;      // 7+ days out: 70% accuracy
        }

        public override string ToString()
        {
            var weather = isActualRevealed ? actualWeather : predictedWeather;
            var accuracyStr = isActualRevealed ? "actual" : $"{accuracy * 100:F0}%";
            return $"{date.ToDisplayString()}: {weather} ({accuracyStr})";
        }
    }
}
