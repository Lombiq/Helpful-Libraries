using OrchardCore.Users.Models;
using System;
using System.Threading.Tasks;

namespace OrchardCore.Users.Services;

public static class UserServiceExtensions
{
    /// <summary>
    /// Retrieves an Orchard user by <paramref name="userName"/> or throws an exception if none were found.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// Thrown if the given <paramref name="userName"/> identifies a non-Orchard user.
    /// </exception>
    public static async Task<User> GetOrchardUserAsync(this IUserService userService, string userName)
    {
        var user = await userService.GetUserAsync(userName);

        if (user == null) return null;

        return user is not User orchardUser
            ? throw new ArgumentException("The given user name identifies a non-Orchard user.", nameof(userName))
            : orchardUser;
    }
}
