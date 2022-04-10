# Lombiq Helpful Libraries - ASP.NET Core Libraries - MVC



- `ActionResultHelpers`: For returning common specialized `IActionResult`s from `Controller`s, such as `ZipFile`.
- Attributes for controllers:
    - `DevelopmentOnlyAttribute`: Enforces the Development environment on controllers.
    - `DevelopmentAndLocalhostOnlyAttribute`: Enforces the Development environment as well as localhost.
- `Controller` extensions: Adds extension methods to `Controller` objects like `.RedirectToLocal(redirectUrl)`.
- `ResultExecutingContextExtensions` to get some shortcuts to usual context operations in `IAsyncResultFilter`s.
