using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Mvc.Routing;
using System;

namespace Lombiq.HelpfulLibraries.OrchardCore.Mvc;

[Obsolete("Use the [Admin(route)] attribute instead of [AdminRoute(route)].")]
public class AdminRouteAttributeRouteMapper : IAreaControllerRouteMapper
{
    public int Order => 0;

    public bool TryMapAreaControllerRoute(IEndpointRouteBuilder routes, ControllerActionDescriptor descriptor) =>
        ThrowNotSupported();

    public static void AddToServices(IServiceCollection services) => ThrowNotSupported();

    private static bool ThrowNotSupported() =>
        throw new NotSupportedException(
            $"Please disable {nameof(AdminRouteAttributeRouteMapper)} and if you still have controllers or actions " +
            $"with the [AdminRoute(route)] attribute, replace them with OrchardCore's built-in [Admin(route)].");
}
