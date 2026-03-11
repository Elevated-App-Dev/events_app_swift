import SwiftUI

struct InquiryListView: View {
    @Environment(GameManager.self) private var gameManager

    var body: some View {
        NavigationStack {
            List {
                ForEach(gameManager.pendingInquiries) { inquiry in
                    InquiryRowView(inquiry: inquiry)
                }
            }
            .navigationTitle("Client Inquiries")
            .overlay {
                if gameManager.pendingInquiries.isEmpty {
                    ContentUnavailableView(
                        "No Inquiries",
                        systemImage: "person.crop.circle.badge.questionmark",
                        description: Text("New client inquiries will appear here as time passes.")
                    )
                }
            }
        }
    }
}

struct InquiryRowView: View {
    @Environment(GameManager.self) private var gameManager
    let inquiry: ClientInquiry

    var body: some View {
        VStack(alignment: .leading, spacing: 8) {
            HStack {
                Text(inquiry.eventDisplayName)
                    .font(.headline)
                Spacer()
                Text("$\(inquiry.budget)")
                    .font(.subheadline)
                    .fontWeight(.semibold)
                    .foregroundStyle(.green)
            }

            HStack {
                Label("\(inquiry.guestCount) guests", systemImage: "person.2")
                    .font(.caption)
                Spacer()
                Label(inquiry.eventDate.shortFormatted, systemImage: "calendar")
                    .font(.caption)
            }
            .foregroundStyle(.secondary)

            HStack {
                Label(inquiry.personality.rawValue.capitalized, systemImage: "person.circle")
                    .font(.caption)
                    .foregroundStyle(.secondary)
                Spacer()

                Button("Decline") {
                    gameManager.declineInquiry(inquiry)
                }
                .buttonStyle(.bordered)
                .controlSize(.small)
                .tint(.red)

                Button("Accept") {
                    gameManager.acceptInquiry(inquiry)
                }
                .buttonStyle(.borderedProminent)
                .controlSize(.small)
            }
        }
        .padding(.vertical, 4)
    }
}
