using Orchard.Environment.Extensions;
using Orchard.Security;

namespace Piedone.HelpfulLibraries.Authentication
{
    [OrchardFeature("Piedone.HelpfulLibraries.Authentication")]
    public static class AuthenticationServiceExtensions
    {
        /// <summary>
        /// Checks if the current user is authenticated or not
        /// </summary>
        public static bool IsAuthenticated(this IAuthenticationService service)
        {
            return service.GetAuthenticatedUser() != null;
        }
    }
}
