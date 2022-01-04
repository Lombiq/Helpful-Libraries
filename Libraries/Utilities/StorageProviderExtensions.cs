using Orchard.FileSystems.Media;
using System.Linq;

namespace Piedone.HelpfulLibraries.Utilities
{
    public static class StorageProviderExtensions
    {
        /// <summary>
        /// Gets the directory separator character used by the <see cref="IStorageProvider"/> instance.
        /// </summary>
        public static char GetDirectorySeparator(this IStorageProvider storageProvider) =>
            storageProvider.Combine("s", "s").Trim('s').First();
    }
}
