using UnityEngine;

namespace EventPlannerSim.UI.Core
{
    /// <summary>
    /// Design tokens following the minimalist style guide.
    /// Inspired by Mini Metro + Coffee Inc aesthetic.
    /// </summary>
    public static class DesignTokens
    {
        #region Colors - Background

        /// <summary>
        /// Pure black background. #0D0D0D
        /// </summary>
        public static readonly Color Background = new Color(0.051f, 0.051f, 0.051f, 1f);

        /// <summary>
        /// Surface color for cards/panels. #1A1A1A
        /// </summary>
        public static readonly Color Surface = new Color(0.102f, 0.102f, 0.102f, 1f);

        /// <summary>
        /// Elevated surface for modals/hover. #252525
        /// </summary>
        public static readonly Color Elevated = new Color(0.145f, 0.145f, 0.145f, 1f);

        #endregion

        #region Colors - Text

        /// <summary>
        /// Primary text color (white). #FFFFFF
        /// </summary>
        public static readonly Color TextPrimary = Color.white;

        /// <summary>
        /// Secondary text color. #B0B0B0
        /// </summary>
        public static readonly Color TextSecondary = new Color(0.69f, 0.69f, 0.69f, 1f);

        /// <summary>
        /// Muted/disabled text. #606060
        /// </summary>
        public static readonly Color TextMuted = new Color(0.376f, 0.376f, 0.376f, 1f);

        #endregion

        #region Colors - Accent

        /// <summary>
        /// Money/financial color (teal). #4ECDC4
        /// </summary>
        public static readonly Color Money = new Color(0.306f, 0.804f, 0.769f, 1f);

        /// <summary>
        /// Reputation/star color (gold). #FFE66D
        /// </summary>
        public static readonly Color Reputation = new Color(1f, 0.902f, 0.427f, 1f);

        /// <summary>
        /// Success/positive color (green). #7BC950
        /// </summary>
        public static readonly Color Success = new Color(0.482f, 0.788f, 0.314f, 1f);

        /// <summary>
        /// Warning/attention color (orange). #F7931A
        /// </summary>
        public static readonly Color Warning = new Color(0.969f, 0.576f, 0.102f, 1f);

        /// <summary>
        /// Error/negative color (red). #E84855
        /// </summary>
        public static readonly Color Error = new Color(0.91f, 0.282f, 0.333f, 1f);

        /// <summary>
        /// Primary accent/interactive (purple-blue). #667EEA
        /// </summary>
        public static readonly Color Accent = new Color(0.4f, 0.494f, 0.918f, 1f);

        #endregion

        #region Colors - Map Zones

        /// <summary>
        /// Neighborhood zone (soft green). #2D4739
        /// </summary>
        public static readonly Color ZoneNeighborhood = new Color(0.176f, 0.278f, 0.224f, 1f);

        /// <summary>
        /// Downtown zone (muted blue). #3D3D5C
        /// </summary>
        public static readonly Color ZoneDowntown = new Color(0.239f, 0.239f, 0.361f, 1f);

        /// <summary>
        /// Uptown zone (soft purple). #4A3D5C
        /// </summary>
        public static readonly Color ZoneUptown = new Color(0.29f, 0.239f, 0.361f, 1f);

        /// <summary>
        /// Waterfront zone (ocean blue). #2D4A5C
        /// </summary>
        public static readonly Color ZoneWaterfront = new Color(0.176f, 0.29f, 0.361f, 1f);

        #endregion

        #region Spacing

        /// <summary>
        /// Extra small spacing (8px).
        /// </summary>
        public const float SpacingXS = 8f;

        /// <summary>
        /// Small spacing (16px).
        /// </summary>
        public const float SpacingSM = 16f;

        /// <summary>
        /// Medium spacing (24px).
        /// </summary>
        public const float SpacingMD = 24f;

        /// <summary>
        /// Large spacing (32px).
        /// </summary>
        public const float SpacingLG = 32f;

        /// <summary>
        /// Extra large spacing (48px).
        /// </summary>
        public const float SpacingXL = 48f;

        /// <summary>
        /// 2x extra large spacing (64px).
        /// </summary>
        public const float Spacing2XL = 64f;

        #endregion

        #region Typography Sizes

        /// <summary>
        /// Display text size (48px).
        /// </summary>
        public const float FontDisplay = 48f;

        /// <summary>
        /// H1 text size (32px).
        /// </summary>
        public const float FontH1 = 32f;

        /// <summary>
        /// H2 text size (24px).
        /// </summary>
        public const float FontH2 = 24f;

        /// <summary>
        /// H3 text size (20px).
        /// </summary>
        public const float FontH3 = 20f;

        /// <summary>
        /// Body text size (16px).
        /// </summary>
        public const float FontBody = 16f;

        /// <summary>
        /// Caption text size (14px).
        /// </summary>
        public const float FontCaption = 14f;

        /// <summary>
        /// Micro text size (12px).
        /// </summary>
        public const float FontMicro = 12f;

        #endregion

        #region Border Radius

        /// <summary>
        /// Small radius for buttons/inputs (8px).
        /// </summary>
        public const float RadiusSmall = 8f;

        /// <summary>
        /// Medium radius for cards (12px).
        /// </summary>
        public const float RadiusMedium = 12f;

        /// <summary>
        /// Large radius for modals (16px).
        /// </summary>
        public const float RadiusLarge = 16f;

        /// <summary>
        /// Full radius for pills/badges (9999px).
        /// </summary>
        public const float RadiusFull = 9999f;

        #endregion

        #region Touch Targets

        /// <summary>
        /// Minimum touch target size (44px).
        /// </summary>
        public const float MinTouchTarget = 44f;

        /// <summary>
        /// Recommended touch target size (48px).
        /// </summary>
        public const float RecommendedTouchTarget = 48f;

        #endregion

        #region Utility Methods

        /// <summary>
        /// Get satisfaction score color based on value.
        /// </summary>
        public static Color GetSatisfactionColor(int score)
        {
            if (score >= 90) return Success;
            if (score >= 70) return Money;
            if (score >= 50) return Warning;
            return Error;
        }

        /// <summary>
        /// Get color for money change (gain vs loss).
        /// </summary>
        public static Color GetMoneyChangeColor(float amount)
        {
            return amount >= 0 ? Success : Error;
        }

        /// <summary>
        /// Get color for reputation change.
        /// </summary>
        public static Color GetReputationChangeColor(int amount)
        {
            return amount >= 0 ? Reputation : Error;
        }

        /// <summary>
        /// Darken a color by a factor (for locked zones).
        /// </summary>
        public static Color Darken(Color color, float factor = 0.5f)
        {
            return new Color(
                color.r * factor,
                color.g * factor,
                color.b * factor,
                color.a
            );
        }

        #endregion
    }
}
