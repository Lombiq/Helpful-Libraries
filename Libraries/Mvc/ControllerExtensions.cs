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
        public static ActionResult RedirectToLocal(this Controller controller, string redirectUrl)
        {
            if (controller.Url.IsLocalUrl(redirectUrl))
            {
                return controller.Redirect(redirectUrl);
            }
            else
            {
                return controller.Redirect("~/");
            }
        }
    }
}
