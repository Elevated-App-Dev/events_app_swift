using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EventPlannerSim.UI.Core;
using EventPlannerSim.Data;
using EventPlannerSim.Core;

namespace EventPlannerSim.UI.Phone.Apps
{
    /// <summary>
    /// Controls the Tasks app view.
    /// Displays tasks grouped by event with status and deadlines.
    /// </summary>
    public class TasksAppController : UIControllerBase, IPhoneAppController
    {
        #region Serialized Fields

        [Header("Content")]
        [SerializeField] private Transform tasksContainer;
        [SerializeField] private GameObject eventTaskGroupPrefab;
        [SerializeField] private TextMeshProUGUI noTasksText;

        [Header("Work Hours (Stage 2+)")]
        [SerializeField] private GameObject workHoursSection;
        [SerializeField] private TextMeshProUGUI workHoursText;
        [SerializeField] private Image workHoursBar;

        #endregion

        #region State

        private List<GameObject> _taskGroups = new List<GameObject>();

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

            UpdateWorkHoursDisplay();
            UpdateTasksList();
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
            if (gameObject.activeInHierarchy)
            {
                UpdateWorkHoursDisplay();
                UpdateTasksList();
            }
        }

        #endregion

        #region Work Hours

        private void UpdateWorkHoursDisplay()
        {
            var player = GameManager?.CurrentPlayer;
            if (player == null) return;

            // Only show work hours for Stage 2+
            bool showWorkHours = player.stage >= BusinessStage.Employee;

            if (workHoursSection != null)
            {
                workHoursSection.SetActive(showWorkHours);
            }

            if (showWorkHours)
            {
                var saveData = GameManager.CurrentSaveData;
                var workHours = saveData?.workHours;

                if (workHours != null && workHoursText != null)
                {
                    int remaining = workHours.RemainingHours;
                    int total = workHours.TotalAvailableHours;

                    workHoursText.text = $"{remaining}h remaining";
                    workHoursText.color = remaining <= 2
                        ? DesignTokens.Warning
                        : DesignTokens.TextPrimary;

                    if (workHoursBar != null)
                    {
                        workHoursBar.fillAmount = total > 0 ? (float)remaining / total : 0f;
                        workHoursBar.color = remaining <= 2
                            ? DesignTokens.Warning
                            : DesignTokens.Accent;
                    }
                }
            }
        }

        #endregion

        #region Tasks List

        private void UpdateTasksList()
        {
            if (tasksContainer == null) return;

            // Clear existing items
            foreach (var group in _taskGroups)
            {
                if (group != null) Destroy(group);
            }
            _taskGroups.Clear();

            // Get active events with tasks
            var eventsWithTasks = GetEventsWithTasks();

            if (eventsWithTasks.Count == 0)
            {
                if (noTasksText != null)
                {
                    noTasksText.gameObject.SetActive(true);
                    noTasksText.text = "No tasks";
                }
                return;
            }

            if (noTasksText != null) noTasksText.gameObject.SetActive(false);

            // Create task group for each event
            foreach (var evt in eventsWithTasks)
            {
                CreateEventTaskGroup(evt);
            }
        }

        private List<EventData> GetEventsWithTasks()
        {
            var events = new List<EventData>();
            var saveData = GameManager?.CurrentSaveData;

            if (saveData?.activeEvents != null)
            {
                foreach (var evt in saveData.activeEvents)
                {
                    // Only show events that are in planning/prep phase
                    if (evt.status == EventStatus.Accepted ||
                        evt.status == EventStatus.Planning)
                    {
                        events.Add(evt);
                    }
                }
            }

            // Sort by event date (soonest first)
            events.Sort((a, b) => a.eventDate.CompareTo(b.eventDate));

            return events;
        }

        private void CreateEventTaskGroup(EventData evt)
        {
            if (eventTaskGroupPrefab == null || tasksContainer == null) return;

            var groupObj = Instantiate(eventTaskGroupPrefab, tasksContainer);
            _taskGroups.Add(groupObj);

            var controller = groupObj.GetComponent<EventTaskGroupController>();
            if (controller != null)
            {
                controller.SetEvent(evt, GameManager.TimeSystem?.CurrentDate ?? new GameDate(1, 1, 1));
            }
        }

        #endregion
    }

    /// <summary>
    /// Controls a group of tasks for a single event.
    /// </summary>
    public class EventTaskGroupController : MonoBehaviour
    {
        [Header("Header")]
        [SerializeField] private TextMeshProUGUI eventNameText;
        [SerializeField] private TextMeshProUGUI eventDateText;
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private Image progressBar;

        [Header("Tasks")]
        [SerializeField] private Transform tasksContainer;
        [SerializeField] private GameObject taskItemPrefab;

        private List<GameObject> _taskItems = new List<GameObject>();

        public void SetEvent(EventData evt, GameDate currentDate)
        {
            // Update header
            if (eventNameText != null)
            {
                eventNameText.text = evt.eventTitle;
            }

            if (eventDateText != null)
            {
                int daysUntil = GameDate.DaysBetween(currentDate, evt.eventDate);
                eventDateText.text = daysUntil == 0
                    ? "Today"
                    : daysUntil == 1
                        ? "Tomorrow"
                        : $"{evt.eventDate.ToShortDisplayString()}";
            }

            // Calculate progress
            var tasks = evt.tasks ?? new List<EventTask>();
            int completed = tasks.Count(t => t.status == TaskStatus.Completed);
            int total = tasks.Count;

            if (progressText != null)
            {
                progressText.text = $"{completed} of {total}";
            }

            if (progressBar != null)
            {
                progressBar.fillAmount = total > 0 ? (float)completed / total : 0f;
                progressBar.color = DesignTokens.Accent;
            }

            // Create task items
            CreateTaskItems(tasks, currentDate);
        }

        private void CreateTaskItems(List<EventTask> tasks, GameDate currentDate)
        {
            if (tasksContainer == null || taskItemPrefab == null) return;

            // Clear existing
            foreach (var item in _taskItems)
            {
                if (item != null) Destroy(item);
            }
            _taskItems.Clear();

            // Sort: incomplete first, then by deadline
            var sortedTasks = tasks
                .OrderBy(t => t.status == TaskStatus.Completed)
                .ThenBy(t => t.deadline.TotalDays)
                .ToList();

            foreach (var task in sortedTasks)
            {
                var itemObj = Instantiate(taskItemPrefab, tasksContainer);
                _taskItems.Add(itemObj);

                var controller = itemObj.GetComponent<TaskItemController>();
                if (controller != null)
                {
                    controller.SetTask(task, currentDate);
                }
            }
        }
    }

    /// <summary>
    /// Controls a single task item display.
    /// </summary>
    public class TaskItemController : MonoBehaviour
    {
        [SerializeField] private GameObject completedIcon;
        [SerializeField] private GameObject pendingIcon;
        [SerializeField] private GameObject failedIcon;
        [SerializeField] private TextMeshProUGUI taskNameText;
        [SerializeField] private TextMeshProUGUI deadlineText;
        [SerializeField] private TextMeshProUGUI workHoursText;
        [SerializeField] private Button startButton;

        private EventTask _task;

        public void SetTask(EventTask task, GameDate currentDate)
        {
            _task = task;

            // Set icons based on status
            if (completedIcon != null) completedIcon.SetActive(task.status == TaskStatus.Completed);
            if (pendingIcon != null) pendingIcon.SetActive(task.status == TaskStatus.Pending || task.status == TaskStatus.InProgress);
            if (failedIcon != null) failedIcon.SetActive(task.status == TaskStatus.Failed);

            // Task name
            if (taskNameText != null)
            {
                taskNameText.text = task.taskName;
                taskNameText.color = task.status == TaskStatus.Completed
                    ? DesignTokens.TextMuted
                    : DesignTokens.TextPrimary;
            }

            // Deadline
            if (deadlineText != null && task.status != TaskStatus.Completed)
            {
                int daysUntil = GameDate.DaysBetween(currentDate, task.deadline);

                if (daysUntil < 0)
                {
                    deadlineText.text = "Overdue";
                    deadlineText.color = DesignTokens.Error;
                }
                else if (daysUntil == 0)
                {
                    deadlineText.text = "Due today";
                    deadlineText.color = DesignTokens.Warning;
                }
                else if (daysUntil == 1)
                {
                    deadlineText.text = "Due tomorrow";
                    deadlineText.color = DesignTokens.Warning;
                }
                else
                {
                    deadlineText.text = $"Due in {daysUntil} days";
                    deadlineText.color = DesignTokens.TextSecondary;
                }

                deadlineText.gameObject.SetActive(true);
            }
            else if (deadlineText != null)
            {
                deadlineText.gameObject.SetActive(false);
            }

            // Work hours (Stage 2+)
            if (workHoursText != null)
            {
                if (task.hoursRequired > 0 && task.status != TaskStatus.Completed)
                {
                    workHoursText.text = $"{task.hoursRequired}h";
                    workHoursText.gameObject.SetActive(true);
                }
                else
                {
                    workHoursText.gameObject.SetActive(false);
                }
            }

            // Start button
            if (startButton != null)
            {
                startButton.gameObject.SetActive(task.status == TaskStatus.Pending);
            }
        }
    }
}
