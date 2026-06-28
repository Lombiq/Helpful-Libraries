using Lombiq.HelpfulLibraries.Common.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using OrchardCore.Environment.Shell;
using OrchardCore.Environment.Shell.Configuration;
using OrchardCore.Environment.Shell.Descriptor.Models;
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
    /// Enables the provided tenant features, but only for the <see cref="ShellSettings.DefaultShellName"/> tenant.
    /// </summary>
    public static IServiceCollection AddDefaultTenantFeatures(
        this IServiceCollection services,
        params string[] featureIds)
    {
        foreach (var id in featureIds)
        {
            services.AddTransient(sp =>
            {
                var shellSettings = sp.GetRequiredService<ShellSettings>();
                return shellSettings.Name == ShellSettings.DefaultShellName
                    ? new ShellFeature(id, alwaysEnabled: true)
                    : new();
            });
        }

        return services;
    }

    /// <summary>
    /// Enables the provided tenant features, but only for the <see cref="ShellSettings.DefaultShellName"/> tenant.
    /// </summary>
    public static OrchardCoreBuilder AddDefaultTenantFeatures(
        this OrchardCoreBuilder builder,
        params string[] featureIds) =>
        builder.ConfigureServices(services => services.AddDefaultTenantFeatures(featureIds));

    /// <summary>
    /// Configures the <typeparamref name="TOptions"/> using configuration found at the <paramref name="sectionKey"/> in
    /// the <see cref="IShellConfiguration"/>. This registers the <see cref="IOptions{TOptions}"/> of <typeparamref
    /// name="TOptions"/> service for use in dependency injection.
    /// </summary>
    public static OptionsBuilder<TOptions> ConfigureFromShellConfiguration<TOptions>(
        this IServiceCollection services,
        string sectionKey)
        where TOptions : class =>
        services
            .AddOptions<TOptions>()
            .Configure<IShellConfiguration>((options, configuration) =>
                configuration.GetSection(sectionKey).Bind(options));
}
