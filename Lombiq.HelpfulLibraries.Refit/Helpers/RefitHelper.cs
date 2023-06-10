using Refit;
using System;
using System.Net.Http;

namespace Lombiq.HelpfulLibraries.Refit.Helpers;

public static class RefitHelper
{
    public static T WithNewtonsoft<T>(string hostUrl, Action<RefitSettings> configure = null) =>
        WithNewtonsoft<T>(new Uri(hostUrl), configure);

    public static T WithNewtonsoft<T>(Uri hostUrl, Action<RefitSettings> configure = null) =>
        RestService.For<T>(hostUrl.AbsoluteUri, CreateSettingsWithNewtonsoft(configure));

    public static T WithNewtonsoft<T>(HttpClient httpClient, Action<RefitSettings> configure = null) =>
        RestService.For<T>(httpClient, CreateSettingsWithNewtonsoft(configure));

    private static RefitSettings CreateSettingsWithNewtonsoft(Action<RefitSettings> configure)
    {
        var settings = new RefitSettings(new NewtonsoftJsonContentSerializer());
        configure?.Invoke(settings);
        return settings;
    }
}
