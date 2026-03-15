import SwiftUI

struct InboxView: View {
    @Environment(GameManager.self) private var gameManager

    var body: some View {
        NavigationStack {
            List {
                ForEach(gameManager.inboxActivities) { activity in
                    InboxActivityRow(activity: activity)
                        .swipeActions(edge: .trailing) {
                            Button("Done") {
                                gameManager.completeActivity(activity.id)
                            }
                            .tint(.green)
                        }
                }
            }
            .navigationTitle("Inbox")
            .overlay {
                if gameManager.inboxActivities.isEmpty {
                    ContentUnavailableView(
                        "Inbox Empty",
                        systemImage: "tray",
                        description: Text("Advance to the next day to receive messages.")
                    )
                }
            }
        }
    }
}

struct InboxActivityRow: View {
    let activity: PlanningActivity

    var body: some View {
        VStack(alignment: .leading, spacing: 6) {
            HStack {
                mediumIcon
                Text(activity.content.senderName)
                    .font(.headline)
                Spacer()
                activityTypeBadge
            }

            Text(activity.content.subject)
                .font(.subheadline)
                .foregroundStyle(.secondary)

            if !activity.content.body.isEmpty {
                Text(activity.content.body)
                    .font(.caption)
                    .foregroundStyle(.secondary)
                    .lineLimit(3)
            }

            // Show dialogue transcript if present
            if let transcript = activity.content.dialogueTranscript, !transcript.isEmpty {
                VStack(alignment: .leading, spacing: 4) {
                    ForEach(Array(transcript.enumerated()), id: \.offset) { _, line in
                        HStack(alignment: .top, spacing: 4) {
                            Text(line.speaker == .client ? "Them:" : "You:")
                                .font(.caption2)
                                .fontWeight(.bold)
                                .foregroundStyle(line.speaker == .client ? .blue : .green)
                            Text(line.text)
                                .font(.caption2)
                                .foregroundStyle(.secondary)
                        }
                    }
                }
                .padding(8)
                .background(Color(.systemGray6))
                .clipShape(RoundedRectangle(cornerRadius: 8))
            }

            // Show vendor options if present
            if let quote = activity.content.quoteAmount {
                HStack {
                    Text("Quote:")
                        .font(.caption)
                        .foregroundStyle(.secondary)
                    Text("$\(quote, specifier: "%.0f")")
                        .font(.caption)
                        .fontWeight(.bold)
                }
            }

            if let deadline = activity.responseDeadline {
                HStack(spacing: 4) {
                    Image(systemName: "clock")
                        .font(.caption2)
                    Text("Respond by \(deadline.formatted)")
                        .font(.caption2)
                }
                .foregroundStyle(activity.status == .overdue ? .red : .orange)
            }
        }
        .padding(.vertical, 4)
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
        .font(.caption)
        .foregroundStyle(.blue)
    }

    private var activityTypeBadge: some View {
        Text(badgeLabel)
            .font(.caption2)
            .fontWeight(.medium)
            .padding(.horizontal, 6)
            .padding(.vertical, 2)
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
            return .purple
        case .vendorAvailabilityRequest, .vendorAvailabilityResponse,
             .vendorOptionsReview, .vendorTasting, .vendorSiteVisit:
            return .blue
        case .vendorNegotiationOffer, .vendorNegotiationResponse:
            return .orange
        case .vendorContractSent, .vendorDepositPayment, .vendorFinalConfirmation:
            return .green
        case .vendorOverdueWarning, .vendorOverdueFinal:
            return .red
        case .eventExecution:
            return .red
        case .eventResults:
            return .green
        case .newInquiry:
            return .blue
        }
    }
}
