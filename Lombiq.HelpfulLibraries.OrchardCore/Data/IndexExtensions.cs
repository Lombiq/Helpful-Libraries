using OrchardCore.ContentManagement;
using System;

namespace YesSql.Indexes;

public static class IndexExtensions
{
    public static IGroupFor<TIndex> Map<TPart, TIndex>(this IMapFor<ContentItem, TIndex> mapFor, Func<TPart, TIndex> map)
        where TPart : ContentPart
        where TIndex : IIndex =>
        mapFor
            .When(item => item.Has<TPart>())
            .Map(item => map(item.As<TPart>()));
}
