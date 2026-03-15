import Foundation

// MARK: - Calendar

struct CalendarEntry: Codable, Equatable, Identifiable {
    var id: String
    var title: String
    var description: String
    var date: GameDate
    var entryType: CalendarEntryType
    var relatedEventId: String?
    var relatedTaskId: String?
    var isCompleted: Bool = false
    var isUrgent: Bool = false

    func isDueOrPast(currentDate: GameDate) -> Bool {
        currentDate >= date
    }

    func daysUntil(currentDate: GameDate) -> Int {
        currentDate.daysBetween(date)
    }
}

// MARK: - Messages

struct MessageThread: Codable, Equatable, Identifiable {
    var id: String { threadId }
    var threadId: String
    var contactName: String
    var contactId: String
    var contactType: MessageContactType
    var messages: [Message] = []
    var hasUnread: Bool = false
    var lastMessageDate: GameDate
    var isCompanyRelated: Bool = false

    func getLatestMessage() -> Message? {
        messages.last
    }

    func getUnreadCount() -> Int {
        messages.filter { !$0.isRead && !$0.isFromPlayer }.count
    }
}

struct Message: Codable, Equatable, Identifiable {
    var id: String { messageId }
    var messageId: String
    var content: String
    var sentDate: GameDate
    var isFromPlayer: Bool
    var isRead: Bool = false
    var priority: MessagePriority = .normal
}

// MARK: - Bank

struct BankTransaction: Codable, Equatable, Identifiable {
    var id: String { transactionId }
    var transactionId: String
    var description: String
    var amount: Double
    var date: GameDate
    var transactionType: TransactionType
    var category: TransactionCategory
    var relatedEventId: String?
    var isPending: Bool = false
    var isCompanyRelated: Bool = false
}

struct BankAccountSummary: Codable, Equatable {
    var currentBalance: Double = 0
    var pendingIncome: Double = 0
    var pendingExpenses: Double = 0
    var salaryEarnings: Double = 0
    var commissionEarnings: Double = 0
    var sideGigEarnings: Double = 0

    func getAvailableFunds() -> Double {
        currentBalance + pendingIncome - pendingExpenses
    }
}

// MARK: - Contacts

struct ContactEntry: Codable, Equatable, Identifiable {
    var id: String { contactId }
    var contactId: String
    var name: String
    var contactType: ContactType
    var vendorCategory: VendorCategory?
    var vendorTier: VendorTier?
    var rating: Double = 0
    var playerNotes: String = ""
    var relationshipLevel: Int = 0
    var reliabilityRevealed: Bool = false
    var flexibilityRevealed: Bool = false
    var timesHired: Int = 0
    var lastHiredDate: GameDate?
    var isFavorite: Bool = false
}

// MARK: - Reviews

struct ReviewEntry: Codable, Equatable, Identifiable {
    var id: String { reviewId }
    var reviewId: String
    var clientName: String
    var eventTitle: String
    var eventId: String
    var eventDate: GameDate
    var satisfactionScore: Double
    var testimonialText: String
    var isPublic: Bool = true
    var isHighlighted: Bool = false
}

struct ReputationSummary: Codable, Equatable {
    var currentReputation: Int = 0
    var totalEventsCompleted: Int = 0
    var averageSatisfaction: Double = 0
    var excellentEvents: Int = 0
    var goodEvents: Int = 0
    var averageEvents: Int = 0
    var poorEvents: Int = 0
    var currentExcellenceStreak: Int = 0
    var bestExcellenceStreak: Int = 0
}

// MARK: - Tasks

struct TaskListEntry: Codable, Equatable {
    var eventId: String
    var eventTitle: String
    var eventDate: GameDate
    var tasks: [TaskItem] = []
    var completedCount: Int = 0
    var totalCount: Int = 0
    var pendingCount: Int = 0
    var failedCount: Int = 0

    func getCompletionPercent() -> Double {
        guard totalCount > 0 else { return 0 }
        return Double(completedCount) / Double(totalCount) * 100.0
    }

    func isComplete() -> Bool {
        completedCount == totalCount && totalCount > 0
    }
}

struct TaskItem: Codable, Equatable {
    var taskId: String
    var taskName: String
    var description: String
    var status: TaskStatus
    var deadline: GameDate
    var hoursRequired: Int
    var isOverdue: Bool = false
    var usedCompanyHelp: Bool = false
}

// MARK: - Clients

struct ClientRecord: Codable, Equatable, Identifiable {
    var id: String { clientId }
    var clientId: String
    var clientName: String
    var personality: ClientPersonality
    var eventHistory: [ClientEventHistory] = []
    var totalEventsPlanned: Int = 0
    var averageSatisfaction: Double = 0
    var isReferralSource: Bool = false
    var referralsGenerated: Int = 0
    var playerNotes: String = ""
    var firstEventDate: GameDate?
    var lastEventDate: GameDate?
}

struct ClientEventHistory: Codable, Equatable {
    var eventId: String
    var eventTitle: String
    var eventTypeId: String
    var eventDate: GameDate
    var satisfactionScore: Double
    var budgetTotal: Double
    var wasReferral: Bool
}
