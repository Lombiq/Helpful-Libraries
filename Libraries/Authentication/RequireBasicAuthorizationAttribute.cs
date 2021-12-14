using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using Orchard;
using Orchard.Environment.Extensions;
using Orchard.Security;
using Orchard.Security.Permissions;
using Piedone.HelpfulLibraries.Authentication;

namespace Piedone.HelpfulLibraries.Authentication
{
    /// <summary>
    /// Enforces the user to be authenticated with HTTP basic authentication and having the supplied permission(s).
    /// </summary>
    [OrchardFeature("Piedone.HelpfulLibraries.Authentication")]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequireBasicAuthorizationAttribute : System.Web.Http.AuthorizeAttribute
    {
        private readonly string[] _permissionNames;


        public RequireBasicAuthorizationAttribute()
        {
        }

        public RequireBasicAuthorizationAttribute(params string[] permissionNames)
        {
            _permissionNames = permissionNames;
        }


        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var workContext = actionContext.ControllerContext.GetWorkContext();

            var user = workContext.Resolve<IBasicAuthenticationService>().GetUserForRequest();

            if (user == null) return false;

            var authorizationService = workContext.Resolve<IAuthorizationService>();

            if (_permissionNames == null) return true;

            foreach (var permission in _permissionNames)
            {
                if (!authorizationService.TryCheckAccess(new Permission { Name = permission }, user, null)) return false;
            }

            return true;
        }
    }
}
