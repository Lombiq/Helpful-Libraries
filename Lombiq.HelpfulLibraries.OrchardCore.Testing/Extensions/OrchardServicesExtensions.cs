using Lombiq.HelpfulLibraries.OrchardCore.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OrchardCore.ContentManagement;
using OrchardCore.Modules;
using OrchardCore.Settings;
using OrchardCore.Users;
using System;

namespace Moq.AutoMock;

public static class OrchardServicesExtensions
{
    /// <summary>
    /// Sets up auto mocking for <see cref="IOrchardServices{T}" />.
    /// </summary>
    /// <typeparam name="T">The type of the dependant service, used for logger.</typeparam>
    public static void MockOrchardServices<T>(this AutoMocker mocker) =>
        mocker.Use<IOrchardServices<T>>(
            new OrchardServices<T>(
                new Lazy<IAuthorizationService>(mocker.Get<IAuthorizationService>),
                new Lazy<IClock>(mocker.Get<IClock>),
                new Lazy<IContentHandleManager>(mocker.Get<IContentHandleManager>),
                new Lazy<IContentManager>(mocker.Get<IContentManager>),
                new Lazy<IHttpContextAccessor>(() => mocker.Get<IHttpContextAccessor>(enablePrivate: true)),
                new Lazy<ILogger<T>>(mocker.Get<ILogger<T>>),
                new Lazy<YesSql.ISession>(mocker.Get<YesSql.ISession>),
                new Lazy<ISiteService>(mocker.Get<ISiteService>),
                new Lazy<IStringLocalizer<T>>(mocker.Get<IStringLocalizer<T>>),
                new Lazy<IHtmlLocalizer<T>>(mocker.Get<IHtmlLocalizer<T>>),
                new Lazy<UserManager<IUser>>(mocker.Get<UserManager<IUser>>)));
}
