# Lombiq Helpful Libraries - ASP.NET Core Libraries - Security

## `Content-Security-Policy`

- `ApplicationBuilderExtensions`: Contains the `AddContentSecurityPolicyHeader` extension method to add a middleware that provides the `Content-Security-Policy` header.
- `CdnContentSecurityPolicyProvider`: An optional policy provider that permits additional CDN host names for the `script-scr` and `style-src` directives.
- `ContentSecurityPolicyDirectives`: The `Content-Security-Policy` directive names that are defined in the W3C [recommendation](https://www.w3.org/TR/CSP2/#directives) and some common values.
- `EmbeddedMediaContentSecurityPolicyProvider`: An optional policy provider that permits additional host names used by usual media embedding sources (like YouTube) for the `frame-scr` directive.
- `IContentSecurityPolicyProvider`: Interface for services that update the dictionary that will be turned into the `Content-Security-Policy` header value.
- `ServiceCollectionExtensions`: Extensions methods for `IServiceCollection`, e.g. `AddContentSecurityPolicyProvider()` is a shortcut to register `IContentSecurityPolicyProvider` in dependency injection.
- `XWidgetsContentSecurityPolicyProvider`: An optional content security policy provider that provides configuration to allow the usage of X (Twitter) social widgets.

There is a similar section for security extensions related to Orchard Core [here](../../Lombiq.HelpfulLibraries.OrchardCore/Docs/Security.md).

These extensions provide additional security and can resolve issues reported by the [ZAP security scanner](https://github.com/Lombiq/UI-Testing-Toolbox/blob/dev/Lombiq.Tests.UI/Docs/SecurityScanning.md).
