using RookieEcommerce.Domain.Enums;

namespace RookieEcommerce.Domain.Entities
{
    public class Order : BaseEntity
    {
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }

        // Store Shipping Address components
        public Address ShippingAddress { get; set; } = new();

        public string? PaymentMethod { get; set; }
        public string? PaymentStatus { get; set; }
        public string? TransactionId { get; set; } // ID from payment gateway
        public DateTime? PaymentDate { get; set; }
        
        // Foreign Key
        public Guid CustomerId { get; set; }

        // Navigation Properties
        public virtual Customer Customer { get; set; } = new Customer();
        public virtual ICollection<OrderItem> OrderItems { get; set; } = [];
    }
}