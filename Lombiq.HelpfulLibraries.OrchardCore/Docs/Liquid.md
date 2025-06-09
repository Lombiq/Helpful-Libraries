# Lombiq Helpful Libraries - Orchard Core Libraries - Liquid for Orchard Core

Contains supplementary extensions and services for using Liquid in Orchard Core.

## The `ILiquidPropertyRegistrar` service interface

Implement this to define Liquid properties that rely on other services and async resolution. This way you can save a lot of boilerplate and inject services the usual way.

Note: Use `services.RegisterLiquidPropertyAccessor<TImplementation>(propertyName)` instead of `services.AddScoped()`. That's handled by the extension.

## Filters

- `DisplayChildrenLiquidFilter`: Tries to convert the input object into `IShape` and render the contents of the `IShape.Metadata.ChildContent` property. This is usually not accessible to Liquid if the input is in some other view-model type that obscures `IShape` properties. Usage: `{{ Model | display-children }}`

## Extensions

- `LiquidServiceCollectionExtensions`: Used for registering related services, like `RegisterLiquidPropertyAccessor()` to register new properties.
  - `RegisterLiquidPropertyAccessor`: Allows registering a new Liquid property with the provided name.
  - `AddLiquidParserTag`: Configures the `LiquidViewOptions` with an additional parser tag, like `{% my_tag "arg1", "arg2" %}`.
  - `AddLiquidEmptyTag`: Configures the `LiquidViewOptions` with an additional tag that takes no arguments. Use this if you want to add a tag like `{% tag_that_writes_out_a_string %}`.
- `TemplateOptionsExtensions`: Used for registering accessors with the Liquid engine inside a `services.Configure<TemplateOptions>(options => ...)` call.
