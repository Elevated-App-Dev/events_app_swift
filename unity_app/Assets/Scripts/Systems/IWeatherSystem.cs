using System.Collections.Generic;
using EventPlannerSim.Core;

namespace EventPlannerSim.Systems
{
    /// <summary>
    /// Manages weather forecasting and outdoor event impacts.
    /// Pure logic interface - no Unity dependencies.
    /// </summary>
    public interface IWeatherSystem
    {
        /// <summary>
        /// Get current 7-day forecast.
        /// </summary>
        /// <returns>List of weather forecasts for the next 7 days.</returns>
        List<Data.WeatherForecast> GetForecast();

        /// <summary>
        /// Get weather forecast for a specific date.
        /// </summary>
        /// <param name="date">The date to get forecast for.</param>
        /// <returns>Weather forecast for the specified date, or null if not in forecast range.</returns>
        Data.WeatherForecast GetForecastForDate(GameDate date);

        /// <summary>
        /// Get simplified weather risk for Stage 1 display.
        /// Stage 1 shows only Good/Risky/Bad with 100% accuracy.
        /// </summary>
        /// <param name="date">The date to check.</param>
        /// <returns>Simplified weather risk indicator.</returns>
        WeatherRisk GetSimplifiedRisk(GameDate date);

        /// <summary>
        /// Advance weather simulation and update forecasts.
        /// Called when game day advances.
        /// </summary>
        /// <param name="newDate">The new current date.</param>
        void AdvanceDay(GameDate newDate);

        /// <summary>
        /// Check if outdoor event on date has weather risk.
        /// Returns warning level and suggested action.
        /// </summary>
        /// <param name="eventDate">The date of the outdoor event.</param>
        /// <param name="stage">Current business stage (1-5).</param>
        /// <returns>Weather warning with risk level and suggestions.</returns>
        Data.WeatherWarning CheckOutdoorEventRisk(GameDate eventDate, int stage);

        /// <summary>
        /// Get the current date the weather system is tracking.
        /// </summary>
        GameDate CurrentDate { get; }

        /// <summary>
        /// Get the current season based on the month.
        /// </summary>
        string CurrentSeason { get; }

        /// <summary>
        /// Set the current date (used for save/load).
        /// </summary>
        /// <param name="date">The date to set.</param>
        void SetCurrentDate(GameDate date);

        /// <summary>
        /// Initialize or regenerate forecasts for the current date.
        /// </summary>
        void RegenerateForecasts();
    }
}
