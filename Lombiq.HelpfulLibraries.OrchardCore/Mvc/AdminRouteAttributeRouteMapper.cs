using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrchardCore.Admin;
using OrchardCore.Mvc.Routing;
using System.Reflection;

namespace Lombiq.HelpfulLibraries.OrchardCore.Mvc;

// Mark this class and AdminRouteAttribute as Obsolete when upgrading to the first Orchard Core version that contains
// https://github.com/OrchardCMS/OrchardCore/pull/15251. The message should inform to use [Admin(template)] instead.

/// <summary>
/// A route mapper that maps a route comprised of the <see cref="AdminOptions.AdminUrlPrefix"/> and the provided
/// template string to any action that has the <see cref="AdminRouteAttribute"/>.
/// </summary>
/// <remarks>
/// <para>
/// In practice this mapper makes <c>[AdminRoute("My/Path/{id}")]</c> work the same way as if you used
/// <c>[Route("Admin/My/Path/{id}")]</c> except the admin prefix is no longer hard coded.
/// </para>
/// <para>
/// It can be added to the DI service collection using the <see cref="AddToServices"/> static method.
/// </para>
/// </remarks>
public class AdminRouteAttributeRouteMapper : IAreaControllerRouteMapper
{
    private readonly string _adminUrlPrefix;

    // Must take precedence over AdminAreaControllerRouteMapper whose Order value is -1000.
    public int Order => -2000;

    public AdminRouteAttributeRouteMapper(IOptions<AdminOptions> adminOptions) =>
        _adminUrlPrefix = adminOptions.Value.AdminUrlPrefix;

    public bool TryMapAreaControllerRoute(IEndpointRouteBuilder routes, ControllerActionDescriptor descriptor)
    {
        if (descriptor.MethodInfo.GetCustomAttribute<AdminRouteAttribute>() is not { } routeAttribute) return false;

        routes.MapAreaControllerRoute(
            name: descriptor.DisplayName,
            areaName: descriptor.RouteValues["area"],
            pattern: $"{_adminUrlPrefix}/{routeAttribute.Template.TrimStart('/')}",
            defaults: new { controller = descriptor.ControllerName, action = descriptor.ActionName }
        );

        return true;
    }

    public static void AddToServices(IServiceCollection services) =>
        services.AddTransient<IAreaControllerRouteMapper, AdminRouteAttributeRouteMapper>();
}
