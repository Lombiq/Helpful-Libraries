using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Orchard.Environment.Extensions;
using Orchard;
using Orchard.Security;
using Orchard.Security.Permissions;

namespace Piedone.HelpfulLibraries.Libraries.Authentication
{
    /// <summary>
    /// Enforces the user to be authenticated and having the supplied permission(s).
    /// </summary>
    [OrchardFeature("Piedone.HelpfulLibraries.Authentication")]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequireAuthorizationAttribute : AuthorizeAttribute
    {
        private readonly string[] _permissionNames;


        public RequireAuthorizationAttribute()
        {
        }

        public RequireAuthorizationAttribute(params string[] permissionNames)
        {
            _permissionNames = permissionNames;
        }


        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var workContext = filterContext.Controller.ControllerContext.GetWorkContext();

            var user = workContext.CurrentUser;

            if (user == null)
            {
                filterContext.Result = new HttpUnauthorizedResult();
                return;
            }

            var authorizer = workContext.Resolve<IAuthorizer>();

            if (_permissionNames == null) return;

            foreach (var permission in _permissionNames)
            {
                if (!authorizer.Authorize(new Permission { Name = permission }))
                {
                    filterContext.Result = new HttpUnauthorizedResult();
                    return;
                }
            }
        }
    }
}
