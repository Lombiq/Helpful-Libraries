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

    /// <summary>
    /// Creates a new instance of <typeparamref name="T"/> and binds it the provided configuration.
    /// </summary>
    /// <param name="configuration">The base configuration.</param>
    /// <param name="sectionKey">
    /// If <see langword="null"/> or empty, then <paramref name="configuration"/> is used, otherwise the section with
    /// this key inside it.
    /// </param>
    public static T BindNew<T>(this IConfiguration configuration, string sectionKey = null)
        where T : new()
    {
        var options = new T();
        var section = string.IsNullOrEmpty(sectionKey) ? configuration : configuration.GetSection(sectionKey);
        section.Bind(options);
        return options;
    }
}
