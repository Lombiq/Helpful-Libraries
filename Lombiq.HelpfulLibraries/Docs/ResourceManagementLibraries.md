# Resource Management Libraries Documentation



## Resource Filter

Makes it possible to include resources automatically based on the current context. E.g. inject home page styling only when the home page is being loaded.

Usage:

Activate the resource filter middleware by adding `app.UseResourceFilters()` to the Configure method of the Startup file located in a common module or the web project. E.g.:

```
public override void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
{
    app.UseResourceFilters();
}
```

To add resource filters the `IResourceFilterProvider` interface needs to be implemented and the registration needs to be added to the service collection as well.

Example:

```
public class ResourceFilters : IResourceFilterProvider
{
    public void AddResourceFilter(ResourceFilterBuilder builder)
    {
        builder.WhenHomePage().RegisterHeadScript("HomePageStyle");
        builder.WhenPath("/my-page").RegisterHeadScript("MyPageScript");
    }
}
```