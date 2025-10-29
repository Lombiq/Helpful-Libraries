using Lombiq.HelpfulLibraries.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Lombiq.HelpfulLibraries.SourceGenerators;

[Generator]
public class ConstantFromJsonGenerator : GeneratorFromFileBase
{
    protected override Type Attribute => typeof(ConstantFromJsonAttribute);

    protected override void GenerateCode(
        SourceProductionContext context,
        Compilation compilation,
        ImmutableArray<(ClassDeclarationSyntax Syntax, List<Dictionary<string, string>> Dictionary)> classDeclarations,
        IDictionary<string, string> additionalFiles)
    {
        // Go through all filtered class declarations.
        foreach (var (classDeclarationSyntax, attributeData) in classDeclarations)
        {
            // We need to get semantic model of the class to retrieve metadata.
            var semanticModel = compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree);

            // Symbols allow us to get the compile-time information.
            if (semanticModel.GetDeclaredSymbol(classDeclarationSyntax, cancellationToken: context.CancellationToken)
                is not INamedTypeSymbol classSymbol)
            {
                continue;
            }

            var namespaceName = classSymbol.ContainingNamespace.ToDisplayString();

            // 'Identifier' means the token of the node. Get class name from the syntax node.
            var className = classDeclarationSyntax.Identifier.Text;

            var partialBody = new StringBuilder();

            // It's possible that a single class is annotated with our marker attribute multiple times
            foreach (var dictionary in attributeData)
            {
                // Get values from dictionary
                var constantName = dictionary["constantName"].Trim('"');
                var fileName = dictionary["fileName"].Trim('"');
                var propertyName = dictionary["propertyName"].Trim('"');

                // Try get content of file from dictionary where key ends with filename
                var fileContent = additionalFiles
                    .FirstOrDefault(keyValuePair =>
                        keyValuePair.Key.EndsWith(fileName, StringComparison.OrdinalIgnoreCase));

                // If the file content is empty, skip
                if (string.IsNullOrEmpty(fileContent.Value))
                {
                    return;
                }

                var jsonDocument = JsonDocument.Parse(fileContent.Value);

                if (FindProperty(jsonDocument.RootElement, propertyName) is { } jsonValue)
                    partialBody.AppendLine($"public const string {constantName} = \"{jsonValue}\";");
            }

            // Create a new partial class with the same name as the original class.
            // Build up the source code
            var code = CreatePartialBody(namespaceName, className, partialBody.ToString());

            // Add the source code to the compilation.
            context.AddSource($"{className}.g.cs", SourceText.From(code, Encoding.UTF8));
        }
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
