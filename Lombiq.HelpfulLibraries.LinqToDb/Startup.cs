using LinqToDB.AspNet;
using LinqToDB.AspNet.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Environment.Shell.Configuration;
using OrchardCore.Modules;
using System;
using static Lombiq.HelpfulLibraries.LinqToDb.Constants.FeatureIds;

namespace Lombiq.HelpfulLibraries.LinqToDb
{
    [Feature(Default)]
    public class Startup : StartupBase
    {
        private readonly IShellConfiguration _shellConfiguration;

        public Startup(IShellConfiguration shellConfiguration) => _shellConfiguration = shellConfiguration;
        public override void ConfigureServices(IServiceCollection services)
        {
            var connectionString = _shellConfiguration["ConnectionString"];
            services.AddLinqToDbContext<PrefixedDataConnection>(
                (provider, options) =>
                    options
                    .UseConnectionString("Microsoft.Data.SqlClient", connectionString)
                    .UseDefaultLogging(provider),
                ServiceLifetime.Transient);

        }

        public override void Configure(
            IApplicationBuilder app,
            IEndpointRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
            // Method intentionally left empty.
        }
    }
}
