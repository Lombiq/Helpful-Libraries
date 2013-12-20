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
    /// Can be used to access the <see cref="System.Configuration.ConfigurationManager">ConfigurationManager</see> collections.
    /// </summary>
    /// <remarks>
    /// As an injectable dependency this can be easily mocked or stubbed, on contrary to the default ConfigurationManager static class.
    /// </remarks>
    public interface IConfigurationAccessor : ISingletonDependency
    {
        NameValueCollection AppSettings { get; }
        ConnectionStringSettingsCollection ConnectionStrings { get; }
    }


    [OrchardFeature("Piedone.HelpfulLibraries.Utilities")]
    public class ConfigurationAccessor : IConfigurationAccessor
    {
        public NameValueCollection AppSettings
        {
            get { return ConfigurationManager.AppSettings; }
        }

        public ConnectionStringSettingsCollection ConnectionStrings
        {
            get { return ConfigurationManager.ConnectionStrings; }
        }
    }
}
