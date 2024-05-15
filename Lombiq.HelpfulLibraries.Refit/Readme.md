# Lombiq Helpful Libraries - Refit Libraries for Orchard Core

## About

Adds helpers for working with the [Refit](https://github.com/reactiveui/refit) RESTful API consumer library.

For general details about and on using the Helpful Libraries see the [root Readme](../Readme.md).

## Helpers

- `RefitHelper`: Adds shortcuts for creating Refit API clients from interfaces. (Note: as of OC 2.0 `Newtonsoft.Json` has been deprecated so methods like `RefitHelper.WithNewtonsoftJson<T>()` are marked obsolete.)

## Models

- `SimpleTextResponse`: An simplified container for the content, headers and some other metadata from `ApiResponse<string>`. This way the `ApiResponse<string>` can be disposed early and doesn't have to be carried around which would be a potential memory leak risk.
