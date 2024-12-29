using Lombiq.HelpfulLibraries.Attributes;

namespace Lombiq.HelpfulLibraries.Tests.Models;

/// <summary>
/// Shows how to use the <see cref="ConstantFromJsonAttribute" />.
/// </summary>
/// <remarks>
/// <para>
/// Using two really simple packages as a test.
/// </para>
/// </remarks>
[ConstantFromJson(constantName: "IsEvenVersion", fileName: "package.json", propertyName: "is-even")]
[ConstantFromJson(constantName: "IsOddVersion", fileName: "package.json", propertyName: "is-odd")]
public partial class ConstantFromJsonSample
{
    public string ReturnVersions()
    {
        var stringBuilder = new System.Text.StringBuilder();
        stringBuilder.AppendLine($"is-even version: {IsEvenVersion}");
        stringBuilder.AppendLine($"is-odd version: {IsOddVersion}");
        return stringBuilder.ToString();
    }
}
