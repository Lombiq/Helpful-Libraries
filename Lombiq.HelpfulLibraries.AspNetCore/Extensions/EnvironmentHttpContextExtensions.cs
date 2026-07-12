using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Microsoft.AspNetCore.Http;

public static class EnvironmentHttpContextExtensions
{
    /// <summary>
    /// Returns <see langword="true"/> if the current <see cref="IHostEnvironment.EnvironmentName"/> is <see
    /// cref="Environments.Development"/>.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the current <see cref="IHostEnvironment.EnvironmentName"/> is <see
    /// cref="Environments.Development"/>.
    /// </returns>
    public static bool IsDevelopment(this HttpContext httpContext) =>
        httpContext.RequestServices.GetRequiredService<IHostEnvironment>().IsDevelopment();

    /// <summary>
    /// Returns <see langword="true"/> if the current host is localhost.
    /// </summary>
    /// <returns><see langword="true"/> if the current host is localhost.</returns>
    public static bool IsLocalhost(this HttpContext httpContext) =>
        httpContext.Request.Host.Host == "localhost";

    /// <summary>
    /// Returns <see langword="true"/> if the current <see cref="IHostEnvironment.EnvironmentName"/> is <see
    /// cref="Environments.Development"/> and the host is localhost.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the current <see cref="IHostEnvironment.EnvironmentName"/> is <see
    /// cref="Environments.Development"/> and the host is localhost.
    /// </returns>
    public static bool IsDevelopmentAndLocalhost(this HttpContext httpContext) =>
        httpContext.IsDevelopment() && httpContext.IsLocalhost();

    /// <summary>
    /// Returns the value for the <c>loading</c> HTML attribute of an <c>&lt;img&gt;</c> element.
    /// </summary>
    public static string GetImageLoadingStrategy(this HttpContext httpContext) =>
        httpContext.IsDevelopmentAndLocalhost() ? "eager" : "lazy";
}
