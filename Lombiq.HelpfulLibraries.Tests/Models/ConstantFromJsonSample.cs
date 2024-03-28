using Lombiq.HelpfulLibraries.SourceGenerators.Attributes;

namespace Lombiq.HelpfulLibraries.Tests.Models;

/// <summary>
/// Shows how to use the <see cref="ConstantFromJsonAttribute" />.
/// </summary>
[ConstantFromJson(constantName: "GulpUglifyVersion", fileName: "package.json", propertyName: "gulp-uglify")]
[ConstantFromJson(constantName: "GulpVersion", fileName: "package.json", propertyName: "gulp")]
public partial class ConstantFromJsonSample
{
    public string ReturnVersions()
    {
        var stringBuilder = new System.Text.StringBuilder();
        stringBuilder.AppendLine($"Gulp version: {GulpVersion}");
        stringBuilder.AppendLine($"Gulp-uglify version: {GulpUglifyVersion}");
        return stringBuilder.ToString();
    }
}
