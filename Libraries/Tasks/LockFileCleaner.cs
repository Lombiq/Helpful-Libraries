using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.Environment;
using Orchard.FileSystems.Media;
using Orchard.Services;
using Orchard.Tasks.Scheduling;
using Orchard.Exceptions;

namespace Piedone.HelpfulLibraries.Tasks
{
    public class LockFileCleaner : IScheduledTaskHandler, IOrchardShellEvents
    {
        private readonly IScheduledTaskManager _scheduledTaskManager;
        private readonly IClock _clock;
        private readonly IStorageProvider _storageProvider;

        private const string TaskType = "Piedone.HelpfulLibraries.LockFileCleaner";


        public LockFileCleaner(
            IScheduledTaskManager scheduledTaskManager,
            IClock clock,
            IStorageProvider storageProvider)
        {
            _scheduledTaskManager = scheduledTaskManager;
            _clock = clock;
            _storageProvider = storageProvider;
        }


        public void Process(ScheduledTaskContext context)
        {
            if (context.Task.TaskType != TaskType) return;

            Renew(true);
            CleanUp();
        }

        public void Activated()
        {
            Renew(false);
            CleanUp();
        }

        public void Terminating()
        {
        }


        private void CleanUp()
        {
            if (!_storageProvider.FolderExists(WellKnownConstants.LockFileFolder)) return;

            foreach (var file in _storageProvider.ListFiles(WellKnownConstants.LockFileFolder))
            {
                // Removing lock files that weren't touched in two minutes.
                if (_clock.UtcNow.Subtract(file.GetLastUpdated().ToUniversalTime()).Minutes > 2)
                    _storageProvider.DeleteFile(file.GetPath());
            }
        }

        private void Renew(bool calledFromTaskProcess)
        {
            _scheduledTaskManager.CreateTaskIfNew(TaskType, _clock.UtcNow.AddMinutes(1), null, calledFromTaskProcess);
        }
    }
}
