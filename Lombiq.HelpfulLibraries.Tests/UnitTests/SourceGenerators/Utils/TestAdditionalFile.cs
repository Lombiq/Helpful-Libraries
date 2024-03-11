using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Threading;

namespace Lombiq.HelpfulLibraries.Tests.UnitTests.SourceGenerators.Utils;

internal sealed class InMemoryAdditionalText(string path, string text) : AdditionalText
{
    private readonly SourceText _text = SourceText.From(text);

    public override SourceText GetText(CancellationToken cancellationToken = default) => _text;

    public override string Path { get; } = path;
}
