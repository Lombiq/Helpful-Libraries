# Contents Libraries Documentation


## Constants

- `CommonContentDisplayTypes`: Has constants for values that can be used with `IContentItemDisplayManager.BuildDisplayAsync` or `OrchardRazorHelperExtensions.DisplayAsync` to safely select the correct display type.


## Extensions

- `ContentDefinitionManagerExtensions`: Adds extension methods to easily fetch settings objects from `ContentTypePartDefinition` objects.
- `ContentEnumerableExtensions`: Adds an extension for selecting parts in a collection of contents.
- `ContentExtensions`: Adds `ContentItem` manipulating extension methods to `IContent` objects, the same ones as it is available for the `ContentItem` objects (e.g. `.As<T>()` or `.Weld<T>()`).
- `ContentManagerExtensions`: Adds extension methods for retrieving, loading or creating content using the `IContentManager`.


## Services

- `IContentVersionNumberService`: Service for getting content version number based on how many different versions of it are in the document database. It has a method for getting the version number of the latest version (`GetLatestVersionNumberAsync`) and another one for a specific content version id (`GetCurrentVersionNumberAsync`).

Please see the inline documentation of each extension methods learn more.
