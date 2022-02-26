using Microsoft.AspNetCore.Mvc.Routing;
using OrchardCore.ContentManagement;

namespace Microsoft.AspNetCore.Mvc
{
    public static class ControllerExtensions
    {
        /// <summary>
        /// Will redirect to the given URL if that is local. Otherwise it will redirect to "~/".
        /// </summary>
        /// <param name="redirectUrl">Local URL to redirect to.</param>
        /// <returns>Redirect action result.</returns>
        /// <remarks>
        /// <para>
        /// Could be part of Orchard but <see href="https://github.com/OrchardCMS/OrchardCore/issues/2830">it
        /// won't</see>.
        /// </para>
        /// </remarks>
        public static RedirectResult RedirectToLocal(this Controller controller, string redirectUrl) =>
            controller.Redirect(controller.Url.IsLocalUrl(redirectUrl) ? redirectUrl : "~/");

        /// <summary>
        /// Uses <see cref="Routing.UrlHelperExtensions.DisplayContentItem"/> extension method to redirect to this <see
        /// cref="ContentItem"/>'s display page.
        /// </summary>
        public static RedirectResult RedirectToContentDisplay(this Controller controller, IContent content) =>
            controller.Redirect(controller.Url.DisplayContentItem(content));
    }
}
