using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.Security;
using Orchard.Environment.Extensions;
using Orchard.ContentManagement;

namespace Piedone.HelpfulLibraries.Contents.DynamicPages
{
    [OrchardFeature("Piedone.HelpfulLibraries.Contents")]
    public class PageAutorizationContext
    {
        public IContent Page { get; private set; }
        public IUser User { get; private set; }
        public bool Granted { get; set; }

        public PageAutorizationContext(IContent page, IUser user)
        {
            Page = page;
            User = user;
            Granted = false;
        }
    }
}
