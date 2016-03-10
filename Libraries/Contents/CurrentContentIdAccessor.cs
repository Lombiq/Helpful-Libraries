using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard;
using Orchard.Alias;
using Orchard.Environment.Extensions;
using Orchard.Mvc;

namespace Piedone.HelpfulLibraries.Libraries.Contents
{
    /// <summary>
    /// Provides access to the currently requested content item's (if any) numerical ID.
    /// </summary>
    public interface ICurrentContentIdAccessor : IDependency
    {
        /// <summary>
        /// Gets the currently requested content item's (if any) numerical ID.
        /// </summary>
        /// <returns>
        /// The currently requested content item's numerical ID if there is a corresponding content item or 0 otherwise.
        /// </returns>
        int GetCurrentContentId();
    }


    [OrchardFeature("Piedone.HelpfulLibraries.Contents")]
    public class CurrentContentIdAccessor : ICurrentContentIdAccessor
    {
        private readonly IHttpContextAccessor _hca;
        private readonly IAliasService _aliasService;


        public CurrentContentIdAccessor(
            IHttpContextAccessor hca,
            IAliasService aliasService)
        {
            _hca = hca;
            _aliasService = aliasService;
        }

        public int GetCurrentContentId()
        {
            var currentContentItemId = 0;
            var request = _hca.Current().Request;
            var path = request.AppRelativeCurrentExecutionFilePath.Substring(1).Trim('/');

            var itemRoute = _aliasService.Get(path);

            if (itemRoute != null) currentContentItemId = Convert.ToInt32(itemRoute["Id"]);
            else
            {
                if (path.StartsWith("Contents/Item/Display/") || path.StartsWith("Contents/Item/Preview/"))
                {
                    currentContentItemId = Convert.ToInt32(request.RequestContext.RouteData.Values["Id"]);
                }
            }

            return currentContentItemId;
        }
    }
}
