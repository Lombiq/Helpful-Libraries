using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OrchardCore.ContentManagement;
using OrchardCore.Modules;
using OrchardCore.Settings;
using System;
using ISession = YesSql.ISession;

namespace Lombiq.HelpfulLibraries.Libraries.DependencyInjection
{
    public class OrchardServices<T> : IOrchardServices<T>
    {
        public Lazy<IClock> Clock { get; }
        public Lazy<IContentHandleManager> ContentHandleManager { get; }
        public Lazy<IContentManager> ContentManager { get; }
        public Lazy<IHttpContextAccessor> HttpContextAccessor { get; }
        public Lazy<ILogger<T>> Logger { get; }
        public Lazy<ISession> Session { get; }
        public Lazy<ISiteService> SiteService { get; }
        public Lazy<IStringLocalizer<T>> StringLocalizer { get; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Major Code Smell",
            "S107:Methods should not have too many parameters",
            Justification = "These are the most common Orchard services.")]
        public OrchardServices(
            Lazy<IClock> clock,
            Lazy<IContentHandleManager> contentHandleManager,
            Lazy<IContentManager> contentManager,
            Lazy<IHttpContextAccessor> httpContextAccessor,
            Lazy<ILogger<T>> logger,
            Lazy<ISession> session,
            Lazy<ISiteService> siteService,
            Lazy<IStringLocalizer<T>> stringLocalizer)
        {
            Clock = clock;
            ContentHandleManager = contentHandleManager;
            ContentManager = contentManager;
            HttpContextAccessor = httpContextAccessor;
            Logger = logger;
            Session = session;
            SiteService = siteService;
            StringLocalizer = stringLocalizer;
        }
    }
}
