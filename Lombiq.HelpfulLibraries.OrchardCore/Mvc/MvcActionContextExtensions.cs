using OrchardCore.Mvc.Core.Utilities;
using System;
using System.Collections.Generic;
using SettingsAdminController = OrchardCore.Settings.Controllers.AdminController;

namespace Microsoft.AspNetCore.Mvc;

public static class MvcActionContextExtensions
{
    private static readonly string settingsAdminControllerName = typeof(SettingsAdminController).ControllerName();

    /// <summary>
    /// Returns a value indicating whether the requested page matches the provided non-empty route values.
    /// </summary>
    public static bool IsMvcRoute(
        this ActionContext context,
        string action = null,
        string controller = null,
        string area = null)
    {
        var routeValues = context.ActionDescriptor.RouteValues;

        if (!string.IsNullOrEmpty(action) && routeValues["Action"]?.EqualsOrdinalIgnoreCase(action) != true) return false;
        if (!string.IsNullOrEmpty(controller) && routeValues["Controller"]?.EqualsOrdinalIgnoreCase(controller) != true) return false;
        if (!string.IsNullOrEmpty(area) && routeValues["Area"]?.EqualsOrdinalIgnoreCase(area) != true) return false;

        return true;
    }

    /// <summary>
    /// Returns a value indicating whether the requested page is a site setting editor for the provided <paramref
    /// name="groupId"/>.
    /// </summary>
    public static bool IsSiteSettingsPage(this ActionContext context, string groupId) =>
        context.IsMvcRoute(
            nameof(SettingsAdminController.Index),
            settingsAdminControllerName,
            $"{nameof(OrchardCore)}.{nameof(OrchardCore.Settings)}") &&
        context.RouteData.Values.GetMaybe("GroupId")?.ToString() == groupId;
}
