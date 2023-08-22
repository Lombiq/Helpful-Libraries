using System;
using System.Threading;
using Orchard.Environment.Extensions;
using Orchard.Logging;
using Piedone.HelpfulLibraries.DependencyInjection;

namespace Piedone.HelpfulLibraries.Tasks.Locking
{
    [OrchardFeature("Piedone.HelpfulLibraries.Tasks.Locking")]
    public class DistributedLockManager : IDistributedLockManager
    {
        private readonly IResolve<IDistributedLock> _lockResolve;

        public ILogger Logger { get; set; }


        public DistributedLockManager(IResolve<IDistributedLock> lockResolve)
        {
            _lockResolve = lockResolve;
            Logger = NullLogger.Instance;
        }


        public IDistributedLock TryAcquireLock(string name, TimeSpan timeout)
        {
            int waitedMilliseconds = 0;
            int millisecondsTimeout = (int)Math.Round(timeout.TotalMilliseconds);
            int waitMilliseconds = millisecondsTimeout / 10;
            var distributedLock = _lockResolve.Value;
            bool acquired;

            Logger.Debug("Trying to acquire lock \"{0}\" with the lock type {1}.", name, distributedLock.GetType().FullName);

            while (!(acquired = distributedLock.TryAcquire(name)) && waitedMilliseconds < millisecondsTimeout)
            {
                Thread.Sleep(waitMilliseconds);
                waitedMilliseconds += waitMilliseconds;
            }

            Logger.Debug("Lock \"{0}\" acquire state: {1}.", name, acquired);

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