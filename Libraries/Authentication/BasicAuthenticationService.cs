using Orchard;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Mvc;
using Orchard.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace Piedone.HelpfulLibraries.Authentication
{
    public interface IBasicAuthenticationCredentials
    {
        string UserName { get; }
        string Password { get; }
    }

    public interface IBasicAuthenticationService : IDependency
    {
        /// <summary>
        /// Returns the basic authentication credentials available in the current request.
        /// </summary>
        /// <returns>The credentials or null if there are no credentials</returns>
        IBasicAuthenticationCredentials GetRequestCredentials();

        /// <summary>
        /// Tries to get the user corresponding to the basic auth credentials.
        /// </summary>
        /// <returns>The matching user if found or null if not.</returns>
        IUser GetUserForRequest();

        /// <summary>
        /// Sets the authenticated user for the current request if the user could be authenticated
        /// with the basic auth credentials.
        /// </summary>
        /// <returns>True if the authentication was successful, false if not.</returns>
        bool SetAuthenticatedUserForRequest();
    }


    [OrchardFeature("Piedone.HelpfulLibraries.Authentication")]
    public class BasicAuthenticationService : IBasicAuthenticationService
    {
        private readonly IHttpContextAccessor _hca;
        private readonly IMembershipService _membershipService;
        private readonly IAuthenticationService _authenticationService;


        public BasicAuthenticationService(
            IHttpContextAccessor hca,
            IMembershipService membershipService,
            IAuthenticationService authenticationService)
        {
            _hca = hca;
            _membershipService = membershipService;
            _authenticationService = authenticationService;
        }
    
            
        public IBasicAuthenticationCredentials GetRequestCredentials()
        {
            var header = _hca.Current().Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(header) ||
                !header.StartsWith("basic", StringComparison.OrdinalIgnoreCase)) return null;

            var credentials = UTF8Encoding.UTF8.GetString(Convert.FromBase64String(header.Substring(6)));
            int separatorIndex = credentials.IndexOf(':');

            if (separatorIndex < 0) return null;

            return new BasicAuthenticationCredentials
            {
                UserName = credentials.Substring(0, separatorIndex),
                Password = credentials.Substring(separatorIndex + 1)
            };
        }

        public IUser GetUserForRequest()
        {
            var credentials = GetRequestCredentials();
            if (credentials == null) return null;

            return _membershipService.ValidateUser(credentials.UserName, credentials.Password, out var validationErrors);
        }

        public bool SetAuthenticatedUserForRequest()
        {
            var user = GetUserForRequest();
            if (user == null) return false;
            _authenticationService.SetAuthenticatedUserForRequest(user);
            return true;
        }


        private class BasicAuthenticationCredentials : IBasicAuthenticationCredentials
        {
            public string UserName { get; set; }
            public string Password { get; set; }
        }
    }
}
