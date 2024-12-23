#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace Lombiq.HelpfulLibraries.Common.Utilities;

/// <summary>
/// Groups <see cref="Version"/> instances into a tree structure based on the specified version parts, so they can be
/// selected by number using indexing.
/// </summary>
/// <param name="Versions">All versions in the current subtree.</param>
/// <param name="SubVersions">Versions grouped by the directly next version part.</param>
public record VersionTree(IReadOnlyList<Version> Versions, IReadOnlyDictionary<int, VersionTree> SubVersions)
{
    /// <summary>
    /// Gets the subtree for the provided key, if it exists (otherwise <see langword="null"/>). If the <paramref
    /// name="index"/> is negative, this instance is returned instead.
    /// </summary>
    /// <remarks><para><see cref="Version"/> uses <c>-1</c> to indicate a floating version part, so you can chain all 4
    /// version parts using null-conditional indexers (<c>?[]</c>).</para></remarks>
    public VersionTree? this[int index]
    {
        get
        {
            if (index < 0) return this;
            return SubVersions.TryGetValue(index, out var sub) ? sub : null;
        }
    }

    /// <summary>
    /// Creates a new tree from a copy of the provided <paramref name="versions"/>.
    /// </summary>
    public static VersionTree Create(IEnumerable<Version> versions)
    {
        var allVersions = versions.ToList();

        var majorVersions =
            FromVersions(allVersions, version => version.Major, major =>
                FromVersions(major, version => version.Minor, minor =>
                    FromVersions(minor, version => version.Build, _ => [])));

        return new(allVersions, majorVersions);
    }

    private static Dictionary<int, VersionTree> FromVersions(
        IEnumerable<Version> versions,
        Func<Version, int> selector,
        Func<IEnumerable<Version>, Dictionary<int, VersionTree>> subSelector) =>
        versions
            .Where(version => selector(version) >= 0)
            .GroupBy(selector)
            .ToDictionary(
                group => group.Key,
                items => new VersionTree([.. items], subSelector(items)));
}
