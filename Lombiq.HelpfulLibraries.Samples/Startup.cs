using Lombiq.HelpfulLibraries.Samples.Migrations;
using Lombiq.HelpfulLibraries.Samples.Navigation;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Data.Migration;
using OrchardCore.Modules;
using OrchardCore.Navigation;

namespace Lombiq.HelpfulLibraries.Samples;

public sealed class Startup : StartupBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddDataMigration<BookRecordMigrations>();
        services.AddDataMigration<ExpressionSampleMigration>();
        services.AddNavigationProvider<HelpfulLibrariesNavigationProvider>();
    }
}
