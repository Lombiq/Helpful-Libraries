using System;
using System.Threading;
using Orchard.Environment.Extensions;
using Piedone.HelpfulLibraries.DependencyInjection;

namespace Piedone.HelpfulLibraries.Tasks
{
    [OrchardFeature("Piedone.HelpfulLibraries.Tasks")]
    public class LockFileManager : ILockFileManager
    {
        private readonly IResolve<ILockFile> _lockFileResolve;

        public LockFileManager(IResolve<ILockFile> lockFileResolve)
        {
            _lockFileResolve = lockFileResolve;
        }

        public ILockFile TryAcquireLock(string name, TimeSpan timeout)
        {
            int waitedMilliseconds = 0;
            int millisecondsTimeout = (int)Math.Round(timeout.TotalMilliseconds);
            int waitMilliseconds = millisecondsTimeout / 10;
            var lockFile = _lockFileResolve.Value;
            bool acquired;

            while (!(acquired = lockFile.TryAcquire(name)) && waitedMilliseconds < millisecondsTimeout)
            {
                // Change to Task.Delay and progressive sleeping after .NET 4.5 update
                Thread.Sleep(waitMilliseconds);
                waitedMilliseconds += waitMilliseconds;
            }

            if (acquired) return lockFile;
            else return null;
        }

        public ILockFile AcquireLock(string name, TimeSpan timeout)
        {
            var lockResult = TryAcquireLock(name, timeout);
            if (lockResult != null) return lockResult;
            throw new TimeoutException("The lock on the file \"" + name + "\" couldn't be acquired in " + timeout.TotalMilliseconds.ToString() + " ms.");
        }
    }
}