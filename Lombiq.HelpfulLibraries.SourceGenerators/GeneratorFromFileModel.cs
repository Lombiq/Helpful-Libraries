using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lombiq.HelpfulLibraries.SourceGenerators;

public class GeneratorFromFileModel
{
    public ClassDeclarationSyntax Syntax { get; }
    public IList<Dictionary<string, string>> AttributeData { get; }
    public SourceProductionContext Context { get; }
    public Compilation Compilation { get; }
    public IDictionary<string, string> AdditionalFiles { get; }
    public INamedTypeSymbol? ClassSymbol { get; }
    public string ClassName { get; }
    public string NamespaceName { get; }

    public GeneratorFromFileModel(
        ClassDeclarationSyntax syntax,
        IList<Dictionary<string, string>> attributeData,
        SourceProductionContext context,
        Compilation compilation,
        IDictionary<string, string> additionalFiles)
    {
        Syntax = syntax;
        AttributeData = attributeData;
        Context = context;
        Compilation = compilation;
        AdditionalFiles = additionalFiles;

        ClassSymbol = compilation
            .GetSemanticModel(syntax.SyntaxTree)
            .GetDeclaredSymbol(syntax, cancellationToken: context.CancellationToken) as INamedTypeSymbol;
        ClassName = syntax.Identifier.Text;
        NamespaceName = ClassSymbol?.ContainingNamespace.ToDisplayString() ?? string.Empty;
    }

    public string GetFileContent(string fileName) =>
        AdditionalFiles
            .FirstOrDefault(keyValuePair => keyValuePair.Key.EndsWith(fileName, StringComparison.OrdinalIgnoreCase))
            .Value;
}
