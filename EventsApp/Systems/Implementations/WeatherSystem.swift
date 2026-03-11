import Foundation

/// Manages weather forecasting and outdoor event risk assessment.
/// Seasonal probability tables, 7-day forecast with accuracy degradation.
class WeatherSystem: WeatherSystemProtocol {

    // MARK: - Seasonal Probabilities

    private static let seasonProbabilities: [String: [(WeatherType, Double)]] = [
        "Spring": [(.clear, 0.35), (.cloudy, 0.25), (.lightRain, 0.25),
                   (.heavyRain, 0.10), (.extremeHeat, 0.03), (.extremeCold, 0.02)],
        "Summer": [(.clear, 0.45), (.cloudy, 0.20), (.lightRain, 0.10),
                   (.heavyRain, 0.05), (.extremeHeat, 0.18), (.extremeCold, 0.02)],
        "Fall":   [(.clear, 0.30), (.cloudy, 0.30), (.lightRain, 0.20),
                   (.heavyRain, 0.10), (.extremeHeat, 0.02), (.extremeCold, 0.08)],
        "Winter": [(.clear, 0.25), (.cloudy, 0.30), (.lightRain, 0.15),
                   (.heavyRain, 0.08), (.extremeHeat, 0.02), (.extremeCold, 0.20)]
    ]

    // MARK: - State

    private(set) var currentDate: GameDate
    private(set) var currentSeason: String
    private var forecasts: [WeatherForecast]

    // MARK: - Init

    init(startDate: GameDate = GameDate(month: 1, day: 1, year: 1)) {
        self.currentDate = startDate
        self.currentSeason = Self.seasonForMonth(startDate.month)
        self.forecasts = []
        regenerateForecasts()
    }

    // MARK: - Protocol Conformance

    func getForecast() -> [WeatherForecast] {
        forecasts
    }

    func getForecastForDate(_ date: GameDate) -> WeatherForecast? {
        forecasts.first { $0.date == date }
    }

    func getSimplifiedRisk(for date: GameDate) -> WeatherRisk {
        guard let forecast = getForecastForDate(date) else { return .good }
        return forecast.getSimplifiedRisk()
    }

    func advanceDay(to date: GameDate) {
        currentDate = date
        currentSeason = Self.seasonForMonth(date.month)

        // Reveal actual weather for today and past dates
        for i in 0..<forecasts.count {
            if forecasts[i].date <= date && !forecasts[i].isActualRevealed {
                let roll = Double.random(in: 0..<1)
                if roll <= forecasts[i].accuracy {
                    // Prediction correct
                    forecasts[i].actualWeather = forecasts[i].predictedWeather
                } else {
                    // Prediction wrong — generate different weather
                    var newWeather = generateRandomWeather(for: currentSeason)
                    while newWeather == forecasts[i].predictedWeather {
                        newWeather = generateRandomWeather(for: currentSeason)
                    }
                    forecasts[i].actualWeather = newWeather
                }
                forecasts[i].isActualRevealed = true
            }
        }

        // Remove old forecasts
        forecasts.removeAll { $0.date < date }

        // Add new forecasts to maintain 7-day window
        while forecasts.count < 7 {
            let lastDate = forecasts.last?.date ?? date.adding(days: -1)
            let nextDate = lastDate.adding(days: 1)
            let daysOut = date.daysBetween(nextDate)
            let accuracy = WeatherForecast.calculateAccuracy(daysOut: daysOut)
            let weather = generateRandomWeather(for: Self.seasonForMonth(nextDate.month))
            forecasts.append(WeatherForecast(date: nextDate, predictedWeather: weather, accuracy: accuracy))
        }

        // Update accuracy for remaining forecasts
        for i in 0..<forecasts.count where !forecasts[i].isActualRevealed {
            let daysOut = date.daysBetween(forecasts[i].date)
            forecasts[i].accuracy = WeatherForecast.calculateAccuracy(daysOut: daysOut)
        }
    }

    func checkOutdoorEventRisk(date: GameDate, daysOut: Int) -> WeatherWarning {
        var forecast = getForecastForDate(date)

        if forecast == nil {
            let calculatedDaysOut = currentDate.daysBetween(date)
            if calculatedDaysOut < 0 {
                return WeatherWarning.noWarning(weatherType: .clear, accuracy: 1.0)
            }
            let accuracy = WeatherForecast.calculateAccuracy(daysOut: calculatedDaysOut)
            let weather = generateRandomWeather(for: Self.seasonForMonth(date.month))
            forecast = WeatherForecast(date: date, predictedWeather: weather, accuracy: accuracy)
        }

        let weatherToCheck = forecast!.getDisplayWeather()
        let currentAccuracy = forecast!.accuracy

        switch weatherToCheck {
        case .clear, .cloudy:
            return .noWarning(weatherType: weatherToCheck, accuracy: currentAccuracy)
        case .lightRain:
            return .riskyWarning(weatherType: weatherToCheck, accuracy: currentAccuracy)
        case .heavyRain, .extremeHeat, .extremeCold:
            return .badWeatherWarning(weatherType: weatherToCheck, accuracy: currentAccuracy)
        }
    }

    func setCurrentDate(_ date: GameDate) {
        currentDate = date
        currentSeason = Self.seasonForMonth(date.month)
    }

    func regenerateForecasts() {
        forecasts.removeAll()

        for i in 0..<7 {
            let forecastDate = currentDate.adding(days: i)
            let accuracy = WeatherForecast.calculateAccuracy(daysOut: i)
            let season = Self.seasonForMonth(forecastDate.month)
            let weather = generateRandomWeather(for: season)

            var forecast = WeatherForecast(date: forecastDate, predictedWeather: weather, accuracy: accuracy)

            // Day 0 (today) has actual weather revealed
            if i == 0 {
                forecast.isActualRevealed = true
                forecast.actualWeather = forecast.predictedWeather
            }

            forecasts.append(forecast)
        }
    }

    // MARK: - Save/Load

    func loadFromData(_ data: WeatherSystemData) {
        currentSeason = data.currentSeason
        forecasts = data.forecasts
    }

    func exportData() -> WeatherSystemData {
        WeatherSystemData(
            forecasts: forecasts,
            currentSeason: currentSeason,
            randomSeed: Int.random(in: 0..<Int.max)
        )
    }

    // MARK: - Private Helpers

    private func generateRandomWeather(for season: String) -> WeatherType {
        let probs = Self.seasonProbabilities[season] ?? Self.seasonProbabilities["Spring"]!
        let roll = Double.random(in: 0..<1)
        var cumulative = 0.0

        for (weatherType, probability) in probs {
            cumulative += probability
            if roll <= cumulative {
                return weatherType
            }
        }

        return .clear
    }

    private static func seasonForMonth(_ month: Int) -> String {
        switch month {
        case 3, 4, 5:   return "Spring"
        case 6, 7, 8:   return "Summer"
        case 9, 10, 11: return "Fall"
        case 12, 1, 2:  return "Winter"
        default:         return "Spring"
        }
    }
}
