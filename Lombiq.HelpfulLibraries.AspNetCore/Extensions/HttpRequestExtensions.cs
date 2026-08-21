using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace Microsoft.AspNetCore.Http;

public static class HttpRequestExtensions
{
    /// <summary>
    /// Returns the query string without the leading <c>?</c> and without the keys in <paramref name="keysToExclude"/>.
    /// </summary>
    public static string GetQueryWithout(this HttpRequest request, params string[] keysToExclude)
    {
        var query = HttpUtility.ParseQueryString(request.QueryString.Value ?? string.Empty);
        return string.Join('&', query
            .AllKeys
            .Where(key => key != null && !keysToExclude.Exists(key.EqualsOrdinalIgnoreCase))
            .Select(key => $"{HttpUtility.UrlEncode(key)}={HttpUtility.UrlEncode(query[key] ?? string.Empty)}"));
    }

    /// <summary>
    /// Returns the current URL but appends a new query string entry.
    /// </summary>
    public static string GetLinkWithAdditionalQuery(this HttpRequest request, string? queryString, string key, object? value)
    {
        queryString ??= string.Empty;
        if (queryString.StartsWith('?')) queryString = queryString[1..];

        var pageQuery = string.IsNullOrEmpty(queryString)
            // MA0185 doesn't apply: value is an arbitrary object that can hold culture-sensitive data at runtime.
#pragma warning disable MA0185
            ? string.Create(CultureInfo.InvariantCulture, $"{key}={value}")
            : string.Create(CultureInfo.InvariantCulture, $"&{key}={value}");
#pragma warning restore MA0185
        return $"{request.PathBase}{request.Path}?{queryString}{pageQuery}";
    }

    /// <summary>
    /// Returns the current URL but appends a new query string entry.
    /// </summary>
    public static string GetLinkWithAdditionalQuery(this HttpRequest request, string key, object? value) =>
        request.GetLinkWithAdditionalQuery(request.QueryString.Value, key, value);

    /// <summary>
    /// Returns the current URL excluding any existing query string entry with the key <paramref name="key"/>, and with
    /// a new <paramref name="key"/>-<paramref name="value"/> entry appended.
    /// </summary>
    public static string GetLinkWithDifferentQuery(this HttpRequest request, string key, object? value) =>
        request.GetLinkWithAdditionalQuery(request.GetQueryWithout(key), key, value);

    /// <summary>
    /// Returns the current URL but with the value of the query string entry of with the key <paramref name="key"/>
    /// cycled to the next item in the <paramref name="values"/>. This can be useful for example to generate table
    /// header links that cycle ascending-descending-unsorted sorting by that column.
    /// </summary>
    public static string GetLinkAndCycleQueryValue(this HttpRequest request, string key, params string[] values)
    {
        var query = HttpUtility.ParseQueryString(request.QueryString.Value ?? string.Empty);

        var value = query[key] ?? string.Empty;
        var index = ((IList<string>)values).IndexOf(value);
        var newValue = values[(index + 1) % values.Length];

        return request.GetLinkWithDifferentQuery(key, newValue);
    }

    /// <summary>
    /// Checks if the <paramref name="area"/>, <paramref name="controller"/> and <paramref name="action"/> route values
    /// match the provided arguments.
    /// </summary>
    public static bool IsAction(this HttpRequest request, string? controller, string? action, string? area = null)
    {
        var values = request.RouteValues;
        return (string.IsNullOrEmpty(controller) || values.GetMaybe(nameof(controller))?.ToString()?.EqualsOrdinalIgnoreCase(controller) == true) &&
               (string.IsNullOrEmpty(action) || values.GetMaybe(nameof(action))?.ToString()?.EqualsOrdinalIgnoreCase(action) == true) &&
               (string.IsNullOrEmpty(area) || values.GetMaybe(nameof(area))?.ToString()?.EqualsOrdinalIgnoreCase(area) == true);
    }

    /// <summary>
    /// Checks if the <paramref name="area"/>, <c>controller</c> and <paramref name="action"/> route values match the
    /// provided arguments.
    /// </summary>
    public static bool IsAction<TController>(this HttpRequest request, string? action, string? area = null)
        where TController : ControllerBase
    {
        var controllerType = typeof(TController);
        var controllerName = controllerType.Name.EndsWith(nameof(Controller), StringComparison.OrdinalIgnoreCase)
            ? controllerType.Name[..^nameof(Controller).Length]
            : controllerType.Name;

        return request.IsAction(controllerName, action, area);
    }

    /// <summary>
    /// Checks if the <c>controller</c> and <c>action</c> route values match the <paramref name="actionSelector"/>.
    /// Optionally, checks if the provided <paramref name="area"/> matches the route value of the same name as well.
    /// </summary>
    public static bool IsAction<TController>(
        this HttpRequest request,
        Expression<Action<TController>> actionSelector,
        string? area = null)
        where TController : ControllerBase
    {
        var action = actionSelector.GetMethodCallInfo().Method.Name;
        return request.IsAction<TController>(action, area);
    }

    /// <inheritdoc cref="IsAction{TController}(HttpRequest,Expression{Action{TController}}, string)"/>
    public static bool IsAction<TController>(
        this HttpRequest request,
        Expression<Func<TController, Task>> actionSelector,
        string? area = null)
        where TController : ControllerBase =>
        request.IsAction(actionSelector.StripResult(), area);

    /// <summary>
    /// If the <paramref name="request"/> has a form body, it tries to find the value for <paramref name="key"/> amd
    /// trims it. If that fails or if the result is an empty string, <see langword="null"/> is returned instead.
    /// </summary>
    public static string? GetFormValueMaybe(this HttpRequest request, string key)
    {
        if (!request.HasFormContentType) return null;

        // We use try-catch in case the request is somehow broken or invalid because then just accessing the form can
        // throw an exception.
        try
        {
            return request.Form.TryGetValue(key, out var values) &&
                values.WhereNot(string.IsNullOrWhiteSpace).FirstOrDefault()?.Trim() is { } value
                    ? value
                    : null;
        }
        catch
        {
            return null;
        }
    }
}
