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

Elasticsearch interfaces

`IElasticsearchIndexingService` and `IElasticsearchIndexManager` are wrappers around methods of the corresponding Elasticserach services. Use the `services.AddDefaultElasticsearchWrapperServices()` extension method to register all the required services to interact with Elasticsearch, then optionally override them with your own custom implementations. For example our [UI Testing Toolbox](https://github.com/Lombiq/UI-Testing-Toolbox) has such implementations for prefixing indexes so multiple tests in parallel can access the same Elasticsearch server without causing conflicts.
