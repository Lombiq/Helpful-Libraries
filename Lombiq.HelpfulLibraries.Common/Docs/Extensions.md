# Lombiq Helpful Libraries - Common Libraries - Extensions

- `ArrayExtensions`: Adds useful extensions to arrays. For example, `Exists()` is a fluid alternative to `Array.Exists()`.
- `CollectionExtensions`: Adds `ICollection<T>` extensions. For example, `CartesianProduct()` for producing all pairs of two collections, or `RemoveAll()` to filter an existing collection in-place.
- `ConfigurationExtensions`: Shortcuts for `IConfiguration` operations.
- `DictionaryExtensions`: Adds `IDictionary<TKey, TValue>` extensions. For example, `GetMaybe()` for safely retrieving an item if it's in the dictionary or returning `default` without throwing an exception.
- `EnumerableExtensions`: Adds `IEnumerable<T>` extensions. For example, `AwaitEachAsync()` for performing async operations on a collection sequentially, or `AsList()` for casting to `List<T>` without necessarily creating a new list.
- `EnumExtensions`: Adds extensions for working with `enum` types. For example, `UnknownEnumException` which can be used to raise a standardized exception on the default arm of a `switch`.
- `ExceptionExtensions`: An `IsFatal()` method to check if the exception is one that the application can't recover from.
- `ExpressionExtensions`: Adds `System.Linq.Expressions`. For example, `StripResult()` turns a `Func<T1, T2>` expression into an `Action<T1>` one.
- `HttpContextExtensions`: Some shortcuts for managing cookies.
- `IoExtensions`: Adds extensions for `String.IO` types. For example, `TextWriter.WriteLineInvariant()` writes interpolated string in a culture invariant manner.
- `JsonExtensions`: Adds extensions for `Newtonsoft.Json.Linq` types. For example, `JObject.TryParse<T>(out var result)` attempts to convert the JSON object into a C# object.
- `MemoryCacheExtensions`: Adds extensions for `IMemoryCache` manipulation. For example, `GetOrNew<T>()` type-safely returns the item or creates a new instance.
- `MulticastDelegateExtensions`: Extensions for `MulticastDelegate`s, e.g. to invoke async delegates in a safe fashion.
- `NumberExtensions`: Adds extensions for primitive numeric types. For example, `ToTechnicalString()` converts `int` into culture invariant `string`.
- `RandomNumberGeneratorExtensions`: Shortcuts for retrieving cryptographically secure random numbers.
- `ServiceCollectionExtensions`: Shortcuts to remove implementations from an `IServiceCollection` instance.
- `StringExtensions`: Adds common useful extensions to the `string` type, such as `SplitByCommas()` and `ContainsLoose()`.
