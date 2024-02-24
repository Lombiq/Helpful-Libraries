using Microsoft.AspNetCore.Html;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Microsoft.AspNetCore.Mvc.Localization;

public static class LocalizedHtmlStringExtensions
{
    /// <summary>
    /// Returns a raw HTML string that's been JSON serialized and therefore safe to use within <c>&lt;script&gt;</c>
    /// tags in a Razor view.
    /// </summary>
    public static IHtmlContent Json(this LocalizedHtmlString htmlString) =>
        htmlString?.Html() is { } html
            ? new HtmlString(JsonSerializer.Serialize(html))
            : new HtmlString("null");

    /// <summary>
    /// Returns a raw HTML string representation of the <paramref name="htmlContent"/>.
    /// </summary>
    public static string Html(this IHtmlContent htmlContent)
    {
        if (htmlContent == null) return null;

        using var stringWriter = new StringWriter();
        htmlContent.WriteTo(stringWriter, HtmlEncoder.Default);
        return stringWriter.ToString();
    }

    /// <summary>
    /// Concatenates the <paramref name="first"/> localized HTML string with the <paramref name="other"/> provided HTML
    /// content. This is suitable for joining individually localizable HTML strings.
    /// </summary>
    public static LocalizedHtmlString Concat(this LocalizedHtmlString first, params IHtmlContent[] other)
    {
        if (other.Length == 0) return first;

        var builder = new StringBuilder(first.Html());
        foreach (var content in other) builder.Append(content.Html());
        var html = builder.ToString().Replace("{", "{{").Replace("}", "}}");

        return new LocalizedHtmlString(html, html);
    }

    /// <summary>
    /// Concatenates the <paramref name="items"/> with the provided <paramref name="separator"/> in-between.
    /// </summary>
    public static LocalizedHtmlString Join(this IHtmlContent separator, params LocalizedHtmlString[] items)
    {
        if (items.Length == 0) return null;

        var first = items[0];
        var other = items.Skip(1).SelectMany(item => new[] { separator, item }).ToArray();

        return first.Concat(other);
    }
}
