using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Scripting;
using OrchardCore.Users.Models;
using OrchardCore.Users.Services;
using System;
using System.Collections.Generic;

namespace Lombiq.HelpfulLibraries.OrchardCore.MethodProviders;

public class UserMethodProvider : IGlobalMethodProvider
{
    private static readonly GlobalMethod _getUserIdByUserName = new()
    {
        Name = "getUserIdByUserName",
        Method = serviceProvider => (Func<string, string>)(email =>
        {
            var userService = serviceProvider.GetRequiredService<IUserService>();
            var user = userService.GetUserAsync(email).Result as User;
            var userId = user.Id.ToTechnicalString();

            return userId;
        }),
    };

    public IEnumerable<GlobalMethod> GetMethods()
    {
        yield return _getUserIdByUserName;
    }
}
