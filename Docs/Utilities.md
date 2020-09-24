# Utilities Documentation


## Extensions

- EnumerableExtensions: Adds `IEnumerable<T>` extensions. (eg. `AwaitEachAsync` for performing async operations on a collection sequentially, and `AsList` for casting to `List<T>` without necessarily creating a new list)
- EnumExtensions: Adds extensions for working with `enum` types. (eg. `UnknownEnumException` which can be used to raise a standardized exception on the default arm of a `switch`)
- ExceptionHelpers: Using these helpers, arguments can be tested without writing `if` statements. There are Orchard Core content-specific helpers as well for example checks if the ContentItem has a the given part attached to it)
- JsonHelpers: JSON syntax can be validated with the `ValidateJsonIfNotNull` helper method.
- NumberExtensions: Adds extensions for primitive numeric types. (eg. `ToTechnicalString` converts int into culture invariant string)
- UserServiceExtensions: Adds extensions for `IUserService`. (eg. `GetOrchardUserAsync` retrieves the user by user name or throws an exception if none found)

Please see the inline documentation of each extension method to learn more.
