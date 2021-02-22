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
        /// <param name="tabIdPart">
        /// The part between the prefix and the content item ID. For example when the hash is
        /// <c>#tab-users-4csgbqx40dgsq5ckebyrsnptcj</c> it should be <c>users</c>.
        /// </param>
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
#pragma warning disable CA1308 // Normalize strings to uppercase. This is not normalization, the ID is lowercase.
            return $"{url}#tab-{tabIdPart.ToLowerInvariant()}-{content.ContentItem.ContentItemId}";
#pragma warning restore CA1308 // Normalize strings to uppercase. This is not normalization, the ID is lowercase.
        }
    }
}
