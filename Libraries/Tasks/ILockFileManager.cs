using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.Caching;

namespace Piedone.HelpfulLibraries.Tasks
{
    public interface ILockFileManager : IVolatileProvider
    {
        ILockFile TryAcquireLock(string name, int millisecondsTimeout = 4000);
    }
}
