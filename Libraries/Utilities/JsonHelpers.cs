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
    }
}
