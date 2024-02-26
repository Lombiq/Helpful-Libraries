# Lombiq Helpful Libraries - Orchard Core Libraries - Shapes for Orchard Core

## Extensions

- `LayoutExtensions`: Adds features for adding shapes to the layout via `ILayoutAccessor`.
- `ServiceCollectionExtensions`: Allows adding `ShapeRenderer` to the service collection via `AddShapeRenderer()`.
- `ShapeExtensions`: Some shortcuts for managing shapes and a pair of extensions for creating ad-hoc shapes and injecting them into the shape table.

## Shape rendering

- `IShapeRenderer`: This service can be used to render `IShape` objects to `string` format.
