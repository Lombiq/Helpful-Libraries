namespace Microsoft.Extensions.Configuration;

/// <summary>
/// Shortcuts for <see cref="IConfigurationSection"/> operations.
/// </summary>
public static class ConfigurationSectionExtensions
{
    /// <summary>
    /// Adds a value to a configuration section if the key doesn't exist yet.
    /// </summary>
    /// <param name="key">The key of the configuration.</param>
    /// <param name="value">The value of the configuration.</param>
    public static IConfigurationSection AddValueIfKeyNotExists(
        this IConfigurationSection configurationSection,
        string key,
        string value)
    {
        configurationSection[key] ??= value;
        return configurationSection;
    }
}
