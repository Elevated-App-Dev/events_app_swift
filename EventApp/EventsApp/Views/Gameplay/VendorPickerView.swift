import SwiftUI

struct VendorPickerView: View {
    @Environment(GameManager.self) private var gameManager
    @Environment(\.dismiss) private var dismiss
    let eventIndex: Int
    let category: VendorCategory
    var onResult: (String) -> Void

    private var availableVendors: [VendorData] {
        SeedData.vendors(forCategory: category)
            .filter { gameManager.playerData.unlockedZones.contains($0.zone) }
    }

    var body: some View {
        NavigationStack {
            List(availableVendors) { vendor in
                VendorRowView(vendor: vendor)
                    .contentShape(Rectangle())
                    .onTapGesture {
                        hireVendor(vendor)
                    }
            }
            .navigationTitle("Hire \(category.rawValue.capitalized)")
            .navigationBarTitleDisplayMode(.inline)
            .toolbar {
                ToolbarItem(placement: .cancellationAction) {
                    Button("Cancel") { dismiss() }
                }
            }
            .overlay {
                if availableVendors.isEmpty {
                    ContentUnavailableView(
                        "No \(category.rawValue.capitalized)s Available",
                        systemImage: "person.slash",
                        description: Text("No vendors of this type are available in your unlocked zones.")
                    )
                }
            }
        }
    }

    private func hireVendor(_ vendor: VendorData) {
        let result = gameManager.assignVendor(eventIndex: eventIndex, vendor: vendor)
        onResult(result.message)
        if result.success {
            dismiss()
        }
    }
}

struct VendorRowView: View {
    let vendor: VendorData

    var body: some View {
        VStack(alignment: .leading, spacing: 6) {
            HStack {
                Text(vendor.vendorName)
                    .font(.headline)
                Spacer()
                Text("$\(vendor.basePrice, specifier: "%.0f")")
                    .font(.subheadline)
                    .fontWeight(.semibold)
                    .foregroundStyle(.green)
            }

            HStack {
                Text(vendor.tier.rawValue.capitalized)
                    .font(.caption)
                    .padding(.horizontal, 6)
                    .padding(.vertical, 2)
                    .background(tierColor.opacity(0.15))
                    .foregroundStyle(tierColor)
                    .clipShape(Capsule())

                Text(vendor.specialty)
                    .font(.caption)
                    .foregroundStyle(.secondary)

                Spacer()

                HStack(spacing: 2) {
                    Image(systemName: "star.fill")
                        .font(.caption2)
                        .foregroundStyle(.yellow)
                    Text("\(vendor.qualityRating, specifier: "%.0f")")
                        .font(.caption)
                }
            }
        }
        .padding(.vertical, 4)
    }

    private var tierColor: Color {
        switch vendor.tier {
        case .budget: return .green
        case .standard: return .blue
        case .premium: return .purple
        case .luxury: return .orange
        }
    }
}
