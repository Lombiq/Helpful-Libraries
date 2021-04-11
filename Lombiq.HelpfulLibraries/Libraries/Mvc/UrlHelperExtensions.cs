using OrchardCore.ContentManagement;
using OrchardCore.Mvc.Core.Utilities;
using OrchardCore.Mvc.Utilities;
using OrchardCore.Queries.Controllers;

namespace Microsoft.AspNetCore.Mvc.Routing
{
    public static class UrlHelperExtensions
    {
        /// <summary>
        /// Returns a relative URL for the editor action of <paramref name="content"/> with the tab of
        /// <paramref name="tabIdPart"/> selected.
        /// </summary>
        /// <param name="tabIdPart">The name of the tab as used in the placement info.</param>
        public static string EditContentItemWithTab(this IUrlHelper helper, string tabIdPart, IContent content)
        {
            var url = helper.Action(
                nameof(AdminController.Edit),
                typeof(AdminController).ControllerName(),
                new
                {
                    area = "OrchardCore.Contents",
                    content.ContentItem.ContentItemId,
                });
            return $"{url}#tab-{tabIdPart.HtmlClassify()}-{content.ContentItem.ContentItemId}";
        }
    }
}
