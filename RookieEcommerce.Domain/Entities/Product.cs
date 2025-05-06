namespace RookieEcommerce.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public decimal Price { get; private set; }
        public string? Details { get; private set; } = string.Empty; // Store as json
        public int TotalQuantity { get; private set; } = 0;
        public string Sku { get; set; } = string.Empty;

        // Foreign Key
        public Guid? CategoryId { get; private set; }

        // Navigation Properties
        public virtual Category? Category { get; set; }

        public virtual ICollection<ProductImage> Images { get; set; } = [];
        public virtual ICollection<ProductVariant> Variants { get; set; } = [];

        public static Product Create(string name, string description, decimal price, Guid? categoryId, string? details)
        {
            var generatedSku = "";
            var nameParts = name.Split(' ');
            var skuParts = nameParts.Select(part =>
                (part.Length >= 2 ? part[..2] : part).ToUpperInvariant()
            );

            generatedSku = string.Join("-", skuParts);

            return new Product
            {
                Name = name,
                Description = description,
                Price = price,
                Sku = generatedSku,
                Details = details,
                CategoryId = categoryId
            };
        }

        public void Update(string? name, string? description, decimal? price, string? detail, int? totalQuantity)
        {
            if (name != null && name != Name) Name = name;
            if (description != null && description != Description) Description = description;
            if (price != null && price != Price) Price = (decimal)price;
            if (detail != null && detail != Details) Details = detail;
            if (totalQuantity != null && totalQuantity != TotalQuantity) TotalQuantity = (int)totalQuantity;
            UpdateModifiedDate();
        }
    }
}