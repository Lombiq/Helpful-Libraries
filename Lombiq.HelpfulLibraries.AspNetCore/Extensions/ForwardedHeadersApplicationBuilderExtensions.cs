using Microsoft.AspNetCore.HttpOverrides;

namespace Microsoft.AspNetCore.Builder;

public static class ForwardedHeadersApplicationBuilderExtensions
{
    /// <summary>
    /// Forwards proxied headers onto the current request with settings suitable for an app behind Cloudflare and hosted
    /// in an Azure App Service. Call this from the web app's <c>Startup</c> class.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Uses and configures <see cref="ForwardedHeadersExtensions.UseForwardedHeaders(IApplicationBuilder,
    /// ForwardedHeadersOptions)"/> under the hood.
    /// </para>
    /// <para>
    /// Use this instead of the Reverse Proxy Configuration feature when the host header is forwarded too. Otherwise
    /// tenant host matching won't work.
    /// </para>
    /// <para>
    /// Doesn't cause any issues if there are no forwarded headers so you can use it during development too.
    /// </para>
    /// </remarks>
    public static IApplicationBuilder UseForwardedHeadersForCloudflareAndAzure(this IApplicationBuilder builder)
    {
        var forwardedHeadersOptions = new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.All,
            // Cloudflare, a custom reverse proxy, and potentially an Azure load balancer.
            ForwardLimit = 3,
        };

        // These are not all known for Cloudflare and Azure.
        forwardedHeadersOptions.KnownNetworks.Clear();
        forwardedHeadersOptions.KnownProxies.Clear();

        builder.UseForwardedHeaders(forwardedHeadersOptions);

        return builder;
    }
}
