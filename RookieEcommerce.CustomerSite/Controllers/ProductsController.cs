using Microsoft.AspNetCore.Mvc;
using RookieEcommerce.CustomerSite.Services;

namespace RookieEcommerce.CustomerSite.Controllers
{
    public class ProductsController(ProductApiClient productApiClient) : Controller
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
            return View(product);
        }
    }
}