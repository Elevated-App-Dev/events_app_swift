import SwiftUI

/// Browse available vendors in a category and contact them.
/// Contacting a vendor starts the multi-step process (availability → quote → negotiate → book).
/// Results arrive as messages in the Messages app.
struct VendorBrowserView: View {
    @Environment(GameManager.self) private var gameManager
    @Environment(\.dismiss) private var dismiss
    let eventId: String
    let category: VendorCategory
    let eventDate: GameDate

    private var availableVendors: [VendorData] {
        SeedData.vendors
            .filter { $0.category == category }
            .filter { gameManager.playerData.unlockedZones.contains($0.zone) }
    }

    var body: some View {
        NavigationStack {
            ScrollView {
                VStack(spacing: GameTheme.Spacing.sm) {
                    if availableVendors.isEmpty {
                        VStack(spacing: GameTheme.Spacing.sm) {
                            Image(systemName: "person.crop.rectangle.badge.xmark")
                                .font(.system(size: 48))
                                .foregroundStyle(GameTheme.Colors.textMuted)
                            Text("No \(category.rawValue)s available")
                                .font(GameTheme.Typography.body)
                                .foregroundStyle(GameTheme.Colors.textMuted)
                        }
                        .frame(maxWidth: .infinity)
                        .padding(.top, GameTheme.Spacing.xl)
                    } else {
                        ForEach(availableVendors, id: \.id) { vendor in
                            VendorContactCard(
                                vendor: vendor,
                                eventId: eventId,
                                eventDate: eventDate,
                                onContact: { dismiss() }
                            )
                        }
                    }
                }
                .padding(.horizontal, GameTheme.Spacing.md)
                .padding(.top, GameTheme.Spacing.sm)
            }
            .navigationTitle("Browse \(category.rawValue.capitalized)s")
            .navigationBarTitleDisplayMode(.inline)
            .toolbar {
                ToolbarItem(placement: .cancellationAction) {
                    Button("Cancel") { dismiss() }
                }
            }
            .background(GameTheme.Colors.background)
            .scrollContentBackground(.hidden)
        }
        .preferredColorScheme(.dark)
    }
}

struct VendorContactCard: View {
    @Environment(GameManager.self) private var gameManager
    let vendor: VendorData
    let eventId: String
    let eventDate: GameDate
    let onContact: () -> Void

    private var relationship: VendorRelationship? {
        gameManager.vendorRelationships[vendor.id]
    }

    private var alreadyContacted: Bool {
        gameManager.advanceSystem.getActivitiesForEvent(eventId: eventId)
            .contains { $0.vendorId == vendor.id }
    }

    var body: some View {
        VStack(alignment: .leading, spacing: GameTheme.Spacing.xs) {
            // Vendor header
            HStack {
                VStack(alignment: .leading, spacing: 4) {
                    Text(vendor.vendorName)
                        .font(GameTheme.Typography.h3)
                        .foregroundStyle(GameTheme.Colors.textPrimary)
                    Text(vendor.specialty)
                        .font(GameTheme.Typography.caption)
                        .foregroundStyle(GameTheme.Colors.textSecondary)
                }
                Spacer()
                VStack(alignment: .trailing, spacing: 4) {
                    Text(vendor.tier.rawValue.capitalized)
                        .font(GameTheme.Typography.micro)
                        .fontWeight(.medium)
                        .padding(.horizontal, 6)
                        .padding(.vertical, 2)
                        .background(tierColor.opacity(0.15))
                        .foregroundStyle(tierColor)
                        .clipShape(Capsule())
                }
            }

            // Details
            HStack(spacing: GameTheme.Spacing.md) {
                Label("~$\(Int(vendor.basePrice))", systemImage: "banknote")
                    .font(GameTheme.Typography.micro)
                    .foregroundStyle(GameTheme.Colors.money)

                Label(vendor.zone.rawValue.capitalized, systemImage: "mappin")
                    .font(GameTheme.Typography.micro)
                    .foregroundStyle(GameTheme.Colors.textMuted)

                if let rel = relationship {
                    HStack(spacing: 2) {
                        Image(systemName: "arrow.triangle.2.circlepath")
                        Text("\(rel.totalBookings)")
                    }
                    .font(GameTheme.Typography.micro)
                    .foregroundStyle(GameTheme.Colors.textMuted)
                }
            }

            // Relationship indicator (subtle)
            if let rel = relationship, rel.tier != .neutral {
                HStack(spacing: 4) {
                    Circle()
                        .fill(relationshipColor(rel.tier))
                        .frame(width: 6, height: 6)
                    Text(relationshipLabel(rel.tier))
                        .font(.system(size: 10))
                        .foregroundStyle(relationshipColor(rel.tier))
                }
            }

            // Action
            if alreadyContacted {
                HStack {
                    Image(systemName: "checkmark.circle")
                    Text("Already contacted — check Messages")
                }
                .font(GameTheme.Typography.micro)
                .foregroundStyle(GameTheme.Colors.success)
            } else {
                Button(action: {
                    gameManager.initiateVendorContact(eventId: eventId, vendor: vendor)
                    onContact()
                }) {
                    HStack {
                        Image(systemName: "envelope")
                        Text("Contact \(vendor.vendorName)")
                    }
                    .font(GameTheme.Typography.micro)
                    .fontWeight(.semibold)
                    .foregroundStyle(GameTheme.Colors.textPrimary)
                    .frame(maxWidth: .infinity)
                    .padding(.vertical, GameTheme.Spacing.xs)
                    .background(GameTheme.Colors.accent)
                    .clipShape(RoundedRectangle(cornerRadius: GameTheme.Radius.small))
                }
            }
        }
        .surfaceCard()
    }

    private var tierColor: Color {
        switch vendor.tier {
        case .budget: return GameTheme.Colors.textMuted
        case .standard: return GameTheme.Colors.accent
        case .premium: return GameTheme.Colors.reputation
        case .luxury: return GameTheme.Colors.money
        }
    }

    private func relationshipColor(_ tier: RelationshipTier) -> Color {
        switch tier {
        case .burned: return GameTheme.Colors.error
        case .strained: return GameTheme.Colors.warning
        case .neutral: return GameTheme.Colors.textMuted
        case .good: return GameTheme.Colors.success
        case .preferred: return GameTheme.Colors.money
        }
    }

    private func relationshipLabel(_ tier: RelationshipTier) -> String {
        switch tier {
        case .burned: return "Burned bridge"
        case .strained: return "Strained"
        case .neutral: return ""
        case .good: return "Good relationship"
        case .preferred: return "Preferred vendor"
        }
    }
}
