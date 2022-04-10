# Lombiq Helpful Libraries - Orchard Core Libraries - Resource Management for Orchard Core



## Resource Filter

Makes it possible to include resources automatically based on the current context. E.g. inject home page styling only when the home page is being loaded.

Usage:

Activate the resource filter middleware by adding `app.UseResourceFilters()` to the Configure method of the Startup file located in a common module or the web project. E.g.:

```C#
public override void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
{
    app.UseResourceFilters();
}
```

To add resource filters the `IResourceFilterProvider` interface needs to be implemented and the registration needs to be added to the service collection as well.

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

## Extensions:

- `ApplicationBuilderExtensions`: Shortcut extensions for application setup, e.g.: `UseResourceFilters` (see above)
- `ResourceManifestExtensions`: Extensions for building the resource manifest, e.g.: `SetDependenciesRecursively` helps registering multi-level dependencies.
- `ResourceManagerExtensions`: Extensions for resource usage, e.g.: `RegisterStyle` registers a stylesheet resource by name without having to use the "stylesheet" literal which is error-prone.
