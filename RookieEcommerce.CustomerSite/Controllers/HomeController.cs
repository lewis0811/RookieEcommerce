using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Client.AspNetCore;
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
            var token = await HttpContext.GetTokenAsync(OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessToken);

            var categories = await categoryApiClient.GetCategoriesAsync(token);
            var product = await productApiClient.GetProductsAsync(sortOrder, minPrice, maxPrice, categoryId, pageNumber, token);
            var model = new HomeViewModel { Categories = categories, Products = product };

            return View(model);
        }
    }
}