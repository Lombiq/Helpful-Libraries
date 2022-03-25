using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Lombiq.HelpfulLibraries.Libraries.DependencyInjection;

public static class ServiceCollectionExtensions
{
    // This implementation is based on this StackOverflow answer: https://stackoverflow.com/a/45775657/4611736
    public static void AddLazyInjectionSupport(this IServiceCollection services) =>
        services.TryAddTransient(typeof(Lazy<>), typeof(Lazier<>));

    public static void AddOrchardServices(this IServiceCollection services)
    {
        services.AddLazyInjectionSupport();
        services.TryAddTransient(typeof(IOrchardServices<>), typeof(OrchardServices<>));
    }

    private class Lazier<T> : Lazy<T>
        where T : class
    {
        public Lazier(IServiceProvider provider)
            : base(() => provider.GetRequiredService<T>()) { }
    }
}
