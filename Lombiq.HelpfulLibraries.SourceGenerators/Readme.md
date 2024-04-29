# Lombiq HelpfulLibraries - Source Generators

## About

A collection of helpful source generators.

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
3. Add reference to both the Source Generator as well as the Attributes project (this adds the marker attribute 'ConstantFromJson') and make sure to include the project as analyzer:
    ```xml
    <ProjectReference Include="..\Lombiq.HelpfulLibraries.Attributes\Lombiq.HelpfulLibraries.Attributes.csproj" OutputItemType="Analyzer" ReferenceOutputAssembly="true" />
    <ProjectReference Include="..\Lombiq.HelpfulLibraries.SourceGenerators\Lombiq.HelpfulLibraries.SourceGenerators.csproj" OutputItemType="Analyzer" ReferenceOutputAssembly="false" />
    ```

4. Wherever you want to use the JSON file, make sure to use a `partial class` and add the `ConstantFromJsonGenerator` attribute to it.
Where the first parameter is the name of the constant and the second parameter is the path to the JSON file, the last parameter is the name or 'key' for the value we are looking for.

    ```csharp
    [ConstantFromJson("GulpVersion", "package.json", "gulp")]
    public partial class YourClass
    {
    
    }
    ```

5. Run a build and the constant will be generated .
6. Use the constant in your code, full example:

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
