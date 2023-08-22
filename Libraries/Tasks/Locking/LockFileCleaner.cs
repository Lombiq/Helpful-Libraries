using Orchard.Environment;
using Orchard.Environment.Extensions;
using Orchard.FileSystems.Media;
using Orchard.Services;

namespace Piedone.HelpfulLibraries.Tasks.Locking
{
    [OrchardFeature("Piedone.HelpfulLibraries.Tasks.Locking.File")]
    public class LockFileCleaner : IOrchardShellEvents
    {
        private readonly IClock _clock;
        private readonly IStorageProvider _storageProvider;


        public LockFileCleaner(
            IClock clock,
            IStorageProvider storageProvider)
        {
            _clock = clock;
            _storageProvider = storageProvider;
        }


        public void Activated()
        {
            if (!_storageProvider.FolderExists(WellKnownConstants.LockFileFolder)) return;

            // If there are lock files on shell start older than one minute they were surely created before the shell startup,
            // thus are remainders of an earlier crash.
            foreach (var file in _storageProvider.ListFiles(WellKnownConstants.LockFileFolder))
            {
                // Removing lock files that weren't touched in one minute.
                if (_clock.UtcNow.Subtract(file.GetLastUpdated().ToUniversalTime()).Minutes > 1)
                    _storageProvider.DeleteFile(file.GetPath());
            }
        }

        public void Terminating()
        {
        }
    }
}
