import SwiftUI

@main
struct EventsApp: App {
    @State private var gameManager = GameManager()

    var body: some Scene {
        WindowGroup {
            ContentView()
                .environment(gameManager)
        }
    }
}
