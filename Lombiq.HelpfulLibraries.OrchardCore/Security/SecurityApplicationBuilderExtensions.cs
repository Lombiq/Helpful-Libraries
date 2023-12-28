using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Builder;

public static class SecurityApplicationBuilderExtensions
{
    /// <summary>
    /// Provides some default security middlewares for Orchard Core. Use it in conjunction with <see
    /// cref="SecurityOrchardCoreBuilderExtensions.ConfigureSecurityDefaults"/>.
    /// </summary>
    public static IApplicationBuilder UseSecurityDefaults(this IApplicationBuilder app) => app
        .UseContentSecurityPolicyHeader(allowInline: true)
        .UseContentTypeOptionsHeader();
}
