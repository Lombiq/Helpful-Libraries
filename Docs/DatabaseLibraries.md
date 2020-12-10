# Database Libraries Documentation



## Extensions

- `SessionExtensions`: Shortcut to execute a query from a raw SQL string.
- `QueryExtensions`: Adds `IQuery` manipulating extension methods to YesSql queries. (eg. `PaginateAsync` for breaking the result into pages).
- `TransactionSqlDialectFactory`: Shortcut to create and `ISqlDialect` with a `DbTransaction`.

Please see the inline documentation of each extension method to learn more.
