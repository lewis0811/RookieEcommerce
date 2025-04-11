namespace RookieEcommerce.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }

        // Foreign Key
        public Guid? CategoryId { get; set; }

        // Navigation Properties
        public virtual Category? Category { get; set; }
        public virtual ICollection<ProductImage> Images { get; set; } = [];
    }
}