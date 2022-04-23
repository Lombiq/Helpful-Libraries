using Microsoft.AspNetCore.Html;
using Newtonsoft.Json;

namespace System;

public static class JsonStringExtensions
{
    /// <summary>
    /// Returns a raw HTML string that's been JSON serialized and therefore safe to use within <c>&lt;script&gt;</c>
    /// tags in a Razor view.
    /// </summary>
    public static IHtmlContent JsonHtmlContent(this string htmlString) =>
        new HtmlString(JsonConvert.SerializeObject(
            htmlString, new JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeHtml }));
}
