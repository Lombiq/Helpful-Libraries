using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.Tasks;
using Orchard.Caching;

namespace Piedone.HelpfulLibraries.Tasks
{
    public interface ILockFileManager : IVolatileProvider
    {
        /// <summary>
        /// Tries to acquire a lock with the specified parameters
        /// </summary>
        /// <param name="name">Unique name of the lock</param>
        /// <param name="millisecondsTimeout">Milliseconds to wait for the lock before timing out</param>
        /// <returns>The ILockFile instance on success or null if the lock couldn't be acquired.</returns>
        ILockFile TryAcquireLock(string name, int millisecondsTimeout = 4000);
    }
}
