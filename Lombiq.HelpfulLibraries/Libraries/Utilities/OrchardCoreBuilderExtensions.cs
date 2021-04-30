using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class OrchardCoreBuilderExtensions
    {
        /// <summary>
        /// Adds database shell configuration usage but only if the necessary connection string configuration is
        /// available.
        /// </summary>
        public static OrchardCoreBuilder AddDatabaseShellsConfigurationIfAvailable(
            this OrchardCoreBuilder builder,
            IConfiguration configuration)
        {
            var shellsConnectionString = configuration
                .GetValue<string>("OrchardCore:OrchardCore_Shells_Database:ConnectionString");

            if (!string.IsNullOrEmpty(shellsConnectionString)) builder.AddDatabaseShellsConfiguration();

            return builder;
        }
    }
}
