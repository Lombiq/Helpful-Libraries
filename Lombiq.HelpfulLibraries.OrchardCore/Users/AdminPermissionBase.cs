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

    /// <summary>
    /// Retrieves <c>admin</c> permissions.
    /// </summary>
    public Task<IEnumerable<Permission>> GetPermissionsAsync() =>
        Task.FromResult(AdminPermissions);

    /// <summary>
    /// Retrieves the default <c>admin</c> permission stereotypes.
    /// </summary>
    public IEnumerable<PermissionStereotype> GetDefaultStereotypes() =>
        [
            new PermissionStereotype
            {
                Name = "Administrator",
                Permissions = AdminPermissions,
            },
        ];
}
