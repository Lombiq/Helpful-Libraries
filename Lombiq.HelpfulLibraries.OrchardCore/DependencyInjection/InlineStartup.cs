#nullable enable

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Modules;
using System;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.DependencyInjection;

/// <summary>
/// A startup class that invokes the delegates provided in the constructor.
/// </summary>
public class InlineStartup : StartupBase
{
    private readonly Action<IServiceCollection>? _configureServices;
    private readonly Action<IApplicationBuilder, IEndpointRouteBuilder, IServiceProvider>? _configure;
    private readonly Func<IApplicationBuilder, IEndpointRouteBuilder, IServiceProvider, ValueTask>? _configureAsync;

    public override int Order { get; }

    public InlineStartup(
        Action<IServiceCollection>? configureServices,
        Action<IApplicationBuilder> configure,
        Func<IApplicationBuilder, IEndpointRouteBuilder, IServiceProvider, ValueTask>? configureAsync = null,
        int order = 0)
        : this(configureServices, (app, _, _) => configure(app), configureAsync, order)
    {
    }

    public InlineStartup(
        Action<IServiceCollection>? configureServices = null,
        Action<IApplicationBuilder, IEndpointRouteBuilder, IServiceProvider>? configure = null,
        Func<IApplicationBuilder, IEndpointRouteBuilder, IServiceProvider, ValueTask>? configureAsync = null,
        int order = 0)
    {
        _configureServices = configureServices;
        _configure = configure;
        _configureAsync = configureAsync;
        Order = order;
    }

    public override void ConfigureServices(IServiceCollection services) =>
        _configureServices?.Invoke(services);

    public override void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider) =>
        _configure?.Invoke(app, routes, serviceProvider);

    public override ValueTask ConfigureAsync(IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider) =>
        _configureAsync?.Invoke(app, routes, serviceProvider) ?? ValueTask.CompletedTask;
}
