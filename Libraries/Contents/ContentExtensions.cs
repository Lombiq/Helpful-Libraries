using System.Linq;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using System;
using Orchard.ContentManagement.Handlers;

namespace Piedone.HelpfulLibraries.Contents
{
    [OrchardFeature("Piedone.HelpfulLibraries.Contents")]
    public static class ContentExtensions
    {
        /// <summary>
        /// Retrieves the first field with the specified type from the part
        /// </summary>
        /// <typeparam name="T">Type of the field</typeparam>
        /// <param name="partName">Name of the part that contains the field</param>
        /// <returns>The field object</returns>
        public static T AsField<T>(this IContent content, string partName)
            where T : ContentField
        {
            return AsField<T>(content, partName, LookupField<T>(content, partName).Name);
        }

        /// <summary>
        /// Checks if the specified part contains the field with the specified type
        /// </summary>
        /// <typeparam name="T">Type of the field</typeparam>
        /// <param name="partName">Name of the part that contains the field</param>
        /// <returns>True if the part contains the field, false otherwise</returns>
        public static bool HasField<T>(this IContent content, string partName)
            where T : ContentField
        {
            return HasField(content, partName, LookupField<T>(content, partName).Name);
        }

        /// <summary>
        /// Retrieves the field with the specified name from the part
        /// </summary>
        /// <typeparam name="T">Type of the field</typeparam>
        /// <param name="partName">Name of the part that contains the field</param>
        /// <param name="fieldName">Name of the field</param>
        /// <returns>The field object</returns>
        public static T AsField<T>(this IContent content, string partName, string fieldName)
            where T : ContentField
        {
            // This is so the behaviour is consistent with Orchard.ContentManagement.ContentExtensions.As<>()
            if (content == null) return default(T);

            // Taken from Orchard.Tokens.Providers.ContentTokens
            return (T)content.ContentItem.Parts
                .Where(part => part.PartDefinition.Name == partName)
                .SelectMany(part => part.Fields.Where(field => field.Name == fieldName))
                .SingleOrDefault();
        }

        /// <summary>
        /// Checks if the specified part contains the specified field
        /// </summary>
        /// <param name="partName">Name of the part that contains the field</param>
        /// <param name="fieldName">Name of the field</param>
        /// <returns>True if the part contains the field, false otherwise</returns>
        public static bool HasField(this IContent content, string partName, string fieldName)
        {
            // This is so the behaviour is consistent with Orchard.ContentManagement.ContentExtensions.Has<>()
            if (content == null) return false;

            return content.ContentItem.Parts
                .Where(part => part.PartDefinition.Name == partName)
                .Any(part => part.Fields.Any(field => field.Name == fieldName));
        }

        /// <summary>
        /// Adds a content part to the content parts of a content item.
        /// </summary>
        /// <typeparam name="TPart">Type of the content part.</typeparam>
        /// <param name="content">The content object.</param>
        /// <param name="initializer">Optional initializer for the content part that will be run after the part is instantiated.</param>
        public static void Weld<TPart>(this IContent content, Action<TPart> initializer = null)
            where TPart : ContentPart, new()
        {
            content.Weld(new ContentItemBuilder(content.ContentItem.TypeDefinition).Weld<TPart>().Build().As<TPart>(), initializer);
        }

        /// <summary>
        /// Adds a content part to the content parts of a content item.
        /// </summary>
        /// <typeparam name="TPart">Type of the content part.</typeparam>
        /// <param name="content">The content object.</param>
        /// <param name="part">The content part object.</param>
        /// <param name="initializer">Optional initializer for the content part that will be run after the part is instantiated.</param>
        public static void Weld<TPart>(this IContent content, TPart part, Action<TPart> initializer = null)
            where TPart : ContentPart
        {
            if (initializer != null) initializer(part);
            content.ContentItem.Weld(part);
        }

        /// <summary>
        /// Saves a value to the transient context of the content. This context will be preserved as long as the content object
        /// lives.
        /// </summary>
        /// <typeparam name="T">Type of the value to save.</typeparam>
        /// <param name="key">The key to identify the value. If there is a value already saved under this key it will be overwritten.</param>
        /// <param name="value">The value to store in the context.</param>
        public static void SetContext<T>(this IContent content, string key, T value)
        {
            var contextPart = content.As<TransientContextPart>();
            if (contextPart == null)
            {
                content.Weld<TransientContextPart>();
                contextPart = content.As<TransientContextPart>();
            }
            contextPart.Context[key] = value;
        }

        /// <summary>
        /// Saves a value to the transient context of the content. This context will be preserved as long as the content object
        /// lives.
        /// </summary>
        /// <typeparam name="T">Type of the value to retrieve.</typeparam>
        /// <param name="key">The key to identify the value.</param>
        /// <returns>The stored value or the type's default if no value was found.</returns>
        public static T GetContext<T>(this IContent content, string key)
        {
            var contextPart = content.As<TransientContextPart>();
            if (contextPart == null || !contextPart.Context.ContainsKey(key)) return default(T);
            return (T)contextPart.Context[key];
        }


        private static T LookupField<T>(IContent content, string partName)
            where T : ContentField
        {
            var fieldType = typeof(T);
            return (T)content.ContentItem.Parts
                .Where(part => part.PartDefinition.Name == partName)
                .SelectMany(part => part.Fields.Where(field => field.GetType() == fieldType))
                .FirstOrDefault();
        }
    }
}
