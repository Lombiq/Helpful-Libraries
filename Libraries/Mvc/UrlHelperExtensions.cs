using OrchardCore.ContentManagement;
using OrchardCore.Mvc.Core.Utilities;
using OrchardCore.Queries.Controllers;

namespace Microsoft.AspNetCore.Mvc.Routing
{
    public static class UrlHelperExtensions
    {
        /// <summary>
        /// Returns a relative URL for the editor action of <paramref name="content"/> with the tab of
        /// <paramref name="tabIdPart"/> selected.
        /// </summary>
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

            return $"{url}#{tabIdPart}-{content.ContentItem.ContentItemId}";
        }
    }
}
