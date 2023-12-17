using System;
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
}
