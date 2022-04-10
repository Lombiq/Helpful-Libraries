using Lombiq.HelpfulLibraries.Common.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Lombiq.HelpfulLibraries.OrchardCore.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static void AddOrchardServices(this IServiceCollection services)
    {
        services.AddLazyInjectionSupport();
        services.TryAddTransient(typeof(IOrchardServices<>), typeof(OrchardServices<>));
    }
}
