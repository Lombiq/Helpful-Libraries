using System;
using System.Threading.Tasks;
using OrchardCore.Users.Models;

namespace OrchardCore.Users.Services
{
    public static class UserServiceExtensions
    {
        public static async Task<User> GetOrchardUserAsync(this IUserService userService, string userName)
        {
            var user = await userService.GetUserAsync(userName);
            if (user == null) return null;

            var orchardUser = user as User;
            if (orchardUser == null)
                throw new ArgumentException("The given username identifies a non-Orchard user.", nameof(userName));

            return orchardUser;
        }
    }
}
