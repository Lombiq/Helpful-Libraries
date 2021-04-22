using Lombiq.HelpfulLibraries.Libraries.Contents;
using System.Security.Claims;
using System.Threading.Tasks;
using YesSql;
using static OrchardCore.Contents.CommonPermissions;

namespace Microsoft.AspNetCore.Authorization
{
    public static class AuthorizationServiceExtensions
    {
        /// <summary>
        /// Determines if the <paramref name="user"/> has access to the given <paramref name="contentType"/> by looking
        /// up an instance of that content type from the database and checking against it.
        /// </summary>
        public static async Task<bool> AuthorizeContentTypeBySampleAsync(
            this IAuthorizationService authorizationService,
            ISession session,
            string contentType,
            ClaimsPrincipal user)
        {
            var first = await session
                .QueryContentItem(PublicationStatus.Published)
                .Where(index => index.ContentType == contentType)
                .FirstOrDefaultAsync();
            return first != null && await authorizationService.AuthorizeAsync(user, DeleteContent, first);
        }

    }
}
