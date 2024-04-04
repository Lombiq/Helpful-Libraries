# Lombiq HelpfulLibraries - Source Generators

## About

A collection of helpful source generators.
> ⚠ When using one of the generators you must run a build before errors will go away.

- [ConstantFromJsonGenerator.cs](ConstantFromJsonGenerator.cs): A source generator that creates a constant from a JSON file.

For general details about and on using the Helpful Libraries see the [root Readme](../Readme.md).

## Documentation

### How to use the `ConstantFromJsonGenerator`?

1. Add a JSON file to your project.
2. Set the `Build Action` of the JSON file to `AdditionalFiles` for example:

```xml
<ItemGroup>
    <AdditionalFiles Include="package.json" />
</ItemGroup>
```

3. Wherever you want to use the JSON file, make sure to use a `partial class` and add the `ConstantFromJsonGenerator` attribute to it.
Where the first parameter is the name of the constant and the second parameter is the path to the JSON file, the last parameter is the name or 'key' for the value we are looking for.

```csharp
[ConstantFromJson("GulpVersion", "package.json", "gulp")]
public partial class YourClass
{

}
```

4. Run a build and the constant will be generated .
5. Use the constant in your code, full example:

```csharp
using System;
using Generators;

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
```
