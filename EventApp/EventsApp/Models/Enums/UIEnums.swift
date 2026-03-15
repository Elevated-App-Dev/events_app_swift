import Foundation

enum PhoneApp: String, CaseIterable, Codable, Hashable {
    case calendar
    case messages
    case email
    case tasks       // Repurposed as "Progress" — completed milestones per event
    case bank
    case contacts
    case reviews
    case clients
    case marketing
}

enum TutorialStep: String, CaseIterable, Codable, Hashable {
    case welcome
    case acceptClient
    case selectVenue
    case selectCaterer
    case eventExecution
    case viewResults
    case complete
}

enum MusicTrack: String, CaseIterable, Codable, Hashable {
    case mainMenu
    case planning
    case execution
    case results
    case celebration
}

enum SoundEffect: String, CaseIterable, Codable, Hashable {
    case buttonClick
    case success
    case failure
    case notification
    case cashRegister
    case warning
}
