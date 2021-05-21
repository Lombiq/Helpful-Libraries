using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement;
using OrchardCore.DisplayManagement;
using System.Threading.Tasks;

namespace OrchardCore
{
    public static class ContentUrlOrchardHelperExtensions
    {
        /// <summary>
        /// Gets the given content item's edit URL.
        /// </summary>
        public static async Task<string> GetItemEditUrlAsync(
            this IOrchardHelper orchardHelper,
            ContentItem contentItem)
        {
            var urlHelperFactory = orchardHelper.HttpContext.RequestServices.GetService<IUrlHelperFactory>();
            var viewContextAccessor = orchardHelper.HttpContext.RequestServices.GetService<ViewContextAccessor>();
            var contentManager = orchardHelper.HttpContext.RequestServices.GetService<IContentManager>();

            var urlHelper = urlHelperFactory.GetUrlHelper(viewContextAccessor.ViewContext);
            var metadata = await contentManager.PopulateAspectAsync<ContentItemMetadata>(contentItem);
            return urlHelper.Action(metadata.EditorRouteValues["action"].ToString(), metadata.EditorRouteValues);
        }

        /// <summary>
        /// Gets the given content item's display URL.
        /// </summary>
        public static async Task<string> GetItemDisplayUrlAsync(
            this IOrchardHelper orchardHelper,
            ContentItem contentItem)
        {
            var urlHelperFactory = orchardHelper.HttpContext.RequestServices.GetService<IUrlHelperFactory>();
            var viewContextAccessor = orchardHelper.HttpContext.RequestServices.GetService<ViewContextAccessor>();
            var contentManager = orchardHelper.HttpContext.RequestServices.GetService<IContentManager>();

            var urlHelper = urlHelperFactory.GetUrlHelper(viewContextAccessor.ViewContext);
            var metadata = await contentManager.PopulateAspectAsync<ContentItemMetadata>(contentItem);
            return urlHelper.Action(metadata.DisplayRouteValues["action"].ToString(), metadata.DisplayRouteValues);
        }
    }
}
