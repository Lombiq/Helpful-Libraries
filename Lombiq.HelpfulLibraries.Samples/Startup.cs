using Lombiq.HelpfulLibraries.AspNetCore.Extensions;
using Lombiq.HelpfulLibraries.OrchardCore.ResourceManagement;
using Lombiq.HelpfulLibraries.Samples.Migrations;
using Lombiq.HelpfulLibraries.Samples.Navigation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrchardCore.Data.Migration;
using OrchardCore.Modules;
using OrchardCore.Navigation;
using OrchardCore.ResourceManagement;

namespace Lombiq.HelpfulLibraries.Samples;

public class Startup : StartupBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddDataMigration<BookRecordMigrations>();
        services.AddDataMigration<ExpressionSampleMigration>();
        services.AddScoped<INavigationProvider, HelpfulLibrariesNavigationProvider>();

        services.AddTransient<IConfigureOptions<ResourceManagementOptions>, ResourceManagementOptionsConfiguration>();
        services.AddAsyncResultFilter<ScriptModuleResourceFilter>();
    }
}
