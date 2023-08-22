using System.Collections.Concurrent;
using Orchard;
using Orchard.Environment.Extensions;

namespace Piedone.HelpfulLibraries.Libraries.Tasks.Locking
{
    public interface IObjectLockHolder : ISingletonDependency
    {
        bool TryAcquire(string name);
        void Release(string name);
    }


    [OrchardFeature("Piedone.HelpfulLibraries.Tasks.Locking")]
    public class ObjectLockHolder : IObjectLockHolder
    {
        private readonly object _dictionaryLock = new object();
        private ConcurrentDictionary<string, bool> _locks = new ConcurrentDictionary<string, bool>();


        public bool TryAcquire(string name)
        {
            lock (_dictionaryLock)
            {
                return _locks.TryAdd(name, true);
            }
        }

        public void Release(string name)
        {
            lock (_dictionaryLock)
            {
                _locks.TryRemove(name, out _);
            }
        }
    }
}
