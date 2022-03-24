# Shapes Libraries Documentation



## Shape rendering

- `IShapeRenderer`: This service can be used to render `IShape` objects to `string` format.

## Extensions

- `ServiceCollectionExtensions`: The `AddShapeRenderer` method adds the `ShapeRenderer` to the service collection.
- `ShapeExtensions`: The `shape.PropertiesAs<T>()` method converts the shape properties into the provided type (basically duck typing) so you can restore the type you used e.g. in the driver.
