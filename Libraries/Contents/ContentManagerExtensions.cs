using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Piedone.HelpfulLibraries.Contents
{
    [OrchardFeature("Piedone.HelpfulLibraries.Contents")]
    public static class ContentManagerExtensions
    {
        /// <summary>
        /// Creates the Shapes for the given Content Items.
        /// </summary>
        public static Func<IEnumerable<dynamic>> GetShapesFactory(
            this IContentManager contentManager,
            IEnumerable<ContentItem> contentItems,
            string displayType = "",
            string groupId = "") =>
            () => contentItems.Select(item => contentManager.BuildDisplay(item, displayType, groupId));
    }
}
