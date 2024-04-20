using OrchardCore.ContentManagement;
using System;

namespace YesSql.Indexes;

public static class IndexExtensions
{
    /// <summary>
    /// Describes a <see cref="ContentItem"/> index to the specified <see cref="ContentPart"/>.
    /// </summary>
    public static IGroupFor<TIndex> MapFor<TPart, TIndex>(this DescribeContext<ContentItem> context, Func<TPart, TIndex> map)
        where TPart : ContentPart
        where TIndex : IIndex =>
        context.For<TIndex>().Map(map);

    /// <summary>
    /// Filters and maps a <see cref="ContentItem"/> index to the specified <see cref="ContentPart"/>.
    /// </summary>
    public static IGroupFor<TIndex> Map<TPart, TIndex>(this IMapFor<ContentItem, TIndex> mapFor, Func<TPart, TIndex> map)
        where TPart : ContentPart
        where TIndex : IIndex =>
        mapFor
            .When(item => item.Has<TPart>())
            .Map(item => map(item.As<TPart>()));
}
