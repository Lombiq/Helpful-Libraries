using OrchardCore.Users.Models;
using System;
using System.Threading.Tasks;

namespace OrchardCore.Users.Services
{
    public static class UserServiceExtensions
    {
        public static async Task<User> GetOrchardUserAsync(this IUserService userService, string userName)
        {
            var user = await userService.GetUserAsync(userName);

            // Would result in a multi-level ternary.
#pragma warning disable IDE0046 // Convert to conditional expression
            if (user == null) return null;

            return !(user is User orchardUser)
                ? throw new ArgumentException("The given username identifies a non-Orchard user.", nameof(userName))
                : orchardUser;
#pragma warning restore IDE0046 // Convert to conditional expression
        }
    }
}
