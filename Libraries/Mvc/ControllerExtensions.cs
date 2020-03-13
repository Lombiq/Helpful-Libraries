namespace Microsoft.AspNetCore.Mvc
{
    public static class ControllerExtensions
    {
        public static ActionResult RedirectLocal(this Controller controller, string redirectUrl) =>
            new RedirectResult(controller.Url.IsLocalUrl(redirectUrl) ? redirectUrl : "~/");
    }
}
