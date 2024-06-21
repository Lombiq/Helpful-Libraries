using Microsoft.AspNetCore.Mvc.Localization;
using Shouldly;
using System;
using System.Text.Json;
using Xunit;

namespace Lombiq.HelpfulLibraries.Tests.UnitTests.Converters;

[Obsolete("The previously custom LocalizedHtmlStringConverter is not needed in STJ, this test only exists to show " +
          "that removing it is safe and non-breaking.")]
public class LocalizedHtmlStringConverterTests
{
    private const string Name = "my text";

    private static readonly JsonSerializerOptions _options = JOptions.Default;

    [Theory]
    [InlineData(Name, Name, null, "{\"Name\":\"my text\",\"Value\":\"my text\",\"IsResourceNotFound\":false}")]
    [InlineData(Name, Name, false, "{\"Name\":\"my text\",\"Value\":\"my text\",\"IsResourceNotFound\":false}")]
    [InlineData(Name, Name, true, "{\"Name\":\"my text\",\"Value\":\"my text\",\"IsResourceNotFound\":true}")]
    [InlineData(
        Name,
        "az én szövegem", // #spell-check-ignore-line
        null,
        "{\"Name\":\"my text\",\"Value\":\"az \\u00E9n sz\\u00F6vegem\",\"IsResourceNotFound\":false}")] // #spell-check-ignore-line
    public void LocalizedHtmlStringShouldBeSerializedCorrectly(string name, string value, bool? notFound, string expected)
    {
        var localized = notFound == null
            ? new LocalizedHtmlString(name, value)
            : new LocalizedHtmlString(name, value, notFound.Value);

        JsonSerializer.Serialize(localized, _options).ShouldBe(expected);
    }

    [Theory]
    [InlineData("\"my text\"", Name, Name, false)]
    [InlineData("{ \"name\": \"my text\", \"value\": \"my text\", \"isResourceNotFound\": true }", Name, Name, true)]
    [InlineData("{ \"name\": \"my text\", \"value\": \"some other text\" }", Name, "some other text", false)]
    [InlineData("{ \"value\": \"my text\" }", Name, Name, false)]
    [InlineData("{ \"NAME\": \"my text\" }", Name, Name, false)]
    public void LocalizedHtmlStringShouldBeDeserializedCorrectly(string json, string name, string value, bool notFound)
    {
        var localized = JsonSerializer.Deserialize<LocalizedHtmlString>(json, _options);

        localized.Name.ShouldBe(name);
        localized.Value.ShouldBe(value);
        localized.IsResourceNotFound.ShouldBe(notFound);
    }
}
