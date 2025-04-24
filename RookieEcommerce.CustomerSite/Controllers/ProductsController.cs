using Microsoft.AspNetCore.Mvc;
using RookieEcommerce.CustomerSite.Models;
using RookieEcommerce.CustomerSite.Services;
using RookieEcommerce.SharedViewModels.ProductRatingDtos;

namespace RookieEcommerce.CustomerSite.Controllers
{
    public class ProductsController(
        ProductApiClient productApiClient,
        ProductRatingApiClient productRatingApiClient,
        OrderApiClient orderApiClient) : Controller
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

            var product = await productApiClient.GetProductByIdAsync(productId);
            var rating = await productRatingApiClient.GetProductRatingsAsync(productId);
            var order = await orderApiClient.GetOrderItemAsync(rating.Items.FirstOrDefault().CustomerId); // Change to get customerId from cookie later // Please check if customer have order please Lewis

            return View(new HomeProductDetailsViewModel { ProductDetails = product, ProductRatings = rating, OrderDetails = order});
        }

        [HttpPost]
        public async Task<IActionResult> Rating([Bind(Prefix = "CreateProductRating")] CreateProductRatingDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await productRatingApiClient.AddProductRatingAsync(dto);

            return RedirectToAction("Details", "Products", new { productId = dto.ProductId });
        }
    }
}