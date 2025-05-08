using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Client.AspNetCore;
using RookieEcommerce.CustomerSite.Models;
using RookieEcommerce.CustomerSite.Services;
using RookieEcommerce.Domain.Entities;
using RookieEcommerce.Domain.Enums;
using RookieEcommerce.SharedViewModels.CartDtos;
using RookieEcommerce.SharedViewModels.OrderDtos;
using RookieEcommerce.SharedViewModels.PaymentDtos;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace RookieEcommerce.CustomerSite.Controllers
{
    public class CheckoutController(CartApiClient cartApiClient,
        OrderApiClient orderApiClient,
        VnPayApiClient vnPayApiClient,
        VnPublicApiClient vnPublicApiClient) : Controller
    {
        public async Task<IActionResult> IndexAsync()
        {
            var token = await HttpContext.GetTokenAsync(OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessToken);
            if (token == null) { RedirectToAction("Login", "Authentication"); }

            CartDetailsDto? currentCart = null;
            var customerId = User.Claims.FirstOrDefault(c => c.Type == Claims.Subject)?.Value;
            if (customerId != null)
            {
                currentCart = await cartApiClient.GetCustomerCartAsync(Guid.Parse(customerId), token);
            }

            if (currentCart == null)
            {
                return RedirectToAction("Index", "Cart");
            }

            var model = new CheckoutViewModel
            {
                Cart = new CartSummaryViewModel
                {
                    TotalPrice = (decimal)currentCart.TotalPrice!,
                    Items = [.. currentCart.Items!.Select(item => new CartItemViewModel
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
            var token = await HttpContext.GetTokenAsync(OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessToken);
            if (token == null) { RedirectToAction("Login", "Authentication"); }

            CartDetailsDto? currentCart = null;
            var customerId = User.Claims.FirstOrDefault(c => c.Type == Claims.Subject)?.Value;
            if (customerId != null)
            {
                currentCart = await cartApiClient.GetCustomerCartAsync(Guid.Parse(customerId), token);
            }

            if (currentCart == null )
            {
                return RedirectToAction("Index", "Checkout");
            }

            var cartItems = currentCart.Items!.Select(item => new CreateOrderItemDto
            {
                ProductId = item.Product!.Id,
                ProductVariantId = item.ProductVariant?.Id ?? null,
                Quantity = item.Quantity
            }).ToList();

            CreateOrderDto dto = new()
            {
                Email = model.Email,
                CustomerName = model.Name,
                ShippingPhoneNumber = model.ShippingPhoneNumber,
                TotalAmount = (decimal)currentCart.TotalPrice!,
                CustomerId = Guid.Parse(customerId!),
                ShippingAddress = new Address
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
            var order = await orderApiClient.CreateOrderAsync(dto, token);

            // Redirect if using EWallet payment method
            if (dto.PaymentMethod == PaymentMethod.VNPay)
            {
                CreatePaymentDto paymentDto = new() { TotalAmount = (decimal)currentCart.TotalPrice, Description = $"{order.Id}", OrderId = order.Id };
                string vnPayUrl = await vnPayApiClient.CreatePaymentUrlAsync(paymentDto, token);

                return Redirect(vnPayUrl);
            }

            // Return order confirmation page
            TempData["SuccessMessage"] = $"Đặt hàng thành công!";
            return RedirectToAction("OrderConfirmation");
        }

        [HttpGet]
        public async Task<IActionResult> OrderConfirmation(string? orderId, string? transactionId)
        {
            var token = await HttpContext.GetTokenAsync(OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessToken);
            if (token == null) { RedirectToAction("Login", "Authentication"); }

            var successMessage = TempData["SuccessMessage"] as string;

            if (string.IsNullOrEmpty(successMessage) && orderId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (orderId != null && transactionId != null)
            {
                await orderApiClient.UpdateOrderAsync(orderId, transactionId, token);
            }

            ViewBag.SuccessMessage = successMessage;

            return View();
        }
    }
}