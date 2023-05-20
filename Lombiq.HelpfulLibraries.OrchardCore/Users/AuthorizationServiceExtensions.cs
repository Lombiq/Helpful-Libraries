using Microsoft.AspNetCore.Http;
using OrchardCore.Security.Permissions;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Authorization;

public static class AuthorizationServiceExtensions
{
    public static Task<bool> AuthorizeCurrentUserAsync(
        this IAuthorizationService service,
        HttpContext context,
        Permission permission) =>
        context?.User is { Identity.IsAuthenticated: true } user
            ? service.AuthorizeAsync(user, permission)
            : Task.FromResult(false);
}
