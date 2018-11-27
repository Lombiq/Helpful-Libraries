using Lombiq.HelpfulLibraries;
using OrchardCore.Modules.Manifest;

[assembly: Module(
    Name = "Helpful Libraries",
    Author = "Lombiq",
    Website = "https://github.com/Lombiq/Helpful-Libraries",
    Version = "0.0.1",
    Description = "Various libraries that can be handy when developing for Orchard.",
    Category = "Developer"
)]

[assembly: Feature(
    Id = FeatureIds.DependencyInjection,
    Name = "Dependency Injection Libraries - Helpful Libraries",
    Category = "Developer",
    Description = "Libraries aiding dependency injection."
)]