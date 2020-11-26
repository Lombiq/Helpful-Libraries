# MVC Libraries Documentation



## Controller extensions

Adds extension methods to `Controller` objects like `.RedirectToLocal(redirectUrl)`.

Please see the inline documentation of each extension methods learn more.


## Attributes for controllers

- `DevelopmentOnlyAttribute`: Enforces the Development environment on controllers.
- `DevelopmentAndLocalhostOnlyAttribute`: Enforces the Development environment as well as localhost.


## Controller Helpers

- `ActionResultHelpers`: For returning common specialized `IActionResult`s, such as `ZipFile`,  
