using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard;
using Orchard.Environment.Extensions;

namespace Piedone.HelpfulLibraries.Utilities
{
    /// <summary>
    /// Exposes application configuration (could be e.g. AppSettings from Web.config or CloudConfiguration on Azure).
    /// </summary>
    public interface IAppConfigurationAccessor : ISingletonDependency
    {
        string GetConfiguration(string name);
    }


    [OrchardFeature("Piedone.HelpfulLibraries.Utilities")]
    public class AppConfigurationAccessor : IAppConfigurationAccessor
    {
        public string GetConfiguration(string name)
        {
            var value = ConfigurationManager.AppSettings[name];
            if (value != null) return value;

            var connectionStringSettings = ConfigurationManager.ConnectionStrings[name];
            if (connectionStringSettings != null) return connectionStringSettings.ConnectionString;

            return string.Empty;
        }
    }
}
