using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.Events;
using Orchard.ContentManagement;

namespace Piedone.HelpfulLibraries.Contents.DynamicPages
{
    public interface IPageEventHandler : IEventHandler
    {
        void OnPageInitializing(IContent page);
        void OnPageInitialized(IContent page);
        void OnPageBuilt(IContent page);
        void OnAuthorization(PageAutorizationContext authorizationContext);
    }
}