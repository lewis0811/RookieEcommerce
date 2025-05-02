using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Client.AspNetCore;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace RookieEcommerce.OpenIddictServer.Controllers
{
    public class AuthenticationController : Controller
    {
        [HttpGet("~/callback/login/{provider}"), HttpPost("~/callback/login/{provider}"), IgnoreAntiforgeryToken]
        public async Task<ActionResult> LogInCallback()
        {
            var result = await HttpContext.AuthenticateAsync(OpenIddictClientAspNetCoreDefaults.AuthenticationScheme);

            if (result.Principal is not ClaimsPrincipal { Identity.IsAuthenticated: true })
            {
                throw new InvalidOperationException("The external authorization data cannot be used for authentication.");
            }

            // Build an identity based on the external claims and that will be used to create the authentication cookie.
            var identity = new ClaimsIdentity(
                authenticationType: "ExternalLogin",
                nameType: ClaimTypes.Name,
                roleType: ClaimTypes.Role);

            identity.SetClaim(ClaimTypes.Email, result.Principal.GetClaim(ClaimTypes.Email))
                    .SetClaim(ClaimTypes.Name, result.Principal.GetClaim(ClaimTypes.Name))
                    .SetClaim(ClaimTypes.NameIdentifier, result.Principal.GetClaim(ClaimTypes.NameIdentifier));

            // Preserve the registration details to be able to resolve them later.
            identity.SetClaim(Claims.Private.RegistrationId, result.Principal.GetClaim(Claims.Private.RegistrationId))
                    .SetClaim(Claims.Private.ProviderName, result.Principal.GetClaim(Claims.Private.ProviderName));

            var properties = new AuthenticationProperties(result.Properties!.Items)
            {
                RedirectUri = result.Properties.RedirectUri ?? "/"
            };

            // If needed, the tokens returned by the authorization server can be stored in the authentication cookie.
            // To make cookies less heavy, tokens that are not used are filtered out before creating the cookie.
            properties.StoreTokens(result.Properties.GetTokens().Where(token => token.Name is
                // Preserve the access and refresh tokens returned in the token response, if available.
                OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessToken or
                OpenIddictClientAspNetCoreConstants.Tokens.RefreshToken));

            return SignIn(new ClaimsPrincipal(identity), properties);
        }
    }
}