using Lombiq.HelpfulLibraries.SourceGenerators;
using Lombiq.HelpfulLibraries.Tests.UnitTests.SourceGenerators.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Immutable;
using Xunit;

namespace Lombiq.HelpfulLibraries.Tests.UnitTests.SourceGenerators;

public class ConstantFromJsonTests
{
    private const string TestClassText = """
using Lombiq.HelpfulLibraries.SourceGenerators;

namespace TestNamespace;

[ConstantFromJson(constantName: "GulpVersion", fileName: "package.json", propertyName: "gulp")]
public partial class SourceGeneratorTest
{
}
""";

    private const string JsonText = """
{
  "private": true,
  "devDependencies": {
    "gulp": "3.9.0",
    "gulp-uglify": "1.4.1",
  },
  "dependencies": { }
}
""";

    [Fact]
    public void TestGeneratedConstants()
    {
        // Create an instance of the source generator.
        var generator = new ConstantFromJsonGenerator();

        var driver = CSharpGeneratorDriver.Create(generator);

        var updatedDriver = driver.AddAdditionalTexts([new InMemoryAdditionalText("./package.json", JsonText)]);
        // We need to create a compilation with the required source code.
        var compilation = CSharpCompilation.Create(
            nameof(ConstantFromJsonTests),
            new[] { CSharpSyntaxTree.ParseText(TestClassText) },
            new[]
            {
                // To support 'System.Attribute' inheritance, add reference to 'System.Private.CoreLib'.
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            });

        compilation.AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));

        // Run generators. Don't forget to use the new compilation rather than the previous one.
        updatedDriver.RunGeneratorsAndUpdateCompilation(compilation, out var newCompilation, out _);

        // Retrieve all files in the compilation.
        var generatedFiles = newCompilation.SyntaxTrees;
        Assert.True(generatedFiles.ToImmutableArray().Length == 1);
    }
}
