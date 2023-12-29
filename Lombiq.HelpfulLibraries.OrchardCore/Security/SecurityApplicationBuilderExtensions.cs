using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Builder;

public static class SecurityApplicationBuilderExtensions
{
    /// <summary>
    /// Provides some default security middlewares for Orchard Core. Use it in conjunction with <see
    /// cref="SecurityOrchardCoreBuilderExtensions.ConfigureSecurityDefaults"/>.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///     <item>
    ///         <description>
    ///            Adds a middleware that supplies the <c>Content-Security-Policy</c> header.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             Adds a middleware that supplies the <c>X-Content-Type-Options: nosniff</c> header.
    ///         </description>
    ///     </item>
    /// </list>
    /// </remarks>
    public static IApplicationBuilder UseSecurityDefaults(this IApplicationBuilder app) => app
        .UseContentSecurityPolicyHeader(allowInline: true)
        .UseContentTypeOptionsHeader();
}
