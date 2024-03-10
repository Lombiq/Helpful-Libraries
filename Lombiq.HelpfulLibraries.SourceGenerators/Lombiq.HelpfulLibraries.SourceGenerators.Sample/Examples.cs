using Generators;
using System;

namespace Lombiq.HelpfulLibraries.SourceGenerators.Sample;

[ConstantFromJson("GulpUglifyVersion", "package.json", "gulp-uglify")]
[ConstantFromJson("GulpVersion", "package.json", "gulp")]
public partial class Examples
{
    // Show usage of the generated constants
    public void LogVersions()
    {
        Console.WriteLine(GulpUglifyVersion);
        Console.WriteLine(GulpVersion);
    }
}
