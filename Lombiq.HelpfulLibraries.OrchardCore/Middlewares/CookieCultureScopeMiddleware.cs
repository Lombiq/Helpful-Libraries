using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Localization;
using System.Globalization;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.Middlewares;

/// <summary>
/// A middleware that looks for the "culture" query string argument or cookie and uses it to initialize a <see
/// cref="CultureScope"/>.
/// </summary>
public class CookieCultureScopeMiddleware(RequestDelegate next)
{
    public const string CultureKeyName = "culture";

    public async Task InvokeAsync(HttpContext context)
    {
        if (!TryGetCultureInfo(context, out var culture))
        {
            culture = await context
                .RequestServices
                .GetRequiredService<ILocalizationService>()
                .GetDefaultCultureAsync() ?? "en-US";
        }

        using var scope = CultureScope.Create(culture, culture, ignoreSystemSettings: true);
        context.SetCookieForever(CultureKeyName, culture);

        await next(context);
    }

    private static bool TryGetCultureInfo(HttpContext context, out string culture)
    {
        if (context.Request.Query.TryGetValue(CultureKeyName, out var queryCulture))
        {
            culture = queryCulture[0];
        }
        else
        {
            context.Request.Cookies.TryGetValue(CultureKeyName, out culture);
        }

        if (string.IsNullOrWhiteSpace(culture)) return false;

        try
        {
            culture = new CultureInfo(culture).Name;
            return true;
        }
        catch
        {
            culture = null;
            return false;
        }
    }
}
