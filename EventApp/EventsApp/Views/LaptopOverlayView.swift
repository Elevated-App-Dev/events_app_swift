import SwiftUI

/// The laptop overlay — the business management hub.
/// Accessed by tapping the laptop on the desk.
/// Contains: Event Management (vendor booking), Client CRM, Financials.
struct LaptopOverlayView: View {
    @Environment(GameManager.self) private var gameManager
    @Binding var isPresented: Bool
    @State private var selectedTab: LaptopTab = .events

    var body: some View {
        ZStack {
            GameTheme.Colors.background.opacity(0.85)
                .ignoresSafeArea()
                .onTapGesture { isPresented = false }

            VStack(spacing: 0) {
                // Header bar
                HStack {
                    Text(selectedTab.title)
                        .font(GameTheme.Typography.h2)
                        .foregroundStyle(GameTheme.Colors.textPrimary)
                    Spacer()
                    Button(action: { isPresented = false }) {
                        Image(systemName: "xmark")
                            .font(.system(size: 18, weight: .medium))
                            .foregroundStyle(GameTheme.Colors.textSecondary)
                            .frame(width: GameTheme.Size.touchTarget, height: GameTheme.Size.touchTarget)
                    }
                }
                .padding(.horizontal, GameTheme.Spacing.md)
                .padding(.top, GameTheme.Spacing.md)

                // Tab bar
                HStack(spacing: 0) {
                    ForEach(LaptopTab.allCases, id: \.self) { tab in
                        Button(action: { selectedTab = tab }) {
                            VStack(spacing: 4) {
                                Image(systemName: tab.icon)
                                    .font(.system(size: 16))
                                Text(tab.label)
                                    .font(GameTheme.Typography.micro)
                            }
                            .frame(maxWidth: .infinity)
                            .padding(.vertical, GameTheme.Spacing.xs)
                            .foregroundStyle(selectedTab == tab ? GameTheme.Colors.accent : GameTheme.Colors.textMuted)
                        }
                    }
                }
                .padding(.horizontal, GameTheme.Spacing.sm)
                .padding(.top, GameTheme.Spacing.sm)

                Divider().background(GameTheme.Colors.border)

                // Content
                Group {
                    switch selectedTab {
                    case .events:
                        LaptopEventsView()
                    case .clients:
                        PhoneClientsView()
                    case .financials:
                        PhoneBankView()
                    }
                }
                .frame(maxHeight: .infinity)
            }
            .frame(maxWidth: .infinity, maxHeight: .infinity)
            .background(GameTheme.Colors.background)
            .clipShape(RoundedRectangle(cornerRadius: GameTheme.Radius.large))
            .padding(GameTheme.Spacing.xs)
            .padding(.top, 50) // Leave room at top for status bar
        }
    }
}

enum LaptopTab: String, CaseIterable {
    case events
    case clients
    case financials

    var title: String {
        switch self {
        case .events: return "Event Management"
        case .clients: return "Client CRM"
        case .financials: return "Financials"
        }
    }

    var label: String {
        switch self {
        case .events: return "Events"
        case .clients: return "Clients"
        case .financials: return "Bank"
        }
    }

    var icon: String {
        switch self {
        case .events: return "calendar.badge.clock"
        case .clients: return "person.2"
        case .financials: return "banknote"
        }
    }
}

/// Event management view inside the laptop — manage active events and vendors.
struct LaptopEventsView: View {
    @Environment(GameManager.self) private var gameManager
    @State private var selectedEventIndex: Int?

    var body: some View {
        ScrollView {
            VStack(alignment: .leading, spacing: GameTheme.Spacing.sm) {
                if gameManager.activeEvents.isEmpty {
                    VStack(spacing: GameTheme.Spacing.sm) {
                        Image(systemName: "calendar.badge.exclamationmark")
                            .font(.system(size: 48))
                            .foregroundStyle(GameTheme.Colors.textMuted)
                        Text("No active events")
                            .font(GameTheme.Typography.body)
                            .foregroundStyle(GameTheme.Colors.textMuted)
                        Text("Accept an inquiry to get started")
                            .font(GameTheme.Typography.caption)
                            .foregroundStyle(GameTheme.Colors.textMuted)
                    }
                    .frame(maxWidth: .infinity)
                    .padding(.top, GameTheme.Spacing.xl)
                } else {
                    ForEach(Array(gameManager.activeEvents.enumerated()), id: \.element.id) { index, event in
                        Button(action: { selectedEventIndex = index }) {
                            HStack {
                                VStack(alignment: .leading, spacing: 4) {
                                    Text(event.eventTitle)
                                        .font(GameTheme.Typography.h3)
                                        .foregroundStyle(GameTheme.Colors.textPrimary)
                                    Text("\(event.clientName) \u{00B7} \(event.eventDate.formatted)")
                                        .font(GameTheme.Typography.caption)
                                        .foregroundStyle(GameTheme.Colors.textSecondary)

                                    HStack(spacing: GameTheme.Spacing.sm) {
                                        Label("$\(event.budget.total, specifier: "%.0f")", systemImage: "banknote")
                                            .font(GameTheme.Typography.micro)
                                            .foregroundStyle(GameTheme.Colors.money)
                                        Label("\(event.guestCount)", systemImage: "person.2")
                                            .font(GameTheme.Typography.micro)
                                            .foregroundStyle(GameTheme.Colors.textMuted)

                                        if event.venueId != nil {
                                            Image(systemName: "building.2.fill")
                                                .font(GameTheme.Typography.micro)
                                                .foregroundStyle(GameTheme.Colors.success)
                                        }

                                        Text("\(event.vendors.count) vendors")
                                            .font(GameTheme.Typography.micro)
                                            .foregroundStyle(event.vendors.isEmpty ? GameTheme.Colors.warning : GameTheme.Colors.success)
                                    }
                                }

                                Spacer()

                                VStack(alignment: .trailing, spacing: 4) {
                                    PhaseBadge(phase: event.phase)
                                    Image(systemName: "chevron.right")
                                        .font(GameTheme.Typography.micro)
                                        .foregroundStyle(GameTheme.Colors.textMuted)
                                }
                            }
                        }
                        .surfaceCard()
                    }
                }
            }
            .padding(.horizontal, GameTheme.Spacing.md)
            .padding(.top, GameTheme.Spacing.sm)
        }
        .sheet(item: Binding(
            get: { selectedEventIndex.map { IdentifiableIndex(value: $0) } },
            set: { selectedEventIndex = $0?.value }
        )) { item in
            NavigationStack {
                EventDetailView(eventIndex: item.value)
            }
        }
    }
}
