using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.Security.Permissions;
using OrchardCore.Users.Models;
using System;
using System.Collections.Generic;
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

    public static async Task<IActionResult> AuthorizeForCurrentUserAndExecuteAsync<TData, TResult>(
        this IAuthorizationService service,
        Controller controller,
        IEnumerable<Permission> permissions,
        Func<Task<(bool IsSuccess, TData Data)>> validateAsync,
        Func<TData, Task<TResult>> executeAsync,
        string authenticationScheme = "Api")
    {
        foreach (var permission in permissions)
        {
            if (!await service.AuthorizeAsync(controller.User, permission))
            {
                return controller.ChallengeOrForbid(authenticationScheme);
            }
        }

        var (isSuccess, data) = validateAsync is null ? (true, default) : await validateAsync();
        if (!isSuccess) return controller.NotFound();

        var result = await executeAsync(data);
        return result is IActionResult actionResult ? actionResult : controller.Ok(result);
    }

    public static Task<IActionResult> AuthorizeForCurrentUserValidateNotNullAndExecuteAsync<TData, TResult>(
        this IAuthorizationService service,
        Controller controller,
        IEnumerable<Permission> permissions,
        Func<Task<TData>> validateAsync,
        Func<TData, Task<TResult>> executeAsync,
        string authenticationScheme = "Api")
        where TData : class =>
        service.AuthorizeForCurrentUserAndExecuteAsync<TData, TResult>(
            controller,
            permissions,
            async () => await validateAsync() is { } data ? (true, data) : (false, default),
            executeAsync,
            authenticationScheme);
}
