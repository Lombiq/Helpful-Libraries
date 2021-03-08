# Dependency Injection Libraries Documentation



## Auto-register Orchard Core service implementations

Using the `.AddCoreOrchardServiceImplementations(assembly)` extension method in the Orchard Core module or theme Startup file will auto-register the common Orchard Core service implementations. A few special ones like ContentParts need to be registered independently.

Usage:

```csharp
public override void ConfigureServices(IServiceCollection services)
{
    services.AddCoreOrchardServiceImplementations(typeof(Startup).Assembly);
}
```


## Lazy injection support

Using the `.AddLazyInjectionSupport()` extension will allow you to inject lazy dependencies like `Lazy<IMyService> myService`.

Usage:

```csharp
public override void ConfigureServices(IServiceCollection services)
{
    services.AddLazyInjectionSupport();
}
```

## Shell scope extensions

Use `WithShellScopeAsync()` and `GetWithShellScopeAsync()` to [access services from another tenant](https://orcharddojo.net/blog/how-to-access-services-from-another-tenant-in-orchard-core-orchard-nuggets). If multi-tenancy is not enabled, the default `scopeName` is suitable to access the Default tenant.

Usage:

```csharp
var contentItem = await context.ServiceProvider.GetWithShellScopeAsync(scope =>
{
    var contentManager = scope.ServiceProvider.GetService<IContentManager>();
    return contentManager.GetAsync(contentItemId);
});
```
