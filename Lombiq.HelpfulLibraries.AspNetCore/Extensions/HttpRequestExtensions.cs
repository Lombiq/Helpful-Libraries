using Lombiq.HelpfulLibraries.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public static string GetLinkWithAdditionalQuery(this HttpRequest request, string queryString, string key, object value)
    {
        queryString ??= string.Empty;
        if (queryString.StartsWith('?')) queryString = queryString[1..];

        var pageQuery = string.IsNullOrEmpty(queryString)
            ? StringHelper.CreateInvariant($"{key}={value}")
            : StringHelper.CreateInvariant($"&{key}={value}");
        return $"{request.PathBase}{request.Path}?{queryString}{pageQuery}";
    }

    /// <summary>
    /// Returns the current URL but appends a new query string entry.
    /// </summary>
    public static string GetLinkWithAdditionalQuery(this HttpRequest request, string key, object value) =>
        request.GetLinkWithAdditionalQuery(request.QueryString.Value, key, value);

    /// <summary>
    /// Returns the current URL excluding any existing query string entry with the key <paramref name="key"/>, and with
    /// a new <paramref name="key"/>-<paramref name="value"/> entry appended.
    /// </summary>
    public static string GetLinkWithDifferentQuery(this HttpRequest request, string key, object value) =>
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
}
