using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using OrchardCore.Navigation;
using System;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.Navigation;

public abstract class NavigationProviderBase : INavigationProvider
{
    protected readonly IHttpContextAccessor _hca;

    protected abstract string NavigationName { get; }
    protected virtual bool RequireAuthentication => false;
    protected IStringLocalizer T { get; }

    protected NavigationProviderBase(
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
        throw new NotSupportedException(FormattableString.Invariant(
            $"Override either {nameof(Build)} or {nameof(BuildAsync)}! Note that {nameof(BuildAsync)} takes precedence in execution."));
}
