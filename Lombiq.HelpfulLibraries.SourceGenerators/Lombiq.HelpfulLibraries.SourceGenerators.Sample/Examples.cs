using Generators;

namespace Lombiq.HelpfulLibraries.SourceGenerators.Sample;

[ConstantFromJson("GulpUglifyVersion", "package.json", "gulp-uglify")]
[ConstantFromJson("GulpVersion", "package.json", "gulp")]
public partial class Examples
{
    // Show usage of the generated constants
    public string ReturnVersions()
    {
        var stringBuilder = new System.Text.StringBuilder();
        stringBuilder.AppendLine($"Gulp version: {GulpVersion}");
        stringBuilder.AppendLine($"Gulp-uglify version: {GulpUglifyVersion}");
        return stringBuilder.ToString();
    }
}
