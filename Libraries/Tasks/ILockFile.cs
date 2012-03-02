using System;
using Orchard;

namespace Piedone.HelpfulLibraries.Tasks
{
    public interface ILockFile : IDisposable, ITransientDependency
    {
        bool TryAcquire(string name);
    }
}
