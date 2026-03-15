import SwiftUI

struct EventDetailView: View {
    @Environment(GameManager.self) private var gameManager
    let eventIndex: Int
    @State private var showVenuePicker = false
    @State private var showVendorPicker = false
    @State private var vendorCategoryToPick: VendorCategory = .caterer
    @State private var bookingMessage: String?
    @State private var showBookingAlert = false

    private var event: EventData? {
        guard eventIndex >= 0 && eventIndex < gameManager.activeEvents.count else { return nil }
        return gameManager.activeEvents[eventIndex]
    }

    /// Allow venue/vendor selection during booking, planning, and active planning phases.
    private func canEditPlanning(_ event: EventData) -> Bool {
        switch event.phase {
        case .booking, .prePlanning, .activePlanning:
            return true
        default:
            return false
        }
    }

    var body: some View {
        if let event {
            List {
                // Header
                Section {
                    LabeledContent("Client", value: event.clientName)
                    LabeledContent("Type", value: event.subCategory)
                    LabeledContent("Guests", value: "\(event.guestCount)")
                    LabeledContent("Date", value: event.eventDate.formatted)
                    LabeledContent("Phase") {
                        PhaseBadge(phase: event.phase)
                    }
                }

                // Budget
                Section("Budget") {
                    LabeledContent("Total") {
                        Text("$\(event.budget.total, specifier: "%.0f")")
                    }
                    LabeledContent("Spent") {
                        Text("$\(event.budget.spent, specifier: "%.0f")")
                    }
                    LabeledContent("Remaining") {
                        Text("$\(event.budget.remaining, specifier: "%.0f")")
                            .foregroundStyle(event.budget.remaining < 0 ? .red : .green)
                    }
                }

                // Venue
                Section("Venue") {
                    if let venueId = event.venueId, let venue = SeedData.venue(byId: venueId) {
                        VStack(alignment: .leading, spacing: 4) {
                            Text(venue.venueName)
                                .font(.headline)
                            HStack {
                                Label(venue.isIndoor ? "Indoor" : "Outdoor", systemImage: venue.isIndoor ? "building.2" : "sun.max")
                                Spacer()
                                Text("Capacity: \(venue.capacityComfortable)")
                            }
                            .font(.caption)
                            .foregroundStyle(.secondary)
                        }
                    } else if canEditPlanning(event) {
                        Button("Select Venue") {
                            showVenuePicker = true
                        }
                    } else {
                        Text("No venue selected")
                            .foregroundStyle(.secondary)
                    }
                }

                // Vendors
                Section("Vendors") {
                    ForEach(event.vendors, id: \.vendorId) { assignment in
                        if let vendor = SeedData.vendor(byId: assignment.vendorId) {
                            HStack {
                                VStack(alignment: .leading) {
                                    Text(vendor.vendorName)
                                        .font(.subheadline)
                                    Text(vendor.category.rawValue.capitalized)
                                        .font(.caption)
                                        .foregroundStyle(.secondary)
                                }
                                Spacer()
                                Text("$\(assignment.agreedPrice, specifier: "%.0f")")
                                    .font(.caption)
                                    .foregroundStyle(.secondary)
                            }
                        }
                    }

                    if canEditPlanning(event) {
                        Menu("Add Vendor") {
                            ForEach(VendorCategory.allCases, id: \.self) { category in
                                let alreadyHas = event.vendors.contains { $0.category == category }
                                if !alreadyHas {
                                    Button(category.rawValue.capitalized) {
                                        vendorCategoryToPick = category
                                        showVendorPicker = true
                                    }
                                }
                            }
                        }
                    }
                }
            }
            .navigationTitle(event.eventTitle)
            .navigationBarTitleDisplayMode(.inline)
            .sheet(isPresented: $showVenuePicker) {
                VenuePickerView(eventIndex: eventIndex) { message in
                    bookingMessage = message
                    showBookingAlert = true
                }
            }
            .sheet(isPresented: $showVendorPicker) {
                VendorPickerView(eventIndex: eventIndex, category: vendorCategoryToPick) { message in
                    bookingMessage = message
                    showBookingAlert = true
                }
            }
            .alert("Booking", isPresented: $showBookingAlert) {
                Button("OK") {}
            } message: {
                Text(bookingMessage ?? "")
            }
        } else {
            ContentUnavailableView("Event Not Found", systemImage: "exclamationmark.triangle")
        }
    }
}
