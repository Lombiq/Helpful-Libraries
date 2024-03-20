using Lombiq.HelpfulLibraries.AspNetCore.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
using OrchardCore.Admin;
using System;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// A provider for some common scenarios where the header should not be applied at all.
/// </summary>
public class SkipContentSecurityPolicyProvider : IContentSecurityPolicyProvider
{
    private readonly string _adminPathPrefix;

    public SkipContentSecurityPolicyProvider(IOptions<AdminOptions> adminOptions) =>
        _adminPathPrefix = '/' + adminOptions.Value.AdminUrlPrefix;

    public ValueTask<bool> ShouldSuppressHeaderAsync(HttpContext context) =>
        new(ShouldSuppressHeaderInnerAsync(context));

    private bool ShouldSuppressHeaderInnerAsync(HttpContext context) =>
        // No need to do content security policy on non-HTML responses.
        context.Response.ContentType?.ContainsOrdinalIgnoreCase(MediaTypeNames.Text.Html) != true ||
        // The Admin dashboard is only accessible to trusted users so content security is not a concern.
        new Uri(context.Request.GetDisplayUrl()).AbsolutePath.StartsWithOrdinalIgnoreCase(_adminPathPrefix);
}
