import Foundation

/// A record of money coming in or going out.
struct Transaction: Codable, Equatable, Identifiable {
    let id: String
    let date: GameDate
    let amount: Double
    let description: String
    let category: TransactionKind

    var isIncome: Bool { amount > 0 }

    static func income(date: GameDate, amount: Double, description: String, category: TransactionKind = .clientPayment) -> Transaction {
        Transaction(id: UUID().uuidString, date: date, amount: amount, description: description, category: category)
    }

    static func expense(date: GameDate, amount: Double, description: String, category: TransactionKind = .vendorPayment) -> Transaction {
        Transaction(id: UUID().uuidString, date: date, amount: -abs(amount), description: description, category: category)
    }
}

enum TransactionKind: String, Codable, Equatable {
    case clientDeposit
    case clientPayment
    case vendorPayment
    case venuePayment
    case eventProfit
    case eventLoss
}
