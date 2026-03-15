import SwiftUI

/// Formal email inbox — contracts, vendor quotes, availability responses, invoices.
/// Visual style: subject line, sender, body preview. NOT chat bubbles.
struct EmailInboxView: View {
    @Environment(GameManager.self) private var gameManager
    @State private var expandedEmailId: String?

    /// Emails = activities with .email medium that are ready, plus pending inquiries.
    private var emails: [PlanningActivity] {
        gameManager.advanceSystem.scheduledActivities
            .filter { $0.medium == .email && ($0.status == .ready || $0.status == .completed) && $0.scheduledDate <= gameManager.currentDate }
            .sorted { $0.scheduledDate > $1.scheduledDate }
    }

    var body: some View {
        ScrollView {
            VStack(spacing: 0) {
                // Pending inquiries as emails
                ForEach(gameManager.pendingInquiries) { inquiry in
                    inquiryEmailRow(inquiry)
                    Divider().background(GameTheme.Colors.border)
                }

                // Activity emails
                ForEach(emails) { email in
                    emailRow(email)
                    Divider().background(GameTheme.Colors.border)
                }
            }

            if emails.isEmpty && gameManager.pendingInquiries.isEmpty {
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

    // MARK: - Email Row

    @ViewBuilder
    private func emailRow(_ email: PlanningActivity) -> some View {
        let isExpanded = expandedEmailId == email.id
        let isUnread = email.status == .ready

        VStack(alignment: .leading, spacing: 0) {
            // Header row (always visible)
            Button(action: { withAnimation(GameTheme.Anim.panelSlide) { expandedEmailId = isExpanded ? nil : email.id } }) {
                HStack(alignment: .top, spacing: GameTheme.Spacing.sm) {
                    // Unread dot
                    Circle()
                        .fill(isUnread ? GameTheme.Colors.accent : Color.clear)
                        .frame(width: 8, height: 8)
                        .padding(.top, 6)

                    VStack(alignment: .leading, spacing: 4) {
                        HStack {
                            Text(email.content.senderName)
                                .font(GameTheme.Typography.caption)
                                .fontWeight(isUnread ? .bold : .regular)
                                .foregroundStyle(GameTheme.Colors.textPrimary)
                            Spacer()
                            Text(email.scheduledDate.shortFormatted)
                                .font(GameTheme.Typography.micro)
                                .foregroundStyle(GameTheme.Colors.textMuted)
                        }

                        Text(email.content.subject)
                            .font(GameTheme.Typography.caption)
                            .foregroundStyle(isUnread ? GameTheme.Colors.textPrimary : GameTheme.Colors.textSecondary)
                            .lineLimit(isExpanded ? nil : 1)

                        if !isExpanded && !email.content.body.isEmpty {
                            Text(email.content.body.prefix(80) + "...")
                                .font(GameTheme.Typography.micro)
                                .foregroundStyle(GameTheme.Colors.textMuted)
                                .lineLimit(1)
                        }
                    }
                }
            }
            .padding(.horizontal, GameTheme.Spacing.md)
            .padding(.vertical, GameTheme.Spacing.sm)

            // Expanded body
            if isExpanded {
                VStack(alignment: .leading, spacing: GameTheme.Spacing.sm) {
                    // Full body
                    if !email.content.body.isEmpty {
                        Text(email.content.body)
                            .font(GameTheme.Typography.caption)
                            .foregroundStyle(GameTheme.Colors.textSecondary)
                    }

                    // Quote display
                    if let quote = email.content.quoteAmount {
                        HStack {
                            Text("Quoted price:")
                                .font(GameTheme.Typography.caption)
                                .foregroundStyle(GameTheme.Colors.textMuted)
                            Text("$\(quote, specifier: "%.0f")")
                                .font(GameTheme.Typography.moneyLarge)
                                .foregroundStyle(GameTheme.Colors.money)
                        }
                    }

                    // Deadline
                    if let deadline = email.responseDeadline, email.status == .ready {
                        HStack(spacing: 4) {
                            Image(systemName: "clock")
                                .font(GameTheme.Typography.micro)
                            Text("Respond by \(deadline.formatted)")
                                .font(GameTheme.Typography.micro)
                        }
                        .foregroundStyle(GameTheme.Colors.warning)
                    }

                    // Actions
                    if email.status == .ready {
                        emailActions(for: email)
                    }

                    if email.status == .completed {
                        HStack(spacing: 4) {
                            Image(systemName: "checkmark.circle.fill")
                            Text("Handled")
                        }
                        .font(GameTheme.Typography.micro)
                        .foregroundStyle(GameTheme.Colors.success)
                    }
                }
                .padding(.horizontal, GameTheme.Spacing.md)
                .padding(.bottom, GameTheme.Spacing.sm)
            }
        }
        .background(isUnread ? GameTheme.Colors.surface : Color.clear)
    }

    @ViewBuilder
    private func emailActions(for email: PlanningActivity) -> some View {
        // Vendor quote — accept or negotiate
        if email.type == .vendorOptionsReview, email.content.quoteAmount != nil {
            HStack(spacing: GameTheme.Spacing.xs) {
                Button(action: {
                    withAnimation { gameManager.acceptVendorQuote(activityId: email.id) }
                }) {
                    Text("Accept Quote")
                        .font(GameTheme.Typography.micro)
                        .fontWeight(.semibold)
                        .foregroundStyle(.white)
                        .frame(maxWidth: .infinity)
                        .padding(.vertical, GameTheme.Spacing.xs)
                        .background(GameTheme.Colors.success)
                        .clipShape(RoundedRectangle(cornerRadius: GameTheme.Radius.small))
                }

                Button(action: {
                    if let vendorId = email.vendorId, let vendor = SeedData.vendor(byId: vendorId) {
                        let offer = (email.content.quoteAmount ?? vendor.basePrice) * 0.85
                        withAnimation { gameManager.sendNegotiationOffer(activityId: email.id, eventId: email.eventId, vendor: vendor, offerAmount: offer) }
                    }
                }) {
                    Text("Negotiate")
                        .font(GameTheme.Typography.micro)
                        .fontWeight(.semibold)
                        .foregroundStyle(GameTheme.Colors.warning)
                        .frame(maxWidth: .infinity)
                        .padding(.vertical, GameTheme.Spacing.xs)
                        .overlay(
                            RoundedRectangle(cornerRadius: GameTheme.Radius.small)
                                .stroke(GameTheme.Colors.warning, lineWidth: 1)
                        )
                }
            }
        }
        // Draft contract
        else if email.type == .clientContractSent {
            Button(action: {
                withAnimation { gameManager.completeActivity(email.id) }
            }) {
                Text("Send to Client")
                    .font(GameTheme.Typography.micro)
                    .fontWeight(.semibold)
                    .foregroundStyle(.white)
                    .frame(maxWidth: .infinity)
                    .padding(.vertical, GameTheme.Spacing.xs)
                    .background(GameTheme.Colors.accent)
                    .clipShape(RoundedRectangle(cornerRadius: GameTheme.Radius.small))
            }
        }
        // Generic acknowledge
        else {
            Button(action: {
                withAnimation { gameManager.completeActivity(email.id) }
            }) {
                Text(acknowledgeLabel(for: email.type))
                    .font(GameTheme.Typography.micro)
                    .fontWeight(.semibold)
                    .foregroundStyle(.white)
                    .frame(maxWidth: .infinity)
                    .padding(.vertical, GameTheme.Spacing.xs)
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

    // MARK: - Inquiry Email

    @ViewBuilder
    private func inquiryEmailRow(_ inquiry: ClientInquiry) -> some View {
        VStack(alignment: .leading, spacing: GameTheme.Spacing.xs) {
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
                            .font(GameTheme.Typography.micro)
                            .fontWeight(.bold)
                            .padding(.horizontal, 6)
                            .padding(.vertical, 2)
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
                                .font(GameTheme.Typography.micro)
                                .fontWeight(.semibold)
                                .foregroundStyle(.white)
                                .padding(.horizontal, GameTheme.Spacing.sm)
                                .padding(.vertical, 6)
                                .background(GameTheme.Colors.success)
                                .clipShape(RoundedRectangle(cornerRadius: GameTheme.Radius.small))
                        }
                        Button(action: { withAnimation { gameManager.declineInquiry(inquiry) } }) {
                            Text("Decline")
                                .font(GameTheme.Typography.micro)
                                .fontWeight(.semibold)
                                .foregroundStyle(GameTheme.Colors.textMuted)
                                .padding(.horizontal, GameTheme.Spacing.sm)
                                .padding(.vertical, 6)
                                .overlay(
                                    RoundedRectangle(cornerRadius: GameTheme.Radius.small)
                                        .stroke(GameTheme.Colors.textMuted, lineWidth: 1)
                                )
                        }
                    }
                }
            }
        }
        .padding(.horizontal, GameTheme.Spacing.md)
        .padding(.vertical, GameTheme.Spacing.sm)
        .background(GameTheme.Colors.surface)
    }
}
