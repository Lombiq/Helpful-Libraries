# Lombiq Helpful Libraries - Common Libraries - Dependency Injection



## Lazy injection support

Using the `.AddLazyInjectionSupport()` extension will allow you to inject lazy dependencies like `Lazy<IMyService> myService`.

Usage:

```csharp
public override void ConfigureServices(IServiceCollection services)
{
    services.AddLazyInjectionSupport();
}
```
