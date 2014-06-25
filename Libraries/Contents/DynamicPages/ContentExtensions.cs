using System;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using System.Linq;

namespace Piedone.HelpfulLibraries.Contents.DynamicPages
{
    [OrchardFeature("Piedone.HelpfulLibraries.Contents")]
    public static class ContentExtensions
    {
        /// <summary>
        /// Checks whether a content item is a page with the specified name
        /// </summary>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="group">Name of the page group.</param>
        public static bool IsPage(this IContent page, string pageName, string group)
        {
            return page.ContentItem.ContentType.EndsWith(ContentManagerExtensions.CreatePageName(null, pageName, group));
        }

        /// <summary>
        /// Checks whether a content item is a Dynamic Page.
        /// </summary>
        public static bool IsPage(this IContent page)
        {
            return page.ContentItem.ContentType.EndsWith("-Page");
        }

        /// <summary>
        /// Gets the group of a page content object.
        /// </summary>
        /// <returns>The name of the group.</returns>
        public static string PageGroup(this IContent page)
        {
            var contentType = page.ContentItem.ContentType;

            if (!contentType.EndsWith("-Page")) return null;

            var nameSegments = contentType.Split('.');

            if (nameSegments.Count() != 2) return null;

            return nameSegments.First();
        }
    }
}
