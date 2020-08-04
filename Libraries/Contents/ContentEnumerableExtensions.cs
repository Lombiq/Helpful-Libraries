using System.Collections.Generic;
using System.Linq;

namespace OrchardCore.ContentManagement
{
    public static class ContentEnumerableExtensions
    {
        /// <summary>
        /// Retrieves an enumeration of a content part based on its type from an enumeration of content items.
        /// </summary>
        /// <returns>The content part enumeration or empty enumeration if it doesn't exist.</returns>
        public static IEnumerable<TPart> As<TPart>(this IEnumerable<IContent> contents) where TPart : ContentPart
        {
            var parts = contents?.Select(content => content.As<TPart>()) ?? Enumerable.Empty<TPart>();
            return parts.Any(content => content == null) ? Enumerable.Empty<TPart>() : parts;
        }
    }
}
