using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.Caching.Services;
using Orchard.Environment.Extensions;
using Orchard.Services;
using Orchard.TaskLease.Services;
using Piedone.HelpfulLibraries.KeyValueStore;
using Piedone.HelpfulLibraries.Tasks.Locking;
using Orchard.Environment;

namespace Piedone.HelpfulLibraries.Tasks
{
    /// <summary>
    /// A decorator for <see cref="ITaskLeaseService"/> that makes the lease visible from other server nodes even before it was committed to the DB.
    /// </summary>
    /// <remarks>
    /// Since an expiry date is needed distributed locks can't be used. This needs an Orchard.Caching provider with multi-node support.
    /// </remarks>
    [OrchardFeature("Piedone.HelpfulLibraries.Tasks.TaskLease")]
    internal class TaskLeaseServiceDecorator : ITaskLeaseService
    {
        private readonly ITaskLeaseService _decorated;
        private readonly ICacheService _cacheService;
        private readonly IApplicationEnvironment _applicationEnvironment;
        private readonly IClock _clock;


        public TaskLeaseServiceDecorator(
            ITaskLeaseService decorated,
            ICacheService cacheService,
            IApplicationEnvironment applicationEnvironment,
            IClock clock)
        {
            _decorated = decorated;
            _cacheService = cacheService;
            _applicationEnvironment = applicationEnvironment;
            _clock = clock;
        }


        public string Acquire(string taskName, DateTime expiredUtc)
        {
            var cacheKey = MakeCacheKey(taskName);
            var machineName = _applicationEnvironment.GetEnvironmentIdentifier();
            var existingLease = _cacheService.Get(cacheKey);
            if (existingLease != null && ((string)existingLease) != machineName) return null;
            _cacheService.Put(cacheKey, machineName, expiredUtc - _clock.UtcNow);
            return _decorated.Acquire(taskName, expiredUtc);
        }

        public void Update(string taskName, string state)
        {
            _cacheService.Put(MakeCacheKey(taskName), _applicationEnvironment.GetEnvironmentIdentifier());
            _decorated.Update(taskName, state);
        }

        public void Update(string taskName, string state, DateTime expiredUtc)
        {
            _cacheService.Put(MakeCacheKey(taskName), _applicationEnvironment.GetEnvironmentIdentifier(), expiredUtc - _clock.UtcNow);
            _decorated.Update(taskName, state, expiredUtc);
        }


        private static string MakeCacheKey(string taskName)
        {
            return "Piedone.HelpfulLibraries.Tasks.TaskLease.TaskLeaseServiceDecorator." + taskName;
        }
    }
}
