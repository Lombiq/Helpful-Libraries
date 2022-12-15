# Lombiq Helpful Libraries - Orchard Core Libraries - Resource Management for Orchard Core

## Resource Filter

Makes it possible to include resources automatically based on the current context, e.g. allows only injecting home page styling when the home page is being loaded.

Usage:

Activate the resource filter middleware by adding `app.UseResourceFilters()` to the `Configure()` method of the Startup file located in a common module or the web project:

```C#
public override void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
{
    app.UseResourceFilters();
}
```

To add resource filters, the `IResourceFilterProvider` interface needs to be implemented and the registration needs to be added to the service collection as well.

Example:

```C#
public class ResourceFilters : IResourceFilterProvider
{
    public void AddResourceFilter(ResourceFilterBuilder builder)
    {
        builder.WhenHomePage().RegisterHeadScript("HomePageStyle");
        builder.WhenPath("/my-page").RegisterHeadScript("MyPageScript");
    }
}
```

## Extensions

- `ApplicationBuilderExtensions`: Shortcut extensions for application setup, such as `UseResourceFilters()` (see above).
- `ResourceFilterProviderExtensions`: Extension methods for the `IResourceFilterProvider` interface.
- `ResourceManifestExtensions`: Extensions for building the resource manifest, such as `SetDependenciesRecursively()` which helps registering multi-level dependencies.
- `ResourceManagerExtensions`: Extensions for resource usage, such as `RegisterStyle()` which registers a stylesheet resource by name without having to use the error-prone "stylesheet" literal.
