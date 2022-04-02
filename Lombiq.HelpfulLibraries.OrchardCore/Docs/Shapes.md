# Lombiq Helpful Libraries - Orchard Core Libraries - Shapes for Orchard Core



## Extensions

- `LayoutExtensions`: Adds features for adding shapes to the layout via `ILayoutAccessor`.
- `ShapeExtensions`: Some shortcuts for managing shapes.

## Shape rendering

- `IShapeRenderer`: This service can be used to render `IShape` objects to `string` format.
- `LayoutZoneTagHelper`: The `<layout-zone>` tag helper works the same way as `<zone>` but it can be used even [in the _Layout.cshtml_ when widgets are displayed in the same zone](https://github.com/OrchardCMS/OrchardCore/issues/11481).
