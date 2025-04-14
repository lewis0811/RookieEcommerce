namespace RookieEcommerce.Domain.Entities
{
    // Using Hierachical Structure
    public class Category : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Foreign key
        public Guid? ParentCategoryId { get; set; }

        // Navigation Properties
        public virtual Category? ParentCategory { get; set; }

        public virtual ICollection<Category> SubCategories { get; set; } = [];

        // Relationship to Products
        public virtual ICollection<Product> Products { get; set; } = [];
    }
}