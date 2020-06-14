# Dependency Injection Libraries Documentation



## Auto-register Orchard Core service implementations

Using the `.AddCoreOrchardServiceImplementations(assembly)` extension method in the Orchard Core module or theme Startup file will auto-register the common Orchard Core service implementations. A few special ones like ContentParts need to be registered independently.

Usage:

```
public override void ConfigureServices(IServiceCollection services)
{
    services.AddCoreOrchardServiceImplementations(typeof(Startup).Assembly);
}
```