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

/// <summary>
/// A generator that exposes a value from a JSON file at compile time.
/// The target class should be annotated with the 'Generators.ConstantFromJsonAttribute' attribute.
/// </summary>
[Generator]
public class ConstantFromJsonGenerator : IIncrementalGenerator
{
    private const string AttributeName = nameof(ConstantFromJsonAttribute);
    private static readonly string? Namespace = typeof(ConstantFromJsonAttribute).Namespace;

    private readonly Dictionary<string, string> _fileContents = [];

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Filter classes annotated with the [ConstantFromJson] attribute.
        // Only filtered Syntax Nodes can trigger code generation.
        var provider = context.SyntaxProvider
            .CreateSyntaxProvider(
                (node, _) => node is ClassDeclarationSyntax,
                (syntaxContext, _) => GetClassDeclarationForSourceGen(syntaxContext))
            .Where(tuple => tuple.ReportAttributeFound)
            .Select((tuple, _) => (tuple.Syntax, tuple.AttributesData));

        var additionalFiles = context.AdditionalTextsProvider
            .Where(static file => file.Path.EndsWith(".json", StringComparison.OrdinalIgnoreCase));

        var namesAndContents = additionalFiles
            .Select((file, cancellationToken) =>
                (Content: file.GetText(cancellationToken)?.ToString(),
                    file.Path));

        context.RegisterSourceOutput(namesAndContents.Collect(), (_, contents) =>
        {
            foreach ((string? content, string path) in contents)
            {
                // Add to the dictionary
                _fileContents.Add(path, content ?? string.Empty);
            }
        });

        // Generate the source code.
        context.RegisterSourceOutput(
            context.CompilationProvider.Combine(provider.Collect()),
            (productionContext, tuple) => GenerateCode(productionContext, tuple.Left, tuple.Right));
    }

    /// <summary>
    /// Checks whether the Node is annotated with the [ConstantFromJson] attribute and maps syntax context to
    /// the specific node type (ClassDeclarationSyntax).
    /// </summary>
    /// <param name="context">Syntax context, based on CreateSyntaxProvider predicate.</param>
    /// <returns>The specific cast and whether the attribute was found.</returns>
    private static (ClassDeclarationSyntax Syntax, bool ReportAttributeFound, List<Dictionary<string, string>> AttributesData)
        GetClassDeclarationForSourceGen(GeneratorSyntaxContext context)
    {
        var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;
        var attributesData = classDeclarationSyntax.AttributeLists
            .SelectMany(list => list.Attributes)
            .Select(attributeSyntax => GetAttributeArguments(context, attributeSyntax))
            .OfType<Dictionary<string, string>>().ToList();

        return (classDeclarationSyntax, attributesData.Count > 0, attributesData);
    }

    private static Dictionary<string, string>? GetAttributeArguments(GeneratorSyntaxContext context, AttributeSyntax attributeSyntax)
    {
        if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
        {
            return null; // if we can't get the symbol, ignore it
        }

        var attributeName = attributeSymbol.ContainingType.ToDisplayString();
        // Check the full name of the [ConstantFromJson] attribute.
        if (attributeName != $"{Namespace}.{AttributeName}")
        {
            return null;
        }

        var arguments = attributeSyntax.ArgumentList?.Arguments
            .Select(argument => argument.Expression)
            .OfType<LiteralExpressionSyntax>()
            .Select((literalExpression, index) => new
            {
                Key = attributeSymbol.Parameters[index].Name,
                Value = literalExpression.Token.Text,
            })
            .ToDictionary(keyValuePair => keyValuePair.Key, keyValuePair => keyValuePair.Value) ?? [];

        return arguments;
    }

    /// <summary>
    /// Generate code action.
    /// It will be executed on specific nodes (ClassDeclarationSyntax annotated with the [ConstantFromJson] attribute)
    /// changed by the user.
    /// </summary>
    /// <param name="context">Source generation context used to add source files.</param>
    /// <param name="compilation">Compilation used to provide access to the Semantic Model.</param>
    /// <param name="classDeclarations">
    /// Nodes annotated with the [ConstantFromJson] attribute that trigger the
    /// generate action.
    /// </param>
    private void GenerateCode(
        SourceProductionContext context,
        Compilation compilation,
        ImmutableArray<(ClassDeclarationSyntax Syntax, List<Dictionary<string, string>> Dictionary)> classDeclarations)
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
                var fileContent = _fileContents
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
            var code = $@"// <auto-generated/>

using System;
using System.Collections.Generic;

namespace {namespaceName};

partial class {className}
{{
    {partialBody}
}}
";
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

            if (property.Value.ValueKind == JsonValueKind.Object)
            {
                var result = FindProperty(property.Value, propertyName);
                if (result != null)
                {
                    return result;
                }
            }
            else if (property.Value.ValueKind == JsonValueKind.Array)
            {
                var result = property.Value.EnumerateArray()
                    .Where(arrayElement => arrayElement.ValueKind == JsonValueKind.Object)
                    .Select(arrayElement => FindProperty(arrayElement, propertyName))
                    .FirstOrDefault(jsonProperty => jsonProperty != null);

                if (result != null)
                {
                    return result;
                }
            }
        }

        return null;
    }
}
