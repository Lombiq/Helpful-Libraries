# Lombiq Helpful Libraries - Orchard Core Libraries - MVC for Orchard Core



## Extensions and helpers

- `Controller` extensions: Adds extension methods to `Controller` objects like `.RedirectToContentDisplay(content)`.

- `ResultExecutingContextExtensions` to get some shortcuts to usual context operations in `IAsyncResultFilter`s.
- `ShapeResultExtensions`: Adds extensions methods generating placement strings on your shape description, such as `UseTab`.
- `UrlHelperExtensions`: Adds extension methods to the `@Url` helper, such as `EditContentItemWithTab`.


## `TypedRoute`

This class provides a strongly typed way to generate local URLs for Orchard Core MVC actions. It uses lambda expressions to select the action and provide arguments. Use `TypedRoute.CreateFromExpression<TClass>(...).ToString()` or the provided `OrchardHelper.Action()` and `HttpContext.Action()` extensions.

Check out the [`Lombiq.HelpfulLibraries.Samples` project](../../Lombiq.HelpfulLibraries.Samples) for examples and a [video demo here](https://www.youtube.com/watch?v=_q1kCqkeSE0).

If you also use our [UI Testing Toolbox](https://github.com/Lombiq/UI-Testing-Toolbox/), you can use the `UITestContext.GoTo()` extension method from there, as well.


## `WidgetFilterBase<T>`

A base class for creating filters that insert a content as widget in a specified zone with permission check.
