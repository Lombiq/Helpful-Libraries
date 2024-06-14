using OrchardCore.Security.Permissions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.Users;

/// <summary>
/// A provider that has <c>Authenticated</c> stereotype permissions. Reduces boilerplate.
/// </summary>
public abstract class AuthenticatedPermissionBase : IPermissionProvider
{
    protected abstract IEnumerable<Permission> AuthenticatedPermissions { get; }

    /// <summary>
    /// Retrieves <c>authenticated</c> permissions.
    /// </summary>
    public Task<IEnumerable<Permission>> GetPermissionsAsync() =>
        Task.FromResult(AuthenticatedPermissions);

    /// <summary>
    /// Retrieves the default <c>authenticated</c> permission stereotypes.
    /// </summary>
    public IEnumerable<PermissionStereotype> GetDefaultStereotypes() =>
        [
            new PermissionStereotype
            {
                Name = "Authenticated",
                Permissions = AuthenticatedPermissions,
            },
        ];
}
