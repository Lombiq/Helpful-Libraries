using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Scripting;

namespace Lombiq.HelpfulLibraries.OrchardCore.MethodProviders;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds <see cref="IGlobalMethodProvider"/> and its implementation <see cref="UserMethodProvider"/> to the
    /// service collection, making them available for use.
    /// </summary>
    public static IServiceCollection AddHelpfulMethodProviders(this IServiceCollection services) =>
        services.AddSingleton<IGlobalMethodProvider, UserMethodProvider>();
}
