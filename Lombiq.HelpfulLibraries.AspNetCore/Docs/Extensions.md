# Lombiq Helpful Libraries - ASP.NET Core Libraries - Extensions for Orchard Core



- `CookieHttpContextExtensions`: Provides shortcuts for some cookie-related operations.
- `EnvironmentHttpContextExtensions`: Provides shortcuts to determine information about the current hosting environment, like whether the app is running in Development mode.
- `ForwardedHeadersApplicationBuilderExtensions`: Provides `UseForwardedHeadersForCloudflareAndAzure()` that forwards proxied headers onto the current request with settings suitable for an app behind Cloudflare and hosted in an Azure App Service.
- `NonEmptyTagHelper`: An attribute tag helper that conditionally hides its element if the provided collection is null or empty. This eliminates a bulky wrapping `@if(collection?.Count > 1) { ... }` expression that would needlessly increase the document's indentation too.
- `TemporaryResponseWrapper`: An `IAsyncDisposable` that replaces the `HttpContext`'s response stream at creation with a `MemoryStream` and copies its content back into the real response stream during disposal.
