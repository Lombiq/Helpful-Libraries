using System;
using System.Collections.Generic;

namespace OrchardCore.ContentManagement;

public static class ContentEnumerableExtensions
{
    /// <summary>
    /// Retrieves an enumeration of a content part based on its type from an enumeration of content items.
    /// </summary>
    /// <returns>The content part enumeration or empty enumeration if it doesn't exist.</returns>
    [Obsolete($"Use {nameof(GetOrCreate)} instead.")]
    public static IEnumerable<TPart> As<TPart>(this IEnumerable<IContent>? contents)
        where TPart : ContentPart =>
        (contents?.SelectWhere(content => content.As<TPart>())).EmptyIfNull();

    /// <summary>
    /// Retrieves an enumeration of a content part based on its type from an enumeration of content items.
    /// </summary>
    /// <returns>The content part enumeration or empty enumeration if it doesn't exist.</returns>
    public static IEnumerable<TPart> GetOrCreate<TPart>(this IEnumerable<IContent>? contents)
        where TPart : ContentPart, new() =>
        (contents?.SelectWhere(content => content.GetOrCreate<TPart>())).EmptyIfNull();
}
