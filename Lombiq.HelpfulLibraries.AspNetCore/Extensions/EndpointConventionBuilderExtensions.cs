using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Lombiq.HelpfulLibraries.AspNetCore.Extensions;

public static class EndpointConventionBuilderExtensions
{
    public static IEndpointConventionBuilder RequireApiAuthorization(this IEndpointConventionBuilder builder)
    {
        builder.RequireAuthorization(policy =>
            policy.RequireAuthenticatedUser().AddAuthenticationSchemes("Api"));
        return builder;
    }

    public static IEndpointConventionBuilder DefaultSettings(this IEndpointConventionBuilder builder) =>
        builder.DisableAntiforgery().RequireApiAuthorization();

    public static RouteHandlerBuilder MapGetWithDefaultSettings(
        this IEndpointRouteBuilder builder,
        [StringSyntax("Route")] string pattern,
        Delegate handler)
    {
        var router = builder.MapGet(pattern, handler);
        router.DefaultSettings();
        return router;
    }

    public static RouteHandlerBuilder MapPostWithDefaultSettings(
        this IEndpointRouteBuilder builder,
        [StringSyntax("Route")] string pattern,
        Delegate handler)
    {
        var router = builder.MapPost(pattern, handler);
        router.DefaultSettings();
        return router;
    }

    public static RouteHandlerBuilder MapPutWithDefaultSettings(
        this IEndpointRouteBuilder builder,
        [StringSyntax("Route")] string pattern,
        Delegate handler)
    {
        var router = builder.MapPut(pattern, handler);
        router.DefaultSettings();
        return router;
    }

    public static RouteHandlerBuilder MapDeleteWithDefaultSettings(
        this IEndpointRouteBuilder builder,
        [StringSyntax("Route")] string pattern,
        Delegate handler)
    {
        var router = builder.MapDelete(pattern, handler);
        router.DefaultSettings();
        return router;
    }
}
