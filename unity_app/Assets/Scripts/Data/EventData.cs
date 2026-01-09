using System;
using System.Collections.Generic;
using EventPlannerSim.Core;

namespace EventPlannerSim.Data
{
    /// <summary>
    /// Represents a complete event with all planning and execution data.
    /// Serializable for save/load support.
    /// </summary>
    [Serializable]
    public class EventData
    {
        public string id;
        public string clientId;
        public string clientName;
        public string eventTitle; // "[ClientName]'s [SubCategory]"
        public string eventTypeId;
        public string subCategory;
        public EventStatus status = EventStatus.Inquiry;
        public EventPhase phase = EventPhase.Booking;

        // Scheduling
        public GameDate eventDate;
        public GameDate acceptedDate;

        // Client Info
        public ClientPersonality personality;
        public int guestCount;

        // Budget
        public EventBudget budget = new EventBudget();

        // Assignments
        public string venueId;
        public List<VendorAssignment> vendors = new List<VendorAssignment>();
        public List<EventTask> tasks = new List<EventTask>();

        // Results (populated after execution)
        public EventResults results;

        // Flags
        public bool isCompanyEvent; // Stage 2: company vs side gig
        public bool isReferral;
        public string referredByClientName;
    }

    /// <summary>
    /// Tracks a vendor assigned to an event.
    /// </summary>
    [Serializable]
    public class VendorAssignment
    {
        public string vendorId;
        public VendorCategory category;
        public float agreedPrice;
        public bool isConfirmed;
        public GameDate bookingDate;
    }

    /// <summary>
    /// Tracks budget allocation and spending for an event.
    /// Requirements: R7.1-R7.8
    /// </summary>
    [Serializable]
    public class EventBudget
    {
        public float total;
        public float venueAllocation;
        public float cateringAllocation;
        public float entertainmentAllocation;
        public float decorationsAllocation;
        public float staffingAllocation;
        public float contingencyAllocation;
        public float spent;

        /// <summary>
        /// Calculates the remaining budget (total - spent).
        /// </summary>
        public float Remaining => total - spent;

        /// <summary>
        /// Calculates the overage amount (how much over budget).
        /// Returns 0 if under or at budget.
        /// </summary>
        public float OverageAmount => Math.Max(0, spent - total);

        /// <summary>
        /// Calculates the overage as a percentage of total budget.
        /// Returns 0 if total is 0 to avoid division by zero.
        /// </summary>
        public float OveragePercent => total > 0 ? (OverageAmount / total) * 100f : 0f;
    }

    /// <summary>
    /// Stores the results of a completed event.
    /// </summary>
    [Serializable]
    public class EventResults
    {
        // Category scores (0-100)
        public float venueScore;
        public float foodScore;
        public float entertainmentScore;
        public float decorationScore;
        public float serviceScore;
        public float expectationScore;

        // Final calculation
        public float finalSatisfaction; // 0-100, clamped
        public float randomEventModifier = 1f;

        // Outcomes
        public float profit;
        public int reputationChange;
        public bool triggeredReferral;
        public string clientFeedback;

        // Random events that occurred
        public List<string> randomEventsOccurred = new List<string>();
    }

    /// <summary>
    /// Client data for satisfaction calculation.
    /// Extracted from EventData for calculator interface.
    /// </summary>
    [Serializable]
    public class ClientData
    {
        public string clientId;
        public string clientName;
        public ClientPersonality personality;
        public int guestCount;
        public float budgetTotal;
        public List<string> specialRequirements = new List<string>();

        /// <summary>
        /// Create ClientData from an EventData for satisfaction calculation.
        /// </summary>
        public static ClientData FromEvent(EventData eventData) => new ClientData
        {
            clientId = eventData.clientId,
            clientName = eventData.clientName,
            personality = eventData.personality,
            guestCount = eventData.guestCount,
            budgetTotal = eventData.budget.total
        };
    }
}
