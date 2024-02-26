# Lombiq Helpful Libraries - Orchard Core Libraries - Security

## Extensions

- `SecurityOrchardCoreBuilderExtensions`: Adds `BuilderExtensions` extensions. For example, the `ConfigureSecurityDefaultsWithStaticFiles()` that provides some default security configuration for Orchard Core.

There is a similar section for security extensions related to ASP.NET Core [here](../../Lombiq.HelpfulLibraries.AspNetCore/Docs/Security.md). All of the services mentioned in both documents are included in the `ConfigureSecurityDefaults()` and `ConfigureSecurityDefaultsWithStaticFiles()` extensions.

These extensions provide additional security and can resolve issues reported by the [ZAP security scanner](https://github.com/Lombiq/UI-Testing-Toolbox/blob/dev/Lombiq.Tests.UI/Docs/SecurityScanning.md).

## Attributes

- `ContentSecurityPolicyAttribute`: You can add the `[ContentSecurityPolicy(value, name)]` attribute to any MVC action's method. This way you can grant per-action content security policy permissions, right there in the controller. These attributes are handled by the `ContentSecurityPolicyAttributeContentSecurityPolicyProvider`.

## Services

- `ResourceManagerContentSecurityPolicyProvider`: An abstract base class for implementing content security policy providers that trigger when the specified resource is included.
- `VueContentSecurityPolicyProvider`: An implementation of `ResourceManagerContentSecurityPolicyProvider` that adds `script-src: unsafe-eval` permission to the page if it uses the `vuejs` resource. This includes any Vue.js app in stock Orchard Core, apps you create in your view files, and SFCs created with the Lombiq.VueJs module. This is necessary, because without `unsafe-eval` Vue.js only supports templates that are pre-compiled into JS code.
