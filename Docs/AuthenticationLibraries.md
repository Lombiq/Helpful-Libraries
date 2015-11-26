# Authentication Libraries Documentation



## Extensions

Various extensions methods that enhance built-in functionality. Don't forget to add the `using Piedone.HelpfulLibraries.Authentication;` declaration to your code.


## Examples

	// AuthenticationServiceExtensions
	// _authenticationService is an IAuthenticationService reference
	var isAuthenticated _authenticationService.IsAuthenticated();


## HTTP basic authentication

You can use IBasicAuthenticationService for simple HTTP basic auth usage.


## Attributes

The attributes RequireAuthorization enforces authorization for MVC controllers while RequireBasicAuthorization enforces authorization through HTTP basic auth for WebAPI controllers.