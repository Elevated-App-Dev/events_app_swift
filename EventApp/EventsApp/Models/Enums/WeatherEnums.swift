import Foundation

enum WeatherType: String, CaseIterable, Codable, Hashable {
    case clear
    case cloudy
    case lightRain
    case heavyRain
    case extremeHeat
    case extremeCold
}

enum WeatherRisk: String, CaseIterable, Codable, Hashable {
    case good
    case risky
    case bad
}
