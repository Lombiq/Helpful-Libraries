using Refit;
using System;
using System.Net.Http;

namespace Lombiq.HelpfulLibraries.Refit.Helpers;

public static class RefitHelper
{
    public static T WithNewtonsoftJson<T>(string hostUrl, Action<RefitSettings> configure = null) =>
        WithNewtonsoftJson<T>(new Uri(hostUrl), configure);

    public static T WithNewtonsoftJson<T>(Uri hostUrl, Action<RefitSettings> configure = null) =>
        RestService.For<T>(hostUrl.AbsoluteUri, CreateSettingsWithNewtonsoftJson(configure));

    public static T WithNewtonsoftJson<T>(HttpClient httpClient, Action<RefitSettings> configure = null) =>
        RestService.For<T>(httpClient, CreateSettingsWithNewtonsoftJson(configure));

    private static RefitSettings CreateSettingsWithNewtonsoftJson(Action<RefitSettings> configure)
    {
        var settings = new RefitSettings(new NewtonsoftJsonContentSerializer());
        configure?.Invoke(settings);
        return settings;
    }
}
