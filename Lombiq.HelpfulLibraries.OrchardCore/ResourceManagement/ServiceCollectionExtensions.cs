using Lombiq.HelpfulLibraries.OrchardCore.ResourceManagement;
using System;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the provided <typeparamref name="T"/> service as an <see cref="IResourceFilterProvider"/> service.
    /// </summary>
    public static IServiceCollection AddResourceFilter<T>(this IServiceCollection services)
        where T : class, IResourceFilterProvider =>
        services.AddScoped<IResourceFilterProvider, T>();

    /// <summary>
    /// Registers the simple implementation of the <see cref="IResourceFilterProvider"/> service that invokes the
    /// provided <paramref name="filter"/>.
    /// </summary>
    public static IServiceCollection AddResourceFilter(
        this IServiceCollection services,
        Action<ResourceFilterBuilder> filter,
        params string[] requiredThemes) =>
        services.AddScoped<IResourceFilterProvider, SimpleResourceFilterProvider>(_ => new(filter, requiredThemes));
}
