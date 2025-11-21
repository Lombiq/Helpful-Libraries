using Lombiq.HelpfulLibraries.Attributes;
using Microsoft.CodeAnalysis;
using System;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Lombiq.HelpfulLibraries.SourceGenerators;

[Generator]
public class ConstantFromJsonGenerator : GeneratorFromFileBase
{
    protected override Type Attribute => typeof(ConstantFromJsonAttribute);

    protected override string? GenerateCode(GeneratorFromFileModel model)
    {
        var partialBody = new StringBuilder();

        // It's possible that a single class is annotated with our marker attribute multiple times
        foreach (var dictionary in model.AttributeData)
        {
            // Get values from dictionary
            var constantName = dictionary["constantName"].Trim('"');
            var fileName = dictionary["fileName"].Trim('"');
            var propertyName = dictionary["propertyName"].Trim('"');

            // If the file content is empty, skip
            if (model.GetFileContent(fileName) is not { Length: > 0 } fileContent) return null;

            var jsonDocument = JsonDocument.Parse(fileContent);

            if (FindProperty(jsonDocument.RootElement, propertyName) is { } jsonValue)
                partialBody.AppendLine($"public const string {constantName} = \"{jsonValue}\";");
        }

        // Create a new partial class with the same name as the original class.
        // Build up the source code
        return CreatePartialBody(model.NamespaceName, model.ClassName, partialBody.ToString());
    }

    /// <summary>
    /// Find a property in a JSON document recursively.
    /// </summary>
    /// <param name="element">The JSON element to search in.</param>
    /// <param name="propertyName">The property name to look for.</param>
    private static JsonElement? FindProperty(JsonElement element, string propertyName)
    {
        foreach (var property in element.EnumerateObject())
        {
            if (property.Name == propertyName)
            {
                return property.Value;
            }

            var result = property.Value.ValueKind switch
            {
                JsonValueKind.Object => FindProperty(property.Value, propertyName),
                JsonValueKind.Array => property.Value.EnumerateArray()
                    .Where(arrayElement => arrayElement.ValueKind == JsonValueKind.Object)
                    .Select(arrayElement => FindProperty(arrayElement, propertyName))
                    .FirstOrDefault(jsonProperty => jsonProperty != null),
                _ => null,
            };

            if (result != null)
            {
                return result;
            }
        }

        return null;
    }
}
