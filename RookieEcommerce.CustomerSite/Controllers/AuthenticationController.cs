using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Client.AspNetCore;
using System;

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
    }
}
