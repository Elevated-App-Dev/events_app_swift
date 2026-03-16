import SwiftUI

/// The main game screen — an interactive office desk.
/// Tappable objects open overlays (phone, calendar, laptop, map, mail).
/// This replaces the old tab-based GameplayView as the primary navigation hub.
struct OfficeDeskView: View {
    @Environment(GameManager.self) private var gameManager
    @State private var showPhone = false
    @State private var phoneOpenToApp: PhoneApp?
    @State private var showLaptop = false
    @State private var showAdvanceConfirm = false

    var body: some View {
        ZStack {
            // Full-screen dark background
            GameTheme.Colors.background
                .ignoresSafeArea()

            VStack(spacing: 0) {
                // Top HUD bar
                DeskHUDView()

                // Desk workspace
                deskArea
                    .frame(maxHeight: .infinity)

                // Advance button at bottom
                DeskAdvanceButton {
                    gameManager.advanceToNextPoint()
                }
                .padding(.horizontal, GameTheme.Spacing.md)
                .padding(.bottom, GameTheme.Spacing.sm)
            }

            // Laptop overlay
            if showLaptop {
                LaptopOverlayView(isPresented: $showLaptop)
                    .transition(.opacity)
                    .zIndex(10)
            }

            // Phone overlay
            if showPhone {
                PhoneOverlayView(isPresented: $showPhone, initialApp: phoneOpenToApp)
                    .transition(.move(edge: .bottom))
                    .zIndex(10)
                    .onDisappear { phoneOpenToApp = nil }
            }

            // Tutorial disabled — will be rebuilt for new UI flow

            // Pause overlay (available via settings in future)

        }
        .animation(GameTheme.Anim.panelSlide, value: showPhone)
        .animation(GameTheme.Anim.panelSlide, value: showLaptop)
        .sheet(item: Binding(
            get: { gameManager.lastCompletedEvent },
            set: { _ in gameManager.dismissResults() }
        )) { event in
            EventResultsView(event: event)
        }
    }

    // MARK: - Desk Area

    private var deskArea: some View {
        GeometryReader { geo in
            let width = geo.size.width
            let height = geo.size.height

            ZStack {
                // Desk surface
                RoundedRectangle(cornerRadius: GameTheme.Radius.large)
                    .fill(Color(hex: 0x1E1510))
                    .padding(.horizontal, GameTheme.Spacing.sm)
                    .padding(.vertical, GameTheme.Spacing.xs)

                // Desk items — positioned relatively within the desk
                VStack(spacing: GameTheme.Spacing.lg) {
                    // Top row: Calendar on wall, Map on wall
                    HStack(spacing: GameTheme.Spacing.md) {
                        deskItem(
                            icon: "calendar",
                            label: "Calendar",
                            badge: 0,
                            color: GameTheme.Colors.accent
                        ) {
                            phoneOpenToApp = .calendar
                            showPhone = true
                        }

                        Spacer()

                        deskItem(
                            icon: "map",
                            label: "Map",
                            badge: 0,
                            color: GameTheme.Colors.zoneNeighborhood
                        ) {
                            // TODO: Open map overlay
                        }
                    }

                    Spacer()

                    // Middle: Laptop
                    deskItem(
                        icon: "laptopcomputer",
                        label: "Business",
                        badge: 0,
                        color: GameTheme.Colors.money
                    ) {
                        showLaptop = true
                    }

                    Spacer()

                    // Bottom row: Phone only
                    deskItem(
                        icon: "iphone",
                        label: "Phone",
                        badge: gameManager.inboxActivities.count + gameManager.pendingInquiries.count,
                        color: GameTheme.Colors.accent
                    ) {
                        phoneOpenToApp = nil
                        showPhone = true
                    }
                }
                .padding(GameTheme.Spacing.xl)
            }
        }
    }

    // MARK: - Desk Item

    @ViewBuilder
    private func deskItem(
        icon: String,
        label: String,
        badge: Int,
        color: Color,
        action: @escaping () -> Void
    ) -> some View {
        Button(action: action) {
            VStack(spacing: GameTheme.Spacing.xs) {
                ZStack(alignment: .topTrailing) {
                    ZStack {
                        RoundedRectangle(cornerRadius: GameTheme.Radius.medium)
                            .fill(GameTheme.Colors.surface)
                            .frame(width: 72, height: 72)

                        Image(systemName: icon)
                            .font(.system(size: 28))
                            .foregroundStyle(color)
                    }

                    if badge > 0 {
                        Text("\(badge)")
                            .font(GameTheme.Typography.micro)
                            .fontWeight(.bold)
                            .foregroundStyle(.white)
                            .frame(minWidth: GameTheme.Size.badgeSize, minHeight: GameTheme.Size.badgeSize)
                            .background(GameTheme.Colors.error)
                            .clipShape(Circle())
                            .offset(x: 6, y: -6)
                    }
                }

                Text(label)
                    .font(GameTheme.Typography.caption)
                    .foregroundStyle(GameTheme.Colors.textSecondary)
            }
        }
        .buttonStyle(DeskItemButtonStyle())
    }
}

// MARK: - Desk Item Button Style

struct DeskItemButtonStyle: ButtonStyle {
    func makeBody(configuration: Configuration) -> some View {
        configuration.label
            .scaleEffect(configuration.isPressed ? 0.95 : 1.0)
            .animation(.spring(response: 0.2, dampingFraction: 0.7), value: configuration.isPressed)
    }
}

// MARK: - Desk HUD

struct DeskHUDView: View {
    @Environment(GameManager.self) private var gameManager

    var body: some View {
        HStack {
            // Date
            VStack(alignment: .leading, spacing: 2) {
                Text(gameManager.currentDate.formatted)
                    .font(GameTheme.Typography.h3)
                    .foregroundStyle(GameTheme.Colors.textPrimary)
                Text("Stage: \(gameManager.playerData.stage.rawValue.capitalized)")
                    .font(GameTheme.Typography.micro)
                    .foregroundStyle(GameTheme.Colors.textMuted)
            }

            Spacer()

            // Money
            HStack(spacing: 4) {
                Text("$")
                    .foregroundStyle(GameTheme.Colors.money)
                Text("\(gameManager.playerData.money, specifier: "%.0f")")
                    .foregroundStyle(GameTheme.Colors.money)
            }
            .font(GameTheme.Typography.moneyLarge)

            Spacer()

            // Reputation
            HStack(spacing: 4) {
                Image(systemName: "star.fill")
                    .foregroundStyle(GameTheme.Colors.reputation)
                Text("\(gameManager.playerData.reputation)")
                    .foregroundStyle(GameTheme.Colors.reputation)
            }
            .font(GameTheme.Typography.h3)

            // Menu
            Menu {
                Button(action: { gameManager.showSettings() }) {
                    Label("Settings", systemImage: "gear")
                }
                Button(action: { gameManager.returnToMainMenu() }) {
                    Label("Main Menu", systemImage: "house")
                }
            } label: {
                Image(systemName: "line.3.horizontal")
                    .font(.system(size: 20))
                    .foregroundStyle(GameTheme.Colors.textMuted)
                    .frame(width: GameTheme.Size.touchTarget, height: GameTheme.Size.touchTarget)
            }
        }
        .padding(.horizontal, GameTheme.Spacing.md)
        .padding(.vertical, GameTheme.Spacing.sm)
        .background(GameTheme.Colors.surface)
    }
}

// MARK: - Advance Button

struct DeskAdvanceButton: View {
    @Environment(GameManager.self) private var gameManager
    let action: () -> Void

    private var hasInbox: Bool { gameManager.hasInboxItems }

    var body: some View {
        Button(action: action) {
            HStack(spacing: GameTheme.Spacing.xs) {
                Image(systemName: "forward.fill")
                if let nextPoint = gameManager.advanceSystem.findNextDecisionPoint() {
                    Text("Advance to \(nextPoint.date.formatted)")
                } else {
                    Text("Advance")
                }
            }
            .primaryButton()
            .opacity(hasInbox ? 0.4 : 1.0)
        }
        .disabled(hasInbox)
    }
}
