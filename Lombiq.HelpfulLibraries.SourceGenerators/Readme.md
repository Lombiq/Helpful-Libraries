# Lombiq HelpfulLibraries - Source Generators

## About

A collection of helpful source generators.

- [ConstantFromJsonGenerator.cs](ConstantFromJsonGenerator.cs): A source generator that creates a constant from a JSON file.
- [LibManResourceVersionGenerator.cs](LibManResourceVersionGenerator.cs): Automatically creates constants with the `LibMan_` prefix from libraries entries in the _libman.json file_

You can watch a demo video of the project [here](https://www.youtube.com/watch?v=MluV1ocZGiE).

For general details about and on using the Helpful Libraries see the [root Readme](../Readme.md).

## Documentation

### Using our file-sourced generators

1. Add a JSON file to your project.
2. Set the `Build Action` of the JSON file to `AdditionalFiles` for example:

    ```xml
    <ItemGroup>
        <AdditionalFiles Include="package.json" />
    </ItemGroup>
    ```
3. Add reference to both the Source Generator and the Attributes project (which contains the marker attributes like `[ConstantFromJson]`) and make sure to include the project as analyzer:

    ```xml
    <ProjectReference Include="..\Lombiq.HelpfulLibraries.Attributes\Lombiq.HelpfulLibraries.Attributes.csproj" OutputItemType="Analyzer" ReferenceOutputAssembly="true" />
    <ProjectReference Include="..\Lombiq.HelpfulLibraries.SourceGenerators\Lombiq.HelpfulLibraries.SourceGenerators.csproj" OutputItemType="Analyzer" ReferenceOutputAssembly="false" />
    ```
4. In the samples you can also see the snippet below, while not strictly necessary for the source generator to function, it suppresses a warning that happens in Visual Studio when first cloning the project. If you do decide to include this part make sure you update the relative paths to the correct location of the projects.

    ```xml
    <PropertyGroup>
        <SourceGeneratorLocation>$(SolutionDir)src\Libraries\Lombiq.HelpfulLibraries\Lombiq.HelpfulLibraries.SourceGenerators\bin\Debug\netstandard2.0\Lombiq.HelpfulLibraries.SourceGenerators.dll</SourceGeneratorLocation>
        <SourceGeneratorLocation Condition=" '$(Configuration)' != 'Debug' ">$(SolutionDir)src\Libraries\Lombiq.HelpfulLibraries\Lombiq.HelpfulLibraries.SourceGenerators\bin\Release\netstandard2.0\Lombiq.HelpfulLibraries.SourceGenerators.dll</SourceGeneratorLocation>
    </PropertyGroup> 

    <Target Name="CustomBeforeCompile" BeforeTargets="Compile">
        <MSBuild Condition="!Exists('$(SourceGeneratorLocation)')" Projects="..\Lombiq.HelpfulLibraries.SourceGenerators\Lombiq.HelpfulLibraries.SourceGenerators.csproj" />
    </Target>
    ```

5. Wherever you want to use the JSON file, make sure to use a `partial class`.

### Using the `ConstantFromJsonGenerator`

1. Follow the steps above.
2. Add the `[ConstantFromJsonGenerator]` attribute to your `partial` class.
Where the first parameter is the name of the constant and the second parameter is the path to the JSON file, the last parameter is the name or 'key' for the value we are looking for.

    ```csharp
    [ConstantFromJson("GulpVersion", "package.json", "gulp")]
    public partial class YourClass
    {
    
    }
    ```

3. Run a build and the constant will be generated.
4. Use the constant in your code, full example:

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

### Using the `LibManResourceVersionGenerator`

1. Follow the general steps above.
2. Add the `[LibManVersions]` attribute to your `partial` class.
3. Run a build and an individual constant will be generated for each entry in the `libraries` array of your _libman.json_ file. Each array item is turned into a separate constant using the value of its `library` property. The part after the `@` becomes the value and the part before it becomes the constant's name using the `LibMan_{sanitized}` formula. Here `sanitized` is the value where `.` and `-` characters are turned into `_` and then every other non-alphanumeric characters are stripped out to comply with C# variable naming rules.
4. Use the constant in your code, full example:

    ```csharp
    using System;
    using Generators;
    
    namespace Lombiq.HelpfulLibraries.SourceGenerators.Sample;
    
    [LibManVersions]
    public partial class Examples
    {
        // Show usage of the generated constants
        public void LogVersions()
        {
            Console.WriteLine(LibMan_chart_js); // Outputs "4.5.1". Derived from "chart.js@4.5.1".
            Console.WriteLine(LibMan_chartjs_plugin_annotation); // Outputs "3.1.0". Derived from "chartjs-plugin-annotation@3.1.0".
        }
    }
    ``` 
