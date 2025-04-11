namespace RookieEcommerce.Domain.Entities
{
    public class ProductImage : BaseEntity
    {
        public string ImageUrl { get; set; } = string.Empty;
        public string? AltText { get; set; } 
        public int SortOrder { get; set; } = 0; 
        public bool IsPrimary { get; set; } = false; 

        // Foreign Key
        public Guid ProductId { get; set; }

        // Navigation Property
        public virtual Product Product { get; set; } = null!;
    }
}