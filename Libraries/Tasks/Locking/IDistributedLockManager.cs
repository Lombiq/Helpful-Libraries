using System;
using Orchard.Caching;

namespace Piedone.HelpfulLibraries.Tasks.Locking
{
    /// <summary>
    /// Locking services bases on the usage of distributed locks that will work in any server environment
    /// and independent from the lifecycle of database transactions.
    /// </summary>
    public interface IDistributedLockManager : IVolatileProvider
    {
        /// <summary>
        /// Tries to acquire a lock with the specified parameters
        /// </summary>
        /// <param name="name">Unique name of the lock</param>
        /// <param name="timeout">Time span to wait for the lock before timing out</param>
        /// <returns>The ILockFile instance on success or null if the lock couldn't be acquired.</returns>
        IDistributedLock TryAcquireLock(string name, TimeSpan timeout);

        /// <summary>
        /// Tries to acquire a lock with the specified parameters
        /// </summary>
        /// <param name="name">Unique name of the lock</param>
        /// <param name="timeout">Time span to wait for the lock before timing out</param>
        /// <returns>The ILockFile instance on success.</returns>
        /// <exception cref="System.TimeoutException">Thrown if the lock couldn't be acquired.</exception>
        IDistributedLock AcquireLock(string name, TimeSpan timeout);
    }


    public static class DistributedLockManagerExtensions
    {
        /// <summary>
        /// Tries to acquire a lock with the specified parameters
        /// </summary>
        /// <param name="name">Unique name of the lock</param>
        /// <returns>The ILockFile instance on success or null if the lock couldn't be acquired.</returns>
        public static IDistributedLock TryAcquireLock(this IDistributedLockManager lockFileManager, string name)
        {
            return lockFileManager.TryAcquireLock(name, new TimeSpan(0, 0, 0, 4));
        }

        /// <summary>
        /// Tries to acquire a lock with the specified parameters
        /// </summary>
        /// <param name="name">Unique name of the lock</param>
        /// <param name="timeout">Time span to wait for the lock before timing out</param>
        /// <param name="distributedLock">The ILockFile instance on success or null if the lock couldn't be acquired.</param>
        /// <returns>True if the lock could be acquired and false if not.</returns>
        public static bool TryAcquireLock(this IDistributedLockManager lockFileManager, string name, TimeSpan timeout, out IDistributedLock distributedLock)
        {
            distributedLock = lockFileManager.TryAcquireLock(name, timeout);
            return distributedLock != null;
        }

        /// <summary>
        /// Tries to acquire a lock with the specified parameters
        /// </summary>
        /// <param name="name">Unique name of the lock</param>
        /// <returns>The ILockFile instance on success.</returns>
        /// <exception cref="System.TimeoutException">Thrown if the lock couldn't be acquired.</exception>
        public static IDistributedLock AcquireLock(this IDistributedLockManager lockFileManager, string name)
        {
            return lockFileManager.AcquireLock(name, new TimeSpan(0, 0, 0, 4));
        }
    }
}
