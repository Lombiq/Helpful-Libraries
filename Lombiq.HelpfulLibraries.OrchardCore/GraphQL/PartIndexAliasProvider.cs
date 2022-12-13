using GraphQL;
using OrchardCore.ContentManagement.GraphQL.Queries;
using System;
using System.Collections.Generic;
using YesSql.Indexes;

namespace Lombiq.HelpfulLibraries.OrchardCore.GraphQL;

/// <summary>
/// Eliminates boilerplate for <see cref="IIndexAliasProvider"/> for indexes with a name ending with <c>PartIndex</c>.
/// </summary>
/// <typeparam name="TIndex">A content part index with a name ending with <c>PartIndex</c>.</typeparam>
public class PartIndexAliasProvider<TIndex> : IIndexAliasProvider
    where TIndex : class, IIndex
{
    private static readonly IndexAlias[] _aliases =
    {
        new()
        {
            Alias = typeof(TIndex)
                .Name
                .Replace("PartIndex", string.Empty, StringComparison.Ordinal)
                .ToCamelCase(),
            Index = typeof(TIndex).Name,
            IndexType = typeof(TIndex),
        },
    };

    /// <summary>
    /// Gets indexes with a name ending in <c>PartIndex</c>.
    /// </summary>
    public IEnumerable<IndexAlias> GetAliases() => _aliases;
}
