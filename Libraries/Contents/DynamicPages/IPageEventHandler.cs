using Orchard.ContentManagement;
using Orchard.Events;

namespace Piedone.HelpfulLibraries.Contents.DynamicPages
{
    /// <summary>
    /// Base interface for page event handlers
    /// </summary>
    /// <remarks>
    /// When deriving from this interface make sure to duplicate the method headers! See: http://orchard.codeplex.com/workitem/18990
    /// </remarks>
    public interface IPageEventHandler : IEventHandler
    {
        void OnPageInitializing(PageContext pageContext);
        void OnPageInitialized(PageContext pageContext);
        void OnPageBuilt(PageContext pageContext);
        void OnAuthorization(PageAutorizationContext authorizationContext);
    }
}