using RookieEcommerce.Domain.Entities;

namespace RookieEcommerce.SharedViewModels.OrderItemDtos
{
    public class OrderItemDetailsDto
    {
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        // Foreign Keys
        public Guid OrderId { get; set; }

        public Guid ProductId { get; set; }
        public Guid? ProductVariantId { get; set; }

        // Navigation Properties
        public virtual ProductVariant? ProductVariant { get; set; }

        public virtual Product? Product { get; set; }
    }
}