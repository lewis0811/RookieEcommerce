namespace RookieEcommerce.Domain.Entities
{
    public class OrderItem : BaseEntity
    {
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        // Foreign Keys
        public Guid OrderId { get; set; }
        public Guid ProductVariantId { get; set; }

        // Navigation Properties
        public virtual Order Order { get; set; } = new();
        public virtual ProductVariant ProductVariant { get; set; } = new ProductVariant();
    }
}