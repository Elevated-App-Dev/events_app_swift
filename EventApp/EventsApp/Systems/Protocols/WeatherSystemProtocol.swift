import Foundation

protocol WeatherSystemProtocol {
    var currentDate: GameDate { get }
    var currentSeason: String { get }
    func getForecast() -> [WeatherForecast]
    func getForecastForDate(_ date: GameDate) -> WeatherForecast?
    func getSimplifiedRisk(for date: GameDate) -> WeatherRisk
    func advanceDay(to date: GameDate)
    func checkOutdoorEventRisk(date: GameDate, daysOut: Int) -> WeatherWarning
    func setCurrentDate(_ date: GameDate)
    func regenerateForecasts()
}
