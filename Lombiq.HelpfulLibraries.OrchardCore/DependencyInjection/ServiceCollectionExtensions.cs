#nullable enable

using Lombiq.HelpfulLibraries.Common.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OrchardCore.Admin;
using OrchardCore.DisplayManagement.Descriptors;
using OrchardCore.DisplayManagement.Theming;
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

    /// <summary>
    /// Registers an <see cref="IStartup"/> implementation that prepares the shape table for the current site and admin
    /// themes. The <see cref="IStartup.Order"/> is the maximum possible value, ensuring that this will be executed
    /// right before the site starts serving.
    /// </summary>
    public static IServiceCollection PrepareShapeTable(this IServiceCollection services) =>
        services.AddInlineStartup(
            configureAsync: async (_, _, serviceProvider) =>
            {
                var shapeTableManager = serviceProvider.GetRequiredService<IShapeTableManager>();

                var siteTheme = await serviceProvider.GetRequiredService<IThemeManager>().GetThemeAsync();
                var adminTheme = await serviceProvider.GetRequiredService<IAdminThemeService>().GetAdminThemeAsync();

                await shapeTableManager.GetShapeTableAsync(themeId: null);
                await shapeTableManager.GetShapeTableAsync(siteTheme.Id);
                await shapeTableManager.GetShapeTableAsync(adminTheme.Id);
            },
            order: int.MaxValue);
}
