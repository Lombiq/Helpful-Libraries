using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OrchardCore.Environment.Commands;
using OrchardCore.Security;
using System.Threading.Tasks;
using static OrchardCore.Security.Permissions.Permission;

namespace Lombiq.HelpfulLibraries.OrchardCore.Users;

public class RoleCommands(RoleManager<IRole> roleManager, IStringLocalizer<RoleCommands> localizer, ILogger<RoleCommands> logger)
    : DefaultCommandHandler(localizer)
{
    [OrchardSwitch]
    public string RoleName { get; set; }

    [OrchardSwitch]
    public string Permission { get; set; }

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
        await roleManager.UpdateAsync(role);
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
        await roleManager.UpdateAsync(role);
    }

    private async Task<Role> LookupRoleByNameAsync()
    {
        if (await roleManager.FindByNameAsync(roleManager.NormalizeKey(RoleName)) is Role role)
        {
            return role;
        }

        logger.LogError("Unable to find role \"{RoleName}\" to add permission \"{Permission}\" to it.", RoleName, Permission);
        return null;
    }
}
