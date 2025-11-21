using Lombiq.HelpfulLibraries.Attributes;
using Microsoft.CodeAnalysis;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Lombiq.HelpfulLibraries.SourceGenerators;

[Generator]
public class LibManResourceVersionGenerator : GeneratorFromFileBase
{
    protected override Type Attribute => typeof(LibManVersionsAttribute);

    protected override string? GenerateCode(GeneratorFromFileModel model)
    {
        var fileContent = model
            .AttributeData
            .Select(dictionary => dictionary.TryGetValue("fileName", out var fileNameValue)
                ? fileNameValue.Trim('"')
                : "libman.json")
            .Select(model.GetFileContent)
            .FirstOrDefault(text => !string.IsNullOrWhiteSpace(text));
        if (fileContent == null) return null;

        var jsonDocument = JsonDocument.Parse(fileContent);
        return CreateSubclassConstants(
            model.NamespaceName,
            model.ClassName,
            "LibManVersions",
            jsonDocument
                .RootElement
                .GetProperty("libraries")
                .EnumerateArray()
                .Select(arrayItem => ParseLibraryExpression(arrayItem.GetProperty("library").GetString()))
                .Where(pair => !string.IsNullOrWhiteSpace(pair.Value)));
    }

    private static (string Name, string Value) ParseLibraryExpression(string? library)
    {
        var index = library?.IndexOf('@') ?? -1;
        return index <= 0
            ? default
            : (Name: SanitizeToApproximatePascalCase(library!.Substring(0, index)), Value: library.Substring(index + 1));
    }

    [SuppressMessage("Security", "MA0009:Add regex evaluation timeout", Justification = "Not applicable here.")]
    private static string SanitizeToApproximatePascalCase(string text)
    {
        var words = Regex.Replace(text, "[^a-zA-Z0-9]+", " ").Trim();

        return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(words).Replace(" ", string.Empty);
    }
}
