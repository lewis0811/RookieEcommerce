using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using RookieEcommerce.Domain.Entities;
using RookieEcommerce.OpenIddictServer.Helpers;
using RookieEcommerce.OpenIddictServer.Models;
using System.Net;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace RookieEcommerce.OpenIddictServer.Controllers
{
    public class AuthorizationController(
        UserManager<Customer> userManager,
        IOpenIddictApplicationManager applicationManager,
        IOpenIddictAuthorizationManager authorizationManager,
        IOpenIddictScopeManager scopeManager,
        SignInManager<Customer> signInManager,
        IConfiguration configuration
        ) : Controller
    {
        [HttpGet("~/connect/authorize")]
        [HttpPost("~/connect/authorize")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Authorize()
        {
            var request = HttpContext.GetOpenIddictServerRequest() ??
                throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            // Try to retrieve the user principal stored in the authentication cookie and redirect

            // the user agent to the login page (or to an external provider) in the following cases:
            //
            //  - If the user principal can't be extracted or the cookie is too old.
            //  - If prompt=login was specified by the client application.
            //  - If max_age=0 was specified by the client application (max_age=0 is equivalent to prompt=login).
            //  - If a max_age parameter was provided and the authentication cookie is not considered "fresh" enough.
            //
            // For scenarios where the default authentication handler configured in the ASP.NET Core
            // authentication options shouldn't be used, a specific scheme can be specified here.
            var result = await HttpContext.AuthenticateAsync();
            if (result is not { Succeeded: true } ||
                ((request.HasPromptValue(PromptValues.Login) || request.MaxAge is 0 ||
                 (request.MaxAge != null && result.Properties?.IssuedUtc != null &&
                  TimeProvider.System.GetUtcNow() - result.Properties.IssuedUtc > TimeSpan.FromSeconds(request.MaxAge.Value))) &&
                TempData["IgnoreAuthenticationChallenge"] is null or false))
            {
                // If the client application requested promptless authentication,
                // return an error indicating that the user is not logged in.
                if (request.HasPromptValue(PromptValues.None))
                {
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.LoginRequired,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The user is not logged in."
                        }));
                }

                // If have register prompt, then redirect to register page
                string? promptValue = request.GetParameter("action")?.ToString();
                if (promptValue == "register")
                {
                    var registerUrl = 
                        "/connect/authorize?" +
                        $"client_id={request.ClientId}" +
                        $"&redirect_uri={request.RedirectUri}" +
                        $"&response_type={request.ResponseType}" +
                        $"&scope={request.Scope}" +
                        $"&nonce={request.Nonce}" +
                        $"&code_challenge={request.CodeChallenge}" +
                        $"&code_challenge_method={request.CodeChallengeMethod}" +
                        $"&state={request.State}";

                    registerUrl = configuration["OpenIddict:BaseUrl"] +
                        "/Identity/Account/Register?returnUrl=" + WebUtility.UrlEncode(registerUrl);

                    return Redirect(registerUrl);
                }

                TempData["IgnoreAuthenticationChallenge"] = true;



                return Challenge(new AuthenticationProperties
                {
                    RedirectUri = Request.PathBase + Request.Path + QueryString.Create(
                        Request.HasFormContentType ? Request.Form : Request.Query)
                });
            }

            // Retrieve the profile of the logged in user.
            var user = await userManager.GetUserAsync(result.Principal) ??
                throw new InvalidOperationException("The user details cannot be retrieved.");

            // Retrieve the application details from the database.
            var application = await applicationManager.FindByClientIdAsync(request.ClientId!) ??
                throw new InvalidOperationException("Details concerning the calling client application cannot be found.");

            // Convert the IAsyncEnumerable<object> to a List<object> using ToListAsync()
            var authorizations = await authorizationManager.FindAsync(
                subject: await userManager.GetUserIdAsync(user),
                client: await applicationManager.GetIdAsync(application),
                status: Statuses.Valid,
                type: AuthorizationTypes.Permanent,
                scopes: request.GetScopes()).ToListAsync();

            switch (await applicationManager.GetConsentTypeAsync(application))
            {
                // If the consent is external (e.g when authorizations are granted by a sysadmin),
                // immediately return an error if no authorization can be found in the database.
                case ConsentTypes.External when authorizations.Count is 0:
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                                "The logged in user is not allowed to access this client application."
                        }));

                // If the consent is implicit or if an authorization was found,
                // return an authorization response without displaying the consent form.
                case ConsentTypes.Implicit:
                case ConsentTypes.External when authorizations.Count is not 0:
                case ConsentTypes.Explicit when authorizations.Count is not 0 && !request.HasPromptValue(PromptValues.Consent):
                    // Create the claims-based identity that will be used by OpenIddict to generate tokens.
                    var identity = new ClaimsIdentity(
                        authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                        nameType: Claims.Name,
                        roleType: Claims.Role);

                    // Add the claims that will be persisted in the tokens.
                    identity.SetClaim(Claims.Subject, await userManager.GetUserIdAsync(user))
                            .SetClaim(Claims.Email, await userManager.GetEmailAsync(user))
                            .SetClaim(Claims.Name, await userManager.GetUserNameAsync(user))
                            .SetClaim(Claims.PreferredUsername, await userManager.GetUserNameAsync(user))
                            .SetClaims(Claims.Role, [.. (await userManager.GetRolesAsync(user))]);

                    // Note: in this sample, the granted scopes match the requested scope
                    // but you may want to allow the user to uncheck specific scopes.
                    // For that, simply restrict the list of scopes before calling SetScopes.
                    identity.SetScopes(request.GetScopes());
                    identity.SetResources(await scopeManager.ListResourcesAsync(identity.GetScopes()).ToListAsync());

                    // Automatically create a permanent authorization to avoid requiring explicit consent
                    // for future authorization or token requests containing the same scopes.
                    var authorization = authorizations.LastOrDefault();
                    var clientForAuthZ = await applicationManager.GetIdAsync(application);

                    authorization ??= await authorizationManager.CreateAsync(
                        identity: identity,
                        subject: await userManager.GetUserIdAsync(user),
                        client: clientForAuthZ!,
                        type: AuthorizationTypes.Permanent,
                        scopes: identity.GetScopes());

                    identity.SetAuthorizationId(await authorizationManager.GetIdAsync(authorization));
                    identity.SetDestinations(GetDestinations);

                    return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

                // At this point, no authorization was found in the database and an error must be returned
                // if the client application specified prompt=none in the authorization request.
                case ConsentTypes.Explicit when request.HasPromptValue(PromptValues.None):
                case ConsentTypes.Systematic when request.HasPromptValue(PromptValues.None):
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                                "Interactive user consent is required."
                        }));

                // In every other case, render the consent form.
                default:
                    var applicationName = await applicationManager.GetLocalizedDisplayNameAsync(application);
                    return View(new AuthorizeViewModel
                    {
                        ApplicationName = applicationName!,
                        Scope = request.Scope!
                    });
            }
        }

        [Authorize, FormValueRequired("submit.Accept")]
        [HttpPost("~/connect/authorize"), ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept()
        {
            var request = HttpContext.GetOpenIddictServerRequest() ??
                throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            // Retrieve the profile of the logged in user.
            var user = await userManager.GetUserAsync(User) ??
                throw new InvalidOperationException("The user details cannot be retrieved.");

            // Retrieve the application details from the database.
            var application = await applicationManager.FindByClientIdAsync(request.ClientId!) ??
                throw new InvalidOperationException("Details concerning the calling client application cannot be found.");

            // Retrieve the permanent authorizations associated with the user and the calling client application.
            var authorizations = await authorizationManager.FindAsync(
                subject: await userManager.GetUserIdAsync(user),
                client: await applicationManager.GetIdAsync(application),
                status: Statuses.Valid,
                type: AuthorizationTypes.Permanent,
                scopes: request.GetScopes()).ToListAsync();

            // Note: the same check is already made in the other action but is repeated
            // here to ensure a malicious user can't abuse this POST-only endpoint and
            // force it to return a valid response without the external authorization.
            if (authorizations.Count is 0 && await applicationManager.HasConsentTypeAsync(application, ConsentTypes.External))
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "The logged in user is not allowed to access this client application."
                    }));
            }

            // Create the claims-based identity that will be used by OpenIddict to generate tokens.
            var identity = new ClaimsIdentity(
                authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                nameType: Claims.Name,
                roleType: Claims.Role);

            // Add the claims that will be persisted in the tokens.
            identity.SetClaim(Claims.Subject, await userManager.GetUserIdAsync(user))
                    .SetClaim(Claims.Email, await userManager.GetEmailAsync(user))
                    .SetClaim(Claims.Name, await userManager.GetUserNameAsync(user))
                    .SetClaim(Claims.PreferredUsername, await userManager.GetUserNameAsync(user))
                    .SetClaims(Claims.Role, [.. (await userManager.GetRolesAsync(user))]);

            identity.SetScopes(request.GetScopes());
            identity.SetResources(await scopeManager.ListResourcesAsync(identity.GetScopes()).ToListAsync());

            // Automatically create a permanent authorization to avoid requiring explicit consent
            // for future authorization or token requests containing the same scopes.
            var authorization = authorizations.LastOrDefault();
            var clientForAuthZ = await applicationManager.GetIdAsync(application);
            authorization ??= await authorizationManager.CreateAsync(
                identity: identity,
                subject: await userManager.GetUserIdAsync(user),
                client: clientForAuthZ!,
                type: AuthorizationTypes.Permanent,
                scopes: identity.GetScopes());

            identity.SetAuthorizationId(await authorizationManager.GetIdAsync(authorization));
            identity.SetDestinations(GetDestinations);

            // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
            return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        [HttpPost("~/connect/token"), IgnoreAntiforgeryToken, Produces("application/json")]
        public async Task<IActionResult> Exchange()
        {
            var request = HttpContext.GetOpenIddictServerRequest() ??
                throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            if (request.IsAuthorizationCodeGrantType() || request.IsRefreshTokenGrantType())
            {
                // Retrieve the claims principal stored in the authorization code/refresh token.
                var result = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

                // Retrieve the user profile corresponding to the authorization code/refresh token.
                var user = await userManager.FindByIdAsync(result.Principal?.GetClaim(Claims.Subject)!);
                if (user is null)
                {
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The token is no longer valid."
                        }));
                }

                // Ensure the user is still allowed to sign in.
                if (!await signInManager.CanSignInAsync(user))
                {
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The user is no longer allowed to sign in."
                        }));
                }

                var identity = new ClaimsIdentity(result.Principal?.Claims,
                    authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                    nameType: Claims.Name,
                    roleType: Claims.Role);

                // Override the user claims present in the principal in case they
                // changed since the authorization code/refresh token was issued.
                identity.SetClaim(Claims.Subject, await userManager.GetUserIdAsync(user))
                        .SetClaim(Claims.Email, await userManager.GetEmailAsync(user))
                        .SetClaim(Claims.Name, await userManager.GetUserNameAsync(user))
                        .SetClaim(Claims.PreferredUsername, await userManager.GetUserNameAsync(user))
                        .SetClaims(Claims.Role, [.. (await userManager.GetRolesAsync(user))]);

                identity.SetDestinations(GetDestinations);

                // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
                return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            throw new InvalidOperationException("The specified grant type is not supported.");
        }

        private static IEnumerable<string> GetDestinations(Claim claim)
        {
            // Note: by default, claims are NOT automatically included in the access and identity tokens.
            // To allow OpenIddict to serialize them, you must attach them a destination, that specifies
            // whether they should be included in access tokens, in identity tokens or in both.
            switch (claim.Type)
            {
                case Claims.Name or Claims.PreferredUsername:
                    yield return Destinations.AccessToken;

                    if (claim.Subject!.HasScope(Scopes.Profile))
                        yield return Destinations.IdentityToken;

                    yield break;

                case Claims.Email:
                    yield return Destinations.AccessToken;

                    if (claim.Subject!.HasScope(Scopes.Email))
                        yield return Destinations.IdentityToken;

                    yield break;

                case Claims.Role:
                    yield return Destinations.AccessToken;

                    if (claim.Subject!.HasScope(Scopes.Roles))
                        yield return Destinations.IdentityToken;

                    yield break;

                // Never include the security stamp in the access and identity tokens, as it's a secret value.
                case "AspNet.Identity.SecurityStamp": yield break;

                default:
                    yield return Destinations.AccessToken;
                    yield break;
            }
        }
    }
}