using Microsoft.Extensions.Localization;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Entities;
using OrchardCore.Environment.Commands;
using OrchardCore.Modules;
using OrchardCore.Users.Models;
using OrchardCore.Users.Services;
using System.Threading.Tasks;
using YesSql;

namespace Lombiq.HelpfulLibraries.Libraries.Contents
{
    public class ContentDefinitionCommands : DefaultCommandHandler
    {
        private const string AttachContentPartCommandName = "attachContentPart";

        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IUserService _userService;
        private readonly ISession _session;
        private readonly IClock _clock;

        [OrchardSwitch]
        public string Type { get; set; }

        [OrchardSwitch]
        public string Part { get; set; }

        public ContentDefinitionCommands(
            IStringLocalizer<ContentDefinitionCommands> localizer,
            IContentDefinitionManager contentDefinitionManager,
            IUserService userService,
            ISession session,
            IClock clock)
            : base(localizer)
        {
            _contentDefinitionManager = contentDefinitionManager;
            _userService = userService;
            _session = session;
            _clock = clock;
        }

        [CommandName(AttachContentPartCommandName)]
        [CommandHelp(AttachContentPartCommandName +
            " /Type:<contentTypeName>" +
            " /Part:<contentPartName>" +
            "\r\n\t" + "Attaches a content type to a content part.")]
        [OrchardSwitches("UserName, EmailConfirmed, UserVerified, UserApproved")]
        public void AttachContentPart() =>
            _contentDefinitionManager.AlterTypeDefinition(Type, type => type.WithPart(Part));
    }
}
