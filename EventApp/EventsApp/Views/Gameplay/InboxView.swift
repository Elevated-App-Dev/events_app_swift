import SwiftUI

// MARK: - Messages App (Thread List)

/// Threaded messages view — groups all communication by contact.
/// Shows thread list first, tap to open conversation detail.
struct InboxView: View {
    @Environment(GameManager.self) private var gameManager
    @State private var selectedContactName: String?

    var body: some View {
        if let contactName = selectedContactName,
           let thread = gameManager.messageThreads.first(where: { $0.contactName == contactName }) {
            ThreadDetailView(thread: thread, onBack: { selectedContactName = nil })
        } else {
            threadListView
        }
    }

    private var threadListView: some View {
        ScrollView {
            VStack(spacing: 0) {
                // New inquiries as special threads at top
                ForEach(gameManager.pendingInquiries) { inquiry in
                    InquiryThreadRow(inquiry: inquiry)
                    Divider().background(GameTheme.Colors.border)
                }

                // Conversation threads
                ForEach(gameManager.messageThreads) { thread in
                    Button(action: { selectedContactName = thread.contactName }) {
                        ThreadRow(thread: thread)
                    }
                    Divider().background(GameTheme.Colors.border)
                }
            }

            if gameManager.messageThreads.isEmpty && gameManager.pendingInquiries.isEmpty {
                VStack(spacing: GameTheme.Spacing.sm) {
                    Image(systemName: "message")
                        .font(.system(size: 48))
                        .foregroundStyle(GameTheme.Colors.textMuted)
                    Text("No messages")
                        .font(GameTheme.Typography.body)
                        .foregroundStyle(GameTheme.Colors.textMuted)
                    Text("Advance to receive new messages")
                        .font(GameTheme.Typography.caption)
                        .foregroundStyle(GameTheme.Colors.textMuted)
                }
                .frame(maxWidth: .infinity)
                .padding(.top, GameTheme.Spacing.xl)
            }
        }
    }
}

// MARK: - Thread Row (in thread list)

struct ThreadRow: View {
    let thread: ConversationThread

    var body: some View {
        HStack(spacing: GameTheme.Spacing.sm) {
            // Avatar circle
            ZStack {
                Circle()
                    .fill(GameTheme.Colors.surface)
                    .frame(width: 44, height: 44)
                Text(thread.contactName.prefix(1).uppercased())
                    .font(GameTheme.Typography.h3)
                    .foregroundStyle(GameTheme.Colors.accent)
            }

            VStack(alignment: .leading, spacing: 4) {
                HStack {
                    Text(thread.contactName)
                        .font(GameTheme.Typography.caption)
                        .fontWeight(thread.hasUnread ? .bold : .regular)
                        .foregroundStyle(GameTheme.Colors.textPrimary)
                    Spacer()
                    if let latest = thread.latestMessage {
                        Text(latest.scheduledDate.shortFormatted)
                            .font(GameTheme.Typography.micro)
                            .foregroundStyle(GameTheme.Colors.textMuted)
                    }
                }

                HStack {
                    if let latest = thread.latestMessage {
                        mediumIcon(for: latest.medium)
                        Text(latest.content.subject)
                            .font(GameTheme.Typography.micro)
                            .foregroundStyle(thread.hasUnread ? GameTheme.Colors.textSecondary : GameTheme.Colors.textMuted)
                            .lineLimit(1)
                    }
                    Spacer()
                    if thread.unreadCount > 0 {
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
        }
        .padding(.horizontal, GameTheme.Spacing.md)
        .padding(.vertical, GameTheme.Spacing.sm)
    }

    @ViewBuilder
    private func mediumIcon(for medium: CommunicationMedium) -> some View {
        Group {
            switch medium {
            case .email: Image(systemName: "envelope")
            case .text: Image(systemName: "message")
            case .call: Image(systemName: "phone")
            case .inPerson: Image(systemName: "person")
            }
        }
        .font(.system(size: 10))
        .foregroundStyle(GameTheme.Colors.textMuted)
    }
}

// MARK: - Inquiry Thread Row (special, with accept/decline)

struct InquiryThreadRow: View {
    @Environment(GameManager.self) private var gameManager
    let inquiry: ClientInquiry

    var body: some View {
        HStack(spacing: GameTheme.Spacing.sm) {
            // Avatar with warning color
            ZStack {
                Circle()
                    .fill(GameTheme.Colors.warning.opacity(0.2))
                    .frame(width: 44, height: 44)
                Image(systemName: "envelope.badge")
                    .font(.system(size: 18))
                    .foregroundStyle(GameTheme.Colors.warning)
            }

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

                Text("\(inquiry.subCategory) \u{00B7} $\(inquiry.budget) \u{00B7} \(inquiry.guestCount) guests \u{00B7} \(inquiry.eventDate.shortFormatted)")
                    .font(GameTheme.Typography.micro)
                    .foregroundStyle(GameTheme.Colors.textSecondary)
                    .lineLimit(1)

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
                            .padding(.horizontal, GameTheme.Spacing.sm)
                            .padding(.vertical, 6)
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
        .padding(.horizontal, GameTheme.Spacing.md)
        .padding(.vertical, GameTheme.Spacing.sm)
    }
}

// MARK: - Thread Detail (conversation view)

struct ThreadDetailView: View {
    @Environment(GameManager.self) private var gameManager
    let thread: ConversationThread
    let onBack: () -> Void

    var body: some View {
        VStack(spacing: 0) {
            // Thread header
            HStack {
                Button(action: onBack) {
                    HStack(spacing: 4) {
                        Image(systemName: "chevron.left")
                        Text("Messages")
                    }
                    .font(GameTheme.Typography.caption)
                    .foregroundStyle(GameTheme.Colors.accent)
                }

                Spacer()

                Text(thread.contactName)
                    .font(GameTheme.Typography.h3)
                    .foregroundStyle(GameTheme.Colors.textPrimary)

                Spacer()

                // Spacer for visual balance
                Color.clear.frame(width: 70, height: 1)
            }
            .padding(.horizontal, GameTheme.Spacing.md)
            .padding(.vertical, GameTheme.Spacing.sm)

            Divider().background(GameTheme.Colors.border)

            // Messages
            ScrollView {
                VStack(spacing: GameTheme.Spacing.sm) {
                    ForEach(thread.activities) { activity in
                        MessageBubble(
                            activity: activity,
                            onAction: {
                                if activity.status == .ready {
                                    withAnimation(GameTheme.Anim.spring) {
                                        gameManager.completeActivity(activity.id)
                                    }
                                }
                            },
                            onAcceptQuote: {
                                withAnimation(GameTheme.Anim.spring) {
                                    gameManager.acceptVendorQuote(activityId: activity.id)
                                }
                            },
                            onNegotiate: {
                                if let vendorId = activity.vendorId,
                                   let vendor = SeedData.vendor(byId: vendorId) {
                                    let offerAmount = (activity.content.quoteAmount ?? vendor.basePrice) * 0.85
                                    withAnimation(GameTheme.Anim.spring) {
                                        gameManager.sendNegotiationOffer(
                                            activityId: activity.id,
                                            eventId: activity.eventId,
                                            vendor: vendor,
                                            offerAmount: offerAmount
                                        )
                                    }
                                }
                            }
                        )
                    }
                }
                .padding(.horizontal, GameTheme.Spacing.md)
                .padding(.vertical, GameTheme.Spacing.sm)
            }
        }
    }
}

// MARK: - Message Bubble

struct MessageBubble: View {
    let activity: PlanningActivity
    let onAction: () -> Void
    var onAcceptQuote: (() -> Void)? = nil
    var onNegotiate: (() -> Void)? = nil

    private var isVendorQuote: Bool {
        activity.type == .vendorOptionsReview && activity.status == .ready && activity.content.quoteAmount != nil
    }

    /// Contracts are "from" the player but still need an action button (Send to Client).
    private var isDraftContract: Bool {
        activity.type == .clientContractSent && activity.status == .ready
    }

    private var isFromPlayer: Bool {
        activity.content.senderName == "You"
    }

    var body: some View {
        HStack {
            if isFromPlayer { Spacer(minLength: 60) }

            VStack(alignment: isFromPlayer ? .trailing : .leading, spacing: 4) {
                // Date stamp
                Text(activity.scheduledDate.shortFormatted)
                    .font(.system(size: 10))
                    .foregroundStyle(GameTheme.Colors.textMuted)

                // Bubble
                VStack(alignment: .leading, spacing: GameTheme.Spacing.xs) {
                    // Medium indicator
                    HStack(spacing: 4) {
                        mediumIcon
                        Text(activity.content.subject)
                            .font(GameTheme.Typography.caption)
                            .fontWeight(.medium)
                            .foregroundStyle(isFromPlayer ? .white : GameTheme.Colors.textPrimary)
                    }

                    // Body
                    if !activity.content.body.isEmpty {
                        if activity.type == .clientContractSent {
                            // Contract as a document
                            Text(activity.content.body)
                                .font(GameTheme.Typography.micro)
                                .foregroundStyle(GameTheme.Colors.textSecondary)
                                .padding(GameTheme.Spacing.xs)
                                .background(GameTheme.Colors.background)
                                .clipShape(RoundedRectangle(cornerRadius: GameTheme.Radius.small))
                        } else {
                            Text(activity.content.body)
                                .font(GameTheme.Typography.micro)
                                .foregroundStyle(isFromPlayer ? .white.opacity(0.9) : GameTheme.Colors.textSecondary)
                        }
                    }

                    // Transcript
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
                        .background(GameTheme.Colors.background)
                        .clipShape(RoundedRectangle(cornerRadius: GameTheme.Radius.small))
                    }

                    // Quote
                    if let quote = activity.content.quoteAmount {
                        HStack {
                            Text("Quote:")
                                .font(GameTheme.Typography.micro)
                            Text("$\(quote, specifier: "%.0f")")
                                .font(GameTheme.Typography.money)
                                .foregroundStyle(GameTheme.Colors.money)
                        }
                    }

                    // Deadline
                    if let deadline = activity.responseDeadline, activity.status == .ready {
                        HStack(spacing: 4) {
                            Image(systemName: "clock")
                                .font(.system(size: 10))
                            Text("Respond by \(deadline.formatted)")
                                .font(.system(size: 10))
                        }
                        .foregroundStyle(activity.status == .overdue ? GameTheme.Colors.error : GameTheme.Colors.warning)
                    }

                    // Vendor quote actions: Accept / Negotiate
                    if isVendorQuote {
                        HStack(spacing: GameTheme.Spacing.xs) {
                            if let accept = onAcceptQuote {
                                Button(action: accept) {
                                    Text("Accept Quote")
                                        .font(GameTheme.Typography.micro)
                                        .fontWeight(.semibold)
                                        .foregroundStyle(.white)
                                        .padding(.horizontal, GameTheme.Spacing.sm)
                                        .padding(.vertical, 6)
                                        .background(GameTheme.Colors.success)
                                        .clipShape(RoundedRectangle(cornerRadius: GameTheme.Radius.small))
                                }
                            }
                            if let negotiate = onNegotiate {
                                Button(action: negotiate) {
                                    Text("Negotiate")
                                        .font(GameTheme.Typography.micro)
                                        .fontWeight(.semibold)
                                        .foregroundStyle(GameTheme.Colors.warning)
                                        .padding(.horizontal, GameTheme.Spacing.sm)
                                        .padding(.vertical, 6)
                                        .overlay(
                                            RoundedRectangle(cornerRadius: GameTheme.Radius.small)
                                                .stroke(GameTheme.Colors.warning, lineWidth: 1)
                                        )
                                }
                            }
                        }
                    }
                    // Draft contract needs Send button even though it's "from" player
                    else if isDraftContract {
                        Button(action: onAction) {
                            Text("Send to Client")
                                .font(GameTheme.Typography.micro)
                                .fontWeight(.semibold)
                                .foregroundStyle(.white)
                                .padding(.horizontal, GameTheme.Spacing.sm)
                                .padding(.vertical, 6)
                                .background(GameTheme.Colors.accent)
                                .clipShape(RoundedRectangle(cornerRadius: GameTheme.Radius.small))
                        }
                    }
                    // Generic action button for other unread messages
                    else if activity.status == .ready && !isFromPlayer {
                        Button(action: onAction) {
                            Text(acknowledgeLabel)
                                .font(GameTheme.Typography.micro)
                                .fontWeight(.semibold)
                                .foregroundStyle(.white)
                                .padding(.horizontal, GameTheme.Spacing.sm)
                                .padding(.vertical, 6)
                                .background(GameTheme.Colors.accent)
                                .clipShape(RoundedRectangle(cornerRadius: GameTheme.Radius.small))
                        }
                    }

                    // Completed indicator
                    if activity.status == .completed {
                        HStack(spacing: 4) {
                            Image(systemName: "checkmark")
                                .font(.system(size: 10))
                            Text("Done")
                                .font(.system(size: 10))
                        }
                        .foregroundStyle(GameTheme.Colors.success)
                    }
                }
                .padding(GameTheme.Spacing.sm)
                .background(isFromPlayer ? GameTheme.Colors.accent : GameTheme.Colors.surface)
                .clipShape(RoundedRectangle(cornerRadius: GameTheme.Radius.medium))
            }

            if !isFromPlayer { Spacer(minLength: 60) }
        }
    }

    private var mediumIcon: some View {
        Group {
            switch activity.medium {
            case .email: Image(systemName: "envelope")
            case .text: Image(systemName: "message")
            case .call: Image(systemName: "phone")
            case .inPerson: Image(systemName: "person")
            }
        }
        .font(.system(size: 10))
        .foregroundStyle(isFromPlayer ? .white.opacity(0.7) : GameTheme.Colors.textMuted)
    }

    private var acknowledgeLabel: String {
        switch activity.type {
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
