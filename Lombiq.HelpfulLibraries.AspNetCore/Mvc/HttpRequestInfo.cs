#nullable enable

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System.Collections.Generic;
using System.Text.Json;

namespace Lombiq.HelpfulLibraries.AspNetCore.Mvc;

/// <summary>
/// A simplified intermediate type that can be used to debug <see cref="HttpRequest"/>.
/// </summary>
public record HttpRequestInfo(
    string Url,
    string? ContentType,
    IHeaderDictionary Headers,
    IDictionary<string, string> Form,
    IRequestCookieCollection Cookies)
{
    public HttpRequestInfo(HttpRequest request)
        : this(
            request.GetDisplayUrl(),
            request.ContentType,
            request.Headers,
            request.HasFormContentType ?
                request.Form.ToDictionaryIgnoreCase(pair => pair.Key, pair => pair.Value.ToString()) :
                new Dictionary<string, string>(),
            request.Cookies)
    {
    }

    public override string ToString() => $"HTTP Request at \"{Url}\"";

    public string ToJson() => JsonSerializer.Serialize(this);

    public static string? ToJson(HttpRequest? request) => request == null ? null : new HttpRequestInfo(request).ToJson();
}
