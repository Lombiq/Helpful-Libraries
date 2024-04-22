using System.Threading.Tasks;

namespace OrchardCore.Environment.Shell;

/// <summary>
/// Shortcuts for <see cref="IShellFeaturesManager"/>.
/// </summary>
public static class ShellFeaturesManagerExtensions
{
    /// <summary>
    /// Checks whether the given module feature is enabled for the current shell.
    /// </summary>
    /// <param name="featureId">Technical ID of the module feature.</param>
    public static async Task<bool> IsFeatureEnabledAsync(this IShellFeaturesManager shellFeaturesManager, string featureId) =>
        (await shellFeaturesManager.GetEnabledFeaturesAsync()).Any(featureId);
}
