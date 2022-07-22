namespace Microsoft.Extensions.Configuration;

public static class ConfigurationExtensions
{
    /// <summary>
    /// Retrieves a value indicating whether the <c>OrchardCore:IsAzureHosting</c> configuration key is set to <see
    /// langword="true"/>.
    /// </summary>
    public static bool IsAzureHosting(
        this IConfiguration configuration) =>
        configuration.GetValue<bool>("OrchardCore:IsAzureHosting");

    /// <summary>
    /// Retrieves a value indicating whether the <c>OrchardCore:IsMedia</c> configuration key is set to <see
    /// langword="true"/>.
    /// </summary>
    public static bool IsMedia(
        this IConfiguration configuration) =>
        configuration.GetValue<bool>("OrchardCore.Media");
}
