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
        new(ShouldSuppressHeaderInner(context));

    private bool ShouldSuppressHeaderInner(HttpContext context) =>
        // No need to do content security policy on non-HTML responses.
        context.Response.ContentType?.ContainsOrdinalIgnoreCase(MediaTypeNames.Text.Html) != true;
}
