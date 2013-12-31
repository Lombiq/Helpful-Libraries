using System;
using Orchard;

namespace Piedone.HelpfulLibraries.Tasks.Locking
{
    /// <summary>
    /// An entity used for locking purposes. This lock works even across nodes of a web farm.
    /// </summary>
    public interface IDistributedLock : IDisposable, ITransientDependency
    {
        /// <summary>
        /// Tries to acquire the lock
        /// </summary>
        /// <param name="name">Name of the lock</param>
        /// <returns>True, if the lock could be acquired or false if not</returns>
        bool TryAcquire(string name);
    }
}
