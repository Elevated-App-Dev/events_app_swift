import SwiftUI

/// Formal email inbox — threaded conversations grouped by contact.
/// Shows subject line, sender, preview. Tap to expand the thread.
/// Contracts, vendor quotes, availability responses, invoices arrive here.
struct EmailInboxView: View {
    @Environment(GameManager.self) private var gameManager
    @State private var expandedThreadContact: String?

    /// Email threads grouped by contact, newest first.
    private var emailThreads: [EmailThread] {
        let allEmails = gameManager.advanceSystem.scheduledActivities
            .filter { $0.medium == .email
                      && ($0.status == .ready || $0.status == .completed || $0.status == .overdue)
                      && $0.scheduledDate <= gameManager.currentDate }

        var threadMap: [String: [PlanningActivity]] = [:]
        for email in allEmails {
            let contact = gameManager.resolveContactName(for: email)
            threadMap[contact, default: []].append(email)
        }

        return threadMap.map { contact, emails in
            let sorted = emails.sorted { $0.scheduledDate < $1.scheduledDate }
            let unread = emails.filter { $0.status == .ready }.count
            return EmailThread(
                contactName: contact,
                emails: sorted,
                unreadCount: unread,
                latestDate: sorted.last?.scheduledDate ?? gameManager.currentDate,
                latestSubject: sorted.last?.content.subject ?? ""
            )
        }
        .sorted { lhs, rhs in
            if lhs.unreadCount > 0 && rhs.unreadCount == 0 { return true }
            if lhs.unreadCount == 0 && rhs.unreadCount > 0 { return false }
            return lhs.latestDate > rhs.latestDate
        }
    }

    var body: some View {
        ScrollView {
            VStack(spacing: 0) {
                // Pending inquiries
                ForEach(gameManager.pendingInquiries) { inquiry in
                    inquiryRow(inquiry)
                    Divider().background(GameTheme.Colors.border)
                }

                // Email threads
                ForEach(emailThreads) { thread in
                    VStack(spacing: 0) {
                        threadHeaderRow(thread)

                        if expandedThreadContact == thread.contactName {
                            threadDetail(thread)
                        }
                    }
                    Divider().background(GameTheme.Colors.border)
                }
            }

            if emailThreads.isEmpty && gameManager.pendingInquiries.isEmpty {
                VStack(spacing: GameTheme.Spacing.sm) {
                    Image(systemName: "envelope.open")
                        .font(.system(size: 48))
                        .foregroundStyle(GameTheme.Colors.textMuted)
                    Text("No emails")
                        .font(GameTheme.Typography.body)
                        .foregroundStyle(GameTheme.Colors.textMuted)
                }
                .frame(maxWidth: .infinity)
                .padding(.top, GameTheme.Spacing.xl)
            }
        }
    }

    // MARK: - Thread Header

    @ViewBuilder
    private func threadHeaderRow(_ thread: EmailThread) -> some View {
        let isExpanded = expandedThreadContact == thread.contactName

        Button(action: {
            withAnimation(GameTheme.Anim.panelSlide) {
                expandedThreadContact = isExpanded ? nil : thread.contactName
            }
        }) {
            HStack(alignment: .top, spacing: GameTheme.Spacing.sm) {
                // Unread indicator
                Circle()
                    .fill(thread.hasUnread ? GameTheme.Colors.accent : Color.clear)
                    .frame(width: 8, height: 8)
                    .padding(.top, 6)

                VStack(alignment: .leading, spacing: 4) {
                    HStack {
                        Text(thread.contactName)
                            .font(GameTheme.Typography.caption)
                            .fontWeight(thread.hasUnread ? .bold : .regular)
                            .foregroundStyle(GameTheme.Colors.textPrimary)

                        if thread.emails.count > 1 {
                            Text("(\(thread.emails.count))")
                                .font(GameTheme.Typography.micro)
                                .foregroundStyle(GameTheme.Colors.textMuted)
                        }

                        Spacer()

                        Text(thread.latestDate.shortFormatted)
                            .font(GameTheme.Typography.micro)
                            .foregroundStyle(GameTheme.Colors.textMuted)
                    }

                    Text(thread.latestSubject)
                        .font(GameTheme.Typography.caption)
                        .foregroundStyle(thread.hasUnread ? GameTheme.Colors.textPrimary : GameTheme.Colors.textSecondary)
                        .lineLimit(1)

                    // Preview of latest email body
                    if !isExpanded, let latest = thread.emails.last, !latest.content.body.isEmpty {
                        Text(latest.content.body.prefix(100).replacingOccurrences(of: "\n", with: " "))
                            .font(GameTheme.Typography.micro)
                            .foregroundStyle(GameTheme.Colors.textMuted)
                            .lineLimit(1)
                    }
                }

                if thread.hasUnread {
                    Text("\(thread.unreadCount)")
                        .font(GameTheme.Typography.micro)
                        .fontWeight(.bold)
                        .foregroundStyle(.white)
                        .frame(minWidth: GameTheme.Size.badgeSize, minHeight: GameTheme.Size.badgeSize)
                        .background(GameTheme.Colors.accent)
                        .clipShape(Circle())
                }
            }
        }
        .padding(.horizontal, GameTheme.Spacing.md)
        .padding(.vertical, GameTheme.Spacing.sm)
        .background(thread.hasUnread ? GameTheme.Colors.surface : Color.clear)
    }

    // MARK: - Thread Detail (expanded emails)

    @ViewBuilder
    private func threadDetail(_ thread: EmailThread) -> some View {
        VStack(alignment: .leading, spacing: 0) {
            ForEach(thread.emails.reversed()) { email in
                VStack(alignment: .leading, spacing: GameTheme.Spacing.xs) {
                    // Email header
                    HStack {
                        Text(email.content.senderName == "You" ? "You" : email.content.senderName)
                            .font(GameTheme.Typography.micro)
                            .fontWeight(.bold)
                            .foregroundStyle(email.content.senderName == "You" ? GameTheme.Colors.accent : GameTheme.Colors.textPrimary)
                        Spacer()
                        Text(email.scheduledDate.shortFormatted)
                            .font(.system(size: 10))
                            .foregroundStyle(GameTheme.Colors.textMuted)

                        if email.status == .completed {
                            Image(systemName: "checkmark")
                                .font(.system(size: 10))
                                .foregroundStyle(GameTheme.Colors.success)
                        }
                    }

                    // Subject (if different from thread subject)
                    Text(email.content.subject)
                        .font(GameTheme.Typography.micro)
                        .fontWeight(.medium)
                        .foregroundStyle(GameTheme.Colors.textSecondary)

                    // Body
                    if !email.content.body.isEmpty {
                        if email.type == .clientContractSent {
                            // Contract as document
                            Text(email.content.body)
                                .font(GameTheme.Typography.micro)
                                .foregroundStyle(GameTheme.Colors.textSecondary)
                                .padding(GameTheme.Spacing.xs)
                                .background(GameTheme.Colors.background)
                                .clipShape(RoundedRectangle(cornerRadius: GameTheme.Radius.small))
                                .overlay(
                                    RoundedRectangle(cornerRadius: GameTheme.Radius.small)
                                        .stroke(GameTheme.Colors.border, lineWidth: 1)
                                )
                        } else {
                            Text(email.content.body)
                                .font(GameTheme.Typography.micro)
                                .foregroundStyle(GameTheme.Colors.textMuted)
                        }
                    }

                    // Quote amount
                    if let quote = email.content.quoteAmount {
                        HStack {
                            Text("Quoted:")
                                .font(GameTheme.Typography.micro)
                                .foregroundStyle(GameTheme.Colors.textMuted)
                            Text("$\(quote, specifier: "%.0f")")
                                .font(GameTheme.Typography.money)
                                .foregroundStyle(GameTheme.Colors.money)
                        }
                    }

                    // Deadline
                    if let deadline = email.responseDeadline, email.status == .ready {
                        HStack(spacing: 4) {
                            Image(systemName: "clock")
                                .font(.system(size: 10))
                            Text("Respond by \(deadline.formatted)")
                                .font(.system(size: 10))
                        }
                        .foregroundStyle(GameTheme.Colors.warning)
                    }

                    // Actions for unread emails
                    if email.status == .ready {
                        emailActions(for: email)
                    }
                }
                .padding(.horizontal, GameTheme.Spacing.md)
                .padding(.vertical, GameTheme.Spacing.xs)
                .background(email.status == .ready ? GameTheme.Colors.elevated.opacity(0.5) : Color.clear)

                if email.id != thread.emails.last?.id {
                    Divider()
                        .background(GameTheme.Colors.border)
                        .padding(.horizontal, GameTheme.Spacing.lg)
                }
            }
        }
        .padding(.bottom, GameTheme.Spacing.xs)
    }

    // MARK: - Email Actions

    @ViewBuilder
    private func emailActions(for email: PlanningActivity) -> some View {
        if (email.type == .vendorOptionsReview || email.type == .vendorNegotiationResponse),
           email.content.quoteAmount != nil {
            VendorQuoteActions(email: email)
        } else if email.type == .clientContractSent {
            ContractFeeActions(email: email)
        } else {
            Button(action: { withAnimation { gameManager.completeActivity(email.id) } }) {
                Text(acknowledgeLabel(for: email.type))
                    .font(GameTheme.Typography.micro).fontWeight(.semibold)
                    .foregroundStyle(.white)
                    .frame(maxWidth: .infinity)
                    .padding(.vertical, 6)
                    .background(GameTheme.Colors.accent)
                    .clipShape(RoundedRectangle(cornerRadius: GameTheme.Radius.small))
            }
        }
    }

    private func acknowledgeLabel(for type: ActivityType) -> String {
        switch type {
        case .clientContractSigned: return "Process Contract"
        case .clientDepositReceived: return "Acknowledge Payment"
        case .vendorAvailabilityResponse: return "Acknowledge"
        case .vendorOverdueWarning, .vendorOverdueFinal: return "Dismiss"
        default: return "Done"
        }
    }

    // MARK: - Inquiry Row

    @ViewBuilder
    private func inquiryRow(_ inquiry: ClientInquiry) -> some View {
        HStack(alignment: .top, spacing: GameTheme.Spacing.sm) {
            Circle()
                .fill(GameTheme.Colors.warning)
                .frame(width: 8, height: 8)
                .padding(.top, 6)

            VStack(alignment: .leading, spacing: 4) {
                HStack {
                    Text(inquiry.clientName)
                        .font(GameTheme.Typography.caption)
                        .fontWeight(.bold)
                        .foregroundStyle(GameTheme.Colors.textPrimary)
                    Spacer()
                    Text("New")
                        .font(GameTheme.Typography.micro).fontWeight(.bold)
                        .padding(.horizontal, 6).padding(.vertical, 2)
                        .background(GameTheme.Colors.warning.opacity(0.15))
                        .foregroundStyle(GameTheme.Colors.warning)
                        .clipShape(Capsule())
                }

                Text("Event inquiry — \(inquiry.subCategory)")
                    .font(GameTheme.Typography.caption)
                    .foregroundStyle(GameTheme.Colors.textPrimary)

                Text("$\(inquiry.budget) \u{00B7} \(inquiry.guestCount) guests \u{00B7} \(inquiry.eventDate.shortFormatted)")
                    .font(GameTheme.Typography.micro)
                    .foregroundStyle(GameTheme.Colors.textMuted)

                HStack(spacing: GameTheme.Spacing.xs) {
                    Button(action: { withAnimation { gameManager.acceptInquiry(inquiry) } }) {
                        Text("Accept")
                            .font(GameTheme.Typography.micro).fontWeight(.semibold)
                            .foregroundStyle(.white)
                            .padding(.horizontal, GameTheme.Spacing.sm).padding(.vertical, 6)
                            .background(GameTheme.Colors.success)
                            .clipShape(RoundedRectangle(cornerRadius: GameTheme.Radius.small))
                    }
                    Button(action: { withAnimation { gameManager.declineInquiry(inquiry) } }) {
                        Text("Decline")
                            .font(GameTheme.Typography.micro).fontWeight(.semibold)
                            .foregroundStyle(GameTheme.Colors.textMuted)
                            .padding(.horizontal, GameTheme.Spacing.sm).padding(.vertical, 6)
                            .overlay(RoundedRectangle(cornerRadius: GameTheme.Radius.small).stroke(GameTheme.Colors.textMuted, lineWidth: 1))
                    }
                }
            }
        }
        .padding(.horizontal, GameTheme.Spacing.md)
        .padding(.vertical, GameTheme.Spacing.sm)
        .background(GameTheme.Colors.surface)
    }
}

// MARK: - Vendor Quote Actions (Accept / Negotiate with amount picker)

struct VendorQuoteActions: View {
    @Environment(GameManager.self) private var gameManager
    let email: PlanningActivity
    @State private var showNegotiate = false
    @State private var offerAmount: Double

    init(email: PlanningActivity) {
        self.email = email
        let quote = email.content.quoteAmount ?? 0
        _offerAmount = State(initialValue: (quote * 0.85).rounded())
    }

    private var quoteAmount: Double { email.content.quoteAmount ?? 0 }

    var body: some View {
        VStack(spacing: GameTheme.Spacing.xs) {
            // Accept button
            Button(action: { withAnimation { gameManager.acceptVendorQuote(activityId: email.id) } }) {
                Text("Accept — $\(Int(quoteAmount))")
                    .font(GameTheme.Typography.micro).fontWeight(.semibold)
                    .foregroundStyle(.white)
                    .frame(maxWidth: .infinity)
                    .padding(.vertical, 6)
                    .background(GameTheme.Colors.success)
                    .clipShape(RoundedRectangle(cornerRadius: GameTheme.Radius.small))
            }

            // Negotiate toggle
            Button(action: { withAnimation(GameTheme.Anim.panelSlide) { showNegotiate.toggle() } }) {
                Text(showNegotiate ? "Cancel" : "Make a Counter Offer")
                    .font(GameTheme.Typography.micro).fontWeight(.semibold)
                    .foregroundStyle(GameTheme.Colors.warning)
                    .frame(maxWidth: .infinity)
                    .padding(.vertical, 6)
                    .overlay(RoundedRectangle(cornerRadius: GameTheme.Radius.small).stroke(GameTheme.Colors.warning, lineWidth: 1))
            }

            // Negotiate amount picker
            if showNegotiate {
                VStack(spacing: GameTheme.Spacing.xs) {
                    Text("Your offer:")
                        .font(GameTheme.Typography.micro)
                        .foregroundStyle(GameTheme.Colors.textMuted)

                    Text("$\(Int(offerAmount))")
                        .font(GameTheme.Typography.moneyLarge)
                        .foregroundStyle(GameTheme.Colors.money)

                    // Slider from 50% to 100% of quote
                    Slider(value: $offerAmount,
                           in: (quoteAmount * 0.5)...(quoteAmount),
                           step: 10)
                    .tint(GameTheme.Colors.warning)

                    HStack {
                        Text("$\(Int(quoteAmount * 0.5))")
                            .font(.system(size: 10))
                            .foregroundStyle(GameTheme.Colors.textMuted)
                        Spacer()
                        Text("$\(Int(quoteAmount))")
                            .font(.system(size: 10))
                            .foregroundStyle(GameTheme.Colors.textMuted)
                    }

                    let savings = quoteAmount - offerAmount
                    if savings > 0 {
                        Text("Saving $\(Int(savings)) (\(Int(savings / quoteAmount * 100))% off)")
                            .font(GameTheme.Typography.micro)
                            .foregroundStyle(GameTheme.Colors.success)
                    }

                    Button(action: {
                        if let vendorId = email.vendorId, let vendor = SeedData.vendor(byId: vendorId) {
                            withAnimation {
                                gameManager.sendNegotiationOffer(
                                    activityId: email.id,
                                    eventId: email.eventId,
                                    vendor: vendor,
                                    offerAmount: offerAmount
                                )
                            }
                        }
                    }) {
                        Text("Send Offer — $\(Int(offerAmount))")
                            .font(GameTheme.Typography.micro).fontWeight(.semibold)
                            .foregroundStyle(.white)
                            .frame(maxWidth: .infinity)
                            .padding(.vertical, 6)
                            .background(GameTheme.Colors.warning)
                            .clipShape(RoundedRectangle(cornerRadius: GameTheme.Radius.small))
                    }
                }
                .padding(GameTheme.Spacing.sm)
                .background(GameTheme.Colors.elevated)
                .clipShape(RoundedRectangle(cornerRadius: GameTheme.Radius.small))
            }
        }
    }
}

// MARK: - Contract Fee Actions (set service fee before sending)

struct ContractFeeActions: View {
    @Environment(GameManager.self) private var gameManager
    let email: PlanningActivity
    @State private var serviceFeePercent: Double

    init(email: PlanningActivity) {
        self.email = email
        // If client counter-offered, default to their suggested percent
        _serviceFeePercent = State(initialValue: email.content.counterOfferAmount ?? 15)
    }

    private var eventBudget: Double { email.content.contractAmount ?? 0 }
    private var serviceFee: Double { eventBudget * (serviceFeePercent / 100) }
    private var totalToClient: Double { eventBudget + serviceFee }

    var body: some View {
        VStack(alignment: .leading, spacing: GameTheme.Spacing.xs) {
            Text("Set Your Service Fee")
                .font(GameTheme.Typography.micro)
                .fontWeight(.bold)
                .foregroundStyle(GameTheme.Colors.textPrimary)

            // Fee breakdown
            HStack {
                Text("Event budget:")
                    .font(GameTheme.Typography.micro)
                    .foregroundStyle(GameTheme.Colors.textMuted)
                Spacer()
                Text("$\(Int(eventBudget))")
                    .font(GameTheme.Typography.money)
                    .foregroundStyle(GameTheme.Colors.textSecondary)
            }

            HStack {
                Text("Your fee (\(Int(serviceFeePercent))%):")
                    .font(GameTheme.Typography.micro)
                    .foregroundStyle(GameTheme.Colors.textMuted)
                Spacer()
                Text("$\(Int(serviceFee))")
                    .font(GameTheme.Typography.money)
                    .foregroundStyle(GameTheme.Colors.money)
            }

            Slider(value: $serviceFeePercent, in: 5...30, step: 1)
                .tint(GameTheme.Colors.money)

            HStack {
                Text("5%")
                    .font(.system(size: 10))
                    .foregroundStyle(GameTheme.Colors.textMuted)
                Spacer()
                Text("30%")
                    .font(.system(size: 10))
                    .foregroundStyle(GameTheme.Colors.textMuted)
            }

            Divider().background(GameTheme.Colors.border)

            HStack {
                Text("Total to client:")
                    .font(GameTheme.Typography.caption)
                    .fontWeight(.bold)
                    .foregroundStyle(GameTheme.Colors.textPrimary)
                Spacer()
                Text("$\(Int(totalToClient))")
                    .font(GameTheme.Typography.moneyLarge)
                    .foregroundStyle(GameTheme.Colors.money)
            }

            Button(action: {
                withAnimation {
                    gameManager.sendContractWithFee(
                        activityId: email.id,
                        serviceFeePercent: serviceFeePercent
                    )
                }
            }) {
                Text("Send Contract — $\(Int(totalToClient))")
                    .font(GameTheme.Typography.micro).fontWeight(.semibold)
                    .foregroundStyle(.white)
                    .frame(maxWidth: .infinity)
                    .padding(.vertical, 6)
                    .background(GameTheme.Colors.accent)
                    .clipShape(RoundedRectangle(cornerRadius: GameTheme.Radius.small))
            }
        }
        .padding(GameTheme.Spacing.sm)
        .background(GameTheme.Colors.elevated)
        .clipShape(RoundedRectangle(cornerRadius: GameTheme.Radius.small))
    }
}

// MARK: - Email Thread Model

struct EmailThread: Identifiable {
    var id: String { contactName }
    let contactName: String
    let emails: [PlanningActivity]
    let unreadCount: Int
    let latestDate: GameDate
    let latestSubject: String

    var hasUnread: Bool { unreadCount > 0 }
}
