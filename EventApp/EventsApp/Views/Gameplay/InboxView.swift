import SwiftUI

struct InboxView: View {
    @Environment(GameManager.self) private var gameManager

    var body: some View {
        ScrollView {
            VStack(spacing: GameTheme.Spacing.sm) {
                // Pending inquiries show as inbox emails
                ForEach(gameManager.pendingInquiries) { inquiry in
                    VStack(alignment: .leading, spacing: GameTheme.Spacing.xs) {
                        HStack {
                            Image(systemName: "envelope.fill")
                                .font(GameTheme.Typography.caption)
                                .foregroundStyle(GameTheme.Colors.warning)
                            Text(inquiry.clientName)
                                .font(GameTheme.Typography.h3)
                                .foregroundStyle(GameTheme.Colors.textPrimary)
                            Spacer()
                            Text("New Client")
                                .font(GameTheme.Typography.micro)
                                .fontWeight(.medium)
                                .padding(.horizontal, GameTheme.Spacing.xs)
                                .padding(.vertical, 3)
                                .background(GameTheme.Colors.warning.opacity(0.15))
                                .foregroundStyle(GameTheme.Colors.warning)
                                .clipShape(Capsule())
                        }

                        Text("Event inquiry — \(inquiry.subCategory)")
                            .font(GameTheme.Typography.caption)
                            .foregroundStyle(GameTheme.Colors.textSecondary)

                        HStack(spacing: GameTheme.Spacing.sm) {
                            Label("$\(inquiry.budget)", systemImage: "banknote")
                                .font(GameTheme.Typography.micro)
                                .foregroundStyle(GameTheme.Colors.money)
                            Label("\(inquiry.guestCount) guests", systemImage: "person.2")
                                .font(GameTheme.Typography.micro)
                                .foregroundStyle(GameTheme.Colors.textMuted)
                            Label(inquiry.eventDate.shortFormatted, systemImage: "calendar")
                                .font(GameTheme.Typography.micro)
                                .foregroundStyle(GameTheme.Colors.textMuted)
                        }

                        HStack(spacing: GameTheme.Spacing.xs) {
                            Button(action: {
                                withAnimation(GameTheme.Anim.spring) {
                                    gameManager.acceptInquiry(inquiry)
                                }
                            }) {
                                Text("Accept")
                                    .font(GameTheme.Typography.micro)
                                    .fontWeight(.semibold)
                                    .foregroundStyle(GameTheme.Colors.textPrimary)
                                    .frame(maxWidth: .infinity)
                                    .padding(.vertical, GameTheme.Spacing.xs)
                                    .background(GameTheme.Colors.success)
                                    .clipShape(RoundedRectangle(cornerRadius: GameTheme.Radius.small))
                            }

                            Button(action: {
                                withAnimation(GameTheme.Anim.spring) {
                                    gameManager.declineInquiry(inquiry)
                                }
                            }) {
                                Text("Decline")
                                    .font(GameTheme.Typography.micro)
                                    .fontWeight(.semibold)
                                    .foregroundStyle(GameTheme.Colors.textSecondary)
                                    .frame(maxWidth: .infinity)
                                    .padding(.vertical, GameTheme.Spacing.xs)
                                    .overlay(
                                        RoundedRectangle(cornerRadius: GameTheme.Radius.small)
                                            .stroke(GameTheme.Colors.textMuted, lineWidth: 1)
                                    )
                            }
                        }
                        .padding(.top, GameTheme.Spacing.xs)
                    }
                    .surfaceCard()
                }

                // Planning activities
                ForEach(gameManager.inboxActivities) { activity in
                    VStack(spacing: 0) {
                        InboxActivityRow(activity: activity)

                        Button(action: {
                            withAnimation(GameTheme.Anim.spring) {
                                gameManager.completeActivity(activity.id)
                            }
                        }) {
                            Text(acknowledgeLabel(for: activity.type))
                                .font(GameTheme.Typography.micro)
                                .fontWeight(.semibold)
                                .foregroundStyle(GameTheme.Colors.textPrimary)
                                .frame(maxWidth: .infinity)
                                .padding(.vertical, GameTheme.Spacing.xs)
                                .background(GameTheme.Colors.accent)
                                .clipShape(RoundedRectangle(cornerRadius: GameTheme.Radius.small))
                        }
                        .padding(.top, GameTheme.Spacing.xs)
                    }
                    .surfaceCard()
                }
            }
            .padding(.horizontal, GameTheme.Spacing.md)

            if gameManager.inboxActivities.isEmpty && gameManager.pendingInquiries.isEmpty {
                VStack(spacing: GameTheme.Spacing.sm) {
                    Image(systemName: "tray")
                        .font(.system(size: 48))
                        .foregroundStyle(GameTheme.Colors.textMuted)
                    Text("Inbox empty")
                        .font(GameTheme.Typography.body)
                        .foregroundStyle(GameTheme.Colors.textMuted)
                    Text("Advance to receive messages")
                        .font(GameTheme.Typography.caption)
                        .foregroundStyle(GameTheme.Colors.textMuted)
                }
                .frame(maxWidth: .infinity)
                .padding(.top, GameTheme.Spacing.xl)
            }
        }
    }

    private func acknowledgeLabel(for type: ActivityType) -> String {
        switch type {
        case .clientMeeting: return "End Call"
        case .clientContractSent: return "Send to Client"
        case .clientContractSigned: return "Process Contract"
        case .clientDepositReceived: return "Acknowledge Payment"
        case .clientFinalPayment: return "Confirm Payment"
        case .vendorAvailabilityResponse: return "Acknowledge"
        case .vendorOptionsReview: return "Review Complete"
        case .vendorNegotiationResponse: return "Acknowledge"
        case .vendorOverdueWarning, .vendorOverdueFinal: return "Dismiss"
        default: return "Done"
        }
    }
}

struct InboxActivityRow: View {
    let activity: PlanningActivity

    var body: some View {
        VStack(alignment: .leading, spacing: GameTheme.Spacing.xs) {
            HStack {
                mediumIcon
                Text(activity.content.senderName)
                    .font(GameTheme.Typography.h3)
                    .foregroundStyle(GameTheme.Colors.textPrimary)
                Spacer()
                activityTypeBadge
            }

            Text(activity.content.subject)
                .font(GameTheme.Typography.caption)
                .foregroundStyle(GameTheme.Colors.textSecondary)

            if !activity.content.body.isEmpty {
                Text(activity.content.body)
                    .font(GameTheme.Typography.caption)
                    .foregroundStyle(GameTheme.Colors.textMuted)
                    .lineLimit(3)
            }

            // Dialogue transcript
            if let transcript = activity.content.dialogueTranscript, !transcript.isEmpty {
                VStack(alignment: .leading, spacing: 6) {
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

            // Quote
            if let quote = activity.content.quoteAmount {
                HStack {
                    Text("Quote:")
                        .font(GameTheme.Typography.caption)
                        .foregroundStyle(GameTheme.Colors.textMuted)
                    Text("$\(quote, specifier: "%.0f")")
                        .font(GameTheme.Typography.money)
                        .foregroundStyle(GameTheme.Colors.money)
                }
            }

            // Deadline
            if let deadline = activity.responseDeadline {
                HStack(spacing: 4) {
                    Image(systemName: "clock")
                        .font(GameTheme.Typography.micro)
                    Text("Respond by \(deadline.formatted)")
                        .font(GameTheme.Typography.micro)
                }
                .foregroundStyle(activity.status == .overdue ? GameTheme.Colors.error : GameTheme.Colors.warning)
            }
        }
    }

    private var mediumIcon: some View {
        Group {
            switch activity.medium {
            case .email:
                Image(systemName: "envelope.fill")
            case .text:
                Image(systemName: "message.fill")
            case .call:
                Image(systemName: "phone.fill")
            case .inPerson:
                Image(systemName: "person.fill")
            }
        }
        .font(GameTheme.Typography.caption)
        .foregroundStyle(GameTheme.Colors.accent)
    }

    private var activityTypeBadge: some View {
        Text(badgeLabel)
            .font(GameTheme.Typography.micro)
            .fontWeight(.medium)
            .padding(.horizontal, GameTheme.Spacing.xs)
            .padding(.vertical, 3)
            .background(badgeColor.opacity(0.15))
            .foregroundStyle(badgeColor)
            .clipShape(Capsule())
    }

    private var badgeLabel: String {
        switch activity.type {
        case .clientMeeting: return "Meeting"
        case .clientContractSent: return "Contract"
        case .clientContractSigned: return "Signed"
        case .clientDepositReceived: return "Payment"
        case .clientFinalPayment: return "Final Pay"
        case .clientDateChangeRequest: return "Date Change"
        case .vendorAvailabilityRequest: return "Sent"
        case .vendorAvailabilityResponse: return "Available?"
        case .vendorOptionsReview: return "Options"
        case .vendorTasting: return "Tasting"
        case .vendorSiteVisit: return "Site Visit"
        case .vendorNegotiationOffer: return "Your Offer"
        case .vendorNegotiationResponse: return "Counter"
        case .vendorContractSent: return "Contract"
        case .vendorDepositPayment: return "Deposit"
        case .vendorFinalConfirmation: return "Confirm"
        case .vendorOverdueWarning: return "Overdue"
        case .vendorOverdueFinal: return "Final Warning"
        case .eventExecution: return "Event Day"
        case .eventResults: return "Results"
        case .newInquiry: return "New Client"
        }
    }

    private var badgeColor: Color {
        switch activity.type {
        case .clientMeeting, .clientContractSent, .clientContractSigned,
             .clientDepositReceived, .clientFinalPayment, .clientDateChangeRequest:
            return GameTheme.Colors.accent
        case .vendorAvailabilityRequest, .vendorAvailabilityResponse,
             .vendorOptionsReview, .vendorTasting, .vendorSiteVisit:
            return GameTheme.Colors.accent
        case .vendorNegotiationOffer, .vendorNegotiationResponse:
            return GameTheme.Colors.warning
        case .vendorContractSent, .vendorDepositPayment, .vendorFinalConfirmation:
            return GameTheme.Colors.success
        case .vendorOverdueWarning, .vendorOverdueFinal:
            return GameTheme.Colors.error
        case .eventExecution:
            return GameTheme.Colors.error
        case .eventResults:
            return GameTheme.Colors.success
        case .newInquiry:
            return GameTheme.Colors.accent
        }
    }
}
