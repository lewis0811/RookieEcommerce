using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Client.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;
using System.Security.Claims;
using System.Net.Http;
using System.Text.Json;
using RookieEcommerce.CustomerSite.Models;
using RookieEcommerce.CustomerSite.Services;

namespace RookieEcommerce.CustomerSite.Controllers
{
    public class AuthenticationController : Controller
    {
        [HttpGet("~/login")]
        public ActionResult LogIn(string returnUrl)
        {
            var properties = new AuthenticationProperties
            {
                // Only allow local return URLs to prevent open redirect attacks.
                RedirectUri = Url.IsLocalUrl(returnUrl) ? returnUrl : "/"
            };

            // Ask the OpenIddict client middleware to redirect the user agent to the identity provider.
            return Challenge(properties, OpenIddictClientAspNetCoreDefaults.AuthenticationScheme);
        }

        [HttpPost("~/logout"), ValidateAntiForgeryToken]
        public async Task<ActionResult> LogOut(string returnUrl)
        {
            var result = await HttpContext.AuthenticateAsync();
            if (result is not { Succeeded: true })
            {
                return Redirect(Url.IsLocalUrl(returnUrl) ? returnUrl : "/");
            }

            await HttpContext.SignOutAsync();

            var properties = new AuthenticationProperties(new Dictionary<string, string?>
            {
                [OpenIddictClientAspNetCoreConstants.Properties.IdentityTokenHint] =
                    result.Properties.GetTokenValue(OpenIddictClientAspNetCoreConstants.Tokens.BackchannelIdentityToken)
            })
            {
                RedirectUri = Url.IsLocalUrl(returnUrl) ? returnUrl : "/"
            };

            // Ask the OpenIddict client middleware to redirect the user agent to the identity provider.
            return SignOut(properties, OpenIddictClientAspNetCoreDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public IActionResult Register(string? returnUrl = null)
        {
            var properties = new AuthenticationProperties
            {
                // Only allow local return URLs to prevent open redirect attacks.
                RedirectUri = Url.IsLocalUrl(returnUrl) ? returnUrl : "/",
                
            };
            properties.Parameters["action"] = "register";

            // Ask the OpenIddict client middleware to redirect the user agent to the identity provider.
            return Challenge(properties, OpenIddictClientAspNetCoreDefaults.AuthenticationScheme);
        }

        [HttpGet("~/callback/login/{provider}"), HttpPost("~/callback/login/{provider}"), IgnoreAntiforgeryToken]
        public async Task<ActionResult> LogInCallback()
        {
            // Retrieve the authorization data validated by OpenIddict as part of the callback handling.
            var result = await HttpContext.AuthenticateAsync(OpenIddictClientAspNetCoreDefaults.AuthenticationScheme);

            if (result.Principal is not ClaimsPrincipal { Identity.IsAuthenticated: true })
            {
                throw new InvalidOperationException("The external authorization data cannot be used for authentication.");
            }

            // Build an identity based on the external claims and that will be used to create the authentication cookie.
            var identity = new ClaimsIdentity(
                authenticationType: "ExternalLogin",
                nameType: Claims.Name,
                roleType: Claims.Role);

            identity
                .SetClaim(Claims.Subject, result.Principal.GetClaim(Claims.Subject))
                .SetClaim(Claims.Email, result.Principal.GetClaim(Claims.Email))
                .SetClaim(Claims.Name, result.Principal.GetClaim(Claims.Name))
                .SetClaim(Claims.PreferredUsername, result.Principal.GetClaim(Claims.PreferredUsername))
                .SetClaim(Claims.Role, result.Principal.GetClaim(Claims.Role));

            var properties = new AuthenticationProperties(result.Properties!.Items)
            {
                RedirectUri = result.Properties.RedirectUri ?? "/"
            };

            properties.StoreTokens(result.Properties.GetTokens().Where(token => token.Name is
                // Preserve the access, identity and refresh tokens returned in the token response, if available.
                OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessToken or
                OpenIddictClientAspNetCoreConstants.Tokens.BackchannelIdentityToken or
                OpenIddictClientAspNetCoreConstants.Tokens.RefreshToken));

            return SignIn(new ClaimsPrincipal(identity), properties);
        }

        [HttpGet("~/callback/logout/{provider}"), HttpPost("~/callback/logout/{provider}"), IgnoreAntiforgeryToken]
        public async Task<ActionResult> LogOutCallback()
        {
            var result = await HttpContext.AuthenticateAsync(OpenIddictClientAspNetCoreDefaults.AuthenticationScheme);
            return Redirect(result!.Properties!.RedirectUri!);
        }
    }
}