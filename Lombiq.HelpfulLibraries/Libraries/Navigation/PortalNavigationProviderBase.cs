using Lombiq.HelpfulLibraries.Libraries.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using OrchardCore.Navigation;
using System;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.Libraries.Navigation
{
    public abstract class PortalNavigationProviderBase : INavigationProvider
    {
        public const string NavigationName = "portal";
        private readonly IHttpContextAccessor _hca;

        protected virtual bool RequireAuthentication => false;
        protected IStringLocalizer T { get; }

        protected PortalNavigationProviderBase(
            IHttpContextAccessor hca,
            IStringLocalizer stringLocalizer)
        {
            _hca = hca;
            T = stringLocalizer;
        }

        public Task BuildNavigationAsync(string name, NavigationBuilder builder) =>
            name.EqualsOrdinalIgnoreCase(NavigationName) &&
            (!RequireAuthentication || _hca.HttpContext?.User?.Identity?.IsAuthenticated == true)
                ? BuildAsync(builder)
                : Task.CompletedTask;

        protected virtual Task BuildAsync(NavigationBuilder builder)
        {
            Build(builder);
            return Task.CompletedTask;
        }

        protected virtual void Build(NavigationBuilder builder) =>
            throw new NotSupportedException(
                StringHelper.Join(
                    " ",
                    $"Override either {nameof(Build)} or {nameof(BuildAsync)}!",
                    $"Note that {nameof(BuildAsync)} takes precedence in execution."));
    }
}
