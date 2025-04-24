using RookieEcommerce.Domain.Entities;
using RookieEcommerce.Domain.Enums;
using RookieEcommerce.SharedViewModels.OrderItemDtos;

namespace RookieEcommerce.SharedViewModels.OrderDtos
{
    public class OrderDetailsDto : BaseEntity
    {
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }

        // Store Shipping Address components
        public Address ShippingAddress { get; set; } = new();

        public string? PaymentMethod { get; set; }
        public string? PaymentStatus { get; set; }
        public string? TransactionId { get; set; }
        public DateTime? PaymentDate { get; set; }

        // Foreign Key
        public Guid CustomerId { get; set; }

        public virtual ICollection<OrderItemDetailsDto>? OrderItems { get; set; }
    }
}