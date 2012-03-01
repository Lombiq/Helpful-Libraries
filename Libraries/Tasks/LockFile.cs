using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.Environment.Extensions;
using Orchard.FileSystems.Media;

namespace Piedone.HelpfulLibraries.Tasks
{
    [OrchardFeature("Piedone.HelpfulLibraries.Tasks")]
    public class LockFile : ILockFile
    {
        private readonly IStorageProvider _storageProvider;
        private string _name;

        private const string _folder = "HelpfulLibraries/Tasks/LockFiles/";

        public LockFile(IStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;
        }

        public bool TryAcquire(string name)
        {
            _name = name;

            // Here really should be a file existence check, but it's not possible currently:
            // http://orchard.codeplex.com/workitem/18279
            try
            {
                _storageProvider.CreateFile(MakeFilePath(name));
                return true;
            }
            catch (ArgumentException)
            {
                // The file exists
            }

            return false;
        }

        public void Dispose()
        {
            if (String.IsNullOrEmpty(_name)) return;

            // Could throw exception...
            _storageProvider.DeleteFile(MakeFilePath(_name));
        }

        private static string MakeFilePath(string name)
        {
            return _folder + name + ".lock";
        }
    }
}
