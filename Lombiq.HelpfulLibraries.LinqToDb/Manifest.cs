using OrchardCore.Modules.Manifest;
using static Lombiq.HelpfulLibraries.LinqToDb.Constants.FeatureIds;

[assembly: Module(
    Name = "Lombiq HelpfulLibraries LinqToDb",
    Author = "Lombiq Technologies",
    Version = "1.0",
    Description = "Module for writing linq db queries."
)]

[assembly: Feature(
    Id = Default,
    Name = "Lombiq HelpfulLibraries LinqToDb",
    Category = "Lombiq - HelpfulLibraries",
    Description = "Module for writing linq db queries.",
    Dependencies = new[]
    {
        "OrchardCore.Environment",
        "OrchardCore.Modules",
        "OrchardCore.Abstractions",
        "OrchardCore.Data.Abstractions",
    }
)]
