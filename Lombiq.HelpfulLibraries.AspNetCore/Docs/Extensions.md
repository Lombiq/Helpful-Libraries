# Lombiq Helpful Libraries - ASP.NET Core Libraries - Extensions



- `CookieHttpContextExtensions`: Provides shortcuts for some cookie-related operations.
- `DateTimeHttpContextExtensions`: Makes it possible to set or get IANA time-zone IDs in the HTTP context.
- `EnvironmentHttpContextExtensions`: Provides shortcuts to determine information about the current hosting environment, like whether the app is running in Development mode.
- `ForwardedHeadersApplicationBuilderExtensions`: Provides `UseForwardedHeadersForCloudflareAndAzure()` that forwards proxied headers onto the current request with settings suitable for an app behind Cloudflare and hosted in an Azure App Service.
- `JsonStringExtensions`: Adds JSON related extensions for the `string` type. (E.g. `JsonHtmlContent` which safely serializes a string for use in `<script>` elements.)
- `NonEmptyTagHelper`: An attribute tag helper that conditionally hides its element if the provided collection is null or empty. This eliminates a bulky wrapping `@if(collection?.Count > 1) { ... }` expression that would needlessly increase the document's indentation too.
- `TemporaryResponseWrapper`: An `IAsyncDisposable` that replaces the `HttpContext`'s response stream at creation with a `MemoryStream` and copies its content back into the real response stream during disposal.
