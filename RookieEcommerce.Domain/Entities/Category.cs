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

        public static Category Create(string name, string description, Guid? parentCategoryId)
        {
            return new Category { Name = name, Description = description, ParentCategoryId = parentCategoryId };
        }

        public void Update(string name, string description)
        {
            if (Name != name) { Name = name; }
            if (Description != description) { Description = description; }
        }
    }
}