using Orchard;
using Orchard.Environment.Configuration;
using Orchard.Environment.Extensions;
using Orchard.FileSystems.AppData;

namespace Piedone.HelpfulLibraries.Utilities
{
    /// <summary>
    /// Service for managing ORM mappings
    /// </summary>
    public interface IMappingsManager : IDependency
    {
        /// <summary>
        /// Clears cached mappings and thus forces the regeneration of the cache.
        /// </summary>
        void Clear();
    }


    [OrchardFeature("Piedone.HelpfulLibraries.Utilities")]
    public class MappingsManager : IMappingsManager
    {
        private readonly ShellSettings _shellSettings;
        private readonly IAppDataFolder _appDataFolder;


        public MappingsManager(ShellSettings shellSettings, IAppDataFolder appDataFolder)
        {
            _shellSettings = shellSettings;
            _appDataFolder = appDataFolder;
        }


        public void Clear()
        {
            _appDataFolder.DeleteFile(_appDataFolder.Combine("Sites", _shellSettings.Name, "mappings.bin"));
        }
    }
}
