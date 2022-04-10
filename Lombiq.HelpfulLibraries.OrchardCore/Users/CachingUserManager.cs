using Microsoft.AspNetCore.Identity;
using OrchardCore.Users;
using OrchardCore.Users.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using YesSql;

namespace Lombiq.HelpfulLibraries.OrchardCore.Users;

public class CachingUserManager : ICachingUserManager
{
    private readonly Dictionary<string, User> _userByNameCache = new();
    private readonly Dictionary<string, User> _userByEmailCache = new();
    private readonly Dictionary<string, User> _userByIdCache = new();
    private readonly Dictionary<string, User> _userByUserIdCache = new();

    private readonly Lazy<UserManager<IUser>> _userManagerLazy;
    private readonly Lazy<ISession> _sessionLazy;

    // Injecting UserManager<IUser> lazily to avoid StackOverflowException when injecting ICachingUserManager to
    // ContentHandlers.
    public CachingUserManager(
        Lazy<UserManager<IUser>> userManagerLazy,
        Lazy<ISession> sessionLazy)
    {
        _userManagerLazy = userManagerLazy;
        _sessionLazy = sessionLazy;
    }

    public Task<User> GetUserByIdAsync(string id, bool forceUpdate = false) =>
        GetUserAsync(
            forceUpdate,
            id,
            () => int.TryParse(id, NumberStyles.Integer, CultureInfo.InvariantCulture, out var documentId)
                ? _sessionLazy.Value.GetAsync<User>(documentId)
                : null,
            _userByIdCache);

    public Task<User> GetUserByUserIdAsync(string userId, bool forceUpdate = false) =>
        GetUserAsync(
            forceUpdate,
            userId,
            async () => await _userManagerLazy.Value.FindByIdAsync(userId) as User,
            _userByUserIdCache);

    public Task<User> GetUserByNameAsync(string username, bool forceUpdate = false) =>
        GetUserAsync(
            forceUpdate,
            username,
            async () => await _userManagerLazy.Value.FindByNameAsync(username) as User,
            _userByNameCache);

    public Task<User> GetUserByEmailAsync(string email, bool forceUpdate = false) =>
        GetUserAsync(
            forceUpdate,
            email,
            async () => await _userManagerLazy.Value.FindByEmailAsync(email) as User,
            _userByEmailCache);

    public async Task<User> GetUserByClaimsPrincipalAsync(ClaimsPrincipal claimsPrincipal, bool forceUpdate = false) =>
        claimsPrincipal.Identity?.Name != null
            ? await GetUserAsync(
                forceUpdate,
                claimsPrincipal.Identity.Name,
                async () => await _userManagerLazy.Value.GetUserAsync(claimsPrincipal) as User,
                _userByNameCache)
            : null;

    private async Task<User> GetUserAsync(
        bool forceUpdate,
        string identifier,
        Func<Task<User>> factory,
        IDictionary<string, User> cache)
    {
        if (string.IsNullOrWhiteSpace(identifier)) return null;

        User user;
        if (forceUpdate)
        {
            user = await factory();
            cache.TryAdd(identifier, user);
        }
        else
        {
            user = await cache.GetValueOrAddIfMissingAsync(identifier, _ => factory());
        }

        if (string.IsNullOrEmpty(user?.UserId) ||
            string.IsNullOrEmpty(user.UserName) ||
            string.IsNullOrEmpty(user.Email))
        {
            return user;
        }

        if (!Equals(cache, _userByIdCache)) _userByIdCache.TryAdd(user.Id.ToTechnicalString(), user);
        if (!Equals(cache, _userByUserIdCache)) _userByUserIdCache.TryAdd(user.UserId, user);
        if (!Equals(cache, _userByNameCache)) _userByNameCache.TryAdd(user.UserName, user);
        if (!Equals(cache, _userByEmailCache)) _userByEmailCache.TryAdd(user.Email, user);

        return user;
    }
}
