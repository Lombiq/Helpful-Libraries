using Microsoft.AspNetCore.Mvc.Localization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Lombiq.HelpfulLibraries.AspNetCore.Localization;

public class LocalizedHtmlStringConverter : JsonConverter<LocalizedHtmlString>
{
    public override void WriteJson(JsonWriter writer, LocalizedHtmlString value, JsonSerializer serializer)
    {
        writer.WriteStartObject();

        writer.WritePropertyName(nameof(LocalizedHtmlString.Name));
        writer.WriteValue(value.Name);

        writer.WritePropertyName(nameof(LocalizedHtmlString.Value));
        writer.WriteValue(value.Html());

        writer.WritePropertyName(nameof(LocalizedHtmlString.IsResourceNotFound));
        writer.WriteValue(value.IsResourceNotFound);

        writer.WriteEndObject();
    }

    public override LocalizedHtmlString ReadJson(
        JsonReader reader,
        Type objectType,
        LocalizedHtmlString existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
    {
        var token = JToken.Load(reader);

        if (token.Type == JTokenType.String)
        {
            var text = token.Value<string>();
            return new LocalizedHtmlString(text, text);
        }

        if (token is JObject jObject)
        {
            var name = jObject.GetMaybe(nameof(LocalizedHtmlString.Name))?.ToObject<string>();
            var value = jObject.GetMaybe(nameof(LocalizedHtmlString.Value))?.ToObject<string>() ?? name;
            var isResourceNotFound = jObject.GetMaybe(nameof(LocalizedHtmlString.IsResourceNotFound))?.ToObject<bool>();

            name ??= value;
            if (string.IsNullOrEmpty(name)) throw new InvalidOperationException("Missing name.");

            return new LocalizedHtmlString(name, value, isResourceNotFound == true);
        }

        throw new InvalidOperationException($"Can't parse token \"{token}\". It should be an object or a string");
    }
}
