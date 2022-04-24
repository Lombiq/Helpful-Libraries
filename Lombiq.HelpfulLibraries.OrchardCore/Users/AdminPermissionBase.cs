using OrchardCore.Security.Permissions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.Users;

/// <summary>
/// A provider that only has <c>Administrator</c> stereotype permissions. Reduces boilerplate.
/// </summary>
public abstract class AdminPermissionBase : IPermissionProvider
{
    protected abstract IEnumerable<Permission> AdminPermissions { get; }

    public Task<IEnumerable<Permission>> GetPermissionsAsync() =>
        Task.FromResult(AdminPermissions);

    public IEnumerable<PermissionStereotype> GetDefaultStereotypes() =>
        new[]
        {
            new PermissionStereotype
            {
                Name = "Administrator",
                Permissions = AdminPermissions,
            },
        };
}
