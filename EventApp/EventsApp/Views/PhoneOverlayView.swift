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
    /// Apps shown on the phone home screen. Clients moved to laptop CRM.
    static var homeScreenApps: [PhoneApp] {
        [.messages, .calendar, .bank, .contacts, .reviews, .tasks]
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
    @State private var viewingMonth: Int
    @State private var viewingYear: Int
    @State private var selectedDate: GameDate?
    @State private var eventDetailIndex: Int?

    private static let dayNames = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"]
    private static let daysInMonth = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31]

    init() {
        _viewingMonth = State(initialValue: 3)
        _viewingYear = State(initialValue: 2026)
    }

    var body: some View {
        VStack(spacing: GameTheme.Spacing.sm) {
            // Month navigation
            HStack {
                Button(action: previousMonth) {
                    Image(systemName: "chevron.left")
                        .foregroundStyle(GameTheme.Colors.textSecondary)
                        .frame(width: GameTheme.Size.touchTarget, height: GameTheme.Size.touchTarget)
                }
                Spacer()
                Text(monthYearLabel)
                    .font(GameTheme.Typography.h2)
                    .foregroundStyle(GameTheme.Colors.textPrimary)
                Spacer()
                Button(action: nextMonth) {
                    Image(systemName: "chevron.right")
                        .foregroundStyle(GameTheme.Colors.textSecondary)
                        .frame(width: GameTheme.Size.touchTarget, height: GameTheme.Size.touchTarget)
                }
            }
            .padding(.horizontal, GameTheme.Spacing.md)

            // Day headers
            LazyVGrid(columns: Array(repeating: GridItem(.flexible()), count: 7), spacing: 0) {
                ForEach(Self.dayNames, id: \.self) { day in
                    Text(day)
                        .font(GameTheme.Typography.micro)
                        .foregroundStyle(GameTheme.Colors.textMuted)
                        .frame(height: 24)
                }
            }
            .padding(.horizontal, GameTheme.Spacing.sm)

            // Calendar grid
            LazyVGrid(columns: Array(repeating: GridItem(.flexible()), count: 7), spacing: 4) {
                // Empty cells for offset (simple: start all months on Sunday for now)
                ForEach(0..<firstDayOffset, id: \.self) { _ in
                    Color.clear.frame(height: 44)
                }

                ForEach(1...daysInCurrentMonth, id: \.self) { day in
                    let date = GameDate(month: viewingMonth, day: day, year: viewingYear)
                    calendarCell(date: date)
                }
            }
            .padding(.horizontal, GameTheme.Spacing.sm)

            // Selected date details
            if let selected = selectedDate {
                selectedDateDetail(selected)
            }

            Spacer()
        }
        .onAppear {
            viewingMonth = gameManager.currentDate.month
            viewingYear = gameManager.currentDate.year
            selectedDate = gameManager.currentDate
        }
        .sheet(item: Binding(
            get: { eventDetailIndex.map { IdentifiableIndex(value: $0) } },
            set: { eventDetailIndex = $0?.value }
        )) { item in
            NavigationStack {
                EventDetailView(eventIndex: item.value)
            }
        }
    }

    // MARK: - Calendar Cell

    @ViewBuilder
    private func calendarCell(date: GameDate) -> some View {
        let isToday = date == gameManager.currentDate
        let isSelected = date == selectedDate
        let hasEvent = gameManager.activeEvents.contains { $0.eventDate == date }
        let hasActivity = gameManager.advanceSystem.scheduledActivities.contains {
            $0.scheduledDate == date && ($0.status == .scheduled || $0.status == .ready)
        }

        Button(action: { selectedDate = date }) {
            VStack(spacing: 2) {
                ZStack {
                    // Today indicator — filled accent circle
                    if isToday {
                        Circle()
                            .fill(GameTheme.Colors.accent)
                            .frame(width: 28, height: 28)
                    }

                    Text("\(date.day)")
                        .font(GameTheme.Typography.caption)
                        .fontWeight(isToday ? .bold : .regular)
                        .foregroundStyle(
                            isToday ? GameTheme.Colors.background :
                            date < gameManager.currentDate ? GameTheme.Colors.textMuted :
                            GameTheme.Colors.textPrimary
                        )
                }

                // Dots indicating content
                HStack(spacing: 2) {
                    if hasEvent {
                        Circle()
                            .fill(GameTheme.Colors.error)
                            .frame(width: 5, height: 5)
                    }
                    if hasActivity {
                        Circle()
                            .fill(GameTheme.Colors.accent.opacity(isToday ? 0.5 : 1.0))
                            .frame(width: 5, height: 5)
                    }
                }
                .frame(height: 5)
            }
            .frame(height: 44)
            .frame(maxWidth: .infinity)
            .background(
                isSelected ? GameTheme.Colors.elevated :
                Color.clear
            )
            .clipShape(RoundedRectangle(cornerRadius: GameTheme.Radius.small))
        }
    }

    // MARK: - Selected Date Detail

    @ViewBuilder
    private func selectedDateDetail(_ date: GameDate) -> some View {
        let eventsOnDate = gameManager.activeEvents.filter { $0.eventDate == date }
        let activitiesOnDate = gameManager.advanceSystem.scheduledActivities.filter {
            $0.scheduledDate == date
        }

        ScrollView {
            VStack(alignment: .leading, spacing: GameTheme.Spacing.xs) {
                Text(date.formatted)
                    .font(GameTheme.Typography.h3)
                    .foregroundStyle(GameTheme.Colors.textPrimary)
                    .padding(.horizontal, GameTheme.Spacing.md)

                // Events on this date
                ForEach(eventsOnDate) { event in
                    Button(action: {
                        if let idx = gameManager.activeEvents.firstIndex(where: { $0.id == event.id }) {
                            eventDetailIndex = idx
                        }
                    }) {
                        HStack {
                            Circle()
                                .fill(GameTheme.Colors.error)
                                .frame(width: 8, height: 8)
                            VStack(alignment: .leading, spacing: 2) {
                                Text(event.eventTitle)
                                    .font(GameTheme.Typography.caption)
                                    .foregroundStyle(GameTheme.Colors.textPrimary)
                                Text("Tap to manage")
                                    .font(GameTheme.Typography.micro)
                                    .foregroundStyle(GameTheme.Colors.accent)
                            }
                            Spacer()
                            PhaseBadge(phase: event.phase)
                        }
                    }
                    .padding(.horizontal, GameTheme.Spacing.md)
                }

                // Activities on this date
                ForEach(activitiesOnDate) { activity in
                    HStack {
                        Circle()
                            .fill(activity.status == .completed ? GameTheme.Colors.success : GameTheme.Colors.accent)
                            .frame(width: 8, height: 8)
                        Text(activity.content.subject)
                            .font(GameTheme.Typography.caption)
                            .foregroundStyle(GameTheme.Colors.textSecondary)
                            .lineLimit(1)
                        Spacer()
                        if activity.status == .completed {
                            Image(systemName: "checkmark")
                                .font(GameTheme.Typography.micro)
                                .foregroundStyle(GameTheme.Colors.success)
                        }
                    }
                    .padding(.horizontal, GameTheme.Spacing.md)

                    // Show transcript for completed meetings
                    if activity.status == .completed,
                       let transcript = activity.content.dialogueTranscript, !transcript.isEmpty {
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
                        .padding(.horizontal, GameTheme.Spacing.md)
                    }
                }

                if eventsOnDate.isEmpty && activitiesOnDate.isEmpty {
                    Text("Nothing scheduled")
                        .font(GameTheme.Typography.caption)
                        .foregroundStyle(GameTheme.Colors.textMuted)
                        .padding(.horizontal, GameTheme.Spacing.md)
                }
            }
        }
    }

    // MARK: - Helpers

    private var monthYearLabel: String {
        let names = ["January", "February", "March", "April", "May", "June",
                     "July", "August", "September", "October", "November", "December"]
        return "\(names[viewingMonth - 1]) \(viewingYear)"
    }

    private var daysInCurrentMonth: Int {
        Self.daysInMonth[viewingMonth - 1]
    }

    /// Day-of-week offset for the 1st of the viewing month (Sun=0, Sat=6).
    /// Uses Jan 1, 2026 = Thursday (offset 4) as anchor.
    private var firstDayOffset: Int {
        // Days from Jan 1, 2026 to the 1st of viewingMonth/viewingYear
        let yearsFromAnchor = viewingYear - 2026
        var daysSinceAnchor = yearsFromAnchor * 365
        // Add leap days (simplified: no leap years in game for now)
        daysSinceAnchor += Self.daysInMonth.prefix(viewingMonth - 1).reduce(0, +)
        // Jan 1, 2026 is Thursday = offset 4 (Sun=0 grid)
        return (daysSinceAnchor + 4) % 7
    }

    private func previousMonth() {
        if viewingMonth == 1 {
            viewingMonth = 12
            viewingYear -= 1
        } else {
            viewingMonth -= 1
        }
        selectedDate = nil
    }

    private func nextMonth() {
        if viewingMonth == 12 {
            viewingMonth = 1
            viewingYear += 1
        } else {
            viewingMonth += 1
        }
        selectedDate = nil
    }
}

struct PhoneBankView: View {
    @Environment(GameManager.self) private var gameManager

    private let startingBalance: Double = 500.0

    var body: some View {
        ScrollView {
            VStack(spacing: 0) {
                // Account summary
                VStack(spacing: GameTheme.Spacing.md) {
                    // Current balance (large)
                    VStack(spacing: 4) {
                        Text("Current Balance")
                            .font(GameTheme.Typography.micro)
                            .foregroundStyle(GameTheme.Colors.textMuted)
                        Text("$\(gameManager.playerData.money, specifier: "%.2f")")
                            .font(GameTheme.Typography.display)
                            .foregroundStyle(GameTheme.Colors.money)
                    }

                    // Starting balance
                    HStack {
                        Text("Starting Balance")
                            .font(GameTheme.Typography.caption)
                            .foregroundStyle(GameTheme.Colors.textMuted)
                        Spacer()
                        Text("$\(startingBalance, specifier: "%.2f")")
                            .font(GameTheme.Typography.money)
                            .foregroundStyle(GameTheme.Colors.textSecondary)
                    }
                    .padding(.horizontal, GameTheme.Spacing.md)

                    // Net change
                    let netChange = gameManager.playerData.money - startingBalance
                    HStack {
                        Text("Net Change")
                            .font(GameTheme.Typography.caption)
                            .foregroundStyle(GameTheme.Colors.textMuted)
                        Spacer()
                        Text("\(netChange >= 0 ? "+" : "")$\(netChange, specifier: "%.2f")")
                            .font(GameTheme.Typography.money)
                            .foregroundStyle(netChange >= 0 ? GameTheme.Colors.success : GameTheme.Colors.error)
                    }
                    .padding(.horizontal, GameTheme.Spacing.md)

                    Divider()
                        .background(GameTheme.Colors.border)
                        .padding(.horizontal, GameTheme.Spacing.md)
                }
                .padding(.top, GameTheme.Spacing.md)

                // Transaction ledger
                VStack(alignment: .leading, spacing: 0) {
                    Text("Transactions")
                        .font(GameTheme.Typography.h3)
                        .foregroundStyle(GameTheme.Colors.textPrimary)
                        .padding(.horizontal, GameTheme.Spacing.md)
                        .padding(.vertical, GameTheme.Spacing.sm)

                    if gameManager.transactions.isEmpty {
                        Text("No transactions yet")
                            .font(GameTheme.Typography.caption)
                            .foregroundStyle(GameTheme.Colors.textMuted)
                            .frame(maxWidth: .infinity)
                            .padding(.vertical, GameTheme.Spacing.lg)
                    } else {
                        // Show transactions in reverse order with running balance
                        let reversed = Array(gameManager.transactions.reversed())
                        var runningBalance = gameManager.playerData.money

                        ForEach(Array(reversed.enumerated()), id: \.element.id) { index, tx in
                            let balanceAfter = balanceAfterTransaction(index: index)

                            HStack(alignment: .top) {
                                // Date
                                Text(tx.date.shortFormatted)
                                    .font(GameTheme.Typography.micro)
                                    .foregroundStyle(GameTheme.Colors.textMuted)
                                    .frame(width: 70, alignment: .leading)

                                // Description
                                VStack(alignment: .leading, spacing: 2) {
                                    Text(tx.description)
                                        .font(GameTheme.Typography.caption)
                                        .foregroundStyle(GameTheme.Colors.textPrimary)
                                        .lineLimit(1)
                                }
                                .frame(maxWidth: .infinity, alignment: .leading)

                                // Amount
                                Text("\(tx.isIncome ? "+" : "")\(tx.amount, specifier: "%.0f")")
                                    .font(GameTheme.Typography.money)
                                    .foregroundStyle(tx.isIncome ? GameTheme.Colors.success : GameTheme.Colors.error)
                                    .frame(width: 80, alignment: .trailing)
                            }
                            .padding(.horizontal, GameTheme.Spacing.md)
                            .padding(.vertical, GameTheme.Spacing.xs)

                            if index < reversed.count - 1 {
                                Divider()
                                    .background(GameTheme.Colors.border)
                                    .padding(.horizontal, GameTheme.Spacing.md)
                            }
                        }
                    }
                }
            }
        }
    }

    private func balanceAfterTransaction(index: Int) -> Double {
        let reversed = Array(gameManager.transactions.reversed())
        var balance = gameManager.playerData.money
        for i in 0..<index {
            balance -= reversed[i].amount
        }
        return balance
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
    @State private var expandedClientId: String?

    var body: some View {
        ScrollView {
            VStack(alignment: .leading, spacing: GameTheme.Spacing.sm) {
                // Active clients
                if !gameManager.activeEvents.isEmpty {
                    ForEach(gameManager.activeEvents) { event in
                        VStack(alignment: .leading, spacing: 0) {
                            Button(action: {
                                withAnimation(GameTheme.Anim.panelSlide) {
                                    expandedClientId = expandedClientId == event.id ? nil : event.id
                                }
                            }) {
                                HStack {
                                    VStack(alignment: .leading, spacing: 4) {
                                        Text(event.clientName)
                                            .font(GameTheme.Typography.h3)
                                            .foregroundStyle(GameTheme.Colors.textPrimary)
                                        Text(event.eventTitle)
                                            .font(GameTheme.Typography.caption)
                                            .foregroundStyle(GameTheme.Colors.textSecondary)
                                    }
                                    Spacer()
                                    PhaseBadge(phase: event.phase)
                                }

                                HStack(spacing: GameTheme.Spacing.sm) {
                                    Label("$\(event.budget.total, specifier: "%.0f")", systemImage: "banknote")
                                        .font(GameTheme.Typography.micro)
                                        .foregroundStyle(GameTheme.Colors.money)
                                    Label("\(event.guestCount)", systemImage: "person.2")
                                        .font(GameTheme.Typography.micro)
                                        .foregroundStyle(GameTheme.Colors.textMuted)
                                    Label(event.eventDate.shortFormatted, systemImage: "calendar")
                                        .font(GameTheme.Typography.micro)
                                        .foregroundStyle(GameTheme.Colors.textMuted)
                                }
                            }

                            // Expanded: show meeting notes / transcripts
                            if expandedClientId == event.id {
                                clientNotes(for: event.id)
                                    .padding(.top, GameTheme.Spacing.sm)
                            }
                        }
                        .surfaceCard()
                    }
                }

                // Past clients
                if !gameManager.completedEvents.isEmpty {
                    Text("Past Clients")
                        .font(GameTheme.Typography.micro)
                        .fontWeight(.bold)
                        .foregroundStyle(GameTheme.Colors.textMuted)
                        .padding(.top, GameTheme.Spacing.sm)

                    ForEach(gameManager.completedEvents.suffix(10).reversed()) { event in
                        HStack {
                            VStack(alignment: .leading, spacing: 2) {
                                Text(event.clientName)
                                    .font(GameTheme.Typography.caption)
                                    .foregroundStyle(GameTheme.Colors.textPrimary)
                                Text(event.eventTitle)
                                    .font(GameTheme.Typography.micro)
                                    .foregroundStyle(GameTheme.Colors.textMuted)
                            }
                            Spacer()
                            if let satisfaction = event.results?.finalSatisfaction {
                                Text("\(Int(satisfaction))%")
                                    .font(GameTheme.Typography.micro)
                                    .fontWeight(.bold)
                                    .foregroundStyle(satisfaction >= 70 ? GameTheme.Colors.success : GameTheme.Colors.warning)
                            }
                        }
                        .surfaceCard()
                    }
                }

                if gameManager.activeEvents.isEmpty && gameManager.completedEvents.isEmpty {
                    VStack(spacing: GameTheme.Spacing.sm) {
                        Image(systemName: "person.2")
                            .font(.system(size: 48))
                            .foregroundStyle(GameTheme.Colors.textMuted)
                        Text("No clients yet")
                            .font(GameTheme.Typography.body)
                            .foregroundStyle(GameTheme.Colors.textMuted)
                    }
                    .frame(maxWidth: .infinity)
                    .padding(.top, GameTheme.Spacing.xl)
                }
            }
            .padding(.horizontal, GameTheme.Spacing.md)
        }
    }

    @ViewBuilder
    private func clientNotes(for eventId: String) -> some View {
        let completedActivities = gameManager.advanceSystem
            .getActivitiesForEvent(eventId: eventId)
            .filter { $0.status == .completed }

        VStack(alignment: .leading, spacing: GameTheme.Spacing.xs) {
            Text("Meeting Notes")
                .font(GameTheme.Typography.micro)
                .fontWeight(.bold)
                .foregroundStyle(GameTheme.Colors.textMuted)

            if completedActivities.isEmpty {
                Text("No notes yet")
                    .font(GameTheme.Typography.caption)
                    .foregroundStyle(GameTheme.Colors.textMuted)
            } else {
                ForEach(completedActivities) { activity in
                    // Show transcript for meetings/calls
                    if let transcript = activity.content.dialogueTranscript, !transcript.isEmpty {
                        VStack(alignment: .leading, spacing: 4) {
                            HStack(spacing: 4) {
                                Image(systemName: "phone.fill")
                                    .font(GameTheme.Typography.micro)
                                Text("Call — \(activity.scheduledDate.shortFormatted)")
                                    .font(GameTheme.Typography.micro)
                                    .fontWeight(.medium)
                            }
                            .foregroundStyle(GameTheme.Colors.accent)

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

/// Helper to make Int usable with .sheet(item:)
struct IdentifiableIndex: Identifiable {
    let id: Int
    let value: Int
    init(value: Int) { self.id = value; self.value = value }
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
