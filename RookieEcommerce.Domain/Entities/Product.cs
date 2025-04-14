namespace RookieEcommerce.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public decimal Price { get; private set; }
        public string? Details { get; private set; } = string.Empty; // Store as json

        // Foreign Key
        public Guid? CategoryId { get; private set; }

        // Navigation Properties
        public virtual Category? Category { get; set; }

        public virtual ICollection<ProductImage> Images { get; set; } = [];
        public virtual ICollection<ProductVariant> Variants { get; set; } = [];

        public static Product Create(string name, string description, decimal price, Guid? categoryId, string? details)
        {
            return new Product { Name = name, Description = description, Price = price, Details = details, CategoryId = categoryId };
        }

        public void Update(string name, string description, decimal price)
        {
            if (name != Name) Name = name;
            if (description != Description) Description = description;
            if (price != Price) Price = price;
        }
    }
}