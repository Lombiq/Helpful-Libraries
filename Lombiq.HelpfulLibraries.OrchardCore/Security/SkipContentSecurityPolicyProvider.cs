using Lombiq.HelpfulLibraries.AspNetCore.Security;
using Microsoft.AspNetCore.Http;
using System;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// A provider for some common scenarios where the header should not be applied at all.
/// </summary>
public class SkipContentSecurityPolicyProvider : IContentSecurityPolicyProvider
{
    public ValueTask<bool> ShouldSuppressHeaderAsync(HttpContext context) =>
        new(ShouldSuppressHeaderInner(context));

    /// <summary>
    /// Returns a value indicating whether the requested document is non-HTML. There is no need to
    /// do content security policy in such responses.
    /// </summary>
    private static bool ShouldSuppressHeaderInner(HttpContext context) =>
        context.Response.ContentType?.ContainsOrdinalIgnoreCase(MediaTypeNames.Text.Html) != true;
}
