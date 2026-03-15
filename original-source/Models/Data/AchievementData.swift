import Foundation

struct AchievementData: Codable, Equatable, Identifiable {
    var id: AchievementType { type }
    var type: AchievementType
    var name: String
    var description: String
    var category: AchievementCategory
    var isEarned: Bool = false
    var earnedTime: Date?
    var currentProgress: Int = 0
    var targetProgress: Int = 1
    var isSecret: Bool = false
    var iconId: String = ""

    var progressPercent: Double {
        guard targetProgress > 0 else { return 0 }
        return min(1.0, Double(currentProgress) / Double(targetProgress)) * 100
    }

    var isTrackable: Bool {
        targetProgress > 1
    }

    init(from definition: AchievementDefinition) {
        self.type = definition.type
        self.name = definition.name
        self.description = definition.description
        self.category = definition.category
        self.targetProgress = definition.targetProgress
        self.isSecret = definition.isSecret
        self.iconId = definition.iconId
    }

    init(type: AchievementType, name: String, description: String, category: AchievementCategory,
         targetProgress: Int = 1, isSecret: Bool = false, iconId: String = "") {
        self.type = type
        self.name = name
        self.description = description
        self.category = category
        self.targetProgress = targetProgress
        self.isSecret = isSecret
        self.iconId = iconId
    }
}

struct AchievementProgress: Codable, Equatable {
    var type: AchievementType
    var current: Int
    var target: Int
    var isComplete: Bool

    var percent: Double {
        guard target > 0 else { return 0 }
        return Double(current) / Double(target) * 100
    }

    var displayString: String {
        "\(current)/\(target)"
    }
}

struct AchievementDefinition: Codable, Equatable {
    var type: AchievementType
    var name: String
    var description: String
    var hiddenDescription: String = "???"
    var category: AchievementCategory
    var targetProgress: Int = 1
    var isSecret: Bool = false
    var iconId: String = ""
}

struct AchievementSaveData: Codable, Equatable {
    var earnedAchievements: [AchievementType] = []
    var progressEntries: [AchievementProgressEntry] = []
    var earnedTimestamps: [Date] = []
    var isSyncedWithPlatform: Bool = false

    func getProgress(_ type: AchievementType) -> Int {
        progressEntries.first(where: { $0.type == type })?.progress ?? 0
    }

    mutating func setProgress(_ type: AchievementType, progress: Int) {
        if let idx = progressEntries.firstIndex(where: { $0.type == type }) {
            progressEntries[idx].progress = progress
        } else {
            progressEntries.append(AchievementProgressEntry(type: type, progress: progress))
        }
    }

    mutating func markEarned(_ type: AchievementType, at time: Date) {
        if !earnedAchievements.contains(type) {
            earnedAchievements.append(type)
            earnedTimestamps.append(time)
        }
    }

    func isEarned(_ type: AchievementType) -> Bool {
        earnedAchievements.contains(type)
    }
}

struct AchievementProgressEntry: Codable, Equatable {
    var type: AchievementType
    var progress: Int
}
