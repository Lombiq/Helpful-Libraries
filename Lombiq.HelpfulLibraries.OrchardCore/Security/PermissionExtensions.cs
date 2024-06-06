using System;
using System.Collections.Generic;

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
}
