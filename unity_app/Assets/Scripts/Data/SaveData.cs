using System;
using System.Collections.Generic;
using EventPlannerSim.Core;

namespace EventPlannerSim.Data
{
    /// <summary>
    /// Serializable booking entry for vendors/venues.
    /// Unity's JsonUtility doesn't serialize Dictionary, so we use List instead.
    /// Requirements: R27.1-R27.5
    /// </summary>
    [Serializable]
    public class BookingEntry
    {
        /// <summary>
        /// The vendor or venue ID.
        /// </summary>
        public string entityId;

        /// <summary>
        /// List of dates this entity is booked.
        /// </summary>
        public List<GameDate> bookedDates = new List<GameDate>();

        public BookingEntry() { }

        public BookingEntry(string id)
        {
            entityId = id;
        }

        /// <summary>
        /// Checks if the entity is booked on a specific date.
        /// </summary>
        public bool IsBookedOn(GameDate date)
        {
            return bookedDates.Contains(date);
        }

        /// <summary>
        /// Adds a booking for a specific date if not already booked.
        /// </summary>
        public bool AddBooking(GameDate date)
        {
            if (bookedDates.Contains(date))
                return false;

            bookedDates.Add(date);
            return true;
        }

        /// <summary>
        /// Removes a booking for a specific date.
        /// </summary>
        public bool RemoveBooking(GameDate date)
        {
            return bookedDates.Remove(date);
        }
    }


    /// <summary>
    /// Complete save data structure for game persistence.
    /// Requirements: R27.1-R27.5, R2.2-R2.6
    /// </summary>
    [Serializable]
    public class SaveData
    {
        /// <summary>
        /// Version number for save file migration support.
        /// </summary>
        public string saveVersion = "1.0";

        /// <summary>
        /// Timestamp of last save (stored as ticks for serialization).
        /// </summary>
        public long lastSavedTimestamp;

        /// <summary>
        /// Player progression and state data.
        /// </summary>
        public PlayerData playerData = new PlayerData();

        /// <summary>
        /// Currently active events being planned or executed.
        /// </summary>
        public List<EventData> activeEvents = new List<EventData>();

        /// <summary>
        /// History of completed events.
        /// </summary>
        public List<EventData> eventHistory = new List<EventData>();

        /// <summary>
        /// Player's daily work hours tracking.
        /// </summary>
        public WorkHoursData workHours = new WorkHoursData();

        /// <summary>
        /// Weather system state.
        /// </summary>
        public WeatherSystemData weather = new WeatherSystemData();

        /// <summary>
        /// Current in-game date.
        /// </summary>
        public GameDate currentDate = new GameDate(1, 1, 1);

        /// <summary>
        /// Vendor booking calendar (serializable list format).
        /// </summary>
        public List<BookingEntry> vendorBookings = new List<BookingEntry>();

        /// <summary>
        /// Venue booking calendar (serializable list format).
        /// </summary>
        public List<BookingEntry> venueBookings = new List<BookingEntry>();

        /// <summary>
        /// Game settings (audio, notifications, privacy, accessibility).
        /// </summary>
        public GameSettings settings = new GameSettings();

        /// <summary>
        /// Consecutive events with 90%+ satisfaction for referral bonuses.
        /// </summary>
        public int excellenceStreak = 0;

        /// <summary>
        /// Pending client inquiries.
        /// </summary>
        public List<ClientInquiry> pendingInquiries = new List<ClientInquiry>();

        /// <summary>
        /// Stage 3 milestone progress tracking.
        /// </summary>
        public MilestoneProgress milestoneProgress = new MilestoneProgress();

        /// <summary>
        /// Gets or sets the last saved time as DateTime.
        /// </summary>
        public DateTime LastSavedTime
        {
            get => new DateTime(lastSavedTimestamp);
            set => lastSavedTimestamp = value.Ticks;
        }

        /// <summary>
        /// Helper to find booking dates for a vendor.
        /// </summary>
        public List<GameDate> GetVendorBookedDates(string vendorId)
        {
            var entry = vendorBookings.Find(b => b.entityId == vendorId);
            return entry?.bookedDates ?? new List<GameDate>();
        }

        /// <summary>
        /// Helper to find booking dates for a venue.
        /// </summary>
        public List<GameDate> GetVenueBookedDates(string venueId)
        {
            var entry = venueBookings.Find(b => b.entityId == venueId);
            return entry?.bookedDates ?? new List<GameDate>();
        }

        /// <summary>
        /// Helper to add a vendor booking.
        /// </summary>
        public void AddVendorBooking(string vendorId, GameDate date)
        {
            var entry = vendorBookings.Find(b => b.entityId == vendorId);
            if (entry == null)
            {
                entry = new BookingEntry(vendorId);
                vendorBookings.Add(entry);
            }
            entry.AddBooking(date);
        }

        /// <summary>
        /// Helper to add a venue booking.
        /// </summary>
        public void AddVenueBooking(string venueId, GameDate date)
        {
            var entry = venueBookings.Find(b => b.entityId == venueId);
            if (entry == null)
            {
                entry = new BookingEntry(venueId);
                venueBookings.Add(entry);
            }
            entry.AddBooking(date);
        }

        /// <summary>
        /// Helper to check if a vendor is available on a date.
        /// </summary>
        public bool IsVendorAvailable(string vendorId, GameDate date)
        {
            var entry = vendorBookings.Find(b => b.entityId == vendorId);
            return entry == null || !entry.IsBookedOn(date);
        }

        /// <summary>
        /// Helper to check if a venue is available on a date.
        /// </summary>
        public bool IsVenueAvailable(string venueId, GameDate date)
        {
            var entry = venueBookings.Find(b => b.entityId == venueId);
            return entry == null || !entry.IsBookedOn(date);
        }

        /// <summary>
        /// Helper to remove a vendor booking.
        /// </summary>
        public bool RemoveVendorBooking(string vendorId, GameDate date)
        {
            var entry = vendorBookings.Find(b => b.entityId == vendorId);
            return entry?.RemoveBooking(date) ?? false;
        }

        /// <summary>
        /// Helper to remove a venue booking.
        /// </summary>
        public bool RemoveVenueBooking(string venueId, GameDate date)
        {
            var entry = venueBookings.Find(b => b.entityId == venueId);
            return entry?.RemoveBooking(date) ?? false;
        }

        /// <summary>
        /// Creates a deep copy of the save data for backup purposes.
        /// </summary>
        public SaveData Clone()
        {
            // Use JSON serialization for deep copy
            string json = UnityEngine.JsonUtility.ToJson(this);
            return UnityEngine.JsonUtility.FromJson<SaveData>(json);
        }
    }

    /// <summary>
    /// Stage 3 milestone progress tracking.
    /// </summary>
    [Serializable]
    public class MilestoneProgress
    {
        public bool hasSeenStage3Milestone = false;
        public bool hasChosenPath = false;
        public CareerPath chosenPath = CareerPath.None;
        public bool canSkipMilestoneSequence = false;
    }

    /// <summary>
    /// Career summary data for Stage 3 milestone display.
    /// </summary>
    [Serializable]
    public class CareerSummaryData
    {
        public int totalEventsCompleted;
        public string firstEventName;
        public string highestSatisfactionEventName;
        public float highestSatisfactionScore;
        public float totalMoneyEarned;
        public int currentReputation;
        public long journeyStartDateTicks;
        public long stage3ReachedDateTicks;

        public DateTime JourneyStartDate
        {
            get => new DateTime(journeyStartDateTicks);
            set => journeyStartDateTicks = value.Ticks;
        }

        public DateTime Stage3ReachedDate
        {
            get => new DateTime(stage3ReachedDateTicks);
            set => stage3ReachedDateTicks = value.Ticks;
        }
    }
}
