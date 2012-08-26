using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.Security;
using Orchard.Environment.Extensions;

namespace Piedone.HelpfulLibraries.Contents.DynamicPages
{
    [OrchardFeature("Piedone.HelpfulLibraries.Contents")]
    public class PageAutorizationContext
    {
        public IUser User { get; private set; }
        public bool Granted { get; set; }

        public PageAutorizationContext(IUser user)
        {
            User = user;
            Granted = false;
        }
    }
}
