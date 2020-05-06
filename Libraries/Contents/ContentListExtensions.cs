using System.Collections.Generic;
using System.Linq;

namespace OrchardCore.ContentManagement
{
    public static class ContentListExtensions
    {
        /// <summary>
        /// Gets a content part list by its type.
        /// </summary>
        /// <returns>The content part list or empty list if it doesn't exist.</returns>
        public static IEnumerable<TPart> As<TPart>(this IEnumerable<IContent> contents) where TPart : ContentPart
        {
            var list = contents?.Select(content => content.As<TPart>()) ?? Enumerable.Empty<TPart>();

            return list.Any(content => content == null) ? Enumerable.Empty<TPart>() : list;
        }
    }
}
