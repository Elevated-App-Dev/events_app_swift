using System.Collections.Generic;
using EventPlannerSim.Core;
using EventPlannerSim.Data;

namespace EventPlannerSim.Systems
{
    /// <summary>
    /// Result of a booking operation (vendor or venue).
    /// </summary>
    public class BookingResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public float AmountDeducted { get; set; }
        public bool ShowWarning { get; set; }
        public string WarningMessage { get; set; }

        public static BookingResult Successful(float amount, string message = null)
        {
            return new BookingResult
            {
                Success = true,
                AmountDeducted = amount,
                Message = message ?? "Booking successful"
            };
        }

        public static BookingResult SuccessfulWithWarning(float amount, string warning)
        {
            return new BookingResult
            {
                Success = true,
                AmountDeducted = amount,
                ShowWarning = true,
                WarningMessage = warning,
                Message = "Booking successful with warning"
            };
        }

        public static BookingResult Failed(string message)
        {
            return new BookingResult
            {
                Success = false,
                Message = message
            };
        }
    }

    /// <summary>
    /// Manages event lifecycle from inquiry to results.
    /// Requirements: R5.1-R5.8, R6.18, R8.1-R8.7, R9.1-R9.7
    /// </summary>
    public interface IEventPlanningSystem
    {
        /// <summary>
        /// Generate a new client inquiry based on stage and reputation.
        /// Requirements: R5.1-R5.8
        /// </summary>
        /// <param name="stage">Current business stage (1-5)</param>
        /// <param name="reputation">Current player reputation</param>
        /// <param name="currentDate">Current in-game date for scheduling</param>
        /// <returns>A new client inquiry with all required fields populated</returns>
        ClientInquiry GenerateInquiry(int stage, int reputation, GameDate currentDate);

        /// <summary>
        /// Accept an inquiry and create an active event.
        /// Requirements: R5.3
        /// </summary>
        /// <param name="inquiry">The inquiry to accept</param>
        /// <param name="currentDate">Current in-game date</param>
        /// <returns>The created EventData with status set to Accepted</returns>
        EventData AcceptInquiry(ClientInquiry inquiry, GameDate currentDate);

        /// <summary>
        /// Decline an inquiry with no penalty.
        /// Requirements: R5.4
        /// </summary>
        /// <param name="inquiry">The inquiry to decline</param>
        void DeclineInquiry(ClientInquiry inquiry);

        /// <summary>
        /// Get current workload status based on active event count.
        /// Requirements: R5.9-R5.18
        /// </summary>
        /// <param name="stage">Current business stage</param>
        /// <param name="activeEventCount">Number of active events</param>
        /// <returns>Current workload status</returns>
        WorkloadStatus GetWorkloadStatus(int stage, int activeEventCount);

        /// <summary>
        /// Calculate satisfaction penalty based on workload.
        /// Requirements: R5.12-R5.15
        /// </summary>
        /// <param name="stage">Current business stage</param>
        /// <param name="activeEventCount">Number of active events</param>
        /// <returns>Satisfaction penalty percentage (0-100)</returns>
        float CalculateWorkloadPenalty(int stage, int activeEventCount);

        /// <summary>
        /// Calculate task failure probability increase based on workload.
        /// Requirements: R5.14-R5.15
        /// </summary>
        /// <param name="stage">Current business stage</param>
        /// <param name="activeEventCount">Number of active events</param>
        /// <returns>Task failure probability increase percentage</returns>
        float CalculateTaskFailureProbabilityIncrease(int stage, int activeEventCount);

        /// <summary>
        /// Calculate overlapping event preparation penalty.
        /// Requirements: R5.17
        /// </summary>
        /// <param name="overlappingEventCount">Number of events with overlapping preparation periods</param>
        /// <returns>Task failure probability increase (5% per overlapping event)</returns>
        float CalculateOverlappingPrepPenalty(int overlappingEventCount);

        /// <summary>
        /// Book a vendor for an event, deducting from budget.
        /// Requirements: R8.1-R8.7
        /// </summary>
        /// <param name="eventData">The event to book the vendor for</param>
        /// <param name="vendor">The vendor to book</param>
        /// <param name="bookedDates">List of dates the vendor is already booked</param>
        /// <returns>Result of the booking operation</returns>
        BookingResult BookVendor(EventData eventData, VendorData vendor, List<GameDate> bookedDates);

        /// <summary>
        /// Book a venue for an event, validating capacity.
        /// Requirements: R9.1-R9.7
        /// </summary>
        /// <param name="eventData">The event to book the venue for</param>
        /// <param name="venue">The venue to book</param>
        /// <param name="bookedDates">List of dates the venue is already booked</param>
        /// <returns>Result of the booking operation</returns>
        BookingResult BookVenue(EventData eventData, VenueData venue, List<GameDate> bookedDates);

        /// <summary>
        /// Generate event title in format "[ClientName]'s [SubCategory]".
        /// Requirements: R6.18
        /// </summary>
        /// <param name="clientName">Name of the client</param>
        /// <param name="subCategory">Sub-category of the event</param>
        /// <returns>Formatted event title</returns>
        string GenerateEventTitle(string clientName, string subCategory);

        /// <summary>
        /// Get the inquiry interval range in minutes for a given stage.
        /// Requirements: R5.6
        /// </summary>
        /// <param name="stage">Current business stage</param>
        /// <returns>Tuple of (minMinutes, maxMinutes)</returns>
        (float min, float max) GetInquiryIntervalRange(int stage);

        /// <summary>
        /// Calculate adjusted inquiry interval based on reputation.
        /// Requirements: R5.7
        /// </summary>
        /// <param name="stage">Current business stage</param>
        /// <param name="reputation">Current player reputation</param>
        /// <returns>Adjusted interval in minutes</returns>
        float CalculateAdjustedInquiryInterval(int stage, int reputation);
    }
}
