using OrchardCore.Users.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.Libraries.Users
{
    /// <summary>
    /// Retrieves <see cref="User"/>s from a transient per-request cache or sets them if they are not set yet.
    /// </summary>
    public interface ICachingUserManager
    {
        /// <summary>
        /// Retrieves <see cref="User"/>s from a transient per-request cache by their unique ID or gets them from the
        /// store if not yet cached.
        /// </summary>
        /// <param name="userId">Unique ID identifying the <see cref="User"/>.</param>
        /// <returns>Potentially cached <see cref="User"/>.</returns>
        Task<User> GetUserByIdAsync(string userId);

        /// <summary>
        /// Retrieves <see cref="User"/>s from a transient per-request cache by their username or gets them from the
        /// store if not yet cached.
        /// </summary>
        /// <param name="username">Username of the <see cref="User"/>.</param>
        /// <returns>Potentially cached <see cref="User"/>.</returns>
        Task<User> GetUserByNameAsync(string username);

        /// <summary>
        /// Retrieves <see cref="User"/>s from a transient per-request cache by their email or gets them from the store
        /// if not yet cached.
        /// </summary>
        /// <param name="email">Email of the <see cref="User"/>.</param>
        /// <returns>Potentially cached <see cref="User"/>.</returns>
        Task<User> GetUserByEmailAsync(string email);

        /// <summary>
        /// Retrieves an authenticated <see cref="User"/> from a transient per-request cache or gets them from the
        /// store if not yet cached.
        /// </summary>
        /// <param name="claimsPrincipal">
        /// <see cref="ClaimsPrincipal"/> representing the authenticated <see cref="User"/>.
        /// </param>
        /// <returns>Potentially cached <see cref="User"/>.</returns>
        Task<User> GetUserByClaimsPrincipalAsync(ClaimsPrincipal claimsPrincipal);
    }

    public static class CachingUserServiceExtensions
    {
        /// <summary>
        /// Retrieves <see cref="User"/>s from a transient per-request cache by their username or email or gets them
        /// from the store if not yet cached.
        /// </summary>
        /// <param name="nameOrEmail">Username or email of the <see cref="User"/>.</param>
        /// <returns>Potentially cached <see cref="User"/>.</returns>
        public static async Task<User> GetUserByNameOrEmailAsync(this ICachingUserManager manager, string nameOrEmail) =>
            await manager.GetUserByNameAsync(nameOrEmail) ?? await manager.GetUserByEmailAsync(nameOrEmail);
    }
}
