using System;
using Orchard.Caching;
using Orchard.Environment.Extensions;

namespace Piedone.HelpfulLibraries.Tasks
{
    [OrchardFeature("Piedone.HelpfulLibraries.Tasks")]
    public class LockingCacheManager : ILockingCacheManager
    {
        private readonly ICacheManager _cacheManager;
        private readonly ILockFileManager _lockFileManager;

        public LockingCacheManager(
            ICacheManager cacheManager,
            ILockFileManager lockFileManager)
        {
            _cacheManager = cacheManager;
            _lockFileManager = lockFileManager;
        }

        public TResult Get<TResult>(string key, Func<AcquireContext<string>, TResult> acquire, Func<TResult> fallback, int millisecondsTimeout = 4000)
        {
            // When using with arbitrary types, key.ToString() could lead to errors if the key is not string or the ToString() is not properly implemented.
            // That's why we only allow string keys here.

            return _cacheManager.Get(key, ctx =>
                {
                    using (var lockFile = _lockFileManager.TryAcquireLock(key, millisecondsTimeout))
                    {
                        if (lockFile != null)
                        {
                            // If we waited for the lock to be released, here the result computed by the locking code should be returned.
                            return _cacheManager.Get(key, acquire);
                        }
                        else return fallback();
                    }
                });
        }
    }
}
