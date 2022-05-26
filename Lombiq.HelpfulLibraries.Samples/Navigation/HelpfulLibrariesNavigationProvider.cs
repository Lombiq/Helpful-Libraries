using Lombiq.HelpfulLibraries.OrchardCore.Navigation;
using Lombiq.HelpfulLibraries.Samples.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using OrchardCore.Navigation;

namespace Lombiq.HelpfulLibraries.Samples.Navigation;

public class HelpfulLibrariesNavigationProvider : MainMenuNavigationProviderBase
{
    public HelpfulLibrariesNavigationProvider(
        IHttpContextAccessor hca,
        IStringLocalizer<HelpfulLibrariesNavigationProvider> stringLocalizer)
        : base(hca, stringLocalizer)
    {
    }

    protected override void Build(NavigationBuilder builder)
    {
        var context = _hca.HttpContext;
        builder
            .Add(T["Helpful Libraries"], builder => builder
                .AddLabel(T["LINQ to DB"])
                .Add(T["Simple Query"], subMenu => subMenu
                    .ActionTask<LinqToDbSamplesController>(context, controller => controller.SimpleQuery()))
                .Add(T["Join Query"], subMenu => subMenu
                    .ActionTask<LinqToDbSamplesController>(context, controller => controller.JoinQuery()))
                .Add(T["CRUD"], subMenu => subMenu
                    .ActionTask<LinqToDbSamplesController>(context, controller => controller.Crud()))
                .Add(T["---"], subMenu => subMenu.Url("#"))
                .Add(T["Typed Route"], itemBuilder => itemBuilder
                    .Action<TypedRouteController>(context, controller => controller.Index())));
    }
}
