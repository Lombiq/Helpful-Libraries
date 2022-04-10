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

/// <summary>
/// A convenience bundle of services that are common dependencies of other CMS services in Orchard Core.
/// </summary>
/// <typeparam name="T">The type of the dependant service, used for logger.</typeparam>
[SuppressMessage(
    "StyleCop.CSharp.DocumentationRules",
    "SA1600:Elements should be documented",
    Justification = "There is nothing to add past what's already on the individual services' documentations.")]
public interface IOrchardServices<T>
{
    Lazy<IAuthorizationService> AuthorizationService { get; }
    Lazy<IClock> Clock { get; }
    Lazy<IContentHandleManager> ContentHandleManager { get; }
    Lazy<IContentManager> ContentManager { get; }
    Lazy<IHttpContextAccessor> HttpContextAccessor { get; }
    Lazy<ILogger<T>> Logger { get; }
    Lazy<ISession> Session { get; }
    Lazy<ISiteService> SiteService { get; }
    Lazy<IStringLocalizer<T>> StringLocalizer { get; }
    Lazy<IHtmlLocalizer<T>> HtmlLocalizer { get; }
    Lazy<UserManager<IUser>> UserManager { get; }
}
