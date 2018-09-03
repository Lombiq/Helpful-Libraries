using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.Environment.Extensions;
using Orchard.FileSystems.Media;

namespace Piedone.HelpfulLibraries.Utilities
{
    [OrchardFeature("Piedone.HelpfulLibraries.Utilities")]
    public static class StorageProviderExtensions
    {
        /// <summary>
        /// Gets the directory separator character used by the <see cref="IStorageProvider"/> instance.
        /// </summary>
        public static char GetDirectorySeparator(this IStorageProvider storageProvider) =>
            storageProvider.Combine("s", "s").Trim('s').First();
    }
}
