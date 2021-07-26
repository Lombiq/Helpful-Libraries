# Utilities Documentation


## Extensions

- `ConfigurationExtensions`: Shortcuts for `IConfiguration` operations.
- `ContentOrchardHelperExtensions`: Extensions for managing content items better via `IOrchardHelper`.
- `DictionaryExtensions`: Adds `IDictionary<TKey, TValue>` extensions. (eg. `GetMaybe` for safely retrieving an item if it's in the dictionary or returning `default` without throwing an exception).
- `EnumerableExtensions`: Adds `IEnumerable<T>` extensions. (eg. `AwaitEachAsync` for performing async operations on a collection sequentially, and `AsList` for casting to `List<T>` without necessarily creating a new list).
- `EnumExtensions`: Adds extensions for working with `enum` types. (eg. `UnknownEnumException` which can be used to raise a standardized exception on the default arm of a `switch`).
- `ExceptionHelpers`: Using these helpers, arguments can be tested without writing `if` statements. There are Orchard Core content-specific helpers as well for example checks if the `ContentItem` has a the given part attached to it).
- `HttpContextExtensions`: Some shortcuts for managing cookies.
- `JsonHelpers`: JSON syntax can be validated with the `ValidateJsonIfNotNull` helper method.
- `JsonStringExtensions`: Adds JSON related extensions for the `string` type. (E.g. `JsonHtmlContent` which safely serializes a string for use in `<script>` elements.)
- `MemoryCacheExtensions`: Adds extensions for `IMemoryCache` manipulation. (E.g. `GetOrNew<T>` type-safely returns the item or creates a new instance.)
- `NonSecurityRandomizer`: A wrapper around `System.Random` for explicitly not security-related usage-cases.
- `NumberExtensions`: Adds extensions for primitive numeric types. (E.g. `ToTechnicalString` converts `int` into culture invariant `string`.)
- `OrchardCoreBuilderExtensions`: Shortcuts when initializing Orchard with `OrchardCoreBuilder`, i.e. `AddOrchardCms()`.
- `RNGCryptoServiceProviderExtensions`: Shortcuts for retrieving cryptographically secure random numbers.
- `Sha256Helper`: A static helper class with the `ComputeHash` utility function that converts text into [SHA-256](https://en.wikipedia.org/wiki/SHA-256) hash string.
- `StringExtensions`: Adds common useful extensions to the `string` type. (E.g. `SplitByCommas` and `ContainsLoose`.) 
- `UserServiceExtensions`: Adds extensions for `IUserService`. (E.g. `GetOrchardUserAsync` retrieves the user by user name or throws an exception if none found.)

Please see the inline documentation of each extension method to learn more.
