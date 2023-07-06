using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;

namespace Lombiq.HelpfulLibraries.OrchardCore.Navigation;

public class AdminMenuNavigationProviderBase : NavigationProviderBase
{
    public const string AdminNavigationName = "admin";

    protected override string NavigationName => AdminNavigationName;

    protected AdminMenuNavigationProviderBase(IHttpContextAccessor hca, IStringLocalizer stringLocalizer)
        : base(hca, stringLocalizer)
    {
    }
}
