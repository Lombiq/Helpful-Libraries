using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace OrchardCore.Security.Permissions;

public static class PermissionExtensions
{
    /// <summary>
    /// Returns the provided <paramref name="permissions"/> and recursively all of those listed in <see
    /// cref="Permission.ImpliedBy"/>.
    /// </summary>
    public static ICollection<Permission> WithImplicitPermissions(this IEnumerable<Permission> permissions)
    {
        static void Inspect(Dictionary<string, Permission> results, Permission permission)
        {
            results[permission.Name] = permission;
            permission.ImpliedBy?.ForEach(impliedBy => Inspect(results, impliedBy));
        }

        return permissions
            .AggregateSeed(new Dictionary<string, Permission>(StringComparer.OrdinalIgnoreCase), Inspect)
            .Values;
    }

    /// <summary>
    /// Returns the provided <paramref name="permission"/> and recursively all of those listed in <see
    /// cref="Permission.ImpliedBy"/>.
    /// </summary>
    public static ICollection<Permission> WithImplicitPermissions(this Permission permission) =>
        new[] { permission }.WithImplicitPermissions();

    /// <summary>
    /// Goes through all <paramref name="providers"/> and returns the permissions from them.
    /// </summary>
    public static async IAsyncEnumerable<Permission> GetAllPermissionsAsync(
        this IEnumerable<IPermissionProvider> providers,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (var provider in providers)
        {
            if (cancellationToken.IsCancellationRequested) yield break;

            var permissions = (await provider.GetPermissionsAsync()).AsList();

            if (permissions.Count <= 0) continue;

            foreach (var permission in permissions)
            {
                yield return permission;
            }
        }
    }

    /// <summary>
    /// Find the permissions with the given <paramref name="permissionName"/> by iterating through the <paramref
    /// name="providers"/>.
    /// </summary>
    public static ValueTask<Permission?> GetPermissionAsync(
        this IEnumerable<IPermissionProvider> providers,
        string? permissionName,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(permissionName)) return new(result: null);

        var permissions = providers
            .GetAllPermissionsAsync(cancellationToken)
            .Where(permission => permissionName.EqualsOrdinalIgnoreCase(permission.Name));

        return permissions.FirstOrDefaultAsync(cancellationToken);
    }
}
