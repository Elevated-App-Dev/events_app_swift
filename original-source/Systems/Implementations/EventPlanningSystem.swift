import Foundation

/// Manages event lifecycle from inquiry generation through booking.
/// Handles workload capacity, vendor/venue booking, and inquiry intervals.
class EventPlanningSystem: EventPlanningSystemProtocol {

    // MARK: - Workload Capacity Thresholds (Stage index 0 = Stage 1)

    private static let optimalCapacity   = [2, 4, 6, 10, 15]
    private static let comfortableCapacity = [3, 6, 9, 14, 20]
    private static let strainedCapacity  = [5, 8, 12, 18, 25]

    // MARK: - Inquiry Intervals (minutes, by stage)

    private static let inquiryIntervals: [(min: Double, max: Double)] = [
        (8, 12),   // Stage 1
        (6, 10),   // Stage 2
        (5, 8),    // Stage 3
        (4, 7),    // Stage 4
        (3, 6)     // Stage 5
    ]

    private static let stageMinReputation = [0, 25, 50, 100, 200]

    // MARK: - Client Name Pool

    private static let clientNames = [
        "Emma", "Liam", "Olivia", "Noah", "Ava", "Ethan", "Sophia", "Mason",
        "Isabella", "William", "Mia", "James", "Charlotte", "Benjamin", "Amelia",
        "Lucas", "Harper", "Henry", "Evelyn", "Alexander", "Abigail", "Michael",
        "Emily", "Daniel", "Elizabeth", "Jacob", "Sofia", "Logan", "Avery", "Jackson"
    ]

    // MARK: - Protocol Conformance

    func generateInquiry(stage: Int, reputation: Int, currentDate: GameDate) -> ClientInquiry {
        let clampedStage = max(1, min(5, stage))
        let clientName = Self.clientNames.randomElement()!
        let personality = generatePersonalityForStage(clampedStage)
        let eventTypeId = getRandomEventTypeForStage(clampedStage)
        let subCategory = getRandomSubCategory(for: eventTypeId)
        let budgetRange = getBudgetRange(for: eventTypeId)
        let budget = Int.random(in: budgetRange.min...budgetRange.max)
        let guestCount = generateGuestCount(for: eventTypeId)
        let complexity = getComplexity(for: eventTypeId)
        let schedRange = getSchedulingRange(for: complexity)
        let daysUntil = Int.random(in: schedRange.min...schedRange.max)
        let eventDate = currentDate.adding(days: daysUntil)

        return ClientInquiry.create(
            clientName: clientName,
            eventTypeId: eventTypeId,
            subCategory: subCategory,
            personality: personality,
            budget: budget,
            guestCount: guestCount,
            eventDate: eventDate
        )
    }

    func acceptInquiry(_ inquiry: ClientInquiry, currentDate: GameDate) -> EventData {
        EventData(
            id: UUID().uuidString,
            clientId: inquiry.inquiryId,
            clientName: inquiry.clientName,
            eventTitle: generateEventTitle(clientName: inquiry.clientName, subCategory: inquiry.subCategory),
            eventTypeId: inquiry.eventTypeId,
            subCategory: inquiry.subCategory,
            status: .accepted,
            phase: .booking,
            eventDate: inquiry.eventDate,
            acceptedDate: currentDate,
            personality: inquiry.personality,
            guestCount: inquiry.guestCount,
            budget: EventBudget(total: Double(inquiry.budget)),
            isReferral: inquiry.isReferral,
            referredByClientName: inquiry.referredByClientName
        )
    }

    func declineInquiry(_ inquiry: ClientInquiry) {
        // No penalty for declining — removal handled by caller
    }

    func getWorkloadStatus(activeEvents: Int, stage: Int) -> WorkloadStatus {
        let clampedStage = max(1, min(5, stage))
        let idx = clampedStage - 1

        // Stage 1 simplified: soft cap at 3 with warning
        if clampedStage == 1 {
            return activeEvents <= 3 ? .optimal : .comfortable
        }

        if activeEvents <= Self.optimalCapacity[idx]    { return .optimal }
        if activeEvents <= Self.comfortableCapacity[idx] { return .comfortable }
        if activeEvents <= Self.strainedCapacity[idx]    { return .strained }
        return .critical
    }

    func calculateWorkloadPenalty(activeEvents: Int, stage: Int) -> Double {
        let clampedStage = max(1, min(5, stage))
        let idx = clampedStage - 1

        // Stage 1: no percentage penalties
        if clampedStage == 1 { return 0 }

        let optimal    = Self.optimalCapacity[idx]
        let comfortable = Self.comfortableCapacity[idx]
        let strained   = Self.strainedCapacity[idx]

        if activeEvents <= optimal { return 0 }

        if activeEvents <= comfortable {
            return Double(activeEvents - optimal) * 3.0
        } else if activeEvents <= strained {
            let overOptimal = comfortable - optimal
            let overComfortable = activeEvents - comfortable
            return Double(overOptimal) * 3.0 + Double(overComfortable) * 7.0
        } else {
            let overOptimal = comfortable - optimal
            let overComfortable = strained - comfortable
            let overStrained = activeEvents - strained
            return Double(overOptimal) * 3.0 + Double(overComfortable) * 7.0 + Double(overStrained) * 12.0
        }
    }

    func calculateTaskFailureProbabilityIncrease(activeEvents: Int, stage: Int) -> Double {
        let clampedStage = max(1, min(5, stage))
        let idx = clampedStage - 1

        if clampedStage == 1 { return 0 }

        let comfortable = Self.comfortableCapacity[idx]
        let strained   = Self.strainedCapacity[idx]

        if activeEvents <= comfortable { return 0 }
        if activeEvents <= strained    { return 10 }
        return 25
    }

    func calculateOverlappingPrepPenalty(_ overlappingEvents: Int) -> Double {
        guard overlappingEvents > 1 else { return 0 }
        return Double(overlappingEvents - 1) * 5.0
    }

    func bookVendor(event: EventData, vendor: VendorData, dates: [GameDate]) -> BookingResult {
        // Check availability
        if dates.contains(event.eventDate) {
            return .failed("\(vendor.vendorName) is not available on \(event.eventDate.formatted)")
        }

        let price = vendor.basePrice
        let remaining = event.budget.remaining

        if price > remaining {
            return .successfulWithWarning(
                price: price,
                message: "Booking \(vendor.vendorName) exceeds remaining budget."
            )
        }

        return .successful(price: price, message: "Successfully booked \(vendor.vendorName)")
    }

    func bookVenue(event: EventData, venue: VenueData, dates: [GameDate]) -> BookingResult {
        // Check availability
        if dates.contains(event.eventDate) {
            return .failed("\(venue.venueName) is not available on \(event.eventDate.formatted)")
        }

        // Validate capacity
        if event.guestCount > venue.capacityMax {
            return .failed("\(venue.venueName) cannot accommodate \(event.guestCount) guests. Maximum: \(venue.capacityMax)")
        }

        let price = venue.basePrice + (venue.pricePerGuest * Double(event.guestCount))
        let remaining = event.budget.remaining

        // Check cramped conditions
        if event.guestCount > venue.capacityComfortable {
            return .successfulWithWarning(
                price: price,
                message: "Guest count (\(event.guestCount)) exceeds comfortable capacity (\(venue.capacityComfortable)). May affect satisfaction."
            )
        }

        if price > remaining {
            return .successfulWithWarning(
                price: price,
                message: "Booking \(venue.venueName) exceeds remaining budget."
            )
        }

        return .successful(price: price, message: "Successfully booked \(venue.venueName)")
    }

    func generateEventTitle(clientName: String, subCategory: String) -> String {
        let name = clientName.isEmpty ? "Client" : clientName
        let sub = subCategory.isEmpty ? "Event" : subCategory
        return "\(name)'s \(sub)"
    }

    func getInquiryIntervalRange(stage: Int) -> (min: Double, max: Double) {
        let idx = max(0, min(4, max(1, stage) - 1))
        return Self.inquiryIntervals[idx]
    }

    func calculateAdjustedInquiryInterval(stage: Int, reputation: Int) -> Double {
        let clampedStage = max(1, min(5, stage))
        let (minInterval, maxInterval) = getInquiryIntervalRange(stage: clampedStage)
        let baseInterval = (minInterval + maxInterval) / 2.0

        let stageMinRep = Self.stageMinReputation[clampedStage - 1]
        let reputationAboveMin = Double(max(0, reputation - stageMinRep))

        // Reduce interval by 5% per 25 reputation points above minimum
        let reductionPercent = (reputationAboveMin / 25.0) * 5.0
        let reduction = baseInterval * (reductionPercent / 100.0)

        return max(minInterval, baseInterval - reduction)
    }

    // MARK: - Private Helpers

    private func generatePersonalityForStage(_ stage: Int) -> ClientPersonality {
        let roll = Int.random(in: 0..<100)

        switch stage {
        case 1:
            if roll < 50 { return .easyGoing }
            if roll < 80 { return .budgetConscious }
            return .perfectionist
        case 2:
            if roll < 40 { return .easyGoing }
            if roll < 75 { return .budgetConscious }
            return .perfectionist
        default:
            if roll < 33 { return .easyGoing }
            if roll < 66 { return .budgetConscious }
            return .perfectionist
        }
    }

    private func getRandomEventTypeForStage(_ stage: Int) -> String {
        var types = ["KidsBirthday", "FamilyGathering", "SchoolEvent", "BabyShower"]
        if stage >= 2 {
            types += ["AdultBirthday", "EngagementParty", "CorporateMeeting", "MilestoneBirthday"]
        }
        return types.randomElement()!
    }

    private func getRandomSubCategory(for eventTypeId: String) -> String {
        let subCategories: [String]
        switch eventTypeId {
        case "KidsBirthday":
            subCategories = ["Princess Theme Birthday", "Superhero Theme Birthday", "Pool Party",
                             "Bounce House Party", "Arts & Crafts Party", "Sports Party"]
        case "FamilyGathering":
            subCategories = ["Graduation Celebration", "4th of July BBQ", "Thanksgiving Dinner",
                             "Christmas Party", "New Year's Eve Party", "Easter Brunch", "Family Reunion"]
        case "SchoolEvent":
            subCategories = ["Science Fair", "Talent Show", "Prom", "Sports Banquet",
                             "Graduation Ceremony", "PTA Fundraiser"]
        case "AdultBirthday":
            subCategories = ["Surprise Party", "Cocktail Party", "Dinner Party",
                             "Themed Costume Party", "Outdoor Adventure Party"]
        case "EngagementParty":
            subCategories = ["Garden Party", "Rooftop Celebration", "Intimate Dinner",
                             "Brunch Engagement", "Cocktail Reception"]
        case "CorporateMeeting":
            subCategories = ["Board Meeting", "Team Building Retreat", "Training Workshop",
                             "Quarterly Review", "Client Presentation", "Staff Appreciation Luncheon"]
        case "MilestoneBirthday":
            subCategories = ["Sweet 16", "21st Birthday", "30th Birthday Bash",
                             "40th Birthday", "50th Golden Birthday", "Quinceañera"]
        case "BabyShower":
            subCategories = ["Traditional Baby Shower", "Gender Reveal Party",
                             "Couples Baby Shower", "Virtual Baby Shower", "Sprinkle"]
        default:
            subCategories = ["Event"]
        }
        return subCategories.randomElement()!
    }

    private func getBudgetRange(for eventTypeId: String) -> (min: Int, max: Int) {
        switch eventTypeId {
        case "KidsBirthday":      return (500, 2000)
        case "FamilyGathering":   return (300, 1500)
        case "SchoolEvent":       return (1000, 3000)
        case "AdultBirthday":     return (1000, 5000)
        case "EngagementParty":   return (2000, 8000)
        case "CorporateMeeting":  return (3000, 15000)
        case "MilestoneBirthday": return (2000, 6000)
        case "BabyShower":        return (1000, 4000)
        default:                  return (1000, 5000)
        }
    }

    private func generateGuestCount(for eventTypeId: String) -> Int {
        let range: (min: Int, max: Int)
        switch eventTypeId {
        case "KidsBirthday":      range = (10, 30)
        case "FamilyGathering":   range = (15, 50)
        case "SchoolEvent":       range = (50, 200)
        case "AdultBirthday":     range = (20, 75)
        case "EngagementParty":   range = (30, 100)
        case "CorporateMeeting":  range = (20, 100)
        case "MilestoneBirthday": range = (25, 100)
        case "BabyShower":        range = (15, 50)
        default:                  range = (20, 50)
        }
        return Int.random(in: range.min...range.max)
    }

    private func getComplexity(for eventTypeId: String) -> EventComplexity {
        switch eventTypeId {
        case "KidsBirthday", "FamilyGathering": return .low
        default:                                 return .medium
        }
    }

    private func getSchedulingRange(for complexity: EventComplexity) -> (min: Int, max: Int) {
        switch complexity {
        case .low:      return (3, 7)
        case .medium:   return (7, 14)
        case .high:     return (14, 21)
        case .veryHigh: return (21, 30)
        }
    }
}
