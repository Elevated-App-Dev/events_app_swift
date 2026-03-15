import SwiftUI

/// Progress tracker — shows completed milestones per event.
/// Read-only. The player's "filing cabinet" — what's been done, not what needs doing.
struct ProgressView_: View {
    @Environment(GameManager.self) private var gameManager

    var body: some View {
        ScrollView {
            VStack(alignment: .leading, spacing: GameTheme.Spacing.sm) {
                // Active events
                ForEach(gameManager.activeEvents) { event in
                    eventProgressCard(event)
                }

                // Completed events
                if !gameManager.completedEvents.isEmpty {
                    Text("Completed")
                        .font(GameTheme.Typography.micro)
                        .fontWeight(.bold)
                        .foregroundStyle(GameTheme.Colors.textMuted)
                        .padding(.top, GameTheme.Spacing.sm)
                        .padding(.horizontal, GameTheme.Spacing.md)

                    ForEach(gameManager.completedEvents.suffix(5).reversed()) { event in
                        completedEventCard(event)
                    }
                }

                if gameManager.activeEvents.isEmpty && gameManager.completedEvents.isEmpty {
                    VStack(spacing: GameTheme.Spacing.sm) {
                        Image(systemName: "checklist")
                            .font(.system(size: 48))
                            .foregroundStyle(GameTheme.Colors.textMuted)
                        Text("No events yet")
                            .font(GameTheme.Typography.body)
                            .foregroundStyle(GameTheme.Colors.textMuted)
                    }
                    .frame(maxWidth: .infinity)
                    .padding(.top, GameTheme.Spacing.xl)
                }
            }
            .padding(.horizontal, GameTheme.Spacing.md)
            .padding(.top, GameTheme.Spacing.sm)
        }
    }

    @ViewBuilder
    private func eventProgressCard(_ event: EventData) -> some View {
        let activities = gameManager.advanceSystem.getActivitiesForEvent(eventId: event.id)
        let completed = activities.filter { $0.status == .completed }
        let total = activities.count
        let progress = total > 0 ? Double(completed.count) / Double(total) : 0

        VStack(alignment: .leading, spacing: GameTheme.Spacing.xs) {
            // Header
            HStack {
                VStack(alignment: .leading, spacing: 2) {
                    Text(event.eventTitle)
                        .font(GameTheme.Typography.h3)
                        .foregroundStyle(GameTheme.Colors.textPrimary)
                    Text(event.clientName)
                        .font(GameTheme.Typography.caption)
                        .foregroundStyle(GameTheme.Colors.textSecondary)
                }
                Spacer()
                Text("\(completed.count)/\(total)")
                    .font(GameTheme.Typography.money)
                    .foregroundStyle(GameTheme.Colors.accent)
            }

            // Progress bar
            GeometryReader { geo in
                ZStack(alignment: .leading) {
                    RoundedRectangle(cornerRadius: 2)
                        .fill(GameTheme.Colors.border)
                        .frame(height: GameTheme.Size.progressBarHeight)
                    RoundedRectangle(cornerRadius: 2)
                        .fill(GameTheme.Colors.accent)
                        .frame(width: geo.size.width * progress, height: GameTheme.Size.progressBarHeight)
                }
            }
            .frame(height: GameTheme.Size.progressBarHeight)

            // Completed milestones
            ForEach(completed) { activity in
                HStack(spacing: GameTheme.Spacing.xs) {
                    Image(systemName: "checkmark.circle.fill")
                        .font(.system(size: 14))
                        .foregroundStyle(GameTheme.Colors.success)
                    VStack(alignment: .leading, spacing: 1) {
                        Text(activity.content.subject)
                            .font(GameTheme.Typography.micro)
                            .foregroundStyle(GameTheme.Colors.textPrimary)
                            .lineLimit(1)
                        Text(activity.scheduledDate.shortFormatted)
                            .font(.system(size: 10))
                            .foregroundStyle(GameTheme.Colors.textMuted)
                    }
                }
            }

            // Pending count
            let pending = activities.filter { $0.status == .scheduled || $0.status == .ready }
            if !pending.isEmpty {
                HStack(spacing: GameTheme.Spacing.xs) {
                    Image(systemName: "circle")
                        .font(.system(size: 14))
                        .foregroundStyle(GameTheme.Colors.textMuted)
                    Text("\(pending.count) step\(pending.count == 1 ? "" : "s") remaining")
                        .font(GameTheme.Typography.micro)
                        .foregroundStyle(GameTheme.Colors.textMuted)
                }
            }
        }
        .surfaceCard()
    }

    @ViewBuilder
    private func completedEventCard(_ event: EventData) -> some View {
        HStack {
            Image(systemName: "checkmark.circle.fill")
                .foregroundStyle(GameTheme.Colors.success)
            VStack(alignment: .leading, spacing: 2) {
                Text(event.eventTitle)
                    .font(GameTheme.Typography.caption)
                    .foregroundStyle(GameTheme.Colors.textPrimary)
                Text(event.clientName)
                    .font(GameTheme.Typography.micro)
                    .foregroundStyle(GameTheme.Colors.textMuted)
            }
            Spacer()
            if let satisfaction = event.results?.finalSatisfaction {
                Text("\(Int(satisfaction))%")
                    .font(GameTheme.Typography.money)
                    .foregroundStyle(satisfaction >= 70 ? GameTheme.Colors.success : GameTheme.Colors.warning)
            }
        }
        .surfaceCard()
    }
}
