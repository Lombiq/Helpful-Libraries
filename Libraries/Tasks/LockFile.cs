using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.Environment.Extensions;
using Orchard.FileSystems.Media;
using System.IO;

namespace Piedone.HelpfulLibraries.Tasks
{
    [OrchardFeature("Piedone.HelpfulLibraries.Tasks")]
    public class LockFile : ILockFile
    {
        private readonly IStorageProvider _storageProvider;
        private string _name;
        private bool isDisposed = false;
        private bool isAcquired = false;

        private const string _folder = "HelpfulLibraries/Tasks/LockFiles/";

        public LockFile(IStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;
        }

        public bool TryAcquire(string name)
        {
            _name = name;
            isAcquired = true;

            // Here really should be a file existence check, but it's not possible currently:
            // http://orchard.codeplex.com/workitem/18279
            using (var stream = new MemoryStream())
            {
                return _storageProvider.TrySaveStream(MakeFilePath(name), stream);
            }
        }

        public void Dispose()
        {
            if (String.IsNullOrEmpty(_name) || isDisposed || !isAcquired) return;

            isDisposed = true;
            // Could throw exception e.g. if the file was deleted. This should not happen.
            _storageProvider.DeleteFile(MakeFilePath(_name));
        }

        private static string MakeFilePath(string name)
        {
            return _folder + name + ".lock";
        }
    }
}
