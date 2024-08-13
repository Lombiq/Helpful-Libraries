# Lombiq Helpful Libraries - Orchard Core Libraries - Environment for Orchard Core

## Extensions

- `FeatureInfoEnumerableExtensions`: Shortcuts for `IEnumerable<IFeatureInfo>`, like `Any(featureId)`.
- `HostingDefaultsOrchardCoreBuilderExtensions`: Lombiq-recommended opinionated default configuration for features of a standard Orchard Core application, including one hosted in Azure. It substitutes much of what you'd write as configuration in a `Program` class or _appsettings.json_ files.
- `OrchardCoreBuilderExtensions`: Shortcuts that can be used when initializing Orchard with `OrchardCoreBuilder`, e.g. `AddOrchardCms()`.
- `ShellFeaturesManagerExtensions`: Shortcuts for `IShellFeaturesManager`, like `IsFeatureEnabledAsync()`.
