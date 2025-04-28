using Microsoft.AspNetCore.Mvc;
using RookieEcommerce.CustomerSite.Models;
using RookieEcommerce.CustomerSite.Services;
using RookieEcommerce.SharedViewModels.CartDtos;
using RookieEcommerce.SharedViewModels.ProductRatingDtos;

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

            var product = await productApiClient.GetProductByIdAsync(productId)
                ?? throw new InvalidOperationException($"Product Id {productId} not found.");
            var rating = await productRatingApiClient.GetProductRatingsAsync(productId) 
                ?? throw new InvalidOperationException($"Rating for product Id {productId} not found.");

            var customerId = Guid.Parse("4C1E0C92-0BEA-A47A-6C8A-59E397F632F2"); // Change to get from cookie later
            var order = await orderApiClient.GetOrderItemAsync(customerId)
                ?? throw new InvalidOperationException($"Order for customer Id {customerId}"); // Change to get customerId from cookie later // Please check if customer have order please Lewis

            return View(new HomeProductDetailsViewModel { ProductDetails = product, ProductRatings = rating, OrderDetails = order });
        }

        [HttpPost]
        public async Task<IActionResult> Rating(CreateProductRatingDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await productRatingApiClient.AddProductRatingAsync(dto);

            return RedirectToAction("Details", "Products", new { productId = dto.ProductId });
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(CreateCartItemDto cartItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customerId = Guid.Parse("4C1E0C92-0BEA-A47A-6C8A-59E397F632F2"); // Change to get from cookie later
            var cart = await cartApiClient.GetCustomerCartAsync(customerId);
            Guid cartId;

            if (cart == null)
            {
                cartId = (Guid)await cartApiClient.CreateCustomerCartAsync(customerId);
            }
            else { cartId = cart.Id; }
            await cartItemApiClient.AddCartItemAsync(cartItem, cartId);

            return RedirectToAction("Index", "Cart");
        }
    }
}