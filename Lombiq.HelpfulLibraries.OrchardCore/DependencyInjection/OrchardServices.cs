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
using System.Diagnostics.CodeAnalysis;
using ISession = YesSql.ISession;

namespace Lombiq.HelpfulLibraries.OrchardCore.DependencyInjection;

public class OrchardServices<T> : IOrchardServices<T>
{
    public Lazy<IAuthorizationService> AuthorizationService { get; }
    public Lazy<IClock> Clock { get; }
    public Lazy<IContentHandleManager> ContentHandleManager { get; }
    public Lazy<IContentManager> ContentManager { get; }
    public Lazy<IHttpContextAccessor> HttpContextAccessor { get; }
    public Lazy<ILogger<T>> Logger { get; }
    public Lazy<ISession> Session { get; }
    public Lazy<ISiteService> SiteService { get; }
    public Lazy<IStringLocalizer<T>> StringLocalizer { get; }
    public Lazy<IHtmlLocalizer<T>> HtmlLocalizer { get; }
    public Lazy<UserManager<IUser>> UserManager { get; }

    [SuppressMessage(
        "Major Code Smell",
        "S107:Methods should not have too many parameters",
        Justification = "These are the most common Orchard services.")]
    public OrchardServices(
        Lazy<IAuthorizationService> authorizationService,
        Lazy<IClock> clock,
        Lazy<IContentHandleManager> contentHandleManager,
        Lazy<IContentManager> contentManager,
        Lazy<IHttpContextAccessor> httpContextAccessor,
        Lazy<ILogger<T>> logger,
        Lazy<ISession> session,
        Lazy<ISiteService> siteService,
        Lazy<IStringLocalizer<T>> stringLocalizer,
        Lazy<IHtmlLocalizer<T>> htmlLocalizer,
        Lazy<UserManager<IUser>> userManager)
    {
        AuthorizationService = authorizationService;
        Clock = clock;
        ContentHandleManager = contentHandleManager;
        ContentManager = contentManager;
        HttpContextAccessor = httpContextAccessor;
        Logger = logger;
        Session = session;
        SiteService = siteService;
        StringLocalizer = stringLocalizer;
        HtmlLocalizer = htmlLocalizer;
        UserManager = userManager;
    }
}
