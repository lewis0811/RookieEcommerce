namespace RookieEcommerce.Domain.Entities
{
    public class ProductVariant : BaseEntity
    {
        private readonly int minStock = 0;
        private int stockQuantity;

        public string VariantType { get; set; } = string.Empty;
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

        // Methods
        public static ProductVariant Create(Guid productId, string name, decimal price, int stockQuantity, string variantType)
        {
            var generatedSku = "";
            var nameParts = name.Split(' ');
            var skuParts = nameParts.Select(part =>
                (part.Length >= 2 ? part[..2] : part).ToUpperInvariant()
            );

            generatedSku = string.Join("-", skuParts);

            return new ProductVariant
            {
                ProductId = productId,
                Name = name,
                Sku = generatedSku,
                Price = price,
                StockQuantity = stockQuantity,
                VariantType = variantType
            };
        }

        public void Update(Guid id, string? name, decimal? price, int? stockQuantity)
        {
            if (name != null && name != Name) { Name = name; }
            if (price != null && price != Price) { Price = (decimal)price; }
            if (stockQuantity != null && stockQuantity != StockQuantity) { StockQuantity = (int)stockQuantity; }
        }
    }
}