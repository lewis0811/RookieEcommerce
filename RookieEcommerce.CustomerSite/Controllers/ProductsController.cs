using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Client.AspNetCore;
using RookieEcommerce.CustomerSite.Models;
using RookieEcommerce.CustomerSite.Services;
using RookieEcommerce.SharedViewModels.CartDtos;
using RookieEcommerce.SharedViewModels.OrderDtos;
using RookieEcommerce.SharedViewModels.ProductRatingDtos;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace RookieEcommerce.CustomerSite.Controllers
{
    public class ProductsController(
        ProductApiClient productApiClient,
        ProductRatingApiClient productRatingApiClient,
        OrderApiClient orderApiClient,
        CartItemApiClient cartItemApiClient,
        CartApiClient cartApiClient) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Details(Guid productId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var token = await HttpContext.GetTokenAsync(OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessToken);

            bool userIsLoggedIn = User.Identity!.IsAuthenticated;
            var customerId = User.Claims.FirstOrDefault(c => c.Type == Claims.Subject)?.Value;
            ViewBag.UserIsLoggedIn = userIsLoggedIn;
            ViewBag.CustomerId = customerId;

            OrderDetailsDto? order = new();
            var product = await productApiClient.GetProductByIdAsync(productId, token)
                ?? throw new InvalidOperationException($"Product Id {productId} not found.");

            var rating = await productRatingApiClient.GetProductRatingsAsync(productId, token)
                ?? throw new InvalidOperationException($"Rating for product Id {productId} not found.");


            if (customerId != null)
            {
                order = await orderApiClient.GetOrderDetailAsync(Guid.Parse(customerId));
            }

            return View(new HomeProductDetailsViewModel { ProductDetails = product, ProductRatings = rating, OrderDetails = order });
        }

        [HttpPost]
        public async Task<IActionResult> Rating(CreateProductRatingDto createProductRating)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var token = await HttpContext.GetTokenAsync(OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessToken);
            if (token == null) { RedirectToAction("Login", "Authentication"); }

            await productRatingApiClient.AddProductRatingAsync(createProductRating, token);

            return RedirectToAction("Details", "Products", new { productId = createProductRating.ProductId });
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(CreateCartItemDto cartItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var token = await HttpContext.GetTokenAsync(OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessToken);
            if (token == null) { RedirectToAction("Login", "Authentication"); }

            Guid cartId = Guid.Empty;
            var customerId = User.Claims.FirstOrDefault(c => c.Type == Claims.Subject)?.Value;

            if (customerId != null)
            {
                var cart = await cartApiClient.GetCustomerCartAsync(Guid.Parse(customerId), token);

                if (cart == null)
                {
                    cartId = (Guid)await cartApiClient.CreateCustomerCartAsync(Guid.Parse(customerId));
                }
                else { cartId = cart.Id; }
            }

            await cartItemApiClient.AddCartItemAsync(cartItem, cartId);

            return RedirectToAction("Index", "Cart");
        }
    }
}