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
        /// Could be part of Orchard but <see href="https://github.com/OrchardCMS/OrchardCore/issues/2830">it won't</see>.
        /// </remarks>
        public static ActionResult RedirectToLocal(this Controller controller, string redirectUrl) =>
            controller.Url.IsLocalUrl(redirectUrl) ? controller.Redirect(redirectUrl) : controller.Redirect("~/");
    }
}
