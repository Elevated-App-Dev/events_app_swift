import Foundation

struct BookingResult: Codable, Equatable {
    var success: Bool
    var hasWarning: Bool
    var price: Double
    var message: String

    static func successful(price: Double, message: String) -> BookingResult {
        BookingResult(success: true, hasWarning: false, price: price, message: message)
    }

    static func successfulWithWarning(price: Double, message: String) -> BookingResult {
        BookingResult(success: true, hasWarning: true, price: price, message: message)
    }

    static func failed(_ message: String) -> BookingResult {
        BookingResult(success: false, hasWarning: false, price: 0, message: message)
    }
}

protocol EventPlanningSystemProtocol {
    func generateInquiry(stage: Int, reputation: Int, currentDate: GameDate) -> ClientInquiry
    func acceptInquiry(_ inquiry: ClientInquiry, currentDate: GameDate) -> EventData
    func declineInquiry(_ inquiry: ClientInquiry)
    func getWorkloadStatus(activeEvents: Int, stage: Int) -> WorkloadStatus
    func calculateWorkloadPenalty(activeEvents: Int, stage: Int) -> Double
    func calculateTaskFailureProbabilityIncrease(activeEvents: Int, stage: Int) -> Double
    func calculateOverlappingPrepPenalty(_ overlappingEvents: Int) -> Double
    func bookVendor(event: EventData, vendor: VendorData, dates: [GameDate]) -> BookingResult
    func bookVenue(event: EventData, venue: VenueData, dates: [GameDate]) -> BookingResult
    func generateEventTitle(clientName: String, subCategory: String) -> String
    func getInquiryIntervalRange(stage: Int) -> (min: Double, max: Double)
    func calculateAdjustedInquiryInterval(stage: Int, reputation: Int) -> Double
}
