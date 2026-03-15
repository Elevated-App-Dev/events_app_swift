import Foundation

struct PlayerData: Codable, Equatable {
    var playerName: String = "Alex"
    var avatar: AvatarData = AvatarData()
    var stage: BusinessStage = .solo
    var careerPath: CareerPath = .none
    var reputation: Int = 0
    var money: Double = 500.0
    var employeeData: EmployeeData?
    var activeEventIds: [String] = []
    var completedEventIds: [String] = []
    var vendorRelationships: [VendorRelationship] = []
    var hiddenAboveAndBeyondCount: Int = 0
    var hiddenExploitationCount: Int = 0
    var familyHelpUsed: Int = 0
    var unlockedZones: [MapZone] = [.neighborhood]

    var stageNumber: Int {
        switch stage {
        case .solo: return 1
        case .employee: return 2
        case .smallCompany: return 3
        case .established: return 4
        case .premier: return 5
        }
    }
}

struct AvatarData: Codable, Equatable {
    var faceShapeIndex: Int = 0
    var skinToneIndex: Int = 0
    var hairStyleIndex: Int = 0
    var hairColorIndex: Int = 0
    var outfitIndex: Int = 0
}

struct EmployeeData: Codable, Equatable {
    var companyName: String = "Premier Events Co."
    var employeeLevel: Int = 1
    var performanceScore: Int = 0
    var consecutiveNegativeReviews: Int = 0
    var eventsCompletedSinceReview: Int = 0
    var activeSideGigs: Int = 0

    var title: String {
        switch employeeLevel {
        case 1: return "Junior Planner"
        case 2: return "Planner"
        case 3...5: return "Senior Planner"
        default: return "Planner"
        }
    }

    var compensation: (basePay: Double, commission: Double) {
        switch employeeLevel {
        case 1: return (300, 0.05)
        case 2: return (500, 0.10)
        case 3...5: return (750, 0.15)
        default: return (300, 0.05)
        }
    }
}
