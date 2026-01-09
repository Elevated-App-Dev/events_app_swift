namespace EventPlannerSim.Core
{
    /// <summary>
    /// Represents the type of weather condition.
    /// </summary>
    public enum WeatherType
    {
        Clear,
        Cloudy,
        LightRain,
        HeavyRain,
        ExtremeHeat,
        ExtremeCold
    }

    /// <summary>
    /// Simplified weather risk indicator for Stage 1 display.
    /// </summary>
    public enum WeatherRisk
    {
        Good,
        Risky,
        Bad
    }
}
