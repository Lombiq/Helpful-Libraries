using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;

namespace Lombiq.HelpfulLibraries.OrchardCore.Navigation;

public abstract class MainMenuNavigationProviderBase : NavigationProviderBase
{
    public const string MainNavigationName = "main";

    protected override string NavigationName => MainNavigationName;

    protected MainMenuNavigationProviderBase(IHttpContextAccessor hca, IStringLocalizer stringLocalizer)
        : base(hca, stringLocalizer)
    {
    }
}
