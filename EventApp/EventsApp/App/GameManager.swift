import Foundation
import Observation

/// Central game state manager. Uses @Observable for SwiftUI reactivity.
/// Game progression is turn-based: the player makes decisions, then taps
/// Advance to jump to the next date with pending activities.
@Observable
class GameManager: GameContext {
    var gameState: GameState = .mainMenu
    var playerData: PlayerData = PlayerData()
    var saveData: SaveData = SaveData()
    var isInitialized: Bool = false

    // MARK: - Gameplay State

    var pendingInquiries: [ClientInquiry] = []
    var activeEvents: [EventData] = []
    var completedEvents: [EventData] = []
    var lastCompletedEvent: EventData?
    var vendorRelationships: [String: VendorRelationship] = [:]
    var transactions: [Transaction] = []

    // MARK: - Systems

    var advanceSystem: AdvanceSystem
    let timeSystem: TimeSystem
    let satisfactionCalculator: SatisfactionCalculator
    let eventPlanningSystem: EventPlanningSystem
    let referralSystem: ReferralSystem
    let consequenceSystem: ConsequenceSystem
    let weatherSystem: WeatherSystem
    let mapSystem: MapSystem
    let phoneSystem: PhoneSystem
    let progressionSystem: ProgressionSystem
    let profitCalculator: ProfitCalculator
    let achievementSystem: AchievementSystem
    let emergencyFundingSystem: EmergencyFundingSystem
    let tutorialSystem: TutorialSystem

    // MARK: - Convenience Accessors

    /// Current game date — views should use this instead of timeSystem.currentDate.
    var currentDate: GameDate { advanceSystem.currentDate }

    /// All activities currently in the player's inbox (ready on today's date).
    var inboxActivities: [PlanningActivity] { advanceSystem.getInboxActivities() }

    /// Email activities only (formal: contracts, quotes, vendor correspondence).
    var emailActivities: [PlanningActivity] {
        inboxActivities.filter { $0.medium == .email }
    }

    /// Message/call activities only (quick: texts, calls, confirmations).
    var messageActivities: [PlanningActivity] {
        inboxActivities.filter { $0.medium == .text || $0.medium == .call || $0.medium == .inPerson }
    }

    /// Whether there are unread inbox items.
    var hasInboxItems: Bool { !inboxActivities.isEmpty }

    /// All activities grouped by contact name for threaded message display.
    /// Includes both active (ready) and completed activities.
    var messageThreads: [ConversationThread] {
        // Only show activities that have actually arrived (not future scheduled)
        let allActivities = advanceSystem.scheduledActivities
            .filter { ($0.status == .ready || $0.status == .completed || $0.status == .overdue)
                      && $0.scheduledDate <= currentDate }

        // Group by contact — resolve "You" sender to the event's client or vendor name
        var threadMap: [String: [PlanningActivity]] = [:]
        for activity in allActivities {
            let contactName = resolveContactName(for: activity)
            threadMap[contactName, default: []].append(activity)
        }

        return threadMap
            .filter { $0.key != "System" } // Exclude system messages
            .map { name, activities in
                let sorted = activities.sorted { $0.scheduledDate < $1.scheduledDate }
                let unread = activities.filter { $0.status == .ready }.count
                return ConversationThread(
                    contactName: name,
                    activities: sorted,
                    unreadCount: unread,
                    latestDate: sorted.last?.scheduledDate ?? currentDate
                )
            }
            .sorted { lhs, rhs in
                if lhs.unreadCount > 0 && rhs.unreadCount == 0 { return true }
                if lhs.unreadCount == 0 && rhs.unreadCount > 0 { return false }
                return lhs.latestDate > rhs.latestDate
            }
    }

    /// Resolve the contact name for threading — "You"-sent activities get
    /// grouped with the client or vendor they belong to, not a "You" thread.
    func resolveContactName(for activity: PlanningActivity) -> String {
        // If clientName is set, use it (client-related activities)
        if let clientName = activity.clientName, !clientName.isEmpty {
            return clientName
        }
        // If it's from a vendor, use the vendor name
        if let vendorId = activity.vendorId, let vendor = SeedData.vendor(byId: vendorId) {
            return vendor.vendorName
        }
        // If sender is "You", look up the event's client name
        if activity.content.senderName == "You" {
            if let event = activeEvents.first(where: { $0.id == activity.eventId }) {
                return event.clientName
            }
        }
        return activity.content.senderName
    }

    init() {
        let startDate = GameDate(month: 3, day: 1, year: 2026)
        advanceSystem = AdvanceSystem(startDate: startDate)
        timeSystem = TimeSystem()
        satisfactionCalculator = SatisfactionCalculator()
        eventPlanningSystem = EventPlanningSystem()
        referralSystem = ReferralSystem()
        consequenceSystem = ConsequenceSystem()
        weatherSystem = WeatherSystem(startDate: startDate)
        mapSystem = MapSystem()
        phoneSystem = PhoneSystem()
        progressionSystem = ProgressionSystem()
        profitCalculator = ProfitCalculator()
        achievementSystem = AchievementSystem()
        emergencyFundingSystem = EmergencyFundingSystem()
        tutorialSystem = TutorialSystem()
    }

    // MARK: - Game State Management

    func startNewGame() {
        playerData = PlayerData()
        saveData = SaveData()
        saveData.playerData = playerData
        saveData.currentDate = GameDate(month: 3, day: 1, year: 2026)
        saveData.journeyStartTime = Date()

        pendingInquiries = []
        activeEvents = []
        completedEvents = []
        lastCompletedEvent = nil
        vendorRelationships = [:]

        advanceSystem = AdvanceSystem(startDate: saveData.currentDate)
        weatherSystem.setCurrentDate(saveData.currentDate)
        weatherSystem.regenerateForecasts()
        achievementSystem.resetAll()

        // Register seed data
        for venue in SeedData.venues {
            mapSystem.registerVenue(venue)
            mapSystem.registerLocation(
                LocationData.fromVenue(venue, mapPosition: CGPoint(x: Double.random(in: 0.1...0.9), y: Double.random(in: 0.1...0.9)))
            )
        }
        for vendor in SeedData.vendors {
            mapSystem.registerVendor(vendor)
            mapSystem.registerLocation(
                LocationData.fromVendor(vendor, mapPosition: CGPoint(x: Double.random(in: 0.1...0.9), y: Double.random(in: 0.1...0.9)))
            )
        }

        // Schedule the first inquiry
        advanceSystem.scheduleNextInquiry(stage: playerData.stageNumber, reputation: playerData.reputation)

        // Start tutorial
        startTutorial()
        gameState = .tutorial
        isInitialized = true
    }

    func continueGame() {
        gameState = .playing
    }

    func pauseGame() {
        gameState = .paused
    }

    func resumeGame() {
        gameState = .playing
    }

    func showSettings() {
        gameState = .settings
    }

    func returnToMainMenu() {
        gameState = .mainMenu
    }

    // MARK: - Turn-Based Advancement

    /// Called when the player taps the Advance button.
    /// Finds the next decision point, advances time, and presents inbox.
    func advanceToNextPoint() {
        // Check for overdue activities before advancing
        let overdueWarnings = advanceSystem.processOverdueActivities()
        processOverdueVendorImpacts(overdueWarnings)

        guard let nextPoint = advanceSystem.findNextDecisionPoint() else {
            // Nothing scheduled — generate an inquiry and try again
            advanceSystem.scheduleNextInquiry(stage: playerData.stageNumber, reputation: playerData.reputation)
            guard let fallbackPoint = advanceSystem.findNextDecisionPoint() else { return }
            performAdvance(to: fallbackPoint)
            return
        }

        performAdvance(to: nextPoint)
    }

    private func performAdvance(to point: DecisionPoint) {
        let previousDate = advanceSystem.currentDate

        // Advance — this marks activities as ready and flags overdue ones
        let overdueActivities = advanceSystem.advance(to: point)
        processOverdueVendorImpacts(overdueActivities)

        // Process each day that was skipped for weather updates
        var walkDate = previousDate
        while walkDate < point.date {
            walkDate = walkDate.adding(days: 1)
            weatherSystem.advanceDay(to: walkDate)
        }

        saveData.currentDate = advanceSystem.currentDate

        // Check if today is an inquiry day
        // Don't generate new inquiries until at least one event has been completed
        let hasCompletedFirstEvent = !completedEvents.isEmpty
        if hasCompletedFirstEvent,
           let inquiryDate = advanceSystem.nextScheduledInquiryDate,
           inquiryDate <= advanceSystem.currentDate {
            generateNewInquiry()
            advanceSystem.scheduleNextInquiry(stage: playerData.stageNumber, reputation: playerData.reputation)
        }

        // Process event phases for the new date
        processEventPhases(currentDate: advanceSystem.currentDate)
    }

    private func processOverdueVendorImpacts(_ overdueActivities: [PlanningActivity]) {
        for activity in overdueActivities {
            guard let vendorId = activity.vendorId else { continue }
            if vendorRelationships[vendorId] != nil {
                vendorRelationships[vendorId]?.recordMissedDeadline()
            }
        }
    }

    // MARK: - Event Phase Processing

    private func processEventPhases(currentDate: GameDate) {
        for i in stride(from: activeEvents.count - 1, through: 0, by: -1) {
            guard let acceptedDate = activeEvents[i].acceptedDate else { continue }

            let schedule = EventPhaseCalculator.calculatePhaseSchedule(
                acceptedDate: acceptedDate,
                eventDate: activeEvents[i].eventDate
            )
            let phaseInfo = EventPhaseCalculator.getCurrentPhase(
                currentDate: currentDate,
                schedule: schedule
            )

            activeEvents[i].phase = phaseInfo.phase

            // Execution day — or we've passed it
            if (phaseInfo.phase == .executionDay || currentDate >= activeEvents[i].eventDate)
                && activeEvents[i].status != .executing && activeEvents[i].status != .completed {
                activeEvents[i].status = .executing
                executeEvent(at: i)
            }

            // Results phase — day after event or later
            if currentDate > activeEvents[i].eventDate && activeEvents[i].status == .executing {
                completeEvent(at: i)
            }
        }
    }

    // MARK: - Event Execution

    private func executeEvent(at index: Int) {
        let event = activeEvents[index]

        let venueScore = calculateVenueScore(for: event)
        let foodScore = calculateVendorCategoryScore(for: event, category: .caterer)
        let entertainmentScore = calculateVendorCategoryScore(for: event, category: .entertainer)
        let decorationScore = calculateVendorCategoryScore(for: event, category: .decorator)
        let serviceScore = 65.0
        let expectationScore = calculateExpectationScore(for: event)

        let randomEvents = consequenceSystem.evaluateRandomEvents(event: event, stage: playerData.stageNumber)
        let randomModifier = 1.0 + (consequenceSystem.calculateRandomEventModifier(results: randomEvents) / 100.0)

        var results = EventResults()
        results.venueScore = venueScore
        results.foodScore = foodScore
        results.entertainmentScore = entertainmentScore
        results.decorationScore = decorationScore
        results.serviceScore = serviceScore
        results.expectationScore = expectationScore
        results.randomEventModifier = randomModifier
        results.randomEventsOccurred = randomEvents.map { $0.eventDescription }

        activeEvents[index].results = results

        let client = ClientData.fromEvent(activeEvents[index])
        let satResult = satisfactionCalculator.calculate(event: activeEvents[index], client: client)
        activeEvents[index].results?.finalSatisfaction = satResult.finalSatisfaction

        let profitResult = profitCalculator.calculateProfit(
            revenue: event.budget.total,
            costs: satResult.finalSatisfaction
        )
        activeEvents[index].results?.profit = profitResult.netProfit
    }

    private func completeEvent(at index: Int) {
        var event = activeEvents[index]
        event.status = .completed

        let satisfaction = event.results?.finalSatisfaction ?? 50
        let profit = event.results?.profit ?? 0

        let repChange = progressionSystem.applyEventResult(
            satisfaction: satisfaction,
            currentReputation: playerData.reputation,
            stage: playerData.stageNumber
        )
        playerData.reputation = repChange.newReputation
        event.results?.reputationChange = repChange.change

        playerData.money += profit
        if profit >= 0 {
            transactions.append(.income(date: advanceSystem.currentDate, amount: profit, description: "Profit — \(event.eventTitle)", category: .eventProfit))
        } else {
            transactions.append(.expense(date: advanceSystem.currentDate, amount: abs(profit), description: "Loss — \(event.eventTitle)", category: .eventLoss))
        }

        event.results?.clientFeedback = generateFeedback(satisfaction: satisfaction, tier: repChange.satisfactionTier)

        let referralResult = referralSystem.evaluateReferral(
            satisfaction: satisfaction,
            excellenceStreak: saveData.excellenceStreak
        )
        event.results?.triggeredReferral = referralResult.wasReferred

        let streakResult = referralSystem.updateExcellenceStreak(
            currentStreak: saveData.excellenceStreak,
            satisfaction: satisfaction
        )
        saveData.excellenceStreak = streakResult.newStreak

        // Mark vendor bookings as completed
        for assignment in event.vendors {
            if vendorRelationships[assignment.vendorId] != nil {
                vendorRelationships[assignment.vendorId]?.recordCompletedBooking()
            }
        }

        // Cancel any remaining scheduled activities for this event
        advanceSystem.cancelActivitiesForEvent(eventId: event.id)

        activeEvents.remove(at: index)
        completedEvents.append(event)
        lastCompletedEvent = event

        playerData.completedEventIds.append(event.id)
        playerData.activeEventIds.removeAll { $0 == event.id }

        if tutorialSystem.isTutorialActive && tutorialSystem.currentStep == .eventExecution {
            advanceTutorial()
        }
    }

    // MARK: - Score Calculation Helpers

    private func calculateVenueScore(for event: EventData) -> Double {
        guard let venueId = event.venueId,
              let venue = SeedData.venue(byId: venueId) else { return 30.0 }

        var score = venue.ambianceRating

        if event.guestCount > venue.capacityComfortable {
            let overPercent = Double(event.guestCount - venue.capacityComfortable)
                / Double(max(1, venue.capacityMax - venue.capacityComfortable))
            score -= overPercent * 20
        }

        if venue.weatherDependent {
            let risk = weatherSystem.getSimplifiedRisk(for: event.eventDate)
            switch risk {
            case .bad: score -= 25
            case .risky: score -= 10
            case .good: break
            }
        }

        return max(0, min(100, score))
    }

    private func calculateVendorCategoryScore(for event: EventData, category: VendorCategory) -> Double {
        guard let assignment = event.vendors.first(where: { $0.category == category }),
              let vendor = SeedData.vendor(byId: assignment.vendorId) else {
            return 40.0
        }
        return vendor.qualityRating
    }

    private func calculateExpectationScore(for event: EventData) -> Double {
        let budgetUsed = event.budget.spent
        let budgetTotal = event.budget.total
        guard budgetTotal > 0 else { return 50 }
        let ratio = budgetUsed / budgetTotal
        if ratio <= 1.0 {
            return 70 + (1.0 - ratio) * 30
        } else {
            return max(20, 70 - (ratio - 1.0) * 100)
        }
    }

    private func generateFeedback(satisfaction: Double, tier: String) -> String {
        switch tier {
        case "Excellent": return "Amazing event! Everything was perfect. We'll definitely recommend you!"
        case "Good": return "Great job! The event went really well. Thank you!"
        case "Okay": return "The event was fine, but there's room for improvement."
        case "Poor": return "We were disappointed with some aspects of the event."
        default: return "The event did not meet our expectations at all."
        }
    }

    // MARK: - Inquiry Management

    private func generateNewInquiry() {
        let workload = eventPlanningSystem.getWorkloadStatus(
            activeEvents: activeEvents.count,
            stage: playerData.stageNumber
        )
        guard workload != .critical else { return }

        let inquiry = eventPlanningSystem.generateInquiry(
            stage: playerData.stageNumber,
            reputation: playerData.reputation,
            currentDate: advanceSystem.currentDate
        )
        pendingInquiries.append(inquiry)
    }

    func acceptInquiry(_ inquiry: ClientInquiry) {
        let event = eventPlanningSystem.acceptInquiry(inquiry, currentDate: advanceSystem.currentDate)
        activeEvents.append(event)
        playerData.activeEventIds.append(event.id)
        pendingInquiries.removeAll { $0.inquiryId == inquiry.inquiryId }

        // Schedule the client meeting (1-3 days after accepting)
        let meetingDaysOffset = Int.random(in: 1...3)
        let meetingDate = advanceSystem.currentDate.adding(days: meetingDaysOffset)

        let transcript = generateClientMeetingTranscript(event: event)
        let requirements = deriveRequirementsFromEvent(event: event)

        let meetingActivity = PlanningActivity.create(
            eventId: event.id,
            clientName: event.clientName,
            type: .clientMeeting,
            medium: .call,
            scheduledDate: meetingDate,
            content: ActivityContent(
                senderName: event.clientName,
                subject: "Call with \(event.clientName) — \(event.subCategory)",
                body: "",
                dialogueTranscript: transcript,
                revealedRequirements: requirements
            )
        )
        advanceSystem.scheduleActivity(meetingActivity)

        // Schedule event execution day as a decision point
        let eventDayActivity = PlanningActivity.create(
            eventId: event.id,
            clientName: event.clientName,
            type: .eventExecution,
            medium: .text,
            scheduledDate: event.eventDate,
            content: ActivityContent(
                senderName: "System",
                subject: "Event Day — \(event.eventTitle)",
                body: "Today is the day! \(event.eventTitle) is happening."
            )
        )
        advanceSystem.scheduleActivity(eventDayActivity)

        // Schedule results day (day after event)
        let resultsDayActivity = PlanningActivity.create(
            eventId: event.id,
            clientName: event.clientName,
            type: .eventResults,
            medium: .email,
            scheduledDate: event.eventDate.adding(days: 1),
            content: ActivityContent(
                senderName: event.clientName,
                subject: "How did it go? — \(event.eventTitle)",
                body: ""  // Populated when event completes
            )
        )
        advanceSystem.scheduleActivity(resultsDayActivity)

        if tutorialSystem.isTutorialActive && tutorialSystem.currentStep == .acceptClient {
            advanceTutorial()
        }
    }

    func declineInquiry(_ inquiry: ClientInquiry) {
        eventPlanningSystem.declineInquiry(inquiry)
        pendingInquiries.removeAll { $0.inquiryId == inquiry.inquiryId }
    }

    // MARK: - Vendor Process Initiation

    /// Player initiates contact with a vendor for an event.
    /// Schedules the availability check and response activities.
    func initiateVendorContact(eventId: String, vendor: VendorData) {
        if vendorRelationships[vendor.id] == nil {
            vendorRelationships[vendor.id] = VendorRelationship.createNew(vendorId: vendor.id)
        }

        guard let relationship = vendorRelationships[vendor.id],
              relationship.willAcceptBooking else { return }

        let eventTitle = activeEvents.first(where: { $0.id == eventId })?.eventTitle ?? "your event"

        vendorRelationships[vendor.id]?.recordBookingStarted()

        // Step 1: Availability request (immediate)
        let requestActivity = PlanningActivity.create(
            eventId: eventId,
            vendorId: vendor.id,
            vendorCategory: vendor.category,
            type: .vendorAvailabilityRequest,
            medium: .email,
            scheduledDate: advanceSystem.currentDate,
            content: ActivityContent(
                senderName: "You",
                subject: "Availability inquiry — \(vendor.vendorName) for \(eventTitle)",
                body: "Sent availability request to \(vendor.vendorName) for \(eventTitle)."
            )
        )
        advanceSystem.scheduleActivity(requestActivity)
        advanceSystem.completeActivity(id: requestActivity.id)

        // Step 2: Availability response (1-2 days)
        let responseSpeedBonus = relationship.responseSpeedBonus
        let responseDays = max(1, Int.random(in: 1...2) - responseSpeedBonus)
        let responseDate = advanceSystem.currentDate.adding(days: responseDays)

        let responseActivity = PlanningActivity.create(
            eventId: eventId,
            vendorId: vendor.id,
            vendorCategory: vendor.category,
            type: .vendorAvailabilityResponse,
            medium: .email,
            scheduledDate: responseDate,
            responseDeadline: responseDate.adding(days: 3),
            content: ActivityContent(
                senderName: vendor.vendorName,
                subject: "Re: Availability for \(eventTitle)",
                body: "Hi! Thanks for reaching out about \(eventTitle). I checked my calendar and I'm available on the event date. I'd be happy to put together a quote for you."
            )
        )
        advanceSystem.scheduleActivity(responseActivity)
    }

    /// Player sends a negotiation offer to a vendor.
    func sendNegotiationOffer(activityId: String, eventId: String, vendor: VendorData, offerAmount: Double) {
        advanceSystem.completeActivity(id: activityId)

        guard let relationship = vendorRelationships[vendor.id] else { return }

        // Record the offer
        let offerActivity = PlanningActivity.create(
            eventId: eventId,
            vendorId: vendor.id,
            vendorCategory: vendor.category,
            type: .vendorNegotiationOffer,
            medium: .email,
            scheduledDate: advanceSystem.currentDate,
            content: ActivityContent(
                senderName: "You",
                subject: "Counter offer — \(vendor.vendorName)",
                body: "You offered $\(Int(offerAmount)) for \(vendor.vendorName)'s services.",
                counterOfferAmount: offerAmount
            )
        )
        advanceSystem.scheduleActivity(offerActivity)
        advanceSystem.completeActivity(id: offerActivity.id)

        // Vendor responds in 1 day
        let responseDate = advanceSystem.currentDate.adding(days: 1)
        let negotiationRound = (getCurrentNegotiationRound(eventId: eventId, vendorId: vendor.id)) + 1

        // Determine vendor's response based on relationship and offer
        let flexibility = relationship.pricingFlexibility
        let minAcceptable = vendor.basePrice * (1.0 - flexibility)
        let eventTitle = activeEvents.first(where: { $0.id == eventId })?.eventTitle ?? "your event"

        let responseBody: String
        let responseSubject: String
        let responseQuote: Double?

        if offerAmount >= minAcceptable {
            // Vendor accepts
            responseSubject = "Re: Your offer for \(eventTitle) — Accepted!"
            responseBody = "Thanks for the offer. $\(Int(offerAmount)) works for me for \(eventTitle). I'll confirm the booking once you give the go-ahead."
            responseQuote = offerAmount
        } else if negotiationRound < 2 {
            // Vendor counters
            let counterAmount = ((offerAmount + vendor.basePrice) / 2).rounded()
            responseSubject = "Re: Your offer for \(eventTitle) — Counter"
            responseBody = "I appreciate the offer, but $\(Int(offerAmount)) is a bit low for \(eventTitle). I could do $\(Int(counterAmount)) — that's the best I can offer. Let me know."
            responseQuote = counterAmount
        } else {
            // Vendor walks away
            responseSubject = "Re: Your offer for \(eventTitle) — Can't do it"
            responseBody = "Sorry, I can't go that low for \(eventTitle). I wish you the best finding someone in your budget. Feel free to reach out for future events."
            responseQuote = nil
        }

        let responseActivity = PlanningActivity.create(
            eventId: eventId,
            vendorId: vendor.id,
            vendorCategory: vendor.category,
            type: .vendorNegotiationResponse,
            medium: .email,
            scheduledDate: responseDate,
            responseDeadline: responseDate.adding(days: 3),
            content: ActivityContent(
                senderName: vendor.vendorName,
                subject: responseSubject,
                body: responseBody,
                quoteAmount: responseQuote,
                negotiationRound: negotiationRound
            )
        )
        advanceSystem.scheduleActivity(responseActivity)

        vendorRelationships[vendor.id]?.recordNegotiation(successful: offerAmount >= minAcceptable)
    }

    private func getCurrentNegotiationRound(eventId: String, vendorId: String) -> Int {
        advanceSystem.getActivitiesForEvent(eventId: eventId)
            .filter { $0.vendorId == vendorId && $0.type == .vendorNegotiationResponse }
            .count
    }

    // MARK: - Venue & Vendor Assignment (Legacy — retained for compatibility)

    func assignVenue(eventIndex: Int, venue: VenueData) -> BookingResult {
        guard eventIndex >= 0 && eventIndex < activeEvents.count else {
            return .failed("Invalid event.")
        }

        let bookedDates = saveData.getVenueBookedDates(venue.id)
        let result = eventPlanningSystem.bookVenue(
            event: activeEvents[eventIndex],
            venue: venue,
            dates: bookedDates
        )

        if result.success {
            activeEvents[eventIndex].venueId = venue.id
            activeEvents[eventIndex].budget.spent += result.price
            saveData.addVenueBooking(venue.id, date: activeEvents[eventIndex].eventDate)

            if tutorialSystem.isTutorialActive && tutorialSystem.currentStep == .selectVenue {
                advanceTutorial()
            }
        }

        return result
    }

    func assignVendor(eventIndex: Int, vendor: VendorData) -> BookingResult {
        guard eventIndex >= 0 && eventIndex < activeEvents.count else {
            return .failed("Invalid event.")
        }

        let bookedDates = saveData.getVendorBookedDates(vendor.id)
        let result = eventPlanningSystem.bookVendor(
            event: activeEvents[eventIndex],
            vendor: vendor,
            dates: bookedDates
        )

        if result.success {
            let assignment = VendorAssignment(
                vendorId: vendor.id,
                category: vendor.category,
                agreedPrice: result.price,
                isConfirmed: true,
                bookingDate: advanceSystem.currentDate
            )
            activeEvents[eventIndex].vendors.append(assignment)
            activeEvents[eventIndex].budget.spent += result.price
            saveData.addVendorBooking(vendor.id, date: activeEvents[eventIndex].eventDate)

            if tutorialSystem.isTutorialActive && tutorialSystem.currentStep == .selectCaterer && vendor.category == .caterer {
                advanceTutorial()
            }
        }

        return result
    }

    // MARK: - Activity Completion

    /// Player completes/acknowledges an inbox activity.
    /// Triggers follow-up activities based on the activity type.
    func completeActivity(_ activityId: String) {
        guard let activity = advanceSystem.scheduledActivities.first(where: { $0.id == activityId }) else {
            advanceSystem.completeActivity(id: activityId)
            return
        }

        advanceSystem.completeActivity(id: activityId)

        switch activity.type {
        case .clientMeeting:
            onClientMeetingCompleted(activity)
        case .clientContractSent:
            onClientContractSent(activity)
        case .clientContractSigned:
            onClientContractSigned(activity)
        case .clientDepositReceived:
            onClientDepositAcknowledged(activity)
        case .vendorAvailabilityResponse:
            onVendorAvailabilityReceived(activity)
        case .vendorOptionsReview:
            // Quote reviewed — handled by accept/negotiate actions in UI
            break
        default:
            break
        }
    }

    /// After client meeting: schedule the contract with event details for review.
    private func onClientMeetingCompleted(_ activity: PlanningActivity) {
        guard let event = activeEvents.first(where: { $0.id == activity.eventId }) else { return }

        let contractDate = advanceSystem.currentDate.adding(days: 1)
        let depositAmount = event.budget.total * 0.25
        let eventName = event.subCategory

        let contractBody = """
        Draft contract ready for your review before sending to \(event.clientName).

        — EVENT PLANNING CONTRACT —

        Client: \(event.clientName)
        Event: \(eventName)
        Date: \(event.eventDate.formatted)
        Guests: \(event.guestCount)
        Total Budget: $\(Int(event.budget.total))
        Deposit (25%): $\(Int(depositAmount))

        Services included:
        • Event planning and coordination
        • Venue sourcing and booking
        • Vendor management (catering, entertainment, etc.)
        • Day-of event oversight

        Payment terms:
        • 25% deposit due upon signing ($\(Int(depositAmount)))
        • Remaining balance due 5 days before the event

        Review the details above, then tap Send to Client.
        """

        let contractActivity = PlanningActivity.create(
            eventId: activity.eventId,
            clientName: activity.clientName,
            type: .clientContractSent,
            medium: .email,
            scheduledDate: contractDate,
            responseDeadline: contractDate.adding(days: 3),
            content: ActivityContent(
                senderName: "You",
                subject: "Contract — \(event.clientName), \(eventName)",
                body: contractBody,
                contractAmount: event.budget.total
            )
        )
        advanceSystem.scheduleActivity(contractActivity)
    }

    /// Player sends contract with their chosen service fee.
    /// The client may accept or negotiate based on personality.
    func sendContractWithFee(activityId: String, serviceFeePercent: Double) {
        guard let activity = advanceSystem.scheduledActivities.first(where: { $0.id == activityId }),
              let eventIndex = activeEvents.firstIndex(where: { $0.id == activity.eventId }) else { return }

        let event = activeEvents[eventIndex]
        let serviceFee = event.budget.total * (serviceFeePercent / 100)

        activeEvents[eventIndex].serviceFeePercent = serviceFeePercent
        activeEvents[eventIndex].serviceFee = serviceFee

        advanceSystem.completeActivity(id: activityId)

        // Client response depends on personality and fee level
        let signDays = Int.random(in: 1...2)
        let signDate = advanceSystem.currentDate.adding(days: signDays)

        let willNegotiate = shouldClientNegotiateFee(personality: event.personality, feePercent: serviceFeePercent)

        if willNegotiate && event.negotiationRoundsUsed < 2 {
            // Client pushes back
            let counterPercent = max(5, serviceFeePercent - Double.random(in: 3...8))
            let counterFee = event.budget.total * (counterPercent / 100)

            let pushbackActivity = PlanningActivity.create(
                eventId: event.id,
                clientName: event.clientName,
                type: .clientContractSent, // Reuse type — it's another contract draft
                medium: .email,
                scheduledDate: signDate,
                responseDeadline: signDate.adding(days: 3),
                content: ActivityContent(
                    senderName: event.clientName,
                    subject: "Re: Contract — fee discussion",
                    body: clientNegotiationResponse(personality: event.personality, originalFee: serviceFee, counterFee: counterFee, feePercent: serviceFeePercent, counterPercent: counterPercent),
                    counterOfferAmount: counterPercent,
                    contractAmount: event.budget.total
                )
            )
            advanceSystem.scheduleActivity(pushbackActivity)
            activeEvents[eventIndex].negotiationRoundsUsed += 1
        } else {
            // Client accepts
            onClientContractSent(activity)
        }
    }

    private func shouldClientNegotiateFee(personality: ClientPersonality, feePercent: Double) -> Bool {
        switch personality {
        case .easyGoing:
            return feePercent > 20 // Only pushes back on high fees
        case .budgetConscious:
            return feePercent > 10 // Very sensitive to fees
        case .perfectionist:
            return feePercent > 25 // Expects to pay for quality
        case .demanding:
            return feePercent > 15 // Negotiates everything
        case .indecisive:
            return false // Doesn't negotiate, accepts whatever
        case .celebrity:
            return false // Money isn't the issue
        }
    }

    private func clientNegotiationResponse(personality: ClientPersonality, originalFee: Double, counterFee: Double, feePercent: Double, counterPercent: Double) -> String {
        let response: String
        switch personality {
        case .budgetConscious:
            response = "I appreciate the detailed contract, but the \(Int(feePercent))% service fee is higher than I was expecting. Would you consider \(Int(counterPercent))% ($\(Int(counterFee)))? That would help us stay within our budget."
        case .demanding:
            response = "The contract looks good, but I've worked with planners who charge less. I think \(Int(counterPercent))% ($\(Int(counterFee))) is more in line with what I'd expect. Can we make that work?"
        case .easyGoing:
            response = "Hey, the contract looks great overall! Just wondering if there's any flexibility on the fee? Something around \(Int(counterPercent))% would be easier for us."
        default:
            response = "Thanks for sending this over. Could we discuss the service fee? I was thinking something closer to \(Int(counterPercent))% ($\(Int(counterFee))) might be more reasonable."
        }
        return response
    }

    /// After contract sent: client signs in 1-2 days.
    private func onClientContractSent(_ activity: PlanningActivity) {
        let signDays = Int.random(in: 1...2)
        let signDate = advanceSystem.currentDate.adding(days: signDays)

        let signedActivity = PlanningActivity.create(
            eventId: activity.eventId,
            clientName: activity.clientName,
            type: .clientContractSigned,
            medium: .email,
            scheduledDate: signDate,
            content: ActivityContent(
                senderName: activity.content.senderName,
                subject: "Contract signed!",
                body: "\(activity.content.senderName) has reviewed and signed the contract. They're ready to proceed with a deposit."
            )
        )
        advanceSystem.scheduleActivity(signedActivity)
    }

    /// After contract signed: schedule deposit notification and enable vendor planning.
    private func onClientContractSigned(_ activity: PlanningActivity) {
        guard let eventIndex = activeEvents.firstIndex(where: { $0.id == activity.eventId }) else { return }
        let event = activeEvents[eventIndex]

        let depositAmount = event.budget.total * 0.25

        let depositActivity = PlanningActivity.create(
            eventId: activity.eventId,
            clientName: activity.clientName,
            type: .clientDepositReceived,
            medium: .email,
            scheduledDate: advanceSystem.currentDate,
            content: ActivityContent(
                senderName: activity.content.senderName,
                subject: "Deposit received — $\(Int(depositAmount))",
                body: "\(activity.content.senderName) has signed the contract and paid a 25% deposit of $\(Int(depositAmount)). You can now begin booking vendors.",
                depositAmount: depositAmount
            )
        )
        advanceSystem.scheduleActivity(depositActivity)
    }

    /// After player acknowledges deposit: money arrives, event enters planning.
    private func onClientDepositAcknowledged(_ activity: PlanningActivity) {
        guard let eventIndex = activeEvents.firstIndex(where: { $0.id == activity.eventId }) else { return }

        let depositAmount = activity.content.depositAmount ?? (activeEvents[eventIndex].budget.total * 0.25)
        playerData.money += depositAmount
        transactions.append(.income(date: advanceSystem.currentDate, amount: depositAmount, description: "Deposit — \(activeEvents[eventIndex].clientName)", category: .clientDeposit))

        activeEvents[eventIndex].status = .planning
    }

    /// Player accepts a vendor quote — books the vendor for the event.
    func acceptVendorQuote(activityId: String) {
        guard let activity = advanceSystem.scheduledActivities.first(where: { $0.id == activityId }),
              let vendorId = activity.vendorId,
              let vendor = SeedData.vendor(byId: vendorId),
              let eventIndex = activeEvents.firstIndex(where: { $0.id == activity.eventId }) else { return }

        let price = activity.content.quoteAmount ?? vendor.basePrice

        // Book the vendor
        let assignment = VendorAssignment(
            vendorId: vendor.id,
            category: vendor.category,
            agreedPrice: price,
            isConfirmed: true,
            bookingDate: advanceSystem.currentDate
        )
        activeEvents[eventIndex].vendors.append(assignment)
        activeEvents[eventIndex].budget.spent += price
        saveData.addVendorBooking(vendor.id, date: activeEvents[eventIndex].eventDate)

        // Log transaction
        transactions.append(.expense(date: advanceSystem.currentDate, amount: price, description: "Vendor — \(vendor.vendorName)", category: .vendorPayment))
        playerData.money -= price

        // Mark quote as completed
        advanceSystem.completeActivity(id: activityId)

        // Send confirmation message
        let confirmActivity = PlanningActivity.create(
            eventId: activity.eventId,
            vendorId: vendorId,
            vendorCategory: activity.vendorCategory,
            type: .vendorContractSent,
            medium: .email,
            scheduledDate: advanceSystem.currentDate,
            content: ActivityContent(
                senderName: "You",
                subject: "Booking confirmed — \(vendor.vendorName)",
                body: "Booked \(vendor.vendorName) for $\(Int(price)). They're confirmed for your event."
            )
        )
        advanceSystem.scheduleActivity(confirmActivity)
        advanceSystem.completeActivity(id: confirmActivity.id)
    }

    /// After vendor confirms availability: schedule a quote.
    private func onVendorAvailabilityReceived(_ activity: PlanningActivity) {
        guard let vendorId = activity.vendorId,
              let vendor = SeedData.vendor(byId: vendorId) else { return }

        let eventTitle = activeEvents.first(where: { $0.id == activity.eventId })?.eventTitle ?? "your event"

        // Vendor sends a quote 1 day after availability confirmation
        let quoteDate = advanceSystem.currentDate.adding(days: 1)

        let quoteActivity = PlanningActivity.create(
            eventId: activity.eventId,
            vendorId: vendorId,
            vendorCategory: activity.vendorCategory,
            type: .vendorOptionsReview,
            medium: .email,
            scheduledDate: quoteDate,
            responseDeadline: quoteDate.adding(days: 5),
            content: ActivityContent(
                senderName: vendor.vendorName,
                subject: "Quote — \(eventTitle)",
                body: "Thanks for reaching out about \(eventTitle)! Here's my pricing:\n\nBase rate: $\(Int(vendor.basePrice))\nSpecialty: \(vendor.specialty)\n\nLet me know if you'd like to proceed, or if you'd like to discuss pricing.",
                quoteAmount: vendor.basePrice
            )
        )
        advanceSystem.scheduleActivity(quoteActivity)
    }

    // MARK: - Tutorial

    private func startTutorial() {
        tutorialSystem.startTutorial()
        let inquiry = eventPlanningSystem.generateInquiry(
            stage: 1,
            reputation: 0,
            currentDate: advanceSystem.currentDate
        )
        pendingInquiries.append(inquiry)
    }

    func advanceTutorial() {
        tutorialSystem.advanceStep()

        // Once the player reaches the "advance to event day" step,
        // or completes the tutorial, switch to normal play mode
        // so they can use the Advance button freely.
        if tutorialSystem.currentStep == .eventExecution || tutorialSystem.isTutorialComplete {
            gameState = .playing
        }
    }

    func skipTutorial() {
        tutorialSystem.skipTutorial()
        gameState = .playing
    }

    func dismissResults() {
        lastCompletedEvent = nil

        if tutorialSystem.isTutorialActive && tutorialSystem.currentStep == .viewResults {
            advanceTutorial()
        }
    }

    // MARK: - Client Meeting Dialogue Generation

    /// Generates a conversation transcript for a client meeting.
    /// The client drops hints about their personality, budget, and requirements.
    /// The player reads this as their "notepad" for later vendor decisions.
    private func generateClientMeetingTranscript(event: EventData) -> [DialogueLine] {
        var lines: [DialogueLine] = []

        // Opening — player greets
        lines.append(DialogueLine(speaker: .player, text: "Thanks for taking the time to chat, \(event.clientName). Tell me about your \(event.subCategory)!"))

        // Client describes the event — personality colors the tone
        lines.append(contentsOf: eventDescriptionLines(event: event))

        // Budget signals — personality determines how they talk about money
        lines.append(contentsOf: budgetSignalLines(event: event))

        // Guest count and venue hints
        lines.append(contentsOf: venueHintLines(event: event))

        // Vendor requirements — what they care about
        lines.append(contentsOf: vendorRequirementLines(event: event))

        // Closing
        lines.append(DialogueLine(speaker: .player, text: "I've got a great picture of what you're looking for. I'll put together some options and send over a contract."))
        lines.append(contentsOf: closingLines(event: event))

        return lines
    }

    private func eventDescriptionLines(event: EventData) -> [DialogueLine] {
        switch event.personality {
        case .easyGoing:
            return [
                DialogueLine(speaker: .client, text: "We're really looking forward to it! Honestly, as long as everyone has a good time, I'm happy."),
                DialogueLine(speaker: .client, text: "We're pretty flexible on most things — just want it to feel fun and relaxed.")
            ]
        case .budgetConscious:
            return [
                DialogueLine(speaker: .client, text: "I've been planning this for a while. I want it to be nice, but I need to be realistic about what we can afford."),
                DialogueLine(speaker: .client, text: "I've done some research on prices and I have a pretty firm idea of what we should be spending.")
            ]
        case .perfectionist:
            return [
                DialogueLine(speaker: .client, text: "I have a very specific vision for this. I've put together a Pinterest board and I know exactly what I want."),
                DialogueLine(speaker: .client, text: "The details really matter to me. I'd rather spend more and get it right than cut corners.")
            ]
        case .demanding:
            return [
                DialogueLine(speaker: .client, text: "I expect this to be top-notch. I've been to a lot of events and I know what good looks like."),
                DialogueLine(speaker: .client, text: "I need you to be on top of every detail. I'll be checking in frequently.")
            ]
        case .indecisive:
            return [
                DialogueLine(speaker: .client, text: "I have a few ideas but honestly I keep going back and forth. There are so many options!"),
                DialogueLine(speaker: .client, text: "Maybe you can help me narrow things down? I trust your expertise.")
            ]
        case .celebrity:
            return [
                DialogueLine(speaker: .client, text: "This needs to be memorable. People will be talking about this, if you know what I mean."),
                DialogueLine(speaker: .client, text: "I need discretion, but also... it should look incredible. No compromises.")
            ]
        }
    }

    private func budgetSignalLines(event: EventData) -> [DialogueLine] {
        let budget = event.budget.total

        lines_player()

        switch event.personality {
        case .easyGoing:
            return [
                DialogueLine(speaker: .player, text: "What kind of budget are you working with?"),
                DialogueLine(speaker: .client, text: "We've got around $\(budgetHint(budget)) set aside. If we go a little over, it's not the end of the world.")
            ]
        case .budgetConscious:
            return [
                DialogueLine(speaker: .player, text: "Let's talk budget — what range are you comfortable with?"),
                DialogueLine(speaker: .client, text: "I need to keep it under $\(budgetCeiling(budget)). That's a hard number for us. Every dollar counts.")
            ]
        case .perfectionist:
            return [
                DialogueLine(speaker: .player, text: "Do you have a budget in mind?"),
                DialogueLine(speaker: .client, text: "I'm willing to invest what it takes to get the quality I want. I'm thinking around $\(budgetHint(budget)), but I'd go higher for the right vendors.")
            ]
        case .demanding:
            return [
                DialogueLine(speaker: .player, text: "What's your budget looking like?"),
                DialogueLine(speaker: .client, text: "Money isn't my main concern — results are. But let's keep it reasonable. I'd say around $\(budgetHint(budget)).")
            ]
        case .indecisive:
            return [
                DialogueLine(speaker: .player, text: "Have you thought about budget?"),
                DialogueLine(speaker: .client, text: "Hmm, I'm not totally sure. Maybe $\(budgetLow(budget))? Or $\(budgetHigh(budget))? What do most people spend on something like this?")
            ]
        case .celebrity:
            return [
                DialogueLine(speaker: .player, text: "What should I plan around budget-wise?"),
                DialogueLine(speaker: .client, text: "I'll leave that to you. Just make sure it's done right. I'd expect something in the $\(budgetHint(budget)) range, minimum.")
            ]
        }
    }

    private func venueHintLines(event: EventData) -> [DialogueLine] {
        let guests = event.guestCount
        return [
            DialogueLine(speaker: .player, text: "How many guests are you expecting?"),
            DialogueLine(speaker: .client, text: "We're looking at about \(guests) people. Maybe a few more if some plus-ones come through.")
        ]
    }

    private func vendorRequirementLines(event: EventData) -> [DialogueLine] {
        // Stage 1 events need venue + caterer
        var lines: [DialogueLine] = []

        lines.append(DialogueLine(speaker: .player, text: "What's most important to you for the event?"))

        switch event.personality {
        case .easyGoing:
            lines.append(DialogueLine(speaker: .client, text: "Good food is a must — people always remember the food. The venue just needs to fit everyone comfortably."))
        case .budgetConscious:
            lines.append(DialogueLine(speaker: .client, text: "Food is the priority. I'd rather have great food in a simple space than fancy decor with mediocre catering."))
        case .perfectionist:
            lines.append(DialogueLine(speaker: .client, text: "Everything. But if I had to pick — the venue sets the tone, and the food has to match the quality. I don't want a beautiful space with disappointing catering."))
        case .demanding:
            lines.append(DialogueLine(speaker: .client, text: "The venue needs to impress, and the food needs to be flawless. I'll notice if either is off."))
        case .indecisive:
            lines.append(DialogueLine(speaker: .client, text: "I think the food? Or maybe the venue? Both are important, right? What do you usually recommend?"))
        case .celebrity:
            lines.append(DialogueLine(speaker: .client, text: "The ambiance. People need to walk in and feel something. And the food — it has to be Instagram-worthy."))
        }

        return lines
    }

    private func closingLines(event: EventData) -> [DialogueLine] {
        switch event.personality {
        case .easyGoing:
            return [DialogueLine(speaker: .client, text: "Sounds great! I'm excited. Just let me know what you need from me.")]
        case .budgetConscious:
            return [DialogueLine(speaker: .client, text: "Perfect. Just please keep me posted on costs as we go — I don't want any surprises on the final bill.")]
        case .perfectionist:
            return [DialogueLine(speaker: .client, text: "I'll send you my Pinterest board. I want to approve everything before it's finalized.")]
        case .demanding:
            return [DialogueLine(speaker: .client, text: "Good. I'll expect regular updates. Don't wait until the last minute to tell me if something's wrong.")]
        case .indecisive:
            return [DialogueLine(speaker: .client, text: "Thank you so much! I feel better already having someone to guide me through this.")]
        case .celebrity:
            return [DialogueLine(speaker: .client, text: "My assistant will handle the contract details. Looking forward to seeing what you put together.")]
        }
    }

    // Budget hint helpers — vague enough that the player has to interpret
    private func budgetHint(_ budget: Double) -> String {
        let rounded = Int((budget / 100).rounded()) * 100
        return "\(rounded)"
    }

    private func budgetCeiling(_ budget: Double) -> String {
        let ceiling = Int((budget * 1.05 / 100).rounded()) * 100
        return "\(ceiling)"
    }

    private func budgetLow(_ budget: Double) -> String {
        let low = Int((budget * 0.8 / 100).rounded()) * 100
        return "\(low)"
    }

    private func budgetHigh(_ budget: Double) -> String {
        let high = Int((budget * 1.2 / 100).rounded()) * 100
        return "\(high)"
    }

    private func lines_player() {
        // Placeholder for future interactive call mode
    }

    /// Derive vendor requirements based on event type (Stage 1: venue + caterer).
    private func deriveRequirementsFromEvent(event: EventData) -> [String] {
        var requirements = ["Venue", "Caterer"]

        if playerData.stageNumber >= 2 {
            switch event.eventTypeId {
            case "AdultBirthday", "EngagementParty", "MilestoneBirthday":
                requirements.append("Entertainer")
            case "CorporateMeeting":
                requirements.append("AV Technician")
            default:
                break
            }
        }

        return requirements
    }
}
