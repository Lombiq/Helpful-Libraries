using Lombiq.HelpfulLibraries.Libraries.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OrchardCore.ContentManagement;
using OrchardCore.Environment.Shell.Configuration;
using OrchardCore.Modules;
using OrchardCore.Settings;
using OrchardCore.Users;
using System;

namespace Moq.AutoMock
{
    public static class OrchardServicesExtensions
    {
        public static void MockOrchardServices<T>(this AutoMocker mocker) =>
            mocker.Use<IOrchardServices<T>>(
                new OrchardServices<T>(
                    new Lazy<IAuthorizationService>(mocker.Get<IAuthorizationService>),
                    new Lazy<IClock>(mocker.Get<IClock>),
                    new Lazy<IContentAliasManager>(mocker.Get<IContentAliasManager>),
                    new Lazy<IContentManager>(mocker.Get<IContentManager>),
                    new Lazy<IHttpContextAccessor>(() => mocker.Get<IHttpContextAccessor>(true)),
                    new Lazy<ILogger<T>>(mocker.Get<ILogger<T>>),
                    new Lazy<YesSql.ISession>(mocker.Get<YesSql.ISession>),
                    new Lazy<ISiteService>(mocker.Get<ISiteService>),
                    new Lazy<IStringLocalizer<T>>(mocker.Get<IStringLocalizer<T>>),
                    new Lazy<IHtmlLocalizer<T>>(mocker.Get<IHtmlLocalizer<T>>),
                    new Lazy<UserManager<IUser>>(mocker.Get<UserManager<IUser>>),
                    new Lazy<IShellConfiguration>(mocker.Get<IShellConfiguration>)));
    }
}
