using OrchardCore.Environment.Extensions.Features;
using System.Collections.Generic;
using System.Linq;

namespace OrchardCore.Environment.Shell;

/// <summary>
/// Shortcuts for <see cref="IEnumerable{IFeatureInfo}"/>.
/// </summary>
public static class FeatureInfoEnumerableExtensions
{
    /// <summary>
    /// Checks whether the given <see cref="IEnumerable{IFeatureInfo}"/> contains a feature with the given technical ID.
    /// </summary>
    /// <param name="featureId">Technical ID of the module feature.</param>
    public static bool Any(this IEnumerable<IFeatureInfo> featureInfos, string featureId) =>
        featureInfos.Any(feature => feature.Id == featureId);
}
