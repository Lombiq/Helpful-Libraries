using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

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
        /// Alters a <see cref="JObject"/> by iterating through all their inner JObject nodes deeply and executing the
        /// provided alter operation on it.
        /// </summary>
        /// <param name="jObject"><see cref="JObject"/> to alter.</param>
        /// <param name="alter">Operation that alters a deep <see cref="JObject"/> node.</param>
        public static void AlterDeep(JObject jObject, Action<JObject> alter)
        {
            alter(jObject);

            foreach (var (_, value) in jObject)
            {
                if (value is JObject innerObject)
                {
                    AlterDeep(innerObject, alter);
                }

                if (value is JArray innerArray)
                {
                    foreach (var innerToken in innerArray)
                    {
                        if (innerToken is JObject innerTokenObject)
                        {
                            AlterDeep(innerTokenObject, alter);
                        }
                    }
                }
            }
        }
    }
}
