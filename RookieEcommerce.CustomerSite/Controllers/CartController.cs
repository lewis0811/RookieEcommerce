using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Client.AspNetCore;
using RookieEcommerce.CustomerSite.Models;
using RookieEcommerce.CustomerSite.Services;
using RookieEcommerce.Domain.Entities;
using RookieEcommerce.SharedViewModels.CartDtos;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace RookieEcommerce.CustomerSite.Controllers
{
    public class CartController(CartApiClient cartApiClient) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var token = await HttpContext.GetTokenAsync(OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessToken);
            if (token == null) { RedirectToAction("Login", "Authentication"); }

            CartDetailsDto? cart = null;
            var customerId = User.Claims.FirstOrDefault(c => c.Type == Claims.Subject)?.Value;
            if (customerId != null)
            {
                cart = await cartApiClient.GetCustomerCartAsync(Guid.Parse(customerId), token);
                if (cart == null) { return RedirectToAction("Index", "Home"); }
            }

            return View(new CartViewModel { CartDetails = cart });
        }

        [HttpPost]
        public async Task<IActionResult> HandleCartItem(Guid? cartItemId)
        {
            var token = await HttpContext.GetTokenAsync(OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessToken);
            if (token == null) { RedirectToAction("Login", "Authentication"); }
           
            CartDetailsDto? cart = null;
            var customerId = User.Claims.FirstOrDefault(c => c.Type == Claims.Subject)?.Value;
            if (customerId != null)
            {
                cart = await cartApiClient.GetCustomerCartAsync(Guid.Parse(customerId), token);
            }
            if (cartItemId != null && cart != null)
            {
                var cartId = cart.Id;
                await cartApiClient.RemoveCartItemAsync(cartId, (Guid)cartItemId);
            }

            return RedirectToAction("Index");
        }
    }
}