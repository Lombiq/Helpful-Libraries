using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using OrchardCore.Mvc.Core.Utilities;
using System;
using System.Collections.Generic;
using SettingsAdminController = OrchardCore.Settings.Controllers.AdminController;

namespace Microsoft.AspNetCore.Mvc;

public static class MvcActionContextExtensions
{
    private static readonly string _settingsAdminControllerName = typeof(SettingsAdminController).ControllerName();

    /// <summary>
    /// Returns a value indicating whether the requested page matches the provided non-empty route values.
    /// </summary>
    public static bool IsMvcRoute(
        this ActionContext context,
        string? action = null,
        string? controller = null,
        string? area = null)
    {
        var routeValues = new Dictionary<string, string?>(
            context.ActionDescriptor.RouteValues,
            StringComparer.OrdinalIgnoreCase);

        return
            IsMatch(routeValues, "Action", action) &&
            IsMatch(routeValues, "Controller", controller) &&
            IsMatch(routeValues, "Area", area);
    }

    /// <summary>
    /// Returns a value indicating whether the requested page matches the provided non-empty route values.
    /// </summary>
    public static bool IsMvcRoute(
        this HttpContext context,
        string? action = null,
        string? controller = null,
        string? area = null)
    {
        if (context.GetEndpoint()?.Metadata.GetMetadata<ActionDescriptor>()?.RouteValues is not { } routeValues)
        {
            return false;
        }

        // Make it case-insensitive.
        routeValues = new Dictionary<string, string?>(routeValues, StringComparer.OrdinalIgnoreCase);

        return
            IsMatch(routeValues, "Action", action) &&
            IsMatch(routeValues, "Controller", controller) &&
            IsMatch(routeValues, "Area", area);
    }

    /// <summary>
    /// Returns a value indicating whether the requested page is a site setting editor for the provided <paramref
    /// name="groupId"/>.
    /// </summary>
    public static bool IsSiteSettingsPage(this ActionContext context, string? groupId) =>
        context.IsMvcRoute(
            nameof(SettingsAdminController.Index),
            _settingsAdminControllerName,
            $"{nameof(OrchardCore)}.{nameof(OrchardCore.Settings)}") &&
        context.RouteData.Values.GetMaybe("GroupId")?.ToString() == groupId;

    private static bool IsMatch(IDictionary<string, string?> routeValues, string key, string? expected) =>
        expected == null ||
        (routeValues.TryGetValue(key, out var value) && (expected?.EqualsOrdinalIgnoreCase(value) ?? value is null));
}
