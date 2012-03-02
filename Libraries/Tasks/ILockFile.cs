using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard;

namespace Piedone.HelpfulLibraries.Tasks
{
    public interface ILockFile : IDisposable, ITransientDependency
    {
        bool TryAcquire(string name);
    }
}
