using System;

namespace EventPlannerSim.Core
{
    /// <summary>
    /// Represents an in-game date. Serializable for save/load support.
    /// Uses simplified 30-day months for easier calculations.
    /// </summary>
    [Serializable]
    public struct GameDate : IComparable<GameDate>, IEquatable<GameDate>
    {
        public int day;   // 1-30 (simplified month)
        public int month; // 1-12
        public int year;  // Starting year: 1

        public GameDate(int day, int month, int year)
        {
            this.day = day;
            this.month = month;
            this.year = year;
        }

        /// <summary>
        /// Total days since game start for easy comparison.
        /// </summary>
        public int TotalDays => (year - 1) * 360 + (month - 1) * 30 + day;

        /// <summary>
        /// Add days to current date, handling month/year overflow.
        /// </summary>
        public GameDate AddDays(int days)
        {
            int totalDays = TotalDays + days;
            int newYear = (totalDays - 1) / 360 + 1;
            int remainingDays = (totalDays - 1) % 360;
            int newMonth = remainingDays / 30 + 1;
            int newDay = remainingDays % 30 + 1;
            return new GameDate(newDay, newMonth, newYear);
        }

        /// <summary>
        /// Calculate days between two dates.
        /// </summary>
        public static int DaysBetween(GameDate from, GameDate to) => to.TotalDays - from.TotalDays;

        /// <summary>
        /// Check if this date is in the past relative to another date.
        /// </summary>
        public bool IsBefore(GameDate other) => TotalDays < other.TotalDays;

        /// <summary>
        /// Check if this date is in the future relative to another date.
        /// </summary>
        public bool IsAfter(GameDate other) => TotalDays > other.TotalDays;

        public int CompareTo(GameDate other) => TotalDays.CompareTo(other.TotalDays);
        public bool Equals(GameDate other) => TotalDays == other.TotalDays;
        public override bool Equals(object obj) => obj is GameDate other && Equals(other);
        public override int GetHashCode() => TotalDays;

        public static bool operator ==(GameDate left, GameDate right) => left.Equals(right);
        public static bool operator !=(GameDate left, GameDate right) => !left.Equals(right);
        public static bool operator <(GameDate left, GameDate right) => left.TotalDays < right.TotalDays;
        public static bool operator >(GameDate left, GameDate right) => left.TotalDays > right.TotalDays;
        public static bool operator <=(GameDate left, GameDate right) => left.TotalDays <= right.TotalDays;
        public static bool operator >=(GameDate left, GameDate right) => left.TotalDays >= right.TotalDays;

        public override string ToString() => $"Day {day}, Month {month}, Year {year}";

        /// <summary>
        /// Display format for UI (e.g., "Month 3, Day 15").
        /// </summary>
        public string ToDisplayString() => $"Month {month}, Day {day}";

        /// <summary>
        /// Create a GameDate from total days since game start.
        /// </summary>
        public static GameDate FromTotalDays(int totalDays)
        {
            int newYear = (totalDays - 1) / 360 + 1;
            int remainingDays = (totalDays - 1) % 360;
            int newMonth = remainingDays / 30 + 1;
            int newDay = remainingDays % 30 + 1;
            return new GameDate(newDay, newMonth, newYear);
        }
    }
}
