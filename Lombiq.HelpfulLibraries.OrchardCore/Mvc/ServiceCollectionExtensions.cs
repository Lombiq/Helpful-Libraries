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
        where T : class, new()
    {
        var options = new T();
        var configSection = shellConfiguration.GetSection(sectionName);
        configSection.Bind(options);
        services.Configure<T>(configSection);

        return services;
    }
}
