# Lombiq Helpful Libraries - Orchard Core Libraries - Dependency Injection for Orchard Core

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

## Inline Startup

Using the `serviceCollection.AddInlineStartup()` extension method you provide `Startup.Configure()` and `Startup.ConfigureServices()` as lambda expressions without the need to create a whole class. Ideal for your _Program.cs_ or in extension methods.

Usage:

```csharp
serviceCollection.AddInlineStartup(services => services.AddScoped<IMyService, MyService>());
serviceCollection.AddInlineStartup(services => services.AddScoped<IMyService, OtherService>(), (app, _, _) => app.UseStaticFiles());
```
