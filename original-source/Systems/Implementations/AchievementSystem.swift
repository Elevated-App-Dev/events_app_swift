import Foundation
import Combine

/// Achievement system managing player achievements and Game Center integration.
class AchievementSystem: AchievementSystemProtocol {

    // MARK: - Achievement Definitions

    private static let definitions: [AchievementType: AchievementDefinition] = [
        // Progression
        .firstSteps:           AchievementDefinition(type: .firstSteps, name: "First Steps", description: "Complete your first event", category: .progression),
        .risingStar:           AchievementDefinition(type: .risingStar, name: "Rising Star", description: "Reach Stage 2", category: .progression),
        .goingPro:             AchievementDefinition(type: .goingPro, name: "Going Pro", description: "Reach Stage 3", category: .progression),
        .industryLeader:       AchievementDefinition(type: .industryLeader, name: "Industry Leader", description: "Reach Stage 4", category: .progression),
        .eventPlanningMogul:   AchievementDefinition(type: .eventPlanningMogul, name: "Event Planning Mogul", description: "Reach Stage 5", category: .progression),

        // Mastery
        .perfectPlanner:       AchievementDefinition(type: .perfectPlanner, name: "Perfect Planner", description: "Achieve 100% satisfaction on any event", category: .mastery),
        .consistencyIsKey:     AchievementDefinition(type: .consistencyIsKey, name: "Consistency is Key", description: "Complete 10 events with 80%+ satisfaction", category: .mastery, targetProgress: 10),
        .excellenceStreak:     AchievementDefinition(type: .excellenceStreak, name: "Excellence Streak", description: "Achieve 5 consecutive 90%+ satisfaction events", category: .mastery, targetProgress: 5),
        .budgetMaster:         AchievementDefinition(type: .budgetMaster, name: "Budget Master", description: "Complete 10 events under budget", category: .mastery, targetProgress: 10),
        .vendorWhisperer:      AchievementDefinition(type: .vendorWhisperer, name: "Vendor Whisperer", description: "Reach relationship level 5 with 5 different vendors", category: .mastery, targetProgress: 5),
        .weatherWatcher:       AchievementDefinition(type: .weatherWatcher, name: "Weather Watcher", description: "Successfully handle 5 weather-related challenges", category: .mastery, targetProgress: 5),

        // Challenge
        .perfectionistsPerfectionist: AchievementDefinition(type: .perfectionistsPerfectionist, name: "Perfectionist's Perfectionist", description: "Satisfy a Perfectionist client with 95%+ satisfaction", category: .challenge),
        .jugglingAct:          AchievementDefinition(type: .jugglingAct, name: "Juggling Act", description: "Successfully complete 3 concurrent events", category: .challenge),
        .crisisManager:        AchievementDefinition(type: .crisisManager, name: "Crisis Manager", description: "Recover from 3 random events using contingency", category: .challenge, targetProgress: 3),
        .selfMade:             AchievementDefinition(type: .selfMade, name: "Self-Made", description: "Reach Stage 3 via Entrepreneur Path", category: .challenge),
        .corporateClimber:     AchievementDefinition(type: .corporateClimber, name: "Corporate Climber", description: "Reach Director on Corporate Path", category: .challenge),
        .celebrityHandler:     AchievementDefinition(type: .celebrityHandler, name: "Celebrity Handler", description: "Successfully complete a Celebrity event", category: .challenge),

        // Secret
        .aboveAndBeyond:       AchievementDefinition(type: .aboveAndBeyond, name: "Above and Beyond", description: "Trigger hidden reputation bonuses 10 times", category: .secret, targetProgress: 10, isSecret: true, hiddenDescription: "Go the extra mile..."),
        .familyFirst:          AchievementDefinition(type: .familyFirst, name: "Family First", description: "Never use family emergency funds", category: .secret, isSecret: true, hiddenDescription: "Stand on your own two feet"),
        .comebackKid:          AchievementDefinition(type: .comebackKid, name: "Comeback Kid", description: "Recover from a Financial Crisis", category: .secret, isSecret: true, hiddenDescription: "What doesn't kill you...")
    ]

    // MARK: - State

    private var achievements: [AchievementType: AchievementData] = [:]
    private var isSyncedWithPlatform = false

    private let _onAchievementEarned = PassthroughSubject<AchievementData, Never>()
    private let _onProgressUpdated = PassthroughSubject<(AchievementType, Int, Int), Never>()

    var onAchievementEarned: AnyPublisher<AchievementData, Never> { _onAchievementEarned.eraseToAnyPublisher() }
    var onProgressUpdated: AnyPublisher<(AchievementType, Int, Int), Never> { _onProgressUpdated.eraseToAnyPublisher() }

    init() {
        initializeAchievements()
    }

    private func initializeAchievements() {
        achievements.removeAll()
        for (type, def) in Self.definitions {
            achievements[type] = AchievementData(
                type: type,
                name: def.name,
                description: def.description,
                category: def.category,
                isSecret: def.isSecret,
                targetProgress: def.targetProgress
            )
        }
    }

    // MARK: - Protocol

    func checkAndAward(_ achievement: AchievementType) {
        guard let data = achievements[achievement], !data.isEarned else { return }

        if data.targetProgress > 1 {
            if data.currentProgress >= data.targetProgress {
                awardAchievement(achievement)
            }
        } else {
            awardAchievement(achievement)
        }
    }

    func incrementProgress(_ achievement: AchievementType, amount: Int) {
        guard var data = achievements[achievement], !data.isEarned else { return }
        let old = data.currentProgress
        data.currentProgress = min(data.currentProgress + amount, data.targetProgress)
        achievements[achievement] = data

        if data.currentProgress != old {
            _onProgressUpdated.send((achievement, data.currentProgress, data.targetProgress))
        }
        if data.currentProgress >= data.targetProgress {
            awardAchievement(achievement)
        }
    }

    func getProgress(_ achievement: AchievementType) -> AchievementProgress {
        guard let data = achievements[achievement] else {
            return AchievementProgress(type: achievement, current: 0, target: 1, isEarned: false)
        }
        return AchievementProgress(type: achievement, current: data.currentProgress, target: data.targetProgress, isEarned: data.isEarned)
    }

    func isAchievementEarned(_ achievement: AchievementType) -> Bool {
        achievements[achievement]?.isEarned ?? false
    }

    func getEarnedAchievements() -> [AchievementData] {
        achievements.values.filter { $0.isEarned }
    }

    func getAllAchievements() -> [AchievementData] {
        achievements.values.map { data in
            var copy = data
            if copy.isSecret && !copy.isEarned {
                if let def = Self.definitions[copy.type] {
                    copy.description = def.hiddenDescription ?? "???"
                    copy.name = "???"
                }
            }
            return copy
        }
    }

    func getAchievementsByCategory(_ category: AchievementCategory) -> [AchievementData] {
        getAllAchievements().filter { $0.category == category }
    }

    func getEarnedCount() -> Int {
        achievements.values.filter { $0.isEarned }.count
    }

    func getTotalCount() -> Int {
        achievements.count
    }

    func syncWithPlatform() {
        // In production, integrate with GameKit / Game Center
        isSyncedWithPlatform = true
    }

    func resetAll() {
        initializeAchievements()
        isSyncedWithPlatform = false
    }

    // MARK: - Save / Load

    func getSaveData() -> AchievementSaveData {
        var save = AchievementSaveData()
        save.isSyncedWithPlatform = isSyncedWithPlatform
        for (type, data) in achievements {
            if data.isEarned {
                save.earnedAchievements.append(type)
                save.earnedTimestamps.append(data.earnedTimestamp)
            }
            if data.currentProgress > 0 {
                save.progressEntries.append(AchievementProgressEntry(type: type, progress: data.currentProgress))
            }
        }
        return save
    }

    func initialize(saveData: AchievementSaveData?) {
        guard let saveData else {
            initializeAchievements()
            return
        }

        for type in saveData.earnedAchievements {
            if var data = achievements[type] {
                data.isEarned = true
                achievements[type] = data
            }
        }
        for entry in saveData.progressEntries {
            if var data = achievements[entry.type] {
                data.currentProgress = entry.progress
                achievements[entry.type] = data
            }
        }
        isSyncedWithPlatform = saveData.isSyncedWithPlatform
    }

    // MARK: - Private

    private func awardAchievement(_ type: AchievementType) {
        guard var data = achievements[type], !data.isEarned else { return }
        data.isEarned = true
        data.earnedTimestamp = Date().timeIntervalSince1970
        data.currentProgress = data.targetProgress
        achievements[type] = data

        _onAchievementEarned.send(data)
        isSyncedWithPlatform = false
    }
}
