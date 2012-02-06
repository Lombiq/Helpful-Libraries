using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Piedone.HelpfulLibraries.Contents
{
    [OrchardFeature("Piedone.HelpfulLibraries.Contents")]
    public static class ContentExtensions
    {
        public static T AsField<T>(this IContent content, string partName)
            where T : ContentField
        {
            return AsField<T>(content, partName, LookupField<T>(content, partName).Name);
        }

        public static bool HasField<T>(this IContent content, string partName)
            where T : ContentField
        {
            return HasField(content, partName, LookupField<T>(content, partName).Name);
        }

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

        public static bool HasField(this IContent content, string partName, string fieldName)
        {
            // This is so the behaviour is consistent with Orchard.ContentManagement.ContentExtensions.Has<>()
            if (content == null) return false;

            return content.ContentItem.Parts
                .Where(part => part.PartDefinition.Name == partName)
                .Any(part => part.Fields.Any(field => field.Name == fieldName));
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
