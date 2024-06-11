using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.Security.Permissions;
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

    /// <summary>
    /// Authorizes the current user against the provided <paramref name="permissions"/>, performs additional optional
    /// validation and if all passes returns <paramref name="executeAsync"/>.
    /// </summary>
    /// <param name="service">The service used for authorization.</param>
    /// <param name="controller">The controller whose current context is used for authorization and results..</param>
    /// <param name="permissions">The <see cref="Controller.User"/> must pass each of these permissions.</param>
    /// <param name="validateAsync">
    /// An optional fetching or validation step, if it's <see langword="null"/> then it's ignored and <see
    /// langword="default"/> is passed to the <paramref name="executeAsync"/>. The first value of its result must be <
    /// see langword="true"/>, otherwise <see cref="NotFoundResult"/> is returned. Use this for example to query a value
    /// from the database or to simply return an opaque failure state.
    /// </param>
    /// <param name="executeAsync">
    /// The method that returns the final result if everything is ok. If the result is <see cref="IActionResult"/> then
    /// it's returned as-is, otherwise it's passed to <see cref="ControllerBase.Ok()"/>.
    /// </param>
    /// <param name="authenticationScheme">
    /// The information used by <see cref="ChallengeResult"/> or <see cref="ForbidResult"/> if the authorization fails.
    /// This is conventionally <c>Api</c> so usually the parameter can be skipped.
    /// </param>
    /// <typeparam name="TData">
    /// The type of the intermediate result received from <paramref name="validateAsync"/> and passed to <paramref
    /// name="executeAsync"/>.
    /// </typeparam>
    /// <typeparam name="TResult">The final outcome.</typeparam>
    public static async Task<IActionResult> AuthorizeForCurrentUserValidateAndExecuteAsync<TData, TResult>(
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

    /// <inheritdoc cref="AuthorizeForCurrentUserValidateAndExecuteAsync{TData,TResult}"/>
    public static Task<IActionResult> AuthorizeForCurrentUserAndExecuteAsync<TResult>(
        this IAuthorizationService service,
        Controller controller,
        IEnumerable<Permission> permissions,
        Func<Task<TResult>> executeAsync,
        string authenticationScheme = "Api") =>
        service.AuthorizeForCurrentUserValidateAndExecuteAsync<object, TResult>(
            controller,
            permissions,
            validateAsync: null,
            _ => executeAsync(),
            authenticationScheme);

    /// <inheritdoc cref="AuthorizeForCurrentUserValidateAndExecuteAsync{TData,TResult}"/>
    public static Task<IActionResult> AuthorizeForCurrentUserValidateNotNullAndExecuteAsync<TData, TResult>(
        this IAuthorizationService service,
        Controller controller,
        IEnumerable<Permission> permissions,
        Func<Task<TData>> validateAsync,
        Func<TData, Task<TResult>> executeAsync,
        string authenticationScheme = "Api")
        where TData : class =>
        service.AuthorizeForCurrentUserValidateAndExecuteAsync(
            controller,
            permissions,
            async () => await validateAsync() is { } data ? (true, data) : (false, default),
            executeAsync,
            authenticationScheme);
}
