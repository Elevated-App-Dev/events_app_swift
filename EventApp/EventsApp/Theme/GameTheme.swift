import SwiftUI

/// Central theme for the Event Planning Simulator.
/// All colors, typography, spacing, and animation constants in one place.
/// Reference: documentation/design/style-guide.md
enum GameTheme {

    // MARK: - Colors

    enum Colors {
        // Backgrounds & surfaces
        static let background = Color(hex: 0x0D0D0D)
        static let surface = Color(hex: 0x1A1A1A)
        static let elevated = Color(hex: 0x252525)
        static let border = Color(hex: 0x333333)

        // Text
        static let textPrimary = Color.white
        static let textSecondary = Color(hex: 0xB0B0B0)
        static let textMuted = Color(hex: 0x606060)

        // Accent — each color has ONE meaning
        static let money = Color(hex: 0x4ECDC4)       // Teal — finances
        static let reputation = Color(hex: 0xFFE66D)   // Gold — stars/rating
        static let success = Color(hex: 0x7BC950)      // Green — positive
        static let warning = Color(hex: 0xF7931A)      // Orange — attention
        static let error = Color(hex: 0xE84855)        // Red — negative
        static let accent = Color(hex: 0x667EEA)       // Purple/blue — interactive

        // Map zones
        static let zoneNeighborhood = Color(hex: 0x2D4739)
        static let zoneDowntown = Color(hex: 0x3D3D5C)
        static let zoneUptown = Color(hex: 0x4A3D5C)
        static let zoneWaterfront = Color(hex: 0x2D4A5C)
    }

    // MARK: - Typography

    enum Typography {
        static let display = Font.system(size: 48, weight: .bold)
        static let h1 = Font.system(size: 32, weight: .semibold)
        static let h2 = Font.system(size: 24, weight: .semibold)
        static let h3 = Font.system(size: 20, weight: .medium)
        static let body = Font.system(size: 16, weight: .regular)
        static let caption = Font.system(size: 14, weight: .regular)
        static let micro = Font.system(size: 12, weight: .medium)

        /// Monospaced digits for financial values.
        static let money = Font.system(size: 16, weight: .semibold).monospacedDigit()
        static let moneyLarge = Font.system(size: 24, weight: .bold).monospacedDigit()
    }

    // MARK: - Spacing (8pt grid)

    enum Spacing {
        static let xs: CGFloat = 8
        static let sm: CGFloat = 16
        static let md: CGFloat = 24
        static let lg: CGFloat = 32
        static let xl: CGFloat = 48
        static let xxl: CGFloat = 64

        /// Standard screen horizontal margin.
        static let screenMargin: CGFloat = 24
    }

    // MARK: - Radii

    enum Radius {
        static let small: CGFloat = 8
        static let medium: CGFloat = 12
        static let large: CGFloat = 16
        static let pill: CGFloat = 9999
    }

    // MARK: - Sizing

    enum Size {
        static let buttonHeight: CGFloat = 56
        static let touchTarget: CGFloat = 44
        static let iconBase: CGFloat = 24
        static let statusDot: CGFloat = 8
        static let progressBarHeight: CGFloat = 4
        static let badgeSize: CGFloat = 20
    }

    // MARK: - Animation

    enum Anim {
        static let fast: Double = 0.1
        static let normal: Double = 0.2
        static let slow: Double = 0.3
        static let emphasis: Double = 0.4

        static let spring = Animation.spring(response: 0.3, dampingFraction: 0.7)
        static let panelSlide = Animation.easeOut(duration: 0.25)
        static let fadeIn = Animation.easeOut(duration: 0.2)
        static let scoreReveal = Animation.spring(response: 0.3, dampingFraction: 0.6)
    }
}

// MARK: - Color Hex Initializer

extension Color {
    init(hex: UInt, opacity: Double = 1.0) {
        self.init(
            .sRGB,
            red: Double((hex >> 16) & 0xFF) / 255,
            green: Double((hex >> 8) & 0xFF) / 255,
            blue: Double(hex & 0xFF) / 255,
            opacity: opacity
        )
    }
}

// MARK: - View Modifiers

extension View {
    /// Apply the game's dark background.
    func gameBackground() -> some View {
        self.background(GameTheme.Colors.background.ignoresSafeArea())
    }

    /// Style as a surface card.
    func surfaceCard() -> some View {
        self
            .padding(GameTheme.Spacing.sm)
            .background(GameTheme.Colors.surface)
            .clipShape(RoundedRectangle(cornerRadius: GameTheme.Radius.medium))
    }

    /// Style as an elevated card.
    func elevatedCard() -> some View {
        self
            .padding(GameTheme.Spacing.sm)
            .background(GameTheme.Colors.elevated)
            .overlay(
                RoundedRectangle(cornerRadius: GameTheme.Radius.large)
                    .stroke(GameTheme.Colors.border, lineWidth: 1)
            )
            .clipShape(RoundedRectangle(cornerRadius: GameTheme.Radius.large))
    }

    /// Primary button style.
    func primaryButton() -> some View {
        self
            .font(.system(size: 16, weight: .semibold))
            .foregroundStyle(GameTheme.Colors.textPrimary)
            .frame(maxWidth: .infinity)
            .frame(height: GameTheme.Size.buttonHeight)
            .background(GameTheme.Colors.accent)
            .clipShape(RoundedRectangle(cornerRadius: GameTheme.Radius.medium))
    }

    /// Secondary button style.
    func secondaryButton() -> some View {
        self
            .font(.system(size: 16, weight: .semibold))
            .foregroundStyle(GameTheme.Colors.textSecondary)
            .frame(maxWidth: .infinity)
            .frame(height: GameTheme.Size.buttonHeight)
            .overlay(
                RoundedRectangle(cornerRadius: GameTheme.Radius.medium)
                    .stroke(GameTheme.Colors.textMuted, lineWidth: 1)
            )
    }
}
