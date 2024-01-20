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

[method: SuppressMessage(
        "Major Code Smell",
        "S107:Methods should not have too many parameters",
        Justification = "These are the most common Orchard services.")]
public class OrchardServices<T>(
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
    Lazy<UserManager<IUser>> userManager) : IOrchardServices<T>
{
    public Lazy<IAuthorizationService> AuthorizationService { get; } = authorizationService;
    public Lazy<IClock> Clock { get; } = clock;
    public Lazy<IContentHandleManager> ContentHandleManager { get; } = contentHandleManager;
    public Lazy<IContentManager> ContentManager { get; } = contentManager;
    public Lazy<IHttpContextAccessor> HttpContextAccessor { get; } = httpContextAccessor;
    public Lazy<ILogger<T>> Logger { get; } = logger;
    public Lazy<ISession> Session { get; } = session;
    public Lazy<ISiteService> SiteService { get; } = siteService;
    public Lazy<IStringLocalizer<T>> StringLocalizer { get; } = stringLocalizer;
    public Lazy<IHtmlLocalizer<T>> HtmlLocalizer { get; } = htmlLocalizer;
    public Lazy<UserManager<IUser>> UserManager { get; } = userManager;
}
