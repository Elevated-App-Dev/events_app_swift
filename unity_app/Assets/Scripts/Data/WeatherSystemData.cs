using System;
using System.Collections.Generic;

namespace EventPlannerSim.Data
{
    /// <summary>
    /// Weather system state data for serialization.
    /// Used for save/load functionality.
    /// </summary>
    [Serializable]
    public class WeatherSystemData
    {
        /// <summary>
        /// 7-day rolling weather forecast.
        /// </summary>
        public List<WeatherForecast> forecasts = new List<WeatherForecast>();

        /// <summary>
        /// Current season affecting weather probabilities.
        /// </summary>
        public string currentSeason = "Spring";

        /// <summary>
        /// Random seed for reproducible weather generation.
        /// </summary>
        public int randomSeed;

        /// <summary>
        /// Create a deep copy of this weather system data.
        /// </summary>
        public WeatherSystemData Clone()
        {
            var clone = new WeatherSystemData
            {
                currentSeason = this.currentSeason,
                randomSeed = this.randomSeed,
                forecasts = new List<WeatherForecast>()
            };

            foreach (var forecast in this.forecasts)
            {
                clone.forecasts.Add(new WeatherForecast
                {
                    date = forecast.date,
                    predictedWeather = forecast.predictedWeather,
                    accuracy = forecast.accuracy,
                    actualWeather = forecast.actualWeather,
                    isActualRevealed = forecast.isActualRevealed
                });
            }

            return clone;
        }
    }
}
