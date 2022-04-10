using Microsoft.Extensions.Localization;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.Environment.Commands;

namespace Lombiq.HelpfulLibraries.OrchardCore.Contents;

public class ContentDefinitionCommands : DefaultCommandHandler
{
    private const string AttachContentPartCommandName = "attachContentPart";

    private readonly IContentDefinitionManager _contentDefinitionManager;

    [OrchardSwitch]
    public string Type { get; set; }

    [OrchardSwitch]
    public string Part { get; set; }

    public ContentDefinitionCommands(
        IStringLocalizer<ContentDefinitionCommands> localizer,
        IContentDefinitionManager contentDefinitionManager)
        : base(localizer) =>
        _contentDefinitionManager = contentDefinitionManager;

    [CommandName(AttachContentPartCommandName)]
    [CommandHelp(AttachContentPartCommandName +
        " /Type:<contentTypeName>" +
        " /Part:<contentPartName>" +
        "\r\n\t" + "Attaches a content part to a content type.")]
    [OrchardSwitches(nameof(Type) + ", " + nameof(Part))]
    public void AttachContentPart() =>
        _contentDefinitionManager.AlterTypeDefinition(Type, type => type.WithPart(Part));
}
