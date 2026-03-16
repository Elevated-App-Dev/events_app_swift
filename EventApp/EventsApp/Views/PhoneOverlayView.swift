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
        case .email:
            EmailInboxView()
        case .calendar:
            PhoneCalendarView()
        case .tasks:
            ProgressView_()
        default:
            placeholderAppView(title: app.title, icon: app.icon)
        }
    }

    // MARK: - Badge Counts

    private func badgeCount(for app: PhoneApp) -> Int {
        switch app {
        case .messages:
            // Only texts and calls that need action
            return gameManager.messageActivities.count
        case .email:
            // Emails that need action + pending inquiries
            return gameManager.emailActivities.count + gameManager.pendingInquiries.count
        case .calendar, .tasks:
            return 0  // Informational only — no actionable badge
        default:
            return 0
        }
    }
}

// MARK: - PhoneApp UI Extensions

extension PhoneApp {
    /// Apps shown on the phone home screen (excludes marketing for now).
    /// Apps shown on the phone home screen.
    /// Communication (Messages + Email) + Calendar + Progress.
    /// Business ops (vendors, CRM, financials) live in the laptop.
    static var homeScreenApps: [PhoneApp] {
        [.messages, .email, .calendar, .tasks]
    }

    var title: String {
        switch self {
        case .messages: return "Messages"
        case .email: return "Email"
        case .calendar: return "Calendar"
        case .tasks: return "Progress"
        case .bank: return "Bank"
        case .contacts: return "Contacts"
        case .reviews: return "Reviews"
        case .clients: return "Clients"
        case .marketing: return "Marketing"
        }
    }

    var icon: String {
        switch self {
        case .messages: return "message"
        case .email: return "envelope"
        case .calendar: return "calendar"
        case .tasks: return "checklist"
        case .bank: return "banknote"
        case .contacts: return "person.crop.rectangle.stack"
        case .reviews: return "star"
        case .clients: return "person.2"
        case .marketing: return "megaphone"
        }
    }

    var color: Color {
        switch self {
        case .messages: return GameTheme.Colors.accent
        case .email: return GameTheme.Colors.warning
        case .calendar: return GameTheme.Colors.warning
        case .tasks: return GameTheme.Colors.success
        case .bank: return GameTheme.Colors.money
        case .contacts: return GameTheme.Colors.textSecondary
        case .reviews: return GameTheme.Colors.reputation
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

    private let eventColors: [Color] = [
        GameTheme.Colors.accent, GameTheme.Colors.warning,
        GameTheme.Colors.money, GameTheme.Colors.reputation,
        GameTheme.Colors.success
    ]

    init() {
        _viewingMonth = State(initialValue: 3)
        _viewingYear = State(initialValue: 2026)
    }

    var body: some View {
        ScrollView {
            VStack(spacing: 0) {
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
                .padding(.bottom, GameTheme.Spacing.xs)

                // Day headers
                LazyVGrid(columns: Array(repeating: GridItem(.flexible()), count: 7), spacing: 0) {
                    ForEach(Self.dayNames, id: \.self) { day in
                        Text(day)
                            .font(GameTheme.Typography.micro)
                            .foregroundStyle(GameTheme.Colors.textMuted)
                            .frame(height: 20)
                    }
                }
                .padding(.horizontal, GameTheme.Spacing.sm)

                // Calendar grid (compact)
                LazyVGrid(columns: Array(repeating: GridItem(.flexible()), count: 7), spacing: 2) {
                    ForEach(0..<firstDayOffset, id: \.self) { _ in
                        Color.clear.frame(height: 38)
                    }

                    ForEach(1...daysInCurrentMonth, id: \.self) { day in
                        let date = GameDate(month: viewingMonth, day: day, year: viewingYear)
                        calendarCell(date: date)
                    }
                }
                .padding(.horizontal, GameTheme.Spacing.sm)

                Divider().background(GameTheme.Colors.border)
                    .padding(.vertical, GameTheme.Spacing.xs)

                // Event timeline bars
                eventTimeline
                    .padding(.horizontal, GameTheme.Spacing.md)

                Divider().background(GameTheme.Colors.border)
                    .padding(.vertical, GameTheme.Spacing.xs)

                // Upcoming deadlines
                upcomingDeadlines
                    .padding(.horizontal, GameTheme.Spacing.md)

                // Advance preview
                advancePreview
                    .padding(.horizontal, GameTheme.Spacing.md)
                    .padding(.top, GameTheme.Spacing.sm)

                // Selected date detail
                if let selected = selectedDate {
                    Divider().background(GameTheme.Colors.border)
                        .padding(.vertical, GameTheme.Spacing.xs)
                    selectedDateDetail(selected)
                }

                Spacer().frame(height: GameTheme.Spacing.lg)
            }
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
            VStack(spacing: 1) {
                ZStack {
                    if isToday {
                        Circle()
                            .fill(GameTheme.Colors.accent)
                            .frame(width: 26, height: 26)
                    }
                    Text("\(date.day)")
                        .font(.system(size: 13))
                        .fontWeight(isToday ? .bold : .regular)
                        .foregroundStyle(
                            isToday ? GameTheme.Colors.background :
                            date < gameManager.currentDate ? GameTheme.Colors.textMuted :
                            GameTheme.Colors.textPrimary
                        )
                }

                HStack(spacing: 2) {
                    if hasEvent {
                        Circle().fill(GameTheme.Colors.error).frame(width: 4, height: 4)
                    }
                    if hasActivity {
                        Circle().fill(GameTheme.Colors.accent.opacity(isToday ? 0.5 : 1.0)).frame(width: 4, height: 4)
                    }
                }
                .frame(height: 4)
            }
            .frame(height: 38)
            .frame(maxWidth: .infinity)
            .background(isSelected ? GameTheme.Colors.elevated : Color.clear)
            .clipShape(RoundedRectangle(cornerRadius: 6))
        }
    }

    // MARK: - Event Timeline

    private var eventTimeline: some View {
        VStack(alignment: .leading, spacing: GameTheme.Spacing.xs) {
            Text("Event Timeline")
                .font(GameTheme.Typography.micro)
                .fontWeight(.bold)
                .foregroundStyle(GameTheme.Colors.textMuted)

            if gameManager.activeEvents.isEmpty {
                Text("No active events")
                    .font(GameTheme.Typography.micro)
                    .foregroundStyle(GameTheme.Colors.textMuted)
            } else {
                // Show each event as a horizontal bar from accepted date to event date
                ForEach(Array(gameManager.activeEvents.enumerated()), id: \.element.id) { index, event in
                    let color = eventColors[index % eventColors.count]
                    let acceptedDay = event.acceptedDate?.day ?? gameManager.currentDate.day
                    let eventDay = event.eventDate.day
                    let totalDaysInMonth = daysInCurrentMonth

                    Button(action: {
                        eventDetailIndex = index
                    }) {
                        VStack(alignment: .leading, spacing: 2) {
                            // Event label
                            HStack(spacing: 4) {
                                Circle().fill(color).frame(width: 8, height: 8)
                                Text(event.eventTitle)
                                    .font(GameTheme.Typography.micro)
                                    .foregroundStyle(GameTheme.Colors.textPrimary)
                                    .lineLimit(1)
                                Spacer()
                                Text(event.eventDate.shortFormatted)
                                    .font(.system(size: 10))
                                    .foregroundStyle(GameTheme.Colors.textMuted)
                            }

                            // Timeline bar
                            GeometryReader { geo in
                                let width = geo.size.width
                                let startFrac = max(0, CGFloat(acceptedDay - 1) / CGFloat(totalDaysInMonth))
                                let endFrac = min(1, CGFloat(eventDay) / CGFloat(totalDaysInMonth))
                                let todayFrac = CGFloat(gameManager.currentDate.day - 1) / CGFloat(totalDaysInMonth)

                                ZStack(alignment: .leading) {
                                    // Track
                                    RoundedRectangle(cornerRadius: 2)
                                        .fill(GameTheme.Colors.border)
                                        .frame(height: 6)

                                    // Event span bar
                                    if event.eventDate.month == viewingMonth && event.eventDate.year == viewingYear {
                                        RoundedRectangle(cornerRadius: 2)
                                            .fill(color.opacity(0.4))
                                            .frame(width: max(4, (endFrac - startFrac) * width), height: 6)
                                            .offset(x: startFrac * width)
                                    }

                                    // Today marker
                                    if gameManager.currentDate.month == viewingMonth {
                                        Rectangle()
                                            .fill(GameTheme.Colors.textPrimary)
                                            .frame(width: 2, height: 10)
                                            .offset(x: todayFrac * width)
                                    }
                                }
                            }
                            .frame(height: 10)
                        }
                    }
                }
            }
        }
    }

    // MARK: - Upcoming Deadlines

    private var upcomingDeadlines: some View {
        let upcoming = gameManager.advanceSystem.scheduledActivities
            .filter { $0.status == .scheduled && $0.scheduledDate >= gameManager.currentDate }
            .sorted { $0.scheduledDate < $1.scheduledDate }
            .prefix(5)

        return VStack(alignment: .leading, spacing: GameTheme.Spacing.xs) {
            Text("Upcoming")
                .font(GameTheme.Typography.micro)
                .fontWeight(.bold)
                .foregroundStyle(GameTheme.Colors.textMuted)

            if upcoming.isEmpty {
                Text("Nothing scheduled ahead")
                    .font(GameTheme.Typography.micro)
                    .foregroundStyle(GameTheme.Colors.textMuted)
            } else {
                ForEach(Array(upcoming)) { activity in
                    HStack(spacing: GameTheme.Spacing.xs) {
                        Text(activity.scheduledDate.shortFormatted)
                            .font(.system(size: 11, weight: .medium).monospacedDigit())
                            .foregroundStyle(GameTheme.Colors.textMuted)
                            .frame(width: 65, alignment: .leading)

                        Circle()
                            .fill(deadlineColor(for: activity))
                            .frame(width: 6, height: 6)

                        Text(activity.content.subject)
                            .font(GameTheme.Typography.micro)
                            .foregroundStyle(GameTheme.Colors.textSecondary)
                            .lineLimit(1)
                    }
                }
            }
        }
    }

    private func deadlineColor(for activity: PlanningActivity) -> Color {
        switch activity.type {
        case .eventExecution: return GameTheme.Colors.error
        case .clientMeeting: return GameTheme.Colors.accent
        case .clientContractSent, .clientContractSigned: return GameTheme.Colors.accent
        case .vendorAvailabilityResponse, .vendorNegotiationResponse: return GameTheme.Colors.warning
        case .vendorFinalConfirmation: return GameTheme.Colors.warning
        default: return GameTheme.Colors.textMuted
        }
    }

    // MARK: - Advance Preview

    private var advancePreview: some View {
        Group {
            if let nextPoint = gameManager.advanceSystem.findNextDecisionPoint() {
                HStack(spacing: GameTheme.Spacing.xs) {
                    Image(systemName: "forward.fill")
                        .font(.system(size: 10))
                        .foregroundStyle(GameTheme.Colors.accent)
                    Text("Next advance: \(nextPoint.date.formatted)")
                        .font(GameTheme.Typography.micro)
                        .foregroundStyle(GameTheme.Colors.accent)
                    if nextPoint.activities.count > 0 {
                        Text("(\(nextPoint.activities.count) item\(nextPoint.activities.count == 1 ? "" : "s"))")
                            .font(.system(size: 10))
                            .foregroundStyle(GameTheme.Colors.textMuted)
                    }
                    Spacer()
                }
            }
        }
    }

    // MARK: - Selected Date Detail

    @ViewBuilder
    private func selectedDateDetail(_ date: GameDate) -> some View {
        let eventsOnDate = gameManager.activeEvents.filter { $0.eventDate == date }
        let activitiesOnDate = gameManager.advanceSystem.scheduledActivities.filter {
            $0.scheduledDate == date
        }

        VStack(alignment: .leading, spacing: GameTheme.Spacing.xs) {
            Text(date.formatted)
                .font(GameTheme.Typography.h3)
                .foregroundStyle(GameTheme.Colors.textPrimary)
                .padding(.horizontal, GameTheme.Spacing.md)

            ForEach(eventsOnDate) { event in
                Button(action: {
                    if let idx = gameManager.activeEvents.firstIndex(where: { $0.id == event.id }) {
                        eventDetailIndex = idx
                    }
                }) {
                    HStack {
                        Circle().fill(GameTheme.Colors.error).frame(width: 8, height: 8)
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

            ForEach(activitiesOnDate) { activity in
                HStack {
                    Circle()
                        .fill(activity.status == .completed ? GameTheme.Colors.success :
                              activity.status == .ready ? GameTheme.Colors.warning :
                              GameTheme.Colors.textMuted)
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
                    } else if activity.status == .ready {
                        Text("Action needed")
                            .font(.system(size: 10))
                            .foregroundStyle(GameTheme.Colors.warning)
                    } else {
                        Text("Scheduled")
                            .font(.system(size: 10))
                            .foregroundStyle(GameTheme.Colors.textMuted)
                    }
                }
                .padding(.horizontal, GameTheme.Spacing.md)
            }

            if eventsOnDate.isEmpty && activitiesOnDate.isEmpty {
                Text("Nothing scheduled")
                    .font(GameTheme.Typography.caption)
                    .foregroundStyle(GameTheme.Colors.textMuted)
                    .padding(.horizontal, GameTheme.Spacing.md)
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
        let yearsFromAnchor = viewingYear - 2026
        var daysSinceAnchor = yearsFromAnchor * 365
        daysSinceAnchor += Self.daysInMonth.prefix(viewingMonth - 1).reduce(0, +)
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

    /// Total service fees earned across all completed events.
    private var totalFeesEarned: Double {
        gameManager.transactions
            .filter { $0.category == .eventProfit }
            .reduce(0) { $0 + $1.amount }
    }

    /// Total client deposits received (held for vendor payments).
    private var totalClientDeposits: Double {
        gameManager.transactions
            .filter { $0.category == .clientDeposit }
            .reduce(0) { $0 + $1.amount }
    }

    /// Total vendor payments made (from event budgets).
    private var totalVendorPayments: Double {
        gameManager.transactions
            .filter { $0.category == .vendorPayment }
            .reduce(0) { $0 + abs($1.amount) }
    }

    var body: some View {
        ScrollView {
            VStack(spacing: 0) {
                // Your Money
                VStack(spacing: GameTheme.Spacing.sm) {
                    VStack(spacing: 4) {
                        Text("Your Money")
                            .font(GameTheme.Typography.micro)
                            .foregroundStyle(GameTheme.Colors.textMuted)
                        Text("$\(gameManager.playerData.money, specifier: "%.0f")")
                            .font(GameTheme.Typography.display)
                            .foregroundStyle(GameTheme.Colors.money)
                    }

                    // Breakdown
                    VStack(spacing: 6) {
                        bankRow("Starting capital", amount: startingBalance, color: GameTheme.Colors.textSecondary)
                        bankRow("Fees earned", amount: totalFeesEarned, color: GameTheme.Colors.success, showPlus: true)
                    }
                    .padding(.horizontal, GameTheme.Spacing.md)
                }
                .padding(.vertical, GameTheme.Spacing.md)

                Divider().background(GameTheme.Colors.border).padding(.horizontal, GameTheme.Spacing.md)

                // Event Funds Summary (client money flowing through)
                VStack(alignment: .leading, spacing: GameTheme.Spacing.xs) {
                    Text("Event Funds (Client Money)")
                        .font(GameTheme.Typography.micro)
                        .fontWeight(.bold)
                        .foregroundStyle(GameTheme.Colors.textMuted)

                    bankRow("Client deposits received", amount: totalClientDeposits, color: GameTheme.Colors.success, showPlus: true)
                    bankRow("Vendor payments made", amount: totalVendorPayments, color: GameTheme.Colors.error, showMinus: true)

                    let eventFundBalance = totalClientDeposits - totalVendorPayments
                    HStack {
                        Text("Event fund balance")
                            .font(GameTheme.Typography.caption)
                            .fontWeight(.medium)
                            .foregroundStyle(GameTheme.Colors.textPrimary)
                        Spacer()
                        Text("$\(eventFundBalance, specifier: "%.0f")")
                            .font(GameTheme.Typography.money)
                            .foregroundStyle(eventFundBalance >= 0 ? GameTheme.Colors.money : GameTheme.Colors.error)
                    }
                }
                .padding(.horizontal, GameTheme.Spacing.md)
                .padding(.vertical, GameTheme.Spacing.sm)

                Divider().background(GameTheme.Colors.border).padding(.horizontal, GameTheme.Spacing.md)

                // Transaction ledger
                VStack(alignment: .leading, spacing: 0) {
                    Text("All Transactions")
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
                        let reversed = Array(gameManager.transactions.reversed())

                        ForEach(Array(reversed.enumerated()), id: \.element.id) { index, tx in
                            HStack(alignment: .top) {
                                Text(tx.date.shortFormatted)
                                    .font(GameTheme.Typography.micro)
                                    .foregroundStyle(GameTheme.Colors.textMuted)
                                    .frame(width: 65, alignment: .leading)

                                VStack(alignment: .leading, spacing: 2) {
                                    Text(tx.description)
                                        .font(GameTheme.Typography.caption)
                                        .foregroundStyle(GameTheme.Colors.textPrimary)
                                        .lineLimit(2)

                                    Text(categoryLabel(tx.category))
                                        .font(.system(size: 10))
                                        .foregroundStyle(categoryColor(tx.category))
                                }
                                .frame(maxWidth: .infinity, alignment: .leading)

                                Text("\(tx.isIncome ? "+" : "")$\(abs(tx.amount), specifier: "%.0f")")
                                    .font(GameTheme.Typography.money)
                                    .foregroundStyle(tx.isIncome ? GameTheme.Colors.success : GameTheme.Colors.error)
                                    .frame(width: 75, alignment: .trailing)
                            }
                            .padding(.horizontal, GameTheme.Spacing.md)
                            .padding(.vertical, GameTheme.Spacing.xs)

                            if index < reversed.count - 1 {
                                Divider().background(GameTheme.Colors.border).padding(.horizontal, GameTheme.Spacing.md)
                            }
                        }
                    }
                }
            }
        }
    }

    @ViewBuilder
    private func bankRow(_ label: String, amount: Double, color: Color, showPlus: Bool = false, showMinus: Bool = false) -> some View {
        HStack {
            Text(label)
                .font(GameTheme.Typography.caption)
                .foregroundStyle(GameTheme.Colors.textMuted)
            Spacer()
            Text("\(showPlus ? "+" : showMinus ? "-" : "")$\(amount, specifier: "%.0f")")
                .font(GameTheme.Typography.money)
                .foregroundStyle(color)
        }
    }

    private func categoryLabel(_ category: TransactionKind) -> String {
        switch category {
        case .clientDeposit: return "Client Funds"
        case .clientPayment: return "Client Payment"
        case .vendorPayment: return "Event Budget"
        case .venuePayment: return "Event Budget"
        case .eventProfit: return "Your Fee"
        case .eventLoss: return "Loss"
        }
    }

    private func categoryColor(_ category: TransactionKind) -> Color {
        switch category {
        case .clientDeposit, .clientPayment: return GameTheme.Colors.accent
        case .vendorPayment, .venuePayment: return GameTheme.Colors.warning
        case .eventProfit: return GameTheme.Colors.success
        case .eventLoss: return GameTheme.Colors.error
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
