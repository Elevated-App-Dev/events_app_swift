import SwiftUI

/// The planner's phone — slides up from the bottom of the screen.
/// Contains 7 app icons in a grid. Tapping an app navigates to its view.
/// Close button returns to the desk.
struct PhoneOverlayView: View {
    @Environment(GameManager.self) private var gameManager
    @Binding var isPresented: Bool
    var initialApp: PhoneApp?
    @State private var selectedApp: PhoneApp?

    var body: some View {
        ZStack {
            // Dim background
            GameTheme.Colors.background.opacity(0.85)
                .ignoresSafeArea()
                .onTapGesture { isPresented = false }

            VStack(spacing: 0) {
                Spacer()

                // Phone frame
                VStack(spacing: GameTheme.Spacing.md) {
                    // Header
                    HStack {
                        Text(selectedApp?.title ?? "Phone")
                            .font(GameTheme.Typography.h2)
                            .foregroundStyle(GameTheme.Colors.textPrimary)

                        Spacer()

                        Button(action: {
                            if selectedApp != nil {
                                withAnimation(GameTheme.Anim.panelSlide) {
                                    selectedApp = nil
                                }
                            } else {
                                isPresented = false
                            }
                        }) {
                            Image(systemName: selectedApp != nil ? "chevron.left" : "xmark")
                                .font(.system(size: 18, weight: .medium))
                                .foregroundStyle(GameTheme.Colors.textSecondary)
                                .frame(width: GameTheme.Size.touchTarget, height: GameTheme.Size.touchTarget)
                        }
                    }
                    .padding(.horizontal, GameTheme.Spacing.md)
                    .padding(.top, GameTheme.Spacing.md)

                    // Content
                    if let app = selectedApp {
                        phoneAppContent(app)
                            .frame(maxHeight: .infinity)
                            .transition(.asymmetric(
                                insertion: .move(edge: .trailing),
                                removal: .move(edge: .leading)
                            ))
                    } else {
                        phoneHomeScreen
                            .transition(.opacity)
                    }
                }
                .frame(maxWidth: .infinity)
                .frame(height: UIScreen.main.bounds.height * 0.75)
                .background(GameTheme.Colors.background)
                .clipShape(RoundedRectangle(cornerRadius: GameTheme.Radius.large))
                .padding(.horizontal, GameTheme.Spacing.xs)
            }
        }
        .onAppear {
            if let app = initialApp {
                selectedApp = app
            }
        }
    }

    // MARK: - Home Screen Grid

    private var phoneHomeScreen: some View {
        let apps = PhoneApp.homeScreenApps
        let columns = [
            GridItem(.flexible(), spacing: GameTheme.Spacing.xl),
            GridItem(.flexible(), spacing: GameTheme.Spacing.xl),
            GridItem(.flexible(), spacing: GameTheme.Spacing.xl)
        ]

        return LazyVGrid(columns: columns, spacing: GameTheme.Spacing.xl) {
            ForEach(apps, id: \.self) { app in
                phoneAppIcon(app)
            }
        }
        .padding(.horizontal, GameTheme.Spacing.lg)
        .padding(.vertical, GameTheme.Spacing.xl)
        .frame(maxHeight: .infinity, alignment: .top)
    }

    @ViewBuilder
    private func phoneAppIcon(_ app: PhoneApp) -> some View {
        let badge = badgeCount(for: app)

        Button(action: {
            withAnimation(GameTheme.Anim.panelSlide) {
                selectedApp = app
            }
        }) {
            VStack(spacing: GameTheme.Spacing.xs) {
                ZStack(alignment: .topTrailing) {
                    Image(systemName: app.icon)
                        .font(.system(size: 28))
                        .foregroundStyle(app.color)
                        .frame(width: 56, height: 56)

                    if badge > 0 {
                        Text("\(badge)")
                            .font(GameTheme.Typography.micro)
                            .fontWeight(.bold)
                            .foregroundStyle(.white)
                            .frame(minWidth: GameTheme.Size.badgeSize, minHeight: GameTheme.Size.badgeSize)
                            .background(GameTheme.Colors.error)
                            .clipShape(Circle())
                            .offset(x: 8, y: -4)
                    }
                }

                Text(app.title)
                    .font(GameTheme.Typography.caption)
                    .foregroundStyle(GameTheme.Colors.textMuted)
            }
        }
        .buttonStyle(DeskItemButtonStyle())
    }

    // MARK: - App Content

    @ViewBuilder
    private func phoneAppContent(_ app: PhoneApp) -> some View {
        switch app {
        case .messages:
            InboxView()
        case .calendar:
            PhoneCalendarView()
        case .bank:
            PhoneBankView()
        case .contacts:
            PhoneContactsView()
        case .reviews:
            PhoneReviewsView()
        case .tasks:
            PhoneTasksView()
        case .clients:
            PhoneClientsView()
        case .marketing:
            placeholderAppView(title: "Marketing", icon: "megaphone")
        }
    }

    // MARK: - Badge Counts

    private func badgeCount(for app: PhoneApp) -> Int {
        switch app {
        case .messages:
            return gameManager.inboxActivities.count
        case .calendar:
            return gameManager.activeEvents.count
        case .clients:
            return gameManager.pendingInquiries.count
        default:
            return 0
        }
    }
}

// MARK: - PhoneApp UI Extensions

extension PhoneApp {
    /// Apps shown on the phone home screen (excludes marketing for now).
    static var homeScreenApps: [PhoneApp] {
        [.messages, .calendar, .bank, .contacts, .reviews, .tasks, .clients]
    }

    var title: String {
        switch self {
        case .messages: return "Messages"
        case .calendar: return "Calendar"
        case .bank: return "Bank"
        case .contacts: return "Contacts"
        case .reviews: return "Reviews"
        case .tasks: return "Tasks"
        case .clients: return "Clients"
        case .marketing: return "Marketing"
        }
    }

    var icon: String {
        switch self {
        case .messages: return "message"
        case .calendar: return "calendar"
        case .bank: return "banknote"
        case .contacts: return "person.crop.rectangle.stack"
        case .reviews: return "star"
        case .tasks: return "checklist"
        case .clients: return "person.2"
        case .marketing: return "megaphone"
        }
    }

    var color: Color {
        switch self {
        case .messages: return GameTheme.Colors.accent
        case .calendar: return GameTheme.Colors.warning
        case .bank: return GameTheme.Colors.money
        case .contacts: return GameTheme.Colors.textSecondary
        case .reviews: return GameTheme.Colors.reputation
        case .tasks: return GameTheme.Colors.success
        case .clients: return GameTheme.Colors.accent
        case .marketing: return GameTheme.Colors.warning
        }
    }
}

// MARK: - Placeholder Phone App Views

struct PhoneCalendarView: View {
    @Environment(GameManager.self) private var gameManager
    @State private var expandedEventId: String?

    var body: some View {
        ScrollView {
            VStack(alignment: .leading, spacing: GameTheme.Spacing.sm) {
                ForEach(gameManager.activeEvents) { event in
                    VStack(alignment: .leading, spacing: 0) {
                        Button(action: {
                            withAnimation(GameTheme.Anim.panelSlide) {
                                expandedEventId = expandedEventId == event.id ? nil : event.id
                            }
                        }) {
                            HStack {
                                VStack(alignment: .leading, spacing: 4) {
                                    Text(event.eventTitle)
                                        .font(GameTheme.Typography.h3)
                                        .foregroundStyle(GameTheme.Colors.textPrimary)
                                    Text(event.eventDate.formatted)
                                        .font(GameTheme.Typography.caption)
                                        .foregroundStyle(GameTheme.Colors.textSecondary)
                                }
                                Spacer()
                                PhaseBadge(phase: event.phase)
                            }
                        }

                        // Expanded: show notes from completed activities
                        if expandedEventId == event.id {
                            eventNotes(for: event.id)
                                .padding(.top, GameTheme.Spacing.sm)
                        }
                    }
                    .surfaceCard()
                }

                if gameManager.activeEvents.isEmpty {
                    Text("No upcoming events")
                        .font(GameTheme.Typography.body)
                        .foregroundStyle(GameTheme.Colors.textMuted)
                        .frame(maxWidth: .infinity, alignment: .center)
                        .padding(.top, GameTheme.Spacing.xl)
                }
            }
            .padding(.horizontal, GameTheme.Spacing.md)
        }
    }

    @ViewBuilder
    private func eventNotes(for eventId: String) -> some View {
        let completedActivities = gameManager.advanceSystem
            .getActivitiesForEvent(eventId: eventId)
            .filter { $0.status == .completed }

        if completedActivities.isEmpty {
            Text("No notes yet")
                .font(GameTheme.Typography.caption)
                .foregroundStyle(GameTheme.Colors.textMuted)
        } else {
            VStack(alignment: .leading, spacing: GameTheme.Spacing.xs) {
                Text("Notes")
                    .font(GameTheme.Typography.micro)
                    .fontWeight(.bold)
                    .foregroundStyle(GameTheme.Colors.textMuted)

                ForEach(completedActivities) { activity in
                    VStack(alignment: .leading, spacing: 4) {
                        HStack(spacing: 4) {
                            Image(systemName: activity.medium == .call ? "phone.fill" : "envelope.fill")
                                .font(GameTheme.Typography.micro)
                            Text(activity.content.subject)
                                .font(GameTheme.Typography.caption)
                                .fontWeight(.medium)
                            Spacer()
                            Text(activity.scheduledDate.shortFormatted)
                                .font(GameTheme.Typography.micro)
                                .foregroundStyle(GameTheme.Colors.textMuted)
                        }
                        .foregroundStyle(GameTheme.Colors.textSecondary)

                        // Show transcript if it exists
                        if let transcript = activity.content.dialogueTranscript, !transcript.isEmpty {
                            VStack(alignment: .leading, spacing: 4) {
                                ForEach(Array(transcript.enumerated()), id: \.offset) { _, line in
                                    HStack(alignment: .top, spacing: 6) {
                                        Text(line.speaker == .client ? "Them:" : "You:")
                                            .font(GameTheme.Typography.micro)
                                            .fontWeight(.bold)
                                            .foregroundStyle(line.speaker == .client ? GameTheme.Colors.accent : GameTheme.Colors.success)
                                            .frame(width: 36, alignment: .leading)
                                        Text(line.text)
                                            .font(GameTheme.Typography.micro)
                                            .foregroundStyle(GameTheme.Colors.textSecondary)
                                    }
                                }
                            }
                            .padding(GameTheme.Spacing.xs)
                            .background(GameTheme.Colors.elevated)
                            .clipShape(RoundedRectangle(cornerRadius: GameTheme.Radius.small))
                        }
                    }
                }
            }
        }
    }
}

struct PhoneBankView: View {
    @Environment(GameManager.self) private var gameManager

    var body: some View {
        ScrollView {
            VStack(spacing: GameTheme.Spacing.lg) {
                // Balance
                VStack(spacing: GameTheme.Spacing.xs) {
                    Text("Balance")
                        .font(GameTheme.Typography.caption)
                        .foregroundStyle(GameTheme.Colors.textMuted)
                    Text("$\(gameManager.playerData.money, specifier: "%.0f")")
                        .font(GameTheme.Typography.display)
                        .foregroundStyle(GameTheme.Colors.money)
                }
                .frame(maxWidth: .infinity)
                .padding(.top, GameTheme.Spacing.md)

                // Transaction history
                if !gameManager.transactions.isEmpty {
                    VStack(alignment: .leading, spacing: GameTheme.Spacing.sm) {
                        Text("Recent Transactions")
                            .font(GameTheme.Typography.h3)
                            .foregroundStyle(GameTheme.Colors.textPrimary)
                            .padding(.horizontal, GameTheme.Spacing.md)

                        ForEach(gameManager.transactions.reversed()) { tx in
                            HStack {
                                VStack(alignment: .leading, spacing: 2) {
                                    Text(tx.description)
                                        .font(GameTheme.Typography.caption)
                                        .foregroundStyle(GameTheme.Colors.textPrimary)
                                    Text(tx.date.shortFormatted)
                                        .font(GameTheme.Typography.micro)
                                        .foregroundStyle(GameTheme.Colors.textMuted)
                                }
                                Spacer()
                                Text("\(tx.isIncome ? "+" : "")$\(abs(tx.amount), specifier: "%.0f")")
                                    .font(GameTheme.Typography.money)
                                    .foregroundStyle(tx.isIncome ? GameTheme.Colors.success : GameTheme.Colors.error)
                            }
                            .surfaceCard()
                            .padding(.horizontal, GameTheme.Spacing.md)
                        }
                    }
                } else {
                    Text("No transactions yet")
                        .font(GameTheme.Typography.body)
                        .foregroundStyle(GameTheme.Colors.textMuted)
                        .padding(.top, GameTheme.Spacing.lg)
                }
            }
        }
    }
}

struct PhoneContactsView: View {
    var body: some View {
        placeholderAppView(title: "Contacts", icon: "person.crop.rectangle.stack")
    }
}

struct PhoneReviewsView: View {
    @Environment(GameManager.self) private var gameManager

    var body: some View {
        VStack(spacing: GameTheme.Spacing.lg) {
            HStack(spacing: GameTheme.Spacing.xs) {
                Image(systemName: "star.fill")
                    .foregroundStyle(GameTheme.Colors.reputation)
                Text("\(gameManager.playerData.reputation)")
                    .font(GameTheme.Typography.display)
                    .foregroundStyle(GameTheme.Colors.reputation)
            }
            .padding(.top, GameTheme.Spacing.xl)

            Text("Reputation")
                .font(GameTheme.Typography.caption)
                .foregroundStyle(GameTheme.Colors.textMuted)

            Spacer()
        }
    }
}

struct PhoneTasksView: View {
    var body: some View {
        placeholderAppView(title: "Tasks", icon: "checklist")
    }
}

struct PhoneClientsView: View {
    @Environment(GameManager.self) private var gameManager

    var body: some View {
        ScrollView {
            VStack(alignment: .leading, spacing: GameTheme.Spacing.sm) {
                if !gameManager.pendingInquiries.isEmpty {
                    Text("Pending Inquiries")
                        .font(GameTheme.Typography.h3)
                        .foregroundStyle(GameTheme.Colors.textPrimary)
                        .padding(.horizontal, GameTheme.Spacing.md)

                    ForEach(gameManager.pendingInquiries) { inquiry in
                        HStack {
                            VStack(alignment: .leading, spacing: 4) {
                                Text(inquiry.clientName)
                                    .font(GameTheme.Typography.h3)
                                    .foregroundStyle(GameTheme.Colors.textPrimary)
                                Text(inquiry.subCategory)
                                    .font(GameTheme.Typography.caption)
                                    .foregroundStyle(GameTheme.Colors.textSecondary)
                                Text("$\(inquiry.budget) \u{00B7} \(inquiry.guestCount) guests")
                                    .font(GameTheme.Typography.micro)
                                    .foregroundStyle(GameTheme.Colors.textMuted)
                            }
                            Spacer()
                            VStack(spacing: GameTheme.Spacing.xs) {
                                Button("Accept") {
                                    gameManager.acceptInquiry(inquiry)
                                }
                                .font(GameTheme.Typography.micro)
                                .foregroundStyle(GameTheme.Colors.success)

                                Button("Decline") {
                                    gameManager.declineInquiry(inquiry)
                                }
                                .font(GameTheme.Typography.micro)
                                .foregroundStyle(GameTheme.Colors.error)
                            }
                        }
                        .surfaceCard()
                        .padding(.horizontal, GameTheme.Spacing.md)
                    }
                }

                if gameManager.pendingInquiries.isEmpty {
                    Text("No pending inquiries")
                        .font(GameTheme.Typography.body)
                        .foregroundStyle(GameTheme.Colors.textMuted)
                        .frame(maxWidth: .infinity, alignment: .center)
                        .padding(.top, GameTheme.Spacing.xl)
                }
            }
        }
    }
}

@ViewBuilder
private func placeholderAppView(title: String, icon: String) -> some View {
    VStack(spacing: GameTheme.Spacing.md) {
        Image(systemName: icon)
            .font(.system(size: 48))
            .foregroundStyle(GameTheme.Colors.textMuted)
        Text("Coming soon")
            .font(GameTheme.Typography.body)
            .foregroundStyle(GameTheme.Colors.textMuted)
    }
    .frame(maxWidth: .infinity, maxHeight: .infinity)
}
