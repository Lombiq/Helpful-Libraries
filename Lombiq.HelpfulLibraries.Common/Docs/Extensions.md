# Lombiq Helpful Libraries - Common Libraries - Extensions



- `CollectionExtensions`: Adds `ICollection<T>` extensions. (eg. `CartesianProduct` for producing all pairs of two collections or `RemoveAll` to filter an existing collection in-place).
- `ConfigurationExtensions`: Shortcuts for `IConfiguration` operations.
- `DictionaryExtensions`: Adds `IDictionary<TKey, TValue>` extensions. (eg. `GetMaybe` for safely retrieving an item if it's in the dictionary or returning `default` without throwing an exception).
- `EnumerableExtensions`: Adds `IEnumerable<T>` extensions. (eg. `AwaitEachAsync` for performing async operations on a collection sequentially, and `AsList` for casting to `List<T>` without necessarily creating a new list).
- `EnumExtensions`: Adds extensions for working with `enum` types. (eg. `UnknownEnumException` which can be used to raise a standardized exception on the default arm of a `switch`).
- `ExceptionExtensions`: An `IsFatal()` method to check if the exception is one that the application can't recover from.
- `ExpressionExtensions`: Adds `System.Linq.Expressions`. (E.g. `StripResult` turns a `Func<T1, T2>` expression int an `Action<T1>` one).
- `HttpContextExtensions`: Some shortcuts for managing cookies.
- `IoExtensions`: Adds extensions for `String.IO` types. (E.g. `TextWriter.WriteLineInvariant` writes interpolated string in a culture invariant manner.)
- `MemoryCacheExtensions`: Adds extensions for `IMemoryCache` manipulation. (E.g. `GetOrNew<T>` type-safely returns the item or creates a new instance.)
- `MulticastDelegateExtensions`: Extensions for `MulticastDelegate`s, e.g. to invoke async delegates in a safe fashion.
- `NumberExtensions`: Adds extensions for primitive numeric types. (E.g. `ToTechnicalString` converts `int` into culture invariant `string`.)
- `RandomNumberGeneratorExtensions`: Shortcuts for retrieving cryptographically secure random numbers.
- `ServiceCollectionExtensions`: Shortcuts to remove implementations from an `IServiceCollection` instance.
- `StringExtensions`: Adds common useful extensions to the `string` type. (E.g. `SplitByCommas` and `ContainsLoose`.)
