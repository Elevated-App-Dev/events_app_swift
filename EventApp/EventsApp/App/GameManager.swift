import Foundation
import Observation

/// Central game state manager - replaces Unity's GameManager MonoBehaviour singleton.
/// Uses @Observable so SwiftUI views react to state changes with fine-grained invalidation.
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

    // MARK: - Systems

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

    // MARK: - Game Loop

    private var gameLoopTimer: Timer?
    private var lastTickDate: Date?
    private var inquiryAccumulator: Double = 0
    private var nextInquiryInterval: Double = 0

    init() {
        let startDate = GameDate(month: 3, day: 1, year: 1)
        timeSystem = TimeSystem(startDate: startDate)
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
        saveData.currentDate = GameDate(month: 3, day: 1, year: 1)
        saveData.journeyStartTime = Date()

        pendingInquiries = []
        activeEvents = []
        completedEvents = []
        lastCompletedEvent = nil

        timeSystem.setCurrentDate(saveData.currentDate)
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

        // Start tutorial
        startTutorial()

        gameState = .tutorial
        isInitialized = true

        // Start game loop (time is paused during early tutorial)
        timeSystem.pause()
        startGameLoop()
    }

    func continueGame() {
        gameState = .playing
    }

    func pauseGame() {
        gameState = .paused
        timeSystem.pause()
    }

    func resumeGame() {
        gameState = .playing
        timeSystem.resume()
        lastTickDate = Date()
    }

    func showSettings() {
        gameState = .settings
    }

    func returnToMainMenu() {
        stopGameLoop()
        gameState = .mainMenu
    }

    // MARK: - Game Loop

    private func startGameLoop() {
        lastTickDate = Date()
        nextInquiryInterval = eventPlanningSystem.calculateAdjustedInquiryInterval(
            stage: playerData.stageNumber,
            reputation: playerData.reputation
        ) * 60.0
        inquiryAccumulator = 0

        gameLoopTimer = Timer.scheduledTimer(withTimeInterval: 1.0, repeats: true) { [weak self] _ in
            self?.gameTick()
        }
    }

    private func stopGameLoop() {
        gameLoopTimer?.invalidate()
        gameLoopTimer = nil
    }

    private func gameTick() {
        guard gameState == .playing || gameState == .tutorial else { return }
        guard !timeSystem.isPaused else { return }

        let now = Date()
        let deltaTime = now.timeIntervalSince(lastTickDate ?? now)
        lastTickDate = now

        let previousDate = timeSystem.currentDate

        // Advance game time
        timeSystem.advanceTime(deltaTime: deltaTime, stage: playerData.stageNumber)

        let currentDate = timeSystem.currentDate

        // Day changed — daily processing
        if currentDate != previousDate {
            onDayChanged(from: previousDate, to: currentDate)
        }

        // Inquiry generation (only while playing, not tutorial)
        if gameState == .playing {
            inquiryAccumulator += deltaTime
            if inquiryAccumulator >= nextInquiryInterval {
                generateNewInquiry()
                inquiryAccumulator = 0
                nextInquiryInterval = eventPlanningSystem.calculateAdjustedInquiryInterval(
                    stage: playerData.stageNumber,
                    reputation: playerData.reputation
                ) * 60.0
            }
        }

        // Expire old inquiries
        pendingInquiries.removeAll { $0.isExpired }
    }

    private func onDayChanged(from previousDate: GameDate, to currentDate: GameDate) {
        weatherSystem.advanceDay(to: currentDate)
        saveData.currentDate = currentDate
        processEventPhases(currentDate: currentDate)
    }

    // MARK: - Event Phase Processing

    private func processEventPhases(currentDate: GameDate) {
        // Process in reverse so removals don't shift indices
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

            // Execution day
            if phaseInfo.phase == .executionDay && activeEvents[i].status != .executing {
                activeEvents[i].status = .executing
                executeEvent(at: i)
            }

            // Results phase (day after event)
            if phaseInfo.phase == .results && activeEvents[i].status == .executing {
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

        // Random events
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

        // Calculate satisfaction
        let client = ClientData.fromEvent(activeEvents[index])
        let satResult = satisfactionCalculator.calculate(event: activeEvents[index], client: client)
        activeEvents[index].results?.finalSatisfaction = satResult.finalSatisfaction

        // Calculate profit
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

        // Apply reputation
        let repChange = progressionSystem.applyEventResult(
            satisfaction: satisfaction,
            currentReputation: playerData.reputation,
            stage: playerData.stageNumber
        )
        playerData.reputation = repChange.newReputation
        event.results?.reputationChange = repChange.change

        // Apply profit
        playerData.money += profit

        // Generate feedback
        event.results?.clientFeedback = generateFeedback(satisfaction: satisfaction, tier: repChange.satisfactionTier)

        // Referral check
        let referralResult = referralSystem.evaluateReferral(
            satisfaction: satisfaction,
            excellenceStreak: saveData.excellenceStreak
        )
        event.results?.triggeredReferral = referralResult.wasReferred

        // Update excellence streak
        let streakResult = referralSystem.updateExcellenceStreak(
            currentStreak: saveData.excellenceStreak,
            satisfaction: satisfaction
        )
        saveData.excellenceStreak = streakResult.newStreak

        // Move to completed
        activeEvents.remove(at: index)
        completedEvents.append(event)
        lastCompletedEvent = event

        playerData.completedEventIds.append(event.id)
        playerData.activeEventIds.removeAll { $0 == event.id }

        // Tutorial: advance if at viewResults step
        if tutorialSystem.isTutorialActive && tutorialSystem.currentStep == .eventExecution {
            advanceTutorial()
        }
    }

    // MARK: - Score Calculation Helpers

    private func calculateVenueScore(for event: EventData) -> Double {
        guard let venueId = event.venueId,
              let venue = SeedData.venue(byId: venueId) else { return 30.0 }

        var score = venue.ambianceRating

        // Overcrowding penalty
        if event.guestCount > venue.capacityComfortable {
            let overPercent = Double(event.guestCount - venue.capacityComfortable)
                / Double(max(1, venue.capacityMax - venue.capacityComfortable))
            score -= overPercent * 20
        }

        // Weather risk for outdoor venues
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
            currentDate: timeSystem.currentDate
        )
        pendingInquiries.append(inquiry)
    }

    func acceptInquiry(_ inquiry: ClientInquiry) {
        let event = eventPlanningSystem.acceptInquiry(inquiry, currentDate: timeSystem.currentDate)
        activeEvents.append(event)
        playerData.activeEventIds.append(event.id)
        pendingInquiries.removeAll { $0.inquiryId == inquiry.inquiryId }

        // Tutorial: advance if at acceptClient step
        if tutorialSystem.isTutorialActive && tutorialSystem.currentStep == .acceptClient {
            advanceTutorial()
        }
    }

    func declineInquiry(_ inquiry: ClientInquiry) {
        eventPlanningSystem.declineInquiry(inquiry)
        pendingInquiries.removeAll { $0.inquiryId == inquiry.inquiryId }
    }

    // MARK: - Venue & Vendor Assignment

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

            // Tutorial: advance if at selectVenue step
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
                bookingDate: timeSystem.currentDate
            )
            activeEvents[eventIndex].vendors.append(assignment)
            activeEvents[eventIndex].budget.spent += result.price
            saveData.addVendorBooking(vendor.id, date: activeEvents[eventIndex].eventDate)

            // Tutorial: advance if at selectCaterer step and vendor is a caterer
            if tutorialSystem.isTutorialActive && tutorialSystem.currentStep == .selectCaterer && vendor.category == .caterer {
                advanceTutorial()
            }
        }

        return result
    }

    // MARK: - Tutorial

    private func startTutorial() {
        tutorialSystem.startTutorial()
        // Generate one guaranteed tutorial inquiry
        let inquiry = eventPlanningSystem.generateInquiry(
            stage: 1,
            reputation: 0,
            currentDate: timeSystem.currentDate
        )
        pendingInquiries.append(inquiry)
    }

    func advanceTutorial() {
        tutorialSystem.advanceStep()

        // At eventExecution step, start time flowing
        if tutorialSystem.currentStep == .eventExecution {
            gameState = .playing
            timeSystem.resume()
            lastTickDate = Date()
        }

        // Tutorial complete
        if tutorialSystem.isTutorialComplete {
            gameState = .playing
            timeSystem.resume()
            lastTickDate = Date()
        }
    }

    func skipTutorial() {
        tutorialSystem.skipTutorial()
        gameState = .playing
        timeSystem.resume()
        lastTickDate = Date()
    }

    func dismissResults() {
        lastCompletedEvent = nil

        // Tutorial: advance if at viewResults step
        if tutorialSystem.isTutorialActive && tutorialSystem.currentStep == .viewResults {
            advanceTutorial()
        }
    }
}
