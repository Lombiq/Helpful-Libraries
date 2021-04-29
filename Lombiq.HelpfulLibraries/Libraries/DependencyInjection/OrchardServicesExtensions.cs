using Lombiq.HelpfulLibraries.Libraries.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OrchardCore.ContentManagement;
using OrchardCore.Modules;
using OrchardCore.Settings;
using System;

namespace Moq.AutoMock
{
    public static class OrchardServicesExtensions
    {
        public static void MockOrchardServices<T>(this AutoMocker mocker) =>
            mocker.Use<IOrchardServices<T>>(
                new OrchardServices<T>(
                    new Lazy<IClock>(mocker.Get<IClock>),
                    new Lazy<IContentHandleManager>(mocker.Get<IContentHandleManager>),
                    new Lazy<IContentManager>(mocker.Get<IContentManager>),
                    new Lazy<IHttpContextAccessor>(() => mocker.Get<IHttpContextAccessor>(true)),
                    new Lazy<ILogger<T>>(mocker.Get<ILogger<T>>),
                    new Lazy<YesSql.ISession>(mocker.Get<YesSql.ISession>),
                    new Lazy<ISiteService>(mocker.Get<ISiteService>),
                    new Lazy<IStringLocalizer<T>>(mocker.Get<IStringLocalizer<T>>)));
    }
}
