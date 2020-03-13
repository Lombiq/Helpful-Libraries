﻿using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace OrchardCore.ContentManagement
{
    public static class ContentExtensions
    {
        /// <summary>
        /// Gets a content part by its type.
        /// </summary>
        /// <returns>The content part or <code>null</code> if it doesn't exist.</returns>
        public static TPart As<TPart>(this IContent content) where TPart : ContentPart =>
            content.ContentItem.As<TPart>();

        /// <summary>
        /// Gets a content part by its type or create a new one.
        /// </summary>
        /// <typeparam name="TPart">The type of the content part.</typeparam>
        /// <returns>The content part instance or a new one if it doesn't exist.</returns>
        public static TPart GetOrCreate<TPart>(this IContent content) where TPart : ContentPart, new() =>
            content.ContentItem.GetOrCreate<TPart>();

        /// <summary>
        /// Adds a content part by its type.
        /// </summary>
        /// <typeparam name="part">The part to add to the <see cref="ContentItem"/>.</typeparam>
        /// <returns>The current <see cref="IContent"/> instance.</returns>
        public static IContent Weld<TPart>(this IContent content, TPart part) where TPart : ContentPart =>
            content.ContentItem.Weld(part);

        /// <summary>
        /// Updates the content part with the specified type.
        /// </summary>
        /// <typeparam name="TPart">The type of the part to update.</typeparam>
        /// <returns>The current <see cref="IContent"/> instance.</returns>
        public static IContent Apply<TPart>(this IContent content, TPart part) where TPart : ContentPart =>
            content.ContentItem.Apply(part);

        /// <summary>
        /// Modifies a new or existing content part by name.
        /// </summary>
        /// <typeparam name="name">The name of the content part to update.</typeparam>
        /// <typeparam name="action">An action to apply on the content part.</typeparam>
        /// <returns>The current <see cref="IContent"/> instance.</returns>
        public static IContent Alter<TPart>(this IContent content, Action<TPart> action) where TPart : ContentPart, new() =>
            content.ContentItem.Alter(action);

        /// <summary>
        /// Modifies a new or existing content part by name.
        /// </summary>
        /// <typeparam name="name">The name of the content part to update.</typeparam>
        /// <typeparam name="action">An action to apply on the content part.</typeparam>
        /// <returns>The current <see cref="IContent"/> instance.</returns>
        public static async Task<IContent> AlterAsync<TPart>(this IContent content, Func<TPart, Task> action) where TPart : ContentPart, new() =>
            await content.ContentItem.AlterAsync(action);

        /// <summary>
        /// Merges properties to the contents of a content item.
        /// </summary>
        /// <typeparam name="properties">The object to merge.</typeparam>
        /// <returns>The modified <see cref="ContentItem"/> instance.</returns>
        public static IContent Merge(this IContent content, object properties, JsonMergeSettings jsonMergeSettings = null) =>
            content.ContentItem.Merge(properties, jsonMergeSettings);

    }

}
