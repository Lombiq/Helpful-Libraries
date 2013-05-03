using Orchard.Environment.Configuration;
using Orchard.Environment.Extensions;
using Orchard.FileSystems.AppData;

namespace Piedone.HelpfulLibraries.Libraries.Utilities
{
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
