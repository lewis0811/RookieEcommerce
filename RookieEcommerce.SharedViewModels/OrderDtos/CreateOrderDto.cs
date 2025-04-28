using RookieEcommerce.Domain.Entities;
using RookieEcommerce.Domain.Enums;

namespace RookieEcommerce.SharedViewModels.OrderDtos
{
    public class CreateOrderDto
    {
        public string Email { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string ShippingPhoneNumber { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public Guid CustomerId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public Address? ShippingAddress { get; set; }
        public List<CreateOrderItemDto>? OrderItems { get; set; }
    }
}