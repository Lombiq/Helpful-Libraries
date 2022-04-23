using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace Microsoft.AspNetCore.Http;

public static class LocalizationHttpContextExtensions
{
    /// <summary>
    /// Returns the CultureInfo set in the request.
    /// </summary>
    /// <returns>CultureInfo set in the request.</returns>
    public static CultureInfo GetUICulture(this HttpContext httpContext) =>
        httpContext.Features.Get<IRequestCultureFeature>().RequestCulture.UICulture;
}
