using Orchard.Environment.Extensions;
using Orchard.Mvc;
using System.Web;
using static Orchard.Mvc.MvcModule;

namespace Piedone.HelpfulLibraries.Libraries.Utilities
{
    /// <summary>
    /// Custom HttpContextAccessor that provides a placeholder HttpContext to be used outside a request pipeline.
    /// </summary>
    [OrchardFeature("Piedone.HelpfulLibraries.Utilities")]
    public class HttpContextPlaceholderAccessor : IHttpContextAccessor
    {
        private HttpContextBase _stub = new HttpContextPlaceholder(() => "http://localhost");


        public HttpContextBase Current()
        {
            return _stub;
        }

        public void Set(HttpContextBase stub)
        {
            _stub = stub;
        }
    }
}
