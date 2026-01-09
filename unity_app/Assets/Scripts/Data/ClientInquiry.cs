using System;
using System.Collections.Generic;
using EventPlannerSim.Core;

namespace EventPlannerSim.Data
{
    /// <summary>
    /// Represents a client inquiry awaiting player response.
    /// Requirements: R5.1-R5.8
    /// </summary>
    [Serializable]
    public class ClientInquiry
    {
        /// <summary>
        /// Unique identifier for this inquiry.
        /// </summary>
        public string inquiryId;

        /// <summary>
        /// Name of the client making the inquiry.
        /// </summary>
        public string clientName;

        /// <summary>
        /// ID of the event type being requested.
        /// </summary>
        public string eventTypeId;

        /// <summary>
        /// Specific sub-category of the event (e.g., "Princess Theme Birthday").
        /// </summary>
        public string subCategory;

        /// <summary>
        /// Display name combining client name and sub-category.
        /// Format: "[clientName]'s [subCategory]"
        /// </summary>
        public string eventDisplayName;

        /// <summary>
        /// Personality type of the client.
        /// </summary>
        public ClientPersonality personality;

        /// <summary>
        /// Total budget for the event.
        /// </summary>
        public int budget;

        /// <summary>
        /// Expected number of guests.
        /// </summary>
        public int guestCount;

        /// <summary>
        /// Requested date for the event.
        /// </summary>
        public GameDate eventDate;

        /// <summary>
        /// Special requirements or constraints for the event.
        /// </summary>
        public List<string> specialRequirements = new List<string>();

        /// <summary>
        /// Timestamp when the inquiry was created (stored as ticks for serialization).
        /// </summary>
        public long createdTimeTicks;

        /// <summary>
        /// Timestamp when the inquiry expires (stored as ticks for serialization).
        /// Inquiries expire 20 minutes after creation.
        /// </summary>
        public long expiresTimeTicks;

        /// <summary>
        /// Gets or sets the creation time as DateTime.
        /// </summary>
        public DateTime CreatedTime
        {
            get => new DateTime(createdTimeTicks);
            set => createdTimeTicks = value.Ticks;
        }

        /// <summary>
        /// Gets or sets the expiration time as DateTime.
        /// </summary>
        public DateTime ExpiresTime
        {
            get => new DateTime(expiresTimeTicks);
            set => expiresTimeTicks = value.Ticks;
        }

        /// <summary>
        /// Checks if the inquiry has expired.
        /// </summary>
        public bool IsExpired => DateTime.UtcNow > ExpiresTime;

        /// <summary>
        /// Whether this inquiry was generated from a referral.
        /// </summary>
        public bool isReferral;

        /// <summary>
        /// Name of the client who referred this inquiry (if applicable).
        /// </summary>
        public string referredByClientName;

        /// <summary>
        /// Expiration time in minutes for inquiries.
        /// </summary>
        public const int ExpirationMinutes = 20;

        /// <summary>
        /// Creates a new inquiry with automatic expiration time.
        /// </summary>
        public static ClientInquiry Create(
            string clientName,
            string eventTypeId,
            string subCategory,
            ClientPersonality personality,
            int budget,
            int guestCount,
            GameDate eventDate)
        {
            var now = DateTime.UtcNow;
            return new ClientInquiry
            {
                inquiryId = Guid.NewGuid().ToString(),
                clientName = clientName,
                eventTypeId = eventTypeId,
                subCategory = subCategory,
                eventDisplayName = $"{clientName}'s {subCategory}",
                personality = personality,
                budget = budget,
                guestCount = guestCount,
                eventDate = eventDate,
                createdTimeTicks = now.Ticks,
                expiresTimeTicks = now.AddMinutes(ExpirationMinutes).Ticks
            };
        }

        /// <summary>
        /// Creates a referral inquiry with bonus attributes.
        /// </summary>
        public static ClientInquiry CreateReferral(
            string clientName,
            string eventTypeId,
            string subCategory,
            ClientPersonality personality,
            int budget,
            int guestCount,
            GameDate eventDate,
            string referredByClientName)
        {
            var inquiry = Create(clientName, eventTypeId, subCategory, personality, budget, guestCount, eventDate);
            inquiry.isReferral = true;
            inquiry.referredByClientName = referredByClientName;
            return inquiry;
        }
    }
}
