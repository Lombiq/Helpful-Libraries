using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OrchardCore.ContentManagement;
using OrchardCore.Modules;
using OrchardCore.Settings;
using System;
using YesSql;
using ISession = YesSql.ISession;

namespace Lombiq.HelpfulLibraries.Libraries.DependencyInjection
{
    public class OrchardServices<T> : IOrchardServices<T>
    {
        public Lazy<IClock> Clock { get; }
        public Lazy<IContentAliasManager> ContentAliasManager { get; }
        public Lazy<IContentManager> ContentManager { get; }
        public Lazy<IHttpContextAccessor> HttpContextAccessor { get; }
        public Lazy<ILogger<T>> Logger { get; }
        public Lazy<ISession> Session { get; }
        public Lazy<ISiteService> SiteService { get; }

        public OrchardServices(
            Lazy<IClock> clock,
            Lazy<IContentAliasManager> contentAliasManager,
            Lazy<IContentManager> contentManager,
            Lazy<IHttpContextAccessor> httpContextAccessor,
            Lazy<ILogger<T>> logger,
            Lazy<ISession> session,
            Lazy<ISiteService> siteService)
        {
            Clock = clock;
            ContentAliasManager = contentAliasManager;
            ContentManager = contentManager;
            HttpContextAccessor = httpContextAccessor;
            Logger = logger;
            Session = session;
            SiteService = siteService;
        }
    }
}
