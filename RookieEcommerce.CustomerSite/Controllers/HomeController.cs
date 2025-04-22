using Microsoft.AspNetCore.Mvc;
using RookieEcommerce.CustomerSite.Models;
using RookieEcommerce.CustomerSite.Services;

namespace RookieEcommerce.CustomerSite.Controllers
{
    public class HomeController(CategoryApiClient categoryApiClient, ProductApiClient productApiClient) : Controller
    {
        public async Task<IActionResult> IndexAsync()
        {
            var categories = await categoryApiClient.GetCategoriesAsync();
            var product = await productApiClient.GetProductsAsync();
            var model = new HomeViewModel { Categories = categories, Products = product };

            return View(model);
        }
        // Model.Categories.Items
        // Model.Products.Items
        // item.Images
        public IActionResult Privacy()
        {
            return View();
        }
    }
}