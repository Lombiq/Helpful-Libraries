using Microsoft.AspNetCore.Identity;
using OrchardCore.Users;
using OrchardCore.Users.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.Libraries.Users
{
    public class CachingUserManager : ICachingUserManager
    {
        private readonly Dictionary<string, User> _userByNameCache = new();
        private readonly Dictionary<string, User> _userByEmailCache = new();
        private readonly Dictionary<string, User> _userByIdCache = new();

        private readonly UserManager<IUser> _userManager;

        public CachingUserManager(UserManager<IUser> userManager) => _userManager = userManager;

        public Task<User> GetUserByIdAsync(string userId) =>
            GetUserAsync(
                userId,
                async () => await _userManager.FindByIdAsync(userId) as User,
                _userByIdCache);

        public Task<User> GetUserByNameAsync(string username) =>
            GetUserAsync(
                username,
                async () => await _userManager.FindByNameAsync(username) as User,
                _userByNameCache);

        public Task<User> GetUserByEmailAsync(string email) =>
            GetUserAsync(
                email,
                async () => await _userManager.FindByEmailAsync(email) as User,
                _userByEmailCache);

        public async Task<User> GetUserByClaimsPrincipalAsync(ClaimsPrincipal claimsPrincipal) =>
            claimsPrincipal.Identity?.Name != null
                ? await GetUserAsync(
                    claimsPrincipal.Identity.Name,
                    async () => await _userManager.GetUserAsync(claimsPrincipal) as User,
                    _userByNameCache)
                : null;

        private async Task<User> GetUserAsync(
            string identifier,
            Func<Task<User>> factory,
            IDictionary<string, User> cache)
        {
            var user = await cache.GetValueOrAddIfMissingAsync(
                identifier,
                _ => factory());

            if (user == null) return null;

            if (!Equals(cache, _userByIdCache)) _userByIdCache.TryAdd(user.Id.ToTechnicalString(), user);
            if (!Equals(cache, _userByNameCache)) _userByNameCache.TryAdd(user.UserName, user);
            if (!Equals(cache, _userByEmailCache)) _userByEmailCache.TryAdd(user.Email, user);

            return user;
        }
    }
}
