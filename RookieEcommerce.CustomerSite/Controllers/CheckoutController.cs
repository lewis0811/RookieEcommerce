using Microsoft.AspNetCore.Mvc;
using RookieEcommerce.Application.Mappers;
using RookieEcommerce.CustomerSite.Models;
using RookieEcommerce.CustomerSite.Services;
using RookieEcommerce.Domain.Enums;
using RookieEcommerce.SharedViewModels.CartDtos;
using RookieEcommerce.SharedViewModels.OrderDtos;
using RookieEcommerce.SharedViewModels.PaymentDtos;
using RookieEcommerce.SharedViewModels.VnApiDtos;
using System.Threading.Tasks;

namespace RookieEcommerce.CustomerSite.Controllers
{
    public class CheckoutController(CartApiClient cartApiClient,
        OrderApiClient orderApiClient,
        VnPayApiClient vnPayApiClient,
        VnPublicApiClient vnPublicApiClient) : Controller
    {
        public async Task<IActionResult> IndexAsync()
        {
            var customerId = Guid.Parse("4C1E0C92-0BEA-A47A-6C8A-59E397F632F2"); // Change to get from cookie later
            var currentCart = await cartApiClient.GetCustomerCartAsync(customerId);
            // **** END MOCK DATA ****

            if (currentCart == null || currentCart.Items!.Count == 0)
            {
                TempData["ErrorMessage"] = "Giỏ hàng của bạn trống!";
                return RedirectToAction("Index", "Cart");
            }

            var model = new CheckoutViewModel
            {
                Cart = new CartSummaryViewModel
                {
                    TotalPrice = (decimal)currentCart.TotalPrice!,
                    Items = [.. currentCart.Items.Select(item => new CartItemViewModel
                    {
                        ProductName = item.Product?.Name,
                        ProductVariantName = item.ProductVariant?.Name,
                        Quantity = item.Quantity,
                        LineTotal = (decimal)item.LineTotal!,
                        ImageUrl = item.Product?.Images?.FirstOrDefault(img => img.IsPrimary)?.ImageUrl
                    })]
                }
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetVnProvinces()
        {
            var provinces = await vnPublicApiClient.GetProvincesAsync();
            return Json(provinces);
        }

        [HttpGet]
        public async Task<IActionResult> GetVnDistricts(string provinceCode)
        {
            var districts = await vnPublicApiClient.GetDistrictsAsync(provinceCode);
            if (districts != null)
            {
                var orderedDatas = districts.Data!.Data!.OrderBy(c => c.Name).ToList();
                districts.Data.Data = orderedDatas;
            }
            return Json(districts);
        }

        [HttpGet]
        public async Task<IActionResult> GetVnWards(string districtCode)
        {
            var wards = await vnPublicApiClient.GetWardsAsync(districtCode);
            return Json(wards);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
        {
            // Validate model
            var customerId = Guid.Parse("4C1E0C92-0BEA-A47A-6C8A-59E397F632F2");

            // Mock data
            var currentCart = await cartApiClient.GetCustomerCartAsync(customerId);
            if (currentCart == null || currentCart.Items!.Count == 0)
            {
                TempData["ErrorMessage"] = "Giỏ hàng của bạn trống!";
                return RedirectToAction("Index", "Cart");
            }

            // Validate model
            var cartItems = currentCart.Items.Select(item => new CreateOrderItemDto
            {
                ProductId = item.Product!.Id,
                ProductVariantId = item.ProductVariant!.Id,
                Quantity = item.Quantity
            }).ToList();

            // Create order
            CreateOrderDto dto = new ()
            {
                Email = model.Email,
                CustomerName = model.Name,
                ShippingPhoneNumber = model.ShippingPhoneNumber,
                TotalAmount = (decimal)currentCart.TotalPrice!,
                CustomerId = Guid.Parse("4C1E0C92-0BEA-A47A-6C8A-59E397F632F2"), // Change to get from cookie later
                ShippingAddress = new Domain.Entities.Address
                {
                    CityProvince = model.ShippingCity,
                    District = model.ShippingDistrict,
                    Ward = model.ShippingWard,
                    StreetAddress = model.ShippingStreetAddress,
                },
                PaymentMethod = (PaymentMethod)Enum.Parse(typeof(PaymentMethod), model.PaymentMethod),
                OrderItems = cartItems
            };

            // Call API to create order
            var order = await orderApiClient.CreateOrderAsync(dto);

            // Redirect if using EWallet payment method
            if (dto.PaymentMethod == PaymentMethod.VNPay)
            {
                CreatePaymentDto paymentDto = new() { TotalAmount = (decimal)currentCart.TotalPrice, Description = $"{order.Id}"};
                string vnPayUrl = await vnPayApiClient.CreatePaymentUrlAsync(paymentDto);

                return Redirect(vnPayUrl);
            }

            // Return order confirmation page
            TempData["SuccessMessage"] = $"Đặt hàng thành công!";
            return RedirectToAction("OrderConfirmation");
        }

        [HttpGet]
        public async Task<IActionResult> OrderConfirmation(string? orderId, string? transactionId)
        {
            var successMessage = TempData["SuccessMessage"] as string;

            // Nếu không có thông báo (ví dụ: người dùng truy cập trực tiếp URL)
            // thì chuyển hướng về trang chủ hoặc trang giỏ hàng.
            if (string.IsNullOrEmpty(successMessage) && orderId == null)
            {
                return RedirectToAction("Index", "Home");      
            }

            if (orderId != null && transactionId != null)
            {
                await orderApiClient.UpdateOrderAsync(orderId, transactionId);
            }

            ViewBag.SuccessMessage = successMessage;

            return View();
        }
    }
}
