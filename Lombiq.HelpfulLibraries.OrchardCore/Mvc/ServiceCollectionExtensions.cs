using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Environment.Shell.Configuration;

namespace Lombiq.HelpfulLibraries.OrchardCore.Mvc;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Binds a configuration section to a class and configures it in the service collection.
    /// </summary>
    public static IServiceCollection BindAndConfigureSection<T>(
        this IServiceCollection services,
        IShellConfiguration shellConfiguration,
        string sectionName)
        where T : class, new() =>
        services.Configure<T>(shellConfiguration.BindNew<T>(sectionName).ConfigurationSection);
}
