using Microsoft.AspNetCore.Mvc;
using RookieEcommerce.CustomerSite.Models;
using RookieEcommerce.CustomerSite.Services;
using System.Threading.Tasks;

namespace RookieEcommerce.CustomerSite.Controllers
{
    public class CartController(CartApiClient cartApiClient) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var customerId = Guid.Parse("4C1E0C92-0BEA-A47A-6C8A-59E397F632F2"); // Change to get from cookie later
            var cart = await cartApiClient.GetCustomerCartAsync(customerId);

            return View(new CartViewModel { CartDetails = cart });
        }
    

        [HttpPost]
        public async Task<IActionResult> HandleCartItem(Guid? cartItemId)
        {
            var customerId = Guid.Parse("4C1E0C92-0BEA-A47A-6C8A-59E397F632F2"); // Change to get from cookie later
            var cart = await cartApiClient.GetCustomerCartAsync(customerId);
            if (cartItemId != null && cart != null)
            {
                var cartId = cart.Id;
                await cartApiClient.RemoveCartItemAsync(cartId, (Guid)cartItemId);
            }

            return RedirectToAction("Index");
        }
    }
}
