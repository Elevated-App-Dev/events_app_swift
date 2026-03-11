import SwiftUI

enum GameplayTab: String, CaseIterable {
    case events
    case inquiries
}

struct GameplayView: View {
    @Environment(GameManager.self) private var gameManager
    @State private var selectedTab: GameplayTab = .events

    var body: some View {
        VStack(spacing: 0) {
            // HUD Bar
            HUDBarView()

            // Main content
            Group {
                switch selectedTab {
                case .events:
                    ActiveEventsListView()
                case .inquiries:
                    InquiryListView()
                }
            }
            .frame(maxHeight: .infinity)

            // Bottom tab bar
            HStack(spacing: 0) {
                tabButton(
                    tab: .events,
                    icon: "calendar",
                    label: "Events",
                    badge: gameManager.activeEvents.count
                )
                tabButton(
                    tab: .inquiries,
                    icon: "envelope",
                    label: "Inquiries",
                    badge: gameManager.pendingInquiries.count
                )
                Button(action: { gameManager.pauseGame() }) {
                    VStack(spacing: 2) {
                        Image(systemName: "pause.circle")
                            .font(.title2)
                        Text("Pause")
                            .font(.caption2)
                    }
                    .frame(maxWidth: .infinity)
                }
            }
            .padding(.vertical, 8)
            .background(.ultraThinMaterial)
        }
        .overlay {
            if gameManager.tutorialSystem.isTutorialActive {
                TutorialOverlayView()
            }
        }
        .sheet(item: Binding(
            get: { gameManager.lastCompletedEvent },
            set: { _ in gameManager.dismissResults() }
        )) { event in
            EventResultsView(event: event)
        }
    }

    @ViewBuilder
    private func tabButton(tab: GameplayTab, icon: String, label: String, badge: Int) -> some View {
        Button(action: { selectedTab = tab }) {
            VStack(spacing: 2) {
                ZStack(alignment: .topTrailing) {
                    Image(systemName: icon)
                        .font(.title2)
                    if badge > 0 {
                        Text("\(badge)")
                            .font(.caption2)
                            .fontWeight(.bold)
                            .foregroundStyle(.white)
                            .padding(3)
                            .background(Color.red, in: Circle())
                            .offset(x: 8, y: -4)
                    }
                }
                Text(label)
                    .font(.caption2)
            }
            .frame(maxWidth: .infinity)
            .foregroundStyle(selectedTab == tab ? .blue : .primary)
        }
    }
}

struct HUDBarView: View {
    @Environment(GameManager.self) private var gameManager

    var body: some View {
        HStack {
            VStack(alignment: .leading, spacing: 2) {
                Text(gameManager.playerData.playerName)
                    .font(.headline)
                Text("Stage: \(gameManager.playerData.stage.rawValue.capitalized)")
                    .font(.caption)
                    .foregroundStyle(.secondary)
            }
            Spacer()
            VStack(alignment: .center, spacing: 2) {
                Text(gameManager.timeSystem.currentDate.formatted)
                    .font(.caption)
                    .fontWeight(.medium)
            }
            Spacer()
            VStack(alignment: .trailing, spacing: 2) {
                Text("$\(gameManager.playerData.money, specifier: "%.0f")")
                    .font(.headline)
                HStack(spacing: 2) {
                    Image(systemName: "star.fill")
                        .font(.caption2)
                        .foregroundStyle(.yellow)
                    Text("\(gameManager.playerData.reputation)")
                        .font(.caption)
                        .foregroundStyle(.secondary)
                }
            }
        }
        .padding(.horizontal)
        .padding(.vertical, 8)
        .background(.ultraThinMaterial)
    }
}
