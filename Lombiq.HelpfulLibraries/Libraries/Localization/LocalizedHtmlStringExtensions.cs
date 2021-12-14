using Microsoft.AspNetCore.Html;
using Newtonsoft.Json;
using System.IO;
using System.Text.Encodings.Web;

namespace Microsoft.AspNetCore.Mvc.Localization
{
    public static class LocalizedHtmlStringExtensions
    {
        /// <summary>
        /// Returns a raw HTML string that's been JSON serialized and therefore safe to use within <c>&lt;script&gt;</c>
        /// tags in a Razor view.
        /// </summary>
        public static IHtmlContent Json(this LocalizedHtmlString htmlString)
        {
            if (htmlString != null)
            {
                using var stringWriter = new StringWriter();
                htmlString.WriteTo(stringWriter, HtmlEncoder.Default);
                return new HtmlString(JsonConvert.SerializeObject(
                    stringWriter.ToString(),
                    new JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeHtml }));
            }
            else
            {
                return new HtmlString("null");
            }
        }
    }
}
