# Lombiq Helpful Libraries - Orchard Core Libraries - Data for Orchard Core

## Extensions

- `ManualConnectingIndexServiceExtensions`: Adds `IManualConnectingIndexService` extension methods for removing indices by specifying document ID or content.
- `SessionExtensions`: Shortcut to execute a query from a raw SQL string.
- `QueryExtensions`: Adds `IQuery` manipulating extension methods to YesSql queries. For example, `PaginateAsync()` for breaking the result into pages.
- `SchemaBuilderExtensions`: Adds shortcut extension method for `SchemaBuilder`.
- `SqlDialectExtensions`: Adds extensions to check the type of SQL language used.
- `IndexExtensions`: Adds extensions for index creation.

## Services

- `IManualConnectingIndexService<in T>`: A service for managing a `MapIndex` without an automatic provider. The index refers to documents of `T` type and they can be added or removed via the service's methods only.

## Migrations

- `IndexDataMigration<TIndex>`: A base class for index migrations that handles a single index table.
- `RecipeMigrationsBase`: A base class for recipe migrations that automatically calls the `{module-or-theme-id}.UpdateFrom0.recipe.json` recipe on creation and makes it easier to call update recipes in a similar format.
