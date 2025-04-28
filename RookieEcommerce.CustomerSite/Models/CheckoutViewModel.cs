using RookieEcommerce.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace RookieEcommerce.CustomerSite.Models
{
    public class CheckoutViewModel
    {
        public CartSummaryViewModel? Cart { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ Email")]
        [Display(Name = "Địa chỉ Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập họ tên người nhận")]
        [Display(Name = "Họ và Tên")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số Điện Thoại")]
        public string ShippingPhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập thành phố / tỉnh")]
        [Display(Name = "Thành phố / Tỉnh")]
        public string ShippingCity { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập quận / huyện / thị xã")]
        [Display(Name = "Quận / Huyện / Thị xã")]
        public string ShippingDistrict { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập phường / xã")]
        [Display(Name = "Phường / Xã")]
        public string ShippingWard { get; set; } = string.Empty;


        [Required(ErrorMessage = "Vui lòng nhập tên đường/ số nhà")]
        [Display(Name = "Tên đường/ số nhà")]
        public string ShippingStreetAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán")]
        [Display(Name = "Phương thức thanh toán")]
        public string PaymentMethod { get; set; } = string.Empty;
    }
}
