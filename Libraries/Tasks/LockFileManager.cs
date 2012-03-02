using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Piedone.HelpfulLibraries.DependencyInjection;
using System.Threading;
using Orchard.Environment.Extensions;

namespace Piedone.HelpfulLibraries.Tasks
{
    [OrchardFeature("Piedone.HelpfulLibraries.Caching")]
    public class LockFileManager : ILockFileManager
    {
        private readonly IResolve<ILockFile> _lockFileResolve;

        public LockFileManager(IResolve<ILockFile> lockFileResolve)
        {
            _lockFileResolve = lockFileResolve;
        }

        public ILockFile TryAcquireLock(string name, int millisecondsTimeout = 4000)
        {
            int waitedMilliseconds = 0;
            var lockFile = _lockFileResolve.Value;

            while (!lockFile.TryAcquire(name) && waitedMilliseconds < millisecondsTimeout)
            {
                Thread.Sleep(1000);
                waitedMilliseconds += 1000;
            }

            if (waitedMilliseconds != millisecondsTimeout) return lockFile;
            else return null;
        }
    }
}