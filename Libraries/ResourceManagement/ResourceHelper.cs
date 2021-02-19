using Microsoft.Extensions.FileProviders;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Lombiq.HelpfulLibraries.Libraries.ResourceManagement
{
    public static class ResourceHelper
    {
        /// <summary>
        /// Returns the contents of an embedded text file in a given assembly.
        /// </summary>
        /// <param name="assembly">The assembly the file is embedded into.</param>
        /// <param name="path">The relative path inside the assembly's project.</param>
        public static string GetFile(Assembly assembly, string path)
        {
            var provider = new EmbeddedFileProvider(assembly);
            var fileInfo = provider.GetDirectoryContents(string.Empty).Single(x => x.Name == path);
            using var stream = fileInfo.CreateReadStream();
            using var streamReader = new StreamReader(stream);
            return streamReader.ReadToEnd();
        }

        /// <summary>
        /// Returns the contents of an embedded text file inside the same assembly as the given type. The embedded path
        /// is "Resources/{typeName}.{extension}".
        /// </summary>
        /// <typeparam name="T">A type defined in the same assembly where the embedded resource is.</typeparam>
        /// <param name="extension">The extension of the target file.</param>
        public static string GetFile<T>(string extension) =>
            GetFile(typeof(T).Assembly, $"Resources>{typeof(T).Name}.{extension}");
    }
}
