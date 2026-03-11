import SwiftUI

struct SettingsView: View {
    @Environment(GameManager.self) private var gameManager

    var body: some View {
        NavigationStack {
            List {
                Section("Audio") {
                    Text("Music Volume")
                    Text("SFX Volume")
                }
                Section("Accessibility") {
                    Text("Text Size")
                    Text("Colorblind Mode")
                }
            }
            .navigationTitle("Settings")
            .toolbar {
                Button("Done") {
                    gameManager.returnToMainMenu()
                }
            }
        }
    }
}
