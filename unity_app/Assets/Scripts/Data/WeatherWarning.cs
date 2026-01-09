using System;
using EventPlannerSim.Core;

namespace EventPlannerSim.Data
{
    /// <summary>
    /// Weather warning for outdoor event booking decisions.
    /// Provides risk assessment and suggested actions.
    /// </summary>
    [Serializable]
    public class WeatherWarning
    {
        public WeatherRisk riskLevel;
        public string warningMessage;
        public string suggestedAction;
        public float satisfactionPenaltyIfIgnored;
        public WeatherType weatherType;
        public float forecastAccuracy;

        /// <summary>
        /// Default constructor for serialization.
        /// </summary>
        public WeatherWarning()
        {
            riskLevel = WeatherRisk.Good;
            warningMessage = string.Empty;
            suggestedAction = string.Empty;
            satisfactionPenaltyIfIgnored = 0f;
            weatherType = WeatherType.Clear;
            forecastAccuracy = 1.0f;
        }

        /// <summary>
        /// Create a weather warning with all parameters.
        /// </summary>
        public WeatherWarning(
            WeatherRisk riskLevel,
            string warningMessage,
            string suggestedAction,
            float satisfactionPenaltyIfIgnored,
            WeatherType weatherType,
            float forecastAccuracy)
        {
            this.riskLevel = riskLevel;
            this.warningMessage = warningMessage;
            this.suggestedAction = suggestedAction;
            this.satisfactionPenaltyIfIgnored = satisfactionPenaltyIfIgnored;
            this.weatherType = weatherType;
            this.forecastAccuracy = forecastAccuracy;
        }

        /// <summary>
        /// Create a "no warning" result for good weather.
        /// </summary>
        public static WeatherWarning NoWarning(WeatherType weatherType, float accuracy)
        {
            return new WeatherWarning(
                WeatherRisk.Good,
                "Weather looks good for your outdoor event!",
                string.Empty,
                0f,
                weatherType,
                accuracy
            );
        }

        /// <summary>
        /// Create a warning for risky weather (light rain).
        /// </summary>
        public static WeatherWarning RiskyWarning(WeatherType weatherType, float accuracy)
        {
            return new WeatherWarning(
                WeatherRisk.Risky,
                "Light rain is possible on your event date.",
                "Consider booking a tent rental or having an indoor backup plan.",
                15f,
                weatherType,
                accuracy
            );
        }

        /// <summary>
        /// Create a strong warning for bad weather.
        /// </summary>
        public static WeatherWarning BadWeatherWarning(WeatherType weatherType, float accuracy)
        {
            string message = weatherType switch
            {
                WeatherType.HeavyRain => "Heavy rain is forecasted for your event date!",
                WeatherType.ExtremeHeat => "Extreme heat is forecasted for your event date!",
                WeatherType.ExtremeCold => "Extreme cold is forecasted for your event date!",
                _ => "Severe weather is forecasted for your event date!"
            };

            string suggestion = weatherType switch
            {
                WeatherType.HeavyRain => "Strongly recommend booking an indoor venue or tent rental.",
                WeatherType.ExtremeHeat => "Consider an indoor venue with air conditioning or provide cooling stations.",
                WeatherType.ExtremeCold => "Consider an indoor venue with heating or provide warming stations.",
                _ => "Consider an indoor alternative."
            };

            float penalty = weatherType switch
            {
                WeatherType.HeavyRain => 30f,
                WeatherType.ExtremeHeat => 25f,
                WeatherType.ExtremeCold => 25f,
                _ => 20f
            };

            return new WeatherWarning(
                WeatherRisk.Bad,
                message,
                suggestion,
                penalty,
                weatherType,
                accuracy
            );
        }

        /// <summary>
        /// Check if this warning requires player attention.
        /// </summary>
        public bool RequiresAttention => riskLevel != WeatherRisk.Good;

        /// <summary>
        /// Check if this is a severe warning.
        /// </summary>
        public bool IsSevere => riskLevel == WeatherRisk.Bad;

        public override string ToString()
        {
            return $"[{riskLevel}] {warningMessage}";
        }
    }
}
