using LinqToDB.AspNet;
using LinqToDB.AspNet.Logging;
using LinqToDB.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrchardCore.Environment.Shell;
using OrchardCore.Modules;
using System;
using System.IO;
using static Lombiq.HelpfulLibraries.LinqToDb.Constants.FeatureIds;
using Configuration = LinqToDB.Common.Configuration;

namespace Lombiq.HelpfulLibraries.LinqToDb
{
    [Feature(Default)]
    public class Startup : StartupBase
    {
        private readonly IOptions<ShellOptions> _shellOptions;
        private readonly ShellSettings _shellSettings;

        public Startup(IOptions<ShellOptions> shellOptions, ShellSettings shellSettings)
        {
            _shellOptions = shellOptions;
            _shellSettings = shellSettings;
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            switch (_shellSettings["DatabaseProvider"])
            {
                case "SqlConnection":
                    services.AddLinqToDbContext<LinqToDbConnection>(
                    (provider, options) =>
                        options
                        // Using explicit string instead of LinqToDB.ProviderName.SQLServer because the "System.Data.SqlClient"
                        // provider will be used causing "Could not load type System.Data.SqlClient.SqlCommandBuilder"
                        // exception. See: https://github.com/linq2db/linq2db/issues/2191#issuecomment-618450439
                        .UseConnectionString("Microsoft.Data.SqlClient", _shellSettings["ConnectionString"])
                        .UseDefaultLogging(provider),
                    ServiceLifetime.Transient);
                    break;
                case "Sqlite":
                    var option = _shellOptions.Value;
                    var databaseFolder = Path.Combine(option.ShellsApplicationDataPath, option.ShellsContainerName, _shellSettings.Name);
                    var databaseFile = Path.Combine(databaseFolder, "yessql.db");

                    services.AddLinqToDbContext<LinqToDbConnection>(
                    (provider, options) =>
                        options
                        .UseSQLite($"Data Source={databaseFile};Cache=Shared")
                        .UseDefaultLogging(provider),
                    ServiceLifetime.Transient);
                    break;
                default:
                    throw new ArgumentException("Unknown database provider: " + _shellSettings["DatabaseProvider"]);
            }

            LinqToDbConnection.TablePrefix = _shellSettings["TablePrefix"];

            // Generate aliases for final projection.
            Configuration.Sql.GenerateFinalAliases = true;
        }
    }
}
