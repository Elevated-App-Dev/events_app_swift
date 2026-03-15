import SwiftUI

struct ActiveEventsListView: View {
    @Environment(GameManager.self) private var gameManager

    var body: some View {
        NavigationStack {
            List {
                ForEach(Array(gameManager.activeEvents.enumerated()), id: \.element.id) { index, event in
                    NavigationLink(value: index) {
                        EventRowView(event: event, currentDate: gameManager.currentDate)
                    }
                }
            }
            .navigationTitle("Active Events")
            .navigationDestination(for: Int.self) { index in
                if index < gameManager.activeEvents.count {
                    EventDetailView(eventIndex: index)
                }
            }
            .overlay {
                if gameManager.activeEvents.isEmpty {
                    ContentUnavailableView(
                        "No Active Events",
                        systemImage: "calendar.badge.exclamationmark",
                        description: Text("Accept a client inquiry to start planning an event.")
                    )
                }
            }
        }
    }
}

struct EventRowView: View {
    let event: EventData
    let currentDate: GameDate

    var body: some View {
        VStack(alignment: .leading, spacing: 6) {
            HStack {
                Text(event.eventTitle)
                    .font(.headline)
                Spacer()
                PhaseBadge(phase: event.phase)
            }

            HStack {
                Text(event.clientName)
                    .font(.subheadline)
                    .foregroundStyle(.secondary)
                Spacer()
                Text(event.eventDate.shortFormatted)
                    .font(.caption)
                    .foregroundStyle(.secondary)
            }

            // Budget bar
            let ratio = event.budget.total > 0 ? event.budget.spent / event.budget.total : 0
            HStack(spacing: 4) {
                ProgressView(value: min(ratio, 1.0))
                    .tint(ratio > 1.0 ? .red : ratio > 0.8 ? .orange : .blue)
                Text("$\(event.budget.spent, specifier: "%.0f") / $\(event.budget.total, specifier: "%.0f")")
                    .font(.caption2)
                    .foregroundStyle(.secondary)
            }
        }
        .padding(.vertical, 4)
    }
}

struct PhaseBadge: View {
    let phase: EventPhase

    var body: some View {
        Text(phaseLabel)
            .font(.caption2)
            .fontWeight(.medium)
            .padding(.horizontal, 8)
            .padding(.vertical, 3)
            .background(phaseColor.opacity(0.15))
            .foregroundStyle(phaseColor)
            .clipShape(Capsule())
    }

    private var phaseLabel: String {
        switch phase {
        case .booking: return "Booking"
        case .prePlanning: return "Pre-Plan"
        case .activePlanning: return "Planning"
        case .finalPrep: return "Final Prep"
        case .executionDay: return "Event Day!"
        case .results: return "Results"
        }
    }

    private var phaseColor: Color {
        switch phase {
        case .booking: return .blue
        case .prePlanning: return .purple
        case .activePlanning: return .indigo
        case .finalPrep: return .orange
        case .executionDay: return .red
        case .results: return .green
        }
    }
}
