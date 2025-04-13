using RookieEcommerce.Domain.Enums;

namespace RookieEcommerce.Domain.Entities
{
    public class ProductVariant : BaseEntity
    {
        private readonly int minStock = 0;
        private int stockQuantity;

        public PVariantType VariantType { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;

        public int StockQuantity
        {
            get { return stockQuantity; }
            set
            {
                stockQuantity = value < minStock ? minStock : value;
            }
        }

        public decimal Price { get; set; }
        
        // Foreign key
        public Guid ProductId { get; set; }

        // Navigation property
        public Product Product { get; set; } = new Product();
    }
}