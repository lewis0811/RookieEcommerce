using Microsoft.AspNetCore.Mvc;
using RookieEcommerce.CustomerSite.Models;
using RookieEcommerce.CustomerSite.Services;

namespace RookieEcommerce.CustomerSite.Controllers
{
    public class HomeController(CategoryApiClient categoryApiClient, ProductApiClient productApiClient) : Controller
    {
        public async Task<IActionResult> IndexAsync(string? sortOrder,
            double? minPrice, // Thêm minPrice
            double? maxPrice,
            Guid? categoryId,
            int? pageNumber) // Thêm maxPrice
        {
            //ViewData["CurrentSort"] = sortOrder;
            //ViewData["MinPrice"] = minPrice; // Lưu minPrice
            //ViewData["MaxPrice"] = maxPrice; // Lưu maxPrice

            var categories = await categoryApiClient.GetCategoriesAsync();
            var product = await productApiClient.GetProductsAsync(sortOrder, minPrice, maxPrice, categoryId, pageNumber);
            var model = new HomeViewModel { Categories = categories, Products = product };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}