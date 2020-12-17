using OrchardCore.Users.Models;
using OrchardCore.Users.Services;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.Libraries.Users
{
    public class CachingUserService : ICachingUserService
    {
        private readonly Dictionary<string, User> _userByNameCache = new();
        private readonly Dictionary<string, User> _userByIdCache = new();

        private readonly IUserService _userService;

        public CachingUserService(IUserService userService) => _userService = userService;

        public async Task<User> GetUserByIdAsync(string userId) =>
            await GetUserAsync(
                userId,
                async () => await _userService.GetUserByUniqueIdAsync(userId) as User,
                _userByIdCache);

        public async Task<User> GetUserByNameAsync(string username) =>
            await GetUserAsync(
                username,
                async () => await _userService.GetUserAsync(username) as User,
                _userByNameCache);

        public async Task<User> GetUserByClaimsPrincipalAsync(ClaimsPrincipal claimsPrincipal) =>
            claimsPrincipal.Identity?.Name != null
                ? await GetUserAsync(
                    claimsPrincipal.Identity.Name,
                    async () => await _userService.GetAuthenticatedUserAsync(claimsPrincipal) as User,
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

            return user;
        }
    }
}
