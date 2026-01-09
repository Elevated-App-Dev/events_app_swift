using System;
using System.Collections.Generic;
using EventPlannerSim.Core;

namespace EventPlannerSim.Data
{
    /// <summary>
    /// Represents a preparation task for an event.
    /// Requirements: R10.1-R10.6
    /// </summary>
    [Serializable]
    public class EventTask
    {
        /// <summary>
        /// Unique identifier for this task.
        /// </summary>
        public string id;

        /// <summary>
        /// Display name of the task.
        /// </summary>
        public string taskName;

        /// <summary>
        /// Detailed description of what needs to be done.
        /// </summary>
        public string description;

        /// <summary>
        /// Current status of the task.
        /// </summary>
        public TaskStatus status = TaskStatus.Pending;

        /// <summary>
        /// Deadline for completing this task.
        /// </summary>
        public GameDate deadline;

        /// <summary>
        /// Work hours required to complete this task.
        /// Note: Only used in Stage 2+ (Stage 1 uses simplified deadline-only system).
        /// </summary>
        public int hoursRequired;

        /// <summary>
        /// IDs of tasks that must be completed before this one can start.
        /// </summary>
        public List<string> dependencyTaskIds = new List<string>();

        /// <summary>
        /// Description of what happens if this task fails.
        /// </summary>
        public string failureConsequence;

        /// <summary>
        /// Whether company help was used to complete this task (Stage 2 only).
        /// Using company help reduces event profit by 10%.
        /// </summary>
        public bool usedCompanyHelp = false;

        /// <summary>
        /// Number of company help requests used on this event.
        /// Limited to 2 per event.
        /// </summary>
        public int companyHelpCount = 0;

        /// <summary>
        /// Maximum company help requests allowed per event.
        /// </summary>
        public const int MaxCompanyHelpPerEvent = 2;

        /// <summary>
        /// Checks if this task can be started based on dependencies.
        /// </summary>
        public bool CanStart(List<EventTask> allTasks)
        {
            if (status != TaskStatus.Pending)
                return false;

            foreach (var depId in dependencyTaskIds)
            {
                var depTask = allTasks.Find(t => t.id == depId);
                if (depTask == null || depTask.status != TaskStatus.Completed)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if this task is overdue based on the current date.
        /// </summary>
        public bool IsOverdue(GameDate currentDate)
        {
            return currentDate > deadline && status != TaskStatus.Completed;
        }

        /// <summary>
        /// Marks the task as failed if deadline has passed.
        /// </summary>
        public void CheckDeadline(GameDate currentDate)
        {
            if (IsOverdue(currentDate) && status == TaskStatus.Pending)
            {
                status = TaskStatus.Failed;
            }
        }
    }

    /// <summary>
    /// Represents the status of an event task.
    /// </summary>
    public enum TaskStatus
    {
        /// <summary>
        /// Task cannot be started yet (dependencies not met).
        /// </summary>
        Locked,

        /// <summary>
        /// Task is available to be started.
        /// </summary>
        Pending,

        /// <summary>
        /// Task is currently being worked on.
        /// </summary>
        InProgress,

        /// <summary>
        /// Task has been successfully completed.
        /// </summary>
        Completed,

        /// <summary>
        /// Task deadline passed without completion.
        /// </summary>
        Failed
    }
}
