using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Piedone.HelpfulLibraries.Contents.DynamicPages
{
    [OrchardFeature("Piedone.HelpfulLibraries.Contents")]
    public static class ContentManagerExtensions
    {
        public static IContent NewPage(this IContentManager contentManager, string pageName, IPageEventHandler evenHandler)
        {
            var page = contentManager.New(pageName);

            evenHandler.OnPageInitializing(page);
            evenHandler.OnPageInitialized(page);

            return page;
        }

        public static dynamic BuildPageDisplay(this IContentManager contentManager, IContent page)
        {
            var shape = contentManager.BuildDisplay(page);
            shape.Metadata.Wrappers.Add("PageWrapper_" + page.ContentItem.ContentType.Replace('.', '_'));
            return shape;
        }
    }
}
