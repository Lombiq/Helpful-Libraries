using System;
using System.Linq;

namespace OrchardCore.ContentManagement.Metadata
{
    public static class ContentDefinitionManagerExtensions
    {
        public static T GetContentPartSettings<T>(
            this IContentDefinitionManager contentDefinitionManager,
            string contentType,
            string contentPart) where T : new()
        {
            var contentTypeDefinition = contentDefinitionManager.GetTypeDefinition(contentType);
            var contentTypePartDefinition = contentTypeDefinition.Parts
                .FirstOrDefault(x => string.Equals(x.PartDefinition.Name, contentPart, StringComparison.Ordinal));

            return contentTypePartDefinition.GetSettings<T>();
        }
    }
}