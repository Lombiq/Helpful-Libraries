using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OrchardCore.Environment.Commands;
using OrchardCore.Security;
using System.Threading.Tasks;
using static OrchardCore.Security.Permissions.Permission;

namespace Lombiq.HelpfulLibraries.OrchardCore.Users;

public class RoleCommands : DefaultCommandHandler
{
    private readonly RoleManager<IRole> _roleManager;
    private readonly ILogger<RoleCommands> _logger;

    [OrchardSwitch]
    public string RoleName { get; set; }

    [OrchardSwitch]
    public string Permission { get; set; }

    public RoleCommands(RoleManager<IRole> roleManager, IStringLocalizer<RoleCommands> localizer, ILogger<RoleCommands> logger)
        : base(localizer)
    {
        _roleManager = roleManager;
        _logger = logger;
    }

    [CommandName("addPermissionToRole")]
    [CommandHelp("addPermissionToRole " +
        "/RoleName:<rolename> " +
        "/Permission:<permission> " +
        "\r\n\t" + "Adds the permission to the role")]
    [OrchardSwitches("RoleName, Permission")]
    public async Task AddPermissionToRoleAsync()
    {
        if (await LookupRoleByNameAsync() is not { } role) return;
        role.RoleClaims.Add(new RoleClaim { ClaimType = ClaimType, ClaimValue = Permission });
        await _roleManager.UpdateAsync(role);
    }

    [CommandName("removePermissionFromRole")]
    [CommandHelp("removePermissionFromRole " +
                 "/RoleName:<rolename> " +
                 "/Permission:<permission> " +
                 "\r\n\t" + "Removes the permission from the role")]
    [OrchardSwitches("RoleName, Permission")]
    public async Task RemovePermissionFromRoleAsync()
    {
        if (await LookupRoleByNameAsync() is not { } role) return;
        role.RoleClaims.RemoveAll(claim => claim.ClaimType == ClaimType && claim.ClaimValue == Permission);
        await _roleManager.UpdateAsync(role);
    }

    private async Task<Role> LookupRoleByNameAsync()
    {
        if (await _roleManager.FindByNameAsync(_roleManager.NormalizeKey(RoleName)) is Role role)
        {
            return role;
        }

        _logger.LogError("Unable to find role \"{RoleName}\" to add permission \"{Permission}\" to it.", RoleName, Permission);
        return null;
    }
}
