using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;
using Orchard.Security;

namespace Piedone.HelpfulLibraries.Contents.DynamicPages
{
    [OrchardFeature("Piedone.HelpfulLibraries.Contents")]
    public static class ActivatedContentContextExtensions
    {
        public static PageContext PageContext(this ActivatedContentContext context)
        {
            return new PageContext(context.ContentItem, context.ContentItem.PageGroup());
        }

        public static PageContext PageContext(this InitializingContentContext context)
        {
            return new PageContext(context.ContentItem, context.ContentItem.PageGroup());
        }

        public static PageContext PageContext(this CheckAccessContext context)
        {
            return new PageContext(context.Content, context.Content.PageGroup());
        }
    }


    public class PageContext
    {
        private readonly IContent _page;
        public ContentItem Page
        {
            get { return _page.ContentItem; }
        }

        private readonly string _group;
        public string Group
        {
            get { return _group; }
        }


        public PageContext(IContent page, string group)
        {
            _page = page;
            _group = group;
        }
    }
}
