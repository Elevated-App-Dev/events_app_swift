import Foundation

enum NotificationType: String, CaseIterable, Codable, Hashable {
    case eventDeadline
    case taskDeadline
    case newInquiry
    case referral
    case financialWarning
    case weatherAlert
}
