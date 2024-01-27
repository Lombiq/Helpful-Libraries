using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.IO;
using System.Web;

namespace Microsoft.AspNetCore.Mvc.Rendering;

public static class JsonHelperExtensions
{
    /// <summary>
    /// Returns a full HTML element attribute withe the given <paramref name="name"/> prefixed with <c>data-</c> and the
    /// value appropriately encoded.
    /// </summary>
    public static IHtmlContent DataAttribute(this IJsonHelper helper, string name, object value)
    {
        using var stringWriter = new StringWriter();
        helper.Serialize(value).WriteTo(stringWriter, NullHtmlEncoder.Default);

        return new HtmlString($"data-{name}=\"{HttpUtility.HtmlAttributeEncode(stringWriter.ToString())}\"");
    }
}
