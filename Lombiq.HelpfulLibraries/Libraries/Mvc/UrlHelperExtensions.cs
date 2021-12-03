using OrchardCore.ContentManagement;
using OrchardCore.Mvc.Core.Utilities;
using OrchardCore.Mvc.Utilities;
using OrchardCore.Queries.Controllers;
using System.Text.RegularExpressions;

namespace Microsoft.AspNetCore.Mvc.Routing
{
    public static class UrlHelperExtensions
    {
        private const string OrchardCoreContentsArea = "OrchardCore.Contents";

        /// <summary>
        /// Returns an absolute URL for the editor action of <paramref name="contentItemId"/>.
        /// </summary>
        public static string EditContentItem(this IUrlHelper helper, string contentItemId) =>
            EditContentItemWithTab(helper, tabIdPart: null, contentItemId);

        /// <summary>
        /// Returns an absolute URL for the editor action of <paramref name="content"/> with the tab of
        /// <paramref name="tabIdPart"/> selected.
        /// </summary>
        /// <param name="tabIdPart">The name of the tab as used in the placement info.</param>
        public static string EditContentItemWithTab(this IUrlHelper helper, string tabIdPart, IContent content) =>
            EditContentItemWithTab(helper, tabIdPart, content.ContentItem.ContentItemId);

        /// <summary>
        /// Returns an absolute URL for the editor action of <paramref name="contentItemId"/> with the tab of
        /// <paramref name="tabIdPart"/> selected.
        /// </summary>
        /// <param name="tabIdPart">The name of the tab as used in the placement info.</param>
        public static string EditContentItemWithTab(this IUrlHelper helper, string tabIdPart, string contentItemId)
        {
            var url = helper.Action(
                nameof(AdminController.Edit),
                typeof(AdminController).ControllerName(),
                new
                {
                    area = OrchardCoreContentsArea,
                    contentItemId,
                });
            return string.IsNullOrEmpty(tabIdPart)
                ? url
                : $"{url}#tab-{tabIdPart.HtmlClassify()}-{contentItemId}";
        }

        /// <summary>
        /// Returns an absolute URL for the <see cref="ContentItem"/> display page for the given <paramref name="content"/>.
        /// </summary>
        public static string DisplayContentItem(this IUrlHelper helper, IContent content) =>
            helper.Action(
                "Display",
                "Item",
                new
                {
                    area = OrchardCoreContentsArea,
                    content.ContentItem.ContentItemId,
                });

        /// <summary>
        /// Returns the relative URL for the given <paramref name="controllerAction"/> in a <paramref
        /// name="controller"/>. Additional <paramref name="values"/> can be added.
        /// </summary>
        public static string RelativeAction(
            this IUrlHelper helper,
            string controllerAction,
            string controller,
            object values)
        {
            var absoluteUri = helper.Action(controllerAction, controller, values);
            return Regex.Replace(absoluteUri, @"^(([^:/?#]+):)?(\/\/([^/?#]*))", string.Empty);
        }
    }
}
