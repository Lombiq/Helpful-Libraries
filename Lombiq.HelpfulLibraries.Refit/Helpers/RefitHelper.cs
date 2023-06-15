using Refit;
using System;
using System.Net.Http;

namespace Lombiq.HelpfulLibraries.Refit.Helpers;

public static class RefitHelper
{
    /// <summary>
    /// Generate a Refit implementation of the specified interface with <c>Newtonsoft.Json</c> as the serializer instead
    /// of <c>System.Text.Json</c>.
    /// </summary>
    /// <param name="hostUrl">Base address the implementation will use.</param>
    /// <param name="configure">Optional action for configuring other settings.</param>
    /// <typeparam name="T">Interface to create the implementation for.</typeparam>
    /// <returns>An instance that implements <typeparamref name="T"/>.</returns>
    public static T WithNewtonsoftJson<T>(string hostUrl, Action<RefitSettings> configure = null) =>
        WithNewtonsoftJson<T>(new Uri(hostUrl), configure);

    /// <inheritdoc cref="WithNewtonsoftJson{T}(string,Action{RefitSettings})"/>
    public static T WithNewtonsoftJson<T>(Uri hostUrl, Action<RefitSettings> configure = null) =>
        RestService.For<T>(hostUrl.AbsoluteUri, CreateSettingsWithNewtonsoftJson(configure));

    /// <summary>
    /// Generate a Refit implementation of the specified interface with <c>Newtonsoft.Json</c> as the serializer instead
    /// of <c>System.Text.Json</c>.
    /// </summary>
    /// <param name="httpClient">The <see cref="HttpClient"/> the implementation will use to send requests.</param>
    /// <param name="configure">Optional action for configuring other settings.</param>
    /// <typeparam name="T">Interface to create the implementation for.</typeparam>
    /// <returns>An instance that implements <typeparamref name="T"/>.</returns>
    public static T WithNewtonsoftJson<T>(HttpClient httpClient, Action<RefitSettings> configure = null) =>
        RestService.For<T>(httpClient, CreateSettingsWithNewtonsoftJson(configure));

    private static RefitSettings CreateSettingsWithNewtonsoftJson(Action<RefitSettings> configure)
    {
        var settings = new RefitSettings(new NewtonsoftJsonContentSerializer());
        configure?.Invoke(settings);
        return settings;
    }
}
