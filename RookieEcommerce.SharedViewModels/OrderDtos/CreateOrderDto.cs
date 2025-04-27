using RookieEcommerce.Domain.Entities;
using RookieEcommerce.Domain.Enums;

namespace RookieEcommerce.SharedViewModels.OrderDtos
{
    public class CreateOrderDto
    {
        public string Email { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public Guid CustomerId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string StreetAddress { get; set; } = string.Empty;
        public string Ward { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string CityProvince { get; set; } = string.Empty;
        public List<CreateOrderItemDto> OrderItems { get; set; } = [];
    }
}