using OrchardCore.ContentManagement;
using System;

namespace YesSql.Indexes;

public static class IndexExtensions
{
    /// <summary>
    /// Describes a <see cref="ContentItem"/> index to the specified <see cref="ContentPart"/>.
    /// </summary>
    /// <param name="map">Maps the <typeparamref name="TPart"/> to the <typeparamref name="TIndex"/>.</param>
    /// <param name="latest">
    /// If <see langword="true"/> it only applies when <see cref="ContentItem.Latest"/> is <see langword="true"/>.
    /// </param>
    public static IGroupFor<TIndex> MapFor<TPart, TIndex>(
        this DescribeContext<ContentItem> context,
        Func<TPart, TIndex> map,
        bool latest = true)
        where TPart : ContentPart
        where TIndex : IIndex =>
        context.For<TIndex>().Map(map, latest);

    /// <summary>
    /// Filters and maps a <see cref="ContentItem"/> index to the specified <see cref="ContentPart"/>.
    /// </summary>
    /// <inheritdoc cref="MapFor{TPart,TIndex}"/>
    public static IGroupFor<TIndex> Map<TPart, TIndex>(
        this IMapFor<ContentItem, TIndex> mapFor,
        Func<TPart, TIndex> map,
        bool latest = true)
        where TPart : ContentPart
        where TIndex : IIndex =>
        mapFor
            .When(item => item.Has<TPart>() && (item.Latest || !latest))
            .Map(item => map(item.As<TPart>()));
}
