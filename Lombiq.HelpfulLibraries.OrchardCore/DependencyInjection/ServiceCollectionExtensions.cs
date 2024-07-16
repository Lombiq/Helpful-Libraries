#nullable enable

using Lombiq.HelpfulLibraries.Common.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OrchardCore.Modules;
using System;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds <see cref="IOrchardServices{T}"/> and its implementation <see cref="OrchardServices{T}"/> to the
    /// service collection, making them available for use. Also enables lazy dependency injection.
    /// </summary>
    public static void AddOrchardServices(this IServiceCollection services)
    {
        services.AddLazyInjectionSupport();
        services.TryAddTransient(typeof(IOrchardServices<>), typeof(OrchardServices<>));
    }

    /// <summary>
    /// Creates a new <see cref="InlineStartup"/> instance using the provided parameters, and adds it to the service
    /// collection.
    /// </summary>
    public static IServiceCollection AddInlineStartup(
        this IServiceCollection services,
        Action<IServiceCollection>? configureServices = null,
        Action<IApplicationBuilder, IEndpointRouteBuilder, IServiceProvider>? configure = null,
        Func<IApplicationBuilder, IEndpointRouteBuilder, IServiceProvider, ValueTask>? configureAsync = null,
        int order = 0) =>
        services.AddSingleton<IStartup>(new InlineStartup(configureServices, configure, configureAsync, order));

    /// <summary>
    /// Creates a new <see cref="InlineStartup"/> instance using the provided parameters, and adds it to the service
    /// collection.
    /// </summary>
    public static IServiceCollection AddInlineStartup(
        this IServiceCollection services,
        Action<IServiceCollection>? configureServices,
        Action<IApplicationBuilder> configure,
        Func<IApplicationBuilder, IEndpointRouteBuilder, IServiceProvider, ValueTask>? configureAsync = null,
        int order = 0) =>
        services.AddSingleton<IStartup>(new InlineStartup(configureServices, configure, configureAsync, order));
}
