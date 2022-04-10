using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.Common.Utilities;

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
    /// <param name="propertyName">Name of the deep <see cref="JObject"/> node.</param>
    public static void AlterDeep(JObject jObject, Action<string, JObject> alter, string propertyName = null)
    {
        alter(propertyName, jObject);

        foreach (var (key, value) in jObject)
        {
            if (value is JObject innerObject)
            {
                AlterDeep(innerObject, alter, key);
            }

            if (value is JArray innerArray)
            {
                foreach (var innerToken in innerArray)
                {
                    if (innerToken is JObject innerTokenObject)
                    {
                        AlterDeep(innerTokenObject, alter, key);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Alters a <see cref="JObject"/> by iterating through all their inner JObject nodes deeply and executing the
    /// provided asynchronous alter operation on it.
    /// </summary>
    /// <param name="jObject"><see cref="JObject"/> to alter.</param>
    /// <param name="alterAsync">Async operation that alters a deep <see cref="JObject"/> node.</param>
    /// <param name="propertyName">Name of the deep <see cref="JObject"/> node.</param>
    public static async Task AlterDeepAsync(JObject jObject, Func<string, JObject, Task> alterAsync, string propertyName = null)
    {
        await alterAsync(propertyName, jObject);

        foreach (var (key, value) in jObject)
        {
            if (value is JObject innerObject)
            {
                await AlterDeepAsync(innerObject, alterAsync, key);
            }

            if (value is JArray innerArray)
            {
                foreach (var innerToken in innerArray)
                {
                    if (innerToken is JObject innerTokenObject)
                    {
                        await AlterDeepAsync(innerTokenObject, alterAsync, key);
                    }
                }
            }
        }
    }
}
