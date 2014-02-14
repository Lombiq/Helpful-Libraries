using System;
using Orchard.Caching;
using Orchard.Environment.Extensions;

namespace Piedone.HelpfulLibraries.Tasks.Locking
{
    [OrchardFeature("Piedone.HelpfulLibraries.Tasks.Locking")]
    public class LockingCacheManager : ILockingCacheManager
    {
        private readonly ICacheManager _cacheManager;
        private readonly IDistributedLockManager _lockManager;


        public LockingCacheManager(
            ICacheManager cacheManager,
            IDistributedLockManager lockManager)
        {
            _cacheManager = cacheManager;
            _lockManager = lockManager;
        }


        public TResult Get<TResult>(string key, Func<AcquireContext<string>, TResult> acquire, Func<TResult> fallback, TimeSpan timeout)
        {
            // When using with arbitrary types, key.ToString() could lead to errors if the key is not string or the ToString() is not properly implemented.
            // That's why we only allow string keys here.

            try
            {
                return _cacheManager.Get(key, ctx =>
                    {
                        using (var lockFile = _lockManager.AcquireLock(key, timeout))
                        {
                            // If we waited for the lock to be released, here the result computed by the locking code should be returned.
                            return _cacheManager.Get(key, acquire);
                        }
                    });
            }
            catch (TimeoutException)
            {
                return fallback();
            }
        }
    }
}
