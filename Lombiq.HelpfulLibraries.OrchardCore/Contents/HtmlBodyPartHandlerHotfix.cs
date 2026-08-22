using Lombiq.HelpfulLibraries.OrchardCore.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.Html.Handlers;
using OrchardCore.Html.Models;
using OrchardCore.Infrastructure.Html;
using OrchardCore.Liquid;
using OrchardCore.Shortcodes.Services;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.Contents;

public class HtmlBodyPartHandlerHotfix : ContentPartHandler<HtmlBodyPart>
{
    private readonly HtmlBodyPartHandler _handler;

    public HtmlBodyPartHandlerHotfix(
        IContentDefinitionManager contentDefinitionManager,
        IShortcodeService shortcodeService,
        ILiquidTemplateManager liquidTemplateManager,
        HtmlEncoder htmlEncoder,
        IHtmlSanitizerService htmlSanitizerService) =>
        _handler = new(contentDefinitionManager, shortcodeService, liquidTemplateManager, htmlEncoder, htmlSanitizerService);

    public override Task GetContentItemAspectAsync(ContentItemAspectContext context, HtmlBodyPart part) =>
        _handler.GetContentItemAspectAsync(context, part);

    public override Task ImportedAsync(ImportContentContext context, HtmlBodyPart part) =>
        part.Html?.Contains("{{") == true
            ? Task.CompletedTask
            : _handler.ImportedAsync(context, part);

    // Mark this as obsolete once https://github.com/OrchardCMS/OrchardCore/issues/19767 is fixed.
    public static void ReplaceHandler(IServiceCollection services) =>
        services
            .AddContentPart<HtmlBodyPart>()
            .RemoveHandler<HtmlBodyPartHandler>()
            .AddHandler<HtmlBodyPartHandlerHotfix>();

    public static void ReplaceHandler(OrchardCoreBuilder builder) =>
        builder.ApplicationServices.AddInlineStartup(ReplaceHandler, order: 99);
}
