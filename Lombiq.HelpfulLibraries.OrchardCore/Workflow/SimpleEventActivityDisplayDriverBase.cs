using Lombiq.HelpfulLibraries.OrchardCore.Contents;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Localization;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement.Html;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Workflows.Activities;
using OrchardCore.Workflows.Helpers;

namespace Lombiq.HelpfulLibraries.OrchardCore.Workflow;

/// <summary>
/// A base class for a simple workflow event driver that only displays a title, description and optional icon in a
/// conventional format.
/// </summary>
public abstract class SimpleEventActivityDisplayDriverBase<TActivity> : DisplayDriver<IActivity, TActivity>
    where TActivity : class, IActivity
{
    public virtual string IconClass { get; }
    public virtual LocalizedHtmlString Title { get; }
    public abstract LocalizedHtmlString Description { get; }

    private string IconHtml => string.IsNullOrEmpty(IconClass) ? string.Empty : $"<i class=\"fa {IconClass}\"></i>";

    public override IDisplayResult Display(TActivity model) =>
        Combine(
            this.RawHtml(ThumbnailHtml(model)).Location(CommonContentDisplayTypes.Thumbnail, CommonLocationNames.Content),
            this.RawHtml(DesignHtml(model)).Location(CommonContentDisplayTypes.Design, CommonLocationNames.Content));

    private string ThumbnailHtml(TActivity model) =>
        $"<h4 class=\"card-title\">{IconHtml}{GetTitle(model)}</h4><p>{Description?.Html()}</p>";

    private string DesignHtml(TActivity model) =>
        $"<header><h4>{IconHtml}{GetTitle(model)}</h4></header>";

    private string GetTitle(TActivity model)
    {
        var title = model.GetTitleOrDefault(() => Title).Html();

        return string.IsNullOrWhiteSpace(title)
            ? new HtmlContentString(model.DisplayText).Html()
            : title;
    }
}
