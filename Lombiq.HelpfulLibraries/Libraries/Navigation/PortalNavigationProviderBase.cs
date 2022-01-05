using Lombiq.HelpfulLibraries.Libraries.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using OrchardCore.Navigation;
using OrchardCore.Security.Permissions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.Libraries.Navigation
{
    public abstract class PortalNavigationProviderBase : INavigationProvider
    {
        public const string NavigationName = "portal";

        private readonly IAuthorizationService _authorizationService;
        private readonly IHttpContextAccessor _hca;

        public virtual IEnumerable<Permission> RequiredPermissions => Array.Empty<Permission>();

        protected IStringLocalizer T { get; }

        protected PortalNavigationProviderBase(
            IAuthorizationService authorizationService,
            IHttpContextAccessor hca,
            IStringLocalizer stringLocalizer)
        {
            _authorizationService = authorizationService;
            _hca = hca;
            T = stringLocalizer;
        }

        public Task BuildNavigationAsync(string name, NavigationBuilder builder) =>
            name.EqualsOrdinalIgnoreCase(NavigationName)
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
