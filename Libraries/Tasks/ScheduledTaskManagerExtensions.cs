using Orchard.ContentManagement;
using Orchard.Tasks.Scheduling;
using System;
using System.Linq;

namespace Piedone.HelpfulLibraries.Tasks
{
    public static class ScheduledTaskManagerExtensions
    {
        /// <summary>
        /// Creates a new scheduled task entry if one with the specified type is not already preset.
        /// </summary>
        /// <param name="taskType">Type of the task.</param>
        /// <param name="scheduledUtc">Date when the task is scheduled to run.</param>
        /// <param name="contentItem">Corresponding content item, if any.</param>
        /// <param name="calledFromTaskProcess">
        /// Set to true if this method is called from a task's Process() for renewal of itself.
        /// </param>
        public static void CreateTaskIfNew(
            this IScheduledTaskManager taskManager,
            string taskType,
            DateTime scheduledUtc,
            ContentItem contentItem,
            bool calledFromTaskProcess)
        {
            var outdatedTaskCount = taskManager.GetTasks(taskType, DateTime.UtcNow).Count();
            var taskCount = taskManager.GetTasks(taskType).Count();

            if ((!calledFromTaskProcess || taskCount != 1) && taskCount != 0 && taskCount - outdatedTaskCount >= 0) return;

            taskManager.CreateTask(taskType, scheduledUtc, contentItem);
        }

        /// <summary>
        /// Removes existing tasks of a given type (if any) and registers it again with the given parameters.
        /// </summary>
        /// <param name="taskType">Type of the task.</param>
        /// <param name="scheduledUtc">Date when the task is scheduled to run.</param>
        /// <param name="contentItem">Corresponding content item, if any.</param>
        public static void RescheduleTaskOrCreate(
            this IScheduledTaskManager taskManager,
            string taskType,
            DateTime scheduledUtc,
            ContentItem contentItem)
        {
            taskManager.DeleteTasks(contentItem, task => task.TaskType == taskType);
            taskManager.CreateTask(taskType, scheduledUtc, contentItem);
        }
    }
}
