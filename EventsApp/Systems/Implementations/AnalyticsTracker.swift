import Foundation

/// Analytics event tracking helper.
/// In production, integrate with Firebase Analytics or similar.
class AnalyticsTracker {

    private var sessionStartTime: Date
    private var lastSessionEndTime: Date?

    init() {
        sessionStartTime = Date()
    }

    // MARK: - Session

    func trackSessionStart() {
        sessionStartTime = Date()
        log("session_start", params: ["time_since_last": timeSinceLastSession()])
    }

    func trackSessionEnd() {
        let duration = Date().timeIntervalSince(sessionStartTime)
        lastSessionEndTime = Date()
        log("session_end", params: ["duration": duration])
    }

    // MARK: - Events

    func trackEventAccepted(eventType: String, personality: String, budget: Double) {
        log("event_accepted", params: [
            "event_type": eventType,
            "personality": personality,
            "budget": budget
        ])
    }

    func trackEventCompleted(eventType: String, satisfaction: Double, profit: Double) {
        log("event_completed", params: [
            "event_type": eventType,
            "satisfaction": satisfaction,
            "profit": profit
        ])
    }

    func trackStageAdvanced(newStage: Int) {
        log("stage_advanced", params: ["new_stage": newStage])
    }

    func trackPathChosen(_ path: String) {
        log("path_chosen", params: ["path": path])
    }

    func trackPurchase(productId: String, price: Double) {
        log("iap_purchase", params: ["product_id": productId, "price": price])
    }

    func trackAdWatched(placement: String) {
        log("ad_watched", params: ["placement": placement])
    }

    func trackAchievementEarned(_ achievementName: String) {
        log("achievement_earned", params: ["name": achievementName])
    }

    func trackTutorialStep(_ step: String) {
        log("tutorial_step", params: ["step": step])
    }

    func trackError(_ error: String, context: String) {
        log("error", params: ["error": error, "context": context])
    }

    // MARK: - Private

    private func timeSinceLastSession() -> Double {
        guard let last = lastSessionEndTime else { return 0 }
        return Date().timeIntervalSince(last)
    }

    private func log(_ event: String, params: [String: Any]) {
        // In production: send to Firebase Analytics, Amplitude, etc.
        #if DEBUG
        print("[Analytics] \(event): \(params)")
        #endif
    }
}
