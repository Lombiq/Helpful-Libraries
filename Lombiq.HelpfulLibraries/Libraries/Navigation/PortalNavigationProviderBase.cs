using Lombiq.HelpfulLibraries.Libraries.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using OrchardCore.Navigation;
using System;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.Libraries.Navigation
{
    public abstract class PortalNavigationProviderBase : NavigationProviderBase
    {
        public const string PortalNavigationName = "portal";

        protected override string NavigationName => PortalNavigationName;

        protected PortalNavigationProviderBase(IHttpContextAccessor hca, IStringLocalizer stringLocalizer)
            : base(hca, stringLocalizer)
        {
        }
    }
}
