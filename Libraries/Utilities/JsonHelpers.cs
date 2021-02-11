using Microsoft.AspNetCore.Html;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Lombiq.HelpfulLibraries.Libraries.Utilities
{
    public static class JsonHelpers
    {
        public static bool ValidateJsonIfNotNull(string json)
        {
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    JObject.Parse(json);
                    return true;
                }
                catch (JsonReaderException)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns a raw HTML string that's been JSON serialized and therefore safe to use within <c>&lt;script&gt;</c>
        /// tags in a Razor view.
        /// </summary>
        public static IHtmlContent JsonHtmlContent(this string htmlString) =>
            new HtmlString(JsonConvert.SerializeObject(htmlString));
    }
}
