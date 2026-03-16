import SwiftUI

struct EventDetailView: View {
    @Environment(GameManager.self) private var gameManager
    let eventIndex: Int
    @State private var showVenuePicker = false
    @State private var vendorCategoryToBrowse: IdentifiableVendorCategory?
    @State private var bookingMessage: String?
    @State private var showBookingAlert = false

    private var event: EventData? {
        guard eventIndex >= 0 && eventIndex < gameManager.activeEvents.count else { return nil }
        return gameManager.activeEvents[eventIndex]
    }

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
                    LabeledContent("Status", value: event.status.rawValue.capitalized)
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

                // Vendors — show booked vendors and allow contacting new ones
                Section("Vendors") {
                    // Already booked vendors
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
                                VStack(alignment: .trailing) {
                                    Text("$\(assignment.agreedPrice, specifier: "%.0f")")
                                        .font(.caption)
                                    Image(systemName: "checkmark.circle.fill")
                                        .foregroundStyle(.green)
                                        .font(.caption)
                                }
                            }
                        }
                    }

                    // Vendors in progress (contacted but not booked)
                    let contactedVendorIds = Set(
                        gameManager.advanceSystem.getActivitiesForEvent(eventId: event.id)
                            .compactMap { $0.vendorId }
                    )
                    let bookedVendorIds = Set(event.vendors.map { $0.vendorId })
                    let pendingVendorIds = contactedVendorIds.subtracting(bookedVendorIds)

                    ForEach(Array(pendingVendorIds), id: \.self) { vendorId in
                        if let vendor = SeedData.vendor(byId: vendorId) {
                            HStack {
                                VStack(alignment: .leading) {
                                    Text(vendor.vendorName)
                                        .font(.subheadline)
                                    Text(vendor.category.rawValue.capitalized)
                                        .font(.caption)
                                        .foregroundStyle(.secondary)
                                }
                                Spacer()
                                Text("In progress")
                                    .font(.caption)
                                    .foregroundStyle(.orange)
                            }
                        }
                    }

                    // Contact new vendors
                    if canEditPlanning(event) {
                        let bookedCategories = Set(event.vendors.map { $0.category })
                        let pendingCategories = Set(
                            pendingVendorIds.compactMap { id in SeedData.vendor(byId: id)?.category }
                        )
                        let availableCategories = VendorCategory.allCases.filter {
                            !bookedCategories.contains($0) && !pendingCategories.contains($0)
                        }

                        if !availableCategories.isEmpty {
                            Menu("Contact Vendor") {
                                ForEach(availableCategories, id: \.self) { category in
                                    Button(category.rawValue.capitalized) {
                                        vendorCategoryToBrowse = IdentifiableVendorCategory(category: category)
                                    }
                                }
                            }
                        }
                    }
                }

                // Activity log for this event
                let activities = gameManager.advanceSystem.getActivitiesForEvent(eventId: event.id)
                if !activities.isEmpty {
                    Section("Activity Log") {
                        ForEach(activities) { activity in
                            HStack {
                                Circle()
                                    .fill(statusColor(activity.status))
                                    .frame(width: 8, height: 8)
                                VStack(alignment: .leading, spacing: 2) {
                                    Text(activity.content.subject)
                                        .font(.caption)
                                        .lineLimit(1)
                                    Text(activity.scheduledDate.shortFormatted)
                                        .font(.caption2)
                                        .foregroundStyle(.secondary)
                                }
                                Spacer()
                                Text(activity.status.rawValue)
                                    .font(.caption2)
                                    .foregroundStyle(.secondary)
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
            .sheet(item: $vendorCategoryToBrowse) { item in
                VendorBrowserView(
                    eventId: event.id,
                    category: item.category,
                    eventDate: event.eventDate
                )
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

    private func statusColor(_ status: ActivityStatus) -> Color {
        switch status {
        case .completed: return .green
        case .ready: return .orange
        case .scheduled: return .blue
        case .overdue: return .red
        case .missed: return .red
        case .cancelled: return .gray
        }
    }
}

struct IdentifiableVendorCategory: Identifiable {
    let id: String
    let category: VendorCategory
    init(category: VendorCategory) {
        self.id = category.rawValue
        self.category = category
    }
}
