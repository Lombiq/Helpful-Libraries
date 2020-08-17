using System;
using System.Linq;

namespace OrchardCore.ContentManagement.Metadata
{
    public static class ContentDefinitionManagerExtensions
    {
        /// <summary>
        /// Returns the content part settings object defined for the given content part on the given content type.
        /// </summary>
        /// <typeparam name="T">Type of the content part settings.</typeparam>
        /// <param name="contentType">Technical name of the content type.</param>
        /// <param name="contentPart">Technical name of the content part.</param>
        /// <returns>Content part settings object.</returns>
        public static T GetContentPartSettings<T>(
            this IContentDefinitionManager contentDefinitionManager,
            string contentType,
            string contentPart)
            where T : new()
        {
            var contentTypeDefinition = contentDefinitionManager.GetTypeDefinition(contentType);
            var contentTypePartDefinition = contentTypeDefinition.Parts
                .FirstOrDefault(x => string.Equals(x.PartDefinition.Name, contentPart, StringComparison.Ordinal));

            return contentTypePartDefinition.GetSettings<T>();
        }
    }
}
