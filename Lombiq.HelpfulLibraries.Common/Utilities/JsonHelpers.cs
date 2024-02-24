using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.Common.Utilities;

public static class JsonHelpers
{
    /// <summary>
    /// Attempts to validate a string that contains JSON by parsing it.
    /// </summary>
    /// <returns>
    /// <see langword="null"/> if the string is null or empty, <see langword="true"/> if parsing was successful,
    /// <see langword="false"/> otherwise.
    /// </returns>
    public static bool? ValidateJsonIfNotNull(string json)
    {
        if (string.IsNullOrEmpty(json)) return null;
        return TryParse(json, out _);
    }

    /// <summary>
    /// Attempts to validate a string that contains JSON by parsing it.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if parsing was successful, <see langword="false"/> otherwise.
    /// </returns>
    public static bool TryParse(string json, out JsonNode result)
    {
        try
        {
            result = JsonNode.Parse(json);
            return true;
        }
        catch (JsonException)
        {
            result = default;
            return false;
        }
    }

    /// <summary>
    /// Walks through the properties of <paramref name="jsonObject"/> recursively and invokes <paramref name="alter"/>
    /// on each property with an object value.
    /// </summary>
    /// <param name="alter">
    /// The action to be invoked. The first parameter is the current property name, the second is its value.
    /// </param>
    public static void AlterDeep(JsonObject jsonObject, Action<string, JsonObject> alter) =>
        AlterDeep(jsonObject, alter, propertyName: null);

    private static void AlterDeep(JsonObject jsonObject, Action<string, JsonObject> alter, string propertyName)
    {
        if (propertyName != null) alter(propertyName, jsonObject);

        foreach (var (key, value) in jsonObject)
        {
            if (value is JsonObject innerObject)
            {
                AlterDeep(innerObject, alter, key);
            }
            else if (value is JsonArray innerArray)
            {
                innerArray
                    .CastWhere<JsonObject>()
                    .ForEach(innerTokenObject => AlterDeep(innerTokenObject, alter, key));
            }
        }
    }

    /// <summary>
    /// Walks through the properties of <paramref name="jsonObject"/> recursively and invokes <paramref
    /// name="alterAsync"/> on each property with an object value.
    /// </summary>
    /// <param name="alterAsync">
    /// The asynchronous operation to be invoked. The first parameter is the current property name, the second is its
    /// value.
    /// </param>
    public static Task AlterDeepAsync(JsonObject jsonObject, Func<string, JsonObject, Task> alterAsync) =>
        AlterDeepAsync(jsonObject, alterAsync, propertyName: null);

    public static async Task AlterDeepAsync(JsonObject jsonObject, Func<string, JsonObject, Task> alterAsync, string propertyName)
    {
        if (propertyName != null) await alterAsync(propertyName, jsonObject);

        foreach (var (key, value) in jsonObject)
        {
            if (value is JsonObject innerObject)
            {
                await AlterDeepAsync(innerObject, alterAsync, key);
            }
            else if (value is JsonArray innerArray)
            {
                await innerArray
                    .CastWhere<JsonObject>()
                    .AwaitEachAsync(innerTokenObject => AlterDeepAsync(innerTokenObject, alterAsync, key));
            }
        }
    }
}
