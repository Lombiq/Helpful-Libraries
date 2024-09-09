using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OrchardCore.Environment.Shell.Configuration;

public static class ShellConfigurationExtensions
{
    /// <summary>
    /// Binds the section of <paramref name="key"/> in the shell configuration to a new instance of <typeparamref
    /// name="T"/> and binds the configuration section to the <paramref name="services"/>.
    /// </summary>
    public static T Configure<T>(this IShellConfiguration shellConfiguration, IServiceCollection services, string key)
        where T : class, new()
    {
        var configSection = shellConfiguration.GetSection(key);
        var options = configSection.BindNew<T>();
        services.Configure<T>(configSection);

        return options;
    }
}
