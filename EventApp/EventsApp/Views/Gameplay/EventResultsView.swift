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
        case "Excellent": return .green
        case "Good": return .blue
        case "Okay": return .yellow
        case "Poor": return .orange
        default: return .red
        }
    }

    var body: some View {
        NavigationStack {
            ScrollView {
                VStack(spacing: 20) {
                    // Title
                    Text(event.eventTitle)
                        .font(.title2)
                        .fontWeight(.bold)

                    // Overall score
                    VStack(spacing: 4) {
                        Text("\(results.finalSatisfaction, specifier: "%.0f")")
                            .font(.system(size: 64, weight: .bold, design: .rounded))
                            .foregroundStyle(tierColor)
                        Text(satisfactionTier)
                            .font(.title3)
                            .fontWeight(.semibold)
                            .foregroundStyle(tierColor)
                    }
                    .padding()

                    // Category scores
                    VStack(alignment: .leading, spacing: 12) {
                        Text("Category Scores")
                            .font(.headline)
                        ScoreBar(label: "Venue", score: results.venueScore)
                        ScoreBar(label: "Food", score: results.foodScore)
                        ScoreBar(label: "Entertainment", score: results.entertainmentScore)
                        ScoreBar(label: "Decoration", score: results.decorationScore)
                        ScoreBar(label: "Service", score: results.serviceScore)
                        ScoreBar(label: "Expectations", score: results.expectationScore)
                    }
                    .padding()
                    .background(.ultraThinMaterial, in: RoundedRectangle(cornerRadius: 12))

                    // Financial results
                    VStack(alignment: .leading, spacing: 8) {
                        Text("Financial Results")
                            .font(.headline)
                        HStack {
                            Text("Profit")
                            Spacer()
                            Text(results.profit >= 0 ? "+$\(results.profit, specifier: "%.0f")" : "-$\(abs(results.profit), specifier: "%.0f")")
                                .fontWeight(.semibold)
                                .foregroundStyle(results.profit >= 0 ? .green : .red)
                        }
                        HStack {
                            Text("Reputation")
                            Spacer()
                            let change = results.reputationChange
                            Text(change >= 0 ? "+\(change)" : "\(change)")
                                .fontWeight(.semibold)
                                .foregroundStyle(change >= 0 ? .green : .red)
                        }
                        if results.triggeredReferral {
                            HStack {
                                Image(systemName: "person.badge.plus")
                                    .foregroundStyle(.blue)
                                Text("Client referred you to a friend!")
                                    .foregroundStyle(.blue)
                            }
                        }
                    }
                    .padding()
                    .background(.ultraThinMaterial, in: RoundedRectangle(cornerRadius: 12))

                    // Random events
                    if !results.randomEventsOccurred.isEmpty {
                        VStack(alignment: .leading, spacing: 8) {
                            Text("Random Events")
                                .font(.headline)
                            ForEach(results.randomEventsOccurred, id: \.self) { eventDesc in
                                HStack {
                                    Image(systemName: "exclamationmark.triangle")
                                        .foregroundStyle(.orange)
                                    Text(eventDesc)
                                        .font(.subheadline)
                                }
                            }
                        }
                        .padding()
                        .background(.ultraThinMaterial, in: RoundedRectangle(cornerRadius: 12))
                    }

                    // Client feedback
                    if !results.clientFeedback.isEmpty {
                        VStack(alignment: .leading, spacing: 8) {
                            Text("Client Feedback")
                                .font(.headline)
                            Text("\"\(results.clientFeedback)\"")
                                .font(.subheadline)
                                .italic()
                                .foregroundStyle(.secondary)
                        }
                        .padding()
                        .background(.ultraThinMaterial, in: RoundedRectangle(cornerRadius: 12))
                    }

                    Button("Continue") {
                        dismiss()
                    }
                    .buttonStyle(.borderedProminent)
                    .controlSize(.large)
                    .padding(.top)
                }
                .padding()
            }
            .navigationBarTitleDisplayMode(.inline)
        }
    }
}

struct ScoreBar: View {
    let label: String
    let score: Double

    var body: some View {
        HStack {
            Text(label)
                .font(.subheadline)
                .frame(width: 100, alignment: .leading)
            ProgressView(value: score, total: 100)
                .tint(scoreColor)
            Text("\(score, specifier: "%.0f")")
                .font(.caption)
                .monospacedDigit()
                .frame(width: 30, alignment: .trailing)
        }
    }

    private var scoreColor: Color {
        if score >= 80 { return .green }
        if score >= 60 { return .blue }
        if score >= 40 { return .orange }
        return .red
    }
}
