using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Mvc;

namespace Piedone.HelpfulLibraries.Contents
{
    /// <summary>
    /// Generates the output (e.g. the HTML markup) and captures it for shapes
    /// </summary>
    [Obsolete("Use IShapeDisplay instead.")]
    public interface IShapeOutputGenerator : IDependency
    {
        /// <summary>
        /// Generates the output (e.g. the HTML markup) and captures it for a shape 
        /// </summary>
        /// <param name="shape">The shape to generate the output for</param>
        /// <returns>Stream containing the output</returns>
        Stream GenerateOutput(dynamic shape);
    }


    [OrchardFeature("Piedone.HelpfulLibraries.Contents")]
    public class ShapeOutputGenerator : IShapeOutputGenerator
    {
        private readonly IHttpContextAccessor _hca;


        public ShapeOutputGenerator(IHttpContextAccessor hca)
        {
            _hca = hca;
        }


        public Stream GenerateOutput(dynamic shape)
        {
            var controller = new DummyController(_hca.Current().Request.RequestContext);

            using (var stream = new MemoryStream())
            using (var streamWriter = new StreamWriter(stream))
            {
                var originalContext = controller.ControllerContext.HttpContext;

                try
                {
                    // Get the Request and User objects from the current, unchanged context
                    var currentRequest = HttpContext.Current.ApplicationInstance.Context.Request;
                    var currentUser = HttpContext.Current.ApplicationInstance.Context.User;

                    // Create our new HttpResponse object containing our HtmlTextWriter
                    var newResponse = new HttpResponse(streamWriter);

                    // Create a new HttpContext object using our new Response object and the existing Request and User objects
                    var newContext = new HttpContextWrapper(
                                new HttpContext(currentRequest, newResponse)
                                {
                                    User = currentUser
                                });

                    // Swap in our new HttpContext object - output from this controller is now going to our HtmlTextWriter object
                    controller.ControllerContext.HttpContext = newContext;

                    new ShapePartialResult(controller, shape).ExecuteResult(controller.ControllerContext);

                    newResponse.Flush();
                    streamWriter.Flush();
                    stream.Flush();

                    // New stream so everything else here can be disposed
                    var responseStream = new MemoryStream();
                    stream.Position = 0;
                    stream.CopyTo(responseStream);
                    responseStream.Position = 0;
                    return responseStream;
                }
                finally
                {
                    // Setting context back to original so nothing gets messed up
                    controller.ControllerContext.HttpContext = originalContext;
                }
            }
        }


        private class DummyController : ControllerBase
        {
            public DummyController(RequestContext requestContext)
            {
                ControllerContext = new ControllerContext(requestContext, this);
            }


            protected override void ExecuteCore()
            {
                throw new NotImplementedException();
            }
        }
    }
}