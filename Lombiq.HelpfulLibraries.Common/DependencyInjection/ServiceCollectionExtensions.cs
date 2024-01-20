using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Lombiq.HelpfulLibraries.Common.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Enables lazy dependency injection, i.e. delaying the initialization of dependencies injected into constructors.
    /// </summary>
    // This implementation is based on this StackOverflow answer: https://stackoverflow.com/a/45775657/4611736
    public static void AddLazyInjectionSupport(this IServiceCollection services) =>
        services.TryAddTransient(typeof(Lazy<>), typeof(Lazier<>));

    private sealed class Lazier<T>(IServiceProvider provider) : Lazy<T>(provider.GetRequiredService<T>)
        where T : class
    {
    }
}
