using Microsoft.AspNetCore.Html;
using Newtonsoft.Json;

namespace Microsoft.AspNetCore.Mvc.Localization
{
    public static class LocalizedHtmlStringExtensions
    {
        /// <summary>
        /// Returns a raw HTML string that's been JSON serialized and therefore safe to use within <c>&lt;script&gt;</c>
        /// tags in a Razor view.
        /// </summary>
        public static IHtmlContent Json(this LocalizedHtmlString htmlString) =>
            new HtmlString(JsonConvert.SerializeObject(htmlString.Value));
    }
}
