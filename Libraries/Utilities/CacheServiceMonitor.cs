using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.Caching.Services;
using Orchard.Environment.Extensions;

namespace Piedone.HelpfulLibraries.Utilities
{
    /// <summary>
    /// Lets you subscribe ICacheService cache keys to events. If the event is triggered all the entries for the 
    /// subscribed keys will be removed. You can use this to invalidate multiple subscribed cache entries at once,
    /// without having to keep track of said cache entries.
    /// </summary>
    [OrchardFeature("Piedone.HelpfulLibraries.Utilities")]
    public static class CacheServiceMonitor
    {
        private const string KeyChainCacheKey = "Piedone.HelpfulLibraries.Utilities.CacheServiceMonitor.KeyChain.";


        /// <summary>
        /// Subscribe a cache key to an event. If the event is triggered the entry for the cache key will be removed. 
        /// Call this method only when the corresponding cache entry is newly created (like in the factory delegate of
        /// the ICacheService.Get() extension method) so it's only executed when needed.
        /// </summary>
        /// <param name="eventKey">The key of the event that can be triggered.</param>
        /// <param name="cacheKey">The key of the cache entry.</param>
        public static void Monitor(this ICacheService cacheService, string eventKey, string cacheKey)
        {
            var keys = cacheService.GetKeys(eventKey);
            keys.GetOrAdd(cacheKey, 0);
            cacheService.Put(GetKeyChainKey(eventKey), keys);
        }

        /// <summary>
        /// Removes all cache entries that were subscribed to the specified event.
        /// </summary>
        /// <param name="eventKey">The key of the event that will be triggered.</param>
        public static void Trigger(this ICacheService cacheService, string eventKey)
        {
            foreach (var cacheKey in cacheService.GetKeys(eventKey).Keys.ToList())
            {
                cacheService.Remove(cacheKey);
            }
        }


        // A concurrent HashSet would be better, but there is no such collection currently.
        private static ConcurrentDictionary<string, byte> GetKeys(this ICacheService cacheService, string eventKey)
        {
            return cacheService.Get(GetKeyChainKey(eventKey), () =>
            {
                return new ConcurrentDictionary<string, byte>();
            });
        }

        private static string GetKeyChainKey(string eventKey)
        {
            return KeyChainCacheKey + eventKey;
        }
    }
}
