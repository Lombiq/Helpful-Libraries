# Lombiq Helpful Libraries - ASP.NET Core Libraries - Security

## `Content-Security-Policy`

- `ApplicationBuilderExtensions`: Contains the `AddContentSecurityPolicyHeader` extension method to add a middleware that provides the `Content-Security-Policy` header.
- `ContentSecurityPolicyDirectives`: The `Content-Security-Policy` directive names that are defined in the W3C [recommendation](https://www.w3.org/TR/CSP2/#directives) and some common values.
- `IContentSecurityPolicyProvider`: Interface for services that update the dictionary that will be turned into the `Content-Security-Policy` header value.
- `AntiClickjackingContentSecurityPolicyProvider`: An optional policy provider that prevents clickjacking by including the `frame-ancestors 'self'` directive.
