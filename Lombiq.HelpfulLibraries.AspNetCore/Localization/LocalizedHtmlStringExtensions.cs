using Microsoft.AspNetCore.Html;
using Newtonsoft.Json;
using System.IO;
using System.Text.Encodings.Web;

namespace Microsoft.AspNetCore.Mvc.Localization;

public static class LocalizedHtmlStringExtensions
{
    /// <summary>
    /// Returns a raw HTML string that's been JSON serialized and therefore safe to use within <c>&lt;script&gt;</c>
    /// tags in a Razor view.
    /// </summary>
    public static IHtmlContent Json(this LocalizedHtmlString htmlString) =>
        htmlString?.Html() is { } html
            ? new HtmlString(JsonConvert.SerializeObject(
                html,
                new JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeHtml }))
            : new HtmlString("null");

    /// <summary>
    /// Returns a raw HTML string that's not encoded.
    /// </summary>
    public static string Html(this IHtmlContent htmlString)
    {
        if (htmlString == null) return null;

        using var stringWriter = new StringWriter();
        htmlString.WriteTo(stringWriter, HtmlEncoder.Default);
        return stringWriter.ToString();
    }
}
