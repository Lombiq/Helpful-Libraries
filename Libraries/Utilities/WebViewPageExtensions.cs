using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Orchard.Environment.Extensions;
using Orchard;
using Piedone.HelpfulLibraries.Libraries.Utilities;
using System.Web;
using Orchard.Mvc.Spooling;

namespace Piedone.HelpfulLibraries.Utilities
{
    [OrchardFeature("Piedone.HelpfulLibraries.Utilities")]
    public static class WebViewPageExtensions
    {
        public static IDisposable CaptureOnce(this WebViewPage page)
        {
            return new CaptureScope(page);
        }


        class CaptureScope : IDisposable
        {
            readonly WebViewPage _viewPage;

            public CaptureScope(WebViewPage viewPage)
            {
                _viewPage = viewPage;
                _viewPage.OutputStack.Push(new HtmlStringWriter());
            }

            void IDisposable.Dispose()
            {
                var output = ((HtmlStringWriter)_viewPage.OutputStack.First()).ToHtmlString();
                var workContext = _viewPage.ViewContext.RequestContext.GetWorkContext();

                if (workContext.GetState<string>(output) != null)
                {
                    _viewPage.OutputStack.Pop();
                }
                else
                {
                    workContext.SetState<string>(output, "set");
                    _viewPage.ViewContext.Writer.Write(output);
                }
            }
        }
    }
}
