using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard;

namespace Piedone.HelpfulLibraries.Caching
{
    public interface ILockFile : IDisposable, ITransientDependency
    {
        bool TryAcquire(string name);
    }
}
