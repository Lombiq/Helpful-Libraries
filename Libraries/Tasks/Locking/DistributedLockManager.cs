using System;
using System.Threading;
using Orchard.Environment.Extensions;
using Piedone.HelpfulLibraries.DependencyInjection;

namespace Piedone.HelpfulLibraries.Tasks.Locking
{
    [OrchardFeature("Piedone.HelpfulLibraries.Tasks.Locking")]
    public class DistributedLockManager : IDistributedLockManager
    {
        private readonly IResolve<IDistributedLock> _lockResolve;


        public DistributedLockManager(IResolve<IDistributedLock> lockResolve)
        {
            _lockResolve = lockResolve;
        }


        public IDistributedLock TryAcquireLock(string name, TimeSpan timeout)
        {
            int waitedMilliseconds = 0;
            int millisecondsTimeout = (int)Math.Round(timeout.TotalMilliseconds);
            int waitMilliseconds = millisecondsTimeout / 10;
            var distributedLock = _lockResolve.Value;
            bool acquired;

            while (!(acquired = distributedLock.TryAcquire(name)) && waitedMilliseconds < millisecondsTimeout)
            {
                Thread.Sleep(waitMilliseconds);
                waitedMilliseconds += waitMilliseconds;
            }

            if (acquired) return distributedLock;
            else return null;
        }

        public IDistributedLock AcquireLock(string name, TimeSpan timeout)
        {
            var lockResult = TryAcquireLock(name, timeout);
            if (lockResult != null) return lockResult;
            throw new TimeoutException("The lock \"" + name + "\" couldn't be acquired in " + timeout.TotalMilliseconds.ToString() + " ms.");
        }
    }
}