using Microsoft.Extensions.Localization;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.Environment.Commands;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.Contents;

public class ContentDefinitionCommands(
    IStringLocalizer<ContentDefinitionCommands> localizer,
    IContentDefinitionManager contentDefinitionManager) : DefaultCommandHandler(localizer)
{
    private const string AttachContentPartCommandName = "attachContentPart";

    [OrchardSwitch]
    public string Type { get; set; }

    [OrchardSwitch]
    public string Part { get; set; }

    [CommandName(AttachContentPartCommandName)]
    [CommandHelp(AttachContentPartCommandName +
        " /Type:<contentTypeName>" +
        " /Part:<contentPartName>" +
        "\r\n\t" + "Attaches a content part to a content type.")]
    [OrchardSwitches(nameof(Type) + ", " + nameof(Part))]
    public Task AttachContentPartAsync() =>
         contentDefinitionManager.AlterTypeDefinitionAsync(Type, type => type.WithPart(Part));
}
