import SwiftUI

struct VenuePickerView: View {
    @Environment(GameManager.self) private var gameManager
    @Environment(\.dismiss) private var dismiss
    let eventIndex: Int
    var onResult: (String) -> Void

    private var event: EventData? {
        guard eventIndex >= 0 && eventIndex < gameManager.activeEvents.count else { return nil }
        return gameManager.activeEvents[eventIndex]
    }

    private var availableVenues: [VenueData] {
        SeedData.venues(forZones: gameManager.playerData.unlockedZones)
            .filter { venue in
                guard let event else { return false }
                return event.guestCount <= venue.capacityMax
            }
    }

    var body: some View {
        NavigationStack {
            List(availableVenues) { venue in
                VenueCardView(venue: venue, guestCount: event?.guestCount ?? 0, eventDate: event?.eventDate)
                    .contentShape(Rectangle())
                    .onTapGesture {
                        bookVenue(venue)
                    }
            }
            .navigationTitle("Select Venue")
            .navigationBarTitleDisplayMode(.inline)
            .toolbar {
                ToolbarItem(placement: .cancellationAction) {
                    Button("Cancel") { dismiss() }
                }
            }
            .scrollContentBackground(.hidden)
            .background(GameTheme.Colors.background)
        }
        .preferredColorScheme(.dark)
    }

    private func bookVenue(_ venue: VenueData) {
        let result = gameManager.assignVenue(eventIndex: eventIndex, venue: venue)
        onResult(result.message)
        if result.success {
            dismiss()
        }
    }
}

struct VenueCardView: View {
    @Environment(GameManager.self) private var gameManager
    let venue: VenueData
    let guestCount: Int
    let eventDate: GameDate?

    private var estimatedPrice: Double {
        venue.basePrice + venue.pricePerGuest * Double(guestCount)
    }

    private var isSnug: Bool {
        guestCount > venue.capacityComfortable
    }

    var body: some View {
        VStack(alignment: .leading, spacing: 8) {
            HStack {
                Text(venue.venueName)
                    .font(.headline)
                Spacer()
                Text("$\(estimatedPrice, specifier: "%.0f")")
                    .font(.subheadline)
                    .fontWeight(.semibold)
                    .foregroundStyle(.green)
            }

            HStack(spacing: 12) {
                Label(venue.isIndoor ? "Indoor" : "Outdoor", systemImage: venue.isIndoor ? "building.2" : "sun.max")
                Label("\(venue.capacityComfortable) comf.", systemImage: "person.2")
                if isSnug {
                    Text("Snug fit")
                        .foregroundStyle(.orange)
                }
            }
            .font(.caption)
            .foregroundStyle(.secondary)

            HStack {
                // Ambiance
                HStack(spacing: 2) {
                    Text("Ambiance:")
                        .font(.caption)
                        .foregroundStyle(.secondary)
                    Text("\(venue.ambianceRating, specifier: "%.0f")/100")
                        .font(.caption)
                        .fontWeight(.medium)
                }

                Spacer()

                // Weather risk for outdoor
                if venue.weatherDependent, let eventDate {
                    let risk = gameManager.weatherSystem.getSimplifiedRisk(for: eventDate)
                    HStack(spacing: 2) {
                        Image(systemName: weatherIcon(risk))
                            .foregroundStyle(weatherColor(risk))
                        Text(risk.rawValue.capitalized)
                            .font(.caption)
                            .foregroundStyle(weatherColor(risk))
                    }
                }
            }
        }
        .padding(.vertical, 4)
    }

    private func weatherIcon(_ risk: WeatherRisk) -> String {
        switch risk {
        case .good: return "sun.max"
        case .risky: return "cloud.sun"
        case .bad: return "cloud.rain"
        }
    }

    private func weatherColor(_ risk: WeatherRisk) -> Color {
        switch risk {
        case .good: return .green
        case .risky: return .orange
        case .bad: return .red
        }
    }
}
