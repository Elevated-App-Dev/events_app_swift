import SwiftUI

struct EventResultsView: View {
    @Environment(\.dismiss) private var dismiss
    let event: EventData

    private var results: EventResults {
        event.results ?? EventResults()
    }

    private var satisfactionTier: String {
        let sat = results.finalSatisfaction
        if sat >= 90 { return "Excellent" }
        if sat >= 75 { return "Good" }
        if sat >= 60 { return "Okay" }
        if sat >= 40 { return "Poor" }
        return "Failed"
    }

    private var tierColor: Color {
        switch satisfactionTier {
        case "Excellent": return GameTheme.Colors.success
        case "Good": return GameTheme.Colors.accent
        case "Okay": return GameTheme.Colors.reputation
        case "Poor": return GameTheme.Colors.warning
        default: return GameTheme.Colors.error
        }
    }

    var body: some View {
        NavigationStack {
            ScrollView {
                VStack(spacing: GameTheme.Spacing.md) {
                    // Title
                    Text(event.eventTitle)
                        .font(GameTheme.Typography.h1)
                        .foregroundStyle(GameTheme.Colors.textPrimary)

                    Text(event.clientName)
                        .font(GameTheme.Typography.caption)
                        .foregroundStyle(GameTheme.Colors.textSecondary)

                    // Overall score
                    VStack(spacing: 4) {
                        Text("\(results.finalSatisfaction, specifier: "%.0f")")
                            .font(.system(size: 72, weight: .bold, design: .rounded))
                            .foregroundStyle(tierColor)
                        Text(satisfactionTier)
                            .font(GameTheme.Typography.h2)
                            .foregroundStyle(tierColor)
                    }
                    .padding(.vertical, GameTheme.Spacing.sm)

                    // Category scores
                    VStack(alignment: .leading, spacing: GameTheme.Spacing.sm) {
                        Text("Category Scores")
                            .font(GameTheme.Typography.h3)
                            .foregroundStyle(GameTheme.Colors.textPrimary)
                        ScoreBar(label: "Venue", score: results.venueScore)
                        ScoreBar(label: "Food", score: results.foodScore)
                        ScoreBar(label: "Entertainment", score: results.entertainmentScore)
                        ScoreBar(label: "Decoration", score: results.decorationScore)
                        ScoreBar(label: "Service", score: results.serviceScore)
                        ScoreBar(label: "Expectations", score: results.expectationScore)
                    }
                    .surfaceCard()

                    // Financial results
                    VStack(alignment: .leading, spacing: GameTheme.Spacing.xs) {
                        Text("Financial Results")
                            .font(GameTheme.Typography.h3)
                            .foregroundStyle(GameTheme.Colors.textPrimary)
                        HStack {
                            Text("Service Fee")
                                .font(GameTheme.Typography.caption)
                                .foregroundStyle(GameTheme.Colors.textSecondary)
                            Spacer()
                            Text("+$\(results.profit, specifier: "%.0f")")
                                .font(GameTheme.Typography.money)
                                .foregroundStyle(GameTheme.Colors.success)
                        }
                        HStack {
                            Text("Reputation")
                                .font(GameTheme.Typography.caption)
                                .foregroundStyle(GameTheme.Colors.textSecondary)
                            Spacer()
                            let change = results.reputationChange
                            Text(change >= 0 ? "+\(change)" : "\(change)")
                                .font(GameTheme.Typography.money)
                                .foregroundStyle(change >= 0 ? GameTheme.Colors.success : GameTheme.Colors.error)
                        }
                        if results.triggeredReferral {
                            HStack(spacing: 4) {
                                Image(systemName: "person.badge.plus")
                                Text("Client referred you to a friend!")
                            }
                            .font(GameTheme.Typography.caption)
                            .foregroundStyle(GameTheme.Colors.accent)
                        }
                    }
                    .surfaceCard()

                    // Random events
                    if !results.randomEventsOccurred.isEmpty {
                        VStack(alignment: .leading, spacing: GameTheme.Spacing.xs) {
                            Text("What Happened")
                                .font(GameTheme.Typography.h3)
                                .foregroundStyle(GameTheme.Colors.textPrimary)
                            ForEach(results.randomEventsOccurred, id: \.self) { eventDesc in
                                HStack(spacing: 4) {
                                    Image(systemName: "exclamationmark.triangle")
                                        .foregroundStyle(GameTheme.Colors.warning)
                                    Text(eventDesc)
                                        .font(GameTheme.Typography.caption)
                                        .foregroundStyle(GameTheme.Colors.textSecondary)
                                }
                            }
                        }
                        .surfaceCard()
                    }

                    // Client feedback
                    if !results.clientFeedback.isEmpty {
                        VStack(alignment: .leading, spacing: GameTheme.Spacing.xs) {
                            Text("Client Review")
                                .font(GameTheme.Typography.h3)
                                .foregroundStyle(GameTheme.Colors.textPrimary)
                            Text("\"\(results.clientFeedback)\"")
                                .font(GameTheme.Typography.body)
                                .italic()
                                .foregroundStyle(GameTheme.Colors.textSecondary)
                            Text("— \(event.clientName)")
                                .font(GameTheme.Typography.caption)
                                .foregroundStyle(GameTheme.Colors.textMuted)
                        }
                        .surfaceCard()
                    }

                    Button(action: { dismiss() }) {
                        Text("Continue")
                            .primaryButton()
                    }
                    .padding(.top, GameTheme.Spacing.sm)
                }
                .padding(.horizontal, GameTheme.Spacing.md)
                .padding(.vertical, GameTheme.Spacing.md)
            }
            .background(GameTheme.Colors.background)
            .navigationBarTitleDisplayMode(.inline)
        }
        .preferredColorScheme(.dark)
    }
}

struct ScoreBar: View {
    let label: String
    let score: Double

    var body: some View {
        HStack {
            Text(label)
                .font(GameTheme.Typography.caption)
                .foregroundStyle(GameTheme.Colors.textSecondary)
                .frame(width: 100, alignment: .leading)
            GeometryReader { geo in
                ZStack(alignment: .leading) {
                    RoundedRectangle(cornerRadius: 2)
                        .fill(GameTheme.Colors.border)
                        .frame(height: GameTheme.Size.progressBarHeight)
                    RoundedRectangle(cornerRadius: 2)
                        .fill(scoreColor)
                        .frame(width: geo.size.width * min(score / 100, 1.0), height: GameTheme.Size.progressBarHeight)
                }
            }
            .frame(height: GameTheme.Size.progressBarHeight)
            Text("\(score, specifier: "%.0f")")
                .font(GameTheme.Typography.micro)
                .monospacedDigit()
                .foregroundStyle(scoreColor)
                .frame(width: 30, alignment: .trailing)
        }
    }

    private var scoreColor: Color {
        if score >= 80 { return GameTheme.Colors.success }
        if score >= 60 { return GameTheme.Colors.accent }
        if score >= 40 { return GameTheme.Colors.warning }
        return GameTheme.Colors.error
    }
}
