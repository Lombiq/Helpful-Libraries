using OrchardCore.Modules.Manifest;
using static Lombiq.HelpfulLibraries.LinqToDb.Constants.FeatureIds;

[assembly: Module(
    Name = "LinqToDb - Lombiq HelpfulLibraries",
    Author = "Lombiq Technologies",
    Version = "1.0",
    Description = "Module for writing LINQ to DB queries.",
    Website = "https://github.com/Lombiq/Helpful-Libraries"
)]

[assembly: Feature(
    Id = Default,
    Name = "LinqToDb - Lombiq HelpfulLibraries",
    Category = "Development",
    Description = "Module for writing LINQ to DB queries.",
    Dependencies = new[]
    {
        "OrchardCore.Environment",
        "OrchardCore.Modules",
        "OrchardCore.Abstractions",
        "OrchardCore.Data.Abstractions",
    }
)]
