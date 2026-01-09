using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EventPlannerSim.UI.Core;
using EventPlannerSim.Data;
using EventPlannerSim.Core;

namespace EventPlannerSim.UI.Phone.Apps
{
    /// <summary>
    /// Controls the Calendar app view.
    /// Displays month calendar and upcoming events list.
    /// </summary>
    public class CalendarAppController : UIControllerBase, IPhoneAppController
    {
        #region Serialized Fields

        [Header("Month Navigation")]
        [SerializeField] private Button prevMonthButton;
        [SerializeField] private Button nextMonthButton;
        [SerializeField] private TextMeshProUGUI monthYearText;

        [Header("Calendar Grid")]
        [SerializeField] private Transform calendarGridContainer;
        [SerializeField] private GameObject dayPrefab;

        [Header("Events List")]
        [SerializeField] private Transform eventsListContainer;
        [SerializeField] private GameObject eventItemPrefab;
        [SerializeField] private TextMeshProUGUI noEventsText;

        #endregion

        #region State

        private int _displayMonth;
        private int _displayYear;
        private List<CalendarDayController> _dayControllers = new List<CalendarDayController>();
        private List<GameObject> _eventItems = new List<GameObject>();

        #endregion

        #region Lifecycle

        protected override void Awake()
        {
            base.Awake();

            if (prevMonthButton != null) prevMonthButton.onClick.AddListener(OnPrevMonth);
            if (nextMonthButton != null) nextMonthButton.onClick.AddListener(OnNextMonth);
        }

        #endregion

        #region UIControllerBase Implementation

        protected override void SubscribeToEvents()
        {
            // Note: Event subscriptions will be added when TimeSystem exposes events
        }

        protected override void UnsubscribeFromEvents()
        {
            // Note: Event unsubscriptions will be added when TimeSystem exposes events
        }

        protected override void RefreshDisplay()
        {
            if (!HasValidGameState()) return;

            // Initialize to current month
            var currentDate = GameManager.TimeSystem?.CurrentDate ?? new GameDate(1, 1, 1);
            _displayMonth = currentDate.month;
            _displayYear = currentDate.year;

            UpdateCalendarDisplay();
            UpdateEventsList();
        }

        #endregion

        #region IPhoneAppController

        public void Refresh()
        {
            RefreshDisplay();
        }

        #endregion

        #region Event Handlers

        private void HandleDateChanged(GameDate newDate)
        {
            // Refresh if viewing current month
            if (_displayMonth == newDate.month && _displayYear == newDate.year)
            {
                UpdateCalendarDisplay();
            }
            UpdateEventsList();
        }

        #endregion

        #region Calendar Display

        private void UpdateCalendarDisplay()
        {
            // Update month/year header
            if (monthYearText != null)
            {
                string monthName = GetMonthName(_displayMonth);
                monthYearText.text = _displayYear > 1
                    ? $"{monthName} Y{_displayYear}"
                    : monthName;
            }

            // Update calendar grid
            UpdateCalendarGrid();
        }

        private void UpdateCalendarGrid()
        {
            if (calendarGridContainer == null || dayPrefab == null) return;

            var currentDate = GameManager?.TimeSystem?.CurrentDate ?? new GameDate(1, 1, 1);
            var eventDates = GetEventDatesInMonth(_displayMonth, _displayYear);

            // Create day cells if needed
            while (_dayControllers.Count < 30)
            {
                var dayObj = Instantiate(dayPrefab, calendarGridContainer);
                var controller = dayObj.GetComponent<CalendarDayController>();
                if (controller != null)
                {
                    _dayControllers.Add(controller);
                }
            }

            // Update each day
            for (int i = 0; i < 30; i++)
            {
                if (i < _dayControllers.Count)
                {
                    int day = i + 1;
                    var date = new GameDate(day, _displayMonth, _displayYear);

                    bool isToday = date == currentDate;
                    bool hasEvent = eventDates.Contains(day);

                    _dayControllers[i].SetDay(day, isToday, hasEvent);
                    _dayControllers[i].gameObject.SetActive(true);
                }
            }
        }

        private HashSet<int> GetEventDatesInMonth(int month, int year)
        {
            var dates = new HashSet<int>();

            // Get events from save data
            var saveData = GameManager?.CurrentSaveData;
            if (saveData?.activeEvents != null)
            {
                foreach (var evt in saveData.activeEvents)
                {
                    if (evt.eventDate.month == month && evt.eventDate.year == year)
                    {
                        dates.Add(evt.eventDate.day);
                    }
                }
            }

            return dates;
        }

        #endregion

        #region Events List

        private void UpdateEventsList()
        {
            if (eventsListContainer == null) return;

            // Clear existing items
            foreach (var item in _eventItems)
            {
                if (item != null) Destroy(item);
            }
            _eventItems.Clear();

            // Get upcoming events
            var upcomingEvents = GetUpcomingEvents();

            if (upcomingEvents.Count == 0)
            {
                if (noEventsText != null)
                {
                    noEventsText.gameObject.SetActive(true);
                    noEventsText.text = "No upcoming events";
                }
                return;
            }

            if (noEventsText != null) noEventsText.gameObject.SetActive(false);

            // Create event items
            foreach (var evt in upcomingEvents)
            {
                CreateEventItem(evt);
            }
        }

        private List<EventData> GetUpcomingEvents()
        {
            var events = new List<EventData>();
            var currentDate = GameManager?.TimeSystem?.CurrentDate ?? new GameDate(1, 1, 1);

            var saveData = GameManager?.CurrentSaveData;
            if (saveData?.activeEvents != null)
            {
                foreach (var evt in saveData.activeEvents)
                {
                    if (evt.eventDate >= currentDate)
                    {
                        events.Add(evt);
                    }
                }
            }

            // Sort by date
            events.Sort((a, b) => a.eventDate.CompareTo(b.eventDate));

            return events;
        }

        private void CreateEventItem(EventData evt)
        {
            if (eventItemPrefab == null || eventsListContainer == null) return;

            var itemObj = Instantiate(eventItemPrefab, eventsListContainer);
            _eventItems.Add(itemObj);

            var controller = itemObj.GetComponent<CalendarEventItemController>();
            if (controller != null)
            {
                controller.SetEvent(evt);
            }
        }

        #endregion

        #region Navigation

        private void OnPrevMonth()
        {
            _displayMonth--;
            if (_displayMonth < 1)
            {
                _displayMonth = 12;
                _displayYear--;
            }
            UpdateCalendarDisplay();
        }

        private void OnNextMonth()
        {
            _displayMonth++;
            if (_displayMonth > 12)
            {
                _displayMonth = 1;
                _displayYear++;
            }
            UpdateCalendarDisplay();
        }

        #endregion

        #region Utility

        private string GetMonthName(int month)
        {
            return month switch
            {
                1 => "January",
                2 => "February",
                3 => "March",
                4 => "April",
                5 => "May",
                6 => "June",
                7 => "July",
                8 => "August",
                9 => "September",
                10 => "October",
                11 => "November",
                12 => "December",
                _ => $"Month {month}"
            };
        }

        #endregion
    }

    /// <summary>
    /// Controls a single day cell in the calendar grid.
    /// </summary>
    public class CalendarDayController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI dayText;
        [SerializeField] private GameObject todayIndicator;
        [SerializeField] private GameObject eventDot;
        [SerializeField] private Image background;

        public void SetDay(int day, bool isToday, bool hasEvent)
        {
            if (dayText != null)
            {
                dayText.text = day.ToString();
                dayText.color = isToday ? DesignTokens.TextPrimary : DesignTokens.TextSecondary;
            }

            if (todayIndicator != null)
            {
                todayIndicator.SetActive(isToday);
            }

            if (eventDot != null)
            {
                eventDot.SetActive(hasEvent);
            }

            if (background != null)
            {
                background.color = isToday ? DesignTokens.Accent : DesignTokens.Surface;
            }
        }
    }

    /// <summary>
    /// Controls a single event item in the calendar events list.
    /// </summary>
    public class CalendarEventItemController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI eventNameText;
        [SerializeField] private TextMeshProUGUI eventDateText;
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private Image statusIndicator;

        public void SetEvent(EventData evt)
        {
            if (eventNameText != null)
            {
                eventNameText.text = evt.eventTitle;
            }

            if (eventDateText != null)
            {
                eventDateText.text = evt.eventDate.ToShortDisplayString();
            }

            if (statusText != null)
            {
                statusText.text = evt.status.ToString();
            }

            if (statusIndicator != null)
            {
                statusIndicator.color = evt.status switch
                {
                    EventStatus.Accepted => DesignTokens.Warning,
                    EventStatus.Planning => DesignTokens.Accent,
                    EventStatus.Executing => DesignTokens.Success,
                    _ => DesignTokens.TextMuted
                };
            }
        }
    }
}
