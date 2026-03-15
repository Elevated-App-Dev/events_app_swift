import Foundation

struct WeatherForecast: Codable, Equatable {
    var date: GameDate
    var predictedWeather: WeatherType
    var accuracy: Double
    var actualWeather: WeatherType
    var isActualRevealed: Bool

    init(date: GameDate = GameDate(), predictedWeather: WeatherType = .clear, accuracy: Double = 1.0) {
        self.date = date
        self.predictedWeather = predictedWeather
        self.accuracy = accuracy
        self.actualWeather = predictedWeather
        self.isActualRevealed = false
    }

    func getSimplifiedRisk() -> WeatherRisk {
        let weather = getDisplayWeather()
        switch weather {
        case .clear, .cloudy: return .good
        case .lightRain: return .risky
        case .heavyRain, .extremeHeat, .extremeCold: return .bad
        }
    }

    func getDisplayWeather() -> WeatherType {
        isActualRevealed ? actualWeather : predictedWeather
    }

    static func calculateAccuracy(daysOut: Int) -> Double {
        max(0.3, 1.0 - Double(daysOut) * 0.1)
    }
}

struct WeatherWarning: Codable, Equatable {
    var riskLevel: WeatherRisk
    var warningMessage: String
    var suggestedAction: String
    var satisfactionPenaltyIfIgnored: Double
    var weatherType: WeatherType
    var forecastAccuracy: Double

    var requiresAttention: Bool { riskLevel != .good }
    var isSevere: Bool { riskLevel == .bad }

    static func noWarning(weatherType: WeatherType, accuracy: Double) -> WeatherWarning {
        WeatherWarning(
            riskLevel: .good,
            warningMessage: "Weather looks good!",
            suggestedAction: "No action needed.",
            satisfactionPenaltyIfIgnored: 0,
            weatherType: weatherType,
            forecastAccuracy: accuracy
        )
    }

    static func riskyWarning(weatherType: WeatherType, accuracy: Double) -> WeatherWarning {
        WeatherWarning(
            riskLevel: .risky,
            warningMessage: "Weather may cause minor disruptions.",
            suggestedAction: "Consider a backup plan for outdoor activities.",
            satisfactionPenaltyIfIgnored: 10,
            weatherType: weatherType,
            forecastAccuracy: accuracy
        )
    }

    static func badWeatherWarning(weatherType: WeatherType, accuracy: Double) -> WeatherWarning {
        WeatherWarning(
            riskLevel: .bad,
            warningMessage: "Severe weather expected. Outdoor events at risk.",
            suggestedAction: "Move activities indoors or reschedule.",
            satisfactionPenaltyIfIgnored: 25,
            weatherType: weatherType,
            forecastAccuracy: accuracy
        )
    }
}

struct WeatherSystemData: Codable, Equatable {
    var forecasts: [WeatherForecast] = []
    var currentSeason: String = "Spring"
    var randomSeed: Int = 0
}
