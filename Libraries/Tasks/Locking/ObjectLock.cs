using Orchard.Environment.Extensions;
using Piedone.HelpfulLibraries.Tasks.Locking;

namespace Piedone.HelpfulLibraries.Libraries.Tasks.Locking
{
    [OrchardFeature("Piedone.HelpfulLibraries.Tasks.Locking")]
    public class ObjectLock : IDistributedLock
    {
        private readonly IObjectLockHolder _objectLockHolder;

        private string _name;


        public ObjectLock(IObjectLockHolder objectLockHolder) => _objectLockHolder = objectLockHolder;


        public bool TryAcquire(string name)
        {
            _name = name;
            return _objectLockHolder.TryAcquire(name);
        }

        public void Dispose()
        {
            if (!string.IsNullOrEmpty(_name)) _objectLockHolder.Release(_name);
        }
    }
}
