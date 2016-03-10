using System;
using Orchard;
using Orchard.Caching;

namespace Piedone.HelpfulLibraries.Tasks.Locking
{
    /// <summary>
    /// Provides locking cache management, i.e. concurrent cache access is synchronized so a cache
    /// entry is not computed twice.
    /// </summary>
    public interface ILockingCacheManager : IDependency
    {
        /// <summary>
        /// Tries to get the cache entry, if none is present, tries to acquire a lock on the entry so any other process 
        /// simultaneously requesting the entry has to wait.
        /// </summary>
        /// <typeparam name="TResult">Type of the computation's result</typeparam>
        /// <param name="key">Key for the cache entry</param>
        /// <param name="acquire">Delegate for acquiring the cache entry</param>
        /// <param name="fallback">If the lock couldn't be acquired and the process timed out, this delegate will be run</param>
        /// <param name="timeout">Time span to wait for the lock before timing out</param>
        /// <returns>Result of either the acquire or the fallback delegate</returns>
        TResult Get<TResult>(string key, Func<AcquireContext<string>, TResult> acquire, Func<TResult> fallback, TimeSpan timeout);
    }


    public static class LockingCacheManageExtensions
    {
        /// <summary>
        /// Tries to get the cache entry, if none is present, tries to acquire a lock on the entry so any other process 
        /// simultaneously requesting the entry has to wait.
        /// </summary>
        /// <typeparam name="TResult">Type of the computation's result</typeparam>
        /// <param name="key">Key for the cache entry</param>
        /// <param name="acquire">Delegate for acquiring the cache entry</param>
        /// <param name="fallback">If the lock couldn't be acquired and the process timed out, this delegate will be run</param>
        /// <returns>Result of either the acquire or the fallback delegate</returns>
        public static TResult Get<TResult>(this ILockingCacheManager lockingCacheManager, string key, Func<AcquireContext<string>, TResult> acquire, Func<TResult> fallback)
        {
            return lockingCacheManager.Get(key, acquire, fallback, new TimeSpan(0, 0, 4));
        }
    }
}
