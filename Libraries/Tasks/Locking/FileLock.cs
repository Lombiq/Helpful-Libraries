using System;
using System.IO;
using Orchard.Environment.Extensions;
using Orchard.FileSystems.Media;
using Orchard.Exceptions;

namespace Piedone.HelpfulLibraries.Tasks.Locking
{
    [OrchardFeature("Piedone.HelpfulLibraries.Tasks.Locking")]
    public class FileLock : IDistributedLock
    {
        private readonly IStorageProvider _storageProvider;
        private string _name;
        private bool _isDisposed = false;
        private bool _isAcquired = false;


        public FileLock(IStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;
        }


        public bool TryAcquire(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            name = name.Replace("-", "--").Replace('/', '-').Replace('\\', '-').Replace('.', '_');
            using (var stream = new MemoryStream())
            {
                var canAcquire = _storageProvider.TrySaveStream(MakeFilePath(name), stream);

                if (canAcquire)
                {
                    _name = name;
                    _isAcquired = true;
                }

                return canAcquire;
            }
        }

        // This will be called at least by Autofac when the request ends
        public void Dispose()
        {
            if (_isDisposed || !_isAcquired) return;

            _isDisposed = true;
            // Could throw exception e.g. if the file was deleted. This should not happen.
            try
            {
                _storageProvider.DeleteFile(MakeFilePath(_name));
            }
            catch (Exception ex)
            {
                if (ex.IsFatal()) throw;
            }
        }

        private static string MakeFilePath(string name)
        {
            return WellKnownConstants.LockFileFolder + name + ".lock";
        }
    }
}
