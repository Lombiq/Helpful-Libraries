using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Modules;
using System;

namespace Lombiq.HelpfulLibraries.OrchardCore.DependencyInjection;

/// <summary>
/// A startup class that invokes the delegates provided in the constructor.
/// </summary>
public class InlineStartup(
    Action<IServiceCollection> configureServices,
    Action<IApplicationBuilder, IEndpointRouteBuilder, IServiceProvider> configure = null,
    int order = 0) : StartupBase
{
    private readonly Action<IServiceCollection> _configureServices = configureServices;
    private readonly Action<IApplicationBuilder, IEndpointRouteBuilder, IServiceProvider> _configure = configure;
    private readonly int _order = order;

    public override int Order => _order;

    public InlineStartup(
        Action<IServiceCollection> configureServices,
        Action<IApplicationBuilder> configure,
        int order = 0)
        : this(configureServices, (app, _, _) => configure(app), order)
    {
    }

    public override void ConfigureServices(IServiceCollection services) =>
        _configureServices?.Invoke(services);

    public override void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider) =>
        _configure?.Invoke(app, routes, serviceProvider);
}
